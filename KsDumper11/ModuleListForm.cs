using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using DarkControls;
using KsDumper11.Driver;
using KsDumper11.PE;
using KsDumper11.Utility;

namespace KsDumper11
{
    public partial class ModuleListForm : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        private readonly KsDumperDriverInterface driver;
        private readonly ProcessDumper dumper;
        private readonly ProcessSummary process;

        public ModuleListForm(KsDumperDriverInterface driver, ProcessDumper dumper, ProcessSummary process)
        {
            InitializeComponent();

            this.driver = driver;
            this.dumper = dumper;
            this.process = process;

            this.FormBorderStyle = FormBorderStyle.None;
            this.Region = Region.FromHrgn(Utils.CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
            this.closeBtn.Region = Region.FromHrgn(Utils.CreateRoundRectRgn(0, 0, closeBtn.Width, closeBtn.Height, 10, 10));
            this.appIcon1.DragForm = this;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == Utils.WM_NCHITTEST)
                m.Result = (IntPtr)(Utils.HT_CAPTION);
        }

        private void ModuleListForm_Load(object sender, EventArgs e)
        {
            titleLbl.Text = "Modules - " + process.ProcessName + " (PID: " + process.ProcessId + ")";
            LoadModules();
        }

        private void LoadModules()
        {
            moduleListView.Items.Clear();
            ModuleSummary[] modules;
            bool success = driver.GetModuleList(process.ProcessId, out modules);

            if (success)
            {
                foreach (ModuleSummary mod in modules)
                {
                    ListViewItem item = new ListViewItem(mod.ModuleName);
                    item.SubItems.Add("0x" + mod.ModuleBase.ToString("X"));
                    item.SubItems.Add("0x" + mod.ModuleEntryPoint.ToString("X"));
                    item.SubItems.Add("0x" + mod.ModuleImageSize.ToString("X"));
                    item.SubItems.Add(mod.ModuleFileName);
                    item.Tag = mod;
                    moduleListView.Items.Add(item);
                }
            }
            else
            {
                MessageBox.Show("Unable to retrieve module list!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void moduleContextMenu_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = moduleListView.SelectedItems.Count == 0;
        }

        private void dumpModuleMenuItem_Click(object sender, EventArgs e)
        {
            ModuleSummary targetModule = moduleListView.SelectedItems[0].Tag as ModuleSummary;

            Task.Run(delegate ()
            {
                Logger.Log("Dumping module: " + targetModule.ModuleName, Array.Empty<object>());
                PEFile peFile;
                bool success = dumper.DumpModule(process.ProcessId, targetModule.ModuleBase, process.IsWOW64, targetModule.ModuleName, out peFile);

                if (success)
                {
                    this.Invoke(new Action(delegate ()
                    {
                        using (SaveFileDialog sfd = new SaveFileDialog())
                        {
                            string ext = Path.GetExtension(targetModule.ModuleName);
                            if (string.IsNullOrEmpty(ext)) ext = ".dll";
                            string baseName = Path.GetFileNameWithoutExtension(targetModule.ModuleName);
                            sfd.FileName = baseName + "_dump" + ext;
                            sfd.Filter = "DLL File (.dll)|*.dll|Executable File (.exe)|*.exe|All Files (*.*)|*.*";
                            if (sfd.ShowDialog() == DialogResult.OK)
                            {
                                peFile.SaveToDisk(sfd.FileName);
                                Logger.Log("Module saved at '{0}' !", new object[] { sfd.FileName });
                            }
                        }
                    }));
                }
                else
                {
                    this.Invoke(new Action(delegate ()
                    {
                        MessageBox.Show("Unable to dump module: " + targetModule.ModuleName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }));
                }
            });
        }

        private void moduleListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            using (SolidBrush backBrush = new SolidBrush(Color.FromArgb(45, 45, 45)))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
            }
            using (SolidBrush foreBrush = new SolidBrush(Color.Silver))
            {
                e.Graphics.DrawString(e.Header.Text, e.Font, foreBrush, e.Bounds.X + 3, e.Bounds.Y + 3);
            }
        }

        private void moduleListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void moduleListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }
    }
}
