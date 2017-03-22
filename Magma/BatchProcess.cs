using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MagmaC3.x360;

namespace MagmaC3
{
    public partial class BatchProcess : Form
    {
        private readonly MainForm mMainForm;
        private readonly List<string> FilesToConvert;
        private readonly List<string> ProblemFiles;
        private readonly List<string> SkippedFiles;
        private DateTime StartTime;
        private DateTime EndTime;
        private int TotalSongs;
        private readonly NemoTools Tools;
        private readonly bool DoReverseBatch;
        private readonly DTAParser Parser;

        public BatchProcess(MainForm form, List<string> files, bool reverse = false)
        {
            InitializeComponent();
            mMainForm = form;
            Tools = new NemoTools();
            Parser = new DTAParser();

            FilesToConvert = files;
            ProblemFiles = new List<string>();
            SkippedFiles = new List<string>();
            DoReverseBatch = reverse;
        }

        private void BatchProcess_Shown(object sender, EventArgs e)
        {
            lblFiles.Text = "Processing " + FilesToConvert.Count + (FilesToConvert.Count == 1 ? " file" : " files");
            picWorking.Visible = true;
            //Application.DoEvents();
            StartTime = DateTime.Now;
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (var file in FilesToConvert)
            {
                if (backgroundWorker1.CancellationPending) return;
                var replaced = 0;
                try
                {
                    var xFile = new STFSPackage(file);
                    if (!xFile.ParseSuccess)
                    {
                        ProblemFiles.Add(file);
                        continue;
                    }

                    var xent = xFile.GetFile("/songs/songs.dta");
                    if (xent == null)
                    {
                        xFile.CloseIO();
                        ProblemFiles.Add(file);
                        continue;
                    }

                    var dta1 = Application.StartupPath + "\\bin\\dta1.txt";
                    Tools.DeleteFile(dta1);

                    if (!xent.Extract(dta1))
                    {
                        xFile.CloseIO();
                        ProblemFiles.Add(file);
                        continue;
                    }

                    var dta2 = Application.StartupPath + "\\bin\\dta2.txt";
                    Tools.DeleteFile(dta2);

                    var sr = new StreamReader(dta1, Encoding.Default);
                    var sw = new StreamWriter(dta2, false, Encoding.Default);
                    while (sr.Peek() >= 0)
                    {
                        var line = sr.ReadLine();
                        if (string.IsNullOrEmpty(line.Trim())) continue;

                        if (line.Contains(";ORIG_ID="))
                        {
                            if (DoReverseBatch)
                            {
                                var id = line.Replace(";", "").Replace("ORIG_ID=", "").Trim();
                                sw.WriteLine("   ('song_id' " + id + ")");
                                sr.ReadLine();//skip the old song_id line
                                line = sr.ReadLine();
                                TotalSongs++;
                                replaced++;
                            }
                            else
                            {
                                sw.WriteLine(line);
                                line = sr.ReadLine();
                                sw.WriteLine(line);
                                line = sr.ReadLine();
                            }
                        }
                        else if (line.Contains("song_id") && !DoReverseBatch)
                        {
                            if (!Parser.IsNumericID(line))
                            {
                                line = ";ORIG_ID=" + Parser.GetSongID(line);
                                sw.WriteLine(line);
                                line = "   ('song_id' " + mMainForm.GetNumericID() + ")";
                                TotalSongs++;
                                replaced++;
                            }
                            else
                            {
                                SkippedFiles.Add(file);
                            }
                        } 

                        if (!string.IsNullOrEmpty(line.Trim()))
                        {
                            sw.WriteLine(line);
                        }
                    }
                    sr.Dispose();
                    sw.Dispose();

                    if (replaced == 0) //don't modify this CON file if nothing is edited in this DTA file
                    {
                        if (!SkippedFiles.Contains(file))
                        {
                            SkippedFiles.Add(file);
                        }
                        xFile.CloseIO();
                        continue;
                    }

                    if (backgroundWorker1.CancellationPending)
                    {
                        xFile.CloseIO();
                        return;
                    }
                    if (!xent.Replace(dta2))
                    {
                        xFile.CloseIO();
                        ProblemFiles.Add(file);
                        continue;
                    }

                    Tools.DeleteFile(dta1);
                    Tools.DeleteFile(dta2);

                    xFile.Header.MakeAnonymous();
                    xFile.Header.ThisType = PackageType.SavedGame;
                    var signature = new RSAParams(Application.StartupPath + "\\bin\\KV.bin");
                    xFile.RebuildPackage(signature);
                    xFile.FlushPackage(signature);
                    xFile.CloseIO();
                    Tools.UnlockCON(file);
                    Tools.SignCON(file);
                    xFile.CloseIO();
                }
                catch (Exception)
                {
                    ProblemFiles.Add(file);
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            picWorking.Visible = false;
            EndTime = DateTime.Now;

            if (DoReverseBatch)
            {
                if (TotalSongs == 0)
                {
                    MessageBox.Show("I couldn't restore any IDs in those files, sorry\nNo files were modified", "Batch restore complete", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    MessageBox.Show("I restored IDs for " + TotalSongs + (TotalSongs == 1 ? " song" : " songs") + " in " + FilesToConvert.Count +
                    " CON " + (FilesToConvert.Count == 1 ? "file" : "files") + "\n\nStart Time: " + StartTime + "\nEnd Time: " +
                    EndTime, "Batch restore complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                if (TotalSongs == 0)
                {
                    MessageBox.Show("I couldn't replace any IDs in those files, sorry\nNo files were modified\n\nSee the following message(s) for reasons why this process failed",
                        "Batch process complete", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    MessageBox.Show("Batch replaced IDs for " + TotalSongs + (TotalSongs == 1 ? " song" : " songs") + " in " + FilesToConvert.Count +
                    " CON " + (FilesToConvert.Count == 1 ? "file" : "files") + "\n\nStart Time: " + StartTime + "\nEnd Time: " +
                    EndTime + "\n\nEnjoy cache-wipe-proof customs!", "Batch process complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (SkippedFiles.Count > 0)
                {
                    var skipped = SkippedFiles.Aggregate("", (current, skip) => current + "\n" + Path.GetFileName(skip));
                    MessageBox.Show("The following CON " + (SkippedFiles.Count == 1 ? "file" : "files") +
                        " already had numeric IDs, so I skipped " + (SkippedFiles.Count == 1 ? "it" : "them") + ". You might want to " +
                         "check the songs.dta files to make sure:\n" + skipped, "Batch process complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                if (ProblemFiles.Count > 0)
                {
                    var errors = ProblemFiles.Aggregate("", (current, err) => current + "\n" + Path.GetFileName(err));
                    MessageBox.Show("The following CON " + (ProblemFiles.Count == 1 ? "file" : "files") +
                        " gave me errors and I could not replace the song IDs correctly, try doing it manually:\n" +
                        errors + "\n\nI already copied this to your clipboard, just paste it on a text document so you can save this list",
                        "Batch process complete", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Clipboard.SetText(errors);
                }
            }
            
            mMainForm.isBusy = false;
            Dispose();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            lblStatus.Invoke(new MethodInvoker(() => lblStatus.Text = "CANCELLING"));
        }
    }
}
