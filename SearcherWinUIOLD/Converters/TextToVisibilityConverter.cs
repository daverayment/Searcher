using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace SearcherWpf.Converters;

public class TextToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) =>
        string.IsNullOrEmpty(value as string) ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
