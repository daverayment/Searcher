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
	}

	private async void ResultsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (ResultsListView.SelectedItem is FileInfo selectedFile)
		{
			try
			{
				FileContents.Document.SetText(TextSetOptions.ApplyRtfDocumentDefaults, string.Empty);
				string fileContents =
					await File.ReadAllTextAsync(selectedFile.FullName);

				// Attempt to reset prior selections.
				//FileContents.Document.Selection.SetRange(0, TextConstants.MaxUnitCount);
				//FileContents.Document.Selection.CharacterFormat.BackgroundColor =
				//	Microsoft.UI.Colors.Transparent;
				//FileContents.Document.Selection.SetRange(0, 0);

				FileContents.Document.SetText(TextSetOptions.None, fileContents);
				HighlightMatches();
			}
			catch (Exception ex)
			{
				// TODO:
			}
		}
	}

	private void HighlightMatches()
	{
		string fileContents;
		FileContents.Document.GetText(TextGetOptions.UseObjectText, out fileContents);
		string searchString = ViewModel.SearchString;

		bool found = false;
		int index = fileContents.IndexOf(searchString, 0);

		while (index >= 0)
		{
			// Highlight the found text.
			// NB: setting a range from 0 breaks the control!
			FileContents.Document.Selection.SetRange(index == 0 ? 1 : index, index + searchString.Length);
			FileContents.Document.Selection.CharacterFormat.BackgroundColor =
				Windows.UI.Color.FromArgb(255, 128, 128, 0);

			if (!found)
			{
				// Scroll to the first instance of the found text.
				FileContents.Document.Selection.ScrollIntoView(PointOptions.None);
				found = true;
			}

			index = fileContents.IndexOf(searchString, index + searchString.Length);
		}
	}
}
