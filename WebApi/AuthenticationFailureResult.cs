/// <summary>
/// class which implements IHttpActionResult can be returned as Ok() response.
/// </summary>
public class AuthenticationFailureResult : IHttpActionResult
{
    public AuthenticationFailureResult(string reasonPhrase, HttpRequestMessage request)
    {
        ReasonPhrase = reasonPhrase;
        Request = request;
    }

    public string ReasonPhrase { get; }

    public HttpRequestMessage Request { get; }

    public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(Execute());
    }

    private HttpResponseMessage Execute()
    {
        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized)
        {
            RequestMessage = Request, ReasonPhrase = ReasonPhrase
        };
        return response;
    }
}