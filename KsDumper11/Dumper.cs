using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using DarkControls;
using DarkControls.Controls;
using KsDumper11.Driver;
using KsDumper11.PE;
using KsDumper11.Utility;

namespace KsDumper11
{
	// Token: 0x02000002 RID: 2
	public partial class Dumper : Form
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002048 File Offset: 0x00000248
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 33554432;
				return cp;
			}
		}

		// Token: 0x06000002 RID: 2
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

		// Token: 0x06000003 RID: 3
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int OpenProcessToken(int ProcessHandle, int DesiredAccess, ref int tokenhandle);

		// Token: 0x06000004 RID: 4
		[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
		private static extern int GetCurrentProcess();

		// Token: 0x06000005 RID: 5
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int LookupPrivilegeValue(string lpsystemname, string lpname, ref long lpLuid);

		// Token: 0x06000006 RID: 6
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int AdjustTokenPrivileges(int tokenhandle, int disableprivs, ref Dumper.TOKEN_PRIVILEGES Newstate, int bufferlength, int PreivousState, int Returnlength);

		// Token: 0x06000007 RID: 7
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int GetSecurityInfo(int HANDLE, int SE_OBJECT_TYPE, int SECURITY_INFORMATION, int psidOwner, int psidGroup, out IntPtr pDACL, IntPtr pSACL, out IntPtr pSecurityDescriptor);

		// Token: 0x06000008 RID: 8
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int SetSecurityInfo(int HANDLE, int SE_OBJECT_TYPE, int SECURITY_INFORMATION, int psidOwner, int psidGroup, IntPtr pDACL, IntPtr pSACL);

		// Token: 0x06000009 RID: 9
		[DllImport("ntdll.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool ZwSuspendProcess(IntPtr hProcess);

		// Token: 0x0600000A RID: 10
		[DllImport("ntdll.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool ZwResumeProcess(IntPtr hProcess);

		// Token: 0x0600000B RID: 11
		[DllImport("kernel32")]
		public static extern void GetSystemInfo(ref Dumper.SYSTEM_INFO pSI);

		// Token: 0x0600000C RID: 12
		[DllImport("kernel32.dll")]
		private static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

		// Token: 0x0600000D RID: 13
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CloseHandle(IntPtr hObject);

		// Token: 0x0600000E RID: 14
		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CreateToolhelp32Snapshot([In] uint dwFlags, [In] uint th32ProcessID);

		// Token: 0x0600000F RID: 15
		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool Process32First([In] IntPtr hSnapshot, ref Dumper.PROCESSENTRY32 lppe);

		// Token: 0x06000010 RID: 16
		[DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool Process32Next([In] IntPtr hSnapshot, ref Dumper.PROCESSENTRY32 lppe);

		// Token: 0x06000011 RID: 17
		[DllImport("ntdll.dll", SetLastError = true)]
		private static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref Dumper.PROCESS_BASIC_INFORMATION processInformation, uint processInformationLength, out int returnLength);

		// Token: 0x06000012 RID: 18 RVA: 0x00002078 File Offset: 0x00000278
		public Dumper()
		{
			this.InitializeComponent();
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
			this.driver = new DriverInterface("\\\\.\\KsDumper");
			this.dumper = new ProcessDumper(this.driver);
			this.LoadProcessList();
		}

		// Token: 0x06000013 RID: 19 RVA: 0x000021C4 File Offset: 0x000003C4
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
			bool flag = m.Msg == Utils.WM_NCHITTEST;
			if (flag)
			{
				m.Result = (IntPtr)Utils.HT_CAPTION;
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000021FC File Offset: 0x000003FC
		private void processList_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
		{
			Console.Write("Column Resizing");
			e.NewWidth = this.processList.Columns[e.ColumnIndex].Width;
			e.Cancel = true;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002234 File Offset: 0x00000434
		private void Dumper_Load(object sender, EventArgs e)
		{
			Logger.OnLog += this.Logger_OnLog;
			Logger.Log("KsDumper 11 - [By EquiFox] Given Newlife", Array.Empty<object>());
		}

		// Token: 0x06000016 RID: 22 RVA: 0x0000225C File Offset: 0x0000045C
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

		// Token: 0x06000017 RID: 23 RVA: 0x000022B4 File Offset: 0x000004B4
		private bool DumpProcess(ProcessSummary process)
		{
			bool flag = this.driver.HasValidHandle();
			bool flag2;
			if (flag)
			{
				Logger.Log("Valid driver handle open", Array.Empty<object>());
				bool sucess = false;
				Task.Run(delegate()
				{
					Logger.Log("Dumping process...", Array.Empty<object>());
					PEFile peFile;
					sucess = this.dumper.DumpProcess(process, out peFile);
					if (sucess)
					{
						Logger.Log("Sucess!", Array.Empty<object>());
						this.Invoke(new Action(delegate()
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
						this.Invoke(new Action(delegate()
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

		// Token: 0x06000018 RID: 24 RVA: 0x00002340 File Offset: 0x00000540
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
					base.Invoke(new Action(delegate()
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
					base.Invoke(new Action(delegate()
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

		// Token: 0x06000019 RID: 25 RVA: 0x000024EC File Offset: 0x000006EC
		private void dumpMainModuleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProcessSummary targetProcess = this.processList.SelectedItems[0].Tag as ProcessSummary;
			this.DumpProcess(targetProcess);
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00002520 File Offset: 0x00000720
		private void Logger_OnLog(string message)
		{
			this.logsTextBox.Invoke(new Action(delegate()
			{
				this.logsTextBox.AppendText(message);
				this.logsTextBox.Update();
			}));
		}

		// Token: 0x0600001B RID: 27 RVA: 0x0000255A File Offset: 0x0000075A
		private void refreshMenuBtn_Click(object sender, EventArgs e)
		{
			this.LoadProcessList();
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00002564 File Offset: 0x00000764
		private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
		{
			e.Cancel = this.processList.SelectedItems.Count == 0;
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00002581 File Offset: 0x00000781
		private void logsTextBox_TextChanged(object sender, EventArgs e)
		{
			this.logsTextBox.SelectionStart = this.logsTextBox.Text.Length;
			this.logsTextBox.ScrollToCaret();
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000025AC File Offset: 0x000007AC
		private void openInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProcessSummary targetProcess = this.processList.SelectedItems[0].Tag as ProcessSummary;
			Process.Start("explorer.exe", Path.GetDirectoryName(targetProcess.MainModuleFileName));
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000025EC File Offset: 0x000007EC
		private void suspendProcessToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProcessSummary targetProcess = this.processList.SelectedItems[0].Tag as ProcessSummary;
			this.SuspendProcess(targetProcess.ProcessId);
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002624 File Offset: 0x00000824
		private void KillProcess(int processId)
		{
			IntPtr hProcess = Dumper.OpenProcess(1081U, 0, (uint)processId);
			bool flag = hProcess == IntPtr.Zero;
			if (flag)
			{
				IntPtr pDACL;
				IntPtr pSecDesc;
				Dumper.GetSecurityInfo((int)Process.GetCurrentProcess().Handle, 6, 4, 0, 0, out pDACL, IntPtr.Zero, out pSecDesc);
				hProcess = Dumper.OpenProcess(262144U, 0, (uint)processId);
				Dumper.SetSecurityInfo((int)hProcess, 6, 536870916, 0, 0, pDACL, IntPtr.Zero);
				Dumper.CloseHandle(hProcess);
				hProcess = Dumper.OpenProcess(1080U, 0, (uint)processId);
			}
			try
			{
				Dumper.TerminateProcess(hProcess, 0U);
			}
			catch
			{
			}
			Dumper.CloseHandle(hProcess);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000026D8 File Offset: 0x000008D8
		private void SuspendProcess(int processId)
		{
			IntPtr hProcess = Dumper.OpenProcess(2048U, 0, (uint)processId);
			bool flag = hProcess == IntPtr.Zero;
			if (flag)
			{
				IntPtr pDACL;
				IntPtr pSecDesc;
				Dumper.GetSecurityInfo((int)Process.GetCurrentProcess().Handle, 6, 4, 0, 0, out pDACL, IntPtr.Zero, out pSecDesc);
				hProcess = Dumper.OpenProcess(262144U, 0, (uint)processId);
				Dumper.SetSecurityInfo((int)hProcess, 6, 536870916, 0, 0, pDACL, IntPtr.Zero);
				Dumper.CloseHandle(hProcess);
				hProcess = Dumper.OpenProcess(1080U, 0, (uint)processId);
			}
			try
			{
				Dumper.ZwSuspendProcess(hProcess);
			}
			catch
			{
			}
			Dumper.CloseHandle(hProcess);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x0000278C File Offset: 0x0000098C
		private void ResumeProcess(int processId)
		{
			IntPtr hProcess = Dumper.OpenProcess(2048U, 0, (uint)processId);
			bool flag = hProcess == IntPtr.Zero;
			if (flag)
			{
				IntPtr pDACL;
				IntPtr pSecDesc;
				Dumper.GetSecurityInfo((int)Process.GetCurrentProcess().Handle, 6, 4, 0, 0, out pDACL, IntPtr.Zero, out pSecDesc);
				hProcess = Dumper.OpenProcess(262144U, 0, (uint)processId);
				Dumper.SetSecurityInfo((int)hProcess, 6, 536870916, 0, 0, pDACL, IntPtr.Zero);
				Dumper.CloseHandle(hProcess);
				hProcess = Dumper.OpenProcess(1080U, 0, (uint)processId);
			}
			try
			{
				Dumper.ZwResumeProcess(hProcess);
			}
			catch
			{
			}
			Dumper.CloseHandle(hProcess);
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002840 File Offset: 0x00000A40
		private void resumeProcessToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProcessSummary targetProcess = this.processList.SelectedItems[0].Tag as ProcessSummary;
			this.ResumeProcess(targetProcess.ProcessId);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002878 File Offset: 0x00000A78
		private void killProcessToolStripMenuItem_Click(object sender, EventArgs e)
		{
			ProcessSummary targetProcess = this.processList.SelectedItems[0].Tag as ProcessSummary;
			this.KillProcess(targetProcess.ProcessId);
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000028AF File Offset: 0x00000AAF
		private void T_Tick(object sender, EventArgs e)
		{
			this.LoadProcessList();
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000028B9 File Offset: 0x00000AB9
		private void ClearLog()
		{
			this.logsTextBox.Clear();
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000028C8 File Offset: 0x00000AC8
		private void StartAndDumpFile(string dumpFile)
		{
			Logger.Log(Path.GetFileName(dumpFile) + "  Started", Array.Empty<object>());
			Process process = Process.Start(dumpFile);
			process.WaitForInputIdle();
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

		// Token: 0x06000028 RID: 40 RVA: 0x00002958 File Offset: 0x00000B58
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

		// Token: 0x06000029 RID: 41 RVA: 0x000029C4 File Offset: 0x00000BC4
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

		// Token: 0x0600002A RID: 42 RVA: 0x00002A24 File Offset: 0x00000C24
		private void refreshBtn_Click(object sender, EventArgs e)
		{
			this.LoadProcessList();
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002A30 File Offset: 0x00000C30
		private void autoRefreshCheckBox_CheckedChanged(object sender, EventArgs e)
		{
			bool @checked = this.autoRefreshCheckBox.Checked;
			if (@checked)
			{
				bool flag = this.t == null;
				if (flag)
				{
					this.t = new Timer();
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

		// Token: 0x04000001 RID: 1
		private const int SE_PRIVILEGE_ENABLED = 2;

		// Token: 0x04000002 RID: 2
		private const int TOKEN_ADJUST_PRIVILEGES = 32;

		// Token: 0x04000003 RID: 3
		private const int TOKEN_QUERY = 8;

		// Token: 0x04000004 RID: 4
		private const uint PROCESS_TERMINATE = 1U;

		// Token: 0x04000005 RID: 5
		private const uint PROCESS_CREATE_THREAD = 2U;

		// Token: 0x04000006 RID: 6
		private const uint PROCESS_SET_SESSIONID = 4U;

		// Token: 0x04000007 RID: 7
		private const uint PROCESS_VM_OPERATION = 8U;

		// Token: 0x04000008 RID: 8
		private const uint PROCESS_VM_READ = 16U;

		// Token: 0x04000009 RID: 9
		private const uint PROCESS_VM_WRITE = 32U;

		// Token: 0x0400000A RID: 10
		private const uint PROCESS_DUP_HANDLE = 64U;

		// Token: 0x0400000B RID: 11
		private const uint PROCESS_CREATE_PROCESS = 128U;

		// Token: 0x0400000C RID: 12
		private const uint PROCESS_SET_QUOTA = 256U;

		// Token: 0x0400000D RID: 13
		private const uint PROCESS_SET_INFORMATION = 512U;

		// Token: 0x0400000E RID: 14
		private const uint PROCESS_QUERY_INFORMATION = 1024U;

		// Token: 0x0400000F RID: 15
		private readonly DriverInterface driver;

		// Token: 0x04000010 RID: 16
		private readonly ProcessDumper dumper;

		// Token: 0x04000011 RID: 17
		private Timer t;

		// Token: 0x02000016 RID: 22
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct TOKEN_PRIVILEGES
		{
			// Token: 0x04000078 RID: 120
			public int PrivilegeCount;

			// Token: 0x04000079 RID: 121
			public long Luid;

			// Token: 0x0400007A RID: 122
			public int Attributes;
		}

		// Token: 0x02000017 RID: 23
		public enum ProcessAccess
		{
			// Token: 0x0400007C RID: 124
			AllAccess = 1050235,
			// Token: 0x0400007D RID: 125
			CreateThread = 2,
			// Token: 0x0400007E RID: 126
			DuplicateHandle = 64,
			// Token: 0x0400007F RID: 127
			QueryInformation = 1024,
			// Token: 0x04000080 RID: 128
			SetInformation = 512,
			// Token: 0x04000081 RID: 129
			Terminate = 1,
			// Token: 0x04000082 RID: 130
			VMOperation = 8,
			// Token: 0x04000083 RID: 131
			VMRead = 16,
			// Token: 0x04000084 RID: 132
			VMWrite = 32,
			// Token: 0x04000085 RID: 133
			Synchronize = 1048576
		}

		// Token: 0x02000018 RID: 24
		public struct SYSTEM_INFO
		{
			// Token: 0x04000086 RID: 134
			public uint dwOemId;

			// Token: 0x04000087 RID: 135
			public uint dwPageSize;

			// Token: 0x04000088 RID: 136
			public uint lpMinimumApplicationAddress;

			// Token: 0x04000089 RID: 137
			public uint lpMaximumApplicationAddress;

			// Token: 0x0400008A RID: 138
			public uint dwActiveProcessorMask;

			// Token: 0x0400008B RID: 139
			public uint dwNumberOfProcessors;

			// Token: 0x0400008C RID: 140
			public uint dwProcessorType;

			// Token: 0x0400008D RID: 141
			public uint dwAllocationGranularity;

			// Token: 0x0400008E RID: 142
			public uint dwProcessorLevel;

			// Token: 0x0400008F RID: 143
			public uint dwProcessorRevision;
		}

		// Token: 0x02000019 RID: 25
		[Flags]
		private enum SnapshotFlags : uint
		{
			// Token: 0x04000091 RID: 145
			HeapList = 1U,
			// Token: 0x04000092 RID: 146
			Process = 2U,
			// Token: 0x04000093 RID: 147
			Thread = 4U,
			// Token: 0x04000094 RID: 148
			Module = 8U,
			// Token: 0x04000095 RID: 149
			Module32 = 16U,
			// Token: 0x04000096 RID: 150
			Inherit = 2147483648U,
			// Token: 0x04000097 RID: 151
			All = 31U
		}

		// Token: 0x0200001A RID: 26
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct PROCESSENTRY32
		{
			// Token: 0x04000098 RID: 152
			private const int MAX_PATH = 260;

			// Token: 0x04000099 RID: 153
			internal uint dwSize;

			// Token: 0x0400009A RID: 154
			internal uint cntUsage;

			// Token: 0x0400009B RID: 155
			internal uint th32ProcessID;

			// Token: 0x0400009C RID: 156
			internal IntPtr th32DefaultHeapID;

			// Token: 0x0400009D RID: 157
			internal uint th32ModuleID;

			// Token: 0x0400009E RID: 158
			internal uint cntThreads;

			// Token: 0x0400009F RID: 159
			internal uint th32ParentProcessID;

			// Token: 0x040000A0 RID: 160
			internal int pcPriClassBase;

			// Token: 0x040000A1 RID: 161
			internal uint dwFlags;

			// Token: 0x040000A2 RID: 162
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			internal string szExeFile;
		}

		// Token: 0x0200001B RID: 27
		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		private struct PROCESS_BASIC_INFORMATION
		{
			// Token: 0x17000033 RID: 51
			// (get) Token: 0x060000E0 RID: 224 RVA: 0x00006064 File Offset: 0x00004264
			public int Size
			{
				get
				{
					return 24;
				}
			}

			// Token: 0x040000A3 RID: 163
			public int ExitStatus;

			// Token: 0x040000A4 RID: 164
			public int PebBaseAddress;

			// Token: 0x040000A5 RID: 165
			public int AffinityMask;

			// Token: 0x040000A6 RID: 166
			public int BasePriority;

			// Token: 0x040000A7 RID: 167
			public int UniqueProcessId;

			// Token: 0x040000A8 RID: 168
			public int InheritedFromUniqueProcessId;
		}
	}
}
