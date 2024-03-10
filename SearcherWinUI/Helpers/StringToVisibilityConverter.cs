﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace SearcherWinUI.Helpers;

public class StringToVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, string language) =>
		string.IsNullOrEmpty(value as string) ? Visibility.Collapsed : Visibility.Visible;

	public object ConvertBack(object value, Type targetType, object parameter, string language)
	{
		throw new NotImplementedException();
	}
}
