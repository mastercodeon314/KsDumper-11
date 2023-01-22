using System;
using KsDumper11.Utility;

namespace KsDumper11.Driver
{
	// Token: 0x02000015 RID: 21
	public static class Operations
	{
		// Token: 0x060000DE RID: 222 RVA: 0x00006004 File Offset: 0x00004204
		private static uint CTL_CODE(int deviceType, int function, int method, int access)
		{
			return (uint)((deviceType << 16) | (access << 14) | (function << 2) | method);
		}

		// Token: 0x060000DF RID: 223 RVA: 0x00006025 File Offset: 0x00004225
		// Note: this type is marked as 'beforefieldinit'.
		static Operations()
		{
		}

		// Token: 0x04000076 RID: 118
		public static readonly uint IO_GET_PROCESS_LIST = Operations.CTL_CODE(WinApi.FILE_DEVICE_UNKNOWN, 5924, WinApi.METHOD_BUFFERED, WinApi.FILE_ANY_ACCESS);

		// Token: 0x04000077 RID: 119
		public static readonly uint IO_COPY_MEMORY = Operations.CTL_CODE(WinApi.FILE_DEVICE_UNKNOWN, 5925, WinApi.METHOD_BUFFERED, WinApi.FILE_ANY_ACCESS);

		// Token: 0x02000038 RID: 56
		public struct KERNEL_PROCESS_LIST_OPERATION
		{
			// Token: 0x040001AE RID: 430
			public ulong bufferAddress;

			// Token: 0x040001AF RID: 431
			public int bufferSize;

			// Token: 0x040001B0 RID: 432
			public int processCount;
		}

		// Token: 0x02000039 RID: 57
		public struct KERNEL_COPY_MEMORY_OPERATION
		{
			// Token: 0x040001B1 RID: 433
			public int targetProcessId;

			// Token: 0x040001B2 RID: 434
			public ulong targetAddress;

			// Token: 0x040001B3 RID: 435
			public ulong bufferAddress;

			// Token: 0x040001B4 RID: 436
			public int bufferSize;
		}
	}
}
