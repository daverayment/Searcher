namespace Searcher;

public partial class Form1 : Form
{
	private DirectoryInfo? StartFolder;

	public Form1()
	{
		InitializeComponent();
		RestoreStartFolder();
	}

	private void RestoreStartFolder()
	{
		string saved = Settings.Default.StartFolder;
		RootFolder.Text = saved;
		StartFolder = new DirectoryInfo(saved);
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
			string folder = StartFolder?.FullName ?? "";
			MessageBox.Show(this, $"Directory \"{folder}\" not found.",
				"Invalid Start Folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
			yield break;
		}

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
			MessageBox.Show(this, "Directory doesn't exist.",
				"Invalid Directory", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}

	private void Form1_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (RootFolder.Text != "" && Directory.Exists(RootFolder.Text))
		{
			Settings.Default.StartFolder = RootFolder.Text;
			Settings.Default.Save();
		}
	}
}
