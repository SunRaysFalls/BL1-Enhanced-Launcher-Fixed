using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;

namespace Launcher
{
	// Token: 0x02000019 RID: 25
	public class NativeMethods
	{
		// Token: 0x06000076 RID: 118
		[DllImport("user32.dll")]
		internal static extern bool EnumDisplaySettings([MarshalAs(UnmanagedType.LPWStr)] string deviceName, int modeNum, ref NativeMethods.DEVMODE devMode);

		// Token: 0x06000077 RID: 119
		[DllImport("kernel32.dll")]
		internal static extern void GetSystemInfo(ref NativeMethods.SYSTEM_INFO systemInfo);

		// Token: 0x06000078 RID: 120 RVA: 0x000050D0 File Offset: 0x000032D0
		public static string DeviceInformation(string sWin32ClassName, out Dictionary<string, List<string>> outDeviceProperties, bool bCreateLogString = true)
		{
			outDeviceProperties = new Dictionary<string, List<string>>();
			ManagementClass managementClass = new ManagementClass(sWin32ClassName);
			ManagementObjectCollection instances = managementClass.GetInstances();
			PropertyDataCollection properties = managementClass.Properties;
			string text = "";
			int num = 0;
			checked
			{
				foreach (ManagementBaseObject managementBaseObject in instances)
				{
					ManagementObject managementObject = (ManagementObject)managementBaseObject;
					foreach (PropertyData propertyData in properties)
					{
						object value = managementObject.Properties[propertyData.Name].Value;
						if (value != null)
						{
							if (bCreateLogString)
							{
								text = string.Concat(new string[]
								{
									text,
									"\n    ",
									num.ToString("D4"),
									"-> ",
									propertyData.Name,
									":  ",
									value.ToString()
								});
								num++;
							}
							if (!outDeviceProperties.ContainsKey(propertyData.Name))
							{
								outDeviceProperties[propertyData.Name] = new List<string>();
							}
							outDeviceProperties[propertyData.Name].Add(value.ToString());
						}
					}
					if (bCreateLogString)
					{
						text += "\n";
					}
				}
				if (bCreateLogString)
				{
					MainWindow.LogMessage(string.Concat(new string[] { "\n", sWin32ClassName, ": ", text, "\n" }), false);
				}
				return text;
			}
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00002446 File Offset: 0x00000646
		public static int RoundUpToNearestGB(long lNumBytes)
		{
			return checked((int)(lNumBytes + 536870911L >> 30));
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00005298 File Offset: 0x00003498
		public static int GetVideoMemoryGBFromAdapterRam()
		{
			Dictionary<string, List<string>> dictionary;
			NativeMethods.DeviceInformation("Win32_VideoController", out dictionary, false);
			long num = 0L;
			checked
			{
				if (dictionary.ContainsKey("AdapterRAM"))
				{
					List<string> list = dictionary["AdapterRAM"];
					int count = list.Count;
					for (int i = 0; i < count; i++)
					{
						long num2 = long.Parse(list[i]);
						if (num2 > num)
						{
							num = num2;
						}
					}
				}
				int num3 = NativeMethods.RoundUpToNearestGB(num);
				MainWindow.LogMessage("GetVideoMemoryGB(): " + num3 + "GB", false);
				return num3;
			}
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00005324 File Offset: 0x00003524
		public static int GetVideoMemoryGBFromDescription()
		{
			Dictionary<string, List<string>> dictionary;
			NativeMethods.DeviceInformation("Win32_VideoController", out dictionary, false);
			int num = 0;
			checked
			{
				if (dictionary.ContainsKey("Description"))
				{
					List<string> list = dictionary["Description"];
					int count = list.Count;
					for (int i = 0; i < count; i++)
					{
						string text = list[i];
						int num2 = text.IndexOf("GB");
						if (num2 >= 0)
						{
							int num3 = text.LastIndexOf(' ');
							int num4 = num2 - num3 - 1;
							if (num4 > 0)
							{
								int num5 = int.Parse(text.Substring(num3 + 1, num4));
								if (num5 > num)
								{
									num = num5;
								}
							}
						}
					}
				}
				MainWindow.LogMessage("GetVideoMemoryGBFromDescription(): " + num + "GB", false);
				return num;
			}
		}

		// Token: 0x0600007C RID: 124 RVA: 0x000053E0 File Offset: 0x000035E0
		private static bool ParsePNPDeviceID(string sPNPDeviceID, out string sOutVendorId, out string sOutDeviceId)
		{
			sPNPDeviceID = sPNPDeviceID.Replace(" ", string.Empty);
			sOutVendorId = "";
			sOutDeviceId = "";
			char[] array = new char[] { '\\' };
			string[] array2 = sPNPDeviceID.Split(array);
			if (array2.Length == 3)
			{
				char[] array3 = new char[] { '&' };
				string[] array4 = array2[1].Split(array3);
				if (array4.Length == 4)
				{
					sOutVendorId = "0x" + array4[0].Substring(4);
					sOutDeviceId = "0x" + array4[1].Substring(4);
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00005470 File Offset: 0x00003670
		public static void GetDisplayAdapterDescription(out string sOutVendorName, out string sOutVendorId, out string sOutModelName, out string sOutDeviceId, out int nOutMemoryGB, out string sOutRawDescription)
		{
			sOutVendorName = null;
			sOutVendorId = null;
			sOutModelName = null;
			sOutDeviceId = null;
			nOutMemoryGB = 0;
			bool flag = true;
			sOutRawDescription = null;
			Dictionary<string, List<string>> dictionary;
			NativeMethods.DeviceInformation("Win32_VideoController", out dictionary, false);
			if (dictionary.ContainsKey("PNPDeviceID"))
			{
				List<string> list = dictionary["PNPDeviceID"];
				if (list.Count > 0)
				{
					MainWindow.LogMessage("PNPDeviceID=" + list[0], false);
					if (!NativeMethods.ParsePNPDeviceID(list[0], out sOutVendorId, out sOutDeviceId))
					{
						MainWindow.LogException(new Exception(), "Unable to parse PNPDeviceID in Win32_VideoController");
					}
				}
			}
			checked
			{
				if (dictionary.ContainsKey("Description"))
				{
					List<string> list2 = dictionary["Description"];
					int count = list2.Count;
					for (int i = 0; i < count; i++)
					{
						string text = list2[i];
						sOutRawDescription = text;
						int num = text.IndexOf(' ');
						sOutVendorName = text.Substring(0, num);
						text = text.Substring(num + 1);
						if (text.EndsWith("GB"))
						{
							int num2 = text.LastIndexOf(' ');
							sOutModelName = text.Substring(0, num2);
							text = text.Substring(num2 + 1);
							int num3 = text.IndexOf("GB");
							if (num3 >= 0)
							{
								int num4 = int.Parse(text.Substring(0, num3));
								if (num4 > nOutMemoryGB)
								{
									nOutMemoryGB = num4;
								}
							}
						}
					}
				}
				int videoMemoryGBFromAdapterRam = NativeMethods.GetVideoMemoryGBFromAdapterRam();
				if (videoMemoryGBFromAdapterRam > nOutMemoryGB)
				{
					nOutMemoryGB = videoMemoryGBFromAdapterRam;
					flag = false;
				}
				string text2 = (flag ? "(from VideoCardDescription)" : "(from Win32_VideoController)");
				MainWindow.LogMessage(string.Concat(new object[]
				{
					"GetDisplayAdapterDescription(): vendor=\"", sOutVendorName, "\", vendorId=\"", sOutVendorId, "\", model=\"", sOutModelName, "\", deviceId=\"", sOutDeviceId, "\", memoryGB=\"", nOutMemoryGB,
					"\" ", text2
				}), false);
			}
		}

		// Token: 0x0600007E RID: 126 RVA: 0x0000565C File Offset: 0x0000385C
		public static int GetSystemMemoryGB()
		{
			Dictionary<string, List<string>> dictionary;
			NativeMethods.DeviceInformation("Win32_PhysicalMemory", out dictionary, false);
			long num = 0L;
			checked
			{
				if (dictionary.ContainsKey("Capacity"))
				{
					List<string> list = dictionary["Capacity"];
					int count = list.Count;
					for (int i = 0; i < count; i++)
					{
						num += long.Parse(list[i]);
					}
				}
				int num2 = NativeMethods.RoundUpToNearestGB(num);
				MainWindow.LogMessage("GetSysteMemoryGB(): " + num2 + "GB", false);
				return num2;
			}
		}

		// Token: 0x0600007F RID: 127 RVA: 0x000056E0 File Offset: 0x000038E0
		public static int GetInstalledDriveFreeSpaceGB(out string sOutDriveLetter)
		{
			sOutDriveLetter = Path.GetPathRoot(Environment.CurrentDirectory);
			long num = (long)NativeMethods.RoundUpToNearestGB(NativeMethods.GetTotalFreeSpaceOnDriveBytes(sOutDriveLetter));
			MainWindow.LogMessage(string.Concat(new object[]
			{
				"Game is installed on drive ",
				sOutDriveLetter.Substring(0, 1),
				": ",
				num,
				"GB available"
			}), false);
			return checked((int)num);
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00005748 File Offset: 0x00003948
		public static long GetTotalFreeSpaceOnDriveBytes(string sDriveRootPath)
		{
			foreach (DriveInfo driveInfo in DriveInfo.GetDrives())
			{
				if (driveInfo.IsReady && driveInfo.Name == sDriveRootPath)
				{
					return driveInfo.TotalFreeSpace;
				}
			}
			return -1L;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x0000578C File Offset: 0x0000398C
		public static string GetOperatingSystemVersion(out int nOutMajorVersion, out int nOutMinorVersion, out int nOutBuildNumber, out bool bOutIs64BitOS)
		{
			nOutMajorVersion = 0;
			nOutMinorVersion = 0;
			nOutBuildNumber = 0;
			bOutIs64BitOS = false;
			Dictionary<string, List<string>> dictionary;
			NativeMethods.DeviceInformation("Win32_OperatingSystem", out dictionary, false);
			if (dictionary.ContainsKey("Version"))
			{
				List<string> list = dictionary["Version"];
				if (list.Count > 0)
				{
					string text = list[0];
					char[] array = new char[] { '.' };
					string[] array2 = text.Split(array);
					nOutMajorVersion = ((array2.Length >= 1) ? int.Parse(array2[0]) : 0);
					nOutMinorVersion = ((array2.Length >= 2) ? int.Parse(array2[1]) : 0);
					nOutBuildNumber = ((array2.Length >= 3) ? int.Parse(array2[2]) : 0);
					if (dictionary.ContainsKey("OSArchitecture"))
					{
						List<string> list2 = dictionary["OSArchitecture"];
						if (list2.Count > 0)
						{
							string text2 = list2[0];
							bOutIs64BitOS = text2.Equals("64-bit");
							MainWindow.LogMessage(string.Concat(new object[]
							{
								"GetOperatingSystemVersion(): Version=",
								text,
								", architecture=",
								text2,
								", majorVersion=",
								nOutMajorVersion,
								", minorVersion=",
								nOutMinorVersion,
								", build=",
								nOutBuildNumber,
								", is64BitOS=",
								bOutIs64BitOS.ToString()
							}), false);
							return text;
						}
					}
					else
					{
						MainWindow.LogException(new Exception(), "No OSArchitecture found. fullVersion=" + text);
					}
				}
				else
				{
					MainWindow.LogException(new Exception(), "Win32_OperatingSystem has an empty 'Version' property.");
				}
			}
			else
			{
				MainWindow.LogException(new Exception(), "Win32_OperatingSystem has no 'Version' property.");
			}
			return "";
		}

		// Token: 0x06000082 RID: 130
		[DllImport("steam_api.dll")]
		internal static extern bool SteamAPI_Init();

		// Token: 0x06000083 RID: 131
		[DllImport("steam_api.dll")]
		internal static extern void SteamAPI_Shutdown();

		// Token: 0x06000084 RID: 132
		[DllImport("steam_api.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamApps")]
		internal static extern IntPtr ISteamClient_SteamApps(IntPtr instancePtr, int hSteamUser, int hSteamPipe, IntPtr pchVersion);

		// Token: 0x06000085 RID: 133
		[DllImport("steam_api.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamClient();

		// Token: 0x06000086 RID: 134
		[DllImport("steam_api.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_GetCurrentGameLanguage")]
		internal static extern IntPtr ISteamApps_GetCurrentGameLanguage(IntPtr instancePtr);

		// Token: 0x06000087 RID: 135
		[DllImport("steam_api.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int SteamAPI_GetHSteamUser();

		// Token: 0x06000088 RID: 136
		[DllImport("steam_api.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern int SteamAPI_GetHSteamPipe();

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x06000089 RID: 137 RVA: 0x00002454 File Offset: 0x00000654
		public static string SteamAppsCurrentGameLanguage
		{
			get
			{
				if (string.IsNullOrEmpty(NativeMethods.m_sSteamAppsCurrentGameLanguage))
				{
					NativeMethods.m_sSteamAppsCurrentGameLanguage = NativeMethods.GetSteamAppsCurrentGameLanguage().ToLower();
				}
				return NativeMethods.m_sSteamAppsCurrentGameLanguage;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600008A RID: 138 RVA: 0x00002476 File Offset: 0x00000676
		public static bool IsLoggedIntoSteam
		{
			get
			{
				if (string.IsNullOrEmpty(NativeMethods.m_sSteamAppsCurrentGameLanguage))
				{
					NativeMethods.GetSteamAppsCurrentGameLanguage();
				}
				return NativeMethods.m_bIsLoggedIntoSteam;
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x0000592C File Offset: 0x00003B2C
		private static string GetSteamAppsCurrentGameLanguage()
		{
			string text = "english";
			NativeMethods.m_bIsLoggedIntoSteam = false;
			if (NativeMethods.SteamAPI_Init())
			{
				int num = NativeMethods.SteamAPI_GetHSteamUser();
				int num2 = NativeMethods.SteamAPI_GetHSteamPipe();
				IntPtr intPtr = NativeMethods.SteamClient();
				if (intPtr != IntPtr.Zero)
				{
					IntPtr intPtr2 = Marshal.StringToCoTaskMemAnsi("STEAMAPPS_INTERFACE_VERSION008");
					IntPtr intPtr3 = NativeMethods.ISteamClient_SteamApps(intPtr, num, num2, intPtr2);
					Marshal.Release(intPtr2);
					intPtr2 = IntPtr.Zero;
					if (intPtr3 != IntPtr.Zero)
					{
						IntPtr intPtr4 = NativeMethods.ISteamApps_GetCurrentGameLanguage(intPtr3);
						if (intPtr4 != IntPtr.Zero)
						{
							text = Marshal.PtrToStringAnsi(intPtr4);
							Marshal.Release(intPtr4);
							intPtr4 = IntPtr.Zero;
							MainWindow.LogMessage("GetSteamAppsCurrentGameLanguage(): sGameLanguage=" + text + " (Logged in to Steam)", true);
							NativeMethods.m_bIsLoggedIntoSteam = true;
						}
						Marshal.Release(intPtr3);
						intPtr3 = IntPtr.Zero;
					}
					Marshal.Release(intPtr);
					intPtr = IntPtr.Zero;
				}
				NativeMethods.SteamAPI_Shutdown();
			}
			else
			{
				MainWindow.LogMessage("WARNING: Can't initialize Steam; using default game language --> GetSteamAppsCurrentGameLanguage(): DefaultSteamAppsGameLanguage=english --> (Not logged in to Steam!)", true);
			}
			return text;
		}

		// Token: 0x04000055 RID: 85
		internal const string SteamworksApiDllName = "steam_api.dll";

		// Token: 0x04000056 RID: 86
		private const string DefaultSteamAppsGameLanguage = "english";

		// Token: 0x04000057 RID: 87
		private static string m_sSteamAppsCurrentGameLanguage;

		// Token: 0x04000058 RID: 88
		private static bool m_bIsLoggedIntoSteam;

		// Token: 0x0200001A RID: 26
		public enum ScreenOrientation
		{
			// Token: 0x0400005A RID: 90
			Angle0,
			// Token: 0x0400005B RID: 91
			Angle90,
			// Token: 0x0400005C RID: 92
			Angle180,
			// Token: 0x0400005D RID: 93
			Angle270
		}

		// Token: 0x0200001B RID: 27
		public struct DEVMODE
		{
			// Token: 0x0400005E RID: 94
			private const int CCHDEVICENAME = 32;

			// Token: 0x0400005F RID: 95
			private const int CCHFORMNAME = 32;

			// Token: 0x04000060 RID: 96
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string dmDeviceName;

			// Token: 0x04000061 RID: 97
			public short dmSpecVersion;

			// Token: 0x04000062 RID: 98
			public short dmDriverVersion;

			// Token: 0x04000063 RID: 99
			public short dmSize;

			// Token: 0x04000064 RID: 100
			public short dmDriverExtra;

			// Token: 0x04000065 RID: 101
			public int dmFields;

			// Token: 0x04000066 RID: 102
			public int dmPositionX;

			// Token: 0x04000067 RID: 103
			public int dmPositionY;

			// Token: 0x04000068 RID: 104
			public NativeMethods.ScreenOrientation dmDisplayOrientation;

			// Token: 0x04000069 RID: 105
			public int dmDisplayFixedOutput;

			// Token: 0x0400006A RID: 106
			public short dmColor;

			// Token: 0x0400006B RID: 107
			public short dmDuplex;

			// Token: 0x0400006C RID: 108
			public short dmYResolution;

			// Token: 0x0400006D RID: 109
			public short dmTTOption;

			// Token: 0x0400006E RID: 110
			public short dmCollate;

			// Token: 0x0400006F RID: 111
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string dmFormName;

			// Token: 0x04000070 RID: 112
			public short dmLogPixels;

			// Token: 0x04000071 RID: 113
			public int dmBitsPerPel;

			// Token: 0x04000072 RID: 114
			public int dmPelsWidth;

			// Token: 0x04000073 RID: 115
			public int dmPelsHeight;

			// Token: 0x04000074 RID: 116
			public int dmDisplayFlags;

			// Token: 0x04000075 RID: 117
			public int dmDisplayFrequency;

			// Token: 0x04000076 RID: 118
			public int dmICMMethod;

			// Token: 0x04000077 RID: 119
			public int dmICMIntent;

			// Token: 0x04000078 RID: 120
			public int dmMediaType;

			// Token: 0x04000079 RID: 121
			public int dmDitherType;

			// Token: 0x0400007A RID: 122
			public int dmReserved1;

			// Token: 0x0400007B RID: 123
			public int dmReserved2;

			// Token: 0x0400007C RID: 124
			public int dmPanningWidth;

			// Token: 0x0400007D RID: 125
			public int dmPanningHeight;
		}

		// Token: 0x0200001C RID: 28
		public enum ProcessorArchitectureTypes
		{
			// Token: 0x0400007F RID: 127
			eProcessorArchitectureAMD64 = 9,
			// Token: 0x04000080 RID: 128
			eProcessorArchitectureARM = 5,
			// Token: 0x04000081 RID: 129
			eProcessorArchitectureARM64 = 12,
			// Token: 0x04000082 RID: 130
			eProcessorArchitectureIA64 = 6,
			// Token: 0x04000083 RID: 131
			eProcessorArchitectureINTEL = 0,
			// Token: 0x04000084 RID: 132
			eProcessorArchitectureUNKNOWN = 65535
		}

		// Token: 0x0200001D RID: 29
		[StructLayout(LayoutKind.Explicit)]
		public struct ProcessorArchitecture
		{
			// Token: 0x17000012 RID: 18
			// (get) Token: 0x0600008E RID: 142 RVA: 0x00002491 File Offset: 0x00000691
			public NativeMethods.ProcessorArchitectureTypes ProcessorArchitectureType
			{
				get
				{
					return (NativeMethods.ProcessorArchitectureTypes)this.wProcessorArchitecture;
				}
			}

			// Token: 0x04000085 RID: 133
			[FieldOffset(0)]
			[MarshalAs(UnmanagedType.U2)]
			private ushort wProcessorArchitecture;

			// Token: 0x04000086 RID: 134
			[FieldOffset(2)]
			[MarshalAs(UnmanagedType.U2)]
			private ushort wReserved;
		}

		// Token: 0x0200001E RID: 30
		[StructLayout(LayoutKind.Explicit)]
		public struct ArchitectureInfo
		{
			// Token: 0x04000087 RID: 135
			[FieldOffset(0)]
			[MarshalAs(UnmanagedType.U4)]
			private uint dwOemId;

			// Token: 0x04000088 RID: 136
			[FieldOffset(0)]
			private NativeMethods.ProcessorArchitecture processorArchitecture;
		}

		// Token: 0x0200001F RID: 31
		public enum ProcessorTypeObsolete
		{
			// Token: 0x0400008A RID: 138
			eProcessorIntel386 = 386,
			// Token: 0x0400008B RID: 139
			eProcessorIntel486 = 486,
			// Token: 0x0400008C RID: 140
			eProcessorIntelPentium = 586,
			// Token: 0x0400008D RID: 141
			eProcessorIntelIA64 = 2200,
			// Token: 0x0400008E RID: 142
			eProcessorAMDX8664 = 8664
		}

		// Token: 0x02000020 RID: 32
		public struct SYSTEM_INFO
		{
			// Token: 0x0400008F RID: 143
			private NativeMethods.ArchitectureInfo architectureInfo;

			// Token: 0x04000090 RID: 144
			[MarshalAs(UnmanagedType.U4)]
			private uint dwPageSize;

			// Token: 0x04000091 RID: 145
			[MarshalAs(UnmanagedType.SysUInt)]
			private UIntPtr lpMinimumApplicationAddress;

			// Token: 0x04000092 RID: 146
			[MarshalAs(UnmanagedType.SysUInt)]
			private UIntPtr lpMaximumApplicationAddress;

			// Token: 0x04000093 RID: 147
			[MarshalAs(UnmanagedType.SysUInt)]
			private UIntPtr dwActiveProcessorMask;

			// Token: 0x04000094 RID: 148
			[MarshalAs(UnmanagedType.U4)]
			private uint dwNumberOfProcessors;

			// Token: 0x04000095 RID: 149
			[MarshalAs(UnmanagedType.U4)]
			private NativeMethods.ProcessorTypeObsolete dwProcessorType;

			// Token: 0x04000096 RID: 150
			[MarshalAs(UnmanagedType.U4)]
			private uint dwAllocationGranularity;

			// Token: 0x04000097 RID: 151
			[MarshalAs(UnmanagedType.U2)]
			private ushort wProcessorLevel;

			// Token: 0x04000098 RID: 152
			[MarshalAs(UnmanagedType.U2)]
			private ushort wProcessorRevision;
		}
	}
}
