using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SearcherWinUI.ViewModels;

namespace SearcherWinUI.Views;

public sealed partial class MainPage : Page
{
	[DllImport("user32.dll")]
	private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

	private struct WINDOWPLACEMENT
	{
		public int length;
		public int flags;
		public int showCmd;
		public POINT ptMinPosition;
		public POINT ptMaxPosition;
		public RECT rcNormalPosition;
	}

	private struct POINT
	{
		public int X;
		public int Y;
	}

	private struct RECT
	{
		public int Left;
		public int Top;
		public int Right;
		public int Bottom;
	}

	private const int SW_MAXIMIZE = 3;

	public MainViewModel ViewModel
	{
		get;
	}

	public MainPage()
	{
		ViewModel = App.GetService<MainViewModel>();

		InitializeComponent();

		this.SizeChanged += MainPage_SizeChanged;

		ViewModel.DocumentReady += DocumentReady;

		SetupWatermarks();
		this.Loaded += (_, _) => SetWatermarkVisibility();
	}

	/// <summary>
	/// Wire up watermark visibility events.
	/// </summary>
	private void SetupWatermarks()
	{
		FilenameFilter.GotFocus += (_, _) =>
			FilenameFilterWatermark.Visibility = Visibility.Collapsed;
		FilenameFilter.LostFocus += (sender, e) => SetWatermarkVisibility();

		SearchPrompt.GotFocus += (_, _) =>
			SearchPromptWatermark.Visibility = Visibility.Collapsed;
		SearchPrompt.LostFocus += (_, _) => SetWatermarkVisibility();
	}

	/// <summary>
	/// Show/hide watermarks when the associated controls gain or lose focus.
	/// </summary>
	private void SetWatermarkVisibility()
	{
		FilenameFilterWatermark.Visibility =
			string.IsNullOrEmpty(FilenameFilter.Text) ?
				Visibility.Visible : Visibility.Collapsed;
		SearchPromptWatermark.Visibility =
			string.IsNullOrEmpty(SearchPrompt.Text) ?
				Visibility.Visible : Visibility.Collapsed;
	}

	private static bool IsWindowMaximized()
	{
		nint hWnd = App.MainWindow.GetWindowHandle();
		WINDOWPLACEMENT placement = new();
		placement.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
		GetWindowPlacement(hWnd, ref placement);

		return placement.showCmd == SW_MAXIMIZE;
	}

	private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
	{
		bool maximized = IsWindowMaximized();
		this.ViewModel.IsMaximized = maximized;
		// Not worried about minimized state for now.
		this.ViewModel.CalculatedWindowState =
			maximized ? WindowState.Maximized : WindowState.Normal;
	}

	private void DocumentReady(object? sender, EventArgs e)
	{
		FileContents.Document.SetText(TextSetOptions.None, ViewModel.FileContents);

		bool firstMatch = true;
		foreach (int index in ViewModel.Highlights)
		{
			// Highlight the found text.
			// NB: setting a range from 0 breaks the control!
			FileContents.Document.Selection.SetRange(
				index == 0 ? 0 : index, index + ViewModel.SearchString.Length);
			FileContents.Document.Selection.CharacterFormat.BackgroundColor =
				Windows.UI.Color.FromArgb(255, 128, 128, 0);

			if (!firstMatch)
			{
				// Scroll to the first instance of the found text.
				FileContents.Document.Selection.ScrollIntoView(PointOptions.None);
				firstMatch = false;
			}
		}
	}

	private async void ResultsListView_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
	{
		if (ViewModel.SelectedResult != null)
		{
			string argument = $"/select, \"{ViewModel.SelectedResult.FullName}\"";

			Process.Start("explorer.exe", argument);
		}
	}
}
