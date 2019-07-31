### Dependency
Image resizing has been done with a third party library named Image processor.

### References
https://imageprocessor.org/imageprocessor/imagefactory/
https://www.hanselman.com/blog/NuGetPackageOfTheWeekImageProcessorLightweightImageManipulationInC.aspx

```csharp
private static Image GetResizedImage(string imagepath)
{
    try
    {
        byte[] photoBytes = File.ReadAllBytes(imagepath);
        // Format is automatically detected though can be changed.
        ISupportedImageFormat format = new PngFormat { Quality = 100 };
        Size size = new Size(48, 48);
        using (MemoryStream inStream = new MemoryStream(photoBytes))
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                {
                    // Load, resize, set the format and quality and save an image.
                    imageFactory.Load(inStream)
                        .Resize(size)
                        .Format(format)
                        .Save(outStream);

                    return Image.FromStream(outStream);
                }

            }
        }
    }
    catch (Exception)
    {
        return null;
    }
}
```

### usage
```csharp
var resizedImage = GetResizedImage(fileInfo.FullName);
resizedImage?.Save($"{savingFilePath}//{fileInfo.Name}", ImageFormat.Png);
resizedImage?.Dispose();
```