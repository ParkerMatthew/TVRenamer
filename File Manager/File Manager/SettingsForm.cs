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
    public partial class SettingsForm : Form {

        MainForm Main;
        public Settings settings;
        private Settings tempSettings;
        private string settingsPath;
        public string installPath;
        private string configPath;
        char[] whiteSpaces = { ' ', '.', '_', '-' };
        string[] invalidChars = { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };
        private bool WarnNoSave;
        Settings mainSettingsCopy = new Settings();

        public SettingsForm(Settings sharedSettings, string sharedInstallPath, MainForm main) {
            InitializeComponent();
            settings = sharedSettings;
            settingsPath = sharedInstallPath+@"Config\settings.ini";
            configPath = sharedInstallPath + @"Config\";
            installPath = sharedInstallPath;
            Main = main;
            tempSettings = new Settings();
            CopySettings(settings, tempSettings);
            SetEventHandlers();
            this.FormClosing += SettingsForm_FormClosing;
            WarnNoSave = true;
        }

        public static void CopySettings(Settings source, Settings dest) {
            dest.AskDeleteConfirm = source.AskDeleteConfirm;
            dest.AutoOrganiseOnLoad = source.AutoOrganiseOnLoad;
            dest.AutoRenameOnLoad = source.AutoRenameOnLoad;
            dest.AutoRenameOnQuickSetting = source.AutoRenameOnQuickSetting;
            dest.Error.FilterFile = source.Error.FilterFile;
            dest.Error.NonFilterFile = source.Error.NonFilterFile;
            dest.FindReplaceFolders = source.FindReplaceFolders;
            dest.FindUsesRegexByDefault = source.FindUsesRegexByDefault;
            dest.RemoveBrackets = source.RemoveBrackets;
            dest.RemoveBracketsNonfilters = source.RemoveBracketsNonfilters;
            dest.RemoveUnwanted = source.RemoveUnwanted;
            dest.ReplaceCheckedByDefault = source.ReplaceCheckedByDefault;
            dest.TV.Double = source.TV.Double;
            dest.TV.Episode = source.TV.Episode;
            dest.TV.EpisodeDigits = source.TV.EpisodeDigits;
            dest.TV.ForceGuessSeries = source.TV.ForceGuessSeries;
            dest.TV.FormatEpisode = source.TV.FormatEpisode;
            dest.TV.GuessSeries = source.TV.GuessSeries;
            dest.TV.OnlyEpisode = source.TV.OnlyEpisode;
            dest.TV.Season = source.TV.Season;
            dest.TV.SeasonDigits = source.TV.SeasonDigits;
            dest.TV.SeriesSeparator = source.TV.SeriesSeparator;
            dest.TV.SpaceOption = source.TV.SpaceOption;
            dest.TV.TitleSeparator = source.TV.TitleSeparator;
            dest.TV.UseSeries = source.TV.UseSeries;
            dest.TV.UseTitle = source.TV.UseTitle;
            dest.Warning.Deletions = source.Warning.Deletions;
            dest.Warning.FolderCombiner = source.Warning.FolderCombiner;
            dest.Warning.TreeFreezeFirst = source.Warning.TreeFreezeFirst;
            dest.Warning.TreeFreezeLast = source.Warning.TreeFreezeLast;
        }

        private void SetEventHandlers() {
            checkBoxHighlight.CheckedChanged += CheckBox_CheckedChanged;
            checkBoxAutoOrganiseOnLoad.CheckedChanged += CheckBox_CheckedChanged;
            checkBoxAutoRenameOnLoad.CheckedChanged += CheckBox_CheckedChanged;
            checkBoxAutoRenameOnQuickSetting.CheckedChanged += CheckBox_CheckedChanged;
            checkBoxDeleteConfirm.CheckedChanged += CheckBox_CheckedChanged;
            checkBoxDeleteUnwanted.CheckedChanged += CheckBox_CheckedChanged;
            checkBoxErrorRegexJunk.CheckedChanged += CheckBox_CheckedChanged;
            checkBoxErrorRegexNonJunk.CheckedChanged += CheckBox_CheckedChanged;
            checkBoxFindRegexDefault.CheckedChanged += CheckBox_CheckedChanged;
            checkBoxForceGuessSeries.CheckedChanged += CheckBox_CheckedChanged;
            checkBoxFormatEpisode.CheckedChanged += CheckBox_CheckedChanged;
            checkBoxGuessSeries.CheckedChanged += CheckBox_CheckedChanged;
            checkBoxRemoveBrackets.CheckedChanged += CheckBox_CheckedChanged;
            checkBoxRemoveBracketsNonFilter.CheckedChanged += CheckBox_CheckedChanged;
            checkBoxReplaceCheckedDefault.CheckedChanged += CheckBox_CheckedChanged;
            checkBoxReplaceFolders.CheckedChanged += CheckBox_CheckedChanged;
            checkBoxUseShowName.CheckedChanged += CheckBox_CheckedChanged;
            checkBoxUseTitle.CheckedChanged += CheckBox_CheckedChanged;
            checkBoxWarningDeleteNode.CheckedChanged += CheckBox_CheckedChanged;
            checkBoxWarningFolderCombiner.CheckedChanged += CheckBox_CheckedChanged;

            textBoxSeparatorDouble.TextChanged += TextBox_TextChanged;
            textBoxSeparatorTitle.TextChanged += TextBox_TextChanged;
            textBoxSeparatorShowName.TextChanged += TextBox_TextChanged;
            textBoxPreviewInput.TextChanged += TextBox_TextChanged;
            textBoxStringEpisode.TextChanged += TextBox_TextChanged;
            textBoxStringSeason.TextChanged += TextBox_TextChanged;

            radioButton1.CheckedChanged += RadioButton_CheckedChanged;
            radioButton2.CheckedChanged += RadioButton_CheckedChanged;
            radioButton3.CheckedChanged += RadioButton_CheckedChanged;
            radioButton4.CheckedChanged += RadioButton_CheckedChanged;

            numericUpDownDigitsEpisode.ValueChanged += NumericUpDown_ValueChanged;
            numericUpDownDigitsSeason.ValueChanged += NumericUpDown_ValueChanged;

        }

        private void RemoveEventHandlers() {
            checkBoxHighlight.CheckedChanged -= CheckBox_CheckedChanged;
            checkBoxAutoOrganiseOnLoad.CheckedChanged -= CheckBox_CheckedChanged;
            checkBoxAutoRenameOnLoad.CheckedChanged -= CheckBox_CheckedChanged;
            checkBoxAutoRenameOnQuickSetting.CheckedChanged -= CheckBox_CheckedChanged;
            checkBoxDeleteConfirm.CheckedChanged -= CheckBox_CheckedChanged;
            checkBoxDeleteUnwanted.CheckedChanged -= CheckBox_CheckedChanged;
            checkBoxErrorRegexJunk.CheckedChanged -= CheckBox_CheckedChanged;
            checkBoxErrorRegexNonJunk.CheckedChanged -= CheckBox_CheckedChanged;
            checkBoxFindRegexDefault.CheckedChanged -= CheckBox_CheckedChanged;
            checkBoxForceGuessSeries.CheckedChanged -= CheckBox_CheckedChanged;
            checkBoxFormatEpisode.CheckedChanged -= CheckBox_CheckedChanged;
            checkBoxGuessSeries.CheckedChanged -= CheckBox_CheckedChanged;
            checkBoxRemoveBrackets.CheckedChanged -= CheckBox_CheckedChanged;
            checkBoxRemoveBracketsNonFilter.CheckedChanged -= CheckBox_CheckedChanged;
            checkBoxReplaceCheckedDefault.CheckedChanged -= CheckBox_CheckedChanged;
            checkBoxReplaceFolders.CheckedChanged -= CheckBox_CheckedChanged;
            checkBoxUseShowName.CheckedChanged -= CheckBox_CheckedChanged;
            checkBoxUseTitle.CheckedChanged -= CheckBox_CheckedChanged;
            checkBoxWarningDeleteNode.CheckedChanged -= CheckBox_CheckedChanged;
            checkBoxWarningFolderCombiner.CheckedChanged -= CheckBox_CheckedChanged;

            textBoxSeparatorDouble.TextChanged -= TextBox_TextChanged;
            textBoxSeparatorTitle.TextChanged -= TextBox_TextChanged;
            textBoxSeparatorShowName.TextChanged -= TextBox_TextChanged;
            textBoxPreviewInput.TextChanged -= TextBox_TextChanged;
            textBoxStringEpisode.TextChanged -= TextBox_TextChanged;
            textBoxStringSeason.TextChanged -= TextBox_TextChanged;

            radioButton1.CheckedChanged -= RadioButton_CheckedChanged;
            radioButton2.CheckedChanged -= RadioButton_CheckedChanged;
            radioButton3.CheckedChanged -= RadioButton_CheckedChanged;
            radioButton4.CheckedChanged -= RadioButton_CheckedChanged;

            numericUpDownDigitsEpisode.ValueChanged -= NumericUpDown_ValueChanged;
            numericUpDownDigitsSeason.ValueChanged -= NumericUpDown_ValueChanged;

        }

        private void NumericUpDown_ValueChanged(object sender, EventArgs e) {
            Something_Changed();
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (e.CloseReason == CloseReason.UserClosing) {
                e.Cancel = true;
                Hide();
            }
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e) {
            Something_Changed();
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e) {
            if(sender == checkBoxHighlight) {
                bool booly = checkBoxHighlight.Checked;
                panelWhiteSpace.BackColor = booly ? SystemColors.Info : Color.Transparent;
                checkBoxAutoOrganiseOnLoad.BackColor = booly ? SystemColors.Info : Color.Transparent;
                checkBoxAutoRenameOnLoad.BackColor = booly ? SystemColors.Info : Color.Transparent;
                checkBoxForceGuessSeries.BackColor = booly ? SystemColors.Info : Color.Transparent;
                checkBoxGuessSeries.BackColor = booly ? SystemColors.Info : Color.Transparent;
                textBoxSeparatorDouble.BackColor = booly ? SystemColors.Info : SystemColors.Window;
                textBoxSeparatorShowName.BackColor = booly ? SystemColors.Info : SystemColors.Window;
                textBoxSeparatorTitle.BackColor = booly ? SystemColors.Info : SystemColors.Window;
                return; //This isn't a real setting
            }
            Something_Changed();
        }

        private void TextBox_TextChanged(object sender, EventArgs e) {
            Something_Changed();
        }

        private void Something_Changed() {
            UpdateSettings(tempSettings);
            CopySettings(tempSettings, settings);
            string s = textBoxPreviewInput.Text;
            string ft = Main.GetFileType(s);
            if(tempSettings.TV.FormatEpisode)
                s = Main.FormatEpisode(Main.RemoveJunk(s), "1", "Series Name") + ft;
            else
                s = Main.RemoveJunk(s) + ft;
            textBoxPreviewOutput.Text = s;
            //do it again to see if the format is a problem
            ft = Main.GetFileType(s);
            if (tempSettings.TV.FormatEpisode)
                s = Main.FormatEpisode(Main.RemoveJunk(s), "1", "Series Name") + ft;
            else
                s = Main.RemoveJunk(s) + ft;
            if (textBoxPreviewOutput.Text == s)
                labelProblems.Text = "No problems detected.";
            else
                labelProblems.Text = "Problem: applying renamer again does not give same result. Unexpected format.";
            CopySettings(mainSettingsCopy, settings);
            buttonApply.Enabled = true;
            ButtonOk.Enabled = true;
        }

        private void UpdateSeparators(string space, string dub, string title) {
            //I might not use this
            textBoxSeparatorDouble.TextChanged -= TextBox_TextChanged;
            textBoxSeparatorTitle.TextChanged -= TextBox_TextChanged;
            textBoxSeparatorDouble.Text = space + dub + space;
            textBoxSeparatorTitle.Text = space + title + space;
            textBoxSeparatorDouble.TextChanged += TextBox_TextChanged;
            textBoxSeparatorTitle.TextChanged += TextBox_TextChanged;
        }

        private void UpdateSettings(Settings s) {
            s.AskDeleteConfirm = checkBoxDeleteConfirm.Checked;
            s.AutoOrganiseOnLoad = checkBoxAutoOrganiseOnLoad.Checked;
            s.AutoRenameOnLoad = checkBoxAutoRenameOnLoad.Checked;
            s.AutoRenameOnQuickSetting = checkBoxAutoRenameOnQuickSetting.Checked;
            s.Error.FilterFile = checkBoxErrorRegexJunk.Checked;
            s.Error.NonFilterFile = checkBoxErrorRegexNonJunk.Checked;
            s.FindReplaceFolders = checkBoxReplaceFolders.Checked;
            s.FindUsesRegexByDefault = checkBoxFindRegexDefault.Checked;
            s.RemoveBrackets = checkBoxRemoveBrackets.Checked;
            s.RemoveBracketsNonfilters = checkBoxRemoveBracketsNonFilter.Checked;
            s.RemoveUnwanted = checkBoxDeleteUnwanted.Checked;
            s.ReplaceCheckedByDefault = checkBoxReplaceCheckedDefault.Checked;
            s.TV.Double = textBoxSeparatorDouble.Text;
            s.TV.Episode = textBoxStringEpisode.Text;
            s.TV.EpisodeDigits = Decimal.ToInt32(numericUpDownDigitsEpisode.Value);
            s.TV.ForceGuessSeries = checkBoxForceGuessSeries.Checked;
            s.TV.FormatEpisode = checkBoxFormatEpisode.Checked;
            s.TV.GuessSeries = checkBoxGuessSeries.Checked;
            s.TV.OnlyEpisode = numericUpDownDigitsSeason.Value == 0;
            s.TV.Season = textBoxStringSeason.Text;
            s.TV.SeasonDigits = Decimal.ToInt32(numericUpDownDigitsSeason.Value);
            s.TV.SeriesSeparator = textBoxSeparatorShowName.Text;
            //It's an abomination!
            s.TV.SpaceOption = radioButton1.Checked ? " " : radioButton2.Checked ? "." : radioButton3.Checked ? "_" : radioButton4.Checked ? "-" : " ";
            //MessageBox.Show("\"" + s.TV.SpaceOption + "\"");
            s.TV.TitleSeparator = textBoxSeparatorTitle.Text;
            s.TV.UseSeries = checkBoxUseShowName.Checked;
            s.TV.UseTitle = checkBoxUseTitle.Checked;
            s.Warning.Deletions = checkBoxWarningDeleteNode.Checked;
            s.Warning.FolderCombiner = checkBoxWarningFolderCombiner.Checked;
            s.Warning.TreeFreezeFirst = Decimal.ToInt32(numericUpDownWarningFirst.Value);
            s.Warning.TreeFreezeLast = Decimal.ToInt32(numericUpDownWarningLast.Value);
        }

        public void UpdateFromSettings(Settings s) {
            RemoveEventHandlers();
            checkBoxDeleteConfirm.Checked = s.AskDeleteConfirm;
            checkBoxAutoOrganiseOnLoad.Checked = s.AutoOrganiseOnLoad;
            checkBoxAutoRenameOnLoad.Checked = s.AutoRenameOnLoad;
            checkBoxAutoRenameOnQuickSetting.Checked = s.AutoRenameOnQuickSetting;
            checkBoxErrorRegexJunk.Checked = s.Error.FilterFile;
            checkBoxErrorRegexNonJunk.Checked = s.Error.NonFilterFile;
            checkBoxReplaceFolders.Checked = s.FindReplaceFolders;
            checkBoxFindRegexDefault.Checked = s.FindUsesRegexByDefault;
            checkBoxRemoveBrackets.Checked = s.RemoveBrackets;
            checkBoxRemoveBracketsNonFilter.Checked = s.RemoveBracketsNonfilters;
            checkBoxDeleteUnwanted.Checked = s.RemoveUnwanted;
            checkBoxReplaceCheckedDefault.Checked = s.ReplaceCheckedByDefault;
            textBoxSeparatorDouble.Text = s.TV.Double;
            textBoxStringEpisode.Text = s.TV.Episode;
            numericUpDownDigitsEpisode.Value = s.TV.EpisodeDigits;
            checkBoxForceGuessSeries.Checked = s.TV.ForceGuessSeries;
            checkBoxFormatEpisode.Checked = s.TV.FormatEpisode;
            checkBoxGuessSeries.Checked = s.TV.GuessSeries;
            textBoxStringSeason.Text = s.TV.Season;
            numericUpDownDigitsSeason.Value = s.TV.SeasonDigits;
            textBoxSeparatorShowName.Text = s.TV.SeriesSeparator;
            radioButton1.Checked = s.TV.SpaceOption == " ";
            radioButton2.Checked = s.TV.SpaceOption == ".";
            radioButton3.Checked = s.TV.SpaceOption == "_";
            radioButton4.Checked = s.TV.SpaceOption == "-";
            textBoxSeparatorTitle.Text = s.TV.TitleSeparator;
            checkBoxUseShowName.Checked = s.TV.UseSeries;
            checkBoxUseTitle.Checked = s.TV.UseTitle;
            checkBoxWarningDeleteNode.Checked = s.Warning.Deletions;
            checkBoxWarningFolderCombiner.Checked = s.Warning.FolderCombiner;
            numericUpDownWarningFirst.Value = s.Warning.TreeFreezeFirst;
            numericUpDownWarningLast.Value = s.Warning.TreeFreezeLast;
            SetEventHandlers();
        }

        private void buttonLoad_Click(object sender, EventArgs e) {
            LoadSettings(tempSettings);
        }

        private void LoadSettings(Settings s) {
            try {
                TextReader tr = new StreamReader(settingsPath);
                s.LoadSettings(tr);
                tr.Close();
            } catch (Exception err) {
                MessageBox.Show("Unable to load settings.\r\n" + err.ToString());
                return;
            }
            buttonApply.Enabled = false;
            ButtonOk.Enabled = false;
            UpdateFromSettings(s);
        }

        private void buttonSave_Click(object sender, EventArgs e) {
            SaveSettings();
        }

        private void SaveSettings() {
            ApplySettings();
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
            if (!tempSettings.SaveSettings(tw))
                MessageBox.Show("Unable to save settings.");
            tw.Close();
            Main.LoadSettingsUpdateUI();
        }

        private void buttonOpenConfig_Click(object sender, EventArgs e) {
            Process.Start("explorer.exe", configPath);
        }

        private void buttonOpenFilters_Click(object sender, EventArgs e) {
            Process.Start(configPath + @"regex filters.ini");
        }

        private void buttonOpenNonJunk_Click(object sender, EventArgs e) {
            Process.Start(configPath + @"regex nonfilters.ini");
        }

        private void ButtonOk_Click(object sender, EventArgs e) {
            if (WarnNoSave) {
                MessageBoxWithCheckbox msg = new MessageBoxWithCheckbox("Settings will be changed until the program is closed. Click save to remember settings.", "Setting changes not saved.", MessageBoxButtons.OKCancel);
                msg.Checked = true;
                if (msg.ShowDialog() == DialogResult.OK)
                    WarnNoSave = msg.Checked;
                else
                    return;
            }
            ApplySettings();
            Main.LoadSettingsUpdateUI();
            this.Hide();
        }

        private void buttonApply_Click(object sender, EventArgs e) {
            ApplySettings();
            Main.LoadSettingsUpdateUI();
        }

        private void ApplySettings() {
            CopySettings(tempSettings, settings);
            CopySettings(tempSettings, mainSettingsCopy);
            buttonApply.Enabled = false;
            ButtonOk.Enabled = false;
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            CopySettings(settings, tempSettings);
            UpdateFromSettings(settings);
            buttonApply.Enabled = false;
            ButtonOk.Enabled = false;
            this.Hide();
        }

        private void SettingsForm_Load(object sender, EventArgs e) {
            //In order to use instant preview, I need to call MainForm functions with modified settings
            mainSettingsCopy = new Settings();
            CopySettings(settings, mainSettingsCopy);
            UpdateFromSettings(settings);
            
        }

        private void buttonReset_Click(object sender, EventArgs e) {
            tempSettings.ResetToDefault(true);
            UpdateFromSettings(tempSettings);
            Something_Changed();
        }
    }
}
