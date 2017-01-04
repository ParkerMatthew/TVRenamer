using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace File_Manager {
    public partial class MKVPropEdit : Form {
        MainForm Main;
        OpenFileDialog openFolder;
        Settings sharedSettings;
        string configPath;
        public MKVPropEdit(Settings settings, string sharedInstallPath, MainForm mainform) {
            InitializeComponent();
            Main = mainform;
            this.FormClosing += MKVPropEdit_FormClosing;
            configPath = sharedInstallPath + @"Config\";
            sharedSettings = settings;
            openFolder = new OpenFileDialog();
            openFolder.Filter = "";
            openFolder.ValidateNames = false;
            openFolder.CheckFileExists = false;
            openFolder.CheckPathExists = true;
            openFolder.FileName = "MKVToolNix";
            textBoxPath.Text = sharedSettings.MKV.ProgramPath;
            radioButtonTitleDelete.Checked = sharedSettings.MKV.Title == 1;
            radioButtonTitleEdit.Checked = sharedSettings.MKV.Title == 2;
            radioButtonTitleNothing.Checked = sharedSettings.MKV.Title == 0;
        }

        private bool MKVToolFound() {
            if (!textBoxPath.Text.EndsWith("\\"))
                textBoxPath.Text += "\\";
            string dir = textBoxPath.Text;
            if (Directory.Exists(dir) && File.Exists(dir + "mkvpropedit.exe"))
                return true;
            return false;
        }

        private void MKVPropEdit_FormClosing(object sender, FormClosingEventArgs e) {
            if (e.CloseReason == CloseReason.UserClosing) {
                e.Cancel = true;
                Hide();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(@"https://mkvtoolnix.download/");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            Process.Start(@"https://www.fosshub.com/MKVToolNix.html");
        }

        private bool OkayToRun(string programPath, string filePath) {
            if (!programPath.EndsWith("mkvpropedit.exe")) {
                UpdateOutput("Error: programPath does not end with mkvpropedit.exe");
                return false;
            }
            if (!(File.Exists(filePath) && filePath.ToLower().EndsWith(".mkv"))) {
                UpdateOutput("Error: filePath does not exist or does not end with .mkv");
                return false;
            }
            return true;
        }

        private void ProcessStartAsynch(string program, string arguments) {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.FileName = program;
            p.StartInfo.Arguments = arguments;
            //string output = p.StandardOutput.ReadToEnd();
            p.OutputDataReceived += P_OutputDataReceived;
            p.ErrorDataReceived += P_ErrorDataReceived;
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.WaitForExit();
            p.OutputDataReceived -= P_OutputDataReceived;
            p.ErrorDataReceived -= P_ErrorDataReceived;
        }

        private void P_ErrorDataReceived(object sender, DataReceivedEventArgs e) {
            UpdateOutput(e.Data);
        }

        private void P_OutputDataReceived(object sender, DataReceivedEventArgs e) {
            UpdateOutput(e.Data);
        }

        private void UpdateOutput(string text) {
            textBoxOutput.AppendText("\r\n" + text);
        }

        private void buttonBrowse_Click(object sender, EventArgs e) {
            OpenDirectory();
        }

        private void OpenDirectory() {
            if (openFolder.ShowDialog() == DialogResult.OK) {
                textBoxPath.Text = Path.GetDirectoryName(openFolder.FileName);
                if (!MKVToolFound()) {
                    MessageBox.Show("Unable to find mkvpropedit.exe");
                    return;
                }
            }
            
        }

        private void buttonRun_Click(object sender, EventArgs e) {
            if (!MKVToolFound()) {
                MessageBox.Show("Unable to find mkvpropedit.exe");
                return;
            }

            string progPath = textBoxPath.Text + "mkvpropedit.exe";
            List<string> files = Main.GetAllMKVFiles();
            if (files == null || files.Count == 0) {
                MessageBox.Show("Error: You have not checkmarked any .mkv files.");
                return;
            }

            if (radioButtonTitleDelete.Checked) {
                for (int i = 0; i < files.Count; i++) {
                    MKVDeleteTitle(progPath, files[i]);
                }
            } else if (radioButtonTitleEdit.Checked) {
                for (int i = 0; i < files.Count; i++) {
                    MKVEditTitle(progPath, files[i]);
                }
            }

        }

        private void MKVDeleteTitle(string programPath, string filePath) {
            if (!OkayToRun(programPath, filePath))
                return;
            int idx = filePath.LastIndexOf("\\");
            //at this stage, filePath should always end in .mkv
            string fileName = filePath.Substring(idx+1);
            fileName = fileName.Substring(0, fileName.Length - 4);
            UpdateOutput("Deleting title from " + fileName);
            ProcessStartAsynch(programPath, "-d title \"" + filePath + "\"");
            //example: mkvpropedit -d title "C:\path\ShowName S01E01.mkv"
        }

        private void MKVEditTitle(string programPath, string filePath) {
            if (!OkayToRun(programPath, filePath))
                return;
            int idx = filePath.LastIndexOf("\\");
            //at this stage, filePath should always end in .mkv
            string fileName = filePath.Substring(idx+1);
            fileName = fileName.Substring(0, fileName.Length - 4);
            UpdateOutput("Editing title of " + fileName);
            ProcessStartAsynch(programPath, "-s \"title=" + fileName + "\" \""+filePath+"\"");
            //example: mkvpropedit -s "title=ShowName S01E01" "C:\path\ShowName S01E01.mkv"
        }

        private void buttonSave_Click(object sender, EventArgs e) {
            SaveSettings();
        }

        private void SaveSettings() {
            sharedSettings.MKV.ProgramPath = textBoxPath.Text;
            sharedSettings.MKV.Title = radioButtonTitleDelete.Checked ? 1 : radioButtonTitleEdit.Checked ? 2 : 0;
            string settingsfile = configPath + @"settings.ini";
            string backupfile = configPath + @"backup_settings.ini";
            TextWriter tw;
            try {
                if (!File.Exists(settingsfile))
                    File.Create(settingsfile).Close();
                else {
                    //create a backup, then make a new one
                    if (File.Exists(backupfile))
                        File.Delete(backupfile);
                    File.Move(settingsfile, backupfile);
                    File.Create(settingsfile).Close();
                }
                //Thread.Sleep(500);
                tw = new StreamWriter(settingsfile);
            } catch (Exception e2) {
                MessageBox.Show("Unable to save settings.\r\n\r\n" + e2.ToString());
                return;
            }
            if (!sharedSettings.SaveSettings(tw))
                MessageBox.Show("Unable to save settings.");
            tw.Close();
            Main.LoadSettingsUpdateUI();
        }
    }
}
