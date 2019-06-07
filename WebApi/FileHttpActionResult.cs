/// <summary>
/// class which implements IHttpActionResult can be returned as Ok() response.
/// </summary>
public class FileHttpActionResult : IHttpActionResult 
{
    private readonly string _filePath;
    private readonly string _contentType;
    public FileHttpActionResult(string filePath, string contentType = null) {
        _filePath = filePath;
        _contentType = contentType;
    }

    public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken) {
        return Task.Run(() => {
            var response = new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StreamContent(new FileStream(_filePath, FileMode.Open, FileAccess.Read))
            };

            var contentType = _contentType ?? MimeMapping.GetMimeMapping(Path.GetExtension(_filePath));
            response.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = Path.GetFileName(_filePath)
            };

            return response;
        }, cancellationToken);
    }

}
