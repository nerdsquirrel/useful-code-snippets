public class Base64StringToImageProperty
{
    public static string GetBase64ImageString(DependencyObject obj)
    {
        return (string)obj.GetValue(Base64ImageStringProperty);
    }

    public static void SetBase64ImageString(DependencyObject obj, string value)
    {
        obj.SetValue(Base64ImageStringProperty, value);
    }

    public static readonly DependencyProperty Base64ImageStringProperty =
        DependencyProperty.RegisterAttached("Base64ImageString", typeof(string), typeof(Base64StringToImageProperty), new PropertyMetadata(null, Base64ImageStringPropertyChanged));

    private static void Base64ImageStringPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        string base64String = (string)e.NewValue;
        Image src = d as Image;

        if (!string.IsNullOrWhiteSpace(base64String))
        {
            byte[] binaryData = Convert.FromBase64String(base64String);

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.StreamSource = new MemoryStream(binaryData);
            bi.EndInit();

            src.Source = bi;
        }
        else
        {
            src.Source = null;
        }
    }
}