using System.Windows;
using System.Windows.Controls;

namespace SearcherWpf;

public class WatermarkedTextBox : TextBox
{
    public static readonly DependencyProperty WatermarkTextProperty =
        DependencyProperty.Register(nameof(WatermarkText), typeof(string),
            typeof(WatermarkedTextBox), new PropertyMetadata(default(string)));

    public string WatermarkText
    {
        get => (string)GetValue(WatermarkTextProperty);
        set => SetValue(WatermarkTextProperty, value);
    }

    static WatermarkedTextBox()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(WatermarkedTextBox),
            new FrameworkPropertyMetadata(typeof(WatermarkedTextBox)));
    }
}
