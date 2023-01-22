using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace KsDumperClient.Utility
{
	// Token: 0x02000008 RID: 8
	public static class WinApi
	{
		// Token: 0x06000055 RID: 85
		[DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
		public static extern IntPtr CreateFileA([MarshalAs(UnmanagedType.LPStr)] string filename, [MarshalAs(UnmanagedType.U4)] FileAccess access, [MarshalAs(UnmanagedType.U4)] FileShare share, IntPtr securityAttributes, [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition, [MarshalAs(UnmanagedType.U4)] FileAttributes flagsAndAttributes, IntPtr templateFile);

		// Token: 0x06000056 RID: 86
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true, SetLastError = true)]
		public static extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode, IntPtr lpInBuffer, int nInBufferSize, IntPtr lpOutBuffer, int nOutBufferSize, IntPtr lpBytesReturned, IntPtr lpOverlapped);

		// Token: 0x06000057 RID: 87
		[DllImport("kernel32.dll")]
		public static extern int GetLongPathName(string path, StringBuilder pszPath, int cchPath);

		// Token: 0x06000058 RID: 88 RVA: 0x000044DE File Offset: 0x000026DE
		// Note: this type is marked as 'beforefieldinit'.
		static WinApi()
		{
		}

		// Token: 0x04000036 RID: 54
		public static readonly int FILE_DEVICE_UNKNOWN = 34;

		// Token: 0x04000037 RID: 55
		public static readonly int METHOD_BUFFERED = 0;

		// Token: 0x04000038 RID: 56
		public static readonly int FILE_ANY_ACCESS = 0;

		// Token: 0x04000039 RID: 57
		public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);
	}
}
