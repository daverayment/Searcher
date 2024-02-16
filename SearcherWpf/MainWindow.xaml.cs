using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SearcherWpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

            FilenameFilter.GotFocus += (sender, e) => FilenameFilterWatermark.Visibility = Visibility.Collapsed;
            FilenameFilter.LostFocus += (sender, e) => SetWatermarkVisibility(FilenameFilter, FilenameFilterWatermark);

            SearchPrompt.GotFocus += (_, _) => SearchPromptWatermark.Visibility = Visibility.Collapsed;
            SearchPrompt.LostFocus += (sender, e) => SetWatermarkVisibility(SearchPrompt, SearchPromptWatermark);
        }

        private static void SetWatermarkVisibility(TextBox control, Label watermark)
        {
            if (control != null && watermark != null)
            {
                watermark.Visibility = string.IsNullOrEmpty(control.Text) ?
                    Visibility.Visible : Visibility.Collapsed;
            }            
        }

        public static void AddWatermark(object element, string watermarkText)
        {
            if (element is TextBox textBox)
            {
                RemoveWatermark(element); // Ensure no existing watermark adorner

                if (textBox.Text.Length == 0)
                {
                    var adornerLayer = AdornerLayer.GetAdornerLayer(textBox);
                    if (adornerLayer != null)
                    {
                        var watermarkAdorner = new WatermarkAdorner(textBox, watermarkText);
                        adornerLayer.Add(watermarkAdorner);
                    }
                }
            }
        }

        public static void RemoveWatermark(object element)
        {
            if (element is TextBox textBox)
            {
                var adornerLayer = AdornerLayer.GetAdornerLayer(textBox);
                if (adornerLayer != null)
                {
                    var adorners = adornerLayer.GetAdorners(textBox);
                    if (adorners != null)
                    {
                        var watermarkAdorner = adorners.OfType<WatermarkAdorner>().FirstOrDefault();
                        if (watermarkAdorner != null)
                        {
                            adornerLayer.Remove(watermarkAdorner);
                        }
                    }
                }
            }
        }
    }
}