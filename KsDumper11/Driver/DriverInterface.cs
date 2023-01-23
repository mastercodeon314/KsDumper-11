using System;
using System.IO;
using System.Runtime.InteropServices;
using KsDumper11.Utility;

namespace KsDumper11.Driver
{
	// Token: 0x02000014 RID: 20
	public class DriverInterface
	{
        public static bool IsDriverOpen(string registryPath)
        {
            IntPtr handle = WinApi.CreateFileA(registryPath, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, (FileAttributes)0, IntPtr.Zero);
            bool result = handle != WinApi.INVALID_HANDLE_VALUE;
            WinApi.CloseHandle(handle);
            return result;
        }

        // Token: 0x060000D9 RID: 217 RVA: 0x00005D59 File Offset: 0x00003F59
        public DriverInterface(string registryPath)
		{
			this.driverHandle = WinApi.CreateFileA(registryPath, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, (FileAttributes)0, IntPtr.Zero);
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00005D80 File Offset: 0x00003F80
		public bool HasValidHandle()
		{
			return this.driverHandle != WinApi.INVALID_HANDLE_VALUE;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00005DA4 File Offset: 0x00003FA4
		public bool GetProcessSummaryList(out ProcessSummary[] result)
		{
			result = new ProcessSummary[0];
			bool flag = this.driverHandle != WinApi.INVALID_HANDLE_VALUE;
			if (flag)
			{
				int requiredBufferSize = this.GetProcessListRequiredBufferSize();
				bool flag2 = requiredBufferSize > 0;
				if (flag2)
				{
					IntPtr bufferPointer = MarshalUtility.AllocZeroFilled(requiredBufferSize);
					Operations.KERNEL_PROCESS_LIST_OPERATION operation = new Operations.KERNEL_PROCESS_LIST_OPERATION
					{
						bufferAddress = (ulong)bufferPointer.ToInt64(),
						bufferSize = requiredBufferSize
					};
					IntPtr operationPointer = MarshalUtility.CopyStructToMemory<Operations.KERNEL_PROCESS_LIST_OPERATION>(operation);
					int operationSize = Marshal.SizeOf<Operations.KERNEL_PROCESS_LIST_OPERATION>();
					bool flag3 = WinApi.DeviceIoControl(this.driverHandle, Operations.IO_GET_PROCESS_LIST, operationPointer, operationSize, operationPointer, operationSize, IntPtr.Zero, IntPtr.Zero);
					if (flag3)
					{
						operation = MarshalUtility.GetStructFromMemory<Operations.KERNEL_PROCESS_LIST_OPERATION>(operationPointer, true);
						bool flag4 = operation.processCount > 0;
						if (flag4)
						{
							byte[] managedBuffer = new byte[requiredBufferSize];
							Marshal.Copy(bufferPointer, managedBuffer, 0, requiredBufferSize);
							Marshal.FreeHGlobal(bufferPointer);
							result = new ProcessSummary[operation.processCount];
							using (BinaryReader reader = new BinaryReader(new MemoryStream(managedBuffer)))
							{
								for (int i = 0; i < result.Length; i++)
								{
									result[i] = ProcessSummary.FromStream(reader);
								}
							}
							return true;
						}
					}
				}
			}
			return false;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00005EF4 File Offset: 0x000040F4
		private int GetProcessListRequiredBufferSize()
		{
			IntPtr operationPointer = MarshalUtility.AllocEmptyStruct<Operations.KERNEL_PROCESS_LIST_OPERATION>();
			int operationSize = Marshal.SizeOf<Operations.KERNEL_PROCESS_LIST_OPERATION>();
			bool flag = WinApi.DeviceIoControl(this.driverHandle, Operations.IO_GET_PROCESS_LIST, operationPointer, operationSize, operationPointer, operationSize, IntPtr.Zero, IntPtr.Zero);
			if (flag)
			{
				Operations.KERNEL_PROCESS_LIST_OPERATION operation = MarshalUtility.GetStructFromMemory<Operations.KERNEL_PROCESS_LIST_OPERATION>(operationPointer, true);
				bool flag2 = operation.processCount == 0 && operation.bufferSize > 0;
				if (flag2)
				{
					return operation.bufferSize;
				}
			}
			return 0;
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00005F68 File Offset: 0x00004168
		public bool CopyVirtualMemory(int targetProcessId, IntPtr targetAddress, IntPtr bufferAddress, int bufferSize)
		{
			bool flag = this.driverHandle != WinApi.INVALID_HANDLE_VALUE;
			bool flag2;
			if (flag)
			{
				Operations.KERNEL_COPY_MEMORY_OPERATION operation = new Operations.KERNEL_COPY_MEMORY_OPERATION
				{
					targetProcessId = targetProcessId,
					targetAddress = (ulong)targetAddress.ToInt64(),
					bufferAddress = (ulong)bufferAddress.ToInt64(),
					bufferSize = bufferSize
				};
				IntPtr operationPointer = MarshalUtility.CopyStructToMemory<Operations.KERNEL_COPY_MEMORY_OPERATION>(operation);
				bool result = WinApi.DeviceIoControl(this.driverHandle, Operations.IO_COPY_MEMORY, operationPointer, Marshal.SizeOf<Operations.KERNEL_COPY_MEMORY_OPERATION>(), IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero);
				Marshal.FreeHGlobal(operationPointer);
				flag2 = result;
			}
			else
			{
				flag2 = false;
			}
			return flag2;
		}

        public bool UnloadDriver()
        {
            if (driverHandle != WinApi.INVALID_HANDLE_VALUE)
            {
				bool result = WinApi.DeviceIoControl(driverHandle, Operations.IO_UNLOAD_DRIVER, IntPtr.Zero, 0, IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero);
				this.Dispose();
				return result;
            }
            return false;
        }

        // Token: 0x04000075 RID: 117
        private readonly IntPtr driverHandle;

        public void Dispose()
        {
            WinApi.CloseHandle(driverHandle);
        }

        ~DriverInterface()
        {
			try
			{
				WinApi.CloseHandle(driverHandle);
			}
			catch (Exception ex)
			{
				return;
			}
        }
    }
}
