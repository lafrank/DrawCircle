using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DrawCircle
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void alignCircularToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ArcAligner ca = new ArcAligner(this, 5);
			ca.OnAlignmentFinished += Ca_OnAlignmentFinished;
		}

		private void Ca_OnAlignmentFinished(ArcAligner sender)
		{
			MessageBox.Show("Result : " + sender.AlignmentResult, "Alignment finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}
