using System;
using Microsoft.UI.Xaml.Data;

namespace SearcherWpf.Converters;

public class FontSizeConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter,
		string language) => (int)(Math.Round((double)value * Double.Parse((string)parameter)));

	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}