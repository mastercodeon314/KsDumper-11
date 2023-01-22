using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using KsDumperClient.Driver;
using KsDumperClient.PE;
using KsDumperClient.Utility;

namespace KsDumperClient
{
	// Token: 0x02000003 RID: 3
	public class ProcessDumper
	{
		// Token: 0x0600002E RID: 46 RVA: 0x000038AD File Offset: 0x00001AAD
		public ProcessDumper(DriverInterface kernelDriver)
		{
			this.kernelDriver = kernelDriver;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000038C0 File Offset: 0x00001AC0
		private static bool IsWin64Emulator(Process process)
		{
			bool flag = Environment.OSVersion.Version.Major > 5 || (Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1);
			bool retVal;
			return flag && (ProcessDumper.NativeMethods.IsWow64Process(process.Handle, out retVal) && retVal);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x0000392C File Offset: 0x00001B2C
		public bool DumpProcess(Process processSummary, out PEFile outputFile)
		{
			IntPtr basePointer = processSummary.MainModule.BaseAddress;
			NativePEStructs.IMAGE_DOS_HEADER dosHeader = this.ReadProcessStruct<NativePEStructs.IMAGE_DOS_HEADER>(processSummary.Id, basePointer);
			outputFile = null;
			Logger.SkipLine();
			Logger.Log("Targeting Process: {0} ({1})", new object[] { processSummary.ProcessName, processSummary.Id });
			bool isValid = dosHeader.IsValid;
			if (isValid)
			{
				IntPtr peHeaderPointer = basePointer + dosHeader.e_lfanew;
				Logger.Log("PE Header Found: 0x{0:x8}", new object[] { peHeaderPointer.ToInt64() });
				IntPtr dosStubPointer = basePointer + Marshal.SizeOf<NativePEStructs.IMAGE_DOS_HEADER>();
				byte[] dosStub = this.ReadProcessBytes(processSummary.Id, dosStubPointer, dosHeader.e_lfanew - Marshal.SizeOf<NativePEStructs.IMAGE_DOS_HEADER>());
				bool flag = !ProcessDumper.IsWin64Emulator(processSummary);
				PEFile peFile;
				if (flag)
				{
					peFile = this.Dump64BitPE(processSummary.Id, dosHeader, dosStub, peHeaderPointer);
				}
				else
				{
					peFile = this.Dump32BitPE(processSummary.Id, dosHeader, dosStub, peHeaderPointer);
				}
				bool flag2 = peFile != null;
				if (flag2)
				{
					IntPtr sectionHeaderPointer = peHeaderPointer + peFile.GetFirstSectionHeaderOffset();
					Logger.Log("Header is valid ({0}) !", new object[] { peFile.Type });
					Logger.Log("Parsing {0} Sections...", new object[] { peFile.Sections.Length });
					for (int i = 0; i < peFile.Sections.Length; i++)
					{
						NativePEStructs.IMAGE_SECTION_HEADER sectionHeader = this.ReadProcessStruct<NativePEStructs.IMAGE_SECTION_HEADER>(processSummary.Id, sectionHeaderPointer);
						peFile.Sections[i] = new PESection
						{
							Header = PESection.PESectionHeader.FromNativeStruct(sectionHeader),
							InitialSize = (int)sectionHeader.VirtualSize
						};
						this.ReadSectionContent(processSummary.Id, new IntPtr(basePointer.ToInt64() + (long)((ulong)sectionHeader.VirtualAddress)), peFile.Sections[i]);
						sectionHeaderPointer += Marshal.SizeOf<NativePEStructs.IMAGE_SECTION_HEADER>();
					}
					Logger.Log("Aligning Sections...", Array.Empty<object>());
					peFile.AlignSectionHeaders();
					Logger.Log("Fixing PE Header...", Array.Empty<object>());
					peFile.FixPEHeader();
					Logger.Log("Dump Completed !", Array.Empty<object>());
					outputFile = peFile;
					return true;
				}
				Logger.Log("Bad PE Header !", Array.Empty<object>());
			}
			return false;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003B80 File Offset: 0x00001D80
		public bool DumpProcess(ProcessSummary processSummary, out PEFile outputFile)
		{
			IntPtr basePointer = (IntPtr)((long)processSummary.MainModuleBase);
			NativePEStructs.IMAGE_DOS_HEADER dosHeader = this.ReadProcessStruct<NativePEStructs.IMAGE_DOS_HEADER>(processSummary.ProcessId, basePointer);
			outputFile = null;
			Logger.SkipLine();
			Logger.Log("Targeting Process: {0} ({1})", new object[] { processSummary.ProcessName, processSummary.ProcessId });
			bool isValid = dosHeader.IsValid;
			if (isValid)
			{
				IntPtr peHeaderPointer = basePointer + dosHeader.e_lfanew;
				Logger.Log("PE Header Found: 0x{0:x8}", new object[] { peHeaderPointer.ToInt64() });
				IntPtr dosStubPointer = basePointer + Marshal.SizeOf<NativePEStructs.IMAGE_DOS_HEADER>();
				byte[] dosStub = this.ReadProcessBytes(processSummary.ProcessId, dosStubPointer, dosHeader.e_lfanew - Marshal.SizeOf<NativePEStructs.IMAGE_DOS_HEADER>());
				bool flag = !processSummary.IsWOW64;
				PEFile peFile;
				if (flag)
				{
					peFile = this.Dump64BitPE(processSummary.ProcessId, dosHeader, dosStub, peHeaderPointer);
				}
				else
				{
					peFile = this.Dump32BitPE(processSummary.ProcessId, dosHeader, dosStub, peHeaderPointer);
				}
				bool flag2 = peFile != null;
				if (flag2)
				{
					IntPtr sectionHeaderPointer = peHeaderPointer + peFile.GetFirstSectionHeaderOffset();
					Logger.Log("Header is valid ({0}) !", new object[] { peFile.Type });
					Logger.Log("Parsing {0} Sections...", new object[] { peFile.Sections.Length });
					for (int i = 0; i < peFile.Sections.Length; i++)
					{
						NativePEStructs.IMAGE_SECTION_HEADER sectionHeader = this.ReadProcessStruct<NativePEStructs.IMAGE_SECTION_HEADER>(processSummary.ProcessId, sectionHeaderPointer);
						peFile.Sections[i] = new PESection
						{
							Header = PESection.PESectionHeader.FromNativeStruct(sectionHeader),
							InitialSize = (int)sectionHeader.VirtualSize
						};
						this.ReadSectionContent(processSummary.ProcessId, new IntPtr(basePointer.ToInt64() + (long)((ulong)sectionHeader.VirtualAddress)), peFile.Sections[i]);
						sectionHeaderPointer += Marshal.SizeOf<NativePEStructs.IMAGE_SECTION_HEADER>();
					}
					Logger.Log("Aligning Sections...", Array.Empty<object>());
					peFile.AlignSectionHeaders();
					Logger.Log("Fixing PE Header...", Array.Empty<object>());
					peFile.FixPEHeader();
					Logger.Log("Dump Completed !", Array.Empty<object>());
					outputFile = peFile;
					return true;
				}
				Logger.Log("Bad PE Header !", Array.Empty<object>());
			}
			return false;
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003DD4 File Offset: 0x00001FD4
		private PEFile Dump64BitPE(int processId, NativePEStructs.IMAGE_DOS_HEADER dosHeader, byte[] dosStub, IntPtr peHeaderPointer)
		{
			NativePEStructs.IMAGE_NT_HEADERS64 peHeader = this.ReadProcessStruct<NativePEStructs.IMAGE_NT_HEADERS64>(processId, peHeaderPointer);
			bool isValid = peHeader.IsValid;
			PEFile pefile;
			if (isValid)
			{
				pefile = new PE64File(dosHeader, peHeader, dosStub);
			}
			else
			{
				pefile = null;
			}
			return pefile;
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003E08 File Offset: 0x00002008
		private PEFile Dump32BitPE(int processId, NativePEStructs.IMAGE_DOS_HEADER dosHeader, byte[] dosStub, IntPtr peHeaderPointer)
		{
			NativePEStructs.IMAGE_NT_HEADERS32 peHeader = this.ReadProcessStruct<NativePEStructs.IMAGE_NT_HEADERS32>(processId, peHeaderPointer);
			bool isValid = peHeader.IsValid;
			PEFile pefile;
			if (isValid)
			{
				pefile = new PE32File(dosHeader, peHeader, dosStub);
			}
			else
			{
				pefile = null;
			}
			return pefile;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00003E3C File Offset: 0x0000203C
		private T ReadProcessStruct<T>(int processId, IntPtr address) where T : struct
		{
			IntPtr buffer = MarshalUtility.AllocEmptyStruct<T>();
			bool flag = this.kernelDriver.CopyVirtualMemory(processId, address, buffer, Marshal.SizeOf<T>());
			T t;
			if (flag)
			{
				t = MarshalUtility.GetStructFromMemory<T>(buffer, true);
			}
			else
			{
				t = default(T);
			}
			return t;
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003E80 File Offset: 0x00002080
		private bool ReadSectionContent(int processId, IntPtr sectionPointer, PESection section)
		{
			int readSize = section.InitialSize;
			bool flag = sectionPointer == IntPtr.Zero || readSize == 0;
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				bool flag3 = readSize <= 100;
				if (flag3)
				{
					section.DataSize = readSize;
					section.Content = this.ReadProcessBytes(processId, sectionPointer, readSize);
					flag2 = true;
				}
				else
				{
					this.CalculateRealSectionSize(processId, sectionPointer, section);
					bool flag4 = section.DataSize != 0;
					if (flag4)
					{
						section.Content = this.ReadProcessBytes(processId, sectionPointer, section.DataSize);
						flag2 = true;
					}
					else
					{
						flag2 = false;
					}
				}
			}
			return flag2;
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003F18 File Offset: 0x00002118
		private byte[] ReadProcessBytes(int processId, IntPtr address, int size)
		{
			IntPtr unmanagedBytePointer = MarshalUtility.AllocZeroFilled(size);
			this.kernelDriver.CopyVirtualMemory(processId, address, unmanagedBytePointer, size);
			byte[] buffer = new byte[size];
			Marshal.Copy(unmanagedBytePointer, buffer, 0, size);
			Marshal.FreeHGlobal(unmanagedBytePointer);
			return buffer;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003F5C File Offset: 0x0000215C
		private void CalculateRealSectionSize(int processId, IntPtr sectionPointer, PESection section)
		{
			int readSize = section.InitialSize;
			int currentReadSize = readSize % 100;
			bool flag = currentReadSize == 0;
			if (flag)
			{
				currentReadSize = 100;
			}
			IntPtr currentOffset = sectionPointer + readSize - currentReadSize;
			while (currentOffset.ToInt64() >= sectionPointer.ToInt64())
			{
				byte[] buffer = this.ReadProcessBytes(processId, currentOffset, currentReadSize);
				int codeByteCount = this.GetInstructionByteCount(buffer);
				bool flag2 = codeByteCount != 0;
				if (flag2)
				{
					currentOffset += codeByteCount;
					bool flag3 = sectionPointer.ToInt64() < currentOffset.ToInt64();
					if (flag3)
					{
						section.DataSize = (int)(currentOffset.ToInt64() - sectionPointer.ToInt64());
						section.DataSize += 4;
						bool flag4 = section.InitialSize < section.DataSize;
						if (flag4)
						{
							section.DataSize = section.InitialSize;
						}
					}
					break;
				}
				currentReadSize = 100;
				currentOffset -= currentReadSize;
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x0000404C File Offset: 0x0000224C
		private int GetInstructionByteCount(byte[] dataBlock)
		{
			for (int i = dataBlock.Length - 1; i >= 0; i--)
			{
				bool flag = dataBlock[i] > 0;
				if (flag)
				{
					return i + 1;
				}
			}
			return 0;
		}

		// Token: 0x0400002B RID: 43
		private DriverInterface kernelDriver;

		// Token: 0x02000023 RID: 35
		internal static class NativeMethods
		{
			// Token: 0x060000EF RID: 239
			[DllImport("kernel32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			internal static extern bool IsWow64Process([In] IntPtr process, out bool wow64Process);
		}
	}
}
