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

	public partial class EnhancedListBox : ListBox
	{
		private int _toolTipIndex = -1;
		private System.Windows.Forms.Timer _toolTipTimer;
		private ToolTipPanel _toolTipPanel;
		private ToolTipLabel _toolTipLabel;

		public EnhancedListBox()
		{
			InitializeComponent();
			InitializeToolTipPanel();
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
			this.DrawMode = DrawMode.OwnerDrawFixed;
		}

		private void InitializeToolTipPanel()
		{
			_toolTipLabel = new ToolTipLabel
			{
				AutoSize = true
			};

			_toolTipPanel = new ToolTipPanel
			{
				Visible = false,
				BackColor = SystemColors.Info,
				Controls = { _toolTipLabel }
			};

			this.Controls.Add(_toolTipPanel);

			_toolTipTimer = new System.Windows.Forms.Timer
			{
				Interval = 100
			};

			_toolTipTimer.Tick += (sender, e) => HideToolTip();
		}

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

			string path = this.Items[e.Index].ToString() ?? "";
			var flags = TextFormatFlags.Default | TextFormatFlags.PathEllipsis;

			TextRenderer.DrawText(e.Graphics, path, e.Font, e.Bounds,
				e.ForeColor, e.BackColor, flags);

			e.DrawFocusRectangle();
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

		private void HandleToolTip(MouseEventArgs e)
		{
			int index = this.IndexFromPoint(e.Location);
			if (index >= 0)
			{
				if (index == _toolTipIndex) { return; }

				var item = this.Items[index] as FileInfo;
				if (item != null)
				{
					if (IsResultTruncated(item))
					{
						_toolTipPanel.Parent = this.FindForm();
						var itemPos = this.GetItemRectangle(index).Location;
						_toolTipLabel.Text = this.GetItemText(item);
						_toolTipPanel.Size = _toolTipLabel.Size;
						Point screenPos = this.PointToScreen(itemPos);
						_toolTipPanel.Location = _toolTipPanel.Parent.PointToClient(screenPos);
						_toolTipPanel.BringToFront();
						_toolTipPanel.Show();
						_toolTipIndex = index;

						_toolTipTimer.Start();
						return;
					}
				}
			}

			_toolTipTimer.Stop();
			_toolTipPanel.Hide();
			_toolTipIndex = -1;
		}

		/// <summary>
		/// Determine whether an item's display text is longer than the
		/// control's width and will be truncated if displayed as-is.
		/// </summary>
		/// <param name="item">The ListBox item to measure.</param>
		/// <returns>Whether the item will be truncated on display.</returns>
		private bool IsResultTruncated(object item)
		{
			// Measure and return whether the text is truncated.
			var textSize = TextRenderer.MeasureText(this.GetItemText(item), this.Font);
			return textSize.Width > this.Width;
		}
	}
}
