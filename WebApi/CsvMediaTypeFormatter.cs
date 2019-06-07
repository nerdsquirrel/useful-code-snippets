public class CsvMediaTypeFormatter : MediaTypeFormatter
{
    public CsvMediaTypeFormatter()
    {
        this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));
        
        //SupportedEncodings.Add(new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        //SupportedEncodings.Add(Encoding.GetEncoding("iso-8859-1"));
    }

    public CsvMediaTypeFormatter(MediaTypeMapping mediaTypeMapping) : this()
    {
        MediaTypeMappings.Add(mediaTypeMapping);
    }
    
    public CsvMediaTypeFormatter(IEnumerable<MediaTypeMapping> mediaTypeMappings) : this()
    {
        foreach (var mediaTypeMapping in mediaTypeMappings)
        {
            MediaTypeMappings.Add(mediaTypeMapping);
        }
    }
    
    public override bool CanWriteType(Type type)
    {
        if (type == null)
            throw new ArgumentNullException("type");

        return isTypeOfIEnumerable(type);
    }

    private bool isTypeOfIEnumerable(Type type)
    {
        foreach (Type interfaceType in type.GetInterfaces())
        {
            if (interfaceType == typeof(IEnumerable))
                return true;
        }

        return false;
    }

    public override bool CanReadType(Type type)
    {
        return false;
    }

    public override void SetDefaultContentHeaders
    (Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
    {
        base.SetDefaultContentHeaders(type, headers, mediaType);
        headers.Add("Content-Disposition", DateTime.Now.ToString("yyyyMMddhhmmssfff")+ ".csv");
    }
    
    public override Task WriteToStreamAsync(Type type, object value, 
    Stream stream, HttpContent content, TransportContext transportContext)
    {           
        writeStream(type, value, stream, content);
        var tcs = new TaskCompletionSource<int>();
        tcs.SetResult(0);
        return tcs.Task;
    }

    private void writeStream(Type type, object value, Stream stream, HttpContent contentHeaders)
    {            
        //NOTE: We have check the type inside CanWriteType method
        //If request comes this far, the type is IEnumerable. We are safe.
        
        Type itemType = type.GetGenericArguments()[0];

        StringWriter _stringWriter = new StringWriter();

        _stringWriter.WriteLine(
            string.Join<string>(
                ",", itemType.GetProperties().Select(x => x.Name)
            )
        );

        foreach (var obj in (IEnumerable<object>)value)
        {

            var vals = obj.GetType().GetProperties().Select(
                pi => new {
                    Value = pi.GetValue(obj, null)
                }
            );

            string _valueLine = string.Empty;

            foreach (var val in vals)
            {
                if (val.Value != null)
                {
                    var _val = val.Value.ToString();

                    //Check if the value contains a comma and place it in quotes if so
                    if (_val.Contains(","))
                        _val = string.Concat("\"", _val, "\"");

                    //Replace any \r or \n special characters from a new line with a space
                    if (_val.Contains("\r"))
                        _val = _val.Replace("\r", " ");
                    if (_val.Contains("\n"))
                        _val = _val.Replace("\n", " ");

                    _valueLine = string.Concat(_valueLine, _val, ",");
                }
                else
                {
                    _valueLine = string.Concat(string.Empty, ",");
                }
            }

            _stringWriter.WriteLine(_valueLine.TrimEnd(','));
        }

        var streamWriter = new StreamWriter(stream);            
        streamWriter.Write(_stringWriter.ToString());
        streamWriter.Flush();
        streamWriter.Close();
    }        
}