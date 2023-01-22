using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace KsDumperClient.Utility
{
	// Token: 0x02000009 RID: 9
	public static class Logger
	{
		// Token: 0x14000001 RID: 1
		// (add) Token: 0x06000059 RID: 89 RVA: 0x00004500 File Offset: 0x00002700
		// (remove) Token: 0x0600005A RID: 90 RVA: 0x00004534 File Offset: 0x00002734
		public static event Action<string> OnLog;

		// Token: 0x0600005B RID: 91 RVA: 0x00004568 File Offset: 0x00002768
		public static void SkipLine()
		{
			bool flag = Logger.OnLog != null;
			if (flag)
			{
				Logger.OnLog("\n");
			}
			else
			{
				Console.WriteLine();
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000045A0 File Offset: 0x000027A0
		public static void Log(string message, params object[] args)
		{
			message = string.Format("[{0}] {1}\n", DateTime.Now.ToLongTimeString(), string.Format(message, args));
			bool flag = Logger.OnLog != null;
			if (flag)
			{
				Logger.OnLog(message);
			}
			else
			{
				Console.WriteLine(message);
			}
		}
	}
}
