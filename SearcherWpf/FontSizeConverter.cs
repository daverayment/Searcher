using System.Globalization;
using System.Windows.Data;

namespace SearcherWpf.Converters;

public class FontSizeConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter,
		CultureInfo culture) => (int)(Math.Round((double)value * Double.Parse((string)parameter)));

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}