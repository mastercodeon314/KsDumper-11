using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using KsDumper11.Driver;
using KsDumper11.Utility;

namespace KsDumper11
{
	// Token: 0x02000004 RID: 4
	public class ProcessSummary
	{
		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000039 RID: 57 RVA: 0x00004089 File Offset: 0x00002289
		// (set) Token: 0x0600003A RID: 58 RVA: 0x00004091 File Offset: 0x00002291
		public int ProcessId { get; set; }

        // Token: 0x17000003 RID: 3
        // (get) Token: 0x0600003B RID: 59 RVA: 0x0000409A File Offset: 0x0000229A
        // (set) Token: 0x0600003C RID: 60 RVA: 0x000040A2 File Offset: 0x000022A2
        public string ProcessName { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x0600003D RID: 61 RVA: 0x000040AB File Offset: 0x000022AB
		// (set) Token: 0x0600003E RID: 62 RVA: 0x000040B3 File Offset: 0x000022B3
		public ulong MainModuleBase  { get; set; }

        // Token: 0x17000005 RID: 5
        // (get) Token: 0x0600003F RID: 63 RVA: 0x000040BC File Offset: 0x000022BC
        // (set) Token: 0x06000040 RID: 64 RVA: 0x000040C4 File Offset: 0x000022C4
        public string MainModuleFileName { get; set; }

        // Token: 0x17000006 RID: 6
        // (get) Token: 0x06000041 RID: 65 RVA: 0x000040CD File Offset: 0x000022CD
        // (set) Token: 0x06000042 RID: 66 RVA: 0x000040D5 File Offset: 0x000022D5
        public uint MainModuleImageSize { get; set; }

        // Token: 0x17000007 RID: 7
        // (get) Token: 0x06000043 RID: 67 RVA: 0x000040DE File Offset: 0x000022DE
        // (set) Token: 0x06000044 RID: 68 RVA: 0x000040E6 File Offset: 0x000022E6
        public ulong MainModuleEntryPoint { get; set; }

        // Token: 0x17000008 RID: 8
        // (get) Token: 0x06000045 RID: 69 RVA: 0x000040EF File Offset: 0x000022EF
        // (set) Token: 0x06000046 RID: 70 RVA: 0x000040F7 File Offset: 0x000022F7
        public bool IsWOW64 { get; set; }

        // Token: 0x06000047 RID: 71 RVA: 0x00004100 File Offset: 0x00002300
        public static ProcessSummary ProcessSummaryFromID(KsDumperDriverInterface driver, string processName)
		{
			ProcessSummary result = null;
			ProcessSummary[] processes;
			driver.GetProcessSummaryList(out processes);
			bool flag = processes != null;
			if (flag)
			{
				foreach (ProcessSummary process in processes)
				{
					bool flag2 = process.ProcessName.ToLower().Contains(processName.ToLower());
					if (flag2)
					{
						Logger.Log(process.ProcessName + "      " + processName, Array.Empty<object>());
						return process;
					}
				}
			}
			return result;
		}

		// Token: 0x06000048 RID: 72 RVA: 0x0000418C File Offset: 0x0000238C
		private ProcessSummary(int processId, ulong mainModuleBase, string mainModuleFileName, uint mainModuleImageSize, ulong mainModuleEntryPoint, bool isWOW64)
		{
			this.ProcessId = processId;
			this.MainModuleBase = mainModuleBase;
			this.MainModuleFileName = this.FixFileName(mainModuleFileName);
			this.MainModuleImageSize = mainModuleImageSize;
			this.MainModuleEntryPoint = mainModuleEntryPoint;
			this.ProcessName = Path.GetFileName(this.MainModuleFileName);
			this.IsWOW64 = isWOW64;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x000041EC File Offset: 0x000023EC
		private string FixFileName(string fileName)
		{
			bool flag = fileName.StartsWith("\\");
			string text;
			if (flag)
			{
				text = fileName;
			}
			else
			{
				StringBuilder sb = new StringBuilder(256);
				int length = WinApi.GetLongPathName(fileName, sb, sb.Capacity);
				bool flag2 = length > sb.Capacity;
				if (flag2)
				{
					sb.Capacity = length;
					length = WinApi.GetLongPathName(fileName, sb, sb.Capacity);
				}
				text = sb.ToString();
			}
			return text;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00004258 File Offset: 0x00002458
		public static ProcessSummary FromStream(BinaryReader reader)
		{
			return new ProcessSummary(reader.ReadInt32(), reader.ReadUInt64(), Encoding.Unicode.GetString(reader.ReadBytes(512)).Split(new char[1])[0], reader.ReadUInt32(), reader.ReadUInt64(), reader.ReadBoolean());
		}
	}
}
