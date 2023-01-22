using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DarkControls.Controls
{
    public class CustomStatusStrip : StatusStrip
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            // Set the background color to RGB value 33, 33, 33
            this.BackColor = Color.FromArgb(33, 33, 33);

            // Set the foreground color to Silver
            this.ForeColor = Color.Silver;

            e.Graphics.Clear(this.BackColor);

            // Call the base OnPaint method to handle the actual rendering
            base.OnPaint(e);
        }

        //protected override void OnRender (ToolStripRenderEventArgs e)
        //{
        //    // Set the background color to RGB value 33, 33, 33
        //    e.Graphics.Clear(Color.FromArgb(33, 33, 33));
        //    base.OnRenderToolStripBackground(e);
        //}

        //protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
        //{
        //    e.Graphics.Clear(Color.FromArgb(33, 33, 33));
        //    base.OnRenderToolStripContentPanelBackground(e);
        //}

        //protected override void OnRenderItem(ToolStripItemRenderEventArgs e)
        //{
        //    // Set the background color to RGB value 33, 33, 33
        //    e.Item.BackColor = Color.FromArgb(33, 33, 33);

        //    // Set the foreground color to Silver
        //    e.Item.ForeColor = Color.Silver;

        //    // Call the base OnRenderItem method to handle the actual rendering
        //    base.OnRenderItem(e);
        //}
    }
}
