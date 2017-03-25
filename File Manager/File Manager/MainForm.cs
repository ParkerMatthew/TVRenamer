using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using System.Globalization;

namespace File_Manager {
    public partial class MainForm : Form {
        #region global variables
        SettingsForm settingsForm;
        OpenFileDialog openFolder;
        MKVPropEdit mkvTool;
        public Settings settings;
        char[] whiteSpaces = { ' ', '.', '_', '-' };
        string[] invalidChars = { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };
        string directory;
        uint treeFreeze;
        bool quitEarly;
        bool editingNode;
        string editNodeFileType;
        string installPath;
        bool formLostFocus;
        private bool preventExpandCollapse;
        private DateTime lastMouseDown;

        #endregion

        public MainForm() {
            InitializeComponent();
            string[] args = Environment.GetCommandLineArgs();
            //string[] args = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData;
            if (args.Length == 2)
                directory = args[1];
            else
                directory = Environment.CurrentDirectory;
            installPath = AppDomain.CurrentDomain.BaseDirectory;
            
        }

        private void MainForm_Load(object sender, EventArgs e) {
            settings = new Settings();
            LoadSettings();
            InitializeVariables();
            //Open the folder automatically
            if (!directory.EndsWith("\\"))
                directory += "\\";
            if (!installPath.EndsWith("\\"))
                directory += "\\";
            //MessageBox.Show(directory + "\r\n" + installPath);
            if (directory.Contains(installPath))
                directory = "";
            else
                UpdateTreesAndRename();
        }

        #region Form and UI stuff
        #region Setup
        private void LoadSettings() {
            //these should not be hard coded for the final release
            //installPath = @"T:\Documents\Visual Studio\Projects\File Manager\File Manager\";
            string settingsfile = installPath + @"Config\settings.ini";
            if (!File.Exists(settingsfile)) {
                MessageBox.Show("Unable to load settings, file does not exist");
                return;
            }
            TextReader tr = new StreamReader(settingsfile);
            settings.LoadSettings(tr);
            tr.Close();
            LoadSettingsUpdateUI();
        }

        private void SaveSettings() {
            string configPath = installPath + @"Config\";
            string settingsfile = configPath + @"settings.ini";
            string backupfile = configPath + @"settings_backup.ini";
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
            }
            catch(Exception e2) {
                MessageBox.Show("Unable to save settings.\r\n\r\n" + e2.ToString());
                return;
            }
            if(!settings.SaveSettings(tw))
                MessageBox.Show("Unable to save settings.");
            tw.Close();
        }

        private void InitializeVariables() {
            DisableAllTabIndexes(this.Controls);
            EnableSomeTabIndexes();
            mkvTool = new MKVPropEdit(settings, installPath, this);
            settingsForm = new SettingsForm(settings, installPath, this);
            openFolder = new OpenFileDialog();
            openFolder.Filter = "";
            openFolder.ValidateNames = false;
            openFolder.CheckFileExists = false;
            openFolder.CheckPathExists = true;
            openFolder.FileName = "Select Folder";
            treeView2.LabelEdit = true;
            editingNode = false;
            preventExpandCollapse = false;
            lastMouseDown = DateTime.Now;
            SetEventListeners();
            setToolTips();
        }

        private void SetEventListeners() {
            treeView1.AddLinkedTreeView(treeView2);
            treeView1.MouseEnter += TreeView1_MouseEnter;
            treeView2.MouseEnter += TreeView2_MouseEnter;
            formLostFocus = false;
            //this.Activated += MainForm_Activated;
            this.Deactivate += MainForm_Deactivate;
            treeView2.NodeMouseDoubleClick += TreeView2_NodeMouseDoubleClick;
            this.KeyPreview = true;
            treeView2.KeyDown += TreeView2_KeyDown;
            treeView2.AfterLabelEdit += TreeView2_AfterLabelEdit;
            treeView1.AfterCheck += TreeView1_AfterCheck;
            treeView2.AfterCheck += TreeView2_AfterCheck;
            treeView2.AfterSelect += TreeView2_AfterSelect;
            treeView1.AfterSelect += TreeView1_AfterSelect;

            treeView2.MouseDown += TreeView2_MouseDown;
            treeView2.BeforeExpand += TreeView2_BeforeExpand;
            treeView2.BeforeCollapse += TreeView2_BeforeCollapse;

            textBoxFind.LostFocus += TextBoxFind_LostFocus;
            textBoxReplace.LostFocus += TextBoxReplace_LostFocus;
            treeView2.NodeMouseClick += (sender, args) => treeView2.SelectedNode = args.Node;
            labelRegex.MouseClick += LabelRegex_MouseClick;
            labelReplace.MouseClick += LabelReplace_MouseClick;
            QuickSettingsAddListeners();
        }

        

        private void MainForm_Deactivate(object sender, EventArgs e) {
            treeView1.MouseEnter -= TreeView1_MouseEnter;
            treeView2.MouseEnter -= TreeView2_MouseEnter;
            if(!formLostFocus)
                this.Activated += MainForm_Activated;
            formLostFocus = true;
        }

        private void MainForm_Activated(object sender, EventArgs e) {
            treeView1.MouseEnter += TreeView1_MouseEnter;
            treeView2.MouseEnter += TreeView2_MouseEnter;
        }

        private void QuickSettingsAddListeners() {
            checkBoxFormatEpisode.CheckedChanged += QuickSettingsCheckBox;
            checkBoxUseSeries.CheckedChanged += QuickSettingsCheckBox;
            checkBoxUseTitle.CheckedChanged += QuickSettingsCheckBox;
            checkBoxRemoveBrackets.CheckedChanged += QuickSettingsCheckBox;
            checkBoxAutoRenameQuickSettings.CheckedChanged += QuickSettingsCheckBox;
            radioButton01.CheckedChanged += QuickSettingsRadioButton;
            radioButton101.CheckedChanged += QuickSettingsRadioButton;
            radioButton1x01.CheckedChanged += QuickSettingsRadioButton;
            radioButtonS01E01.CheckedChanged += QuickSettingsRadioButton;
            radioButtonOther.CheckedChanged += QuickSettingsRadioButton;
            textBoxEpisode.TextChanged += QuickSettingsTextBox;
            textBoxSeason.TextChanged += QuickSettingsTextBox;
            numericUpDownSeason.ValueChanged += QuickSettingsNumericUpDown;
            numericUpDownEpisode.ValueChanged += QuickSettingsNumericUpDown;
        }

        private void QuickSettingsRemoveListeners() {
            checkBoxFormatEpisode.CheckedChanged -= QuickSettingsCheckBox;
            checkBoxUseSeries.CheckedChanged -= QuickSettingsCheckBox;
            checkBoxUseTitle.CheckedChanged -= QuickSettingsCheckBox;
            checkBoxRemoveBrackets.CheckedChanged -= QuickSettingsCheckBox;
            checkBoxAutoRenameQuickSettings.CheckedChanged -= QuickSettingsCheckBox;
            radioButton01.CheckedChanged -= QuickSettingsRadioButton;
            radioButton101.CheckedChanged -= QuickSettingsRadioButton;
            radioButton1x01.CheckedChanged -= QuickSettingsRadioButton;
            radioButtonS01E01.CheckedChanged -= QuickSettingsRadioButton;
            radioButtonOther.CheckedChanged -= QuickSettingsRadioButton;
            textBoxEpisode.TextChanged -= QuickSettingsTextBox;
            textBoxSeason.TextChanged -= QuickSettingsTextBox;
            numericUpDownSeason.ValueChanged -= QuickSettingsNumericUpDown;
            numericUpDownEpisode.ValueChanged -= QuickSettingsNumericUpDown;
        }

        private void TreeView1_AfterSelect(object sender, TreeViewEventArgs e) {
            TreeNode[] tn = treeView2.Nodes.Find(treeView1.SelectedNode.Name, true);
            if (tn.Length == 1) {
                treeView2.SelectedNode = tn[0];
                treeView2.SelectedNode.EnsureVisible();
                treeView1.SelectedNode.EnsureVisible();
            }
            //MessageBox.Show(tn.Length.ToString());
        }

        private void TreeView2_AfterSelect(object sender, TreeViewEventArgs e) {
            TreeNode[] tn = treeView1.Nodes.Find(treeView2.SelectedNode.Name, true);
            if (tn.Length == 1) {
                treeView1.SelectedNode = tn[0];
                treeView1.SelectedNode.EnsureVisible();
                treeView2.SelectedNode.EnsureVisible();
            }
        }

        private void setToolTips() {
            ToolTip tooltip = new ToolTip();
            tooltip.AutoPopDelay = 30000;
            tooltip.SetToolTip(checkBoxFindUsesRegex, "If checked, the find textbox will search as a regular expression.");
            tooltip.SetToolTip(labelRegex, "If checked, the find textbox will search as a regular expression.");
            tooltip.SetToolTip(checkBoxReplace, "If checked, the Go button will perform a find and replace for all matches.");
            tooltip.SetToolTip(labelReplace, "If checked, the Go button will perform a find and replace for all matches.");
            tooltip.SetToolTip(buttonFindReplace, "Perform a find / replace on files.\r\nGo to tools->settings to enable folder find / replace.");
            tooltip.SetToolTip(buttonFolderCombine, "Looks for a single file that has a folder with a similar name, and creates a Series and Season folder for it.");
            tooltip.SetToolTip(buttonReload, "Reloads the current directory and undos any name changes.");
            tooltip.SetToolTip(buttonRenamer, "Attempts to automatically rename all files in the right tree based on their current text.");
            tooltip.SetToolTip(radioButtonOther, "For a more customisable format, go to tools->settings");
            tooltip.SetToolTip(checkBoxRemoveBrackets, "For more options regarding bracket behavior, go to tools->settings");
            tooltip.SetToolTip(checkBoxUseTitle, "To change or remove the separator for title text, go to tools->settings");
            tooltip.SetToolTip(checkBoxUseSeries, "To change or remove the separator for series text,\r\nor to change what text to use for series,\r\ngo to tools->settings");
            tooltip.SetToolTip(checkBoxFormatEpisode, "After removing junk text, the episode formatter looks for an episode number format and attempts to change it to the preferred format.\r\nIf it can't detect an episode number, none of the other format options are possible.");
            tooltip.SetToolTip(checkBoxAutoRenameQuickSettings, "If checked, any changes to the Quick Settings will instantly rename the currently open directory.");
            tooltip.SetToolTip(numericUpDownEpisode, "Sets the number of digits in the episode number.\r\nMust be at least 2, and can not be set above 2 if the Episode Separator String is blank.");
            tooltip.SetToolTip(numericUpDownSeason, "Sets the number of digits in the season number.\r\nIf set to 0, the Season Separator String will be removed.");
            tooltip.SetToolTip(textBoxEpisode, "Sets the Episode Separator String.");
            tooltip.SetToolTip(textBoxSeason, "Sets the Season Separator String.");
        }

        #endregion

        #region quicksettings
        private void buttonSaveSettings_Click(object sender, EventArgs e) {
            SaveSettings();
        }

        private void buttonLoadSettings_Click(object sender, EventArgs e) {
            LoadSettings();
            QuickSettingUpdateLabel();
        }

        public void LoadSettingsUpdateUI() {
            QuickSettingsRemoveListeners();
            checkBoxReplace.Checked = settings.ReplaceCheckedByDefault;
            checkBoxFindUsesRegex.Checked = settings.FindUsesRegexByDefault;
            checkBoxAutoRenameQuickSettings.Checked = settings.AutoRenameOnQuickSetting;
            checkBoxRemoveBrackets.Checked = settings.RemoveBrackets;
            checkBoxUseSeries.Checked = settings.TV.UseSeries;
            checkBoxUseTitle.Checked = settings.TV.UseTitle;
            textBoxEpisode.Text = settings.TV.Episode;
            textBoxSeason.Text = settings.TV.Season;
            numericUpDownEpisode.Value = settings.TV.EpisodeDigits;
            numericUpDownSeason.Value = settings.TV.SeasonDigits;
            QuickSettingsChooseRadioButton(settings.TV.Season, settings.TV.Episode, settings.TV.OnlyEpisode, settings.TV.SeasonDigits, settings.TV.EpisodeDigits);
            checkBoxFormatEpisode.Checked = settings.TV.FormatEpisode;
            QuickSettingUpdateLabel();
            QuickSettingsAddListeners();
        }

        private void QuickSettingsChooseRadioButton(string S, string E, bool ep, int sd, int ed) {
            radioButtonS01E01.Checked = (S == "S" && E == "E" && !ep && sd == 2 && ed == 2);
            radioButton01.Checked = (S == "" && E == "" && ep && sd == 0 && ed == 2);
            radioButton101.Checked = (S == "" && E == "" && !ep && sd == 1 && ed == 2);
            radioButton1x01.Checked = (S == "" && E == "x" && !ep && sd == 1 && ed == 2);
            radioButtonOther.Checked = false;

        }

        private void QuickSettingsNumericUpDown(object sender, EventArgs e) {
            if (textBoxEpisode.Text.Trim(whiteSpaces) == "")
                numericUpDownEpisode.Maximum = 2;
            else
                numericUpDownEpisode.Maximum = 3;
            settings.TV.EpisodeDigits = decimal.ToInt32(numericUpDownEpisode.Value);
            settings.TV.SeasonDigits = decimal.ToInt32(numericUpDownSeason.Value);
            settings.TV.OnlyEpisode = (settings.TV.SeasonDigits == 0);
            QuickSettingUpdateLabel();
        }

        private void QuickSettingsTextBox(object sender, EventArgs e) {
            if (textBoxEpisode.Text.Trim(whiteSpaces) == "")
                numericUpDownEpisode.Maximum = 2;
            else
                numericUpDownEpisode.Maximum = 3;
            settings.TV.Season = textBoxSeason.Text;
            settings.TV.Episode = textBoxEpisode.Text;
            QuickSettingUpdateLabel();
        }

        private void QuickSettingsRadioButton(object sender, EventArgs e) {
            RadioButton rb = sender as RadioButton;
            textBoxEpisode.Enabled = radioButtonOther.Checked;
            textBoxSeason.Enabled = radioButtonOther.Checked;
            numericUpDownSeason.Enabled = radioButtonOther.Checked;
            numericUpDownEpisode.Enabled = radioButtonOther.Checked;
            //this probably isn't needed
            if (!rb.Checked)
                return;
            if (rb == radioButtonOther) {
                settings.TV.Season = textBoxSeason.Text;
                settings.TV.Episode = textBoxEpisode.Text;
                settings.TV.SeasonDigits = decimal.ToInt32(numericUpDownSeason.Value);
                settings.TV.EpisodeDigits = decimal.ToInt32(numericUpDownEpisode.Value);
                settings.TV.OnlyEpisode = (settings.TV.SeasonDigits == 0);
                settings.TV.Double = settings.TV.SpaceOption + "&" + settings.TV.SpaceOption + settings.TV.Episode;
            }
            if(rb == radioButtonS01E01) {
                settings.TV.Season = "S";
                settings.TV.Episode = "E";
                settings.TV.SeasonDigits = 2;
                settings.TV.EpisodeDigits = 2;
                settings.TV.OnlyEpisode = false;
                settings.TV.Double = settings.TV.SpaceOption + "&" + settings.TV.SpaceOption + settings.TV.Episode;
            }
            if(rb == radioButton1x01) {
                settings.TV.Season = "";
                settings.TV.Episode = "x";
                settings.TV.SeasonDigits = 1;
                settings.TV.EpisodeDigits = 2;
                settings.TV.OnlyEpisode = false;
                settings.TV.Double = settings.TV.SpaceOption + "&" + settings.TV.SpaceOption + settings.TV.Episode;
            }
            if (rb == radioButton101) {
                settings.TV.Season = "";
                settings.TV.Episode = "";
                settings.TV.SeasonDigits = 1;
                settings.TV.EpisodeDigits = 2;
                settings.TV.OnlyEpisode = false;
                settings.TV.Double = settings.TV.SpaceOption + "&" + settings.TV.SpaceOption + settings.TV.Episode;
            }
            if (rb == radioButton01) {
                settings.TV.Season = "";
                settings.TV.Episode = "";
                settings.TV.SeasonDigits = 0;
                settings.TV.EpisodeDigits = 2;
                settings.TV.OnlyEpisode = true;
                settings.TV.Double = settings.TV.SpaceOption + "&" + settings.TV.SpaceOption + settings.TV.Episode;
            }
            QuickSettingUpdateLabel();
        }

        private void QuickSettingsCheckBox(object sender, EventArgs e) {
            settings.RemoveBrackets = checkBoxRemoveBrackets.Checked;
            settings.TV.UseSeries = checkBoxUseSeries.Checked;
            settings.TV.UseTitle = checkBoxUseTitle.Checked;
            QuickSettingFormatEpisode(checkBoxFormatEpisode.Checked);
            settings.AutoRenameOnQuickSetting = checkBoxAutoRenameQuickSettings.Checked;
            QuickSettingUpdateLabel();
        }

        private void QuickSettingFormatEpisode(bool enable) {
            checkBoxUseSeries.Enabled = enable;
            checkBoxUseTitle.Enabled = enable;
            radioButton01.Enabled = enable;
            radioButton101.Enabled = enable;
            radioButton1x01.Enabled = enable;
            radioButtonOther.Enabled = enable;
            radioButtonS01E01.Enabled = enable;
            textBoxEpisode.Enabled = enable && radioButtonOther.Checked;
            textBoxSeason.Enabled = enable && radioButtonOther.Checked;
            numericUpDownEpisode.Enabled = enable && radioButtonOther.Checked;
            numericUpDownSeason.Enabled = enable && radioButtonOther.Checked;
            settings.TV.FormatEpisode = enable;
        }

        private void QuickSettingUpdateLabel() {
            string s = "Series Name S001E0001 - Title [junk]";
            s = FormatEpisode(RemoveJunk(s), "1", "Series Name");
            labelPreview.Text = s;
            if (settings.AutoRenameOnQuickSetting)
                this.BeginInvoke(new Action(() => RenameTVShows(treeView2.Nodes)));
        }
        #endregion

        #region Find, Replace, and Command/Tab Keys
        private void TextBoxReplace_LostFocus(object sender, EventArgs e) {
            //diving this into two events prevents highlighting all when already focused
            textBoxReplace.LostFocus -= TextBoxReplace_LostFocus;
            textBoxReplace.MouseClick += TextBoxReplace_MouseClick;
        }

        private void TextBoxReplace_MouseClick(object sender, MouseEventArgs e) {
            //diving this into two events prevents highlighting all when already focused
            textBoxReplace.MouseClick -= TextBoxReplace_MouseClick;
            textBoxReplace.LostFocus += TextBoxReplace_LostFocus;
            FocusReplace();
        }

        private void TextBoxFind_LostFocus(object sender, EventArgs e) {
            //diving this into two events prevents highlighting all when already focused
            textBoxFind.LostFocus -= TextBoxFind_LostFocus;
            textBoxFind.MouseClick += TextBoxFind_MouseClick;
        }

        private void TextBoxFind_MouseClick(object sender, MouseEventArgs e) {
            //diving this into two events prevents highlighting all when already focused
            textBoxFind.MouseClick -= TextBoxFind_MouseClick;
            textBoxFind.LostFocus += TextBoxFind_LostFocus;
            FocusFind();
        }

        private void FocusFind() {
            textBoxFind.Focus();
            textBoxFind.SelectionStart = 0;
            textBoxFind.SelectionLength = textBoxFind.Text.Length;
        }

        private void FocusReplace() {
            textBoxReplace.Focus();
            textBoxReplace.SelectionStart = 0;
            textBoxReplace.SelectionLength = textBoxReplace.Text.Length;
        }

        private void LabelReplace_MouseClick(object sender, MouseEventArgs e) {
            checkBoxReplace.Checked = !checkBoxReplace.Checked;
        }

        private void LabelRegex_MouseClick(object sender, MouseEventArgs e) {
            checkBoxFindUsesRegex.Checked = !checkBoxFindUsesRegex.Checked;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == (Keys.Control | Keys.F)) {
                if (keyData == Keys.Shift) {
                    if (textBoxFind.Text != "") {
                        FindNext(treeView2);
                        return true;
                    }
                }
                FocusFind();
                return true;
            } else
            if (keyData == (Keys.Control | Keys.G)) {
                FocusReplace();
                return true;
            } else
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override bool ProcessTabKey(bool forward) {
            /*
            Overide the tab key so that
            1) find&replace text gets highlighted when tabbing
            2) enable going to next node in treeview
            */
            if (ActiveControl == textBoxFind && forward) {
                FocusReplace();
                return true;
            } else
            if (ActiveControl == textBoxReplace && !forward) {
                FocusFind();
                return true;
            } else
            if (ActiveControl == buttonFindReplace) {
                if (forward) {
                    if (textBoxSeason.Enabled)
                        return base.ProcessTabKey(forward);
                    else
                        FocusFind();
                } else
                    FocusReplace();
                return true;
            } else
            if (editingNode) {
                    //tab to edit next tree node
                treeView2.SelectedNode.EndEdit(false);
                if (!forward) {
                    //shift is being held
                    if (treeView2.SelectedNode.PrevVisibleNode != null) {
                        treeView2.SelectedNode = treeView2.SelectedNode.PrevVisibleNode;
                        EditTree2Node();
                    } else {
                        treeView2.SelectedNode = treeView2.SelectedNode.LastNode;
                        EditTree2Node();
                    }
                } else {
                    if (treeView2.SelectedNode.NextVisibleNode != null) {
                        treeView2.SelectedNode = treeView2.SelectedNode.NextVisibleNode;
                        EditTree2Node();
                    } else {
                        treeView2.SelectedNode = treeView2.SelectedNode.FirstNode;
                        EditTree2Node();
                    }
                }
                return true;
            } else
                return base.ProcessTabKey(forward);
        }

        private void DisableAllTabIndexes(Control.ControlCollection controls) {
            foreach (Control c in controls) {
                c.TabStop = false;
                DisableAllTabIndexes(c.Controls);
            }
        }

        private void EnableSomeTabIndexes() {
            //I swear it's easier to do it this way guys. ...Guys?
            textBoxFind.TabStop = true;
            textBoxFind.TabIndex = 1;
            textBoxReplace.TabStop = true;
            textBoxReplace.TabIndex = 2;
            buttonFindReplace.TabStop = true;
            buttonFindReplace.TabIndex = 3;
            textBoxSeason.TabStop = true;
            textBoxSeason.TabIndex = 4;
            textBoxEpisode.TabStop = true;
            textBoxEpisode.TabIndex = 5;
        }

        private void FindReplaceRecursive(TreeNodeCollection tv) {
            if (tv == null || tv.Count == 0) {
                //A single child node
                return;
            }
            foreach (TreeNode node in tv) {
                if (node.Nodes.Count > 1 || settings.FindReplaceFolders)
                    FindReplace(node.Nodes);
                FindReplaceRecursive(node.Nodes);
            }
        }

        private void FindReplace(TreeNodeCollection tnc) {
            string find = textBoxFind.Text;
            string replace = textBoxReplace.Text;
            foreach (TreeNode n in tnc) {
                if (!checkBoxFindUsesRegex.Checked) {
                    if (n.Text.Contains(find)) {
                        n.Text = n.Text.Replace(find, replace);
                        n.BackColor = Color.LightCoral;
                    }
                } else
                    try {
                        string s = n.Text;
                        Regex rgx = new Regex(find);
                        Match found = rgx.Match(s);
                        if (found.Success) {
                            n.Text = rgx.Replace(s, replace);
                            n.BackColor = Color.LightCoral;
                        }
                    } catch (ArgumentException) {
                        MessageBox.Show("Invalid regular expression in the Find textbox");
                        return;
                    }
            }
        }

        public void FindNext(TreeView tv) {
            if (tv == null) {
                return;
            }
            string find = textBoxFind.Text;
            TreeNode selectedNode = tv.SelectedNode;
            TreeNode[] foundNodes = FindAllNodes(tv.Nodes, find, checkBoxFindUsesRegex.Checked);
            if (foundNodes == null || foundNodes.Length == 0)
                return;
            int count = foundNodes.Length;
            for (int i = 0; i < count; i++) {
                //look to see if one is already selected to select the next one
                if (selectedNode == foundNodes[i]) {
                    if (i < count - 1) {
                        tv.SelectedNode = foundNodes[i + 1];
                    } else {
                        tv.SelectedNode = foundNodes[0];
                    }
                    tv.SelectedNode.EnsureVisible();
                    tv.Focus();
                    return;
                }
            }
            //None are already selected, so just select the first
            tv.SelectedNode = foundNodes[0];
            tv.SelectedNode.EnsureVisible();
            tv.Focus();
        }

        #endregion

        #region UI form buttons
        private void buttonReload_Click(object sender, EventArgs e) {
            reOpenFolder();
        }

        private void reOpenFolder() {
            if (directory == "" || directory == null)
                OpenDirectory();
            else {
                treeView1.Nodes.Clear();
                UpdateTrees();
            }
        }

        private void OpenDirectory() {
            if (openFolder.ShowDialog() == DialogResult.OK) {
                treeView1.Nodes.Clear();
                directory = Path.GetDirectoryName(openFolder.FileName);
                UpdateTreesAndRename();
            }
        }

        private void UpdateTreesAndRename() {
            UpdateTrees();
            if (!quitEarly) {
                if (settings.AutoOrganiseOnLoad)
                    FolderCombinerLooper(false);
                if (settings.AutoRenameOnLoad)
                    RenameTVShows(treeView2.Nodes);
            }
        }

        private void buttonRenamer_Click(object sender, EventArgs e) {
            if (treeView2.Nodes.Count > 0)
                RenameTVShows(treeView2.Nodes);
        }

        private void buttonFindReplace_Click(object sender, EventArgs e) {
            if (treeView2.Nodes.Count == 0)
                return;
            if (checkBoxReplace.Checked) {
                if (textBoxFind.Text != "")
                    FindReplaceRecursive(treeView2.Nodes);
            } else {
                if (textBoxFind.Text != "")
                    FindNext(treeView2);
            }
        }

        private void buttonFolderCombine_Click(object sender, EventArgs e) {
            if (treeView2.Nodes.Count > 0)
                FolderCombinerLooper(settings.Warning.FolderCombiner);
        }

        private void buttonFolderSplit_Click(object sender, EventArgs e) {
            if (treeView2.Nodes.Count > 0)
                FolderSplitter();
        }

        private void buttonApplyAll_Click(object sender, EventArgs e) {
            if (treeView2.Nodes.Count > 0)
                ApplyAll();
            reOpenFolder();
        }

        private void ApplyAll() {
            if (!directory.EndsWith("\\"))
                directory += "\\";
            string rootDirectory = directory.Remove(directory.LastIndexOf(treeView2.Nodes[0].Text + "\\"), treeView2.Nodes[0].Text.Length + 1);
            //string[] directories = FindNewDirectories(treeView1, treeView2.Nodes);
            string[] deletions = FindDeletedNodes(treeView1.Nodes, treeView2);
            if (!ApplyDeletesConfirm(deletions))
                return;
            //if (!ApplyCreateNewDirectories(directories))
            //    return;
            //Renames require folder names to be unchanged
            if (!ApplyRenamesFiles(treeView2.Nodes, rootDirectory))
                return;
            //Deletions require folder names to be unchanged
            if (!ApplyDeletes(deletions))
                return;
            if (!ApplyRenamesFolders(treeView2.Nodes, rootDirectory))
                return;
        }

        private bool ApplyDeletes(string[] deletions) {
            bool booly = true;
            for(int i=0; i<deletions.Length; i++) {
                string s = deletions[i];
                FileAttributes attr = File.GetAttributes(s);
                try {
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                        Directory.Delete(s, true);
                    else
                        File.Delete(s);
                    
                }
                catch (Exception e) {
                    MessageBox.Show("Unable to delete file/folder " + deletions[i] + "\r\n" + e.ToString());
                    return false;
                }
            }

            return booly;
        }

        private bool ApplyRenamesFolders(TreeNodeCollection tv2nodes, string newDirectory) {
            if (!newDirectory.EndsWith(@"\")) {
                MessageBox.Show(newDirectory + "\r\nDoes not end in \\");
                return false;
            }
            //ignore files and folders that aren't changed and checked
            bool booly = true;
            foreach (TreeNode node in tv2nodes) {
                if (node.Nodes.Count > 0) {
                    booly &= ApplyRenamesFolders(node.Nodes, newDirectory + node.Text + @"\");
                    if (node.Checked && node.BackColor != Color.Empty && !Directory.Exists(newDirectory + node.Text)) {
                        try {
                            //MessageBox.Show("Attempting to rename folder.\r\nnewDirectory = " + newDirectory + "\r\nnode.Text = " + node.Text + "\r\nnode.Name = " + node.Name);
                            Directory.Move(node.Name+"\\", newDirectory + node.Text);
                        } catch (Exception e) {
                            MessageBox.Show("Unable to rename/move folder\r\n" + node.Name + "\r\n\r\n\r\n" + e.ToString());
                            return false;
                        }
                    }
                }
            }
            return booly;
        }

        private bool ApplyRenamesFiles(TreeNodeCollection tv2nodes, string newDirectory) {
            if (!newDirectory.EndsWith(@"\")) {
                MessageBox.Show(newDirectory+"\r\nDoes not end in \\");
                return false;
            }
            //ignore folders that were renamed until files are moved/renamed
            //ignore unchecked and unchanged files
            bool booly = true;
            foreach(TreeNode node in tv2nodes) {
                if (node.Nodes.Count > 0)
                    booly &= ApplyRenamesFiles(node.Nodes, newDirectory + node.Text + @"\");
                else if (node.Checked && node.BackColor != Color.Empty)
                    try {
                        //The file gets renamed before the folder. Use the current file path, and just rename it
                        File.Move(node.Name, Path.GetDirectoryName(node.Name) + @"\" + node.Text);
                    } catch (Exception e) {
                        MessageBox.Show("Unable to rename/move file\r\n" + node.Name + "\r\n\r\n\r\n" + e.ToString());
                        return false;
                    }
            }
            return booly;
        }

        private bool ApplyCreateNewDirectories(string[] directories) {
            for(int i=0; i<directories.Length; i++) {
                try {
                    Directory.CreateDirectory(directories[i]);
                } 
                catch (Exception e) {
                    MessageBox.Show("Could not create directory "+directories[i]+"\r\n"+e.ToString());
                    return false;
                }
            }
            return true;
        }

        private bool ApplyDeletesConfirm(string[] deletions) {
            if (deletions.Length > 0 && settings.Warning.Deletions) {
                string deletes = "The following files will be deleted (look for any video files!)\r\n------------------------------------------------------------------------------------------------\r\n";
                for (int i = 0; i < deletions.Length; i++) {
                    deletes += deletions[i] + "\r\n";
                }
                MessageBoxWithCheckbox msg = new MessageBoxWithCheckbox(deletes, "Continue?", MessageBoxButtons.OKCancel, true);
                DialogResult dr = msg.ShowDialog();
                if (dr == DialogResult.Cancel)
                    return false;
                settings.Warning.Deletions = !msg.Checked;
                msg.Dispose();
            }
            return true;
        }

        private string[] FindDeletedNodes(TreeNodeCollection tv1nodes, TreeView tv2) {
            //iterate through the first treeview, and mark for delete anything that doesn't exist in the second treeview
            List<string> deleteList = new List<string>();
            foreach (TreeNode node in tv1nodes) {
                if (node.Nodes.Count > 0)
                    deleteList.AddRange(FindDeletedNodes(node.Nodes, tv2));
                TreeNode[] alreadyExists = tv2.Nodes.Find(node.Name, true);
                if (alreadyExists.Length == 0) {
                    deleteList.Add(node.Name);
                }

            }
            return deleteList.ToArray();

        }

        private string[] FindNewDirectories(TreeView tv1, TreeNodeCollection tv2nodes) {
            //iterate through the second treeview, and mark for creation anything that doesn't exist in the first treeview
            List<string> directories = new List<string>();
            foreach(TreeNode node in tv2nodes) {
                if (node.Nodes.Count > 0)
                    directories.AddRange(FindNewDirectories(tv1, node.Nodes));
                TreeNode[] alreadyExists = tv1.Nodes.Find(node.Name, true);
                if(alreadyExists.Length == 0) {
                    directories.Add(node.Name);
                }

            }
            return directories.ToArray();

        }
        #endregion

        #endregion

        #region string stuff
        public string CleanSpaces(string s, string space) {
            if (space != " " && space != "." && space != "_" && space != "-") {
                space = " ";
            }
            string[] strings = s.Split('.', ' ', '_', '-');
            return string.Join(space, strings);
        }

        public string EnglishTitleText(string s) {
            TextInfo ti = new CultureInfo("en-US", false).TextInfo;
            s = ti.ToTitleCase(s);
            s = s.Replace(" A ", " a ");
            s = s.Replace(" Also ", " also ");
            s = s.Replace(" An ", " an ");
            s = s.Replace(" And ", " and ");
            s = s.Replace(" As ", " as ");
            s = s.Replace(" At ", " at ");
            s = s.Replace(" Be ", " be ");
            s = s.Replace(" But ", " but ");
            s = s.Replace(" By ", " by ");
            s = s.Replace(" For ", " for ");
            s = s.Replace(" From ", " from ");
            s = s.Replace(" In ", " in ");
            s = s.Replace(" Into ", " into ");
            s = s.Replace(" Has ", " has ");
            s = s.Replace(" Had ", " had ");
            s = s.Replace(" Is ", " is ");
            s = s.Replace(" Nor ", " nor ");
            s = s.Replace(" Not ", " not ");
            s = s.Replace(" Of ", " of ");
            s = s.Replace(" Off ", " off ");
            s = s.Replace(" On ", " on ");
            s = s.Replace(" Onto ", " onto ");
            s = s.Replace(" Or ", " or ");
            s = s.Replace(" O'c ", " O'C ");
            s = s.Replace(" Over ", " over ");
            s = s.Replace(" So ", " so ");
            s = s.Replace(" To ", " to ");
            s = s.Replace(" That ", " that ");
            s = s.Replace(" This ", " this ");
            s = s.Replace(" Thus ", " thus ");
            s = s.Replace(" The ", " the ");
            s = s.Replace(" Too ", " too ");
            s = s.Replace(" When ", " when ");
            s = s.Replace(" With ", " with ");
            s = s.Replace(" Up ", " up ");
            s = s.Replace(" Yet ", " yet ");
            return s;
        }

        public string RemoveJunk(string s) {
            //this will remove common junk text found in video file names
            //It will also remove the filetype, so be sure to run GetFileType() and save it first!!
            string result = s;
            string filetype = @"\.\w+$";
            string leftoverSpaces = @"( |\.|_|\-){2,}";
            string leftoverBrackets = @"(\[\])|(\{\})|(\(\))";
            string endingSpace = @"( |\.|\-|_)$";
            string startingSpace = @"^( |\.|\-|_)";
            Regex rgx = new Regex(filetype);
            Match fileType = rgx.Match(s);
            //replace the filetype with a period. This is used for filters than check if there is a whitespace after a match
            result = rgx.Replace(result, ".");
            //Apply non-filters then filters
            string[] nonFilters = ApplyNonFilters(ref result);
            ApplyFilters(ref result);
            if (settings.RemoveBrackets) {
                result = RemoveBracketJunk(result);
            }
            //reinsert non-filters 
            if (nonFilters.Length > 0) {
                for (int i = 0; i < nonFilters.Length; i++) {
                    rgx = new Regex("<" + i.ToString() + ">");
                    string t = nonFilters[i];
                    //The goal is to add a single space after the non-filter, then remove any double spaces caused by it
                    if (!t.EndsWith(settings.TV.SpaceOption))
                        t += settings.TV.SpaceOption;
                    result = rgx.Replace(result, t);
                    rgx = new Regex(settings.TV.SpaceOption + settings.TV.SpaceOption);
                    result = rgx.Replace(result, settings.TV.SpaceOption);
                }
            }
            //Remove any leftover brackets
            rgx = new Regex(leftoverBrackets);
            result = rgx.Replace(result, "");
            //Convert to Title Text format, then Clean spaces after applying junk filter
            rgx = new Regex(leftoverSpaces);
            result = CleanSpaces(rgx.Replace(result, settings.TV.SpaceOption), " ");
            result = EnglishTitleText(result);
            result = CleanSpaces(rgx.Replace(result, settings.TV.SpaceOption), settings.TV.SpaceOption);
            rgx = new Regex(endingSpace);
            result = rgx.Replace(result, "");
            rgx = new Regex(startingSpace);
            result = rgx.Replace(result, "");
            
            return result;
        }

        public string[] ApplyNonFilters(ref string result) {
            int i = 0;
            string regexNonFiltersPath = installPath + @"Config\regex nonfilters.ini";
            Regex rgx;
            string[] regexNonFilters;
            List<string> nonFilters = new List<string>();
            if (File.Exists(regexNonFiltersPath)) {
                regexNonFilters = File.ReadAllLines(regexNonFiltersPath);
                foreach (string filter in regexNonFilters) {
                    if (!filter.StartsWith("#") && !String.IsNullOrWhiteSpace(filter)) {
                        try {
                            rgx = new Regex(filter);
                            Match match = rgx.Match(result);
                            if (match.Success) {
                                string t = match.Value;
                                if (settings.RemoveBracketsNonfilters && t.StartsWith("[") && t.EndsWith("]")) {
                                    t = t.Between("[", "]");
                                    result = rgx.Replace(result, t);
                                } else if (settings.RemoveBracketsNonfilters && t.StartsWith("(") && t.EndsWith(")")) {
                                    t = t.Between("(", ")");
                                    result = rgx.Replace(result, t);
                                } else if (settings.RemoveBracketsNonfilters && t.StartsWith("{") && t.EndsWith("}")) {
                                    t = t.Between("{", "}");
                                    result = rgx.Replace(result, t);
                                } else {
                                    result = rgx.Replace(result, "<" + i.ToString() + ">");
                                    nonFilters.Add(match.Value);
                                    i++;
                                }
                            }
                        } catch (ArgumentException e) {
                            if (settings.Error.NonFilterFile)
                                MessageBox.Show("Error: Invalid Regex Filter\r\n" + filter + "\r\nFound in\r\n" + regexNonFiltersPath + "\r\n" + e, "Invalid Regular Expression");
                        }
                    }
                }
                settings.Error.NonFilterFile = false;
            }
            return nonFilters.ToArray();
        }

        public void ApplyFilters(ref string result) {
            Regex rgx;
            string regexFiltersPath = installPath + @"Config\regex filters.ini";
            string[] regexFilters;
            if (File.Exists(regexFiltersPath)) {
                regexFilters = File.ReadAllLines(regexFiltersPath);
                foreach (string filter in regexFilters) {
                    if (!filter.StartsWith("#") && !String.IsNullOrWhiteSpace(filter)) {
                        try {
                            rgx = new Regex(filter);
                            result = rgx.Replace(result, "");
                        } catch (ArgumentException e) {
                            if (settings.Error.FilterFile)
                                MessageBox.Show("Error: Invalid Regex Filter\r\n" + filter + "\r\nFound in\r\n" + regexFiltersPath + "\r\n" + e, "Invalid Regular Expression");
                        }
                    }
                }
                settings.Error.FilterFile = false;
            }
        }

        public string RemoveBracketJunk(string s) {
            string result = s;
            string[] brackets = {
                @"\[[^\]]*\]",
                @"\([^\)]*\)",
                @"\{[^\}]*\}",
            };
            for(int i =0; i<brackets.Length; i++) {
                Regex rgx = new Regex(brackets[i]);
                result = rgx.Replace(result, "");
            }
            return result;
        }

        public string GetFileType(string s) {
            string filetype = @"\.\w+$";
            Regex rgx = new Regex(filetype);
            Match fileType = rgx.Match(s);
            if (fileType.Success) {
                return fileType.Value;
            } else {
                return "";
            }
        }

        public string CommonString(params string[] strings) {
            //Order matters! Whichever string is last will choose things such as episode number or capitalization of words that are in common
            //Example outputs:
            //CommonString("Show 101 Pilot","show 102 two") returns "show 102" and CommonString("show 102 two","Show 101 Pilot") returns "Show 101"
            //expects filenames without the filetype at the end
            //do not give it any empty strings, or an incomplete string array
            //also, it might screw up if it's given junk whitespaces (especially starting spaces!)

            LevenshteinComparer leven = new LevenshteinComparer(2);
            string string1 = "";
            string string2 = "";
            if (strings.Length == 0) {
                return "";
            } else if (strings.Length == 1) {
                return strings[0];
            } else if (strings.Length == 2) {
                string1 = strings[0];
                string2 = strings[1];
                if (string1 == "" || string2 == "")
                    return ""; //instead of returning a space.
            } else {
                string1 = strings[0];
                string[] newStrings = new string[strings.Length - 1];
                Array.ConstrainedCopy(strings, 1, newStrings, 0, strings.Length - 1);
                string2 = CommonString(newStrings);
            }
            string[] words1 = string1.Split(whiteSpaces);
            string[] words2 = string2.Split(whiteSpaces);
            //Calling the built-in function has some unusual behavior in some cases
            var wordsIntersecting = words2.Intersect(words1, leven);
            string output = string.Join(settings.TV.SpaceOption, wordsIntersecting);
            return output;
        }

        public string UniqueString(string commonstring, string s) {
            //gets the unique string of an episode, which is assumed to be the title
            //The leven is stricter than commonString to get all unique words
            LevenshteinComparer leven = new LevenshteinComparer(0);
            //Since the distance is 0, any episode numbers and capitalizations must match exactly or they will appear to be unique
            string c = CommonString(commonstring, s);
            string[] words1 = s.Split('.', ' ', '_', '-');
            string[] words2 = c.Split('.', ' ', '_', '-');
            //we don't care about finding the unique words in words2 because it shouldn't have any
            var wordsUnique = words1.Except(words2, leven);
            string output = string.Join(settings.TV.SpaceOption, wordsUnique);
            return output;
        }

        public List<string> GetAllMKVFiles() {
            //called by the MKVPropEdit form
            List<string> output = new List<string>();
            if (treeView2.Nodes.Count == 0)
                return output;

            foreach (TreeNode node in treeView2.Nodes) {
                if (node.Nodes.Count > 0)
                    output.AddRange(GetAllMKVFiles(node.Nodes));
                else if (node.Checked && node.Name.ToLower().EndsWith(".mkv"))
                    output.Add(node.Name);
            }
            return output;
        }

        private List<string> GetAllMKVFiles(TreeNodeCollection nodes) {
            //recursive looper
            List<string> output = new List<string>();
            foreach (TreeNode node in nodes) {
                if (node.Nodes.Count > 0)
                    output.AddRange(GetAllMKVFiles(node.Nodes));
                else if (node.Checked && node.Name.ToLower().EndsWith(".mkv"))
                    output.Add(node.Name);
            }
            return output;
        }

        #endregion

        #region treeview stuff
        public static TreeNode[] FindAllNodes(TreeNodeCollection tnc, string nodeText, bool useRegex) {
            List<TreeNode> nodeList = new List<TreeNode>();
            foreach (TreeNode childNode in tnc) {
                if (!useRegex && childNode.Text.Contains(nodeText))
                    nodeList.Add(childNode);
                else if (useRegex) 
                    try {
                        Regex rgx = new Regex(nodeText);
                        Match match = rgx.Match(childNode.Text);
                        if(match.Success)
                            nodeList.Add(childNode);
                    } catch (ArgumentException) {
                        MessageBox.Show("Invalid regular expression in the Find textbox");
                        return nodeList.ToArray<TreeNode>();
                    }
                if (childNode.Nodes.Count > 0)
                    nodeList.AddRange(FindAllNodes(childNode.Nodes, nodeText, useRegex));
            }
            return nodeList.ToArray<TreeNode>();
        }

        private void deleteNodeWithConfirm() {
            if (settings.AskDeleteConfirm) {
                MessageBoxWithCheckbox msg = new MessageBoxWithCheckbox("Are you sure you want to delete this file or folder and all of its contents?\r\n(Will not happen until all changes are applied).", "Delete " + treeView2.SelectedNode.Text + "?", MessageBoxButtons.OKCancel);
                if (msg.ShowDialog() == DialogResult.OK) {
                    if (msg.Checked)
                        settings.AskDeleteConfirm = false;
                    msg.Dispose();
                } else
                    return;
            }
            treeView2.SelectedNode.Remove();
        }

        private void UpdateTrees() {
            DirectoryInfo directoryInfo = new DirectoryInfo(@directory);
            if (directoryInfo.Exists) {
                UpdateTree1(directoryInfo);
                UpdateTree2();
                treeView1.ExpandAll();
                treeView1.Nodes[0].EnsureVisible();
                treeView2.ExpandAll();
                treeView2.Nodes[0].EnsureVisible();
                treeView2.Nodes[0].Checked = true;
            } else {
                MessageBox.Show("The directory " + directory + " does not exist.", "Location not found", MessageBoxButtons.OK);
            }
        }

        private void UpdateTree1(DirectoryInfo directoryInfo) {
            treeFreeze = 0;
            quitEarly = false;
            BuildTree(directoryInfo, treeView1.Nodes);
        }

        private void UpdateTree2() {
            //causes a crash if either treeview is empty. checking if they're empty causes a crash too.
            treeView2.Nodes.Clear();
            CopyTreeNodes(treeView1, treeView2);
        }

        #region treeview enhanced UI
        private void TreeView1_MouseEnter(object sender, EventArgs e) {
            //enable scrolling
            if (!editingNode && ActiveControl != textBoxFind && ActiveControl != textBoxReplace) {
                treeView1.Focus();
            }
        }

        private void TreeView2_MouseEnter(object sender, EventArgs e) {
            //enable scrolling
            if (!editingNode && ActiveControl != textBoxFind && ActiveControl != textBoxReplace) {
                treeView2.Focus();
            }
        }

        private void TreeView2_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e) {
            var hitTest = treeView2.HitTest(e.Location);
            bool onPlusMinus = hitTest.Location == TreeViewHitTestLocations.PlusMinus;
            if (!editingNode && !onPlusMinus)
                EditTree2Node();
        }

        private void TreeView2_BeforeExpand(object sender, TreeViewCancelEventArgs e) {
            e.Cancel = preventExpandCollapse;
            preventExpandCollapse = false;
        }

        private void TreeView2_BeforeCollapse(object sender, TreeViewCancelEventArgs e) {
            e.Cancel = preventExpandCollapse;
            preventExpandCollapse = false;
        }

        private void TreeView2_MouseDown(object sender, MouseEventArgs e) {
            DateTime oldMouseDown = lastMouseDown;
            lastMouseDown = DateTime.Now;
            var hitTest = treeView2.HitTest(e.Location);
            bool onPlusMinus = hitTest.Location == TreeViewHitTestLocations.PlusMinus;
            int delta = (int)lastMouseDown.Subtract(oldMouseDown).TotalMilliseconds;
            preventExpandCollapse = !onPlusMinus && (delta < SystemInformation.DoubleClickTime);
        }

        private void TreeView2_KeyDown(object sender, KeyEventArgs e) {
            //F2 to edit
            if (treeView2.SelectedNode != null && e.KeyCode == Keys.F2 && !editingNode) {
                EditTree2Node();
            }
            //DEL to delete
            if (treeView2.SelectedNode != null && e.KeyCode == Keys.Delete && !editingNode) {
                deleteNodeWithConfirm();
            }
            //Tab to start editing next
            //code found in ProcessTabKey()
        }

        private void EditTree2Node() {
            if (treeView2.SelectedNode == null)
                return;
            editingNode = true;
            editNodeFileType = GetFileType(treeView2.SelectedNode.Text);
            treeView2.SelectedNode.BeginEdit();
        }

        private void TreeView2_AfterLabelEdit(object sender, NodeLabelEditEventArgs e) {
            e.Node.BackColor = Color.Cyan;
            editingNode = false;
            this.BeginInvoke(new Action(() => AfterEditFileTypeAppend(e)));
        }

        private void AfterEditFileTypeAppend(NodeLabelEditEventArgs e) {
            treeView2.AfterLabelEdit -= TreeView2_AfterLabelEdit;
            string nodeText = e.Node.Text;
            for(int i=0; i<invalidChars.Length; i++) {
                nodeText = nodeText.Replace(invalidChars[i], "");
            }
            e.Node.Text = nodeText;
            if (editNodeFileType != GetFileType(e.Node.Text)) {
                e.Node.Text += editNodeFileType;
            }
            treeView2.AfterLabelEdit += TreeView2_AfterLabelEdit;
        }

        private void TreeView1_AfterCheck(object sender, TreeViewEventArgs e) {
            //tree 1 has checkboxes, but they represent nothing. To avoid confusion, they will always be unchecked.
            treeView1.AfterCheck -= TreeView1_AfterCheck;
            treeView1.Nodes[0].Checked = false;
            treeView1.AfterCheck += TreeView1_AfterCheck;
        }

        private void TreeView2_AfterCheck(object sender, TreeViewEventArgs e) {
            if (e.Node.Checked) {
                e.Node.ForeColor = Color.Black;
            } else {
                e.Node.ForeColor = Color.Gray;
            }
        }

        #endregion

        #region treeview modifiers
        private void BuildTree(DirectoryInfo directoryInfo, TreeNodeCollection addInMe) {
            TreeNode curNode = addInMe.Add(directoryInfo.Name);
            curNode.Name = directoryInfo.FullName;
            curNode.ContextMenuStrip = contextMenuStripNode;
            if (quitEarly) {
                return;
            } else if (!quitEarly && treeFreeze == settings.Warning.TreeFreezeFirst) {
                DialogResult diagResult = MessageBox.Show("It might take a while to show all files. Do you want to show all files?", "Show all Files?", MessageBoxButtons.YesNo);
                if (diagResult == DialogResult.No) {
                    quitEarly = true;
                    return;
                }
            } else if (treeFreeze == settings.Warning.TreeFreezeLast) {
                DialogResult diagResult = MessageBox.Show("It seems to be taking a REALLY long time, are you sure you want to show all files?", "Show all Files? (last chance!)", MessageBoxButtons.YesNo);
                if (diagResult == DialogResult.No) {
                    quitEarly = true;
                    return;
                }
            }
            try {
                foreach (DirectoryInfo subdir in directoryInfo.GetDirectories()) {
                    treeFreeze++;
                    BuildTree(subdir, curNode.Nodes);
                }
                foreach (FileInfo file in directoryInfo.GetFiles()) {
                    if((settings.RemoveUnwanted && file.Name == ".unwanted") || ((file.Attributes & FileAttributes.Hidden) == 0 && (file.Attributes & FileAttributes.System) == 0)) {
                        //Add it to the list if it's name is .unwanted, so that it may be deleted automatically
                        //Otherwise, ignore any hidden or system files
                        TreeNode newNode = new TreeNode(file.Name);
                        newNode.Name = file.FullName;
                        newNode.ContextMenuStrip = contextMenuStripNode;
                        curNode.Nodes.Add(newNode);
                    }
                }
            } catch (System.Exception excpt) {
                Console.WriteLine(excpt.Message);
            }
        }

        public void CopyTreeNodes(TreeView treeview1, TreeView treeview2) {
            TreeNode newTn;
            foreach (TreeNode tn in treeview1.Nodes) {
                if (settings.RemoveUnwanted && tn.Text == ".unwanted") {
                    //do nothing with it
                } else {
                    newTn = new TreeNode(tn.Text, tn.ImageIndex, tn.SelectedImageIndex);
                    newTn.Name = tn.Name;
                    newTn.ContextMenuStrip = tn.ContextMenuStrip;
                    CopyChildren(newTn, tn);
                    treeview2.Nodes.Add(newTn);
                }
            }
        }

        public void CopyChildren(TreeNode parent, TreeNode original) {
            TreeNode newTn;
            foreach (TreeNode tn in original.Nodes) {
                if (settings.RemoveUnwanted && tn.Text == ".unwanted") {
                    //do nothing with it
                } else {
                    newTn = new TreeNode(tn.Text, tn.ImageIndex, tn.SelectedImageIndex);
                    newTn.Name = tn.Name;
                    newTn.ContextMenuStrip = tn.ContextMenuStrip;
                    parent.Nodes.Add(newTn);
                    CopyChildren(newTn, tn);
                }
            }
        }

        private void FolderCombinerLooper(bool warning) {
            bool itworked = FolderCombiner(treeView2.Nodes, treeView2);
            if (!itworked && warning) {
                MessageBoxWithCheckbox msg = new MessageBoxWithCheckbox("This feature only works on folders that contain one file with a similar name.\r\nDelete any unwanted files before trying again.", "No applicable episodes found", MessageBoxButtons.OK);
                msg.Checked = true;
                msg.Width += 50;
                if (msg.ShowDialog() == DialogResult.OK) {
                    settings.Warning.FolderCombiner = !msg.Checked;
                }
                msg.Dispose();
            }
            while (itworked) {
                itworked = FolderCombiner(treeView2.Nodes, treeView2);
            }
        }

        private bool FolderCombiner(TreeNodeCollection tn, TreeView tv) {
            bool booly = false;
            if (tn == null || tn.Count == 0) {
                //A single child node
                return false;
            }
            foreach (TreeNode node in tn) {
                if (node == null)
                    return booly;
                if (node.Nodes.Count == 1) {
                    TreeNode childnode = null;
                    foreach (TreeNode cn in node.Nodes)
                        childnode = cn; //This "foreach" will only apply to one node
                    if (childnode != null && childnode.Nodes.Count == 0)
                        booly |= FolderCombine(node, childnode, tv);
                } else
                    booly |= FolderCombiner(node.Nodes, tv);
            }
            return booly;
        }

        private bool FolderCombine(TreeNode node, TreeNode childnode, TreeView tv) {
            string folderText = RemoveJunk(node.Text);
            string fileType = GetFileType(childnode.Text);
            if (fileType == "" || fileType == null)
                return false;//This is probably an empty directory
            string episodeText = RemoveJunk(childnode.Text);
            string commonText = CommonString(folderText, episodeText);
            if (commonText == null)
                return false;
            Regex rgx = new Regex("");
            Match episodeFormat = FindEpisodeFormat(commonText, ref rgx);
            if (episodeFormat == null || !episodeFormat.Success)
                return false;
            string showText = episodeText.Before(episodeFormat.Value).Trim(whiteSpaces);
            string[] episodeNumbers = SeparateNumbersFromEpisode(episodeFormat.Value, "1");
            string seasonNumber = episodeNumbers[0];

            if (node.Parent == null) {
                //this is a root of the tree, which is the case if only 1 folder is open
                MessageBox.Show("Root level folder combine not yet implemented");
                return false;
            } else {
                string showKey = directory;
                if (!directory.Contains(showText))
                    showKey += "\\" + showText;
                string seasonText = "Season " + seasonNumber;
                string seasonKey = showKey + "\\" + seasonText;
                TreeNode[] foundShow = tv.Nodes.Find(showKey, true);
                if (foundShow.Count() == 1) {
                    //Show folder already exists
                    TreeNode[] foundSeason = tv.Nodes.Find(seasonKey, true);
                    if(foundSeason.Count() == 0) {
                        //try one more time, looking for something more generic
                        seasonNumber = FixLeadingZeroes(seasonNumber, 0);
                        foundSeason = FindAllNodes(tv.Nodes, "(s|S)(e|E)(a|A)(s|S)(o|O)(n|N)( |.|_|-)?0*" + seasonNumber, true);
                    }
                    if (foundSeason.Count() == 1) {
                        //Season folder already exists
                        node.Remove();
                        childnode.Remove();
                        foundSeason[0].Nodes.Add(childnode);
                        foundSeason[0].Expand();
                    } else if (foundSeason.Count() == 0) {
                        //Season folder does not exist
                        node.Text = seasonText;
                        node.Name = seasonKey;
                        node.BackColor = Color.LightGreen;
                        node.Remove();
                        foundShow[0].Nodes.Add(node);
                        foundShow[0].Expand();
                    }
                } else if (foundShow.Count() == 0) {
                    //show folder does not exist
                    TreeNode ShowNode = new TreeNode(showText);
                    node.Text = seasonText;
                    node.Name = seasonKey;
                    ShowNode.ContextMenuStrip = contextMenuStripNode;
                    ShowNode.BackColor = Color.LightGreen;
                    node.BackColor = Color.LightGreen;
                    ShowNode.Name = showKey;
                    node.Parent.Nodes.Add(ShowNode);
                    node.Remove();
                    ShowNode.Nodes.Add(node);
                    ShowNode.Expand();
                }
                return true;
            }
        }

        private void FolderSplitter() {
            MessageBox.Show("Not Programmed yet :)");
        }
        #endregion
        #endregion

        #region menus
        #region treeview2 context menu
        private void renameToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!editingNode)
                EditTree2Node();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
            deleteNodeWithConfirm();
        }

        private void expandAllBelowToolStripMenuItem_Click(object sender, EventArgs e) {
            treeView2.SelectedNode.ExpandAll();
        }

        private void collapseAllBelowToolStripMenuItem_Click(object sender, EventArgs e) {
            treeView2.SelectedNode.Collapse(false);
        }

        private void selectParentToolStripMenuItem_Click(object sender, EventArgs e) {
            TreeView tv = treeView2;
            TreeNode tn = treeView2.SelectedNode;
            if (tn.Parent != null)
                tv.SelectedNode = tn.Parent;
            else
                tv.SelectedNode = tv.Nodes[0];
            tv.SelectedNode.EnsureVisible();
        }

        private void showFilePathToolStripMenuItem_Click(object sender, EventArgs e) {
            MessageBox.Show(treeView2.SelectedNode.Name);
        }
        #endregion

        #region Form menustrip
        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            OpenDirectory();
        }

        private void reopenFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            reOpenFolder();
        }

        private void closeFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            treeView1.Nodes.Clear();
            treeView2.Nodes.Clear();
        }

        private void mKVDeleteTitlesToolStripMenuItem_Click(object sender, EventArgs e) {
            mkvTool.Show();
        }

        private void saveAllChangesToolStripMenuItem_Click(object sender, EventArgs e) {
            if (treeView2.Nodes.Count > 0)
                ApplyAll();
        }

        private void findNextToolStripMenuItem_Click(object sender, EventArgs e) {
            if (textBoxFind.Text != "")
                FindNext(treeView2);
        }

        private void simpleModeToolStripMenuItem_Click(object sender, EventArgs e) {
            //show form simple
        }

        private void advancedModeToolStripMenuItem_Click(object sender, EventArgs e) {
            //show form advanced
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e) {
            settingsForm.Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            //show form about
        }

        private void helpToolStripMenuItem1_Click(object sender, EventArgs e) {
            //show form help
        }
        #endregion
        #endregion

        #region TV show stuff

        private void RenameTVShows(TreeNodeCollection tv) {
            if (tv.Count == 0) {
                //A single child node
                return;
            }
            foreach(TreeNode node in tv) {
                //if (node.Nodes.Count > 1)
                RenameEpisodes(node.Nodes);
                RenameTVShows(node.Nodes);
            }
        }

        private void RenameEpisodes(TreeNodeCollection episodes) {
            //This function assumes it is given a list of episodes in a single TreeNodeCollection
            //The old version of this function would find the commonString and uniqueString, but it turns out that is unneeded
            int count = 0;
            foreach (TreeNode n in episodes) {
                //Only attempt to rename checkmarked non-directories
                if (n.Checked && n.Nodes.Count == 0)
                    count++;
            }
            if (count < 1) {
                return;
            }
            string filetype = "";
            string newText = "";
            string eptxt = "";
            string series = "";
            foreach (TreeNode ep in episodes) {
                eptxt = ep.Text;
                if (ep.Checked && ep.Nodes.Count == 0) {
                    filetype = GetFileType(eptxt);
                    if (filetype != "") {
                        //ignore empty directories
                        newText = RemoveJunk(eptxt);
                        if (newText != "") {
                            Regex rgx = new Regex(@"(s|S)(e|E)(a|A)(s|S)(o|O)(n|N)");
                            Match match = rgx.Match(ep.Parent.Text);
                            if (!match.Success)
                                series = ep.Parent.Text;
                            else if (ep.Parent.Parent != null)
                                series = ep.Parent.Parent.Text;
                            else
                                series = "";
                            string oldText = ep.Text;
                            if (settings.TV.FormatEpisode)
                                ep.Text = FormatEpisode(newText, ep.Parent.Text, series) + filetype;
                            else
                                ep.Text = newText + filetype;
                            if(oldText != ep.Text)
                                ep.BackColor = Color.LightGreen;
                        }
                    }
                }
            }
        }

        public string FormatEpisode(string episode, string season, string series) {
            //Episode filenames can be broken into 3 parts
            //{Series Name} + {Episode Number} + {Episode Title}
            //After the junk is removed and the pattern for the show established, this function formats the episode in the preferred format
            //assumes filetype is already removed
            string ep = episode;

            string episodeNumber = FormatEpisodeNumber(ref ep, season);
            string episodeSeries = null;
            string episodeTitle = null;
            if (episodeNumber != null) {
                episodeSeries = ep.Before(episodeNumber).TrimEnd(whiteSpaces);
                episodeTitle = ep.After(episodeNumber).TrimStart(whiteSpaces);
                //MessageBox.Show(episodeSeries + "\r\n" + episodeNumber + "\r\n" + episodeTitle);
                ep = "";
                if (settings.TV.UseSeries) {
                    if ((settings.TV.ForceGuessSeries && series != "") || (settings.TV.GuessSeries && (episodeSeries == "" || episodeSeries == null)))
                        episodeSeries = series;
                    ep += episodeSeries + settings.TV.SeriesSeparator;
                }
                ep += episodeNumber;
                if (settings.TV.UseTitle)
                    ep += settings.TV.TitleSeparator + episodeTitle;
                ep = ep.TrimStart(whiteSpaces).TrimEnd(whiteSpaces);
            }
            return ep;
        }

        public string FormatEpisodeNumber(ref string fullEpisodeText, string season) {
            //This will convert the numbering system in the episode to the preferred system
            //returns the formatted episode number (such as S01E01) and modifies fullEdpisodeText to include the formatted ep number
            string ep = fullEpisodeText;
            string formatted = null;
            const string replaceText = @"<>"; //This was chosen because these characters are not allowed in file names
            Regex rgx = null;
            Match match = FindEpisodeFormat(ep, ref rgx);
            if (match == null)
                return null;

            if (match.Success) {
                if (ep.Contains(replaceText)) {
                    MessageBox.Show("Can not format episodes that contain \"<>\" in their filename");
                    return null;
                }
                ep = rgx.Replace(ep, replaceText, 1); //only replace the first match, incase the title has some numbers in it
                string[] n = SeparateNumbersFromEpisode(match.Value, season);
                if (n == null || n.Length < 2 || n.Length > 3) {
                    MessageBox.Show("Something went wrong when trying to Format Episode Numbers.");
                    return null;
                }
                if(!settings.TV.OnlyEpisode)
                    formatted = settings.TV.Season + n[0] + settings.TV.Episode + n[1];
                else
                    formatted = settings.TV.Episode + n[1];
                if (n.Length == 3)
                    formatted += settings.TV.Double + n[2];
                rgx = new Regex(replaceText);
                ep = rgx.Replace(ep, formatted);
                fullEpisodeText = ep;
                return formatted;
            }

            return formatted;
        }

        public string[] SeparateNumbersFromEpisode(string mv, string season) {
            //mv is the matched value found in FormatEpisodeNumber, such as S01E01
            
            string sn = "1";
            string en = null;
            string[] n = null;
            int i = 0;
            Regex rgx = new Regex(@"\d+");
            Match seasonNumber = rgx.Match(season);
            if (seasonNumber.Success)
                sn = seasonNumber.Value;
            MatchCollection matches = rgx.Matches(mv);
            if (matches.Count == 0)
                return null;//this shouldn't happen
            if (matches.Count > 1) {
                //this is the normal case, when numbers are separated nicely
                n = new string[matches.Count];
                foreach (Match m in matches) {
                    if(i == 0)
                        n[i] = FixLeadingZeroes(m.Value,settings.TV.SeasonDigits);
                    else
                        n[i] = FixLeadingZeroes(m.Value, settings.TV.EpisodeDigits);
                    i++;
                }
            }
            if (matches.Count == 1) {
                //Here is when things get tricky, and we must take an educated guess
                string number = rgx.Match(mv).Value;
                switch (number.Length) {
                    case 0:
                        return null; //wat?
                    case 1:
                        sn = FixLeadingZeroes(sn, settings.TV.SeasonDigits);
                        en = FixLeadingZeroes(number, settings.TV.EpisodeDigits);
                        n = new string[2] { sn, en};
                        break;
                    case 2:
                        //Best assumption is that there isn't a season number
                        sn = FixLeadingZeroes(sn, settings.TV.SeasonDigits);
                        en = FixLeadingZeroes(number, settings.TV.EpisodeDigits);
                        n = new string[2] { sn, en };
                        break;
                    case 3:
                        //Most likely in SEE format, but it could be just EEE
                        sn = number.Substring(0, 1);
                        en = number.Substring(1, 2);
                        sn = FixLeadingZeroes(sn, settings.TV.SeasonDigits);
                        en = FixLeadingZeroes(en, settings.TV.EpisodeDigits);
                        n = new string[2] { sn, en };
                        break;
                    case 4:
                        //most likely in SSEE format, but small chance of SEEE
                        sn = number.Substring(0, 2);
                        en = number.Substring(2, 2);
                        sn = FixLeadingZeroes(sn, settings.TV.SeasonDigits);
                        en = FixLeadingZeroes(en, settings.TV.EpisodeDigits);
                        n = new string[2] { sn,  en };
                        break;
                    default:
                        //Assume it's in some format of SSEEE
                        sn = number.Substring(0, number.Length / 2);
                        en = number.Substring(number.Length / 2, (int)Math.Ceiling((double)number.Length / 2));
                        sn = FixLeadingZeroes(sn, settings.TV.SeasonDigits);
                        en = FixLeadingZeroes(en, settings.TV.EpisodeDigits);
                        n = new string[2] { sn , en };
                        break;
                }
            }
            return n;
        }

        public static string FixLeadingZeroes(string s, int digits) {
            string t = s;
            while ((t.Length > digits) && t.StartsWith("0")) {
                t = t.After("0");
            }
            while (t.Length < digits) {
                t = "0" + t;
            }
            return t;
        }

        public static string FixLeadingZeroes(string s) {
            return FixLeadingZeroes(s, 0);
        }

        public Match FindEpisodeFormat(string ep, ref Regex rgx) {
            Match match;
            /*
            The episodeFormats look for a string that matches a common numbering system for TV episodes
            [0] is for double episodes, such as S1E1E2, S01E01&E02, or Season 1 Episode 1 and 1
            [1] is for common episodes1, such as S01E01 or ep101
            [2] is for common episodes2, such as Season 01 Episode 01
            [3] is for common episodes3, such as 1x01, 01.01
            [4] is for common episodes4, such as Ep 01 or episode 1
            [5] is for common episodes5, such as 101, 01
            
            */
            string[] episodeFormats = new string[6] {
                @"((S|s)([EASONeason])*( |\.|-|_)?)?\d+([A-Z]|[a-z]| |\.|-|_)+\d+((([A-Z]|[a-z]| |\.|-|_|&|\+){1,3})|([ _\.\-ANDand]{3,5}))(e|E|x|X)?\d+",
                @"(S|s|E|e)([A-Z]|[a-z])?\d+([A-Z]|[a-z]| |\.|-|_){0,3}\d+",
                @"(S|s)(E|e)(a|A)(s|S)(o|O)(n|N)(| |\.|-|_)?\d+([A-Z]|[a-z]| |\.|-|_)*\d+",
                @"\d{1,2}([A-Z]|[a-z]|\.)\d{1,2}",
                @"([e|E][p|P][a-z]*[A-Z]*[ |\-|\.|_]?)?\d+",
                @"\d{1,3}"
            };
            for (int i = 0; i < episodeFormats.Length; i++) {
                rgx = new Regex(episodeFormats[i]);
                match = rgx.Match(ep);
                if (match.Success) {
                    //sometimes a number in the episode title can cause problems
                    if (i == 0) {
                        if (DoubleEpisodeFound(match.Value))
                            return match;
                        //else: look for a different match
                    } else {
                        return match;
                    }

                }
            }
            return null;
        }

        public bool DoubleEpisodeFound(string mv) {
            Regex rgx = new Regex(@"\d+");
            MatchCollection matches = rgx.Matches(mv);
            if(matches.Count != 3)
                return false;
            int[] nums = new int[matches.Count];
            int j = 0;
            foreach(Match m in matches) {
                nums[j] = Int32.Parse(m.Value);
                j++;
            }
            if (nums[2] == nums[1]+1)
                return true;
            return false;
        }



        #endregion

        
    }
}
