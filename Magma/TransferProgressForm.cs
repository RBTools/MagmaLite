using System;
using System.Windows.Forms;

namespace MagmaC3
{
    public partial class TransferProgressForm : Form
    {
        private readonly Form mParent;
       
        public TransferProgressForm(Form parent)
        {
            mParent = parent;
            InitializeComponent();
        }
        
        public void SetMIDIExportMode()
        {
            Text = "Exporting MIDI file";
            LabelTitle.Text = "Exporting...";
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.Value = 0;
            transferCancelButton.Enabled = true;
        }
        
        public void PauseMarquee()
        {
            progressBar1.Style = ProgressBarStyle.Continuous;
            transferCancelButton.Enabled = false;
        }

        public void ResumeMarquee()
        {
            progressBar1.Style = ProgressBarStyle.Marquee;
            transferCancelButton.Enabled = true;
        }

        private void transferCancelButton_Click(object sender, EventArgs e)
        {
            if (mParent.GetType() == typeof(MainForm))
                ((MainForm)mParent).CancelMIDIExport();
            transferCancelButton.Enabled = false;
        }

        private void TransferProgressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            transferCancelButton_Click(transferCancelButton, new EventArgs());
            if (e.CloseReason != CloseReason.UserClosing)
                return;
            e.Cancel = true;
        }
    }
}
