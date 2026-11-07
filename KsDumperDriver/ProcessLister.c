#include "NTUndocumented.h"
#include "ProcessLister.h"
#include "Utility.h"

static PSYSTEM_PROCESS_INFORMATION GetRawProcessList()
{
	ULONG bufferSize = 0;
	PVOID bufferPtr = NULL;

	if (ZwQuerySystemInformation(SystemProcessInformation, 0, bufferSize, &bufferSize) == STATUS_INFO_LENGTH_MISMATCH)
	{
		bufferPtr = ExAllocatePool(NonPagedPool, bufferSize);

		if (bufferPtr != NULL)
		{
			ZwQuerySystemInformation(SystemProcessInformation, bufferPtr, bufferSize, &bufferSize);
		}
	}
	return (PSYSTEM_PROCESS_INFORMATION)bufferPtr;
}

static ULONG CalculateProcessListOutputSize(PSYSTEM_PROCESS_INFORMATION rawProcessList)
{
	int size = 0;

	while (rawProcessList->NextEntryOffset)
	{
		size += sizeof(PROCESS_SUMMARY);
		rawProcessList = (PSYSTEM_PROCESS_INFORMATION)(((CHAR*)rawProcessList) + rawProcessList->NextEntryOffset);
	}
	return size;
}

static PLDR_DATA_TABLE_ENTRY GetMainModuleDataTableEntry(PPEB64 peb)
{
	if (SanitizeUserPointer(peb, sizeof(PEB64)))
	{
		if (peb->Ldr)
		{
			if (SanitizeUserPointer(peb->Ldr, sizeof(PEB_LDR_DATA)))
			{
				if (!peb->Ldr->Initialized)
				{
					int initLoadCount = 0;

					while (!peb->Ldr->Initialized && initLoadCount++ < 4)
					{
						DriverSleep(250);
					}
				}

				if (peb->Ldr->Initialized)
				{
					return CONTAINING_RECORD(peb->Ldr->InLoadOrderModuleList.Flink, LDR_DATA_TABLE_ENTRY, InLoadOrderLinks);
				}
			}
		}
	}
	return NULL;
}

NTSTATUS GetProcessList(PVOID listedProcessBuffer, INT32 bufferSize, PINT32 requiredBufferSize, PINT32 processCount)
{
	PPROCESS_SUMMARY processSummary = (PPROCESS_SUMMARY)listedProcessBuffer;
	PSYSTEM_PROCESS_INFORMATION rawProcessList = GetRawProcessList();
	PVOID listHeadPointer = rawProcessList;
	*processCount = 0;

	if (rawProcessList)
	{
		int expectedBufferSize = CalculateProcessListOutputSize(rawProcessList);

		if (!listedProcessBuffer || bufferSize < expectedBufferSize)
		{
			*requiredBufferSize = expectedBufferSize;
			return STATUS_INFO_LENGTH_MISMATCH;
		}

		while (rawProcessList->NextEntryOffset)
		{
			PEPROCESS targetProcess;
			PKAPC_STATE state = NULL;

			if (NT_SUCCESS(PsLookupProcessByProcessId(rawProcessList->UniqueProcessId, &targetProcess)))
			{
				PVOID mainModuleBase = NULL;
				PVOID mainModuleEntryPoint = NULL;
				UINT32 mainModuleImageSize = 0;
				PWCHAR mainModuleFileName = NULL;
				BOOLEAN isWow64 = 0;

				__try
				{
					KeStackAttachProcess(targetProcess, &state);

					__try
					{
						mainModuleBase = PsGetProcessSectionBaseAddress(targetProcess);

						if (mainModuleBase)
						{
							PPEB64 peb = (PPEB64)PsGetProcessPeb(targetProcess);

							if (peb)
							{
								PLDR_DATA_TABLE_ENTRY mainModuleEntry = GetMainModuleDataTableEntry(peb);
								mainModuleEntry = SanitizeUserPointer(mainModuleEntry, sizeof(LDR_DATA_TABLE_ENTRY));

								if (mainModuleEntry)
								{
									mainModuleEntryPoint = mainModuleEntry->EntryPoint;
									mainModuleImageSize = mainModuleEntry->SizeOfImage;
									isWow64 = IS_WOW64_PE(mainModuleBase);

									mainModuleFileName = ExAllocatePool(NonPagedPool, 256 * sizeof(WCHAR));
									RtlZeroMemory(mainModuleFileName, 256 * sizeof(WCHAR));
									RtlCopyMemory(mainModuleFileName, mainModuleEntry->FullDllName.Buffer, 256 * sizeof(WCHAR));
								}
							}
						}
					}
					__except (GetExceptionCode())
					{
						DbgPrintEx(0, 0, "Peb Interaction Failed.\n");
					}
				}
				__finally
				{
					KeUnstackDetachProcess(&state);
				}

				if (mainModuleFileName)
				{
					RtlCopyMemory(processSummary->MainModuleFileName, mainModuleFileName, 256 * sizeof(WCHAR));
					ExFreePool(mainModuleFileName);

					processSummary->ProcessId = rawProcessList->UniqueProcessId;
					processSummary->MainModuleBase = mainModuleBase;
					processSummary->MainModuleEntryPoint = mainModuleEntryPoint;
					processSummary->MainModuleImageSize = mainModuleImageSize;
					processSummary->WOW64 = isWow64;

					processSummary++;
					(*processCount)++;
				}

				ObDereferenceObject(targetProcess);
			}

			rawProcessList = (PSYSTEM_PROCESS_INFORMATION)(((CHAR*)rawProcessList) + rawProcessList->NextEntryOffset);
		}

		ExFreePool(listHeadPointer);
		return STATUS_SUCCESS;
	}
}

#define MAX_MODULES 1024

NTSTATUS GetModuleList(INT32 targetProcessId, PVOID moduleBuffer, INT32 bufferSize, PINT32 requiredBufferSize, PINT32 moduleCount)
{
	*moduleCount = 0;
	PEPROCESS targetProcess;
	KAPC_STATE state;
	int count = 0;

	if (!NT_SUCCESS(PsLookupProcessByProcessId((HANDLE)(ULONG_PTR)targetProcessId, &targetProcess)))
	{
		return STATUS_NOT_FOUND;
	}

	PMODULE_SUMMARY kernelBuffer = (PMODULE_SUMMARY)ExAllocatePool(NonPagedPool, MAX_MODULES * sizeof(MODULE_SUMMARY));

	if (!kernelBuffer)
	{
		ObDereferenceObject(targetProcess);
		return STATUS_INSUFFICIENT_RESOURCES;
	}

	RtlZeroMemory(kernelBuffer, MAX_MODULES * sizeof(MODULE_SUMMARY));

	__try
	{
		KeStackAttachProcess(targetProcess, &state);

		__try
		{
			PPEB64 peb = (PPEB64)PsGetProcessPeb(targetProcess);

			if (peb && SanitizeUserPointer(peb, sizeof(PEB64)) &&
				peb->Ldr && SanitizeUserPointer(peb->Ldr, sizeof(PEB_LDR_DATA)))
			{
				if (!peb->Ldr->Initialized)
				{
					int initLoadCount = 0;
					while (!peb->Ldr->Initialized && initLoadCount++ < 4)
					{
						DriverSleep(250);
					}
				}

				if (peb->Ldr->Initialized)
				{
					PLIST_ENTRY head = &peb->Ldr->InLoadOrderModuleList;
					PLIST_ENTRY current = head->Flink;

					while (current != head && count < MAX_MODULES)
					{
						PLDR_DATA_TABLE_ENTRY entry = CONTAINING_RECORD(current, LDR_DATA_TABLE_ENTRY, InLoadOrderLinks);

						if (!SanitizeUserPointer(entry, sizeof(LDR_DATA_TABLE_ENTRY)))
							break;

						if (entry->DllBase == NULL)
							break;

						kernelBuffer[count].ModuleBase = entry->DllBase;
						kernelBuffer[count].ModuleImageSize = entry->SizeOfImage;
						kernelBuffer[count].ModuleEntryPoint = entry->EntryPoint;

						if (entry->FullDllName.Buffer && entry->FullDllName.Length > 0)
						{
							USHORT copyLen = entry->FullDllName.Length;
							if (copyLen > 255 * sizeof(WCHAR))
								copyLen = 255 * sizeof(WCHAR);
							RtlCopyMemory(kernelBuffer[count].ModuleFileName, entry->FullDllName.Buffer, copyLen);
						}

						count++;
						current = current->Flink;
					}
				}
			}
		}
		__except (GetExceptionCode())
		{
			DbgPrintEx(0, 0, "GetModuleList: PEB walk failed.\n");
		}
	}
	__finally
	{
		KeUnstackDetachProcess(&state);
	}

	int expectedSize = count * sizeof(MODULE_SUMMARY);

	if (!moduleBuffer || bufferSize < expectedSize)
	{
		*requiredBufferSize = expectedSize;
		ExFreePool(kernelBuffer);
		ObDereferenceObject(targetProcess);
		return STATUS_SUCCESS;
	}

	RtlCopyMemory(moduleBuffer, kernelBuffer, expectedSize);
	*moduleCount = count;
	*requiredBufferSize = expectedSize;

	ExFreePool(kernelBuffer);
	ObDereferenceObject(targetProcess);

	return STATUS_SUCCESS;
}