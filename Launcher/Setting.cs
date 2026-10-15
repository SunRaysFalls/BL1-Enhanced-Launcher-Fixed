using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using ProtoBuf;

namespace Launcher
{
	// Token: 0x02000025 RID: 37
	[ProtoContract]
	public class Setting : INotifyPropertyChanged
	{
		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000D8 RID: 216 RVA: 0x00002892 File Offset: 0x00000A92
		// (set) Token: 0x060000D9 RID: 217 RVA: 0x0000289A File Offset: 0x00000A9A
		[ProtoMember(1)]
		public string ConfigFileSuffix { get; set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000DA RID: 218 RVA: 0x000028A3 File Offset: 0x00000AA3
		// (set) Token: 0x060000DB RID: 219 RVA: 0x000028AB File Offset: 0x00000AAB
		[ProtoMember(2)]
		public string ConfigSection { get; set; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000DC RID: 220 RVA: 0x000028B4 File Offset: 0x00000AB4
		// (set) Token: 0x060000DD RID: 221 RVA: 0x000028BC File Offset: 0x00000ABC
		[ProtoMember(3)]
		public string ConfigKey { get; set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000DE RID: 222 RVA: 0x000028C5 File Offset: 0x00000AC5
		// (set) Token: 0x060000DF RID: 223 RVA: 0x000028CD File Offset: 0x00000ACD
		public int SelectedValue
		{
			get
			{
				return this._selectedValue;
			}
			set
			{
				if (this._selectedValue != value)
				{
					this._selectedValue = value;
					this.NotifyPropertyChanged("SelectedValue");
					this.NotifyPropertyChanged("DisplayValue");
				}
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000E0 RID: 224 RVA: 0x000028F5 File Offset: 0x00000AF5
		public string ConfigValue
		{
			get
			{
				return this.PossibleValues[this.SelectedValue].Key;
			}
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000E1 RID: 225 RVA: 0x0000290D File Offset: 0x00000B0D
		public string DisplayValue
		{
			get
			{
				return this.PossibleValues[this.SelectedValue].Value;
			}
		}

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x060000E2 RID: 226 RVA: 0x00002925 File Offset: 0x00000B25
		// (set) Token: 0x060000E3 RID: 227 RVA: 0x0000292D File Offset: 0x00000B2D
		[ProtoMember(4)]
		public List<Setting.ModifiableKeyValuePair<string, string>> PossibleValues { get; set; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060000E4 RID: 228 RVA: 0x00002936 File Offset: 0x00000B36
		// (set) Token: 0x060000E5 RID: 229 RVA: 0x0000293E File Offset: 0x00000B3E
		[ProtoMember(5)]
		public int DefaultValue { get; set; }

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060000E6 RID: 230 RVA: 0x00002947 File Offset: 0x00000B47
		// (set) Token: 0x060000E7 RID: 231 RVA: 0x0000294F File Offset: 0x00000B4F
		[ProtoMember(6)]
		public string LocNameKey { get; set; }

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x060000E8 RID: 232 RVA: 0x00002958 File Offset: 0x00000B58
		// (set) Token: 0x060000E9 RID: 233 RVA: 0x00002960 File Offset: 0x00000B60
		[ProtoMember(7)]
		public int LocValuesIndex { get; set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x060000EA RID: 234 RVA: 0x00002969 File Offset: 0x00000B69
		// (set) Token: 0x060000EB RID: 235 RVA: 0x00002971 File Offset: 0x00000B71
		[ProtoMember(8)]
		public ESettingType SettingType { get; set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x060000EC RID: 236 RVA: 0x0000297A File Offset: 0x00000B7A
		// (set) Token: 0x060000ED RID: 237 RVA: 0x00002982 File Offset: 0x00000B82
		[ProtoMember(9)]
		public bool bIsAutoSetSetting { get; set; }

		// Token: 0x060000EE RID: 238 RVA: 0x000063C4 File Offset: 0x000045C4
		public void NextSettingValue(bool bCanWrap)
		{
			int num = checked(this.SelectedValue + 1);
			if (num >= this.PossibleValues.Count)
			{
				if (bCanWrap)
				{
					num = 0;
				}
				else
				{
					num = this.SelectedValue;
				}
			}
			this.SelectedValue = num;
		}

		// Token: 0x060000EF RID: 239 RVA: 0x00006400 File Offset: 0x00004600
		public void PreviousSettingValue(bool bCanWrap)
		{
			checked
			{
				int num = this.SelectedValue - 1;
				if (num < 0)
				{
					if (bCanWrap)
					{
						num = this.PossibleValues.Count - 1;
					}
					else
					{
						num = this.SelectedValue;
					}
				}
				this.SelectedValue = num;
			}
		}

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x060000F0 RID: 240 RVA: 0x0000643C File Offset: 0x0000463C
		// (remove) Token: 0x060000F1 RID: 241 RVA: 0x00006474 File Offset: 0x00004674
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x060000F2 RID: 242 RVA: 0x0000298B File Offset: 0x00000B8B
		public void NotifyPropertyChanged(string PropertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
			}
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x000029A7 File Offset: 0x00000BA7
		public Setting()
		{
			this.bIsAutoSetSetting = false;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x000029B6 File Offset: 0x00000BB6
		private string GetDisplayAdapterKey(string sVendorId, string sDeviceId)
		{
			return sVendorId + "," + sDeviceId;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x000064AC File Offset: 0x000046AC
		private void ParseDisplayAdapterKey(string sDisplayAdapterKey, out string sOutVendorId, out string sOutDeviceId)
		{
			string[] array = sDisplayAdapterKey.Split(new char[] { ',' });
			sOutVendorId = array[0];
			sOutDeviceId = array[1];
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x000064D8 File Offset: 0x000046D8
		internal void Initialize()
		{
			switch (this.SettingType)
			{
			case ESettingType.Basic:
			case ESettingType.AutoDetect:
			case ESettingType.MaxAnisotropy:
				break;
			case ESettingType.Resolution:
				this.InitializeResolution();
				return;
			case ESettingType.GpuSelection:
				this.InitializeGpuSelection();
				return;
			case ESettingType.FramerateMode:
				this.InitializeFramerates();
				return;
			default:
			{
				string text = "Launcher.Setting.Initialize(): Unhandled ESettingType=" + this.SettingType;
				MainWindow.LogException(new Exception(text), text);
				break;
			}
			}
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00006544 File Offset: 0x00004744
		private void InitializeResolution()
		{
			this.PossibleValues = new List<Setting.ModifiableKeyValuePair<string, string>>();
			checked
			{
				string text = ResolutionHelper.SupportedResolution.FormatResolutionName((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight);
				this.DefaultValue = -1;
				int num = -1;
				foreach (ResolutionHelper.SupportedResolution supportedResolution in ResolutionHelper.SupportedResolutions)
				{
					string resolutionName = supportedResolution.ResolutionName;
					if (resolutionName.Equals(text))
					{
						this.DefaultValue = this.PossibleValues.Count;
					}
					if (num == -1 && supportedResolution.Width >= 1920 && supportedResolution.Height >= 1080)
					{
						num = this.PossibleValues.Count;
					}
					this.PossibleValues.Add(new Setting.ModifiableKeyValuePair<string, string>(resolutionName, resolutionName));
				}
				this.DefaultValue = Math.Max(this.DefaultValue, num);
				if (this.DefaultValue == -1 && this.PossibleValues.Count > 0)
				{
					this.DefaultValue = this.PossibleValues.Count - 1;
				}
			}
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00006654 File Offset: 0x00004854
		private void InitializeFramerates()
		{
			this.PossibleValues = new List<Setting.ModifiableKeyValuePair<string, string>>();
			int count = ResolutionHelper.Instance.FramerateModes.Count;
			checked
			{
				for (int i = 0; i < count; i++)
				{
					string text = ResolutionHelper.Instance.FramerateModes[i];
					this.PossibleValues.Add(new Setting.ModifiableKeyValuePair<string, string>(i.ToString(), text));
				}
			}
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x000066B4 File Offset: 0x000048B4
		private void InitializeGpuSelection()
		{
			string text;
			string text2;
			string text3;
			string text4;
			int num;
			string text5;
			NativeMethods.GetDisplayAdapterDescription(out text, out text2, out text3, out text4, out num, out text5);
			this.PossibleValues = new List<Setting.ModifiableKeyValuePair<string, string>>();
			string displayAdapterKey = this.GetDisplayAdapterKey(text2, text4);
			this.PossibleValues.Add(new Setting.ModifiableKeyValuePair<string, string>(displayAdapterKey, text5));
			this.DefaultValue = 0;
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00006704 File Offset: 0x00004904
		// FIX: Mirror VSync into [VSync] Enabled; anisotropy remains exponent-encoded as 2^selection.
		internal void SaveToConfig(IniFile iniFile)
		{
			Section section = iniFile.GetSection(this.ConfigSection);
			switch (this.SettingType)
			{
			case ESettingType.Basic:
			case ESettingType.AutoDetect:
			case ESettingType.FramerateMode:
				section.UpdateParameter(this.ConfigKey, this.ConfigValue, iniFile.FileName);
				if (this.ConfigKey == "UseVSync")
				{
					iniFile.SetValue("VSync", "Enabled", this.ConfigValue);
				}
				return;
			case ESettingType.Resolution:
			{
				string[] array = this.PossibleValues[this.SelectedValue].Key.Split(new char[] { 'x' });
				section.UpdateParameter("ResX", array[0], iniFile.FileName);
				section.UpdateParameter("ResY", array[1], iniFile.FileName);
				return;
			}
			case ESettingType.GpuSelection:
			{
				string text;
				string text2;
				this.ParseDisplayAdapterKey(this.PossibleValues[this.SelectedValue].Key, out text, out text2);
				section.UpdateParameter("GpuVendorId", text, iniFile.FileName);
				section.UpdateParameter("GpuDeviceId", text2, iniFile.FileName);
				return;
			}
			case ESettingType.MaxAnisotropy:
			{
				double num = double.Parse(this.ConfigValue);
				string text3 = Math.Pow(2.0, num).ToString();
				section.UpdateParameter(this.ConfigKey, text3, iniFile.FileName);
				return;
			}
			default:
			{
				string text4 = "SaveToConfig(): Unhandled SettingType=" + this.SettingType;
				MainWindow.LogException(new Exception(text4), text4);
				return;
			}
			}
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00006884 File Offset: 0x00004A84
		internal void SetDefaultValueFromIni(IniFile ini)
		{
			switch (this.SettingType)
			{
			case ESettingType.Basic:
			case ESettingType.AutoDetect:
			case ESettingType.FramerateMode:
				this.SetBasicSettingDefaultValueFromIni(ini);
				return;
			case ESettingType.Resolution:
				this.SetResolutionSettingDefaultValueFromIni(ini);
				return;
			case ESettingType.GpuSelection:
				this.SetGpuSettingDefaultValueFromIni(ini);
				return;
			case ESettingType.MaxAnisotropy:
				this.SetMaxAnisotropySettingDefaultValueFromIni(ini);
				return;
			default:
			{
				string text = "SetDefaultValueFromIni(): Unhandled SettingType=" + this.SettingType;
				MainWindow.LogException(new Exception(text), text);
				return;
			}
			}
		}

		// Token: 0x060000FC RID: 252 RVA: 0x000068FC File Offset: 0x00004AFC
		private void SetGpuSettingDefaultValueFromIni(IniFile ini)
		{
			Section section = ini.GetSection(this.ConfigSection);
			Parameter parameter = ((section != null) ? section.GetParameter("GpuVendorId") : null);
			Parameter parameter2 = ((section != null) ? section.GetParameter("GpuDeviceId") : null);
			if (parameter != null && parameter2 != null)
			{
				string sDisplayAdapterKey = this.GetDisplayAdapterKey(parameter.Value, parameter2.Value);
				if (((parameter != null && parameter2 != null) ? this.PossibleValues.FindIndex((Setting.ModifiableKeyValuePair<string, string> x) => x.Key == sDisplayAdapterKey) : (-1)) == -1)
				{
					string text;
					string text2;
					string text3;
					string text4;
					int num;
					string text5;
					NativeMethods.GetDisplayAdapterDescription(out text, out text2, out text3, out text4, out num, out text5);
					ini.SetValue(this.ConfigSection, "GpuVendorId", text2);
					ini.SetValue(this.ConfigSection, "GpuDeviceId", text4);
				}
			}
			this.SelectedValue = 0;
		}

		// Token: 0x060000FD RID: 253 RVA: 0x000069C0 File Offset: 0x00004BC0
		private void SetResolutionSettingDefaultValueFromIni(IniFile ini)
		{
			Section section = ini.GetSection(this.ConfigSection);
			Parameter parameter = ((section != null) ? section.GetParameter("ResX") : null);
			Parameter parameter2 = ((section != null) ? section.GetParameter("ResY") : null);
			string sDisplayResolution = ResolutionHelper.SupportedResolution.FormatResolutionName(int.Parse(parameter.Value), int.Parse(parameter2.Value));
			int num = ((parameter != null && parameter2 != null) ? this.PossibleValues.FindIndex((Setting.ModifiableKeyValuePair<string, string> x) => x.Key == sDisplayResolution) : (-1));
			if (num >= 0)
			{
				this.SelectedValue = num;
				return;
			}
			this.SelectedValue = this.DefaultValue;
			if (section == null)
			{
				ini.AddSection(this.ConfigSection);
			}
			if (parameter == null)
			{
				ini[this.ConfigSection].AddParameter(new Parameter("ResX", SystemParameters.PrimaryScreenWidth.ToString()));
			}
			if (parameter2 == null)
			{
				ini[this.ConfigSection].AddParameter(new Parameter("ResY", SystemParameters.PrimaryScreenHeight.ToString()));
			}
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00006AC8 File Offset: 0x00004CC8
		private void SetMaxAnisotropySettingDefaultValueFromIni(IniFile ini)
		{
			Section section = ini.GetSection(this.ConfigSection);
			Parameter parameter = ((section != null) ? section.GetParameter(this.ConfigKey) : null);
			int num = -1;
			if (parameter != null)
			{
				string ConfigValueLauncher = Math.Log(double.Parse(parameter.Value), 2.0).ToString();
				num = this.PossibleValues.FindIndex((Setting.ModifiableKeyValuePair<string, string> x) => x.Key.ToLowerInvariant() == ConfigValueLauncher.ToLowerInvariant());
			}
			if (num >= 0)
			{
				this.SelectedValue = num;
				return;
			}
			this.SelectedValue = this.DefaultValue;
			if (section == null)
			{
				ini.AddSection(this.ConfigSection);
			}
			if (parameter == null)
			{
				ini[this.ConfigSection].AddParameter(new Parameter(this.ConfigKey, this.ConfigValue));
			}
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00006B8C File Offset: 0x00004D8C
		private void SetBasicSettingDefaultValueFromIni(IniFile ini)
		{
			Section section = ini.GetSection(this.ConfigSection);
			Parameter parm = ((section != null) ? section.GetParameter(this.ConfigKey) : null);
			int num = ((parm != null) ? this.PossibleValues.FindIndex((Setting.ModifiableKeyValuePair<string, string> x) => x.Key.ToLowerInvariant() == parm.Value.ToLowerInvariant()) : (-1));
			if (num >= 0)
			{
				this.SelectedValue = num;
				return;
			}
			this.SelectedValue = this.DefaultValue;
			if (section == null)
			{
				ini.AddSection(this.ConfigSection);
			}
			if (parm == null)
			{
				ini[this.ConfigSection].AddParameter(new Parameter(this.ConfigKey, this.ConfigValue));
			}
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00006C38 File Offset: 0x00004E38
		internal bool IsDifferentFromSavedValue(IniFile iniFile)
		{
			switch (this.SettingType)
			{
			case ESettingType.Basic:
			case ESettingType.AutoDetect:
			case ESettingType.FramerateMode:
				return this.IsBasicSettingDifferentFromSavedValue(iniFile);
			case ESettingType.Resolution:
				return this.IsResolutionSettingDifferentFromSavedValue(iniFile);
			case ESettingType.GpuSelection:
				return this.IsGpuSettingDifferentFromSavedValue(iniFile);
			case ESettingType.MaxAnisotropy:
				return this.IsMaxAnisotropySettingDifferentFromSavedValue(iniFile);
			default:
				MainWindow.LogException(new Exception(MainWindow.Instance.GetResourceString("ConfigFileLoadError_Text")), "Forgot to set up IsDifferentFromSavedValue() for your new setting type.");
				return false;
			}
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00006CAC File Offset: 0x00004EAC
		private bool IsGpuSettingDifferentFromSavedValue(IniFile iniFile)
		{
			if (iniFile == null)
			{
				return false;
			}
			Section section = iniFile.GetSection(this.ConfigSection);
			Parameter parameter = section.GetParameter("GpuVendorId");
			Parameter parameter2 = section.GetParameter("GpuDeviceId");
			return parameter == null || parameter2 == null || !this.GetDisplayAdapterKey(parameter.Value, parameter2.Value).Equals(this.PossibleValues[this.SelectedValue].Key);
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00006D1C File Offset: 0x00004F1C
		private bool IsResolutionSettingDifferentFromSavedValue(IniFile iniFile)
		{
			if (iniFile == null)
			{
				return false;
			}
			Section section = iniFile.GetSection(this.ConfigSection);
			return !string.Format("{0}x{1}", section.GetParameter("ResX").Value, section.GetParameter("ResY").Value).Equals(this.PossibleValues[this.SelectedValue].Key);
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00006D84 File Offset: 0x00004F84
		private bool IsMaxAnisotropySettingDifferentFromSavedValue(IniFile iniFile)
		{
			if (iniFile == null)
			{
				return false;
			}
			string text = "";
			Section section = iniFile.GetSection(this.ConfigSection);
			if (section != null)
			{
				Parameter parameter = section.GetParameter(this.ConfigKey);
				if (parameter != null)
				{
					text = Math.Log(double.Parse(parameter.Value), 2.0).ToString();
				}
			}
			double num;
			double num2;
			return !text.ToLower().Equals(this.ConfigValue.ToLower()) && (!double.TryParse(text, out num) || !double.TryParse(this.ConfigValue, out num2) || !num.Equals(num2));
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00006E20 File Offset: 0x00005020
		private bool IsBasicSettingDifferentFromSavedValue(IniFile iniFile)
		{
			if (iniFile == null)
			{
				return false;
			}
			string text = "";
			Section section = iniFile.GetSection(this.ConfigSection);
			if (section != null)
			{
				Parameter parameter = section.GetParameter(this.ConfigKey);
				if (parameter != null)
				{
					text = parameter.Value;
				}
			}
			double num;
			double num2;
			return !text.ToLower().Equals(this.ConfigValue.ToLower()) && (!double.TryParse(text, out num) || !double.TryParse(this.ConfigValue, out num2) || !num.Equals(num2));
		}

		// Token: 0x040000C0 RID: 192
		private int _selectedValue;

		// Token: 0x040000C7 RID: 199
		internal const int TrueSettingIdx = 0;

		// Token: 0x040000C8 RID: 200
		internal const int FalseSettingIdx = 1;

		// Token: 0x040000C9 RID: 201
		internal const string FullscreenConfigKey = "Fullscreen";

		// Token: 0x040000CA RID: 202
		internal const string WindowedFullscreenConfigKey = "BorderlessFullscreenWindow";

		// Token: 0x040000CB RID: 203
		internal const string WindowModeConfigKey = "WindowMode";

		// Token: 0x040000CC RID: 204
		private const string GpuVendorIdConfigKey = "GpuVendorId";

		// Token: 0x040000CD RID: 205
		private const string GpuDeviceIdConfigKey = "GpuDeviceId";

		// Token: 0x040000CE RID: 206
		private const string DefaultGpuVendorId = "0x0000";

		// Token: 0x040000CF RID: 207
		private const string DefaultGpuDeviceId = "0x0000";

		// Token: 0x02000026 RID: 38
		[ProtoContract]
		public class ModifiableKeyValuePair<T1, T2>
		{
			// Token: 0x06000105 RID: 261 RVA: 0x0000208E File Offset: 0x0000028E
			public ModifiableKeyValuePair()
			{
			}

			// Token: 0x06000106 RID: 262 RVA: 0x000029C4 File Offset: 0x00000BC4
			public ModifiableKeyValuePair(T1 k, T2 v)
				: this()
			{
				this.Key = k;
				this.Value = v;
			}

			// Token: 0x1700003D RID: 61
			// (get) Token: 0x06000107 RID: 263 RVA: 0x000029DA File Offset: 0x00000BDA
			// (set) Token: 0x06000108 RID: 264 RVA: 0x000029E2 File Offset: 0x00000BE2
			[ProtoMember(1)]
			public T1 Key { get; set; }

			// Token: 0x1700003E RID: 62
			// (get) Token: 0x06000109 RID: 265 RVA: 0x000029EB File Offset: 0x00000BEB
			// (set) Token: 0x0600010A RID: 266 RVA: 0x000029F3 File Offset: 0x00000BF3
			[ProtoMember(2)]
			public T2 Value { get; set; }
		}

		// Token: 0x02000027 RID: 39
		public enum WindowModeEnum
		{
			// Token: 0x040000D4 RID: 212
			eFullscreen,
			// Token: 0x040000D5 RID: 213
			eWindowed,
			// Token: 0x040000D6 RID: 214
			eBorderlessFullscreenWindow
		}
	}
}

