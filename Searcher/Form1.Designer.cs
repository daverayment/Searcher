namespace Searcher
{
	partial class Form1
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			statusStrip1 = new StatusStrip();
			Status = new ToolStripStatusLabel();
			splitContainer1 = new SplitContainer();
			SearchFilenames = new TextBox();
			label5 = new Label();
			Browse = new Button();
			RootFolder = new TextBox();
			label4 = new Label();
			Search = new Button();
			Results = new ListBox();
			label2 = new Label();
			label1 = new Label();
			Prompt = new TextBox();
			FileContents = new RichTextBox();
			label3 = new Label();
			FolderBrowser = new FolderBrowserDialog();
			toolTip1 = new ToolTip(components);
			statusStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
			splitContainer1.Panel1.SuspendLayout();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			SuspendLayout();
			// 
			// statusStrip1
			// 
			statusStrip1.ImageScalingSize = new Size(20, 20);
			statusStrip1.Items.AddRange(new ToolStripItem[] { Status });
			statusStrip1.Location = new Point(0, 613);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Size = new Size(1351, 22);
			statusStrip1.TabIndex = 0;
			statusStrip1.Text = "statusStrip1";
			// 
			// Status
			// 
			Status.Name = "Status";
			Status.Size = new Size(0, 16);
			// 
			// splitContainer1
			// 
			splitContainer1.Dock = DockStyle.Fill;
			splitContainer1.Location = new Point(0, 0);
			splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			splitContainer1.Panel1.Controls.Add(SearchFilenames);
			splitContainer1.Panel1.Controls.Add(label5);
			splitContainer1.Panel1.Controls.Add(Browse);
			splitContainer1.Panel1.Controls.Add(RootFolder);
			splitContainer1.Panel1.Controls.Add(label4);
			splitContainer1.Panel1.Controls.Add(Search);
			splitContainer1.Panel1.Controls.Add(Results);
			splitContainer1.Panel1.Controls.Add(label2);
			splitContainer1.Panel1.Controls.Add(label1);
			splitContainer1.Panel1.Controls.Add(Prompt);
			// 
			// splitContainer1.Panel2
			// 
			splitContainer1.Panel2.Controls.Add(FileContents);
			splitContainer1.Panel2.Controls.Add(label3);
			splitContainer1.Size = new Size(1351, 613);
			splitContainer1.SplitterDistance = 449;
			splitContainer1.TabIndex = 7;
			// 
			// SearchFilenames
			// 
			SearchFilenames.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			SearchFilenames.Location = new Point(12, 103);
			SearchFilenames.Name = "SearchFilenames";
			SearchFilenames.PlaceholderText = "E.g. \"*.txt\"";
			SearchFilenames.Size = new Size(341, 27);
			SearchFilenames.TabIndex = 9;
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new Point(12, 80);
			label5.Name = "label5";
			label5.Size = new Size(277, 20);
			label5.TabIndex = 8;
			label5.Text = "Filename filter (leave blank to search all)";
			// 
			// Browse
			// 
			Browse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			Browse.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
			Browse.Location = new Point(359, 29);
			Browse.Name = "Browse";
			Browse.Size = new Size(82, 33);
			Browse.TabIndex = 7;
			Browse.Text = "Browse...";
			Browse.UseVisualStyleBackColor = true;
			Browse.Click += Browse_Click;
			// 
			// RootFolder
			// 
			RootFolder.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			RootFolder.Location = new Point(12, 32);
			RootFolder.Name = "RootFolder";
			RootFolder.Size = new Size(341, 27);
			RootFolder.TabIndex = 6;
			RootFolder.Leave += RootFolder_Leave;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new Point(12, 9);
			label4.Name = "label4";
			label4.Size = new Size(84, 20);
			label4.TabIndex = 5;
			label4.Text = "Start folder";
			// 
			// Search
			// 
			Search.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			Search.Font = new Font("Segoe UI", 9F);
			Search.Location = new Point(338, 265);
			Search.Name = "Search";
			Search.Size = new Size(103, 33);
			Search.TabIndex = 2;
			Search.Text = "Search";
			Search.UseVisualStyleBackColor = true;
			Search.Click += Prompt_DoSearch;
			// 
			// Results
			// 
			Results.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			Results.DisplayMember = "DirectoryName";
			Results.Font = new Font("Segoe UI", 9F);
			Results.FormattingEnabled = true;
			Results.Location = new Point(12, 335);
			Results.Name = "Results";
			Results.Size = new Size(429, 264);
			Results.TabIndex = 4;
			Results.SelectedIndexChanged += Results_SelectedIndexChanged;
			Results.DoubleClick += Results_DoubleClick;
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Font = new Font("Segoe UI", 9F);
			label2.Location = new Point(12, 312);
			label2.Name = "label2";
			label2.Size = new Size(55, 20);
			label2.TabIndex = 3;
			label2.Text = "Results";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Font = new Font("Segoe UI", 9F);
			label1.Location = new Point(12, 152);
			label1.Name = "label1";
			label1.Size = new Size(82, 20);
			label1.TabIndex = 0;
			label1.Text = "Search text";
			// 
			// Prompt
			// 
			Prompt.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			Prompt.Font = new Font("Segoe UI", 9F);
			Prompt.Location = new Point(12, 175);
			Prompt.Multiline = true;
			Prompt.Name = "Prompt";
			Prompt.PlaceholderText = "Enter your search string";
			Prompt.Size = new Size(429, 84);
			Prompt.TabIndex = 1;
			Prompt.TextChanged += Prompt_TextChanged;
			// 
			// FileContents
			// 
			FileContents.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			FileContents.Location = new Point(3, 32);
			FileContents.Name = "FileContents";
			FileContents.Size = new Size(883, 571);
			FileContents.TabIndex = 1;
			FileContents.Text = "";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
			label3.Location = new Point(3, 9);
			label3.Name = "label3";
			label3.Size = new Size(92, 20);
			label3.TabIndex = 0;
			label3.Text = "File contents";
			// 
			// FolderBrowser
			// 
			FolderBrowser.ShowNewFolderButton = false;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(8F, 20F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1351, 635);
			Controls.Add(splitContainer1);
			Controls.Add(statusStrip1);
			Name = "Form1";
			Text = "Searcher";
			FormClosing += Form1_FormClosing;
			statusStrip1.ResumeLayout(false);
			statusStrip1.PerformLayout();
			splitContainer1.Panel1.ResumeLayout(false);
			splitContainer1.Panel1.PerformLayout();
			splitContainer1.Panel2.ResumeLayout(false);
			splitContainer1.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
			splitContainer1.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private StatusStrip statusStrip1;
		private ToolStripStatusLabel Status;
		private SplitContainer splitContainer1;
		private ListBox Results;
		private Label label2;
		private Label label1;
		private TextBox Prompt;
		private RichTextBox FileContents;
		private Label label3;
		private Button Search;
		private Button Browse;
		private TextBox RootFolder;
		private Label label4;
		private TextBox SearchFilenames;
		private Label label5;
		private FolderBrowserDialog FolderBrowser;
		private ToolTip toolTip1;
	}
}
