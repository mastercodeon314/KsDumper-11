namespace KsDumper11
{
    partial class ModuleListForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.moduleListView = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colBase = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colEntryPoint = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colPath = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.moduleContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.dumpModuleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.titleLbl = new DarkControls.Controls.TransparentLabel();
            this.closeBtn = new DarkControls.Controls.WindowsDefaultTitleBarButton();
            this.appIcon1 = new DarkControls.Controls.AppIcon();
            this.moduleContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.appIcon1)).BeginInit();
            this.SuspendLayout();
            //
            // moduleListView
            //
            this.moduleListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.moduleListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.moduleListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colBase,
            this.colEntryPoint,
            this.colSize,
            this.colPath});
            this.moduleListView.ContextMenuStrip = this.moduleContextMenu;
            this.moduleListView.ForeColor = System.Drawing.Color.Silver;
            this.moduleListView.FullRowSelect = true;
            this.moduleListView.HideSelection = false;
            this.moduleListView.Location = new System.Drawing.Point(12, 40);
            this.moduleListView.MultiSelect = false;
            this.moduleListView.Name = "moduleListView";
            this.moduleListView.OwnerDraw = true;
            this.moduleListView.Size = new System.Drawing.Size(760, 410);
            this.moduleListView.TabIndex = 0;
            this.moduleListView.UseCompatibleStateImageBehavior = false;
            this.moduleListView.View = System.Windows.Forms.View.Details;
            this.moduleListView.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.moduleListView_DrawColumnHeader);
            this.moduleListView.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.moduleListView_DrawItem);
            this.moduleListView.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.moduleListView_DrawSubItem);
            //
            // colName
            //
            this.colName.Text = "Module Name";
            this.colName.Width = 160;
            //
            // colBase
            //
            this.colBase.Text = "Base Address";
            this.colBase.Width = 120;
            //
            // colEntryPoint
            //
            this.colEntryPoint.Text = "Entry Point";
            this.colEntryPoint.Width = 120;
            //
            // colSize
            //
            this.colSize.Text = "Image Size";
            this.colSize.Width = 90;
            //
            // colPath
            //
            this.colPath.Text = "Path";
            this.colPath.Width = 270;
            //
            // moduleContextMenu
            //
            this.moduleContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dumpModuleMenuItem});
            this.moduleContextMenu.Name = "moduleContextMenu";
            this.moduleContextMenu.Size = new System.Drawing.Size(160, 26);
            this.moduleContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.moduleContextMenu_Opening);
            //
            // dumpModuleMenuItem
            //
            this.dumpModuleMenuItem.Name = "dumpModuleMenuItem";
            this.dumpModuleMenuItem.Size = new System.Drawing.Size(159, 22);
            this.dumpModuleMenuItem.Text = "Dump Module";
            this.dumpModuleMenuItem.Click += new System.EventHandler(this.dumpModuleMenuItem_Click);
            //
            // titleLbl
            //
            this.titleLbl.AutoSize = true;
            this.titleLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLbl.Location = new System.Drawing.Point(32, 10);
            this.titleLbl.Name = "titleLbl";
            this.titleLbl.Size = new System.Drawing.Size(100, 17);
            this.titleLbl.TabIndex = 1;
            this.titleLbl.Text = "Loaded Modules";
            //
            // closeBtn
            //
            this.closeBtn.ButtonType = DarkControls.Controls.WindowsDefaultTitleBarButton.Type.Close;
            this.closeBtn.ClickColor = System.Drawing.Color.Red;
            this.closeBtn.ClickIconColor = System.Drawing.Color.Black;
            this.closeBtn.HoverColor = System.Drawing.Color.OrangeRed;
            this.closeBtn.HoverIconColor = System.Drawing.Color.Black;
            this.closeBtn.IconColor = System.Drawing.Color.Black;
            this.closeBtn.IconLineThickness = 2;
            this.closeBtn.Location = new System.Drawing.Point(744, 1);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(40, 40);
            this.closeBtn.TabIndex = 2;
            this.closeBtn.Text = "closeBtn";
            this.closeBtn.UseVisualStyleBackColor = true;
            //
            // appIcon1
            //
            this.appIcon1.AppIconImage = global::KsDumper11.Properties.Resources.icons8_crossed_axes_100;
            this.appIcon1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.appIcon1.DragForm = this;
            this.appIcon1.Location = new System.Drawing.Point(5, 4);
            this.appIcon1.Name = "appIcon1";
            this.appIcon1.Scale = 3.5F;
            this.appIcon1.Size = new System.Drawing.Size(28, 28);
            this.appIcon1.TabIndex = 3;
            this.appIcon1.TabStop = false;
            //
            // ModuleListForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.ClientSize = new System.Drawing.Size(784, 461);
            this.Controls.Add(this.appIcon1);
            this.Controls.Add(this.closeBtn);
            this.Controls.Add(this.titleLbl);
            this.Controls.Add(this.moduleListView);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Silver;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "ModuleListForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Loaded Modules";
            this.Load += new System.EventHandler(this.ModuleListForm_Load);
            this.moduleContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.appIcon1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.ListView moduleListView;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colBase;
        private System.Windows.Forms.ColumnHeader colEntryPoint;
        private System.Windows.Forms.ColumnHeader colSize;
        private System.Windows.Forms.ColumnHeader colPath;
        private System.Windows.Forms.ContextMenuStrip moduleContextMenu;
        private System.Windows.Forms.ToolStripMenuItem dumpModuleMenuItem;
        private DarkControls.Controls.TransparentLabel titleLbl;
        private DarkControls.Controls.WindowsDefaultTitleBarButton closeBtn;
        private DarkControls.Controls.AppIcon appIcon1;
    }
}
