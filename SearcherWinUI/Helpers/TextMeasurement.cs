using System.Collections.Concurrent;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace SearcherWinUI.Helpers
{
	public class TextMeasurement
	{
		public CanvasTextFormat TextFormat { get; private set; }
		private static readonly CanvasDevice _device = CanvasDevice.GetSharedDevice();

		internal TextMeasurement(TextBlock textBlock)
		{
			TextFormat = new CanvasTextFormat
			{
				FontSize = (float)textBlock.FontSize,
				FontFamily = textBlock.FontFamily.Source,
				FontWeight = textBlock.FontWeight,
				FontStyle = textBlock.FontStyle,
				FontStretch = textBlock.FontStretch
			};
		}

		public double MeasureTextWidth(string text)
		{
			using var layout = new CanvasTextLayout(_device, text, TextFormat,
				float.MaxValue, float.MaxValue);
			return layout.LayoutBounds.Width;
		}

		public Size MeasureText(string text)
		{
			using var layout = new CanvasTextLayout(_device, text, TextFormat,
				 float.MaxValue, float.MaxValue);
			return new Size(layout.LayoutBounds.Width, layout.LayoutBounds.Height);
		}
	}

	public class TextMeasurementFactory
	{
		private static readonly ConcurrentDictionary<string, TextMeasurement> _cache = new();

		public static TextMeasurement Create(TextBlock textBlock)
		{
			string key = $"{textBlock.FontSize}-{textBlock.FontFamily.Source}-{textBlock.FontWeight.Weight}-{textBlock.FontStyle}-{textBlock.FontStretch}";
			return _cache.GetOrAdd(key, _ => new TextMeasurement(textBlock));
		}
	}
}
