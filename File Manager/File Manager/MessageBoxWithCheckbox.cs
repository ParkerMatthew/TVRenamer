using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace File_Manager {
    public partial class MessageBoxWithCheckbox : Form {


        private bool largemessage = false;
        private bool check = false;
        public bool Checked {
            get { return checkBoxRemember.Checked; }
            set { checkBoxRemember.Checked = value; }
        }

        public MessageBoxWithCheckbox(string message, string title, MessageBoxButtons buttons, bool largeMessage) {
            InitializeComponent();
            label1.Text = message;
            if (largeMessage) {
                largemessage = true;
                this.MaximumSize = new Size(0, 0);
                this.Size = new Size(1024, 350);
            }
            
            this.Text = title;
            switch (buttons) {
                case MessageBoxButtons.OK:
                    buttonYes.Visible = false;
                    buttonCancel.Visible = false;
                    buttonNoOk.Text = "Ok";
                    checkBoxRemember.Text = "Do not tell me again.";
                    break;
                case MessageBoxButtons.OKCancel:
                    buttonYes.Visible = false;
                    buttonNoOk.Text = "Ok";
                    checkBoxRemember.Text = "Do not ask me again.";
                    break;
                case MessageBoxButtons.YesNo:
                    buttonCancel.Visible = false;
                    break;
                default:
                    break; //Keep everything as it appears in designer
            }
            InitializeVariables();
        }

        public MessageBoxWithCheckbox(string message, string title, MessageBoxButtons buttons) {
            InitializeComponent();
            label1.Text = message;
            this.Text = title;
            switch (buttons) {
                case MessageBoxButtons.OK:
                    buttonYes.Visible = false;
                    buttonCancel.Visible = false;
                    buttonNoOk.Text = "Ok";
                    checkBoxRemember.Text = "Do not tell me again.";
                    break;
                case MessageBoxButtons.OKCancel:
                    buttonYes.Visible = false;
                    buttonNoOk.Text = "Ok";
                    checkBoxRemember.Text = "Do not ask me again.";
                    break;
                case MessageBoxButtons.YesNo:
                    buttonCancel.Visible = false;
                    break;
                default:
                    break; //Keep everything as it appears in designer
            }
            InitializeVariables();
        }

        public MessageBoxWithCheckbox(string message, string title) {
            InitializeComponent();
            label1.Text = message;
            this.Text = title;
            InitializeVariables();
        }

        public MessageBoxWithCheckbox(string message) {
            InitializeComponent();
            label1.Text = message;
            InitializeVariables();
        }

        public MessageBoxWithCheckbox() {
            InitializeComponent();
            InitializeVariables();
        }

        private void InitializeVariables() {
            this.FormClosing += MessageBoxWithCheckbox_FormClosing;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = largemessage;
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonYes.DialogResult = DialogResult.Yes;
            if (buttonNoOk.Text == "No")
                buttonNoOk.DialogResult = DialogResult.No;
            else
                buttonNoOk.DialogResult = DialogResult.OK;
            buttonNoOk.Click += Button_Click;
            buttonYes.Click += Button_Click;
            buttonCancel.Click += Button_Click;
        }

        private void Button_Click(object sender, EventArgs e) {
            this.FormClosing -= MessageBoxWithCheckbox_FormClosing;
            this.Close();
        }

        private void MessageBoxWithCheckbox_FormClosing(object sender, FormClosingEventArgs e) {
            this.DialogResult = DialogResult.Cancel;
        }

    }
}
