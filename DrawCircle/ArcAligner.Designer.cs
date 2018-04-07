namespace DrawCircle
{
	partial class ArcAligner
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			// 
			// ArcAligner
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(511, 288);
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "ArcAligner";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Form1";
			this.TransparencyKey = System.Drawing.Color.Black;
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CircleForm_FormClosed);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.CircleForm_Paint);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CircleForm_KeyDown);
			this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.CircleForm_MouseClick);
			this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CircleForm_MouseMove);
			this.Move += new System.EventHandler(this.CircleForm_Move);
			this.Resize += new System.EventHandler(this.CircleForm_Resize);
			this.ResumeLayout(false);

		}

		#endregion
	}
}

