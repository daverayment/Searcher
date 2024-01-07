using System.Text.RegularExpressions;

namespace Searcher;

public partial class Form1 : Form
{
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
		StartFolder.Text = Settings.Default.StartFolder;
		FilenameFilter.Text = Settings.Default.FileFilter;
		Prompt.Text = Settings.Default.SearchString;
	}

	/// <summary>
	/// Execute the search and update the results as matches are found.
	/// </summary>
	private async void Prompt_DoSearch(object sender, EventArgs e)
	{
		Results.Items.Clear();

		await foreach (var result in SearchFilesForPrompt(Prompt.Text))
		{
			Results.Items.Add(result);
		}

		Status.Text = $"Finished search. Found {Results.Items.Count} file matches.";
	}

	/// <summary>
	/// Execute the search. Yields <see cref="FileInfo"/>s for files which
	/// match the filename pattern and which contain the search text.
	/// </summary>
	private async IAsyncEnumerable<FileInfo> SearchFilesForPrompt(string searchText)
	{
		if (StartFolder.Text == "" || !Directory.Exists(StartFolder.Text))
		{
			ShowInvalidDirectoryAlert();
			yield break;
		}

		SaveSettings();
		FileContents.Clear();

		Regex pattern = CreateFilenameRegex();
		var startDirInfo = new DirectoryInfo(StartFolder.Text);

		foreach (var file in startDirInfo.EnumerateFiles("*", new EnumerationOptions
			{
				IgnoreInaccessible = true,
				RecurseSubdirectories = true,
				AttributesToSkip = FileAttributes.ReparsePoint
			})
		)
		{
			if (pattern.IsMatch(file.Name))
			{
				Status.Text = $"Searching {file.FullName}.";

				string contents = await File.ReadAllTextAsync(file.FullName);

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
		new (string.Join("|",
			FilenameFilter.Text.Split(",").Select(filter =>
				"^" + Regex.Escape(filter.Trim()).Replace("\\*", ".*").Replace("\\?", ".") + "$")),
			RegexOptions.Compiled);

	/// <summary>
	/// Open a new Explorer window when a result is double-clicked.
	/// </summary>
	private void Results_DoubleClick(object sender, EventArgs e)
	{
		int index = Results.IndexFromPoint(((MouseEventArgs)e).Location);

		if (index < 0) return;

		FileInfo info = (FileInfo)Results.Items[index];
		string? directory = info.Directory?.FullName;
		if (directory != null)
		{
			System.Diagnostics.Process.Start("explorer.exe", directory);
		}
	}

	/// <summary>
	/// Update the contents view when a result is selected.
	/// </summary>
	private void Results_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (Results.SelectedIndex < 0) return;

		FileInfo? info = (FileInfo?)Results.SelectedItem;
		if (info == null) return;

		FileContents.Text = File.ReadAllText(info.FullName);
		HighlightText();
	}

	/// <summary>
	/// Highlight all instances of the search text in the contents view and scroll
	/// to the first match.
	/// </summary>
	private void HighlightText()
	{
		if (Prompt.Text.Length == 0) return;

		bool isFirstMatch = true;
		int start = 0;

		while ((start = FileContents.Find(Prompt.Text, start, RichTextBoxFinds.None)) != -1)
		{
			// Scroll to first match.
			if (isFirstMatch)
			{
				FileContents.ScrollToCaret();
				isFirstMatch = false;
			}

			FileContents.SelectionStart = start;
			FileContents.SelectionLength = Prompt.Text.Length;
			FileContents.SelectionBackColor = Color.Yellow;

			start += Prompt.Text.Length;
		}
	}

	/// <summary>
	/// Let the user browse for a new start folder.
	/// </summary>
	private void Browse_Click(object sender, EventArgs e)
	{
		if (FolderBrowser.ShowDialog() == DialogResult.OK)
		{
			string selectedPath = FolderBrowser.SelectedPath;
			if (Directory.Exists(selectedPath))
			{
				StartFolder.Text = selectedPath;
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
		MessageBox.Show(this, $"Directory \"{StartFolder.Text}\"not found.",
			"Directory Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
	}

	/// <summary>
	/// Persist field values between user sessions. Run upon a successful search.
	/// </summary>
	private void SaveSettings()
	{
		Settings.Default.StartFolder = StartFolder.Text;
		Settings.Default.SearchString = Prompt.Text;
		Settings.Default.FileFilter = FilenameFilter.Text;
		Settings.Default.Save();
	}

	private void RootFolder_TextChanged(object sender, EventArgs e)
	{
		UpdateSearchButton();
	}

	private void Prompt_TextChanged(object sender, EventArgs e)
	{
		UpdateSearchButton();
	}

	/// <summary>
	/// Only enable the Seach button when both a start folder and search text
	/// are valid.
	/// </summary>
	private void UpdateSearchButton()
	{
		Search.Enabled = StartFolder.Text.Trim().Length > 0 &&
			Prompt.Text.Trim().Length > 0 &&
			Directory.Exists(StartFolder.Text);
		Status.Text = Search.Enabled ? "Ready to search." : "";
	}

	/// <summary>
	/// Check directory exists when the start folder field has been edited.
	/// </summary>
	private void StartFolder_Leave(object sender, EventArgs e)
	{
		UpdateSearchButton();
		if (!Directory.Exists(StartFolder.Text))
		{
			Status.Text = $"\"{StartFolder.Text}\" doesn't exist.";
		}
	}
}
