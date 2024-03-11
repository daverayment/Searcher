using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Windowing;
using Microsoft.Windows.ApplicationModel.Resources;
using SearcherWinUI.Helpers;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace SearcherWinUI.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
	[ObservableProperty]
	private string _startFolder;

	[ObservableProperty]
	private string _searchString;

	[ObservableProperty]
	private string _filenameFilter;

	[ObservableProperty]
	private readonly string _searchButtonContent;

	[ObservableProperty]
	private bool _isSearchButtonEnabled = true;

	private ObservableCollection<FileInfo> _searchResults = new();

	public IRelayCommand SearchCommand { get; }
	public IRelayCommand<DispatcherQueue?> BrowseForStartFolderCommand { get; }

	public event EventHandler<EventArgs> DirectoryNotFoundError;

	public MainViewModel()
	{
		LoadSettings();

		var resourceLoader = new ResourceLoader();
		SearchButtonContent = resourceLoader.GetString("Main_SearchButton_Search/Text");

		SearchCommand = new RelayCommand(ExecuteSearchOrCancel);
		BrowseForStartFolderCommand = new RelayCommand<DispatcherQueue?>(BrowseForStartFolder);
	}

	/// <summary>
	/// Allow the user to browse for a start folder.
	/// </summary>
	private void BrowseForStartFolder(DispatcherQueue? dispatcherQueue)
	{
		dispatcherQueue?.TryEnqueue(async () =>
		{
			try
			{
				var folderPicker = new FolderPicker
				{
					SuggestedStartLocation = PickerLocationId.Desktop
				};
				folderPicker.FileTypeFilter.Add("*");

				StorageFolder folder = await folderPicker.PickSingleFolderAsync();

				if (folder != null)
				{
					if (Directory.Exists(folder.Path))
					{
						StartFolder = folder.Path;
					}
					else
					{
						OnDirectoryNotFound(folder.Path);
					}
				}
			}
			catch (Exception ex)
			{
				// TODO: handle
			}
		});
	}

	protected virtual void OnDirectoryNotFound(string directory)
	{
		DirectoryNotFoundError?.Invoke(this, EventArgs.Empty);
	}

	private void ExecuteSearchOrCancel()
	{
		throw new NotImplementedException();
	}

	private void LoadSettings()
	{
		var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
		StartFolder = (string)(localSettings.Values["StartFolder"] ?? "C:\\");
		FilenameFilter = (string)(localSettings.Values["FilenameFilter"] ?? "");
		SearchString = (string)(localSettings.Values["SearchString"] ?? "");
	}
}
