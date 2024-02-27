using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Ookii.Dialogs.Wpf;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SearcherWpf.ViewModels
{
	public class MainWindowViewModel : ObservableObject
	{
		// Properties.
		private string _startFolder = Settings.Default.StartFolder;
		private string _searchString = Settings.Default.SearchString;
		private string _filenameFilter = Settings.Default.FilenameFilter;
		private ObservableCollection<FileInfo> _searchResults = [];
		private string _searchButtonContent = "Search";
		private bool _isSearchEnabled = true;
		private bool _searchNotYetRun = true;
		private CancellationTokenSource? _cts;

		public Visibility NoMatchesFoundVisibility => (!_searchNotYetRun && _searchResults.Count == 0) ? Visibility.Visible : Visibility.Hidden;

		public Visibility SearchNotRunVisibility => _searchNotYetRun ? Visibility.Visible : Visibility.Hidden;

		public string StartFolder
		{
			get => _startFolder;
			set => SetProperty(ref _startFolder, value);
		}

		public string SearchString
		{
			get => _searchString;
			set => SetProperty(ref _searchString, value);
		}

		public string FilenameFilter
		{
			get => _filenameFilter;
			set => SetProperty(ref _filenameFilter, value);
		}

		public ObservableCollection<FileInfo> SearchResults
		{
			get => _searchResults;
			set => SetProperty(ref _searchResults, value);
		}

		public string SearchButtonContent
		{
			get => _searchButtonContent;
			private set => SetProperty(ref _searchButtonContent, value);
		}

		public bool IsSearchEnabled
		{
			get => _isSearchEnabled;
			private set => SetProperty(ref _isSearchEnabled, value);
		}

		// Commands.
		public IRelayCommand SearchCommand { get; }
		public IRelayCommand BrowseForStartFolderCommand { get; }

		// Events.
		public event EventHandler<MessageEventArgs> DirectoryNotFoundError = delegate { };


		public MainWindowViewModel()
		{
			SearchCommand = new RelayCommand(ExecuteSearchOrCancel);
			BrowseForStartFolderCommand = new RelayCommand(BrowseForStartFolder);
		}

		private async void ExecuteSearchOrCancel()
		{
			if (SearchButtonContent == "Search")
			{
				// Disable the button to prevent accidental double-clicks.
				IsSearchEnabled = false;
				await Task.Delay(200);

				SearchButtonContent = "Cancel";
				IsSearchEnabled = true;

				_cts = new CancellationTokenSource();

				try
				{
					_searchResults.Clear();
					await foreach (var result in SearchFiles(_cts.Token))
					{
						_searchResults.Add(result);
					}
				}
				catch (OperationCanceledException)
				{

				}
				finally
				{
					SearchButtonContent = "Search";
					IsSearchEnabled = true;
					OnPropertyChanged(nameof(NoMatchesFoundVisibility));
				}
			}
			else if (SearchButtonContent == "Cancel")
			{
				_cts?.Cancel();
				SearchButtonContent = "Search";
				IsSearchEnabled = true;
			}
		}

		/// <summary>
		/// Execute the search. Yields <see cref="FileInfo"/>s for files which
		/// match the filename pattern and which contain the search text.
		/// </summary>
		private async IAsyncEnumerable<FileInfo> SearchFiles(
			[EnumeratorCancellation] CancellationToken cancellationToken)
		{
			string trimmedStartFolder = _startFolder.Trim();
			if (trimmedStartFolder.Length == 0 ||
				!Directory.Exists(trimmedStartFolder))
			{
				OnDirectoryNotFound(trimmedStartFolder);
				yield break;
			}

			if (SearchString.Trim().Length == 0)
			{
				// TODO: search string error event
				//MessageBox.Show(this, $"Please supply a search string.",
				//	"Missing search string", MessageBoxButton.OK,
				//	MessageBoxImage.Error);
				//SearchPrompt.Focus();
				yield break;
			}

			SaveSettings();

			_searchNotYetRun = false;
			OnPropertyChanged(nameof(SearchNotRunVisibility));

			// TODO: clear document event
			//FileContents.Document.Blocks.Clear();

			Regex pattern = CreateFilenameRegex();
			var startDirInfo = new DirectoryInfo(trimmedStartFolder);

			foreach (var file in startDirInfo.EnumerateFiles("*", new EnumerationOptions
			{
				IgnoreInaccessible = true,
				RecurseSubdirectories = true,
				AttributesToSkip = FileAttributes.ReparsePoint
			}))
			{
				cancellationToken.ThrowIfCancellationRequested();

				if (pattern.IsMatch(file.Name))
				{
					// TODO: status changed event
					//Status.Content = $"Searching {file.FullName}...";

					string contents = await File.ReadAllTextAsync(file.FullName,
						cancellationToken);

					if (contents.Contains(SearchString.Trim(),
						StringComparison.OrdinalIgnoreCase))
					{
						yield return file;
					}
				}
			}
		}

		/// <summary>
		/// Convert the user-supplied filename filter text into a regular expression
		/// for matching against each of the filenames enumerated.
		/// </summary>
		/// <returns>The compiled <see cref="Regex"/> pattern for matching filenames.
		/// </returns>
		private Regex CreateFilenameRegex() => new(
			string.Join("|",
				FilenameFilter.Split(",").Select(filter =>
					"^" + Regex.Escape(filter.Trim()).Replace("\\*", ".*").Replace("\\?", ".") + "$")),
				RegexOptions.Compiled);

		protected virtual void OnDirectoryNotFound(string directory)
		{
			DirectoryNotFoundError?.Invoke(this,
				new MessageEventArgs { Message = $"{directory} not found." });
		}

		/// <summary>
		/// Allow the user to browse for a start folder.
		/// </summary>
		private void BrowseForStartFolder()
		{
			VistaFolderBrowserDialog dialog = new();

			if (dialog.ShowDialog().HasValue)
			{
				if (Directory.Exists(dialog.SelectedPath))
				{
					StartFolder = dialog.SelectedPath;
				}
				else
				{
					OnDirectoryNotFound(dialog.SelectedPath);
				}
			}
		}

		/// <summary>
		/// Persist field values between user sessions. Run upon a successful search.
		/// </summary>
		private void SaveSettings()
		{
			Settings.Default.StartFolder = _startFolder;
			Settings.Default.SearchString = _searchString;
			Settings.Default.FilenameFilter = _filenameFilter;
			Settings.Default.Save();
		}
	}

	public class MessageEventArgs : EventArgs
	{
		public string Message { get; set; }
	}
}
