namespace Searcher.Controls
{
	// Both panel and label controls are 'click-through', which requires
	// overriding WndProc.

	public class ToolTipPanel : Panel
	{
		protected override void WndProc(ref Message m)
		{
			const int WM_NCHITTEST = 0x84;
			const int HTTRANSPARENT = -1;

			if (m.Msg == WM_NCHITTEST)
			{
				m.Result = (IntPtr)HTTRANSPARENT;
			}
			else
			{
				base.WndProc(ref m);
			}
		}
	}

	public class ToolTipLabel : Label
	{
		protected override void WndProc(ref Message m)
		{
			const int WM_NCHITTEST = 0x84;
			const int HTTRANSPARENT = -1;

			if (m.Msg == WM_NCHITTEST)
			{
				m.Result = (IntPtr)HTTRANSPARENT;
			}
			else
			{
				base.WndProc(ref m);
			}
		}
	}

	/// <summary>
	/// A <see cref="ListBox"/>-derived control which shortens over-long
	/// items and displays the full text in a toolTip when the item is
	/// hovered-over.
	/// </summary>
	public partial class EnhancedListBox : ListBox
	{
		private Color _toolTipBackColor = SystemColors.Info;
		private Color _toolTipForeColor = SystemColors.InfoText;
		private Font _toolTipFont;

		/// <summary>
		/// Get or set the background color of the toolTip.
		/// </summary>
		public Color ToolTipBackColor
		{
			get => _toolTipBackColor;
			set
			{
				_toolTipBackColor = value;
				if (_toolTipPanel != null)
				{
					_toolTipPanel.BackColor = value;
				}
			}
		}

		/// <summary>
		/// Get or set the foreground color of the toolTip.
		/// </summary>
		public Color ToolTipForeColor
		{
			get => _toolTipForeColor;
			set
			{
				_toolTipForeColor = value;
				if (_toolTipLabel != null)
				{
					_toolTipLabel.ForeColor = value;
				}
			}
		}

		/// <summary>
		/// Get or set the <see cref="Font"/> used for the toolTip.
		/// </summary>
		public Font ToolTipFont
		{
			get => _toolTipFont ?? this.Font;
			set
			{
				_toolTipFont = value;
				if (_toolTipLabel != null)
				{
					_toolTipLabel.Font = _toolTipFont;
				}
			}
		}

		/// <summary>
		/// The index of the item for which a toolTip is currently showing.
		/// </summary>
		private int _toolTipIndex = -1;
		/// <summary>
		/// A timer for handling hiding the toolTip when the user navigates
		/// away from the control.
		/// </summary>
		private System.Windows.Forms.Timer _toolTipTimer;
		/// <summary>
		/// The <see cref="ToolTipPanel"/> which houses the toolTip.
		/// </summary>
		private ToolTipPanel _toolTipPanel;
		/// <summary>
		/// The <see cref="ToolTipLabel"/> which handles displaying the 
		/// toolTip text.
		/// </summary>
		private ToolTipLabel _toolTipLabel;
		/// <summary>
		/// Records the current width of the display bounds for the ListBox.
		/// Used when calculating whether an item should be truncated.
		/// </summary>
		private int _itemWidth = int.MaxValue;

		public EnhancedListBox()
		{
			InitializeComponent();
			InitializeToolTipControls();
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
			this.DrawMode = DrawMode.OwnerDrawFixed;
		}

		private void InitializeToolTipControls()
		{
			_toolTipLabel = new ToolTipLabel
			{
				AutoSize = true,
				ForeColor = this.ToolTipForeColor,
				Font = this.ToolTipFont,
			};

			_toolTipPanel = new ToolTipPanel
			{
				Visible = false,
				BackColor = this.ToolTipBackColor,
				Controls = { _toolTipLabel }
			};

			this.Controls.Add(_toolTipPanel);

			_toolTipTimer = new System.Windows.Forms.Timer
			{
				Interval = 100
			};

			_toolTipTimer.Tick += (sender, e) => HideToolTip();
		}

		/// <summary>
		/// Hide the toolTip if the mouse is no longer over the ListBox and
		/// also isn't over the toolTip itself.
		/// </summary>
		private void HideToolTip()
		{
			var clientPos = this.PointToClient(MousePosition);
			var toolTipPos = _toolTipPanel.PointToClient(MousePosition);
			if (!this.DisplayRectangle.Contains(clientPos) &&
				!_toolTipPanel.DisplayRectangle.Contains(toolTipPos))
			{
				_toolTipPanel.Hide();
				_toolTipIndex = -1;
			}
		}

		protected override void OnDrawItem(DrawItemEventArgs e)
		{
			if (e.Index < 0 || e.Index >= this.Items.Count) { return; }

			e.DrawBackground();

			// Draw the item with ellipses inserted if needed to make it fit.
			string itemString = this.Items[e.Index].ToString() ?? "";
			var flags = TextFormatFlags.Default | TextFormatFlags.PathEllipsis;
			TextRenderer.DrawText(e.Graphics, itemString, e.Font, e.Bounds,
				e.ForeColor, e.BackColor, flags);

			e.DrawFocusRectangle();

			// Save the width of the bounding rectangle. We compare against
			// this in IsResultTruncated() to determine whether an item needs a
			// tooltip.
			_itemWidth = e.Bounds.Width;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			HandleToolTip(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			HideToolTip();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			_itemWidth = int.MaxValue;
		}

		private void HandleToolTip(MouseEventArgs e)
		{
			int index = this.IndexFromPoint(e.Location);
			if (index != ListBox.NoMatches)
			{
				// Don't redraw the currently-showing toolTip. Prevents flicker.
				if (index == _toolTipIndex) { return; }

				var item = this.Items[index];
				if (item != null && IsResultTruncated(item))
				{
					_toolTipPanel.Parent = this.FindForm();
					var itemPos = this.GetItemRectangle(index).Location;
					_toolTipLabel.Text = this.GetItemText(item);
					_toolTipPanel.Size = _toolTipLabel.Size;
					Point screenPos = this.PointToScreen(itemPos);
					// TODO: reposition based on padding and margin?
					_toolTipPanel.Location = _toolTipPanel.Parent.PointToClient(screenPos);
					_toolTipPanel.BringToFront();
					_toolTipPanel.Show();
					_toolTipIndex = index;

					// Ensure we always hide the control, even if the user has
					// navigated elsewhere.
					_toolTipTimer.Start();
					return;
				}
			}

			// Hide the tooltip and reset the tracked item if no item is under the
			// pointer.
			_toolTipTimer.Stop();
			_toolTipPanel.Hide();
			_toolTipIndex = -1;
		}

		/// <summary>
		/// Determine whether an item's display text is longer than the
		/// available width and would be truncated if displayed.
		/// </summary>
		/// <param name="item">The ListBox item to measure.</param>
		/// <returns>Whether the item will be truncated on display.</returns>
		private bool IsResultTruncated(object item) =>
			TextRenderer.MeasureText(this.GetItemText(item), this.Font).Width > _itemWidth;
	}
}
