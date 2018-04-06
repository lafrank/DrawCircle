using System;
using System.Drawing;
using System.Windows.Forms;

namespace DrawCircle
{
	public partial class CircleForm : Form
	{
		Point circleCenter;
		int radius = -1;
		Point mousePosition;
		int clickCount = 0;
		CircleState state = CircleState.Centering;

		public CircleForm()
		{
			InitializeComponent();
			SetStatusText();
		}

		private void SetStatusText()
		{
			switch (state)
			{
				case CircleState.Init:
					{
						tslStatus.Text = "Please click to start operation";
						break;
					}
				case CircleState.Centering:
					{
						tslStatus.Text = "Please click to select circle center";
						break;
					}
				case CircleState.Sizing:
					{
						tslStatus.Text = "Set circle size by moving the mouse, and click to set actual size.";
						break;
					}
				case CircleState.SetWedge:
					{
						tslStatus.Text = "Select desired wedge size by moving the mouse, and click to set actual size.";
						break;
					}
			}
		}

		private void CircleForm_MouseClick(object sender, MouseEventArgs e)
		{
			clickCount++;
			switch (state)
			{
				case CircleState.Init:
					{
						state = CircleState.Centering;
						break;
					}
				case CircleState.Centering:
					{
						state = CircleState.Sizing;
						circleCenter = e.Location;
						break;
					}
				case CircleState.Sizing:
					{
						state = CircleState.SetWedge;
						break;
					}
				case CircleState.SetWedge:
					{
						state = CircleState.Init;
						break;
					}
			}
			SetStatusText();
		}

		private void CircleForm_MouseMove(object sender, MouseEventArgs e)
		{
			switch (state)
			{
				case CircleState.Init:
					{
						break;
					}
				case CircleState.Centering:
					{
						break;
					}
				case CircleState.Sizing:
					{
						mousePosition = e.Location;
						Invalidate();
						break;
					}
				case CircleState.SetWedge:
					{
						mousePosition = e.Location;
						Invalidate();
						break;
					}
			}
			//mousePosition = e.Location;
		}

		private void CircleForm_Paint(object sender, PaintEventArgs e)
		{
			switch (state)
			{
				case CircleState.Init:
					{
						break;
					}
				case CircleState.Centering:
					{
						break;
					}
				case CircleState.Sizing:
					{
						radius = (int)Math.Sqrt(Math.Pow(mousePosition.X - circleCenter.X, 2) + Math.Pow(mousePosition.Y - circleCenter.Y, 2));
						if (radius > 0)
						{
							Graphics g = e.Graphics;
							Rectangle r = new Rectangle(circleCenter.X - radius, circleCenter.Y - radius, 2 * radius, 2 * radius);
							g.DrawEllipse(Pens.Green, r);
						}
						break;
					}
				case CircleState.SetWedge:
					{
						if (radius > 0)
						{
							Graphics g = e.Graphics;
							Rectangle r = new Rectangle(circleCenter.X - radius, circleCenter.Y - radius, 2 * radius, 2 * radius);
							g.DrawEllipse(Pens.Silver, r);
							g.DrawLine(Pens.DarkOrange, circleCenter, mousePosition);
						}
						break;
					}
			}

		}
	}

	public enum CircleState { Init, Centering, Sizing, SetWedge }
}
