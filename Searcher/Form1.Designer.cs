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
			filenameFilterTextBox = new TextBox();
			label5 = new Label();
			browseButton = new Button();
			startFolderTextBox = new TextBox();
			label4 = new Label();
			searchCancelButton = new Button();
			resultsListBox = new ListBox();
			label2 = new Label();
			label1 = new Label();
			searchStringTextBox = new TextBox();
			fileContentsRichTextBox = new RichTextBox();
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
			splitContainer1.Panel1.Controls.Add(filenameFilterTextBox);
			splitContainer1.Panel1.Controls.Add(label5);
			splitContainer1.Panel1.Controls.Add(browseButton);
			splitContainer1.Panel1.Controls.Add(startFolderTextBox);
			splitContainer1.Panel1.Controls.Add(label4);
			splitContainer1.Panel1.Controls.Add(searchCancelButton);
			splitContainer1.Panel1.Controls.Add(resultsListBox);
			splitContainer1.Panel1.Controls.Add(label2);
			splitContainer1.Panel1.Controls.Add(label1);
			splitContainer1.Panel1.Controls.Add(searchStringTextBox);
			// 
			// splitContainer1.Panel2
			// 
			splitContainer1.Panel2.Controls.Add(fileContentsRichTextBox);
			splitContainer1.Panel2.Controls.Add(label3);
			splitContainer1.Size = new Size(1351, 613);
			splitContainer1.SplitterDistance = 449;
			splitContainer1.TabIndex = 7;
			// 
			// filenameFilterTextBox
			// 
			filenameFilterTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			filenameFilterTextBox.Location = new Point(12, 103);
			filenameFilterTextBox.Name = "filenameFilterTextBox";
			filenameFilterTextBox.PlaceholderText = "E.g. \"*.txt\"";
			filenameFilterTextBox.Size = new Size(341, 27);
			filenameFilterTextBox.TabIndex = 9;
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
			// browseButton
			// 
			browseButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			browseButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
			browseButton.Location = new Point(359, 29);
			browseButton.Name = "browseButton";
			browseButton.Size = new Size(82, 33);
			browseButton.TabIndex = 7;
			browseButton.Text = "Browse...";
			browseButton.UseVisualStyleBackColor = true;
			browseButton.Click += BrowseButton_Click;
			// 
			// startFolderTextBox
			// 
			startFolderTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			startFolderTextBox.Location = new Point(12, 32);
			startFolderTextBox.Name = "startFolderTextBox";
			startFolderTextBox.Size = new Size(341, 27);
			startFolderTextBox.TabIndex = 6;
			startFolderTextBox.Leave += StartFolderTextBox_Leave;
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
			// searchCancelButton
			// 
			searchCancelButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			searchCancelButton.Font = new Font("Segoe UI", 9F);
			searchCancelButton.Location = new Point(338, 265);
			searchCancelButton.Name = "searchCancelButton";
			searchCancelButton.Size = new Size(103, 33);
			searchCancelButton.TabIndex = 2;
			searchCancelButton.Text = "Search";
			searchCancelButton.UseVisualStyleBackColor = true;
			searchCancelButton.Click += SearchCancelButton_Click;
			// 
			// resultsListBox
			// 
			resultsListBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			resultsListBox.DisplayMember = "DirectoryName";
			resultsListBox.Font = new Font("Segoe UI", 9F);
			resultsListBox.FormattingEnabled = true;
			resultsListBox.Location = new Point(12, 335);
			resultsListBox.Name = "resultsListBox";
			resultsListBox.Size = new Size(429, 264);
			resultsListBox.TabIndex = 4;
			resultsListBox.SelectedIndexChanged += ResultsListBox_SelectedIndexChanged;
			resultsListBox.DoubleClick += ResultsListBox_DoubleClick;
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
			// searchStringTextBox
			// 
			searchStringTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			searchStringTextBox.Font = new Font("Segoe UI", 9F);
			searchStringTextBox.Location = new Point(12, 175);
			searchStringTextBox.Multiline = true;
			searchStringTextBox.Name = "searchStringTextBox";
			searchStringTextBox.PlaceholderText = "Enter your search string";
			searchStringTextBox.Size = new Size(429, 84);
			searchStringTextBox.TabIndex = 1;
			searchStringTextBox.TextChanged += SearchStringTextBox_TextChanged;
			// 
			// fileContentsRichTextBox
			// 
			fileContentsRichTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			fileContentsRichTextBox.Location = new Point(3, 32);
			fileContentsRichTextBox.Name = "fileContentsRichTextBox";
			fileContentsRichTextBox.Size = new Size(883, 573);
			fileContentsRichTextBox.TabIndex = 1;
			fileContentsRichTextBox.Text = "";
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
		private ListBox resultsListBox;
		private Label label2;
		private Label label1;
		private TextBox searchStringTextBox;
		private RichTextBox fileContentsRichTextBox;
		private Label label3;
		private Button searchCancelButton;
		private Button browseButton;
		private TextBox startFolderTextBox;
		private Label label4;
		private TextBox filenameFilterTextBox;
		private Label label5;
		private FolderBrowserDialog FolderBrowser;
		private ToolTip toolTip1;
	}
}
