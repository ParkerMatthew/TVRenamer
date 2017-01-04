namespace File_Manager {
    partial class MKVPropEdit {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.radioButtonTitleDelete = new System.Windows.Forms.RadioButton();
            this.radioButtonTitleEdit = new System.Windows.Forms.RadioButton();
            this.radioButtonTitleNothing = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonRun = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanelMainVertSep = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxOutput = new System.Windows.Forms.TextBox();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonSave = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanelMainVertSep.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // radioButtonTitleDelete
            // 
            this.radioButtonTitleDelete.AutoSize = true;
            this.radioButtonTitleDelete.Checked = true;
            this.radioButtonTitleDelete.Location = new System.Drawing.Point(0, 21);
            this.radioButtonTitleDelete.Name = "radioButtonTitleDelete";
            this.radioButtonTitleDelete.Size = new System.Drawing.Size(145, 17);
            this.radioButtonTitleDelete.TabIndex = 0;
            this.radioButtonTitleDelete.TabStop = true;
            this.radioButtonTitleDelete.Text = "Delete title from metadata";
            this.radioButtonTitleDelete.UseVisualStyleBackColor = true;
            // 
            // radioButtonTitleEdit
            // 
            this.radioButtonTitleEdit.AutoSize = true;
            this.radioButtonTitleEdit.Location = new System.Drawing.Point(0, 45);
            this.radioButtonTitleEdit.Name = "radioButtonTitleEdit";
            this.radioButtonTitleEdit.Size = new System.Drawing.Size(207, 17);
            this.radioButtonTitleEdit.TabIndex = 1;
            this.radioButtonTitleEdit.Text = "Edit title in metadata to be the filename";
            this.radioButtonTitleEdit.UseVisualStyleBackColor = true;
            // 
            // radioButtonTitleNothing
            // 
            this.radioButtonTitleNothing.AutoSize = true;
            this.radioButtonTitleNothing.Location = new System.Drawing.Point(0, 69);
            this.radioButtonTitleNothing.Name = "radioButtonTitleNothing";
            this.radioButtonTitleNothing.Size = new System.Drawing.Size(143, 17);
            this.radioButtonTitleNothing.TabIndex = 2;
            this.radioButtonTitleNothing.Text = "Do not edit title metadata";
            this.radioButtonTitleNothing.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.radioButtonTitleDelete);
            this.panel1.Controls.Add(this.radioButtonTitleNothing);
            this.panel1.Controls.Add(this.radioButtonTitleEdit);
            this.panel1.Location = new System.Drawing.Point(3, 153);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(400, 91);
            this.panel1.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(125, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Change hard-coded titles";
            // 
            // buttonRun
            // 
            this.buttonRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRun.Location = new System.Drawing.Point(270, 3);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(130, 38);
            this.buttonRun.TabIndex = 4;
            this.buttonRun.Text = "Run MKVToolNix on all checked mkv files";
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-3, 0);
            this.label1.MaximumSize = new System.Drawing.Size(400, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(335, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "This tool requires MKVToolNix to be installed. Get the latest version at";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.linkLabel2);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.textBoxPath);
            this.panel2.Controls.Add(this.buttonBrowse);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.linkLabel1);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(400, 124);
            this.panel2.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(276, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "MKV Tool Nix install path (ensure mkvpropedit.exe exists)";
            // 
            // textBoxPath
            // 
            this.textBoxPath.Location = new System.Drawing.Point(84, 100);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.Size = new System.Drawing.Size(313, 20);
            this.textBoxPath.TabIndex = 7;
            this.textBoxPath.Text = "C:\\Program Files\\MKVToolNix";
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(3, 97);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(75, 23);
            this.buttonBrowse.TabIndex = 7;
            this.buttonBrowse.Text = "Browse";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(277, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "You should run this tool after applying any other changes.";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(4, 16);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(150, 13);
            this.linkLabel1.TabIndex = 6;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = " https://mkvtoolnix.download/";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // tableLayoutPanelMainVertSep
            // 
            this.tableLayoutPanelMainVertSep.ColumnCount = 1;
            this.tableLayoutPanelMainVertSep.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMainVertSep.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanelMainVertSep.Controls.Add(this.panel1, 0, 2);
            this.tableLayoutPanelMainVertSep.Controls.Add(this.textBoxOutput, 0, 3);
            this.tableLayoutPanelMainVertSep.Controls.Add(this.tableLayoutPanel1, 0, 4);
            this.tableLayoutPanelMainVertSep.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMainVertSep.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelMainVertSep.Name = "tableLayoutPanelMainVertSep";
            this.tableLayoutPanelMainVertSep.RowCount = 5;
            this.tableLayoutPanelMainVertSep.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.tableLayoutPanelMainVertSep.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelMainVertSep.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanelMainVertSep.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMainVertSep.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanelMainVertSep.Size = new System.Drawing.Size(409, 435);
            this.tableLayoutPanelMainVertSep.TabIndex = 7;
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.BackColor = System.Drawing.SystemColors.InfoText;
            this.textBoxOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxOutput.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxOutput.ForeColor = System.Drawing.Color.White;
            this.textBoxOutput.Location = new System.Drawing.Point(3, 253);
            this.textBoxOutput.Multiline = true;
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxOutput.Size = new System.Drawing.Size(403, 129);
            this.textBoxOutput.TabIndex = 7;
            this.textBoxOutput.Text = "MKV Tool Nix Output Text";
            this.textBoxOutput.WordWrap = false;
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(173, 16);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(96, 13);
            this.linkLabel2.TabIndex = 9;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "(Windows Installer)";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.buttonRun, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonSave, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 388);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(403, 44);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSave.Location = new System.Drawing.Point(3, 3);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(130, 38);
            this.buttonSave.TabIndex = 5;
            this.buttonSave.Text = "Save Settings";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // MKVPropEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 435);
            this.Controls.Add(this.tableLayoutPanelMainVertSep);
            this.MinimumSize = new System.Drawing.Size(425, 430);
            this.Name = "MKVPropEdit";
            this.Text = "Matroska Video (MKV) Property Editor Tool";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tableLayoutPanelMainVertSep.ResumeLayout(false);
            this.tableLayoutPanelMainVertSep.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton radioButtonTitleDelete;
        private System.Windows.Forms.RadioButton radioButtonTitleEdit;
        private System.Windows.Forms.RadioButton radioButtonTitleNothing;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMainVertSep;
        private System.Windows.Forms.TextBox textBoxOutput;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button buttonSave;
    }
}