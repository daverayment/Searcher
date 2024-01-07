namespace Searcher;

public partial class Form1 : Form
{
	private DirectoryInfo? StartFolder;

	public Form1()
	{
		InitializeComponent();
		RestoreSettings();
	}

	private void RestoreSettings()
	{
		string saved = Settings.Default.StartFolder;
		RootFolder.Text = saved;
		StartFolder = new DirectoryInfo(saved);
		FilenameFilter.Text = Settings.Default.FileFilter;
		Prompt.Text = Settings.Default.SearchString;
	}

	private async void Prompt_DoSearch(object sender, EventArgs e)
	{
		Results.Items.Clear();

		await foreach (var result in SearchFilesForPrompt(Prompt.Text))
		{
			Results.Items.Add(result);
		}

		Status.Text = $"Finished search. Found {Results.Items.Count} file matches.";
	}

	private async IAsyncEnumerable<FileInfo> SearchFilesForPrompt(string prompt)
	{
		if (StartFolder == null || !Directory.Exists(StartFolder.FullName))
		{
			ShowInvalidDirectoryAlert();
			yield break;
		}

		SaveSettings();

		foreach (var file in StartFolder.EnumerateFiles("Prompts.txt",
			new EnumerationOptions
			{
				IgnoreInaccessible = true,
				RecurseSubdirectories = true,
				AttributesToSkip = FileAttributes.ReparsePoint
			}))
		{
			Status.Text = $"Searching {file.FullName}.";

			string contents = await File.ReadAllTextAsync(file.FullName);

			if (contents.Contains(prompt, StringComparison.OrdinalIgnoreCase))
			{
				yield return file;
			}
		}
	}

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

	private void Browse_Click(object sender, EventArgs e)
	{
		if (FolderBrowser.ShowDialog() == DialogResult.OK)
		{
			UpdateStartFolder(FolderBrowser.SelectedPath);
		}
	}

	private void UpdateStartFolder(string selectedPath)
	{
		if (Directory.Exists(selectedPath))
		{
			RootFolder.Text = selectedPath;
			StartFolder = new DirectoryInfo(selectedPath);
		}
		else
		{
			ShowInvalidDirectoryAlert();
		}
	}

	private void ShowInvalidDirectoryAlert()
	{
		string folder = StartFolder?.FullName ?? string.Empty;
		folder = folder.Length > 0 ? " " : folder;
		MessageBox.Show(this, $"Directory \"{folder}\"not found.",
			"Directory Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
	}

	/// <summary>
	/// Persist field values between user sessions. Run upon a successful search.
	/// </summary>
	private void SaveSettings()
	{
		Settings.Default.StartFolder = RootFolder.Text;
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

	private void UpdateSearchButton()
	{
		Search.Enabled = RootFolder.Text.Trim().Length > 0 &&
			Prompt.Text.Trim().Length > 0 &&
			StartFolder != null &&
			Directory.Exists(StartFolder.FullName);
		Status.Text = Search.Enabled ? "Ready to search." : "";
	}

	private void RootFolder_Leave(object sender, EventArgs e)
	{
		UpdateSearchButton();
		if (!Directory.Exists(RootFolder.Text))
		{
			Status.Text = $"\"{RootFolder.Text}\" doesn't exist.";
		}
	}
}
