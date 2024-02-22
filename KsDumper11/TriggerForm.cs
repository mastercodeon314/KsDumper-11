using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using DarkControls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace KsDumper11
{
    public partial class TriggerForm : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                // Activate double buffering at the form level.  All child controls will be double buffered as well.
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        }

        JsonSettingsManager settingsManager;
        LabelDrawer labelDrawer;

        public TriggerForm()
        {
            InitializeComponent();

            settingsManager = new JsonSettingsManager();

            this.appIcon1.DragForm = this;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Region = Region.FromHrgn(Utils.CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
            this.closeBtn.Region = Region.FromHrgn(Utils.CreateRoundRectRgn(0, 0, closeBtn.Width, closeBtn.Height, 10, 10));
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == Utils.WM_NCHITTEST)
                m.Result = (IntPtr)(Utils.HT_CAPTION);
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void TriggerForm_Load(object sender, EventArgs e)
        {
            if (settingsManager.JsonSettings.enableAntiAntiDebuggerTools)
            {
                labelDrawer = new LabelDrawer(this);

                SnifferBypass.SelfTitle(this.Handle);

                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is System.Windows.Forms.TextBox) continue;
                    SnifferBypass.SelfTitle(ctrl.Handle);
                }

                this.Text = SnifferBypass.GenerateRandomString(this.Text.Length);
            }
        }
    }
}
