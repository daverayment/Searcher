using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Searcher;

public partial class Form1 : Form
{
	private const int SearchTransitionDelay = 200;
	private CancellationTokenSource? _cts;

	public Form1()
	{
		InitializeComponent();
		RestoreSettings();
	}

	/// <summary>
	/// Restore field values from previous session (or defaults if this is first run).
	/// </summary>
	private void RestoreSettings()
	{
		startFolderTextBox.Text = Settings.Default.StartFolder;
		filenameFilterTextBox.Text = Settings.Default.FileFilter;
		searchStringTextBox.Text = Settings.Default.SearchString;
	}

	/// <summary>
	/// Execute the search and update the results as matches are found.
	/// </summary>
	private async void SearchCancelButton_Click(object sender, EventArgs e)
	{
		if (searchCancelButton.Text == "Cancel")
		{
			_cts?.Cancel();
			return;
		}

		TransitionSearchButton();
		resultsListBox.Items.Clear();
		_cts = new CancellationTokenSource();

		try
		{
			await foreach (var result in SearchFiles(searchStringTextBox.Text, _cts.Token))
			{
				resultsListBox.Items.Add(result);
			}

			Status.Text = $"Finished search. Found {resultsListBox.Items.Count} file matches.";
		}
		catch (OperationCanceledException)
		{
			Status.Text = "Search cancelled.";
		}
		finally
		{
			_cts.Dispose();
			searchCancelButton.Text = "Search";
			searchCancelButton.Enabled = true;
		}
	}

	/// <summary>
	/// Transition the Search button to a Cancel button after a brief delay.
	/// </summary>
	private async void TransitionSearchButton()
	{
		searchCancelButton.Enabled = false;
		await Task.Delay(SearchTransitionDelay);
		if (_cts != null && !_cts.IsCancellationRequested)
		{
			searchCancelButton.Text = "Cancel";
			searchCancelButton.Enabled = true;
		}
	}

	/// <summary>
	/// Execute the search. Yields <see cref="FileInfo"/>s for files which
	/// match the filename pattern and which contain the search text.
	/// </summary>
	private async IAsyncEnumerable<FileInfo> SearchFiles(string searchText,
		[EnumeratorCancellation] CancellationToken cancellationToken)
	{
		if (startFolderTextBox.Text == "" || !Directory.Exists(startFolderTextBox.Text))
		{
			ShowInvalidDirectoryAlert();
			yield break;
		}

		SaveSettings();
		fileContentsRichTextBox.Clear();

		Regex pattern = CreateFilenameRegex();
		var startDirInfo = new DirectoryInfo(startFolderTextBox.Text);

		foreach (var file in startDirInfo.EnumerateFiles("*", new EnumerationOptions
		{
			IgnoreInaccessible = true,
			RecurseSubdirectories = true,
			AttributesToSkip = FileAttributes.ReparsePoint
		})
		)
		{
			cancellationToken.ThrowIfCancellationRequested();

			if (pattern.IsMatch(file.Name))
			{
				Status.Text = $"Searching {file.FullName}.";

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
	private Regex CreateFilenameRegex() =>
		new(string.Join("|",
			filenameFilterTextBox.Text.Split(",").Select(filter =>
				"^" + Regex.Escape(filter.Trim()).Replace("\\*", ".*").Replace("\\?", ".") + "$")),
			RegexOptions.Compiled);

	/// <summary>
	/// Open a new Explorer window when a result is double-clicked.
	/// </summary>
	private void ResultsListBox_DoubleClick(object sender, EventArgs e)
	{
		int index = resultsListBox.IndexFromPoint(((MouseEventArgs)e).Location);

		if (index < 0) return;

		FileInfo info = (FileInfo)resultsListBox.Items[index];
		string? directory = info.Directory?.FullName;
		if (directory != null)
		{
			System.Diagnostics.Process.Start("explorer.exe", directory);
		}
	}

	/// <summary>
	/// Update the contents view when a result is selected.
	/// </summary>
	private void ResultsListBox_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (resultsListBox.SelectedIndex < 0) return;

		FileInfo? info = (FileInfo?)resultsListBox.SelectedItem;
		if (info == null) return;

		fileContentsRichTextBox.Text = File.ReadAllText(info.FullName);
		HighlightText();
	}

	/// <summary>
	/// Highlight all instances of the search text in the contents view and scroll
	/// to the first match.
	/// </summary>
	private void HighlightText()
	{
		if (searchStringTextBox.Text.Length == 0) return;

		bool isFirstMatch = true;
		int start = 0;

		while ((start = fileContentsRichTextBox.Find(searchStringTextBox.Text, start, RichTextBoxFinds.None)) != -1)
		{
			// Scroll to first match.
			if (isFirstMatch)
			{
				fileContentsRichTextBox.ScrollToCaret();
				isFirstMatch = false;
			}

			fileContentsRichTextBox.SelectionStart = start;
			fileContentsRichTextBox.SelectionLength = searchStringTextBox.Text.Length;
			fileContentsRichTextBox.SelectionBackColor = Color.Yellow;

			start += searchStringTextBox.Text.Length;
		}
	}

	/// <summary>
	/// Let the user browse for a new start folder.
	/// </summary>
	private void BrowseButton_Click(object sender, EventArgs e)
	{
		if (FolderBrowser.ShowDialog() == DialogResult.OK)
		{
			string selectedPath = FolderBrowser.SelectedPath;
			if (Directory.Exists(selectedPath))
			{
				startFolderTextBox.Text = selectedPath;
			}
			else
			{
				ShowInvalidDirectoryAlert();
			}
		}
	}

	/// <summary>
	/// Show directory not found alert.
	/// </summary>
	private void ShowInvalidDirectoryAlert()
	{
		MessageBox.Show(this, $"Directory \"{startFolderTextBox.Text}\"not found.",
			"Directory Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
	}

	/// <summary>
	/// Persist field values between user sessions. Run upon a successful search.
	/// </summary>
	private void SaveSettings()
	{
		Settings.Default.StartFolder = startFolderTextBox.Text;
		Settings.Default.SearchString = searchStringTextBox.Text;
		Settings.Default.FileFilter = filenameFilterTextBox.Text;
		Settings.Default.Save();
	}

	private void SearchStringTextBox_TextChanged(object sender, EventArgs e)
	{
		UpdateSearchButton();
	}

	/// <summary>
	/// Only enable the Seach button when both a start folder and search text
	/// are valid.
	/// </summary>
	private void UpdateSearchButton()
	{
		searchCancelButton.Enabled = startFolderTextBox.Text.Trim().Length > 0 &&
			searchStringTextBox.Text.Trim().Length > 0 &&
			Directory.Exists(startFolderTextBox.Text);
		Status.Text = searchCancelButton.Enabled ? "Ready to search." : "";
	}

	/// <summary>
	/// Check directory exists when the start folder field has been edited.
	/// </summary>
	private void StartFolderTextBox_Leave(object sender, EventArgs e)
	{
		UpdateSearchButton();
		if (!Directory.Exists(startFolderTextBox.Text))
		{
			Status.Text = $"\"{startFolderTextBox.Text}\" doesn't exist.";
		}
	}
}
