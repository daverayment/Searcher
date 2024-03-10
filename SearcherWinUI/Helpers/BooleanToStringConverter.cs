using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace SearcherWinUI.Helpers;

/// <summary>
/// Converts a boolean to one of the passed-in styles. True returns the
/// first style, False the second.
/// </summary>
public class BooleanToStyleConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter,
		string language)
	{
		if (value is bool isFirst && parameter is string styles)
		{
			string styleKey = styles.Split(',')[isFirst ? 0 : 1];
			return Application.Current.Resources[styleKey.Trim()] as Style;
		}

		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, string language)
	{
		throw new NotImplementedException();
	}
}
