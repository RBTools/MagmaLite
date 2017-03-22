using System.ComponentModel;
using System.Windows.Forms;

namespace MagmaC3
{
    partial class BuildForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components;
        private TextBox TextBoxBuild;
        private Button ButtonClose;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BuildForm));
            this.TextBoxBuild = new System.Windows.Forms.TextBox();
            this.ButtonClose = new System.Windows.Forms.Button();
            this.btnVisualize = new System.Windows.Forms.Button();
            this.btnC3Tools = new System.Windows.Forms.Button();
            this.btnClipboard = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // TextBoxBuild
            // 
            this.TextBoxBuild.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxBuild.Location = new System.Drawing.Point(12, 12);
            this.TextBoxBuild.MaxLength = 1048576;
            this.TextBoxBuild.Multiline = true;
            this.TextBoxBuild.Name = "TextBoxBuild";
            this.TextBoxBuild.ReadOnly = true;
            this.TextBoxBuild.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextBoxBuild.Size = new System.Drawing.Size(466, 270);
            this.TextBoxBuild.TabIndex = 0;
            this.TextBoxBuild.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBoxBuild_KeyDown);
            // 
            // ButtonClose
            // 
            this.ButtonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonClose.AutoSize = true;
            this.ButtonClose.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ButtonClose.BackColor = System.Drawing.Color.Black;
            this.ButtonClose.BackgroundImage = global::MagmaC3.Properties.Resources.button_bg_large;
            this.ButtonClose.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ButtonClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ButtonClose.FlatAppearance.BorderSize = 0;
            this.ButtonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.ButtonClose.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.ButtonClose.Location = new System.Drawing.Point(413, 290);
            this.ButtonClose.Name = "ButtonClose";
            this.ButtonClose.Size = new System.Drawing.Size(65, 23);
            this.ButtonClose.TabIndex = 1;
            this.ButtonClose.Text = "&CANCEL";
            this.ButtonClose.UseVisualStyleBackColor = false;
            this.ButtonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // btnVisualize
            // 
            this.btnVisualize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnVisualize.AutoSize = true;
            this.btnVisualize.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnVisualize.BackColor = System.Drawing.Color.Black;
            this.btnVisualize.BackgroundImage = global::MagmaC3.Properties.Resources.button_bg_large;
            this.btnVisualize.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnVisualize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVisualize.FlatAppearance.BorderSize = 0;
            this.btnVisualize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVisualize.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnVisualize.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnVisualize.Location = new System.Drawing.Point(138, 290);
            this.btnVisualize.Name = "btnVisualize";
            this.btnVisualize.Size = new System.Drawing.Size(119, 23);
            this.btnVisualize.TabIndex = 2;
            this.btnVisualize.Text = "Send to &Visualizer";
            this.btnVisualize.UseVisualStyleBackColor = false;
            this.btnVisualize.Visible = false;
            this.btnVisualize.Click += new System.EventHandler(this.btnVisualize_Click);
            // 
            // btnC3Tools
            // 
            this.btnC3Tools.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnC3Tools.AutoSize = true;
            this.btnC3Tools.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnC3Tools.BackColor = System.Drawing.Color.Black;
            this.btnC3Tools.BackgroundImage = global::MagmaC3.Properties.Resources.button_bg_large;
            this.btnC3Tools.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnC3Tools.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnC3Tools.FlatAppearance.BorderSize = 0;
            this.btnC3Tools.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnC3Tools.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnC3Tools.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnC3Tools.Location = new System.Drawing.Point(288, 290);
            this.btnC3Tools.Name = "btnC3Tools";
            this.btnC3Tools.Size = new System.Drawing.Size(118, 23);
            this.btnC3Tools.TabIndex = 3;
            this.btnC3Tools.Text = "Send to RBto&USB";
            this.btnC3Tools.UseVisualStyleBackColor = false;
            this.btnC3Tools.Visible = false;
            this.btnC3Tools.Click += new System.EventHandler(this.btnC3Tools_Click);
            // 
            // btnClipboard
            // 
            this.btnClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClipboard.AutoSize = true;
            this.btnClipboard.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnClipboard.BackColor = System.Drawing.Color.Black;
            this.btnClipboard.BackgroundImage = global::MagmaC3.Properties.Resources.button_bg_large;
            this.btnClipboard.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnClipboard.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClipboard.FlatAppearance.BorderSize = 0;
            this.btnClipboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClipboard.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClipboard.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.btnClipboard.Location = new System.Drawing.Point(12, 290);
            this.btnClipboard.Name = "btnClipboard";
            this.btnClipboard.Size = new System.Drawing.Size(117, 23);
            this.btnClipboard.TabIndex = 4;
            this.btnClipboard.Text = "Copy to Clipboard";
            this.btnClipboard.UseVisualStyleBackColor = false;
            this.btnClipboard.Visible = false;
            this.btnClipboard.Click += new System.EventHandler(this.btnClipboard_Click);
            // 
            // BuildForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.ClientSize = new System.Drawing.Size(490, 323);
            this.Controls.Add(this.btnClipboard);
            this.Controls.Add(this.btnC3Tools);
            this.Controls.Add(this.btnVisualize);
            this.Controls.Add(this.ButtonClose);
            this.Controls.Add(this.TextBoxBuild);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(200, 200);
            this.Name = "BuildForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Building your song...";
            this.Shown += new System.EventHandler(this.BuildForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btnVisualize;
        private Button btnC3Tools;
        private Button btnClipboard;

    }
}