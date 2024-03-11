using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace SearcherWinUI.Helpers
{
	public class TextMeasurement
	{
		private static TextMeasurement _instance;
		private static readonly object _lock = new();

		private static readonly CanvasDevice _device = CanvasDevice.GetSharedDevice();

		public CanvasTextFormat TextFormat { get; private set; }

		private TextMeasurement() { }

		public static TextMeasurement Instance(TextBlock textBlock)
		{
			if (_instance == null)
			{
				lock (_lock)
				{
					if (_instance == null)
					{
						_instance = new TextMeasurement();
						_instance.TextFormat = new CanvasTextFormat
						{
							FontSize = (float)textBlock.FontSize,
							FontFamily = textBlock.FontFamily.Source,
							FontWeight = textBlock.FontWeight,
							FontStyle = textBlock.FontStyle,
							FontStretch = textBlock.FontStretch
						};
					}
				}
			}

			return _instance;
		}

		public static Size MeasureText(string text)
		{
			if (_instance == null)
			{
				return Size.Empty;
			}

			using var layout = new CanvasTextLayout(_device, text, _instance.TextFormat,
				 float.MaxValue, float.MaxValue);

			return new Size(layout.LayoutBounds.Width, layout.LayoutBounds.Height);
		}
	}
}
