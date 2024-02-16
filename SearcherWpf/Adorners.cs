using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace SearcherWpf;

public class WatermarkAdorner : Adorner
{
    private readonly VisualCollection _visuals;
    private readonly TextBlock _watermarkTextBlock;

    public WatermarkAdorner(UIElement adornedElement, string watermarkText) :
        base(adornedElement)
    {
        // Watermark should not be interacted with.
        IsHitTestVisible = false;

        _visuals = new VisualCollection(this);
        _watermarkTextBlock = new TextBlock
        {
            Text = watermarkText,
            Foreground = SystemColors.GrayTextBrush,
        };

        if (adornedElement is TextBox textBox)
        {
            _watermarkTextBlock.Margin = new Thickness(3,1,1,1);
            _watermarkTextBlock.FontFamily = textBox.FontFamily;
            _watermarkTextBlock.FontSize = textBox.FontSize;
        }

        _visuals.Add(_watermarkTextBlock);
    }

    protected override int VisualChildrenCount =>
        _visuals != null ? _visuals.Count : 0;

    protected override Visual GetVisualChild(int index) => _visuals[index];

    protected override Size MeasureOverride(Size constraint)
    {
        _watermarkTextBlock.Measure(constraint);
        return _watermarkTextBlock.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        _watermarkTextBlock.Arrange(new Rect(finalSize));
        return finalSize;
    }
}
