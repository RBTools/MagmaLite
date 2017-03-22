using System;
using System.IO;
using System.Windows.Forms;

namespace MagmaC3
{
    public partial class FileSelector : Form
    {
        private int TimeLeft;
        private readonly string folder;
        private readonly string extension;
        private readonly NemoTools Tools;
        private readonly BuildForm parent;
        private bool starting;

        public FileSelector(BuildForm xParent, string fold, string ext)
        {
            InitializeComponent();
            Tools = new NemoTools();
            parent = xParent;
            folder = fold;
            extension = ext;
            starting = true;
        }

        private void lstFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            var file = folder + lstFiles.SelectedItem;
            if (!File.Exists(file))
            {
                MessageBox.Show("Couldn't find that file!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var fileSize = new FileInfo(file);
            var info = "";

            if (extension == ".png_xbox")
            {
                var png = Application.StartupPath + "\\bin\\temp.png";
                Tools.DeleteFile(png);
                if (!Tools.ConvertRBImage(file, png)) return;
                
                var img = Tools.NemoLoadImage(png);
                picImage.Image = img;
                info = "Dimensions: " + img.Height + "x" + img.Width;
            }

            if (extension == ".mogg")
            {
                var bytes = File.ReadAllBytes(file);

                info = "Encrypted? " + (bytes[0] == 0x0A ? "No" : "Yes (0x" + bytes[0].ToString("X2") + ")");
            }

            var size = "Size: ";
            if (fileSize.Length >= 1048576)
            {
                size = size + (Math.Round((double) fileSize.Length/1048576, 2)) + " MB";
            }
            else
            {
                size = size + (Math.Round((double)fileSize.Length / 1024, 2)) + " KB";
            }
            
            lblInformation.Text = size + (string.IsNullOrEmpty(info) ? "" : "   |   " + info);
            
            if (starting) return;

            timer1.Enabled = false;
            lblTimer.Visible = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeLeft--;

            if (TimeLeft == 0)
            {
                Close();
            }
            lblTimer.Text = "Closing in " + TimeLeft + (TimeLeft == 1 ? " second..." : " seconds...");
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FileSelector_Shown(object sender, EventArgs e)
        {
            if (extension != ".png_xbox")
            {
                Width = 241;
                picImage.Visible = false;
            }

            lblDescription.Text = "Found multiple override " + extension + " files.\nPlease select which one I should use below:";

            var files = Directory.GetFiles(folder, "*" + extension);
            foreach (var file in files)
            {
                lstFiles.Items.Add(Path.GetFileName(file));
            }

            lstFiles.SelectedIndex = 0;
            lblTimer.Text = "Closing in 15 seconds...";
            TimeLeft = 15;
            starting = false;
            timer1.Enabled = true;
        }

        private void FileSelector_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (lstFiles.SelectedIndex < 0)
            {
                lstFiles.SelectedItem = 0;
            }

            parent.ChosenFile = folder + lstFiles.SelectedItem;
            Tools.DeleteFile(Application.StartupPath + "\\bin\\temp.png");
        }
    }
}
