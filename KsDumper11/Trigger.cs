using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KsDumper11
{
    public partial class Trigger : UserControl
    {
        TriggerForm triggerFrm;
        public Trigger()
        {
            InitializeComponent();
            triggerFrm = new TriggerForm();
            this.Click += Trigger_Click;
        }

        private void Trigger_Click(object sender, EventArgs e)
        {
            if (triggerFrm == null)
            {
                triggerFrm = new TriggerForm();
            }
            else
            {
                if (triggerFrm.IsDisposed)
                {
                    triggerFrm = new TriggerForm();
                }
            }

            if (!triggerFrm.Visible)
            {
                triggerFrm.ShowDialog();
            }
        }
    }
}
