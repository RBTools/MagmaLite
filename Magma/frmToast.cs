using System;
using System.Drawing;
using System.Windows.Forms;

namespace MagmaC3
{
    public partial class frmToast : Form
    {
        public int timerTick;

        public frmToast(string message, Point loc)
        {
            InitializeComponent();

            LabelToast.Text = message;

            //Application.DoEvents();

            Location = new Point(loc.X,loc.Y - 40);

            timer1.Enabled = true;
        }

        public override sealed Size MaximumSize
        {
            get { return base.MaximumSize; }
            set { base.MaximumSize = value; }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timerTick++;

            if (timerTick <= 10) return;
            timer1.Interval = 50;
            Opacity = Opacity - 0.20;

            if (Opacity == 0.0)
            {
                Close();
            }
        }
    }
}
