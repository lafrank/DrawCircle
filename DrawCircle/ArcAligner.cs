using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawCircle
{
	/// <summary>
	/// This Form is a special form that can be used as a transparent overlay over another form to select alignment points over an Arc
	/// </summary>
	public partial class ArcAligner
	{
		#region internals
		internal enum AlignerState { Init, Centering, Sizing, SetWedge }
		#endregion

		#region Fields
		private Point circleCenter;
		private Point mousePosition;
		private int radius = -1;
		private AlignerState state = AlignerState.Init;
		private Point[] _placeholders = new Point[0];
		private Control _master;
		#endregion

		#region Properties
		/// <summary>
		/// Returns the result of alignment operation
		/// </summary>
		public AlignmentResult AlignmentResult { get; private set; } = AlignmentResult.Canceled;

		/// <summary>
		/// Gets or sets the number of placeholders to distribute on the arc
		/// </summary>
		public int NumberOfPlaceholders { get; private set; } = 0;

		/// <summary>
		/// Get or sets the Color of the circle guide
		/// </summary>
		public Color GuideCircleColor { get; set; } = Color.Silver;

		/// <summary>
		/// Get or sets the Color of the crosses on the circle guide
		/// </summary>
		public Color GuideCrossColor { get; set; } = Color.Red;

		/// <summary>
		/// Determines if hint texts are displayed
		/// </summary>
		public bool ShowHints { get; set; } = true;

		/// <summary>
		/// Get or sets the Color of the guidance message
		/// </summary>
		public Color HintTextColor { get; set; } = Color.DarkGreen;

		/// <summary>
		/// Get or sets the Size of the crosses on the circle guide
		/// </summary>
		public int GuideCrossSize { get; set; } = 2;

		/// <summary>
		/// Returns the calculated positions
		/// </summary>
		public Point[] CalculatedPositions
		{
			get
			{
				Point[] result = new Point[_placeholders.Length];
				Array.Copy(_placeholders, result, _placeholders.Length);
				return result;
			}
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new instance of CircularAligner
		/// </summary>
		public ArcAligner()
		{
		}
		#endregion

		#region Public members
		public void StartAlignment(Control Master, int PlaceholderNumber)
		{
			if (Master == null) throw new ArgumentException("Underlay control must not be null");
			if (PlaceholderNumber <= 0) throw new ArgumentException("PlaceholderNumber value must a non-negative number greater than zero.");
			if (_master == null)
			{
				NumberOfPlaceholders = PlaceholderNumber;
				RegisterMaster(Master);
				state = AlignerState.Centering;
				_master.Cursor = Cursors.Cross;
				_master.Invalidate();
			}
		}

		public void CancelAlignment()
		{
			if (_master != null)
			{
				AlignmentResult = AlignmentResult.Canceled;
				Control oldMaster = _master;
				UnregisterMaster();
				oldMaster?.Invalidate();
			}
		}
		#endregion

		#region Private methods
		private void Close()
		{
			if (_master != null) _master.Cursor = Cursors.Default;
			Control oldMaster = _master;
			UnregisterMaster();
			oldMaster?.Invalidate();
			OnAlignmentFinished?.Invoke(this);
		}

		private void DrawCross(Point p, Graphics g, Color crossColor, int crossSize = 2, int lineWidth = 1)
		{
			Point a = new Point(p.X - crossSize, p.Y - crossSize);
			Point b = new Point(p.X + crossSize, p.Y - crossSize);
			Point c = new Point(p.X - crossSize, p.Y + crossSize);
			Point d = new Point(p.X + crossSize, p.Y + crossSize);
			using (Brush lineBrush = new SolidBrush(crossColor))
			{
				using (Pen linePen = new Pen(lineBrush, lineWidth))
				{
					g.DrawLine(linePen, a, d);
					g.DrawLine(linePen, b, c);
				}
			}
		}

		private void DrawTextInCenter(string message, Graphics g)
		{
			SizeF s = TextRenderer.MeasureText(message, _master.Font);
			Point center = new Point(_master.Width / 2, _master.Height / 2);
			Point textLocation = new Point(center.X - (int)(s.Width / 2), center.Y - (int)(s.Height / 2));
			using (Brush textBrush = new SolidBrush(HintTextColor))
			{
				g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
				TextRenderer.DrawText(g, message, _master.Font, textLocation, HintTextColor);
			}
		}

		private void RegisterMaster(Control master)
		{
			this._master = master;
			//
			if (_master != null)
			{
				_master.Paint += ArcAligner_Paint;
				_master.MouseMove += ArcAligner_MouseMove;
				_master.MouseClick += ArcAligner_MouseClick;
				_master.KeyDown += ArcAligner_KeyDown;
			}
		}

		private void UnregisterMaster()
		{
			if (_master != null)
			{
				_master.Paint -= ArcAligner_Paint;
				_master.MouseMove -= ArcAligner_MouseMove;
				_master.MouseClick -= ArcAligner_MouseClick;
				_master.KeyDown -= ArcAligner_KeyDown;
				//
				this._master = null;
			}
		}
		#endregion

		#region Events
		public event AlignementFinished OnAlignmentFinished;
		#endregion

		#region Event handlers
		private void ArcAligner_MouseClick(object sender, MouseEventArgs e)
		{
			bool reset = Control.ModifierKeys == Keys.Shift;
			if (reset)
			{
				state = AlignerState.Centering;
				_master.Cursor = Cursors.Cross;
				_master.Invalidate();
			}
			else
			{
				switch (state)
				{
					case AlignerState.Init:
						{
							state = AlignerState.Centering;
							_master.Cursor = Cursors.Cross;
							_master.Invalidate();
							break;
						}
					case AlignerState.Centering:
						{
							state = AlignerState.Sizing;
							_master.Cursor = Cursors.SizeAll;
							circleCenter = e.Location;
							_master.Invalidate();
							break;
						}
					case AlignerState.Sizing:
						{
							state = AlignerState.SetWedge;
							_master.Cursor = Cursors.Hand;
							_master.Invalidate();
							break;
						}
					case AlignerState.SetWedge:
						{
							AlignmentResult = AlignmentResult.Confirmed;
							Close();
							break;
						}
				}
			}

		}

		private void ArcAligner_MouseMove(object sender, MouseEventArgs e)
		{
			switch (state)
			{
				case AlignerState.Sizing:
				case AlignerState.SetWedge:
					{
						mousePosition = e.Location;
						_master.Invalidate();
						break;
					}
			}
		}

		private void ArcAligner_Paint(object sender, PaintEventArgs e)
		{
			switch (state)
			{
				case AlignerState.Centering:
					{
						if (ShowHints) DrawTextInCenter("Click to set circle center, Shit-click to restart, ESC to cancel", e.Graphics);
						break;
					}
				case AlignerState.Sizing:
					{
						if (ShowHints) DrawTextInCenter("Click to accept circle size, Shift-click to restart, ESC to cancel", e.Graphics);
						radius = (int)Math.Sqrt(Math.Pow(mousePosition.X - circleCenter.X, 2) + Math.Pow(mousePosition.Y - circleCenter.Y, 2));
						if (radius > 0)
						{
							Graphics g = e.Graphics;
							g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
							Rectangle r = new Rectangle(circleCenter.X - radius, circleCenter.Y - radius, 2 * radius, 2 * radius);
							using (Brush lineBrush = new SolidBrush(GuideCircleColor))
							{
								using (Pen linePen = new Pen(lineBrush, 1))
								{
									g.DrawEllipse(linePen, r);
								}
							}
						}
						break;
					}
				case AlignerState.SetWedge:
					{
						if (radius > 0)
						{
							if (ShowHints) DrawTextInCenter("Click to accept positions, Shift-click to restart, ESC to cancel", e.Graphics);

							#region Calculate angles
							double mouseDist = (int)Math.Sqrt(Math.Pow(mousePosition.X - circleCenter.X, 2) + Math.Pow(mousePosition.Y - circleCenter.Y, 2));
							double dX = Math.Abs(mousePosition.X - circleCenter.X);
							double dY = Math.Abs(mousePosition.Y - circleCenter.Y);
							double mouseAngle = -1;
							if (mousePosition.X >= circleCenter.X)
							{
								if (mousePosition.Y >= circleCenter.Y)
								{
									mouseAngle = Math.Asin((double)(dY / mouseDist));
								}
								else
								{
									mouseAngle = 2 * Math.PI - Math.Asin((double)(dY / mouseDist));
								}
							}
							else
							{
								if (mousePosition.Y >= circleCenter.Y)
								{
									mouseAngle = Math.PI - Math.Asin((double)(dY / mouseDist));
								}
								else
								{
									mouseAngle = Math.PI + Math.Asin((double)(dY / mouseDist));
								}
							}
							mouseAngle = mouseAngle * (180 / Math.PI);
							double sweepAngle = Math.Max(0, 360 - Math.Abs(mouseDist - radius));
							double startAngle = mouseAngle - sweepAngle / 2;
							#endregion

							#region Draw guide arc
							Graphics g = e.Graphics;
							//e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
							Rectangle r = new Rectangle(circleCenter.X - radius, circleCenter.Y - radius, 2 * radius, 2 * radius);
							using (Brush lineBrush = new SolidBrush(GuideCircleColor))
							{
								using (Pen linePen = new Pen(lineBrush, 1))
								{
									g.DrawArc(linePen, r, (float)startAngle, (float)sweepAngle);
								}
							}
							#endregion

							#region Draw placeholder crosses
							double angleStep = NumberOfPlaceholders == 1 ? sweepAngle : sweepAngle / (NumberOfPlaceholders - 1);
							_placeholders = new Point[NumberOfPlaceholders];
							for (int i = 0; i < NumberOfPlaceholders; i++)
							{
								double pointAngle = ((90 + startAngle + i * angleStep) / 360) * 2 * Math.PI;
								double pdX = radius * Math.Sin(pointAngle);
								double pdY = -1 * radius * Math.Cos(pointAngle);
								_placeholders[i] = new Point((int)(circleCenter.X + pdX), (int)(circleCenter.Y + pdY));
								DrawCross(_placeholders[i], g, GuideCrossColor, GuideCrossSize);
							}
							#endregion
						}
						break;
					}
			}
		}

		private void ArcAligner_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Escape)
			{
				AlignmentResult = AlignmentResult.Canceled;
				Close();
			}
		}

		#endregion
	}

	public enum AlignmentResult { Canceled, Confirmed }

	public delegate void AlignementFinished(ArcAligner sender);
}
