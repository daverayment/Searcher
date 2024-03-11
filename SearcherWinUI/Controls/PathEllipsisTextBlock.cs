using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SearcherWinUI.Helpers;

namespace SearcherWinUI.Controls;

public class PathEllipsisTextBlock : Control
{
	/// <summary>
	/// The wrapped TextBlock control for the possibly-truncated path.
	/// </summary>
	private TextBlock? _textBlock;

	/// <summary>
	/// Cache of strings to their pixel widths.
	/// </summary>
	private Dictionary<string, double> _stringToLengthMap = new();

	/// <summary>
	/// Cache of full paths to their truncated 
	/// </summary>
	private Dictionary<string, string> _stringToTruncatedStringMap = new();

	private TextMeasurement _measurement;

	/// <summary>
	/// Timer to throttle layout changes.
	/// </summary>
	private static DispatcherTimer _throttleTimer;
	private static bool _timerInitialized = false;
	private static List<PathEllipsisTextBlock> _pendingUpdates = new();

	/// <summary>
	/// The Text property receives the path to be trimmed. Changes result in
	/// updates to the displayed TextBlock, but the Text is unchanged.
	/// </summary>
	public static readonly DependencyProperty TextProperty =
		DependencyProperty.Register(
			"Text",
			typeof(string),
			typeof(PathEllipsisTextBlock),
			new PropertyMetadata(default(string), OnTextChanged));

	/// <summary>
	/// The non-filename part of the full path.
	/// </summary>
	private string _directoryPath;

	/// <summary>
	/// The filename part of the path.
	/// </summary>
	private string _filename;

	/// <summary>
	/// The full file path to be represented within this control.
	/// </summary>
	public string Text
	{
		get => (string)GetValue(TextProperty);
		set
		{
			SetValue(TextProperty, value);

			_filename = Path.GetFileName(value);
			_directoryPath = Path.GetDirectoryName(value) ?? "";
		}
	}

	public PathEllipsisTextBlock()
	{
		this.DefaultStyleKey = typeof(PathEllipsisTextBlock);

		if (!_timerInitialized )
		{
			_throttleTimer = new DispatcherTimer();
			_throttleTimer.Interval = TimeSpan.FromMilliseconds(50);
			_throttleTimer.Tick += ThrottleTimer_Tick;
		}

		this.Loaded += OnLoaded;
		this.SizeChanged += OnSizeChanged;
	}

	private void ThrottleTimer_Tick(object? sender, object e)
	{
		foreach (var control in  _pendingUpdates)
		{
			control.ApplyPathEllipsis();
		}
		_pendingUpdates.Clear();
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		ApplyPathEllipsis();
	}

	private void OnSizeChanged(object sender, RoutedEventArgs e)
	{
		ApplyPathEllipsis();
		return;

		if (!_pendingUpdates.Contains(this))
		{
			_pendingUpdates.Add(this);
		}

		if (!_throttleTimer.IsEnabled)
		{
			_throttleTimer.Start();
		}
	}

	protected override void OnApplyTemplate()
	{
		base.OnApplyTemplate();
		_textBlock = GetTemplateChild("PART_PathTextBox") as TextBlock;
		ApplyPathEllipsis();
	}

	private static void OnTextChanged(DependencyObject d,
		DependencyPropertyChangedEventArgs args)
	{
		(d as PathEllipsisTextBlock)?.ApplyPathEllipsis();
	}

	private void ApplyPathEllipsis()
	{
		// TODO: adjust width if scroll bar shown?
		double _availableWidth = this.ActualWidth;

		if (_textBlock == null || string.IsNullOrEmpty(Text) || this.ActualWidth == 0)
		{
			return;
		}

		// Does the full path fit?
		if (MeasureStringWidth(Text) <= _availableWidth)
		{
			_textBlock.Text = this.Text;
			return;
		}

		// First check to see if "...\<filename>" fits.
		string filenameAndEllipsis = "..." + Path.DirectorySeparatorChar + _filename;
		double filenameWidth = MeasureStringWidth(filenameAndEllipsis);

		if (filenameWidth > _availableWidth)
		{
			// The filename suffix doesn't fit, so truncate it.
			// TODO: are there any edge cases here, such as if nothing fits at all?
			_textBlock.Text = TruncateText(_filename, _availableWidth);
			return;
		}

		// The filename suffix fits, so the next step is to add in the directory
		// portion of the path. Reduce the space taken up by the full filename suffix.
		_availableWidth -= filenameWidth;

		string truncatedDirectoryPath = TruncateText(_directoryPath, _availableWidth,
			"", false);

		_textBlock.Text = truncatedDirectoryPath + filenameAndEllipsis;
	}

	/// <summary>
	/// Attempts to shorten a string so that it fits within a specified width,
	/// optionally prepending ellipses to indicate truncation. It uses a binary
	/// search approach for efficiency.
	/// </summary>
	/// <param name="text">The string to truncate, e.g. "filename.txt".</param>
	/// <param name="availableWidth">The pixel width of the container within
	/// which to fit the string.</param>
	/// <param name="prefix">The prefix string to use for left-truncated text.
	/// "..." by default.
	/// <paramref name="truncateLeft"/>Whether to remove characters from the 
	/// left of the string (the default of true), or the right.</param>
	/// <returns>The longest string which fits within the available width.
	/// </returns>
	private string TruncateText(string text, double availableWidth,
		string prefix = "...", bool truncateLeft = true)
	{
		// If the prefix and text fit, return it without truncating.
		if (MeasureStringWidth(prefix + text) <= availableWidth)
		{
			return prefix + text;
		}

		// Subtract the pixel width of the prefix from the available width.
		// NB: this isn't exact, because of the space between the prefix and 
		// the rest of the text, but is a decent compromise. This is so we
		// don't have to allocate (prefix + text) strings each iteration.
		if (prefix.Length > 0)
		{
			availableWidth -= MeasureStringWidth(prefix);
		}

		// The 'good' condition boundary where this number of characters have been
		// tested to fit within the available width.
		int low = 0;
		// This is the 'bad' condition boundary, representing the first guess that
		// is too long to fit.
		int high = text.Length;

		// Converge on the highest value of low that fits. Exits when low and high
		// are adjacent.
		while (low < high - 1)
		{
			// Calculate the midpoint of low and high to test next.
			int mid = low + (high - low) / 2;
			// Create the candidate string.
			string candidate = truncateLeft ? text[^mid..] : text[0..mid];

			// If the candidate fits within the width, move the lower bound up,
			// else shift the higher bound down.
			if (MeasureStringWidth(candidate) <= availableWidth)
			{
				low = mid;
			}
			else
			{
				high = mid;
			}
		}

		// low now represents the last length verified to fit.
		return prefix + (truncateLeft ? text[^low..] : text[0..low]);
	}

	/// <summary>
	/// Measure the length of a piece of text. Uses a cache to retrieve
	/// previously-calculated widths.
	/// </summary>
	/// <param name="text">The text to measure.</param>
	/// <returns>The width of the text as would be rendered in the control.
	/// </returns>
	private double MeasureStringWidth(string text)
	{
		// TODO: clear cache if any font properties change.
		// TODO: should we limit the number of entries?
		// TODO: use hash instead of the full string?
		if (_stringToLengthMap.TryGetValue(text, out double width))
		{
			return width;
		}

		double textWidth =
			TextMeasurement.Instance(this._textBlock).MeasureText(text).Width;
		_stringToLengthMap.Add(text, textWidth);

		return textWidth;
	}
}

public static class ThrottleManager
{
	private static DispatcherTimer _timer;
	private static List<Action> _actions = new();

	static ThrottleManager()
	{
		_timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
		_timer.Tick += (sender, e) =>
		{
			_timer.Stop();
			foreach (var action in _actions)
			{
				action.Invoke();
			}
			_actions.Clear();
		};
	}

	public static void Throttle(Action updateAction)
	{
		if (!_actions.Contains(updateAction))
		{
			_actions.Add(updateAction);
		}

		_timer.Stop();
		_timer.Start();
	}
}
