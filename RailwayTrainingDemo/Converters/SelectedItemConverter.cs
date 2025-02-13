using System.Globalization;

namespace RailwayTrainingDemo.Converters;

public class SelectedItemConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value
            ? Color.FromArgb("#CCD5AE")
            : Color.FromArgb("#D4A373");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
