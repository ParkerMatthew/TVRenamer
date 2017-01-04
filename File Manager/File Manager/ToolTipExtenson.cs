using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace File_Manager {
    public static class ToolTipExtenson {
        public static void SetToolTipChildren(this ToolTip tooltip, Control.ControlCollection controls, string caption) {
            //I was going to use this for numericUpDowns, but it turns out I just had a bad copy-paste error
            tooltip.SetToolTip(controls.Owner, caption);
            foreach (Control c in controls) {
                tooltip.SetToolTip(c, caption);
            }
        }
    }
}
