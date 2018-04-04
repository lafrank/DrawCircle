using System;
using System.Drawing;
using System.Windows.Forms;

namespace DrawCircle
{
	public partial class CircleForm : Form
	{
		Point circleCenter;
		Point mousePosition;

		public CircleForm()
		{
			InitializeComponent();
		}

		private void CircleForm_MouseClick(object sender, MouseEventArgs e)
		{
			circleCenter = e.Location;
		}

		private void CircleForm_MouseMove(object sender, MouseEventArgs e)
		{
			mousePosition = e.Location;
			Invalidate();
		}

		private void CircleForm_Paint(object sender, PaintEventArgs e)
		{
			int radius = (int)Math.Sqrt(Math.Pow(mousePosition.X - circleCenter.X, 2) + Math.Pow(mousePosition.Y - circleCenter.Y, 2));
			Graphics g = e.Graphics;
			Rectangle r = new Rectangle(circleCenter.X - radius, circleCenter.Y - radius, 2 * radius, 2 * radius);
			Pen p = Pens.Green;
			g.DrawEllipse(p, r);
			g.DrawLine(p, circleCenter, mousePosition);
		}
	}
}
