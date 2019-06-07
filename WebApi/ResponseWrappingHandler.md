### What is the purpose of this wrapper hanlder?
It wraps all the response of web api. It's an elegant way to format api response.

```csharp
public class ResponseWrappingHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        return BuildApiResponse(request, response);
    }

    private HttpResponseMessage BuildApiResponse(HttpRequestMessage request, HttpResponseMessage response)
    {
        List<string> modelStateErrors = new List<string>();

        object content;

        if (response.TryGetContentValue(out content) && !response.IsSuccessStatusCode)
        {
            HttpError error = content as HttpError;
            if (error != null)
            {
                content = null;

                if (error.ModelState != null)
                {
                    var httpErrorObject = response.Content.ReadAsStringAsync().Result;

                    var anonymousErrorObject = new { message = "", ModelState = new Dictionary<string, string[]>() };

                    var deserializedErrorObject = JsonConvert.DeserializeAnonymousType(httpErrorObject, anonymousErrorObject);

                    var modelStateValues = deserializedErrorObject.ModelState.Select(kvp => string.Join(". ", kvp.Value)).ToList();

                    for (int i = 0; i < modelStateValues.Count; i++)
                    {
                        modelStateErrors.Add(modelStateValues.ElementAt(i));
                    }
                }
                else
                {
                    return response;
                }
            }
            
        }
        HttpResponseMessage newResponse;

        if (modelStateErrors.Count > 0)
        {
            newResponse = request.CreateResponse(response.StatusCode, new ValidationResponse(content, modelStateErrors));
        }
        else
        {
            return response;
        }            

        foreach (var header in response.Headers)
        {
            newResponse.Headers.Add(header.Key, header.Value);
        }

        return newResponse;
    }
}
```

### How to use?
```csharp
public static class WebApiConfig
{
    public static void Register(HttpConfiguration config)
    {
        // ...
        config.MessageHandlers.Add(new ResponseWrappingHandler());
        // ...
    }

}
```