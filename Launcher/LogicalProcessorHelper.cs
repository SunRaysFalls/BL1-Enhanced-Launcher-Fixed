using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Launcher
{
	// Token: 0x0200000F RID: 15
	internal class LogicalProcessorHelper
	{
		// Token: 0x06000069 RID: 105
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool GetLogicalProcessorInformation(IntPtr Buffer, ref uint ReturnLength);

		// Token: 0x0600006A RID: 106 RVA: 0x00004BD0 File Offset: 0x00002DD0
		public static NativeMethods.SYSTEM_INFO GetSystemInfo()
		{
			NativeMethods.SYSTEM_INFO system_INFO = default(NativeMethods.SYSTEM_INFO);
			NativeMethods.GetSystemInfo(ref system_INFO);
			return system_INFO;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00004BF0 File Offset: 0x00002DF0
		public static LogicalProcessorHelper.CPUInfo GetCpuInfo()
		{
			checked
			{
				try
				{
					LogicalProcessorHelper.CPUInfo cpuinfo = new LogicalProcessorHelper.CPUInfo();
					RegistryKey localMachine = Registry.LocalMachine;
					RegistryKey registryKey = localMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\0");
					string text = registryKey.GetValue("~MHz").ToString();
					cpuinfo.m_uProcessorSpeedMhz = uint.Parse(text);
					cpuinfo.m_processorIdentifier = new LogicalProcessorHelper.CPUInfo.ProcessorIdentifier(registryKey.GetValue("Identifier").ToString());
					cpuinfo.m_sProcessorName = registryKey.GetValue("ProcessornameString").ToString();
					cpuinfo.m_sVendorId = registryKey.GetValue("VendorIdentifier").ToString();
					cpuinfo.m_uNumCores = 0U;
					while (registryKey != null)
					{
						cpuinfo.m_uNumCores += 1U;
						registryKey = localMachine.OpenSubKey("HARDWARE\\DESCRIPTION\\System\\CentralProcessor\\" + cpuinfo.m_uNumCores);
					}
					return cpuinfo;
				}
				catch (Exception ex)
				{
					MainWindow.LogException(ex, ex.Message + "\n" + ex.StackTrace);
				}
				return null;
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00004CEC File Offset: 0x00002EEC
		public static LogicalProcessorHelper.SYSTEM_LOGICAL_PROCESSOR_INFORMATION[] GetLogicalProcessorInformation()
		{
			uint num = 0U;
			LogicalProcessorHelper.GetLogicalProcessorInformation(IntPtr.Zero, ref num);
			checked
			{
				if (Marshal.GetLastWin32Error() == 122)
				{
					IntPtr intPtr = Marshal.AllocHGlobal((int)num);
					try
					{
						if (LogicalProcessorHelper.GetLogicalProcessorInformation(intPtr, ref num))
						{
							int num2 = Marshal.SizeOf(typeof(LogicalProcessorHelper.SYSTEM_LOGICAL_PROCESSOR_INFORMATION));
							int num3 = (int)num / num2;
							LogicalProcessorHelper.SYSTEM_LOGICAL_PROCESSOR_INFORMATION[] array = new LogicalProcessorHelper.SYSTEM_LOGICAL_PROCESSOR_INFORMATION[num3];
							IntPtr intPtr2 = intPtr;
							for (int i = 0; i < num3; i++)
							{
								array[i].Init();
								array[i] = (LogicalProcessorHelper.SYSTEM_LOGICAL_PROCESSOR_INFORMATION)Marshal.PtrToStructure(intPtr2, typeof(LogicalProcessorHelper.SYSTEM_LOGICAL_PROCESSOR_INFORMATION));
								MainWindow.LogMessage(string.Concat(new object[]
								{
									"ProcessorInfo[",
									i,
									"]: ",
									array[i].GetProcessorInformationDescription()
								}), false);
								intPtr2 += num2;
							}
							return array;
						}
					}
					finally
					{
						Marshal.FreeHGlobal(intPtr);
					}
				}
				return null;
			}
		}

		// Token: 0x0400002E RID: 46
		private const int ERROR_INSUFFICIENT_BUFFER = 122;

		// Token: 0x02000010 RID: 16
		public struct PROCESSORCORE
		{
			// Token: 0x0400002F RID: 47
			public byte Flags;
		}

		// Token: 0x02000011 RID: 17
		public struct NUMANODE
		{
			// Token: 0x04000030 RID: 48
			public uint NodeNumber;
		}

		// Token: 0x02000012 RID: 18
		public enum PROCESSOR_CACHE_TYPE
		{
			// Token: 0x04000032 RID: 50
			CacheUnified,
			// Token: 0x04000033 RID: 51
			CacheInstruction,
			// Token: 0x04000034 RID: 52
			CacheData,
			// Token: 0x04000035 RID: 53
			CacheTrace
		}

		// Token: 0x02000013 RID: 19
		public struct CACHE_DESCRIPTOR
		{
			// Token: 0x04000036 RID: 54
			public byte Level;

			// Token: 0x04000037 RID: 55
			public byte Associativity;

			// Token: 0x04000038 RID: 56
			public ushort LineSize;

			// Token: 0x04000039 RID: 57
			public uint Size;

			// Token: 0x0400003A RID: 58
			public LogicalProcessorHelper.PROCESSOR_CACHE_TYPE Type;
		}

		// Token: 0x02000014 RID: 20
		[StructLayout(LayoutKind.Explicit)]
		public struct SYSTEM_LOGICAL_PROCESSOR_INFORMATION_UNION
		{
			// Token: 0x0400003B RID: 59
			[FieldOffset(0)]
			public LogicalProcessorHelper.PROCESSORCORE ProcessorCore;

			// Token: 0x0400003C RID: 60
			[FieldOffset(0)]
			public LogicalProcessorHelper.NUMANODE NumaNode;

			// Token: 0x0400003D RID: 61
			[FieldOffset(0)]
			public LogicalProcessorHelper.CACHE_DESCRIPTOR Cache;

			// Token: 0x0400003E RID: 62
			[FieldOffset(0)]
			private ulong Reserved1;

			// Token: 0x0400003F RID: 63
			[FieldOffset(8)]
			private ulong Reserved2;
		}

		// Token: 0x02000015 RID: 21
		public enum LOGICAL_PROCESSOR_RELATIONSHIP
		{
			// Token: 0x04000041 RID: 65
			RelationProcessorCore,
			// Token: 0x04000042 RID: 66
			RelationNumaNode,
			// Token: 0x04000043 RID: 67
			RelationCache,
			// Token: 0x04000044 RID: 68
			RelationProcessorPackage,
			// Token: 0x04000045 RID: 69
			RelationGroup,
			// Token: 0x04000046 RID: 70
			RelationAll = 65535
		}

		// Token: 0x02000016 RID: 22
		public struct SYSTEM_LOGICAL_PROCESSOR_INFORMATION
		{
			// Token: 0x0600006E RID: 110 RVA: 0x00004DF0 File Offset: 0x00002FF0
			public string GetProcessorInformationDescription()
			{
				string text = "";
				LogicalProcessorHelper.LOGICAL_PROCESSOR_RELATIONSHIP relationship = this.Relationship;
				switch (relationship)
				{
				case LogicalProcessorHelper.LOGICAL_PROCESSOR_RELATIONSHIP.RelationProcessorCore:
					text = this.ProcessorInformation.ProcessorCore.Flags.ToString("X");
					goto IL_0118;
				case LogicalProcessorHelper.LOGICAL_PROCESSOR_RELATIONSHIP.RelationNumaNode:
					text = this.ProcessorInformation.NumaNode.NodeNumber.ToString("X");
					goto IL_0118;
				case LogicalProcessorHelper.LOGICAL_PROCESSOR_RELATIONSHIP.RelationCache:
					text = string.Format("L{0} Cache, Associative={1}, Cache Line Size={2} bytes, Cache Size={3}, CacheType={4}", new object[]
					{
						this.ProcessorInformation.Cache.Level,
						this.ProcessorInformation.Cache.Associativity,
						this.ProcessorInformation.Cache.LineSize,
						this.ProcessorInformation.Cache.Size,
						this.ProcessorInformation.Cache.Type
					});
					goto IL_0118;
				case LogicalProcessorHelper.LOGICAL_PROCESSOR_RELATIONSHIP.RelationProcessorPackage:
				case LogicalProcessorHelper.LOGICAL_PROCESSOR_RELATIONSHIP.RelationGroup:
					break;
				default:
					if (relationship != LogicalProcessorHelper.LOGICAL_PROCESSOR_RELATIONSHIP.RelationAll)
					{
						goto IL_0118;
					}
					break;
				}
				text = "(Not Handled): Relationship=" + this.Relationship;
				IL_0118:
				return string.Format("ProcessorMask={0}, ProcessorInformation={1}", this.ProcessorMask.ToUInt64(), text);
			}

			// Token: 0x0600006F RID: 111 RVA: 0x000023E1 File Offset: 0x000005E1
			public void Init()
			{
				this.ProcessorMask = UIntPtr.Zero;
				this.Relationship = LogicalProcessorHelper.LOGICAL_PROCESSOR_RELATIONSHIP.RelationAll;
				this.ProcessorInformation.ProcessorCore.Flags = 0;
			}

			// Token: 0x04000047 RID: 71
			public UIntPtr ProcessorMask;

			// Token: 0x04000048 RID: 72
			public LogicalProcessorHelper.LOGICAL_PROCESSOR_RELATIONSHIP Relationship;

			// Token: 0x04000049 RID: 73
			public LogicalProcessorHelper.SYSTEM_LOGICAL_PROCESSOR_INFORMATION_UNION ProcessorInformation;
		}

		// Token: 0x02000017 RID: 23
		public class CPUInfo
		{
			// Token: 0x06000070 RID: 112 RVA: 0x0000240A File Offset: 0x0000060A
			public CPUInfo()
			{
				this.m_uProcessorSpeedMhz = 0U;
				this.m_processorIdentifier = null;
				this.m_sProcessorName = null;
				this.m_sVendorId = null;
				this.m_uNumCores = 0U;
			}

			// Token: 0x06000071 RID: 113 RVA: 0x00004F30 File Offset: 0x00003130
			public string GetDescription()
			{
				return string.Format("numCores={0}, speedMhz={1}, id={2}, name={3}, vendorId={4}, processorId=({5})", new object[]
				{
					this.m_uNumCores,
					this.m_uProcessorSpeedMhz,
					this.m_processorIdentifier,
					this.m_sProcessorName,
					this.m_sVendorId,
					this.m_processorIdentifier.GetDescription()
				});
			}

			// Token: 0x0400004A RID: 74
			public uint m_uProcessorSpeedMhz;

			// Token: 0x0400004B RID: 75
			public LogicalProcessorHelper.CPUInfo.ProcessorIdentifier m_processorIdentifier;

			// Token: 0x0400004C RID: 76
			public string m_sProcessorName;

			// Token: 0x0400004D RID: 77
			public string m_sVendorId;

			// Token: 0x0400004E RID: 78
			public uint m_uNumCores;

			// Token: 0x02000018 RID: 24
			public class ProcessorIdentifier
			{
				// Token: 0x06000072 RID: 114 RVA: 0x00004F94 File Offset: 0x00003194
				public ProcessorIdentifier(string sRawProcessorIdentifier)
				{
					this.m_sRawId = sRawProcessorIdentifier;
					this.m_nFamily = -1;
					this.m_nModel = -1;
					this.m_nStepping = -1;
					char[] array = new char[] { ' ' };
					string[] array2 = this.m_sRawId.Split(array);
					this.IsValid = false;
					if (array2.Length == 7 && array2[1] == "Family" && array2[3] == "Model" && array2[5] == "Stepping")
					{
						this.m_sArchitecture = array2[0];
						if (int.TryParse(array2[2], out this.m_nFamily) && int.TryParse(array2[4], out this.m_nModel) && int.TryParse(array2[6], out this.m_nStepping))
						{
							this.IsValid = true;
						}
					}
				}

				// Token: 0x06000073 RID: 115 RVA: 0x00005058 File Offset: 0x00003258
				public string GetDescription()
				{
					if (this.IsValid)
					{
						return string.Format("ProcessorIdentifier: architecture={0}, family={1}, model={2}, stepping={3} <-- \"{4}\"", new object[] { this.m_sArchitecture, this.m_nFamily, this.m_nModel, this.m_nStepping, this.m_sRawId });
					}
					return "ProcessorIdentifier: Unable to parse rawId=\"" + this.m_sRawId + "\"";
				}

				// Token: 0x1700000F RID: 15
				// (get) Token: 0x06000074 RID: 116 RVA: 0x00002435 File Offset: 0x00000635
				// (set) Token: 0x06000075 RID: 117 RVA: 0x0000243D File Offset: 0x0000063D
				public bool IsValid { get; set; }

				// Token: 0x0400004F RID: 79
				public string m_sRawId;

				// Token: 0x04000050 RID: 80
				public string m_sArchitecture;

				// Token: 0x04000051 RID: 81
				public int m_nFamily;

				// Token: 0x04000052 RID: 82
				public int m_nModel;

				// Token: 0x04000053 RID: 83
				public int m_nStepping;
			}
		}
	}
}
