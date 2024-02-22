using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace KsDumper11
{
    public struct LabelInfo
    {
        public Font Font;
        public Point Location;
        public string Text;
        public string Name;
        public bool Visible;
    }

    public class LabelDrawer
    {
        private Form ParentForm;
        public LabelInfo[] labelInfos;

        public LabelDrawer(Form parentFrm)
        {
            ParentForm = parentFrm;
            ParentForm.Paint += ParentForm_Paint;

            List<Control> labelsToRemove = new List<Control>();
            List<LabelInfo> infos = new List<LabelInfo>();

            foreach (Control ctrl in ParentForm.Controls)
            {
                if (ctrl is System.Windows.Forms.Label)
                {
                    LabelInfo labelInfo = new LabelInfo();
                    labelInfo.Text = ctrl.Text;
                    labelInfo.Font = ctrl.Font;
                    labelInfo.Location = ctrl.Location;
                    labelInfo.Name = ctrl.Name;
                    labelInfo.Visible = ctrl.Visible;

                    infos.Add(labelInfo);
                    labelsToRemove.Add(ctrl);
                    continue;
                }
            }

            labelInfos = infos.ToArray();

            foreach (Control ctrl in labelsToRemove)
            {
                ParentForm.Controls.Remove(ctrl);
                ctrl.Dispose();
            }
        }

        private void ParentForm_Paint(object sender, PaintEventArgs e)
        {
            foreach (LabelInfo labelInfo in labelInfos)
            {
                if (labelInfo.Visible)
                {
                    PointF location = new PointF(labelInfo.Location.X, (labelInfo.Location.Y));

                    // Draw the text on the form
                    e.Graphics.DrawString(labelInfo.Text, labelInfo.Font, Brushes.Silver, location);
                }
            }
        }
    }
}
