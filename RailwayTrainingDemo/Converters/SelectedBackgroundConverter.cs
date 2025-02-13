using System.Globalization;

namespace RailwayTrainingDemo.Converters;

public class SelectedBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value 
            ? Color.FromArgb("#E9EDC9") 
            : Colors.White;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
} 