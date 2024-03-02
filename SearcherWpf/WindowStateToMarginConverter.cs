using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SearcherWpf.Converters;

/// <summary>
/// Convert between two thickness values based on whether the Window is
/// Maximised or not. Allows one to remove borders and margins to align
/// scrollbars with the right side of the screen to follow Fitt's Law.
/// </summary>
public class WindowStateThicknessConverter : IValueConverter
{
    /// <summary>
    /// Convert between a <see cref="WindowState"/> and a
    /// <see cref="Thickness"/>.
    /// </summary>
    /// <param name="value">The <see cref="WindowState"/>.</param>
    /// <param name="targetType">Unused.</param>
    /// <param name="parameter">A string containing two semicolon-separated
    /// values, each containing 4 comma-separated thickness numbers. The first
    /// set of numbers represents the default non-maximised thickness values,
    /// and the second set represents the maximised values. For example:
    /// "10,10,10,10;10,10,0,10" would set the right thickness to 0 when the
    /// window is maximised.</param>
    /// <param name="culture">Unused.</param>
    /// <returns>The <see cref="Thickness"/> corresponding to the current
    /// window state.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var thickness = new Thickness(10);
        if (value is WindowState state && parameter is string paramStr)
        {
            var parts = paramStr.Split(';');
            if (parts.Length == 2)
            {
                thickness = state == WindowState.Maximized ?
                    StringToThickness(parts[1]) :
                    StringToThickness(parts[0]);
            }
        }

        return thickness;
    }

    private static Thickness StringToThickness(string str)
    {
        var values = str.Split(',');
        return values.Length == 4 ?
            new Thickness(
                double.Parse(values[0]),
                double.Parse(values[1]),
                double.Parse(values[2]),
                double.Parse(values[3])) :
            new Thickness();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
