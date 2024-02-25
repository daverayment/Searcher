using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ookii.Dialogs.Wpf;

namespace SearcherWpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private bool _initialStateSet = false;
		private const int SearchTransitionDelay = 200;
		private CancellationTokenSource? _cts;
		//private FileSystemWatcher _watcher;

		public MainWindow()
		{
			InitializeComponent();

			Loaded += MainWindow_Loaded;
			SearchCancelButton.Click += SearchCancelButton_Click;

			FilenameFilter.GotFocus += (sender, e) => FilenameFilterWatermark.Visibility = Visibility.Collapsed;
			FilenameFilter.LostFocus += (sender, e) => SetWatermarkVisibility(FilenameFilter, FilenameFilterWatermark);

			SearchPrompt.GotFocus += (_, _) => SearchPromptWatermark.Visibility = Visibility.Collapsed;
			SearchPrompt.LostFocus += (sender, e) => SetWatermarkVisibility(SearchPrompt, SearchPromptWatermark);
		}

		private async void SearchCancelButton_Click(object sender, RoutedEventArgs e)
		{
			if ((string)SearchCancelButton.Content == "Cancel")
			{
				_cts?.Cancel();
				return;
			}

			TransitionSearchCancelButton();
			ResultsList.Items.Clear();
			_cts = new CancellationTokenSource();

			try
			{
				await foreach (var result in SearchFiles(
					SearchPrompt.Text.Trim(), _cts.Token))
				{
					ResultsList.Items.Add(result);
				}

				Status.Content = $"Finished search. Found {ResultsList.Items.Count} files.";
			}
			catch (OperationCanceledException)
			{
				Status.Content = "Search cancelled.";
			}
			finally
			{
				_cts.Dispose();
				SearchCancelButton.Content = "Search";
				SearchCancelButton.IsEnabled = true;
			}
		}

        /// <summary>
        /// Transition the Search button to a Cancel button after a brief delay.
        /// </summary>
        private async void TransitionSearchCancelButton()
		{
			SearchCancelButton.IsEnabled = false;
			await Task.Delay(SearchTransitionDelay);
			if (_cts != null && !_cts.IsCancellationRequested)
			{
				SearchCancelButton.Content = "Cancel";
				SearchCancelButton.IsEnabled = true;
			}
		}

        /// <summary>
        /// Execute the search. Yields <see cref="FileInfo"/>s for files which
        /// match the filename pattern and which contain the search text.
        /// </summary>
        private async IAsyncEnumerable<FileInfo> SearchFiles(string searchText,
			[EnumeratorCancellation] CancellationToken cancellationToken)
		{
			if (StartFolder.Text.Trim().Length == 0 ||
                !Directory.Exists(StartFolder.Text.Trim()))
			{
				ShowInvalidDirectoryAlert();
				yield break;
			}

			if (SearchPrompt.Text.Trim().Length == 0)
			{
				MessageBox.Show(this, $"Please supply a search string.",
					"Missing search string", MessageBoxButton.OK,
					MessageBoxImage.Error);
				SearchPrompt.Focus();
				yield break;
			}

			SaveSettings();
			FileContents.Document.Blocks.Clear();

			Regex pattern = CreateFilenameRegex();
			var startDirInfo = new DirectoryInfo(StartFolder.Text.Trim());

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
					Status.Content = $"Searching {file.FullName}...";

					string contents = await File.ReadAllTextAsync(file.FullName,
						cancellationToken);

					if (contents.Contains(searchText, StringComparison.OrdinalIgnoreCase))
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
				FilenameFilter.Text.Split(",").Select(filter =>
					"^" + Regex.Escape(filter.Trim()).Replace("\\*", ".*").Replace("\\?", ".") + "$")),
				RegexOptions.Compiled);

        /// <summary>
        /// Persist field values between user sessions. Run upon a successful search.
        /// </summary>
        private void SaveSettings()
		{
            Settings.Default.StartFolder = StartFolder.Text;
            Settings.Default.SearchString = SearchPrompt.Text;
            Settings.Default.FilenameFilter = FilenameFilter.Text;
            Settings.Default.Save();
        }

        /// <summary>
        /// Show directory not found alert.
        /// </summary>
        private void ShowInvalidDirectoryAlert()
		{
            MessageBox.Show(this, $"Directory \"{StartFolder.Text}\"not found.",
                "Directory Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			SetInitialState();
		}

		private void SetInitialState()
		{
			if (!_initialStateSet)
			{
				StartFolderPanel.MinHeight = StartFolder.ActualHeight + 1;

                FilenameFilterGrid.MinHeight = FilenameFilter.ActualHeight + 1;

                FilenameFilterWatermark.Visibility = string.IsNullOrEmpty(FilenameFilter.Text) ? Visibility.Visible : Visibility.Collapsed;
                SearchPromptWatermark.Visibility = string.IsNullOrEmpty(SearchPrompt.Text) ? Visibility.Visible : Visibility.Collapsed;

                _initialStateSet = true;
			}
		}

		private static void SetWatermarkVisibility(TextBox control, Label watermark)
		{
			if (control != null && watermark != null)
			{
				watermark.Visibility = string.IsNullOrEmpty(control.Text) ?
					Visibility.Visible : Visibility.Collapsed;
			}            
		}

        private void BrowseForStartFolderButton_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog dialog = new();

            var result = dialog.ShowDialog();
            if (result.HasValue)
            {
                if (Directory.Exists(dialog.SelectedPath))
                {
                    StartFolder.Text = dialog.SelectedPath;
                }
                else
                {
                    ShowInvalidDirectoryAlert();
                }
            }
        }

        //public static void AddWatermark(object element, string watermarkText)
        //{
        //    if (element is TextBox textBox)
        //    {
        //        RemoveWatermark(element); // Ensure no existing watermark adorner

        //        if (textBox.Text.Length == 0)
        //        {
        //            var adornerLayer = AdornerLayer.GetAdornerLayer(textBox);
        //            if (adornerLayer != null)
        //            {
        //                var watermarkAdorner = new WatermarkAdorner(textBox, watermarkText);
        //                adornerLayer.Add(watermarkAdorner);
        //            }
        //        }
        //    }
        //}

        //public static void RemoveWatermark(object element)
        //{
        //    if (element is TextBox textBox)
        //    {
        //        var adornerLayer = AdornerLayer.GetAdornerLayer(textBox);
        //        if (adornerLayer != null)
        //        {
        //            var adorners = adornerLayer.GetAdorners(textBox);
        //            if (adorners != null)
        //            {
        //                var watermarkAdorner = adorners.OfType<WatermarkAdorner>().FirstOrDefault();
        //                if (watermarkAdorner != null)
        //                {
        //                    adornerLayer.Remove(watermarkAdorner);
        //                }
        //            }
        //        }
        //    }
        //}
    }
}