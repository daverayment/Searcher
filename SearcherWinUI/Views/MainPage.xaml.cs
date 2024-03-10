using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using SearcherWinUI.ViewModels;

namespace SearcherWinUI.Views;

public sealed partial class MainPage : Page
{
	public MainViewModel ViewModel
	{
		get;
	}

	public MainPage()
	{
		ViewModel = App.GetService<MainViewModel>();
		InitializeComponent();

		ViewModel.DocumentReady += DocumentReady;
	}

	private void DocumentReady(object? sender, EventArgs e)
	{
		FileContents.Document.SetText(TextSetOptions.None, ViewModel.FileContents);

		bool firstMatch = true;
		foreach (int index in ViewModel.Highlights)
		{
			// Highlight the found text.
			// NB: setting a range from 0 breaks the control!
			FileContents.Document.Selection.SetRange(
				index == 0 ? 0 : index, index + ViewModel.SearchString.Length);
			FileContents.Document.Selection.CharacterFormat.BackgroundColor =
				Windows.UI.Color.FromArgb(255, 128, 128, 0);

			if (!firstMatch)
			{
				// Scroll to the first instance of the found text.
				FileContents.Document.Selection.ScrollIntoView(PointOptions.None);
				firstMatch = false;
			}
		}
	}
}
