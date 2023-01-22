using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;

namespace DarkControls.Controls
{
    public class CustomCheckedListBox : CheckedListBox
    {
        public CustomCheckedListBox()
        {
            //this.SetStyle(
            //    ControlStyles.OptimizedDoubleBuffer |
            //    ControlStyles.ResizeRedraw,
            //    //ControlStyles.UserPaint,
            //    true);
            //this.DrawMode = DrawMode.OwnerDrawFixed;

            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.ForeColor = Color.Silver;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CheckedItemColor = System.Drawing.Color.Silver;
            this.CheckOnClick = true;
        }



        protected override void OnPaint(PaintEventArgs e)
        {
            Region iRegion = new Region(e.ClipRectangle);
            e.Graphics.FillRegion(new SolidBrush(this.BackColor), iRegion);
            if (this.Items.Count > 0)
            {
                for (int i = 0; i < this.Items.Count; ++i)
                {
                    System.Drawing.Rectangle irect = this.GetItemRectangle(i);
                    if (e.ClipRectangle.IntersectsWith(irect))
                    {
                        if ((this.SelectionMode == SelectionMode.One && this.SelectedIndex == i)
                        || (this.SelectionMode == SelectionMode.MultiSimple && this.SelectedIndices.Contains(i))
                        || (this.SelectionMode == SelectionMode.MultiExtended && this.SelectedIndices.Contains(i)))
                        {
                            OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font,
                                irect, i,
                                DrawItemState.Selected, this.ForeColor,
                                this.BackColor));
                        }
                        else
                        {
                            OnDrawItem(new DrawItemEventArgs(e.Graphics, this.Font,
                                irect, i,
                                DrawItemState.Default, this.ForeColor,
                                this.BackColor));
                        }
                        iRegion.Complement(irect);
                    }
                }
            }
            base.OnPaint(e);
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            Size checkSize = CheckBoxRenderer.GetGlyphSize(e.Graphics, System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal);
            int dx = (e.Bounds.Height - checkSize.Width) / 2;
            e.DrawBackground();
            //e.DrawFocusRectangle();
            if (this.Items.Count > 0)
            {
                //var isChecked = GetItemChecked(e.Index) ? CheckState.Checked : CheckState.Unchecked;
                bool isChecked = GetItemChecked(e.Index);//For some reason e.State doesn't work so we have to do this instead.
                //CheckBoxRenderer.DrawCheckBox(e.Graphics, new Point(dx, e.Bounds.Top + dx), isChecked ? System.Windows.Forms.VisualStyles.CheckBoxState.CheckedNormal : System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedNormal);
                DarkCheckBox.DrawCheckBox(e.Graphics, new Point(dx, e.Bounds.Top + dx), this.GetItemCheckState(e.Index));
                using (StringFormat sf = new StringFormat { LineAlignment = StringAlignment.Center })
                {
                    using (Brush brush = new SolidBrush(isChecked ? CheckedItemColor : ForeColor))
                    {
                        e.Graphics.DrawString(Items[e.Index].ToString(), Font, brush, new Rectangle(e.Bounds.Height, e.Bounds.Top, e.Bounds.Width - e.Bounds.Height, e.Bounds.Height), sf);
                    }
                }
            }

            //base.OnDrawItem(e);
        }
        Color checkedItemColor = Color.Green;
        public Color CheckedItemColor
        {
            get { return checkedItemColor; }
            set
            {
                checkedItemColor = value;
                //Invalidate();
            }
        }
    }
}
