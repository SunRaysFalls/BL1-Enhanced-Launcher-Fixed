using System;
using System.Collections.Generic;

namespace Launcher
{
	// Token: 0x02000022 RID: 34
	internal class ResolutionHelper
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x000026D0 File Offset: 0x000008D0
		public static ResolutionHelper Instance
		{
			get
			{
				return ResolutionHelper.m_instance;
			}
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00005F20 File Offset: 0x00004120
		public ResolutionHelper()
		{
			this.FramerateModes = new List<string>();
			this.MinSmoothedFramerate = 22;
			this.MaxSmoothedFramerate = 62;
			ResolutionHelper.UseUnfilteredResolutions = false;
			ResolutionHelper.m_instance = this;
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x000026D7 File Offset: 0x000008D7
		public List<float> SupportedAspectRatios
		{
			get
			{
				return this.m_supportedAspectRatios;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000B6 RID: 182 RVA: 0x000026DF File Offset: 0x000008DF
		// (set) Token: 0x060000B7 RID: 183 RVA: 0x000026E7 File Offset: 0x000008E7
		public float MaxAspectRatioError
		{
			get
			{
				return this.m_fMaxAspectRatioError;
			}
			set
			{
				this.m_fMaxAspectRatioError = value;
			}
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x000026F0 File Offset: 0x000008F0
		public void AddSupportedAspectRatio(float fSupportedAspectRatio)
		{
			this.m_supportedAspectRatios.Add(fSupportedAspectRatio);
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x000026FE File Offset: 0x000008FE
		public int NumSupportedAspectRatios
		{
			get
			{
				return this.m_supportedAspectRatios.Count;
			}
		}

		// Token: 0x060000BA RID: 186 RVA: 0x0000270B File Offset: 0x0000090B
		public void ClearSupportedAspectRatios()
		{
			this.m_supportedAspectRatios.Clear();
		}

		// Token: 0x060000BB RID: 187 RVA: 0x00005F70 File Offset: 0x00004170
		public bool IsSupportedAspectRatio(int nWidth, int nHeight)
		{
			float num = (float)nWidth / (float)nHeight;
			checked
			{
				for (int i = 0; i < this.m_supportedAspectRatios.Count; i++)
				{
					if (Math.Abs(unchecked(this.m_supportedAspectRatios[i] - num)) <= this.MaxAspectRatioError)
					{
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00002718 File Offset: 0x00000918
		// (set) Token: 0x060000BD RID: 189 RVA: 0x00002720 File Offset: 0x00000920
		public List<string> FramerateModes { get; set; }

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000BE RID: 190 RVA: 0x00002729 File Offset: 0x00000929
		// (set) Token: 0x060000BF RID: 191 RVA: 0x00002731 File Offset: 0x00000931
		public int MinSmoothedFramerate { get; set; }

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000C0 RID: 192 RVA: 0x0000273A File Offset: 0x0000093A
		// (set) Token: 0x060000C1 RID: 193 RVA: 0x00002742 File Offset: 0x00000942
		public int MaxSmoothedFramerate { get; set; }

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000C2 RID: 194 RVA: 0x0000274B File Offset: 0x0000094B
		// (set) Token: 0x060000C3 RID: 195 RVA: 0x00002752 File Offset: 0x00000952
		public static bool UseUnfilteredResolutions { get; set; }

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x060000C4 RID: 196 RVA: 0x0000275A File Offset: 0x0000095A
		public static List<ResolutionHelper.SupportedResolution> SupportedResolutions
		{
			get
			{
				if (ResolutionHelper._supportedResolutions == null)
				{
					ResolutionHelper._supportedResolutions = new List<ResolutionHelper.SupportedResolution>();
					ResolutionHelper.PopulateSupportedResolutions();
				}
				if (!ResolutionHelper.UseUnfilteredResolutions)
				{
					return ResolutionHelper._supportedResolutions;
				}
				return ResolutionHelper._unfilteredSupportedResolutions;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x060000C5 RID: 197 RVA: 0x00002784 File Offset: 0x00000984
		public static int NumSupportedResolutions
		{
			get
			{
				return ResolutionHelper._supportedResolutions.Count;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000C6 RID: 198 RVA: 0x00002790 File Offset: 0x00000990
		public static int NumUnfilteredSupportedResolutions
		{
			get
			{
				return ResolutionHelper._unfilteredSupportedResolutions.Count;
			}
		}

		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x0000279C File Offset: 0x0000099C
		public static List<ResolutionHelper.SupportedResolution> UnfilteredSupportedResolutions
		{
			get
			{
				if (ResolutionHelper._unfilteredSupportedResolutions == null)
				{
					ResolutionHelper._unfilteredSupportedResolutions = new List<ResolutionHelper.SupportedResolution>();
					ResolutionHelper.PopulateSupportedResolutions();
				}
				return ResolutionHelper._unfilteredSupportedResolutions;
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000C8 RID: 200 RVA: 0x00005FB8 File Offset: 0x000041B8
		public static ResolutionHelper.SupportedResolution WindowedModeOnlySupportedResolution
		{
			get
			{
				checked
				{
					if (ResolutionHelper._windowedModeOnlySupportedResolution == null)
					{
						List<ResolutionHelper.SupportedResolution> unfilteredSupportedResolutions = ResolutionHelper.UnfilteredSupportedResolutions;
						if (unfilteredSupportedResolutions.Count > 0)
						{
							int num = unfilteredSupportedResolutions.Count - 1;
							float aspectRatio = unfilteredSupportedResolutions[num].AspectRatio;
							int num2 = 1920;
							int num3 = 1080;
							num2 = Math.Min(num2, unfilteredSupportedResolutions[num].Width);
							num3 = Math.Min(num3, unfilteredSupportedResolutions[num].Height);
							float num4 = ResolutionHelper.Instance.SupportedAspectRatios[0];
							int num5;
							int num6;
							if (num4 > 1f)
							{
								num5 = (int)Math.Round((double)((float)num2 / num4));
								num6 = num2;
							}
							else
							{
								num6 = (int)Math.Round((double)(unchecked(num4 * (float)num3)));
								num5 = num3;
							}
							ResolutionHelper._windowedModeOnlySupportedResolution = new ResolutionHelper.SupportedResolution(num6, num5);
						}
						else
						{
							string text = "Can't enumerate *any* display resolutions!!!";
							MainWindow.LogException(new Exception(text), text);
						}
					}
					return ResolutionHelper._windowedModeOnlySupportedResolution;
				}
			}
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00006094 File Offset: 0x00004294
		public static int FindClosestResolution(int nWidth, int nHeight)
		{
			List<ResolutionHelper.SupportedResolution> supportedResolutions = ResolutionHelper.SupportedResolutions;
			checked
			{
				int num = nWidth * nHeight;
				int count = supportedResolutions.Count;
				int num2 = -1;
				int num3 = int.MaxValue;
				for (int i = 0; i < count; i++)
				{
					int num4 = Math.Abs(supportedResolutions[i].Width * supportedResolutions[i].Height - num);
					if (num4 < num3)
					{
						num3 = num4;
						num2 = i;
					}
				}
				return num2;
			}
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00006100 File Offset: 0x00004300
		private static void PopulateSupportedResolutions()
		{
			NativeMethods.DEVMODE devmode = default(NativeMethods.DEVMODE);
			ResolutionHelper.SupportedResolutions.Clear();
			ResolutionHelper.UnfilteredSupportedResolutions.Clear();
			ResolutionHelper.UseUnfilteredResolutions = false;
			int num = 0;
			checked
			{
				while (NativeMethods.EnumDisplaySettings(null, num, ref devmode))
				{
					ResolutionHelper.SupportedResolution supportedResolution = new ResolutionHelper.SupportedResolution(devmode.dmPelsWidth, devmode.dmPelsHeight);
					string text = supportedResolution.AspectRatio.ToString("F2");
					if (supportedResolution.SupportedAspectRatio)
					{
						if (!ResolutionHelper.SupportedResolutions.Contains(supportedResolution))
						{
							ResolutionHelper.SupportedResolutions.Add(supportedResolution);
							MainWindow.LogMessage(string.Concat(new object[] { "PopulateSupportedResolutions(): Resolution ", num, ": ", supportedResolution.ResolutionNameFixedSize, ". AspectRatio=", text }), false);
						}
					}
					else if (!ResolutionHelper.UnfilteredSupportedResolutions.Contains(supportedResolution))
					{
						MainWindow.LogMessage(string.Concat(new object[] { "PopulateSupportedResolutions(): Resolution ", num, ": ", supportedResolution.ResolutionNameFixedSize, ". AspectRatio=", text, " <=== UNSUPPORTED ASPECT RATIO" }), false);
					}
					if (!ResolutionHelper.UnfilteredSupportedResolutions.Contains(supportedResolution))
					{
						ResolutionHelper.UnfilteredSupportedResolutions.Add(supportedResolution);
					}
					num++;
				}
				if (ResolutionHelper.UnfilteredSupportedResolutions.Count == 0)
				{
					string text2 = "PopulateSupportedResolutions(): No supported display resolutions were detected!";
					MainWindow.LogException(new Exception(text2), text2);
				}
				else
				{
					ResolutionHelper.UnfilteredSupportedResolutions.Sort();
				}
				if (ResolutionHelper.SupportedResolutions.Count > 0)
				{
					ResolutionHelper.SupportedResolutions.Sort();
					return;
				}
				ResolutionHelper.UseUnfilteredResolutions = true;
				MainWindow.LogMessage("PopulateSupportedResolutions(): No supported desktop resolution detected that had the supported aspect ratios", false);
			}
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00006298 File Offset: 0x00004498
		private static void InsertSupportedResolution(List<ResolutionHelper.SupportedResolution> supportedResolutions, ResolutionHelper.SupportedResolution targetResolution)
		{
			int count = supportedResolutions.Count;
			checked
			{
				int num = targetResolution.Width * targetResolution.Height;
				for (int i = 0; i < count; i++)
				{
					if (supportedResolutions[i].Width * supportedResolutions[i].Height >= num)
					{
						supportedResolutions.Insert(i, targetResolution);
						MainWindow.LogMessage("InsertSupportedResolution(): Inserted " + targetResolution.ResolutionName + " before " + supportedResolutions[i + 1].ResolutionName, false);
						return;
					}
				}
				supportedResolutions.Add(targetResolution);
				MainWindow.LogMessage("InsertSupportedResolution(): Inserted " + targetResolution.ResolutionName + " after " + supportedResolutions[count - 1].ResolutionName, false);
			}
		}

		// Token: 0x040000A9 RID: 169
		private static ResolutionHelper m_instance;

		// Token: 0x040000AA RID: 170
		private List<float> m_supportedAspectRatios = new List<float>();

		// Token: 0x040000AB RID: 171
		private float m_fMaxAspectRatioError = 0.08f;

		// Token: 0x040000B0 RID: 176
		private static List<ResolutionHelper.SupportedResolution> _supportedResolutions;

		// Token: 0x040000B1 RID: 177
		private static List<ResolutionHelper.SupportedResolution> _unfilteredSupportedResolutions;

		// Token: 0x040000B2 RID: 178
		private static ResolutionHelper.SupportedResolution _windowedModeOnlySupportedResolution;

		// Token: 0x02000023 RID: 35
		public class SupportedResolution : IEquatable<ResolutionHelper.SupportedResolution>, IComparable<ResolutionHelper.SupportedResolution>
		{
			// Token: 0x060000CC RID: 204 RVA: 0x000027B9 File Offset: 0x000009B9
			public SupportedResolution(int nWidth, int nHeight)
			{
				this.Width = nWidth;
				this.Height = nHeight;
				this.SupportedAspectRatio = ResolutionHelper.Instance.IsSupportedAspectRatio(this.Width, this.Height);
			}

			// Token: 0x1700002D RID: 45
			// (get) Token: 0x060000CD RID: 205 RVA: 0x000027EB File Offset: 0x000009EB
			// (set) Token: 0x060000CE RID: 206 RVA: 0x000027F3 File Offset: 0x000009F3
			public bool SupportedAspectRatio { get; set; }

			// Token: 0x1700002E RID: 46
			// (get) Token: 0x060000CF RID: 207 RVA: 0x000027FC File Offset: 0x000009FC
			public string ResolutionName
			{
				get
				{
					return ResolutionHelper.SupportedResolution.FormatResolutionName(this.Width, this.Height);
				}
			}

			// Token: 0x1700002F RID: 47
			// (get) Token: 0x060000D0 RID: 208 RVA: 0x0000280F File Offset: 0x00000A0F
			public float AspectRatio
			{
				get
				{
					return (float)this.Width / (float)this.Height;
				}
			}

			// Token: 0x17000030 RID: 48
			// (get) Token: 0x060000D1 RID: 209 RVA: 0x00002820 File Offset: 0x00000A20
			public string ResolutionNameFixedSize
			{
				get
				{
					return ResolutionHelper.SupportedResolution.FormatResolutionNameFixedSize(this.Width, this.Height);
				}
			}

			// Token: 0x060000D2 RID: 210 RVA: 0x00002833 File Offset: 0x00000A33
			public static string FormatResolutionName(int nWidth, int nHeight)
			{
				return string.Format("{0}x{1}", nWidth, nHeight);
			}

			// Token: 0x060000D3 RID: 211 RVA: 0x0000284B File Offset: 0x00000A4B
			public static string FormatResolutionNameFixedSize(int nWidth, int nHeight)
			{
				return string.Format("{0,4:F0}x{1,4:F0}", nWidth, nHeight);
			}

			// Token: 0x060000D4 RID: 212 RVA: 0x00006344 File Offset: 0x00004544
			public static bool ParseResolutionName(string sFormattedResolution, out int nOutWidth, out int nOutHeight)
			{
				nOutWidth = -1;
				nOutHeight = -1;
				char[] array = new char[] { 'x' };
				string[] array2 = sFormattedResolution.Split(array);
				return array2.Length == 2 && int.TryParse(array2[0], out nOutWidth) && int.TryParse(array2[1], out nOutHeight);
			}

			// Token: 0x060000D5 RID: 213 RVA: 0x00002863 File Offset: 0x00000A63
			public bool Equals(ResolutionHelper.SupportedResolution other)
			{
				return this.Width == other.Width && this.Height == other.Height;
			}

			// Token: 0x060000D6 RID: 214 RVA: 0x00002883 File Offset: 0x00000A83
			public int GetArea()
			{
				return checked(this.Width * this.Height);
			}

			// Token: 0x060000D7 RID: 215 RVA: 0x0000638C File Offset: 0x0000458C
			public int CompareTo(ResolutionHelper.SupportedResolution other)
			{
				int num = this.Width.CompareTo(other.Width);
				if (num == 0)
				{
					num = this.Height.CompareTo(other.Height);
				}
				return num;
			}

			// Token: 0x040000B3 RID: 179
			public int Width;

			// Token: 0x040000B4 RID: 180
			public int Height;
		}
	}
}
