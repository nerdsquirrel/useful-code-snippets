public sealed class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value is bool b && b) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Visibility visibility && visibility == Visibility.Visible;
    }
}

public sealed class CountToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value is int i && i > 0) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}

public class StringToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string colorCode = value as string;
        if (string.IsNullOrWhiteSpace(colorCode))
        {
            colorCode = "#FFFFFF"; // default color
        }
        var brush = (new BrushConverter().ConvertFromString(colorCode) as Brush);
        return brush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}

public class EnumToDescriptionConverter : IValueConverter
{
    private string GetEnumDescription(Enum enumObj)
    {
        if (enumObj == null)
            return null;

        FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

        object[] attributeArray = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

        if (attributeArray.Length == 0)
        {
            return enumObj.ToString();
        }

        DescriptionAttribute attribute = attributeArray[0] as DescriptionAttribute;
        return attribute?.Description;
    }

    object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Enum myEnum = (Enum)value;
        string description = GetEnumDescription(myEnum);
        return description;
    }

    object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return string.Empty;
    }
}

public class DictionaryKeyToValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || !(parameter is IDictionary dictionary) || !dictionary.Contains(value))
            return null;

        return dictionary[value];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || !(parameter is IDictionary dictionary))
            return null;

        return dictionary
            .Cast<DictionaryEntry>()
            .FirstOrDefault(entry => entry.Value == value)
            .Key;
    }
}