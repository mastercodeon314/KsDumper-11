using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DarkControls;
using KsDumper11.Driver;
using KsDumper11.PE;
using KsDumper11.Utility;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace KsDumper11
{
    public partial class DumperForm : Form
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 33554432;
                return cp;
            }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetSecurityInfo(int HANDLE, int SE_OBJECT_TYPE, int SECURITY_INFORMATION, int psidOwner, int psidGroup, out IntPtr pDACL, IntPtr pSACL, out IntPtr pSecurityDescriptor);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int SetSecurityInfo(int HANDLE, int SE_OBJECT_TYPE, int SECURITY_INFORMATION, int psidOwner, int psidGroup, IntPtr pDACL, IntPtr pSACL);

        [DllImport("ntdll.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ZwSuspendProcess(IntPtr hProcess);

        [DllImport("ntdll.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ZwResumeProcess(IntPtr hProcess);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TOKEN_PRIVILEGES
        {
            public int PrivilegeCount;

            public long Luid;

            public int Attributes;
        }

        public struct SYSTEM_INFO
        {
            public uint dwOemId;
            public uint dwPageSize;
            public uint lpMinimumApplicationAddress;
            public uint lpMaximumApplicationAddress;
            public uint dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public uint dwProcessorLevel;
            public uint dwProcessorRevision;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct PROCESSENTRY32
        {
            private const int MAX_PATH = 260;
            internal uint dwSize;
            internal uint cntUsage;
            internal uint th32ProcessID;
            internal IntPtr th32DefaultHeapID;
            internal uint th32ModuleID;
            internal uint cntThreads;
            internal uint th32ParentProcessID;
            internal int pcPriClassBase;
            internal uint dwFlags;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            internal string szExeFile;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct PROCESS_BASIC_INFORMATION
        {
            public int Size
            {
                get
                {
                    return 24;
                }
            }

            public int ExitStatus;
            public int PebBaseAddress;
            public int AffinityMask;
            public int BasePriority;
            public int UniqueProcessId;
            public int InheritedFromUniqueProcessId;
        }

        private readonly KsDumperDriverInterface driver;
        private readonly ProcessDumper dumper;
        private System.Windows.Forms.Timer t;

        bool skip_closeDriverOnExitBox_CheckedChanged_Event = false;
        bool skip_antiantiDebuggerToolsBox_CheckedChanged_Event = false;

        List<LabelInfo> labelInfos = new List<LabelInfo>();

        JsonSettingsManager settingsManager;

        LabelDrawer labelDrawer;

        public DumperForm()
        {
            this.InitializeComponent();

            settingsManager = new JsonSettingsManager();

            skip_closeDriverOnExitBox_CheckedChanged_Event = true;
            closeDriverOnExitBox.Checked = settingsManager.JsonSettings.closeDriverOnExit;

            skip_closeDriverOnExitBox_CheckedChanged_Event = true;
            antiantiDebuggerToolsBox.Checked = settingsManager.JsonSettings.enableAntiAntiDebuggerTools;

            this.FormClosing += Dumper_FormClosing;
            this.Disposed += Dumper_Disposed;
            this.appIcon1.DragForm = this;
            base.FormBorderStyle = FormBorderStyle.None;
            base.Region = Region.FromHrgn(Utils.CreateRoundRectRgn(0, 0, base.Width, base.Height, 10, 10));
            this.closeBtn.Region = Region.FromHrgn(Utils.CreateRoundRectRgn(0, 0, this.closeBtn.Width, this.closeBtn.Height, 10, 10));
            this.groupBox1.ForeColor = Color.Silver;
            foreach (object obj in this.groupBox1.Controls)
            {
                Control c = (Control)obj;
                c.ForeColor = this.groupBox1.ForeColor;
            }
            this.processList.HeaderStyle = ColumnHeaderStyle.Clickable;
            this.processList.ColumnWidthChanging += this.processList_ColumnWidthChanging;
            this.driver = new KsDumperDriverInterface("\\\\.\\KsDumper");
            this.dumper = new ProcessDumper(this.driver);

            this.LoadProcessList();
        }

        private void Dumper_Load(object sender, EventArgs e)
        {
            if (antiantiDebuggerToolsBox.Checked)
            {
                labelDrawer = new LabelDrawer(this);

                SnifferBypass.SelfTitle(this.Handle);

                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl == groupBox1) continue;

                    SnifferBypass.SelfTitle(ctrl.Handle);
                }

                this.Text = SnifferBypass.GenerateRandomString(this.Text.Length);
            }

            Logger.OnLog += this.Logger_OnLog;
            Logger.Log("KsDumper 11 - [By EquiFox] Given Newlife", Array.Empty<object>());
        }

        private void Dumper_Disposed(object sender, EventArgs e)
        {
            if (settingsManager.JsonSettings.closeDriverOnExit)
            {
                driver.UnloadDriver();
            }
        }

        private void closeDriverOnExitBox_CheckedChanged(object sender, EventArgs e)
        {
            if (skip_closeDriverOnExitBox_CheckedChanged_Event)
            {
                skip_closeDriverOnExitBox_CheckedChanged_Event = false;
                return;
            }

            settingsManager.JsonSettings.closeDriverOnExit = closeDriverOnExitBox.Checked;
            settingsManager.Save();
        }

        private void antiantiDebuggerToolsBox_CheckedChanged(object sender, EventArgs e)
        {
            if (skip_antiantiDebuggerToolsBox_CheckedChanged_Event)
            {
                skip_antiantiDebuggerToolsBox_CheckedChanged_Event = false;
                return;
            }

            settingsManager.JsonSettings.enableAntiAntiDebuggerTools = antiantiDebuggerToolsBox.Checked;
            settingsManager.Save();
        }

        private void Dumper_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (closeDriverOnExitBox.Checked)
            {
                driver.UnloadDriver();
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            bool flag = m.Msg == Utils.WM_NCHITTEST;
            if (flag)
            {
                m.Result = (IntPtr)Utils.HT_CAPTION;
            }
        }

        private void processList_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            Console.Write("Column Resizing");
            e.NewWidth = this.processList.Columns[e.ColumnIndex].Width;
            e.Cancel = true;
        }



        private void LoadProcessList()
        {
            bool flag = this.driver.HasValidHandle();
            if (flag)
            {
                ProcessSummary[] result;
                bool processSummaryList = this.driver.GetProcessSummaryList(out result);
                if (processSummaryList)
                {
                    this.processList.LoadProcesses(result);
                }
                else
                {
                    MessageBox.Show("Unable to retrieve process list !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        private bool DumpProcess(ProcessSummary process)
        {
            bool flag = this.driver.HasValidHandle();
            bool flag2;
            if (flag)
            {
                Logger.Log("Valid driver handle open", Array.Empty<object>());
                bool sucess = false;
                Task.Run(delegate ()
                {
                    Logger.Log("Dumping process...", Array.Empty<object>());
                    PEFile peFile;
                    sucess = this.dumper.DumpProcess(process, out peFile);
                    if (sucess)
                    {
                        Logger.Log("Sucess!", Array.Empty<object>());
                        this.Invoke(new Action(delegate ()
                        {
                            using (SaveFileDialog sfd = new SaveFileDialog())
                            {
                                sfd.FileName = process.ProcessName.Replace(".exe", "_dump.exe");
                                sfd.Filter = "Executable File (.exe)|*.exe";
                                bool flag3 = sfd.ShowDialog() == DialogResult.OK;
                                if (flag3)
                                {
                                    peFile.SaveToDisk(sfd.FileName);
                                    Logger.Log("Saved at '{0}' !", new object[] { sfd.FileName });
                                }
                            }
                        }));
                        Logger.Log(process.ProcessName + "  Killed", Array.Empty<object>());
                        this.KillProcess(process.ProcessId);
                    }
                    else
                    {
                        Logger.Log("Failure", Array.Empty<object>());
                        this.Invoke(new Action(delegate ()
                        {
                            MessageBox.Show("Unable to dump target process !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }));
                    }
                });
                flag2 = sucess;
            }
            else
            {
                MessageBox.Show("Unable to communicate with driver ! Make sure it is loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                flag2 = false;
            }
            return flag2;
        }

        private bool DumpProcess(Process process)
        {
            bool flag = this.driver.HasValidHandle();
            bool flag3;
            if (flag)
            {
                Logger.Log("Valid driver handle open", Array.Empty<object>());
                Logger.Log("Dumping process...", Array.Empty<object>());
                PEFile peFile;
                bool sucess = this.dumper.DumpProcess(process, out peFile);
                bool flag2 = sucess;
                if (flag2)
                {
                    Logger.Log("Sucess!", Array.Empty<object>());
                    base.Invoke(new Action(delegate ()
                    {
                        using (SaveFileDialog sfd = new SaveFileDialog())
                        {
                            sfd.FileName = process.ProcessName + "_dump.exe";
                            sfd.Filter = "Executable File (.exe)|*.exe";
                            bool flag4 = sfd.ShowDialog() == DialogResult.OK;
                            if (flag4)
                            {
                                peFile.SaveToDisk(sfd.FileName);
                                Logger.Log("Saved at '{0}' !", new object[] { sfd.FileName });
                            }
                        }
                    }));
                    Logger.Log(process.ProcessName + "  Killed", Array.Empty<object>());
                    this.KillProcess(process.Id);
                }
                else
                {
                    Logger.Log("Failure", Array.Empty<object>());
                    Logger.Log(process.ProcessName + "  Killed", Array.Empty<object>());
                    this.KillProcess(process.Id);
                    base.Invoke(new Action(delegate ()
                    {
                        MessageBox.Show("Unable to dump target process !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }));
                }
                flag3 = sucess;
            }
            else
            {
                MessageBox.Show("Unable to communicate with driver ! Make sure it is loaded.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                Logger.Log(process.ProcessName + "  Killed", Array.Empty<object>());
                this.KillProcess(process.Id);
                flag3 = false;
            }
            return flag3;
        }

        private void dumpMainModuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessSummary targetProcess = this.processList.SelectedItems[0].Tag as ProcessSummary;
            this.DumpProcess(targetProcess);
        }

        private void Logger_OnLog(string message)
        {
            this.logsTextBox.Invoke(new Action(delegate ()
            {
                this.logsTextBox.AppendText(message);
                this.logsTextBox.Update();
            }));
        }

        private void refreshMenuBtn_Click(object sender, EventArgs e)
        {
            this.LoadProcessList();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            e.Cancel = this.processList.SelectedItems.Count == 0;
        }

        private void logsTextBox_TextChanged(object sender, EventArgs e)
        {
            this.logsTextBox.SelectionStart = this.logsTextBox.Text.Length;
            this.logsTextBox.ScrollToCaret();
        }

        private void openInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessSummary targetProcess = this.processList.SelectedItems[0].Tag as ProcessSummary;
            Process.Start("explorer.exe", Path.GetDirectoryName(targetProcess.MainModuleFileName));
        }

        private void suspendProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessSummary targetProcess = this.processList.SelectedItems[0].Tag as ProcessSummary;
            this.SuspendProcess(targetProcess.ProcessId);
        }

        private void KillProcess(int processId)
        {
            IntPtr hProcess = DumperForm.OpenProcess(1081U, 0, (uint)processId);
            bool flag = hProcess == IntPtr.Zero;
            if (flag)
            {
                IntPtr pDACL;
                IntPtr pSecDesc;
                DumperForm.GetSecurityInfo((int)Process.GetCurrentProcess().Handle, 6, 4, 0, 0, out pDACL, IntPtr.Zero, out pSecDesc);
                hProcess = DumperForm.OpenProcess(262144U, 0, (uint)processId);
                DumperForm.SetSecurityInfo((int)hProcess, 6, 536870916, 0, 0, pDACL, IntPtr.Zero);
                DumperForm.CloseHandle(hProcess);
                hProcess = DumperForm.OpenProcess(1080U, 0, (uint)processId);
            }
            try
            {
                DumperForm.TerminateProcess(hProcess, 0U);
            }
            catch
            {
            }
            DumperForm.CloseHandle(hProcess);
        }

        private void SuspendProcess(int processId)
        {
            IntPtr hProcess = DumperForm.OpenProcess(2048U, 0, (uint)processId);
            bool flag = hProcess == IntPtr.Zero;
            if (flag)
            {
                IntPtr pDACL;
                IntPtr pSecDesc;
                DumperForm.GetSecurityInfo((int)Process.GetCurrentProcess().Handle, 6, 4, 0, 0, out pDACL, IntPtr.Zero, out pSecDesc);
                hProcess = DumperForm.OpenProcess(262144U, 0, (uint)processId);
                DumperForm.SetSecurityInfo((int)hProcess, 6, 536870916, 0, 0, pDACL, IntPtr.Zero);
                DumperForm.CloseHandle(hProcess);
                hProcess = DumperForm.OpenProcess(1080U, 0, (uint)processId);
            }
            try
            {
                DumperForm.ZwSuspendProcess(hProcess);
            }
            catch
            {
            }
            DumperForm.CloseHandle(hProcess);
        }

        private void ResumeProcess(int processId)
        {
            IntPtr hProcess = DumperForm.OpenProcess(2048U, 0, (uint)processId);
            bool flag = hProcess == IntPtr.Zero;
            if (flag)
            {
                IntPtr pDACL;
                IntPtr pSecDesc;
                DumperForm.GetSecurityInfo((int)Process.GetCurrentProcess().Handle, 6, 4, 0, 0, out pDACL, IntPtr.Zero, out pSecDesc);
                hProcess = DumperForm.OpenProcess(262144U, 0, (uint)processId);
                DumperForm.SetSecurityInfo((int)hProcess, 6, 536870916, 0, 0, pDACL, IntPtr.Zero);
                DumperForm.CloseHandle(hProcess);
                hProcess = DumperForm.OpenProcess(1080U, 0, (uint)processId);
            }
            try
            {
                DumperForm.ZwResumeProcess(hProcess);
            }
            catch
            {
            }
            DumperForm.CloseHandle(hProcess);
        }

        private void resumeProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessSummary targetProcess = this.processList.SelectedItems[0].Tag as ProcessSummary;
            this.ResumeProcess(targetProcess.ProcessId);
        }

        private void killProcessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessSummary targetProcess = this.processList.SelectedItems[0].Tag as ProcessSummary;
            this.KillProcess(targetProcess.ProcessId);
        }

        private void T_Tick(object sender, EventArgs e)
        {
            this.LoadProcessList();
        }

        private void ClearLog()
        {
            this.logsTextBox.Clear();
        }

        private void StartAndDumpFile(string dumpFile)
        {
            Logger.Log(Path.GetFileName(dumpFile) + "  Started", Array.Empty<object>());
            Process process = Process.Start(dumpFile);
            Thread.Sleep(4);
            this.SuspendProcess(process.Id);
            Logger.Log("Suspending process...", Array.Empty<object>());
            bool flag = this.DumpProcess(process);
            if (flag)
            {
                Logger.Log(Path.GetFileName(dumpFile) + "  Dumped", Array.Empty<object>());
            }
            else
            {
                Logger.Log("process dump failed", Array.Empty<object>());
            }
        }

        private void fileDumpBtn_Click(object sender, EventArgs e)
        {
            this.ClearLog();
            Logger.Log("KsDumper v1.1 - By EquiFox", Array.Empty<object>());
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Executable File (.exe)|*.exe";
            openFileDialog.Title = "File to dump";
            openFileDialog.RestoreDirectory = true;
            bool flag = openFileDialog.ShowDialog() == DialogResult.OK;
            if (flag)
            {
                string dumpFile = openFileDialog.FileName;
                this.StartAndDumpFile(dumpFile);
            }
        }

        private void hideSystemProcessBtn_Click(object sender, EventArgs e)
        {
            bool flag = !this.processList.SystemProcessesHidden;
            if (flag)
            {
                this.processList.HideSystemProcesses();
                this.hideSystemProcessBtn.Text = "Show System Processes";
            }
            else
            {
                this.processList.ShowSystemProcesses();
                this.hideSystemProcessBtn.Text = "Hide System Processes";
            }
        }

        private void refreshBtn_Click(object sender, EventArgs e)
        {
            this.LoadProcessList();
        }

        private void autoRefreshCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool @checked = this.autoRefreshCheckBox.Checked;
            if (@checked)
            {
                bool flag = this.t == null;
                if (flag)
                {
                    this.t = new System.Windows.Forms.Timer();
                    this.t.Tick += this.T_Tick;
                    this.t.Interval = 100;
                    this.t.Start();
                }
                else
                {
                    this.t.Interval = 100;
                    this.t.Start();
                }
            }
            else
            {
                this.t.Stop();
            }
        }

        private void providerBtn_Click(object sender, EventArgs e)
        {
            KsDumperDriverInterface drv = KsDumperDriverInterface.OpenKsDumperDriver();

            drv.UnloadDriver();
            drv.Dispose();

            ProviderSelector prov = new ProviderSelector();

            prov.ShowDialog();

            StartDriver.Start();
        }
    }
}
