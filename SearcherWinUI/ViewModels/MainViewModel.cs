using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.Windows.ApplicationModel.Resources;
using Windows.Storage.Pickers;
using Windows.Storage;
using Microsoft.UI.Xaml;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Runtime.Intrinsics.Arm;
using SearcherWinUI.Helpers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;

namespace SearcherWinUI.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
	[ObservableProperty]
	private string _startFolder = "";

	[ObservableProperty]
	private string _searchString = "";

	[ObservableProperty]
	private string _filenameFilter = "";

	[ObservableProperty]
	private string _searchButtonContent = "";

	[ObservableProperty]
	private bool _isSearchButtonEnabled = true;

	private FileInfo _selectedResult;

	[ObservableProperty]
	private string _statusMessage = "";

	[ObservableProperty]
	private bool _hasError = false;

	/// <summary>
	/// The contents of the loaded file.
	/// </summary>
	[ObservableProperty]
	private string _fileContents;

	/// <summary>
	/// Start indices of the found matches in the loaded document.
	/// </summary>
	[ObservableProperty]
	private List<int> _highlights = new();

	/// <summary>
	/// Set when the user selects an entry from the list of results.
	/// </summary>
	public FileInfo SelectedResult
	{
		get => _selectedResult;
		set
		{
			if (SetProperty(ref _selectedResult, value))
			{
				OnResultSelected();
			}
		}
	}

	[ObservableProperty]
	private ObservableCollection<FileInfo> _searchResults = new();

	/// <summary>
	/// Delay in milliseconds between a click of the Search button and it
	/// becoming available to press again as a Cancel button.
	/// </summary>
	private const int SearchTransitionDelayInMs = 200;

	private CancellationTokenSource? _cts;

	public event EventHandler<EventArgs> DirectoryNotFoundError = delegate { };

	public event EventHandler<EventArgs> BlankSearchStringError = delegate { };

	public event EventHandler<EventArgs> DocumentReady = delegate { };

	public MainViewModel()
	{
		LoadSettings();

		SearchButtonContent = "Main_SearchButton_Search/Text".GetLocalized();
		StatusMessage = "Main_ResultsPreSearchMessage/Text".GetLocalized();
	}

	private void LoadSettings()
	{
		var localSettings = ApplicationData.Current.LocalSettings;
		StartFolder = (string)(localSettings.Values["StartFolder"] ?? "C:\\");
		FilenameFilter = (string)(localSettings.Values["FilenameFilter"] ?? "");
		SearchString = (string)(localSettings.Values["SearchString"] ?? "");
	}

	/// <summary>
	/// Allow the user to browse for a start folder.
	/// </summary>
	[RelayCommand]
	private async void BrowseForStartFolder()
	{
		var folderPicker = new FolderPicker
		{
			SuggestedStartLocation = PickerLocationId.Desktop
		};
		folderPicker.FileTypeFilter.Add("*");

		nint hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
		WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

		var folder = await folderPicker.PickSingleFolderAsync();

		if (folder != null)
		{
			if (Directory.Exists(folder.Path))
			{
				StartFolder = folder.Path;
			}
			else
			{
				OnDirectoryNotFound();
			}
		}
	}

	[RelayCommand]
	private async void ExecuteSearchOrCancel()
	{
		if (IsCancelButtonShown())
		{
			// Cancel a running search operation.
			_cts?.Cancel();
			return;
		}

		_cts = new CancellationTokenSource();

		// Otherwise begin the search by changing the Search button to Cancel.
		TransitionSearchButton();

		// Clear any error messages.
		HasError = false;

		try
		{
			StatusMessage = "";
			SearchResults.Clear();

			await foreach (var result in SearchFiles())
			{
				SearchResults.Add(result);
			}
		}
		catch (BlankSearchStringException)
		{
			StatusMessage = "Main_ResultsBlankSearch/Text".GetLocalized();
			HasError = true;
		}
		catch (DirectoryNotFoundException)
		{
			StatusMessage = "Main_ResultsDirectoryNotFound/Text".GetLocalized();
			HasError = true;
		}
		catch (OperationCanceledException)
		{
			// TODO: report cancelled search.
		}
		finally
		{
			_cts.Dispose();
			var loader = new ResourceLoader();
			SearchButtonContent = loader.GetString("Main_SearchButton_Search/Text");
			IsSearchButtonEnabled = true;

			if (SearchResults.Count == 0)
			{
				StatusMessage = "Main_ResultsNoMatchesMessage/Text".GetLocalized();
			}
		}
	}

	private async IAsyncEnumerable<FileInfo> SearchFiles()
	{
		if (StartFolder == null || StartFolder.Trim().Length == 0 ||
			!Directory.Exists(StartFolder.Trim()))
		{
			throw new DirectoryNotFoundException();
		}

		if (SearchString.Trim().Length == 0)
		{
			throw new BlankSearchStringException();
		}

		if (_cts == null)
		{
			// Should never happen.
			yield break;
		}

		SaveSettings();

		// TODO: clear RichTextBox
		// TODO: ignore non-text files!
		// TODO: set default regex if no value has been provided
		var pattern = CreateFilenameRegex();
		var startDirInfo = new DirectoryInfo(StartFolder.Trim());

		foreach (var file in startDirInfo.EnumerateFiles("*",
			new EnumerationOptions
		{
			IgnoreInaccessible = true,
			RecurseSubdirectories = true,
			AttributesToSkip = System.IO.FileAttributes.ReparsePoint
		}))
		{
			_cts.Token.ThrowIfCancellationRequested();

			if (pattern.IsMatch(file.Name))
			{
				// TODO: status update
				string contents = await File.ReadAllTextAsync(file.FullName,
					_cts.Token);

				// TODO: type-specific checks, e.g. rtf/doc files
				if (contents.Contains(SearchString.Trim(),
					StringComparison.OrdinalIgnoreCase))
				{
					yield return file;
				}
			}
		}
	}

	/// <summary>
	/// Transition the Search button to a Cancel button after a brief delay.
	/// </summary>
	private async void TransitionSearchButton()
	{
		IsSearchButtonEnabled = false;
		await Task.Delay(SearchTransitionDelayInMs);
		if (_cts != null && !_cts.IsCancellationRequested)
		{
			var loader = new ResourceLoader();
			SearchButtonContent = loader.GetString("Main_SearchButton_Cancel/Text");
			IsSearchButtonEnabled = true;
		}
	}

	private bool IsCancelButtonShown()
	{
		var resourceLoader = new ResourceLoader();
		return SearchButtonContent ==
			resourceLoader.GetString("Main_SearchButton_Cancel/Text");
	}

	/// <summary>
	/// Called in response to a new result selection. Loads the selected
	/// file and searches for the requested string, updating the highlight
	/// indices list.
	/// </summary>
	public void OnResultSelected()
	{
		if (SelectedResult is FileInfo file)
		{
			try
			{
				// NB: the RichEditBox will normalise endings to \r so we need
				// this bit of preprocessing to ensure the search reports the
				// correct character indices.
				FileContents =
					File.ReadAllText(file.FullName).ReplaceLineEndings("\r");

				// Clear the list of matches and then search the file.
				Highlights.Clear();

				int index = FileContents.IndexOf(SearchString, 0,
					StringComparison.OrdinalIgnoreCase);
				while (index != -1)
				{
					Highlights.Add(index);
					index = FileContents.IndexOf(SearchString,
						index + SearchString.Length,
						StringComparison.OrdinalIgnoreCase);
				}

				OnDocumentReady();
			}
			catch
			{
				// TODO: report exceptions
			}
		}
	}

	/// <summary>
	/// Convert the user-supplied filename filter text into a regular expression
	/// for matching against each of the filenames enumerated.
	/// </summary>
	/// <returns>The compiled <see cref="Regex"/> pattern for matching filenames.
	/// </returns>
	private Regex CreateFilenameRegex() =>
		new(string.Join("|",
			FilenameFilter.Split(",").Select(filter =>
				"^" + Regex.Escape(filter.Trim()).Replace("\\*", ".*").Replace("\\?", ".") + "$")),
			RegexOptions.Compiled);

	/// <summary>
	/// Persist field values between user sessions. Run upon a successful search.
	/// </summary>
	private void SaveSettings()
	{
		var localSettings = ApplicationData.Current.LocalSettings;
		localSettings.Values["StartFolder"] = StartFolder;
		localSettings.Values["FilenameFilter"] = FilenameFilter;
		localSettings.Values["SearchString"] = SearchString;
	}

	protected virtual void OnDirectoryNotFound()
	{
		DirectoryNotFoundError?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void OnBlankSearchString()
	{
		BlankSearchStringError?.Invoke(this, EventArgs.Empty);
	}

	protected virtual void OnDocumentReady()
	{
		DocumentReady?.Invoke(this, EventArgs.Empty);
	}

}
