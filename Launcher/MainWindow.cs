using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using ProtoBuf;

namespace Launcher
{
	// Token: 0x02000038 RID: 56
	public class MainWindow : Window, INotifyPropertyChanged, IComponentConnector, IStyleConnector
	{
		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000160 RID: 352 RVA: 0x00007DE0 File Offset: 0x00005FE0
		public static string LauncherVersion
		{
			get
			{
				checked
				{
					if (string.IsNullOrEmpty(MainWindow.m_sLauncherVersion))
					{
						Version version = Assembly.GetEntryAssembly().GetName().Version;
						DateTime dateTime = new DateTime(2000, 1, 1).Add(new TimeSpan(864000000000L * unchecked((long)version.Build) + 20000000L * unchecked((long)version.Revision)));
						MainWindow.m_sLauncherVersion = string.Concat(new object[] { "(Launcher: Version=", version, ", BuildDate=", dateTime, ")" });
					}
					return MainWindow.m_sLauncherVersion;
				}
			}
		}

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000161 RID: 353 RVA: 0x00002CA7 File Offset: 0x00000EA7
		public static MainWindow Instance
		{
			get
			{
				return MainWindow.m_instance;
			}
		}

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000162 RID: 354 RVA: 0x00002CAE File Offset: 0x00000EAE
		public int MaxLowResWidth
		{
			get
			{
				return ResolutionHelper.WindowedModeOnlySupportedResolution.Width;
			}
		}

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000163 RID: 355 RVA: 0x00002CBA File Offset: 0x00000EBA
		public int MaxLowResHeight
		{
			get
			{
				return ResolutionHelper.WindowedModeOnlySupportedResolution.Height;
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x06000164 RID: 356 RVA: 0x00002CC6 File Offset: 0x00000EC6
		// (set) Token: 0x06000165 RID: 357 RVA: 0x00002CCE File Offset: 0x00000ECE
		public LauncherSettings MySettings
		{
			get
			{
				return this._mySettings;
			}
			set
			{
				if (this._mySettings != value)
				{
					this._mySettings = value;
					this.NotifyPropertyChanged("MySettings");
				}
			}
		}

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06000166 RID: 358 RVA: 0x00007E80 File Offset: 0x00006080
		// (remove) Token: 0x06000167 RID: 359 RVA: 0x00007EB8 File Offset: 0x000060B8
		private event MainWindow.MessageBoxButtonClicked MessageBoxClickedEvent;

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000168 RID: 360 RVA: 0x00002CEB File Offset: 0x00000EEB
		// (set) Token: 0x06000169 RID: 361 RVA: 0x00002CF3 File Offset: 0x00000EF3
		public DynamicContent DynamicData { get; private set; }

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x0600016A RID: 362 RVA: 0x00002CFC File Offset: 0x00000EFC
		// (set) Token: 0x0600016B RID: 363 RVA: 0x00002D03 File Offset: 0x00000F03
		public static bool Verbose
		{
			get
			{
				return MainWindow.m_bVerboseMsg;
			}
			set
			{
				MainWindow.m_bVerboseMsg = value;
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600016C RID: 364 RVA: 0x00002D0B File Offset: 0x00000F0B
		public static string VerboseIndent
		{
			get
			{
				return MainWindow.m_sVerboseIndents[MainWindow.m_nVerboseCurrIndent];
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x0600016D RID: 365 RVA: 0x00002D18 File Offset: 0x00000F18
		// (set) Token: 0x0600016E RID: 366 RVA: 0x00002D1F File Offset: 0x00000F1F
		public static int VerboseCurrentIndentIndex
		{
			get
			{
				return MainWindow.m_nVerboseCurrIndent;
			}
			set
			{
				MainWindow.m_nVerboseCurrIndent = Math.Min(checked(MainWindow.m_sVerboseIndents.Length - 1), Math.Max(0, value));
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600016F RID: 367 RVA: 0x00002D3B File Offset: 0x00000F3B
		// (set) Token: 0x06000170 RID: 368 RVA: 0x00002D43 File Offset: 0x00000F43
		public ObservableCollection<Setting> ConfigSettings
		{
			get
			{
				return this._configSettings;
			}
			set
			{
				if (this._configSettings != value)
				{
					this._configSettings = value;
					this.NotifyPropertyChanged("ConfigSettings");
				}
			}
		}

		// Token: 0x06000171 RID: 369 RVA: 0x00007EF0 File Offset: 0x000060F0
		private IniFile GetIniFile(string sConfigFileSuffix)
		{
			return this.IniFiles.Find((IniFile x) => this.IsIniFileForSuffix(x, sConfigFileSuffix));
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00007F28 File Offset: 0x00006128
		private IniFile GetInstalledIniFile(string sConfigFileSuffix)
		{
			return this.IniFiles.Find((IniFile x) => this.IsInstalledIniFileForSuffix(x, sConfigFileSuffix));
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000173 RID: 371 RVA: 0x00007F60 File Offset: 0x00006160
		public bool HaveConfigSettingsChanged
		{
			get
			{
				foreach (Setting setting in this.ConfigSettings)
				{
					IniFile iniFile = this.GetIniFile(setting.ConfigFileSuffix);
					if (setting.IsDifferentFromSavedValue(iniFile))
					{
						MainWindow.LogMessage(string.Concat(new string[] { "Launcher.MainWindow.HaveConfigSettingsChanged(): setting: section=", setting.ConfigSection, ", key=", setting.ConfigKey, ", value=", setting.ConfigValue, " is different from the saved value." }), false);
						return true;
					}
				}
				return false;
			}
		}

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06000174 RID: 372 RVA: 0x00008010 File Offset: 0x00006210
		// (remove) Token: 0x06000175 RID: 373 RVA: 0x00008048 File Offset: 0x00006248
		public event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x06000176 RID: 374 RVA: 0x00002D60 File Offset: 0x00000F60
		public void NotifyPropertyChanged(string PropertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000177 RID: 375 RVA: 0x00002D7C File Offset: 0x00000F7C
		public static string GameName
		{
			get
			{
				if (string.IsNullOrEmpty(MainWindow.m_sGameName))
				{
					MainWindow.m_sGameName = MainWindow.GetGameName();
				}
				return MainWindow.m_sGameName;
			}
		}

		// Token: 0x06000178 RID: 376 RVA: 0x00002D99 File Offset: 0x00000F99
		private bool IsSupportedGraphicsQuality(int nGraphicsQuality)
		{
			return nGraphicsQuality == -1 || nGraphicsQuality >= 6;
		}

		// Token: 0x06000179 RID: 377 RVA: 0x00002DA8 File Offset: 0x00000FA8
		private MainWindow.GraphicsPresetType ConvertGraphicsQuality(int nGraphicsQuality)
		{
			nGraphicsQuality = Math.Max(6, Math.Min(9, nGraphicsQuality));
			return checked(nGraphicsQuality - 6 + MainWindow.GraphicsPresetType.eLow);
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x0600017A RID: 378 RVA: 0x00008080 File Offset: 0x00006280
		private static string InstalledConfigPath
		{
			get
			{
				if (string.IsNullOrEmpty(MainWindow.m_sInstalledConfigPath))
				{
					MainWindow.m_sInstalledConfigPath = Path.Combine(new string[]
					{
						AppDomain.CurrentDomain.BaseDirectory,
						"..",
						"..",
						"WillowGame",
						"Config"
					});
				}
				return MainWindow.m_sInstalledConfigPath;
			}
		}

		// Token: 0x0600017B RID: 379 RVA: 0x000080DC File Offset: 0x000062DC
		public MainWindow()
		{
			this.ConfigSettings = new ObservableCollection<Setting>();
			this.IniFiles = new List<IniFile>();
			this.InstalledIniFiles = new List<IniFile>();
			this.DynamicData = new DynamicContent(base.Dispatcher);
			this.MusicPlayer = new MediaPlayer();
			this.ButtonClickSound = new SoundEffect(this, 2, "ButtonClick", 0.2f);
			this.WhooshSound = new SoundEffect(this, 2, "Whoosh", 0.1f);
			this.MouseOverSound = new SoundEffect(this, 4, "MouseOver", 0.05f);
			this.AppliedSound = new SoundEffect(this, 1, "Applied", 0.2f);
			this.ReadMePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", string.Format("ReadMe.{0}.txt", Thread.CurrentThread.CurrentUICulture.Name));
			this.InitializeComponent();
			this.InitGraphicsPresetKeys();
			this.grdMain.Visibility = Visibility.Visible;
			this.grdSettings.Visibility = Visibility.Collapsed;
			this.grdMessageBox.Visibility = Visibility.Collapsed;
			this.grdHelp.Visibility = Visibility.Collapsed;
			this.focusedGrid = this.grdMain;
			MainWindow.m_instance = this;
		}

		// Token: 0x0600017C RID: 380 RVA: 0x00008478 File Offset: 0x00006678
		private void InitGraphicsPresetKeys()
		{
			this.m_configKeys = new Dictionary<string, MainWindow.GraphicsOptionType>();
			foreach (KeyValuePair<MainWindow.ConfigPresetKeys, MainWindow.GraphicsOptionType> keyValuePair in this.m_graphicsConfigAndPresetKeys)
			{
				if (!this.m_configKeys.ContainsKey(keyValuePair.Key.m_sConfigKey))
				{
					this.m_configKeys[keyValuePair.Key.m_sConfigKey] = keyValuePair.Value;
				}
			}
		}

		// Token: 0x0600017D RID: 381 RVA: 0x00008508 File Offset: 0x00006708
		public static void LogMessage(string sMsg, bool bWriteToConsole = false)
		{
			if (MainWindow.m_launcherLog == null)
			{
				string text = MainWindow.GamePath + "\\Launcher\\Logs";
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
				}
				string text2 = Path.Combine(text, "Launcher.log");
				if (!File.Exists(text2))
				{
					MainWindow.m_launcherLog = File.CreateText(text2);
				}
				else
				{
					MainWindow.m_launcherLog = File.AppendText(text2);
				}
				if (MainWindow.m_launcherLog != null)
				{
					MainWindow.m_launcherLog.AutoFlush = true;
					MainWindow.m_launcherLog.WriteLine("\n----------------------------------------------------------------\n" + DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss.fff zzz:  ") + "BEGIN SESSION " + MainWindow.LauncherVersion);
				}
			}
			checked
			{
				if (MainWindow.m_launcherLog != null)
				{
					MainWindow.m_nLauncherLogSessionLineNum++;
					MainWindow.m_launcherLog.WriteLine(string.Concat(new string[]
					{
						DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss.fff zzz:  "),
						MainWindow.m_nLauncherLogSessionLineNum.ToString("D6"),
						":  ",
						MainWindow.VerboseIndent,
						sMsg
					}));
				}
				if (bWriteToConsole)
				{
					Console.WriteLine(MainWindow.VerboseIndent + sMsg);
				}
			}
		}

		// Token: 0x0600017E RID: 382 RVA: 0x00008624 File Offset: 0x00006824
		public static void LogClose()
		{
			if (MainWindow.m_launcherLog != null)
			{
				MainWindow.m_launcherLog.WriteLine(DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss.fff zzz:  ") + "END SESSION " + MainWindow.LauncherVersion + "\n----------------------------------------------------------------\n");
			}
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00002DBF File Offset: 0x00000FBF
		public static void LogException(Exception e, string sLogMsg)
		{
			MainWindow.LogMessage(sLogMsg + "\n" + e.StackTrace, false);
			throw e;
		}

		// Token: 0x06000180 RID: 384 RVA: 0x00002DD9 File Offset: 0x00000FD9
		public void DragWindow(object sender, MouseButtonEventArgs args)
		{
			base.DragMove();
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00002DE1 File Offset: 0x00000FE1
		public void Generic_MouseEnter(object sender, EventArgs args)
		{
			this.MouseOverSound.Play();
		}

		// Token: 0x06000182 RID: 386 RVA: 0x00002DEE File Offset: 0x00000FEE
		public void Generic_ButtonClick(object sender, EventArgs args)
		{
			this.ButtonClickSound.Play();
		}

		// Token: 0x06000183 RID: 387 RVA: 0x00008668 File Offset: 0x00006868
		public static string UCWords(string p)
		{
			bool flag = p.Length > 0 && p[0] != ' ';
			string text = string.Empty;
			checked
			{
				for (int i = 0; i < p.Length; i++)
				{
					if (flag)
					{
						text += p.ToUpper(CultureInfo.CurrentCulture)[i].ToString();
						flag = false;
					}
					else
					{
						if (p[i] == ' ')
						{
							flag = true;
						}
						text += p.ToLower(CultureInfo.CurrentCulture)[i].ToString();
					}
				}
				return text;
			}
		}

		// Token: 0x06000184 RID: 388 RVA: 0x00002DFB File Offset: 0x00000FFB
		public string GetResourceString(string Key)
		{
			if (this.resourceManager == null)
			{
				this.resourceManager = new ResourceManager("Launcher.Properties.Resources", base.GetType().Assembly);
			}
			return this.resourceManager.GetString(Key);
		}

		// Token: 0x06000185 RID: 389 RVA: 0x000086FC File Offset: 0x000068FC
		private void DisplayYesNoCancelBox(string Question)
		{
			this.PreMessageBoxGrid = this.focusedGrid;
			this.mbQuestion.Content = Question;
			this.mbButtonOne.Content = this.GetResourceString("MessageBox_Yes");
			this.mbButtonTwo.Content = this.GetResourceString("MessageBox_No");
			this.mbButtonThree.Content = this.GetResourceString("MessageBox_Cancel");
			this.mbButtonOne.Visibility = Visibility.Visible;
			this.mbButtonTwo.Visibility = Visibility.Visible;
			this.mbButtonThree.Visibility = Visibility.Visible;
			this.TransitionLeft(this.grdMessageBox);
		}

		// Token: 0x06000186 RID: 390 RVA: 0x00008794 File Offset: 0x00006994
		public void DisplayOkBox(string Message)
		{
			this.PreMessageBoxGrid = this.focusedGrid;
			this.mbQuestion.Content = Message;
			this.mbButtonTwo.Content = this.GetResourceString("MessageBox_Ok");
			this.mbButtonOne.Visibility = Visibility.Collapsed;
			this.mbButtonTwo.Visibility = Visibility.Visible;
			this.mbButtonThree.Visibility = Visibility.Collapsed;
			this.TransitionLeft(this.grdMessageBox);
		}

		// Token: 0x06000187 RID: 391 RVA: 0x00008800 File Offset: 0x00006A00
		private Parameter GetIniParameter(IniFile iniFile, string sSectionName, string sParameterName)
		{
			if (iniFile != null && iniFile.HasSection(sSectionName))
			{
				Section section = iniFile.GetSection(sSectionName);
				if (section != null)
				{
					return section.GetParameter(sParameterName);
				}
			}
			return null;
		}

		// Token: 0x06000188 RID: 392 RVA: 0x00008830 File Offset: 0x00006A30
		private void SetIniParameter(IniFile iniFile, string sSectionName, string sParameterName, string sParameterValue)
		{
			if (iniFile != null)
			{
				if (!iniFile.HasSection(sSectionName))
				{
					iniFile.AddSection(sSectionName);
				}
				Section section = iniFile.GetSection(sSectionName);
				Parameter parameter = section.GetParameter(sParameterName);
				if (parameter == null)
				{
					Parameter parameter2 = new Parameter(sParameterName, sParameterValue);
					section.AddParameter(parameter2);
					return;
				}
				parameter.SetValue(sParameterValue, iniFile.FileName, sSectionName, false);
			}
		}

		// Token: 0x06000189 RID: 393 RVA: 0x00008884 File Offset: 0x00006A84
		private string GetIniValue(IniFile iniFile, string sSectionName, string sParameterName)
		{
			if (iniFile != null && iniFile.HasSection(sSectionName))
			{
				Section section = iniFile.GetSection(sSectionName);
				if (section != null)
				{
					return section.GetParameter(sParameterName).Value;
				}
			}
			return null;
		}

		// Token: 0x0600018A RID: 394 RVA: 0x00002E2C File Offset: 0x0000102C
		private string GetInstalledConfigPathname(string sBaseName)
		{
			return MainWindow.InstalledConfigPath + sBaseName + ".ini";
		}

		// Token: 0x0600018B RID: 395 RVA: 0x00002E3E File Offset: 0x0000103E
		private string GetUserConfigPathname(string sBaseName)
		{
			return MainWindow.UserConfigPath + sBaseName + ".ini";
		}

		// Token: 0x0600018C RID: 396 RVA: 0x00002E50 File Offset: 0x00001050
		private void InitDefaultIni(string sBaseName, ref IniFile refIniFile)
		{
			if (refIniFile == null)
			{
				refIniFile = IniFile.Parse(this.GetInstalledConfigPathname("\\Default" + sBaseName));
				if (refIniFile != null)
				{
					MainWindow.LogMessage("InitDefaultIni(): Loaded " + refIniFile.FileName, false);
				}
			}
		}

		// Token: 0x0600018D RID: 397 RVA: 0x00002E89 File Offset: 0x00001089
		private bool IsSteamInstallFolder()
		{
			bool flag = MainWindow.InstalledConfigPath.Contains("steamapps");
			if (flag)
			{
				this.CurrentExeIndex = checked(this.ExesToTryStarting.Count - 1);
			}
			return flag;
		}

		// Token: 0x0600018E RID: 398 RVA: 0x000088B8 File Offset: 0x00006AB8
		private string ExtractPath(string sPathname)
		{
			string text = sPathname.Replace('/', '\\');
			int num = text.LastIndexOf('\\');
			if (num != -1)
			{
				text = text.Substring(0, num);
			}
			return text;
		}

		// Token: 0x0600018F RID: 399 RVA: 0x00002EB0 File Offset: 0x000010B0
		private void EnsurePathExists(string sPath)
		{
			if (!Directory.Exists(sPath))
			{
				Directory.CreateDirectory(sPath);
				MainWindow.LogMessage("EnsurePathExists(): Created path: " + sPath, false);
			}
		}

		// Token: 0x06000190 RID: 400 RVA: 0x000088E8 File Offset: 0x00006AE8
		private bool InitWillowIni(string sBaseName, ref IniFile refIniFile, IniFile defaultIni = null)
		{
			bool flag = true;
			if (refIniFile == null)
			{
				string text = "\\Willow" + sBaseName;
				string text2 = this.GetInstalledConfigPathname(text);
				refIniFile = IniFile.Parse(text2);
				if (refIniFile == null)
				{
					text2 = this.GetUserConfigPathname(text);
					refIniFile = IniFile.Parse(text2);
					if (refIniFile != null)
					{
						MainWindow.LogMessage("InitWillowIni(): Found and loaded user ini: " + refIniFile.FileName, false);
					}
				}
				if (refIniFile == null)
				{
					if (defaultIni == null)
					{
						string installedConfigFullPathForSuffix = this.GetInstalledConfigFullPathForSuffix(sBaseName);
						defaultIni = IniFile.Parse(installedConfigFullPathForSuffix);
						if (defaultIni == null)
						{
							MainWindow.LogException(new Exception(this.GetResourceString("ConfigFileLoadError_Text")), "Unable parse default filename=" + installedConfigFullPathForSuffix + ", willowPathname=" + text2);
						}
						flag = false;
					}
					this.EnsurePathExists(this.ExtractPath(text2));
					refIniFile = defaultIni.Copy(text2);
					refIniFile.Save(null, false);
					MainWindow.LogMessage("InitWillowIni(): Created userIni=" + refIniFile.FileName + " from defaultIni=" + defaultIni.FileName, false);
				}
			}
			return flag;
		}

		// Token: 0x06000191 RID: 401 RVA: 0x000089D0 File Offset: 0x00006BD0
		private void InitializeBaselineIniFiles()
		{
			this.InitDefaultIni("Engine", ref this.m_defaultEngineIni);
			this.IniFiles.Add(this.m_defaultEngineIni);
			this.InitWillowIni("Engine", ref this.m_willowEngineIni, this.m_defaultEngineIni);
			this.IniFiles.Add(this.m_willowEngineIni);
			this.InitializeSupportedAspectRatios();
			this.InitializeTargetFramerates();
			this.InitDefaultIni("Game", ref this.m_defaultGameIni);
			this.IniFiles.Add(this.m_defaultGameIni);
			this.InitWillowIni("Game", ref this.m_willowGameIni, this.m_defaultGameIni);
			this.IniFiles.Add(this.m_willowGameIni);
			this.InitDefaultIni("Launcher", ref this.m_defaultLauncherIni);
			this.IniFiles.Add(this.m_defaultLauncherIni);
			this.InitWillowIni("Launcher", ref this.m_willowLauncherIni, this.m_defaultLauncherIni);
			this.IniFiles.Add(this.m_willowLauncherIni);
		}

		// Token: 0x06000192 RID: 402 RVA: 0x00008ACC File Offset: 0x00006CCC
		private void InitializeSupportedAspectRatios()
		{
			ResolutionHelper resolutionHelper = new ResolutionHelper();
			string text = this.m_willowEngineIni.GetValue("SystemSettings.SupportedAspectRatios", "MaxAspectRatioError", "0.08");
			if (Thread.CurrentThread.CurrentUICulture.Name == "fr-FR")
			{
				text = text.Replace('.', ',');
			}
			resolutionHelper.MaxAspectRatioError = float.Parse(text);
			resolutionHelper.ClearSupportedAspectRatios();
			int num = 0;
			string text2 = string.Format("SupportedAspectRatio({0})", num);
			string text3 = this.m_willowEngineIni.GetValue("SystemSettings.SupportedAspectRatios", text2, null);
			checked
			{
				while (!string.IsNullOrEmpty(text3))
				{
					if (Thread.CurrentThread.CurrentUICulture.Name == "fr-FR")
					{
						text3 = text3.Replace('.', ',');
					}
					float num2 = float.Parse(text3);
					resolutionHelper.AddSupportedAspectRatio(num2);
					num++;
					text2 = string.Format("SupportedAspectRatio({0})", num);
					text3 = this.m_willowEngineIni.GetValue("SystemSettings.SupportedAspectRatios", text2, null);
					MainWindow.LogMessage(string.Concat(new object[] { "InitializeSupportedAspectRatios(): ", text2, "=", num2 }), false);
				}
				if (resolutionHelper.NumSupportedAspectRatios == 0)
				{
					MainWindow.LogException(new Exception(this.GetResourceString("ConfigFileLoadError_Text")), "No supported display aspect ratios parsed from " + this.m_willowEngineIni.FileName);
				}
			}
		}

		// Token: 0x06000193 RID: 403 RVA: 0x00008C30 File Offset: 0x00006E30
		private void InitializeTargetFramerates()
		{
			int.Parse(this.m_willowEngineIni.GetValue("SystemSettings", "FramerateMode", "0"));
			string value = this.m_willowEngineIni.GetValue("SystemSettings", "MinSmoothedFramerate", "22");
			ResolutionHelper.Instance.MinSmoothedFramerate = int.Parse(value);
			string value2 = this.m_willowEngineIni.GetValue("SystemSettings", "MaxSmoothedFramerate", "62");
			ResolutionHelper.Instance.MaxSmoothedFramerate = int.Parse(value2);
			string localizedMenuString = this.GetLocalizedMenuString("FEaudiovideo", "SmoothedFramerate");
			string localizedMenuString2 = this.GetLocalizedMenuString("FEaudiovideo", "UnlimitedFramerate");
			int num = 0;
			string text = string.Format("FramerateSetting({0})", num);
			string text2 = this.m_willowEngineIni.GetValue("SystemSettings.FramerateSettings", text, null);
			checked
			{
				while (!string.IsNullOrEmpty(text2))
				{
					MainWindow.LogMessage("InitializeTargetFramerates(): " + text + "=" + text2, false);
					if (num == 0 || text2.Equals("UNLIMITED", StringComparison.InvariantCultureIgnoreCase))
					{
						if (num == 0)
						{
							text2 = string.Format("{0} {1}-{2}", localizedMenuString, ResolutionHelper.Instance.MinSmoothedFramerate, ResolutionHelper.Instance.MaxSmoothedFramerate);
						}
						else
						{
							text2 = localizedMenuString2;
						}
					}
					ResolutionHelper.Instance.FramerateModes.Add(text2);
					num++;
					text = string.Format("FramerateSetting({0})", num);
					text2 = this.m_willowEngineIni.GetValue("SystemSettings.FramerateSettings", text, null);
				}
				if (ResolutionHelper.Instance.FramerateModes.Count == 0)
				{
					MainWindow.LogException(new Exception(this.GetResourceString("ConfigFileLoadError_Text")), "No supported framerate settings parsed from " + this.m_willowEngineIni.FileName);
				}
			}
		}

		// Token: 0x06000194 RID: 404 RVA: 0x00002ED2 File Offset: 0x000010D2
		private bool IsLauncherDisabled()
		{
			return false;
		}

		// Token: 0x06000195 RID: 405 RVA: 0x00002ED5 File Offset: 0x000010D5
		private bool IsWindowsKeyDisabled()
		{
			return this.GetGameInfoToggle("DisableWindowsKey");
		}

		// Token: 0x06000196 RID: 406 RVA: 0x00008DEC File Offset: 0x00006FEC
		private bool GetGameInfoToggle(string sParameterName)
		{
			if (this.m_defaultGameIni != null && this.m_willowGameIni != null)
			{
				Parameter parameter = this.GetIniParameter(this.m_willowGameIni, "Engine.GameInfo", sParameterName);
				if (parameter == null)
				{
					Parameter iniParameter = this.GetIniParameter(this.m_defaultGameIni, "Engine.GameInfo", sParameterName);
					this.SetIniParameter(this.m_willowGameIni, "Engine.GameInfo", sParameterName, iniParameter.Value);
					this.m_willowGameIni.Save(null, false);
					parameter = this.GetIniParameter(this.m_willowGameIni, "Engine.GameInfo", sParameterName);
				}
				return parameter.Value.Equals(MainWindow.m_configBoolLabel[MainWindow.ConfigBool.eTrue], StringComparison.InvariantCultureIgnoreCase);
			}
			return false;
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00002EE2 File Offset: 0x000010E2
		private bool DoesGameExeExist(int nExeIndex)
		{
			return nExeIndex >= 0 && nExeIndex < this.ExesToTryStarting.Count && this.ExesToTryStarting.Count > 0 && File.Exists(this.ExesToTryStarting[nExeIndex]);
		}

		// Token: 0x06000198 RID: 408 RVA: 0x00008E88 File Offset: 0x00007088
		private void TryAutoLaunchGame()
		{
			if (this.IsLauncherDisabled())
			{
				MainWindow.LogMessage("Launcher is disabled.", false);
				this.InitDefaultIni("Launcher", ref this.m_defaultLauncherIni);
				this.InitWillowIni("Launcher", ref this.m_willowLauncherIni, null);
				Parameter iniParameter = this.GetIniParameter(this.m_willowLauncherIni, "LauncherInfo", "LastExeIndex");
				if (iniParameter != null)
				{
					int num = int.Parse(iniParameter.Value);
					if (this.IsSteamInstallFolder())
					{
						num = checked(this.ExesToTryStarting.Count - 1);
						this.CurrentExeIndex = num;
					}
					if (this.DoesGameExeExist(num))
					{
						this.CurrentExeIndex = num;
						MainWindow.LogMessage(string.Concat(new object[]
						{
							"Trying to autolaunch game: CurrentExeIndex=",
							this.CurrentExeIndex,
							"; exe=",
							this.ExesToTryStarting[this.CurrentExeIndex]
						}), false);
						this.TryStartChosenExe();
						return;
					}
					MainWindow.LogMessage(string.Concat(new object[] { "Launcher is disabled, but index was invalid: nLastExeIndex=", num, ": resetting to CurrentExe=", this.CurrentExeIndex }), false);
					this.SetIniParameter(this.m_willowLauncherIni, "Engine.GameInfo", "LastExeIndex", this.CurrentExeIndex.ToString());
				}
			}
		}

		// Token: 0x06000199 RID: 409 RVA: 0x00002F17 File Offset: 0x00001117
		private void PlayGame()
		{
			this.CurrentExeIndex = 0;
			this.TryPlayGame();
		}

		// Token: 0x0600019A RID: 410 RVA: 0x00008FC8 File Offset: 0x000071C8
		private void TryPlayGame()
		{
			int count = this.ExesToTryStarting.Count;
			checked
			{
				for (int i = 0; i < count; i++)
				{
					string text = this.ExesToTryStarting[i];
					if (File.Exists(text))
					{
						this.CurrentExeIndex = i;
						MainWindow.LogMessage(string.Concat(new object[] { "TryPlayGame(): Trying game executable ", i, ": ", text }), false);
						this.TryStartChosenExe();
						return;
					}
					MainWindow.LogMessage(string.Concat(new object[] { "TryPlayGame(): Can't find game executable ", i, ": ", text }), false);
				}
				string text2 = "Can't find any game executables!";
				MainWindow.LogException(new Exception(text2), text2);
			}
		}

		// Token: 0x0600019B RID: 411 RVA: 0x00009084 File Offset: 0x00007284
		private void ChooseStartupExe_Clicked(int ButtonIdx)
		{
			this.MessageBoxClickedEvent -= this.ChooseStartupExe_Clicked;
			if (ButtonIdx == 0)
			{
				this.TryStartChosenExe();
				return;
			}
			checked
			{
				if (ButtonIdx == 1)
				{
					this.CurrentExeIndex++;
					if (this.CurrentExeIndex >= this.ExesToTryStarting.Count)
					{
						this.CurrentExeIndex = -1;
					}
					this.TryPlayGame();
					return;
				}
				this.TransitionRight(this.grdMain);
			}
		}

		// Token: 0x0600019C RID: 412 RVA: 0x000090EC File Offset: 0x000072EC
		private void TryStartChosenExe()
		{
			try
			{
				if (this.CurrentExeIndex >= 0 && this.CurrentExeIndex < this.ExesToTryStarting.Count)
				{
					string text = this.ExesToTryStarting[this.CurrentExeIndex];
					if (File.Exists(text))
					{
						string text2 = string.Empty;
						if (Application.Current.Properties["StartupArguments"] != null)
						{
							text2 = Application.Current.Properties["StartupArguments"].ToString();
						}
						this.InitDefaultIni("Launcher", ref this.m_defaultLauncherIni);
						this.InitWillowIni("Launcher", ref this.m_willowLauncherIni, this.m_defaultLauncherIni);
						Process.Start(this.ExesToTryStarting[this.CurrentExeIndex], text2);
						this.SetIniParameter(this.m_willowLauncherIni, "LauncherInfo", "LastExeIndex", this.CurrentExeIndex.ToString());
						this.m_willowLauncherIni.Save(null, false);
						MainWindow.LogMessage(string.Concat(new object[]
						{
							"Running ",
							this.ExesToTryStarting[this.CurrentExeIndex],
							" (CurrentExe=",
							this.CurrentExeIndex,
							")"
						}), false);
					}
					else
					{
						MainWindow.LogMessage(string.Concat(new object[] { "TryStartChosenExe(): Couldn't find executable=", text, "; CurrentExeIndex=", this.CurrentExeIndex }), false);
					}
				}
				else
				{
					MainWindow.LogMessage("TryStartChosenExe(): Tried to run executable but CurrentExeIndex is out-of-range: CurrentExeIndex=" + this.CurrentExeIndex, false);
				}
			}
			catch (Exception ex)
			{
				MainWindow.LogMessage(string.Concat(new object[]
				{
					"MainWindow.TryStartChosenExe(): Can't start executable: exe=",
					this.ExesToTryStarting[this.CurrentExeIndex],
					"; CurrentExeIndex=",
					this.CurrentExeIndex,
					"; startupArgs =",
					Application.Current.Properties["StartupArguments"].ToString(),
					"; exception=",
					ex.Message
				}), false);
			}
			base.Close();
		}

		// Token: 0x0600019D RID: 413 RVA: 0x0000931C File Offset: 0x0000751C
		private string GetInstalledConfigFullPathForSuffix(string suffix)
		{
			string text = string.Format("Default{0}.ini", suffix);
			return Path.Combine(MainWindow.InstalledConfigPath, text);
		}

		// Token: 0x0600019E RID: 414 RVA: 0x00009340 File Offset: 0x00007540
		private string GetConfigFullPathForSuffix(string suffix)
		{
			IniFile iniFile = null;
			this.InitWillowIni(suffix, ref iniFile, null);
			string text = string.Format("Willow{0}.ini", suffix);
			string text2 = Path.Combine(MainWindow.UserConfigPath, text);
			if (File.Exists(text2))
			{
				return text2;
			}
			return Path.Combine(MainWindow.InstalledConfigPath, text);
		}

		// Token: 0x0600019F RID: 415 RVA: 0x00009388 File Offset: 0x00007588
		public static string GetGraphicsPresetsPathname(out string sOutDefaultGraphicsPresetsPathaname, out string sOutEngineComfigPathname, string sConfigSuffix)
		{
			sOutDefaultGraphicsPresetsPathaname = Path.Combine(MainWindow.InstalledConfigPath, string.Format("Default{0}.ini", sConfigSuffix));
			sOutEngineComfigPathname = Path.Combine(MainWindow.InstalledEngineConfigPath, string.Format("Default{0}.ini", sConfigSuffix));
			string text = Path.Combine(MainWindow.InstalledConfigPath, string.Format("Willow{0}.ini", sConfigSuffix));
			if (!File.Exists(text))
			{
				text = Path.Combine(MainWindow.UserConfigPath, string.Format("Willow{0}.ini", sConfigSuffix));
			}
			return text;
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x00002F26 File Offset: 0x00001126
		private void OnSplashAnimationComplete(object sender, EventArgs e)
		{
			this.lblOverlayForeground.Visibility = Visibility.Collapsed;
			this.lblOverlayBackground.Visibility = Visibility.Collapsed;
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x000093F8 File Offset: 0x000075F8
		private void ShowSplashText(string SplashTextResourceKey)
		{
			this.lblOverlayForeground.Visibility = Visibility.Visible;
			this.lblOverlayBackground.Visibility = Visibility.Visible;
			this.lblOverlayBackground.Content = this.GetResourceString(SplashTextResourceKey);
			this.lblOverlayForeground.Content = this.GetResourceString(SplashTextResourceKey);
			this.SpinAndFadeFromBack.Begin(this.lblOverlayForeground);
			this.SpinAndFadeThrough.Begin(this.lblOverlayBackground);
			this.AppliedSound.Play();
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x00002F40 File Offset: 0x00001140
		private void OnFadeAnimationCompleted(Grid sender, Storyboard source)
		{
			source.Completed -= this.FadeOutComplete;
			sender.Visibility = Visibility.Collapsed;
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x00009470 File Offset: 0x00007670
		private void Transition(FrameworkElement to, Storyboard sbOut, Storyboard sbIn)
		{
			to.Visibility = Visibility.Visible;
			this.WhooshSound.Play();
			if (this.focusedGrid != to)
			{
				Grid grd = this.focusedGrid;
				this.FadeOutComplete = delegate(object s, EventArgs _)
				{
					this.OnFadeAnimationCompleted(grd, sbOut);
				};
				sbOut.Completed += this.FadeOutComplete;
			}
			sbOut.Begin(this.focusedGrid);
			sbIn.Begin(to);
			if (to is Grid)
			{
				this.focusedGrid = to as Grid;
				if (this.gamepadHandler != null)
				{
					this.gamepadHandler.UpdateFocusedGrid(this.focusedGrid);
				}
			}
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x00002F55 File Offset: 0x00001155
		private void TransitionLeft(FrameworkElement to)
		{
			this.Transition(to, this.slideLeftOut, this.slideLeftIn);
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x00002F6A File Offset: 0x0000116A
		private void TransitionRight(FrameworkElement to)
		{
			this.Transition(to, this.slideRightOut, this.slideRightIn);
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x00002F7F File Offset: 0x0000117F
		private void btnPlay_Click(object sender, RoutedEventArgs e)
		{
			if (this.HaveConfigSettingsChanged)
			{
				this.MessageBoxClickedEvent += this.UnsavedChanges_Click;
				this.DisplayYesNoCancelBox(this.GetResourceString("UnsavedChanges_Text"));
				return;
			}
			this.PlayGame();
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x00002FB3 File Offset: 0x000011B3
		private void UnsavedChanges_Click(int ButtonIdx)
		{
			this.MessageBoxClickedEvent -= this.UnsavedChanges_Click;
			if (ButtonIdx == 2)
			{
				this.TransitionRight(this.PreMessageBoxGrid);
				return;
			}
			if (ButtonIdx == 0)
			{
				this.btnSettings_Apply_Click(null, EventArgs.Empty as RoutedEventArgs);
			}
			this.PlayGame();
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x00002FF2 File Offset: 0x000011F2
		private void OkBoxClicked_MinSpec(int ButtonIdx)
		{
			this.MessageBoxClickedEvent -= this.OkBoxClicked_MinSpec;
			this.TransitionRight(this.grdMain);
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x00003012 File Offset: 0x00001212
		private void OkBoxClicked_CloseApp(int ButtonIdx)
		{
			this.MessageBoxClickedEvent -= this.OkBoxClicked_CloseApp;
			base.Close();
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000302C File Offset: 0x0000122C
		private void mbButtonOne_Click(object sender, RoutedEventArgs e)
		{
			if (this.MessageBoxClickedEvent != null)
			{
				this.MessageBoxClickedEvent(0);
			}
		}

		// Token: 0x060001AB RID: 427 RVA: 0x00003042 File Offset: 0x00001242
		private void mbButtonTwo_Click(object sender, RoutedEventArgs e)
		{
			if (this.MessageBoxClickedEvent != null)
			{
				this.MessageBoxClickedEvent(1);
			}
		}

		// Token: 0x060001AC RID: 428 RVA: 0x00003058 File Offset: 0x00001258
		private void mbButtonThree_Click(object sender, RoutedEventArgs e)
		{
			if (this.MessageBoxClickedEvent != null)
			{
				this.MessageBoxClickedEvent(2);
			}
		}

		// Token: 0x060001AD RID: 429 RVA: 0x00009534 File Offset: 0x00007734
		private void btnSettings_Click(object sender, RoutedEventArgs e)
		{
			this.TransitionLeft(this.grdSettings);
			string text;
			string text2;
			string text3;
			string text4;
			int num;
			string text5;
			NativeMethods.GetDisplayAdapterDescription(out text, out text2, out text3, out text4, out num, out text5);
			string text6 = this.GetLocalizedMenuString("FEaudiovideo", "SelectedGpu") + " - " + text5;
			this.GPULabel.Content = text6;
		}

		// Token: 0x060001AE RID: 430 RVA: 0x0000306E File Offset: 0x0000126E
		private void btnQuit_Click(object sender, RoutedEventArgs e)
		{
			base.Close();
		}

		// Token: 0x060001AF RID: 431 RVA: 0x00003076 File Offset: 0x00001276
		private void btnGbx_Click(object sender, RoutedEventArgs e)
		{
			Process.Start("http://www.gearboxsoftware.com");
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x00003083 File Offset: 0x00001283
		private void btn2K_Click(object sender, RoutedEventArgs e)
		{
			Process.Start("http://www.2kgames.com");
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x00003090 File Offset: 0x00001290
		private void btnSettings_Back_Click(object sender, RoutedEventArgs e)
		{
			this.TransitionRight(this.grdMain);
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x0000958C File Offset: 0x0000778C
		private void btnConfigSetting_Click(object sender, RoutedEventArgs e)
		{
			FrameworkElement frameworkElement = sender as FrameworkElement;
			Setting setting = ((frameworkElement != null) ? (frameworkElement.DataContext as Setting) : null);
			if (setting != null)
			{
				setting.NextSettingValue(true);
				this.ConditionalRefreshSettingListOnSettingChange(setting, false);
			}
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x000095C4 File Offset: 0x000077C4
		private void btnConfigSettingLeft_Click(object sender, RoutedEventArgs e)
		{
			FrameworkElement frameworkElement = sender as FrameworkElement;
			Setting setting = ((frameworkElement != null) ? (frameworkElement.DataContext as Setting) : null);
			if (setting != null)
			{
				setting.PreviousSettingValue(true);
				this.ConditionalRefreshSettingListOnSettingChange(setting, false);
			}
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x0000958C File Offset: 0x0000778C
		private void btnConfigSettingRight_Click(object sender, MouseButtonEventArgs e)
		{
			FrameworkElement frameworkElement = sender as FrameworkElement;
			Setting setting = ((frameworkElement != null) ? (frameworkElement.DataContext as Setting) : null);
			if (setting != null)
			{
				setting.NextSettingValue(true);
				this.ConditionalRefreshSettingListOnSettingChange(setting, false);
			}
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x000095FC File Offset: 0x000077FC
		private int FindConfigSetting(string sConfigKey)
		{
			foreach (Setting setting in this.ConfigSettings)
			{
				if (setting.ConfigKey == sConfigKey)
				{
					return this.ConfigSettings.IndexOf(setting);
				}
			}
			return -1;
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x0000309E File Offset: 0x0000129E
		private void InitCustomSettingsIndex()
		{
			this.m_nCustomPresetIndex = 0;
			this.m_nCustomConfigSettingIndex = this.FindConfigSetting("GraphicsLevel");
			this.m_nAutodetectGraphicsPresetConfigSettingIndex = this.FindConfigSetting("bAutoDetectSettings");
			this.m_nGraphicsLevelConfigSettingIndex = this.FindConfigSetting("GraphicsLevel");
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x000030DA File Offset: 0x000012DA
		private bool ApplyChangedSetting(Setting setting, int nValue)
		{
			bool flag = setting.SelectedValue == nValue;
			setting.SelectedValue = nValue;
			return flag;
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x00009664 File Offset: 0x00007864
		private bool UpdateChangedGraphicsLevel(Setting context)
		{
			Setting setting = this.ConfigSettings[this.m_nAutodetectGraphicsPresetConfigSettingIndex];
			return this.ApplyChangedSetting(setting, 1);
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x0000968C File Offset: 0x0000788C
		private bool UpdateChangedPreset(Setting context)
		{
			if (this.m_configKeys.ContainsKey(context.ConfigKey) && context.ConfigKey != "GraphicsLevel")
			{
				Setting setting = this.ConfigSettings[this.m_nCustomConfigSettingIndex];
				return this.ApplyChangedSetting(setting, this.m_nCustomPresetIndex) | this.UpdateChangedGraphicsLevel(context);
			}
			return false;
		}

		// Token: 0x060001BA RID: 442 RVA: 0x000096E8 File Offset: 0x000078E8
		private bool UpdatePresetSetting(Setting setting, Section section, Dictionary<string, int> defaultValueParamMap)
		{
			Parameter parameter = section.GetParameter(setting.ConfigKey);
			if (parameter != null)
			{
				int num = defaultValueParamMap[parameter.Value];
				setting.SelectedValue = num;
				return true;
			}
			return false;
		}

		// Token: 0x060001BB RID: 443 RVA: 0x0000971C File Offset: 0x0000791C
		private Setting FindSetting(string sSettingConfigKey)
		{
			if (this.ConfigSettings != null)
			{
				foreach (Setting setting in this.ConfigSettings)
				{
					if (setting.ConfigKey.Equals(sSettingConfigKey, StringComparison.InvariantCultureIgnoreCase))
					{
						return setting;
					}
				}
			}
			return null;
		}

		// Token: 0x060001BC RID: 444 RVA: 0x00009780 File Offset: 0x00007980
		public bool IsAutoDetectEnabled()
		{
			Setting setting = this.FindSetting("bAutoDetectSettings");
			return setting != null && setting.SelectedValue == 0;
		}

		// Token: 0x060001BD RID: 445 RVA: 0x000097A8 File Offset: 0x000079A8
		private MainWindow.GraphicsPresetType GetDefaultAutodetectedPreset()
		{
			int num;
			if (this.m_defaultEngineIni != null && int.TryParse(this.m_defaultEngineIni.GetValue("SystemSettings", "AutodetectedPreset", "1"), out num))
			{
				num = Math.Max(0, Math.Min(4, num));
				return (MainWindow.GraphicsPresetType)num;
			}
			string text = "Can't find DefaultEngine.ini[SystemSettings]. Using eLow.AutodetectedPreset";
			MainWindow.LogException(new Exception(text), text);
			return MainWindow.GraphicsPresetType.eLow;
		}

		// Token: 0x060001BE RID: 446 RVA: 0x00009804 File Offset: 0x00007A04
		private void SetGraphicsPreset(MainWindow.GraphicsPresetType graphicsPreset)
		{
			Setting setting = this.FindSetting("GraphicsLevel");
			if (setting != null)
			{
				setting.SelectedValue = (int)graphicsPreset;
				this.ApplyGraphicsPreset(setting);
				MainWindow.LogMessage("SetGraphicsPreset(" + graphicsPreset + ")", false);
				if (graphicsPreset == MainWindow.GraphicsPresetType.eLow && this.IsAutoDetectEnabled() && this.m_willowEngineIni != null && this.m_willowEngineIni.GetValue("SystemSettings", "AutodetectedUseLowRes", "False").Equals("True", StringComparison.InvariantCultureIgnoreCase))
				{
					Setting setting2 = this.FindSetting("WindowMode");
					if (setting2 != null)
					{
						string text = this.MaxLowResWidth.ToString();
						string text2 = this.MaxLowResHeight.ToString();
						int num;
						int num2;
						if (int.TryParse(text, out num) && int.TryParse(text2, out num2))
						{
							Setting setting3 = this.FindSetting("Resolution");
							if (setting3 != null)
							{
								setting2.SelectedValue = 1;
								int num3 = ResolutionHelper.FindClosestResolution(num, num2);
								setting3.SelectedValue = num3;
								this.m_willowEngineIni.SetValue("SystemSettings", "ResX", text);
								this.m_willowEngineIni.SetValue("SystemSettings", "ResY", text2);
								this.m_willowEngineIni.Save(null, false);
								MainWindow.LogMessage("   --> SetGraphicsPreset(): Applying autodetected low resolution windowed mode: " + text + "x" + text2, false);
							}
						}
					}
				}
				this.ApplySettingsToIniFiles();
			}
		}

		// Token: 0x060001BF RID: 447 RVA: 0x00009960 File Offset: 0x00007B60
		private bool UpdateDisplayResolution(Setting setting, string sSectionName, Section graphicsPresetSection)
		{
			bool flag = false;
			if (this.m_graphicsPresetsIni.HasSection(sSectionName))
			{
				Parameter parameter = graphicsPresetSection.GetParameter("ResX");
				Parameter parameter2 = graphicsPresetSection.GetParameter("ResY");
				if (parameter != null && parameter2 != null)
				{
					int num = int.Parse(parameter.Value);
					int num2 = int.Parse(parameter2.Value);
					string sDisplayResolution = ResolutionHelper.SupportedResolution.FormatResolutionName(num, num2);
					Setting.ModifiableKeyValuePair<string, string> modifiableKeyValuePair = setting.PossibleValues.Find((Setting.ModifiableKeyValuePair<string, string> x) => x.Value == sDisplayResolution);
					if (modifiableKeyValuePair == null)
					{
						modifiableKeyValuePair = setting.PossibleValues.Find((Setting.ModifiableKeyValuePair<string, string> x) => x.Value == setting.DisplayValue);
					}
					if (modifiableKeyValuePair != null)
					{
						int num3 = ((modifiableKeyValuePair != null) ? setting.PossibleValues.IndexOf(modifiableKeyValuePair) : (-1));
						if (num3 != -1)
						{
							setting.SelectedValue = num3;
							flag = true;
						}
					}
					if (!flag)
					{
						string text = "Resolution " + sDisplayResolution + " is not supported on the host display. Need to search for next closest resolution.";
						MainWindow.LogException(new Exception(text), text);
					}
				}
			}
			return flag;
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x00009A80 File Offset: 0x00007C80
		// FIX: Apply preset bucket values for anisotropy and copy the preset's shadow resolution.
		private bool ApplyGraphicsPreset(Setting context)
		{
			bool flag = false;
			MainWindow.LogMessage("ApplyGraphicsPreset(): context=" + context, false);
			if (context.LocValuesIndex != 0)
			{
				string text = "AppCompatBucket" + context.ConfigValue;
				if (this.m_graphicsPresetsIni.HasSection(text))
				{
					Section section = this.m_graphicsPresetsIni.GetSection(text);
					if (context.ConfigKey == "GraphicsLevel")
					{
						this.m_willowEngineIni.SetValue("SystemSettings", "MaxShadowResolution", section.GetParameter("MaxShadowResolution").Value);
						flag = true;
					}
					foreach (Setting setting in this.ConfigSettings)
					{
						if (this.m_configKeys.ContainsKey(setting.ConfigKey))
						{
							MainWindow.GraphicsOptionType graphicsOptionType = this.m_configKeys[setting.ConfigKey];
							MainWindow.LogMessage(string.Concat(new object[] { "ApplyGraphicsPreset(): setting.ConfigKey=", setting.ConfigKey, ", graphicsOption=", graphicsOptionType }), false);
							switch (graphicsOptionType)
							{
							case MainWindow.GraphicsOptionType.eResX:
								flag |= this.UpdateDisplayResolution(setting, text, section);
								break;
							case MainWindow.GraphicsOptionType.eAmbientOcclusion:
							case MainWindow.GraphicsOptionType.eBloom:
							case MainWindow.GraphicsOptionType.eDepthOfField:
							case MainWindow.GraphicsOptionType.eAnisotropicFiltering:
							case MainWindow.GraphicsOptionType.eFoliageQuality:
							case MainWindow.GraphicsOptionType.eTextureQuality:
							case MainWindow.GraphicsOptionType.eGameDetail:
							case MainWindow.GraphicsOptionType.bEnablePSSMShadows:
							case MainWindow.GraphicsOptionType.eDynamicDecals:
							case MainWindow.GraphicsOptionType.eFlareOuts:
							{
								Dictionary<string, int> dictionary = (this.m_defaultValueToIndex.ContainsKey(graphicsOptionType) ? this.m_defaultValueToIndex[graphicsOptionType] : null);
								flag |= this.UpdatePresetSetting(setting, section, dictionary);
								break;
							}
							}
						}
					}
				}
			}
			if (flag)
			{
				this.ApplySettingsToIniFiles();
			}
			return flag;
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x00009C34 File Offset: 0x00007E34
		private void ConditionalRefreshSettingListOnSettingChange(Setting context, bool bApplyingAutodetectedGraphicsLevel = false)
		{
			bool flag = false;
			if (context != null)
			{
				flag = this.UpdateChangedPreset(context);
				if (context.ConfigKey == "GraphicsLevel")
				{
					if (!bApplyingAutodetectedGraphicsLevel)
					{
						flag |= this.UpdateChangedGraphicsLevel(context);
					}
					flag |= this.ApplyGraphicsPreset(context);
				}
				if (context.ConfigKey == "bAutoDetectSettings")
				{
					flag = true;
					MainWindow.LogMessage(string.Concat(new object[] { "context: key=", context.ConfigKey, ", value=", context.SelectedValue }), true);
					if (context.SelectedValue == 0)
					{
						this.SetAutodetectedGraphicsPreset(this.m_autoDetectedGraphicsPreset);
					}
				}
			}
			if (context == null || context.SettingType == ESettingType.AutoDetect || flag)
			{
				this.RefreshSettings();
			}
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x00009CF4 File Offset: 0x00007EF4
		private void RefreshSettings()
		{
			ObservableCollection<Setting> configSettings = this.ConfigSettings;
			this.ConfigSettings = null;
			this.ConfigSettings = configSettings;
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x000030EC File Offset: 0x000012EC
		private void EatMouseDown_Click(object sender, MouseButtonEventArgs e)
		{
			this.ButtonClickSound.Play();
			e.Handled = true;
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x00009D18 File Offset: 0x00007F18
		private void btnSettings_ResetDefaults_Click(object sender, RoutedEventArgs e)
		{
			MainWindow.GraphicsPresetType autodetectedGraphicsPreset = this.GetAutodetectedGraphicsPreset();
			MainWindow.LogMessage("btnSettings_ResetDefaults_Click: save graphics preset before reset to default config: autodetectedGraphicsPreset=" + autodetectedGraphicsPreset, false);
			this.LoadGameDefaultAndUserConfigSettings(true);
			this.ConditionalRefreshSettingListOnSettingChange(null, false);
			MainWindow.LogMessage("btnSettings_ResetDefaults_Click: Re-apply autodetectedGraphicsPreset=" + autodetectedGraphicsPreset, false);
			this.SetAutodetectedGraphicsPreset(autodetectedGraphicsPreset);
			this.ShowSplashText("SettingsReset_Text");
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x00003100 File Offset: 0x00001300
		private bool IsIniFileForSuffix(IniFile ini, string suffix)
		{
			return Path.GetFileName(ini.FileName).Equals(string.Format("Willow{0}.ini", suffix));
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x0000311D File Offset: 0x0000131D
		private bool IsInstalledIniFileForSuffix(IniFile ini, string suffix)
		{
			return Path.GetFileName(ini.FileName).Equals(string.Format("Default{0}.ini", suffix));
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x00009D7C File Offset: 0x00007F7C
		private void ApplySettingsToIniFiles()
		{
			try
			{
				MainWindow.LogMessage("ApplySettings()", false);
				foreach (Setting setting in this.ConfigSettings)
				{
					IniFile iniFile = this.GetIniFile(setting.ConfigFileSuffix);
					if (iniFile == null)
					{
						MainWindow.LogException(new Exception(this.GetResourceString("SettingsSaveFailedConfigFileLoadError_Text")), "btnSettings_Apply_Click(): Can't find IniFile: suffix=" + setting.ConfigFileSuffix);
					}
					setting.SaveToConfig(iniFile);
				}
				foreach (IniFile iniFile2 in this.IniFiles)
				{
					if (!iniFile2.FileName.Contains("Default"))
					{
						iniFile2.Save(null, false);
					}
				}
			}
			catch (Exception ex)
			{
				this.MessageBoxClickedEvent += this.FailedSaveSettings_Click;
				this.DisplayOkBox(ex.Message);
			}
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x0000313A File Offset: 0x0000133A
		private void btnSettings_Apply_Click(object sender, RoutedEventArgs e)
		{
			MainWindow.LogMessage("btnSettings_Apply_Click", false);
			this.ApplySettingsToIniFiles();
			if (sender != null)
			{
				this.ShowSplashText("SettingsApplied_Text");
			}
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x0000315B File Offset: 0x0000135B
		private void FailedSaveSettings_Click(int ButtonIdx)
		{
			this.TransitionRight(this.grdSettings);
		}

		// Token: 0x060001CA RID: 458 RVA: 0x00009EAC File Offset: 0x000080AC
		private void btnDynamicData_Click(object sender, MouseButtonEventArgs e)
		{
			FrameworkElement frameworkElement = sender as FrameworkElement;
			object obj = ((frameworkElement != null) ? frameworkElement.DataContext : null);
			if (obj is NewsStory)
			{
				Process.Start(((NewsStory)obj).Url.ToString());
			}
		}

		// Token: 0x060001CB RID: 459 RVA: 0x00003169 File Offset: 0x00001369
		private void btnMinimize_Click(object sender, RoutedEventArgs e)
		{
			base.WindowState = WindowState.Minimized;
		}

		// Token: 0x060001CC RID: 460 RVA: 0x00003172 File Offset: 0x00001372
		private void btnHelp_Click(object sender, RoutedEventArgs e)
		{
			this.TransitionRight(this.grdHelp);
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00003180 File Offset: 0x00001380
		private void btnHelpBack_Click(object sender, RoutedEventArgs e)
		{
			this.TransitionLeft(this.grdMain);
		}

		// Token: 0x060001CE RID: 462 RVA: 0x00009EEC File Offset: 0x000080EC
		private void btnTechSupportForums_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Process.Start("https://forums.gearboxsoftware.com/");
			}
			catch (Exception ex)
			{
				MainWindow.LogMessage("btnTechSupportForums_Click(): exception=" + ex.Message, false);
			}
		}

		// Token: 0x060001CF RID: 463 RVA: 0x00009F30 File Offset: 0x00008130
		private void btnTechSupport2K_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Process.Start("http://support.2k.com/categories/11696-borderlands");
			}
			catch (Exception ex)
			{
				MainWindow.LogMessage("btnTechSupport2K_Click(): exception=" + ex.Message, false);
			}
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x00009F74 File Offset: 0x00008174
		private void btnReadme_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Process.Start(this.ReadMePath);
			}
			catch (Exception ex)
			{
				MainWindow.LogMessage("btnReadme_Click(): exception=" + ex.Message, false);
			}
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x0000318E File Offset: 0x0000138E
		private void SpeakerButton_Click(object sender, RoutedEventArgs e)
		{
			this.SetMusicVolume();
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x00003196 File Offset: 0x00001396
		private void SetMusicVolume()
		{
			this.MusicPlayer.Volume = (double)(this.IsMusicMuted ? 0f : 0.1f);
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060001D3 RID: 467 RVA: 0x000031B8 File Offset: 0x000013B8
		public bool IsMusicMuted
		{
			get
			{
				return this.MySettings != null && this.MySettings.bMuteAudio;
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060001D4 RID: 468 RVA: 0x000031CF File Offset: 0x000013CF
		public bool IsSfxMuted
		{
			get
			{
				return this.IsMusicMuted;
			}
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x00009FB8 File Offset: 0x000081B8
		// FIX: Avoid rewriting the user's config on ordinary launcher startup.
		private void wndMain_Loaded(object sender, RoutedEventArgs e)
		{
			checked
			{
				try
				{
					Version version = Assembly.GetEntryAssembly().GetName().Version;
					DateTime dateTime = new DateTime(2000, 1, 1).Add(new TimeSpan(864000000000L * unchecked((long)version.Build) + 20000000L * unchecked((long)version.Revision)));
					MainWindow.LogMessage(string.Concat(new object[] { "version=", version, ", buildDateTime=", dateTime }), true);
					MainWindow.Verbose = true;
					MainWindow.VerboseCurrentIndentIndex = 0;
					this.FindStoryboards();
					this.LoadLauncherSettings();
					this.StartMusic();
					this.ParseLocalization();
					if (!NativeMethods.IsLoggedIntoSteam)
					{
						this.MessageBoxClickedEvent += this.OkBoxClicked_CloseApp;
						string localizedMenuString = this.GetLocalizedMenuString("MiscText", "SteamLauncherFail");
						this.DisplayOkBox(localizedMenuString);
					}
					else
					{
						this.InitializeBaselineIniFiles();
						this.ReadLauncherDefaultConfigSettings();
						this.LoadGameDefaultAndUserConfigSettings(false);
						this.LoadGraphicsPresets();
						bool flag = this.IsAutoDetectEnabled();
						string localizedErrorMessage;
						if (!this.FindGraphicsCardPreset(out this.m_autoDetectedGraphicsPreset, out localizedErrorMessage))
						{
							if (flag)
							{
								this.SetGraphicsPreset(this.m_autoDetectedGraphicsPreset);
								MainWindow.LogMessage(localizedErrorMessage, false);
								localizedErrorMessage = this.GetLocalizedErrorMessage("GeneralBelowMinSpec");
								MainWindow.LogMessage(localizedErrorMessage, false);
								this.MessageBoxClickedEvent += this.OkBoxClicked_MinSpec;
								this.DisplayOkBox(localizedErrorMessage);
							}
						}
						else if (flag)
						{
							string localizedMenuString2 = this.GetLocalizedMenuString("dlgAutodetectGraphicsPreset", "Prompt");
							this.MessageBoxClickedEvent += this.OkBoxClicked_MinSpec;
							this.DisplayOkBox(localizedMenuString2);
						}
						if (flag)
						{
							this.SetGraphicsPreset(this.m_autoDetectedGraphicsPreset);
						}
						this.InitCustomSettingsIndex();
						if (flag)
						{
							Setting setting = this.ConfigSettings[this.m_nAutodetectGraphicsPresetConfigSettingIndex];
							this.ApplyChangedSetting(setting, 1);
						}
						this.RefreshSettings();
						this.TryAutoLaunchGame();
						try
						{
							this.gamepadHandler = new GamepadHandler(this.focusedGrid);
							this.gamepadHandler.NewButtonFocusedEvents += this.gamepadHandler_NewButtonFocusedEvents;
							ThreadPool.QueueUserWorkItem(new WaitCallback(this.gamepadHandler.TickGamepads));
						}
						catch (Exception ex)
						{
							MainWindow.LogMessage("wndMain_Loaded(): exception=" + ex.Message, false);
						}
					}
				}
				catch (Exception ex2)
				{
					this.MessageBoxClickedEvent = new MainWindow.MessageBoxButtonClicked(this.OkBoxClicked_CloseApp);
					string localizedErrorMessage2 = this.GetLocalizedErrorMessage("AppStartupError");
					MainWindow.LogMessage(localizedErrorMessage2, false);
					this.DisplayOkBox(localizedErrorMessage2);
					MainWindow.LogMessage("wndMain_Loaded(): exception=" + ex2.Message, false);
				}
			}
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x000031D7 File Offset: 0x000013D7
		private void wndMain_Closed(object sender, EventArgs e)
		{
			if (this.MySettings != null)
			{
				this.MySettings.Save();
			}
			MainWindow.LogClose();
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x000031F1 File Offset: 0x000013F1
		private void LoadLauncherSettings()
		{
			this.MySettings = LauncherSettings.Load("LauncherSettings.w2l");
			if (this.MySettings == null)
			{
				this.MySettings = new LauncherSettings("LauncherSettings.w2l");
			}
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x0000321B File Offset: 0x0000141B
		private void StartMusic()
		{
			this.MusicPlayer.Open(new Uri("Audio/Music.mp3", UriKind.Relative));
			this.MusicPlayer.MediaEnded += this.MusicPlayer_MediaEnded;
			this.SetMusicVolume();
			this.MusicPlayer.Play();
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x0000325B File Offset: 0x0000145B
		private void MusicPlayer_MediaEnded(object sender, EventArgs e)
		{
			this.MusicPlayer.Position = TimeSpan.Zero;
			this.MusicPlayer.Play();
		}

		// Token: 0x060001DA RID: 474 RVA: 0x0000A264 File Offset: 0x00008464
		private void gamepadHandler_NewButtonFocusedEvents(Button btn)
		{
			if (btn.DataContext != null && btn.DataContext is Setting)
			{
				double y = btn.TranslatePoint(default(Point), this.svSettings).Y;
				double num = y + btn.ActualHeight;
				if (num > this.svSettings.ActualHeight)
				{
					this.svSettings.ScrollToVerticalOffset(this.svSettings.VerticalOffset + (num - this.svSettings.ActualHeight));
					return;
				}
				double num2 = y;
				if (num2 < 0.0)
				{
					this.svSettings.ScrollToVerticalOffset(this.svSettings.VerticalOffset + num2);
				}
			}
		}

		// Token: 0x060001DB RID: 475 RVA: 0x00003278 File Offset: 0x00001478
		private void InitDynamicContent()
		{
			this.DynamicData.NewBackgroundReceivedEvents += this.DynamicData_NewBackgroundReceivedEvents;
			this.DynamicData.InitDynamicContent();
		}

		// Token: 0x060001DC RID: 476 RVA: 0x0000329C File Offset: 0x0000149C
		private void DynamicData_NewBackgroundReceivedEvents(object sender, EventArgs e)
		{
			if (!(sender is BitmapImage))
			{
				return;
			}
			this.imgDownloaded.Source = sender as BitmapImage;
			this.imgDownloaded.Visibility = Visibility.Visible;
			this.fadeIn.Begin(this.imgDownloaded);
		}

		// Token: 0x060001DD RID: 477 RVA: 0x0000A30C File Offset: 0x0000850C
		private void ParseLocalization()
		{
			MainWindow.ToggleOptionsLocFile = IniFile.Parse(Path.Combine(MainWindow.InstalledLocPath, string.Format(this.LocFilePattern, MainWindow.ThreeLetterLanguageCode, "Game")));
			MainWindow.MenuNamesLocFile = IniFile.Parse(Path.Combine(MainWindow.InstalledLocPath, string.Format(this.LocFilePattern, MainWindow.ThreeLetterLanguageCode, "Menu")));
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060001DE RID: 478 RVA: 0x0000A36C File Offset: 0x0000856C
		public static string ThreeLetterLanguageCode
		{
			get
			{
				if (string.IsNullOrEmpty(MainWindow.m_sThreeLetterLanguageCode))
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>
					{
						{ "english", "INT" },
						{ "german", "DEU" },
						{ "french", "FRA" },
						{ "italian", "ITA" },
						{ "spanish", "ESN" },
						{ "japanese", "JPN" }
					};
					string steamAppsCurrentGameLanguage = NativeMethods.SteamAppsCurrentGameLanguage;
					if (dictionary.ContainsKey(steamAppsCurrentGameLanguage))
					{
						MainWindow.m_sThreeLetterLanguageCode = dictionary[steamAppsCurrentGameLanguage];
					}
					else
					{
						MainWindow.m_sThreeLetterLanguageCode = "INT";
					}
					MainWindow.LogMessage("sSteamAppsCurrentGameLanguage=" + steamAppsCurrentGameLanguage + " --> ThreeLetterLanguageCode = " + MainWindow.m_sThreeLetterLanguageCode, false);
				}
				return MainWindow.m_sThreeLetterLanguageCode;
			}
		}

		// Token: 0x060001DF RID: 479 RVA: 0x0000A438 File Offset: 0x00008638
		private void LoadGameDefaultAndUserConfigSettings(bool bInstalledIniOnly = false)
		{
			try
			{
				this.GetResourceString("ConfigFileLoadError_Text");
				using (IEnumerator<Setting> enumerator = this.ConfigSettings.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Setting setting = enumerator.Current;
						IniFile iniFile = null;
						bool flag;
						if (!this.IniFiles.Exists((IniFile x) => this.IsIniFileForSuffix(x, setting.ConfigFileSuffix)))
						{
							flag = this.InitWillowIni(setting.ConfigFileSuffix, ref iniFile, null);
							this.IniFiles.Add(iniFile);
						}
						else
						{
							iniFile = this.GetIniFile(setting.ConfigFileSuffix);
							flag = true;
						}
						string installedConfigFullPathForSuffix = this.GetInstalledConfigFullPathForSuffix(setting.ConfigFileSuffix);
						if (!this.InstalledIniFiles.Exists((IniFile x) => this.IsInstalledIniFileForSuffix(x, setting.ConfigFileSuffix)))
						{
							if (File.Exists(installedConfigFullPathForSuffix))
							{
								this.InstalledIniFiles.Add(IniFile.Parse(installedConfigFullPathForSuffix));
							}
							else
							{
								MainWindow.LogException(new Exception(this.GetResourceString("ConfigFileLoadError_Text")), "Unable to load installed INI file " + Path.GetFullPath(installedConfigFullPathForSuffix) + ".");
							}
						}
						IniFile iniFile2;
						if (bInstalledIniOnly || !flag)
						{
							iniFile2 = this.InstalledIniFiles.Find((IniFile y) => this.IsInstalledIniFileForSuffix(y, setting.ConfigFileSuffix));
						}
						else
						{
							iniFile2 = this.IniFiles.Find((IniFile y) => this.IsIniFileForSuffix(y, setting.ConfigFileSuffix));
						}
						setting.SetDefaultValueFromIni(iniFile2);
						if (!bInstalledIniOnly && flag)
						{
							setting.SetDefaultValueFromIni(iniFile);
						}
					}
				}
				this.UpdateToggleKeySetting("DisableLauncher", MainWindow.ConfigBool.eFalse);
				this.UpdateToggleKeySetting("DisableWindowsKey", MainWindow.ConfigBool.eFalse);
			}
			catch (Exception ex)
			{
				this.MessageBoxClickedEvent += this.OkBoxClicked_CloseApp;
				this.DisplayOkBox(ex.Message);
			}
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x0000A62C File Offset: 0x0000882C
		private bool ReadLauncherDefaultConfigSettings()
		{
			bool flag;
			try
			{
				using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Launcher.ConfigData.Settings.w2l"))
				{
					using (GZipStream gzipStream = new GZipStream(manifestResourceStream, CompressionMode.Decompress))
					{
						this.ConfigSettings = Serializer.Deserialize<ObservableCollection<Setting>>(gzipStream);
						this.InitializePossibleValueSettings();
						MainWindow.LogMessage("ReadLauncherDefaultConfigSettings(): location=" + AppDomain.CurrentDomain.BaseDirectory + ", deserialized ConfigSettingsManifestResourceName=Launcher.ConfigData.Settings.w2l", false);
						flag = true;
					}
				}
			}
			catch (Exception ex)
			{
				MainWindow.LogMessage("--------------------\nReadLauncherDefaultConfigSettings: Exception: " + ex.Message + "\n--------------------", false);
				flag = false;
			}
			return flag;
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x000032D5 File Offset: 0x000014D5
		private static string GetGameName()
		{
			return IniFile.Parse(MainWindow.InstalledConfigPath + "\\DefaultEngine.ini").GetValue("Windows.StandardUser", "MyDocumentsSubDirName", "Borderlands");
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000A6E4 File Offset: 0x000088E4
		private void LoadGraphicsPresets()
		{
			string text;
			string text2;
			string graphicsPresetsPathname = MainWindow.GetGraphicsPresetsPathname(out text, out text2, "Compat");
			this.m_graphicsPresetsIni = IniFile.Parse(text);
			checked
			{
				if (this.m_graphicsPresetsIni.HasSection("Configuration"))
				{
					string value = this.m_graphicsPresetsIni.GetSection("Configuration").GetParameter("BasedOn").Value;
					int num = value.LastIndexOf('\\');
					string text3 = value.Remove(0, num + 1);
					num = text2.LastIndexOf('\\');
					IniFile iniFile = IniFile.Parse(text2.Remove(num + 1, text2.Length - num - 1) + text3);
					this.m_graphicsPresetsIni.OverlayIni(iniFile, false);
				}
				IniFile iniFile2 = IniFile.Parse(graphicsPresetsPathname);
				if (iniFile2 != null)
				{
					this.m_graphicsPresetsIni.OverlayIni(iniFile2, false);
				}
				string text4 = this.ExtractPath(graphicsPresetsPathname);
				this.EnsurePathExists(text4);
				this.m_graphicsPresetsIni.Save(graphicsPresetsPathname, true);
			}
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x000032FF File Offset: 0x000014FF
		private string GetLocalizedErrorMessage(string sErrorMessageKey)
		{
			return MainWindow.MenuNamesLocFile["FEBelowMinSpec"][sErrorMessageKey].Trim(new char[] { '"' });
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x00003326 File Offset: 0x00001526
		private string GetLocalizedMenuString(string sConfigSection, string sConfigKey)
		{
			return MainWindow.MenuNamesLocFile[sConfigSection][sConfigKey];
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0000A7C4 File Offset: 0x000089C4
		private MainWindow.GraphicsPresetType GetAutodetectedGraphicsPreset()
		{
			MainWindow.GraphicsPresetType defaultAutodetectedPreset = this.GetDefaultAutodetectedPreset();
			int num = (int)defaultAutodetectedPreset;
			string text = num.ToString();
			int num2;
			if (int.TryParse(this.m_willowEngineIni.GetValue("SystemSettings", "AutodetectedPreset", text), out num2))
			{
				return (MainWindow.GraphicsPresetType)num2;
			}
			return defaultAutodetectedPreset;
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000A804 File Offset: 0x00008A04
		private void SetAutodetectedGraphicsPreset(MainWindow.GraphicsPresetType graphicsPreset)
		{
			int num = (int)graphicsPreset;
			this.m_willowEngineIni.SetValue("SystemSettings", "AutodetectedPreset", num.ToString());
			Setting setting = this.FindSetting("GraphicsLevel");
			if (setting != null)
			{
				setting.SelectedValue = num;
				this.ApplyGraphicsPreset(setting);
				this.ConditionalRefreshSettingListOnSettingChange(setting, true);
				if (graphicsPreset == MainWindow.GraphicsPresetType.eLow && ResolutionHelper.SupportedResolutions != null)
				{
					this.m_willowEngineIni.SetValue("SystemSettings", "AutodetectedUseLowRes", "True");
					this.m_willowEngineIni.SetValue("SystemSettings", "AutodetectedLowResX", this.MaxLowResWidth.ToString());
					this.m_willowEngineIni.SetValue("SystemSettings", "AutodetectedLowResY", this.MaxLowResHeight.ToString());
					this.m_willowEngineIni.Save(null, false);
				}
			}
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0000A8D0 File Offset: 0x00008AD0
		private bool FindGraphicsCardPreset(out MainWindow.GraphicsPresetType outGraphicsPreset, out string sErrorMsg)
		{
			outGraphicsPreset = MainWindow.GraphicsPresetType.eLow;
			sErrorMsg = null;
			MainWindow.VerboseCurrentIndentIndex = 0;
			if (MainWindow.Verbose)
			{
				MainWindow.LogMessage("FindGraphicsCardPreset() BEGIN ------------------------------------------------------", false);
			}
			MainWindow.VerboseCurrentIndentIndex = 4;
			int num;
			int num2;
			int num3;
			bool flag;
			string operatingSystemVersion = NativeMethods.GetOperatingSystemVersion(out num, out num2, out num3, out flag);
			if (MainWindow.Verbose)
			{
				MainWindow.VerboseCurrentIndentIndex = 1;
				MainWindow.LogMessage(string.Concat(new object[]
				{
					"1. OS Version: Major=",
					num,
					", Minor=",
					num2,
					", Build=",
					num3,
					", Is64BitOS=",
					flag.ToString(),
					", sFullOSVersion=",
					operatingSystemVersion,
					"\n"
				}), true);
				MainWindow.VerboseCurrentIndentIndex = 4;
			}
			if (!flag || num < 6 || (num == 6 && num2 < 1))
			{
				sErrorMsg = this.GetLocalizedErrorMessage("OSBelowMinSpec");
				if (MainWindow.Verbose)
				{
					MainWindow.LogMessage("1. " + sErrorMsg, false);
					MainWindow.VerboseCurrentIndentIndex = 0;
					MainWindow.LogMessage("FindGraphicsCardPreset() END (FAILURE) ----------------------------------------------", false);
				}
				return false;
			}
			LogicalProcessorHelper.CPUInfo cpuInfo = LogicalProcessorHelper.GetCpuInfo();
			bool flag2 = cpuInfo.m_sVendorId.Equals("GenuineIntel");
			bool flag3 = cpuInfo.m_sVendorId.Equals("AuthenticAMD");
			int num4 = (flag2 ? 2930 : (flag3 ? 2600 : 2600));
			int num5 = (flag2 ? 3100 : (flag3 ? 3600 : 3600));
			int num6 = (flag2 ? 3300 : (flag3 ? 4000 : 4000));
			float num7 = 1f + Math.Min(Math.Max(checked(cpuInfo.m_uNumCores - 4U), 0U), 6U) * 0.2f;
			uint num8;
			int systemMemoryGB;
			int num9;
			bool flag5;
			MainWindow.GraphicsPresetType graphicsPresetType;
			MainWindow.GraphicsPresetType graphicsPresetType2;
			checked
			{
				num8 = (uint)(unchecked(cpuInfo.m_uProcessorSpeedMhz * num7));
				if (MainWindow.Verbose)
				{
					MainWindow.VerboseCurrentIndentIndex = 1;
					MainWindow.LogMessage("2. CPUInfo: " + cpuInfo.GetDescription(), true);
					MainWindow.LogMessage(string.Concat(new object[]
					{
						"   bIsIntel=",
						flag2.ToString(),
						", bIsAmd=",
						flag3.ToString(),
						", IntelMinSpeedMhz=",
						2930,
						", AMDMinSpeedMhz=",
						2600,
						", GenericMinSpeedMhz=",
						2600,
						", nMinProcessorSpeedMhz=",
						num4,
						"\n"
					}), true);
					MainWindow.VerboseCurrentIndentIndex = 4;
				}
				if (cpuInfo.m_uNumCores < 4U || unchecked((ulong)num8 < (ulong)((long)num4)))
				{
					sErrorMsg = this.GetLocalizedErrorMessage("CPUMinSpec");
					if (MainWindow.Verbose)
					{
						MainWindow.LogMessage("2. " + sErrorMsg, false);
						MainWindow.VerboseCurrentIndentIndex = 0;
						MainWindow.LogMessage("FindGraphicsCardPreset() END (FAILURE) ----------------------------------------------", false);
					}
					return false;
				}
				systemMemoryGB = NativeMethods.GetSystemMemoryGB();
				if (MainWindow.Verbose)
				{
					MainWindow.VerboseCurrentIndentIndex = 1;
					MainWindow.LogMessage(string.Concat(new object[] { "3. SystemMemoryGB=", systemMemoryGB, ", MinSupportedSystemMemoryGB=", 4, "\n" }), true);
					MainWindow.VerboseCurrentIndentIndex = 4;
				}
				if (systemMemoryGB < 4)
				{
					sErrorMsg = this.GetLocalizedErrorMessage("SystemMemoryMinSpec");
					if (MainWindow.Verbose)
					{
						MainWindow.LogMessage("3. " + sErrorMsg, false);
						MainWindow.VerboseCurrentIndentIndex = 0;
						MainWindow.LogMessage("FindGraphicsCardPreset() END (FAILURE) ----------------------------------------------", false);
					}
					return false;
				}
				string text;
				string text2;
				string text3;
				string text4;
				string text5;
				NativeMethods.GetDisplayAdapterDescription(out text, out text2, out text3, out text4, out num9, out text5);
				string text6 = "AppCompatGPU-" + text2;
				bool flag4 = this.m_graphicsPresetsIni.HasSection(text6.ToUpperInvariant());
				int num10 = -1;
				flag5 = true;
				string text7 = " (src=Win32_VideoController)";
				if (flag4)
				{
					Parameter parameter = this.m_graphicsPresetsIni.GetSection(text6).GetParameter(text4);
					if (parameter != null)
					{
						char[] array = new char[] { ',' };
						string[] array2 = parameter.Value.Split(array);
						num10 = int.Parse(array2[0]);
						text3 = array2[1];
						text7 = " (source=" + this.m_graphicsPresetsIni.FileName + ")";
						if (!this.IsSupportedGraphicsQuality(num10))
						{
							sErrorMsg = this.GetLocalizedErrorMessage("VideoCardMinSpec");
							if (MainWindow.Verbose)
							{
								MainWindow.LogMessage("4. " + sErrorMsg, false);
								MainWindow.VerboseCurrentIndentIndex = 0;
								MainWindow.LogMessage("FindGraphicsCardPreset() END (FAILURE) ----------------------------------------------", false);
							}
							return false;
						}
					}
					else
					{
						flag5 = false;
						sErrorMsg = this.GetLocalizedErrorMessage("VideoCardGpuDeviceIdNotRecognized");
					}
				}
				else
				{
					flag5 = false;
					sErrorMsg = this.GetLocalizedErrorMessage("VideoCardGpuVendorNotRecognized");
				}
				if (MainWindow.Verbose)
				{
					MainWindow.VerboseCurrentIndentIndex = 1;
					MainWindow.LogMessage("4. GPU: Final sVideoCardModelName=" + text3 + text7 + "\n", true);
					MainWindow.VerboseCurrentIndentIndex = 4;
				}
				if (num9 < 2)
				{
					sErrorMsg = this.GetLocalizedErrorMessage("VideoCardRAMMinSpec");
					if (MainWindow.Verbose)
					{
						MainWindow.LogMessage("5. " + sErrorMsg, false);
						MainWindow.VerboseCurrentIndentIndex = 0;
						MainWindow.LogMessage("FindGraphicsCardPreset() END (FAILURE) ----------------------------------------------", false);
					}
					return false;
				}
				if (MainWindow.Verbose)
				{
					MainWindow.VerboseCurrentIndentIndex = 1;
					MainWindow.LogMessage(string.Concat(new object[] { "5. GPU Video Memory (GB): ", num9, ", MinVideoMemoryGB=", 2, "\n" }), true);
					MainWindow.VerboseCurrentIndentIndex = 4;
				}
				string text8;
				int installedDriveFreeSpaceGB = NativeMethods.GetInstalledDriveFreeSpaceGB(out text8);
				if (installedDriveFreeSpaceGB < 25)
				{
					sErrorMsg = this.GetLocalizedErrorMessage("HardDriveMinSpec");
					if (MainWindow.Verbose)
					{
						MainWindow.LogMessage("6. " + sErrorMsg, false);
						MainWindow.VerboseCurrentIndentIndex = 0;
						MainWindow.LogMessage("FindGraphicsCardPreset() END (FAILURE) ----------------------------------------------", false);
					}
					return false;
				}
				if (MainWindow.Verbose)
				{
					MainWindow.VerboseCurrentIndentIndex = 1;
					MainWindow.LogMessage(string.Concat(new object[] { "6. Min. Hard Drive Space (GB): nDiskSpaceAvailableGB=", installedDriveFreeSpaceGB, ", MinHardDriveSpaceGB=", 25, "\n" }), true);
					MainWindow.VerboseCurrentIndentIndex = 4;
				}
				List<ResolutionHelper.SupportedResolution> supportedResolutions = ResolutionHelper.SupportedResolutions;
				bool flag6 = ResolutionHelper.NumSupportedResolutions == 0;
				int count = supportedResolutions.Count;
				int maxLowResWidth = this.MaxLowResWidth;
				int maxLowResHeight = this.MaxLowResHeight;
				int width = supportedResolutions[count - 1].Width;
				int height = supportedResolutions[count - 1].Height;
				bool flag7 = width < maxLowResWidth || height < maxLowResHeight;
				if (flag7 || flag6)
				{
					sErrorMsg = "";
					if (flag7)
					{
						sErrorMsg += this.GetLocalizedErrorMessage("DisplayResolutionMinSpec");
					}
					if (flag6)
					{
						if (flag7)
						{
							sErrorMsg += " ";
						}
						sErrorMsg += this.GetLocalizedErrorMessage("NoSupportedDisplayAspectRatios");
					}
					if (this.IsAutoDetectEnabled())
					{
						this.SetAutodetectedGraphicsPreset(outGraphicsPreset);
					}
					if (MainWindow.Verbose)
					{
						MainWindow.VerboseCurrentIndentIndex = 1;
						MainWindow.LogMessage("7. " + sErrorMsg, false);
						MainWindow.LogMessage(string.Concat(new object[] { "7. Max. supported resolution: ", width, "x", height }), false);
						MainWindow.VerboseCurrentIndentIndex = 0;
						MainWindow.LogMessage("FindGraphicsCardPreset() END (FAILURE) ----------------------------------------------", false);
					}
					return false;
				}
				if (MainWindow.Verbose)
				{
					MainWindow.VerboseCurrentIndentIndex = 1;
					MainWindow.LogMessage(string.Concat(new object[] { "7. Max. Display Resolution: ", width, "x", height }), true);
					MainWindow.LogMessage(string.Concat(new object[] { "   Min. Game Supported Resolution: ", maxLowResWidth, "x", maxLowResHeight, "\n" }), true);
					MainWindow.LogMessage("-------------------------------", true);
					MainWindow.LogMessage("---> MIN. SPEC REACHED!!!! <---", true);
					MainWindow.LogMessage("-------------------------------\n", true);
					MainWindow.VerboseCurrentIndentIndex = 4;
				}
				graphicsPresetType = MainWindow.GraphicsPresetType.eUltra;
				if (flag5 && num10 >= 6)
				{
					graphicsPresetType = this.ConvertGraphicsQuality(num10);
				}
				if (MainWindow.Verbose)
				{
					MainWindow.VerboseCurrentIndentIndex = 1;
					MainWindow.LogMessage(string.Concat(new object[]
					{
						"8a. nGraphicsQuality=",
						num10,
						": MinGraphicsQuality=",
						6,
						" (bGpuFound=",
						flag5.ToString(),
						")\n"
					}), true);
					MainWindow.VerboseCurrentIndentIndex = 4;
				}
				graphicsPresetType2 = MainWindow.GraphicsPresetType.eLow;
			}
			if ((ulong)num8 > (ulong)((long)num4))
			{
				if ((ulong)num8 >= (ulong)((long)num5))
				{
					if ((ulong)num8 >= (ulong)((long)num6))
					{
						graphicsPresetType2 = MainWindow.GraphicsPresetType.eUltra;
					}
					else
					{
						graphicsPresetType2 = MainWindow.GraphicsPresetType.eHigh;
					}
				}
				else
				{
					graphicsPresetType2 = MainWindow.GraphicsPresetType.eMedium;
				}
			}
			if (MainWindow.Verbose)
			{
				MainWindow.VerboseCurrentIndentIndex = 1;
				MainWindow.LogMessage(string.Concat(new object[]
				{
					"8b. cpuInfo.m_uProcessorSpeedMhz=",
					cpuInfo.m_uProcessorSpeedMhz,
					"aggregateProcessorSpeedMhz=",
					num8,
					": nMinProcessorSpeedMhz=",
					num4,
					", nHighProcessorSpeedMhz=",
					num5,
					", nUltraProcessorSpeedMhz=",
					num6,
					", bIsIntel=",
					flag2.ToString(),
					" (IntelMinSpeedMhz=",
					2930,
					"), bIsAmd=",
					flag3.ToString(),
					" (AMDMinSpeedMhz=",
					2600,
					")\n"
				}), true);
				MainWindow.VerboseCurrentIndentIndex = 4;
			}
			MainWindow.GraphicsPresetType graphicsPresetType3 = MainWindow.GraphicsPresetType.eLow;
			if (systemMemoryGB >= 4)
			{
				if (systemMemoryGB >= 8)
				{
					graphicsPresetType3 = MainWindow.GraphicsPresetType.eUltra;
				}
				else
				{
					graphicsPresetType3 = MainWindow.GraphicsPresetType.eHigh;
				}
			}
			if (MainWindow.Verbose)
			{
				MainWindow.VerboseCurrentIndentIndex = 1;
				MainWindow.LogMessage(string.Concat(new object[] { "8c. nSystemMemoryGB=", systemMemoryGB, ": MedHighSupportedSystemMemoryGB=", 4, ", UltraSupportedSystemMemoryGB=", 8, "\n" }), true);
				MainWindow.VerboseCurrentIndentIndex = 4;
			}
			MainWindow.GraphicsPresetType graphicsPresetType4 = MainWindow.GraphicsPresetType.eLow;
			if (num9 >= 2)
			{
				graphicsPresetType4 = MainWindow.GraphicsPresetType.eMedium;
				if (num9 >= 2)
				{
					graphicsPresetType4 = MainWindow.GraphicsPresetType.eHigh;
					if (num9 >= 4)
					{
						graphicsPresetType4 = MainWindow.GraphicsPresetType.eUltra;
					}
				}
			}
			if (MainWindow.Verbose)
			{
				MainWindow.VerboseCurrentIndentIndex = 1;
				MainWindow.LogMessage(string.Concat(new object[] { "8d. nVideoCardMemoryGB=", num9, ": MedSupportedVideoMemoryGB=", 2, ", HighSupportedVideoMemoryGB=", 2, ", UltraSupportedVideoMemoryGB=", 4, "\n" }), true);
				MainWindow.VerboseCurrentIndentIndex = 4;
			}
			if (MainWindow.Verbose)
			{
				MainWindow.VerboseCurrentIndentIndex = 1;
				MainWindow.LogMessage(string.Concat(new object[] { "8e. maxGraphicsPreset=", graphicsPresetType, ", cpuClockSpeedPreset=", graphicsPresetType2, ", systemMemoryPreset=", graphicsPresetType3, ", videoMemoryPreset=", graphicsPresetType4, "\n" }), true);
				MainWindow.VerboseCurrentIndentIndex = 4;
			}
			outGraphicsPreset = (MainWindow.GraphicsPresetType)Math.Min((int)graphicsPresetType2, Math.Min((int)graphicsPresetType3, (int)graphicsPresetType4));
			if (flag5)
			{
				outGraphicsPreset = (MainWindow.GraphicsPresetType)Math.Min((int)graphicsPresetType, (int)outGraphicsPreset);
				MainWindow.LogMessage(string.Concat(new object[]
				{
					"FindGraphicsCardPreset(", outGraphicsPreset, "): <== min(maxGraphicsPreset=", graphicsPresetType, ", cpuClockSpeedPreset=", graphicsPresetType2, ", systemMemoryPreset=", graphicsPresetType3, ", videoMemoryPreset=", graphicsPresetType4,
					")"
				}), false);
			}
			else
			{
				MainWindow.LogMessage(string.Concat(new object[] { "FindGraphicsCardPreset(", outGraphicsPreset, "): <== min(cpuPreset=", graphicsPresetType2, ", systemPreset=", graphicsPresetType3, ", videoMemoryPreset=", graphicsPresetType4, ")" }), false);
			}
			if (MainWindow.Verbose)
			{
				MainWindow.VerboseCurrentIndentIndex = 1;
				MainWindow.LogMessage("9. outGraphicsPreset=" + outGraphicsPreset + "\n", true);
				MainWindow.VerboseCurrentIndentIndex = 4;
				sErrorMsg = this.GetLocalizedErrorMessage("VideoCardRAMMinSpec");
			}
			if (this.IsAutoDetectEnabled())
			{
				this.SetAutodetectedGraphicsPreset(outGraphicsPreset);
			}
			if (MainWindow.Verbose)
			{
				MainWindow.VerboseCurrentIndentIndex = 0;
				MainWindow.LogMessage("FindGraphicsCardPreset() END (SUCCESS) ----------------------------------------------", false);
			}
			return true;
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x0000B4B8 File Offset: 0x000096B8
		private void InitializePossibleValueSettings()
		{
			checked
			{
				foreach (Setting setting in this.ConfigSettings)
				{
					setting.Initialize();
					if (setting.PossibleValues == null)
					{
						MainWindow.LogException(new Exception(this.GetResourceString("ConfigFileLoadError_Text")), "Must set PossibleValues list for each ConfigSetting. Broken setting: " + setting.ConfigKey);
					}
					string text = string.Format("{0}[{1}]", "SystemOptions", setting.LocValuesIndex);
					string text2 = MainWindow.ToggleOptionsLocFile["WillowGFxMenu"][text];
					List<string> list = new List<string>(this.ParseQuotedCommaSeparatedValues(text2));
					if (setting.PossibleValues.Count < list.Count)
					{
						MainWindow.LogException(new Exception(this.GetResourceString("ConfigFileLoadError_Text")), string.Concat(new object[]
						{
							"PossibleValues array doesn't match the entries in the loc file for setting ",
							setting.ConfigKey,
							". Expected ",
							list.Count,
							", found ",
							setting.PossibleValues.Count,
							"."
						}));
					}
					for (int i = 0; i < list.Count; i++)
					{
						int num = i;
						setting.PossibleValues[i].Value = list[num];
					}
				}
			}
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000B638 File Offset: 0x00009838
		private void InitConfigSettings()
		{
			using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Launcher.ConfigData.Settings.w2l"))
			{
				using (GZipStream gzipStream = new GZipStream(manifestResourceStream, CompressionMode.Decompress))
				{
					this.ConfigSettings = Serializer.Deserialize<ObservableCollection<Setting>>(gzipStream);
				}
			}
			this.InitializePossibleValueSettings();
		}

		// Token: 0x060001EA RID: 490 RVA: 0x0000B6A4 File Offset: 0x000098A4
		[Conditional("DEBUG")]
		private void DebugGenerateUncompressedProtobufForSettings<T>(T Settings, string FileName = "..\\..\\..\\Main\\Development\\Launcher\\ConfigData\\DebugOnly_SettingsUncompressedBinaryProtobuf.pb")
		{
			try
			{
				using (FileStream fileStream = File.Create(FileName))
				{
					Serializer.Serialize<T>(fileStream, Settings);
					MainWindow.LogMessage("--------------------\nUncompressed Protobuf Config Settings Written: " + FileName + "\n--------------------", false);
				}
			}
			catch (Exception ex)
			{
				MainWindow.LogMessage("--------------------\nDebugGenerateUncompressedProtobufForSettings: Exception: " + ex.Message + "\n--------------------", false);
			}
		}

		// Token: 0x060001EB RID: 491 RVA: 0x0000B720 File Offset: 0x00009920
		[Conditional("DEBUG")]
		private void DebugGenerateCompressedProtobufForSettings<T>(T Settings, string FileName = "..\\..\\..\\Main\\Development\\Launcher\\ConfigData\\DebugOnly_SettingsCompressedBinaryProtobuf.w2l")
		{
			try
			{
				using (FileStream fileStream = File.Create(FileName))
				{
					using (GZipStream gzipStream = new GZipStream(fileStream, CompressionMode.Compress))
					{
						Serializer.Serialize<T>(gzipStream, Settings);
						MainWindow.LogMessage("--------------------\nCompressed Protobuf Config Settings Written: " + FileName + "\n--------------------", true);
					}
				}
			}
			catch (Exception ex)
			{
				MainWindow.LogMessage("--------------------\nDebugGenerateCompressedProtobufForSettings: Exception: " + ex.Message + "\n--------------------", true);
			}
		}

		// Token: 0x060001EC RID: 492 RVA: 0x0000B7BC File Offset: 0x000099BC
		private void UpdateToggleKeySetting(string sDisableToggleKeyParameterName, MainWindow.ConfigBool defaultToggleValue)
		{
			this.InitDefaultIni("Game", ref this.m_defaultGameIni);
			this.InitWillowIni("Game", ref this.m_willowGameIni, this.m_defaultGameIni);
			string text = MainWindow.m_configBoolLabel[defaultToggleValue];
			string value = this.m_defaultGameIni.GetValue("Engine.GameInfo", sDisableToggleKeyParameterName, text);
			if (!this.m_willowGameIni.HasValue("Engine.GameInfo", sDisableToggleKeyParameterName))
			{
				this.m_willowGameIni.SetValue("Engine.GameInfo", sDisableToggleKeyParameterName, value);
			}
			this.m_willowGameIni.Save(null, false);
		}

		// Token: 0x060001ED RID: 493 RVA: 0x00003339 File Offset: 0x00001539
		public IEnumerable<string> ParseQuotedCommaSeparatedValues(string CsvString)
		{
			bool flag = false;
			bool bForceNextCharInString = false;
			StringBuilder nextRetval = null;
			foreach (char c in CsvString)
			{
				if (flag)
				{
					if (c == '\\')
					{
						bForceNextCharInString = true;
					}
					else if (c != '"' || bForceNextCharInString)
					{
						nextRetval.Append(c);
						bForceNextCharInString = false;
					}
					else if (c == '"')
					{
						yield return nextRetval.ToString();
						flag = false;
					}
				}
				else if (c == '"')
				{
					flag = true;
					nextRetval = new StringBuilder();
				}
			}
			string text = null;
			yield break;
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000B844 File Offset: 0x00009A44
		private void FindStoryboards()
		{
			this.slideLeftOut = base.TryFindResource("SlideLeftOut") as Storyboard;
			this.slideLeftIn = base.TryFindResource("SlideLeftIn") as Storyboard;
			this.slideRightOut = base.TryFindResource("SlideRightOut") as Storyboard;
			this.slideRightIn = base.TryFindResource("SlideRightIn") as Storyboard;
			this.fadeIn = base.TryFindResource("FadeIn") as Storyboard;
			this.SpinAndFadeFromBack = base.TryFindResource("SpinAndFadeFromBack") as Storyboard;
			this.SpinAndFadeThrough = base.TryFindResource("SpinAndFadeThrough") as Storyboard;
			this.SpinAndFadeFromBack.Completed += this.OnSplashAnimationComplete;
		}

		// Token: 0x060001EF RID: 495 RVA: 0x00003349 File Offset: 0x00001549
		private static IEnumerable<T> FindChildrenByType<T>(DependencyObject parent) where T : DependencyObject
		{
			int num;
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i = checked(num + 1))
			{
				DependencyObject child = VisualTreeHelper.GetChild(parent, i);
				if (child is T)
				{
					yield return child as T;
				}
				foreach (T t in MainWindow.FindChildrenByType<T>(child))
				{
					if (t != null)
					{
						yield return t;
					}
				}
				IEnumerator<T> enumerator = null;
				child = null;
				num = i;
			}
			yield break;
			yield break;
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x0000B904 File Offset: 0x00009B04
		public static void UpdateFocusableButtonList(List<Button> buttonList, object elem)
		{
			if (elem is Panel)
			{
				using (IEnumerator enumerator = (elem as Panel).Children.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						if (obj is Button && (obj as Button).Visibility == Visibility.Visible)
						{
							buttonList.Add(obj as Button);
						}
						else
						{
							MainWindow.UpdateFocusableButtonList(buttonList, obj);
						}
					}
					return;
				}
			}
			if (elem is ContentControl)
			{
				if ((elem as ContentControl).Content != null)
				{
					MainWindow.UpdateFocusableButtonList(buttonList, (elem as ContentControl).Content);
					return;
				}
			}
			else if (elem is ItemsControl)
			{
				foreach (Button button in MainWindow.FindChildrenByType<Button>(elem as ItemsControl))
				{
					if (button != null && button.Visibility == Visibility.Visible)
					{
						buttonList.Add(button);
					}
				}
			}
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x0000BA0C File Offset: 0x00009C0C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri uri = new Uri("/Launcher;component/mainwindow.xaml", UriKind.Relative);
			Application.LoadComponent(this, uri);
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x00003359 File Offset: 0x00001559
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x0000BA3C File Offset: 0x00009C3C
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.wndMain = (MainWindow)target;
				this.wndMain.Loaded += this.wndMain_Loaded;
				this.wndMain.Closed += this.wndMain_Closed;
				return;
			case 4:
				((Image)target).MouseLeftButtonDown += this.DragWindow;
				return;
			case 5:
				this.imgDownloaded = (Image)target;
				this.imgDownloaded.MouseLeftButtonDown += this.DragWindow;
				return;
			case 6:
				this.grdMessageBox = (Grid)target;
				return;
			case 7:
				this.mbQuestion = (Label)target;
				return;
			case 8:
				this.mbButtonOne = (Button)target;
				this.mbButtonOne.Click += this.mbButtonOne_Click;
				return;
			case 9:
				this.mbButtonTwo = (Button)target;
				this.mbButtonTwo.Click += this.mbButtonTwo_Click;
				return;
			case 10:
				this.mbButtonThree = (Button)target;
				this.mbButtonThree.Click += this.mbButtonThree_Click;
				return;
			case 11:
				this.grdHelp = (Grid)target;
				return;
			case 12:
				((Label)target).MouseLeftButtonDown += this.DragWindow;
				return;
			case 13:
				((Button)target).Click += this.btnTechSupport2K_Click;
				return;
			case 14:
				((Button)target).Click += this.btnTechSupportForums_Click;
				return;
			case 15:
				((Button)target).Click += this.btnReadme_Click;
				return;
			case 16:
				((Button)target).Click += this.btnHelpBack_Click;
				return;
			case 17:
				this.grdMain = (Grid)target;
				return;
			case 18:
				this.grdNews = (Grid)target;
				return;
			case 19:
				((Label)target).MouseLeftButtonDown += this.DragWindow;
				return;
			case 21:
				((Button)target).Click += this.btnHelp_Click;
				return;
			case 22:
				((Button)target).Click += this.btnSettings_Click;
				return;
			case 23:
				((Button)target).Click += this.btnPlay_Click;
				return;
			case 24:
				this.grdSettings = (Grid)target;
				return;
			case 25:
				((Label)target).MouseLeftButtonDown += this.DragWindow;
				return;
			case 26:
				this.GPULabel = (Label)target;
				this.GPULabel.MouseLeftButtonDown += this.DragWindow;
				return;
			case 27:
				this.svSettings = (CustomScrollViewer)target;
				return;
			case 31:
				((StackPanel)target).MouseLeftButtonDown += this.DragWindow;
				return;
			case 32:
				((Button)target).Click += this.btnSettings_Apply_Click;
				return;
			case 33:
				((Button)target).Click += this.btnSettings_ResetDefaults_Click;
				return;
			case 34:
				((Button)target).Click += this.btnSettings_Back_Click;
				return;
			case 35:
				((Image)target).MouseLeftButtonUp += new MouseButtonEventHandler(this.btn2K_Click);
				return;
			case 36:
				((Image)target).MouseLeftButtonUp += new MouseButtonEventHandler(this.btnGbx_Click);
				return;
			case 37:
				((Button)target).Click += this.btnQuit_Click;
				return;
			case 38:
				((Button)target).Click += this.btnMinimize_Click;
				return;
			case 39:
				((ToggleButton)target).Click += this.SpeakerButton_Click;
				return;
			case 40:
				this.lblOverlayBackground = (Label)target;
				this.lblOverlayBackground.MouseLeftButtonDown += this.DragWindow;
				return;
			case 41:
				this.lblOverlayForeground = (Label)target;
				this.lblOverlayForeground.MouseLeftButtonDown += this.DragWindow;
				return;
			}
			this._contentLoaded = true;
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000BE74 File Offset: 0x0000A074
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId <= 3)
			{
				EventSetter eventSetter;
				if (connectionId == 2)
				{
					eventSetter = new EventSetter();
					eventSetter.Event = UIElement.MouseEnterEvent;
					eventSetter.Handler = new MouseEventHandler(this.Generic_MouseEnter);
					((Style)target).Setters.Add(eventSetter);
					eventSetter = new EventSetter();
					eventSetter.Event = ButtonBase.ClickEvent;
					eventSetter.Handler = new RoutedEventHandler(this.Generic_ButtonClick);
					((Style)target).Setters.Add(eventSetter);
					return;
				}
				if (connectionId != 3)
				{
					return;
				}
				eventSetter = new EventSetter();
				eventSetter.Event = UIElement.MouseEnterEvent;
				eventSetter.Handler = new MouseEventHandler(this.Generic_MouseEnter);
				((Style)target).Setters.Add(eventSetter);
				eventSetter = new EventSetter();
				eventSetter.Event = ButtonBase.ClickEvent;
				eventSetter.Handler = new RoutedEventHandler(this.Generic_ButtonClick);
				((Style)target).Setters.Add(eventSetter);
				return;
			}
			else
			{
				if (connectionId == 20)
				{
					((Grid)target).MouseLeftButtonUp += this.btnDynamicData_Click;
					((Grid)target).PreviewMouseLeftButtonDown += this.EatMouseDown_Click;
					return;
				}
				switch (connectionId)
				{
				case 28:
					((Image)target).PreviewMouseLeftButtonDown += this.EatMouseDown_Click;
					((Image)target).MouseLeftButtonUp += new MouseButtonEventHandler(this.btnConfigSettingLeft_Click);
					return;
				case 29:
					((Button)target).Click += this.btnConfigSetting_Click;
					return;
				case 30:
					((Image)target).PreviewMouseLeftButtonDown += this.EatMouseDown_Click;
					((Image)target).MouseLeftButtonUp += this.btnConfigSettingRight_Click;
					return;
				default:
					return;
				}
			}
		}

		// Token: 0x040000F1 RID: 241
		private static string m_sLauncherVersion;

		// Token: 0x040000F2 RID: 242
		private static MainWindow m_instance;

		// Token: 0x040000F3 RID: 243
		public const bool TestForceAutodetectedLowResWindowHighResDesktop = false;

		// Token: 0x040000F4 RID: 244
		public const int TestForceAutodetectedLowResWindowMaxLowResWidth = 1920;

		// Token: 0x040000F5 RID: 245
		public const int TestForceAutodetectedLowResWindowMaxLowResHeight = 1080;

		// Token: 0x040000F6 RID: 246
		private Storyboard slideLeftOut;

		// Token: 0x040000F7 RID: 247
		private Storyboard slideLeftIn;

		// Token: 0x040000F8 RID: 248
		private Storyboard slideRightOut;

		// Token: 0x040000F9 RID: 249
		private Storyboard slideRightIn;

		// Token: 0x040000FA RID: 250
		private Storyboard fadeIn;

		// Token: 0x040000FB RID: 251
		private Storyboard SpinAndFadeFromBack;

		// Token: 0x040000FC RID: 252
		private Storyboard SpinAndFadeThrough;

		// Token: 0x040000FD RID: 253
		private List<IniFile> IniFiles;

		// Token: 0x040000FE RID: 254
		private List<IniFile> InstalledIniFiles;

		// Token: 0x040000FF RID: 255
		private IniFile m_defaultEngineIni;

		// Token: 0x04000100 RID: 256
		private IniFile m_willowEngineIni;

		// Token: 0x04000101 RID: 257
		private IniFile m_defaultGameIni;

		// Token: 0x04000102 RID: 258
		private IniFile m_willowGameIni;

		// Token: 0x04000103 RID: 259
		private IniFile m_defaultLauncherIni;

		// Token: 0x04000104 RID: 260
		private IniFile m_willowLauncherIni;

		// Token: 0x04000105 RID: 261
		private const string IniBaseGameName = "Game";

		// Token: 0x04000106 RID: 262
		private const string IniBaseEngineName = "Engine";

		// Token: 0x04000107 RID: 263
		private const string IniSuffixDefaultName = "Default";

		// Token: 0x04000108 RID: 264
		private const string IniBaseLauncherName = "Launcher";

		// Token: 0x04000109 RID: 265
		private const string DisableLauncherParameterName = "DisableLauncher";

		// Token: 0x0400010A RID: 266
		private const string DisableWindowsKeyParameterName = "DisableWindowsKey";

		// Token: 0x0400010B RID: 267
		private const string WindowModeParameterName = "WindowMode";

		// Token: 0x0400010C RID: 268
		private const string WindowModeLocKeyName = "Fullscreen";

		// Token: 0x0400010D RID: 269
		private const string GameInfoSectionName = "Engine.GameInfo";

		// Token: 0x0400010E RID: 270
		private const string LauncherInfoSectionName = "LauncherInfo";

		// Token: 0x0400010F RID: 271
		public const string SystemSettingsConfigSection = "SystemSettings";

		// Token: 0x04000110 RID: 272
		private const string LastExeIndexParameterName = "LastExeIndex";

		// Token: 0x04000111 RID: 273
		private const string LogMessageDateTimeFormat = "yyyy.MM.dd HH:mm:ss.fff zzz:  ";

		// Token: 0x04000112 RID: 274
		private Grid focusedGrid;

		// Token: 0x04000113 RID: 275
		private ResourceManager resourceManager;

		// Token: 0x04000114 RID: 276
		private GamepadHandler gamepadHandler;

		// Token: 0x04000115 RID: 277
		private Grid PreMessageBoxGrid;

		// Token: 0x04000116 RID: 278
		private EventHandler FadeOutComplete;

		// Token: 0x04000117 RID: 279
		private readonly MediaPlayer MusicPlayer;

		// Token: 0x04000118 RID: 280
		private readonly SoundEffect ButtonClickSound;

		// Token: 0x04000119 RID: 281
		private readonly SoundEffect WhooshSound;

		// Token: 0x0400011A RID: 282
		private readonly SoundEffect MouseOverSound;

		// Token: 0x0400011B RID: 283
		private readonly SoundEffect AppliedSound;

		// Token: 0x0400011C RID: 284
		private const float MusicVolume = 0.1f;

		// Token: 0x0400011D RID: 285
		private LauncherSettings _mySettings;

		// Token: 0x0400011F RID: 287
		public static IniFile ToggleOptionsLocFile;

		// Token: 0x04000120 RID: 288
		public static IniFile MenuNamesLocFile;

		// Token: 0x04000122 RID: 290
		private static bool m_bVerboseMsg = false;

		// Token: 0x04000123 RID: 291
		private static int m_nVerboseCurrIndent = 0;

		// Token: 0x04000124 RID: 292
		private static string[] m_sVerboseIndents = new string[] { "", " ", "  ", "  ", "    ", "     ", "      ", "       ", "        " };

		// Token: 0x04000125 RID: 293
		private ObservableCollection<Setting> _configSettings;

		// Token: 0x04000126 RID: 294
		private static StreamWriter m_launcherLog;

		// Token: 0x04000127 RID: 295
		private static int m_nLauncherLogSessionLineNum = 0;

		// Token: 0x04000129 RID: 297
		private const int MessageBoxYesIndex = 0;

		// Token: 0x0400012A RID: 298
		private const int MessageBoxNoIndex = 1;

		// Token: 0x0400012B RID: 299
		private const int MessageBoxCancelIndex = 2;

		// Token: 0x0400012C RID: 300
		private const string ConfigDataPath = "..\\..\\..\\Main\\Development\\Launcher\\ConfigData\\";

		// Token: 0x0400012D RID: 301
		private const string ConfigSettingsFilename = "Settings.w2l";

		// Token: 0x0400012E RID: 302
		private const string ConfigSettingsPathname = "..\\..\\..\\Main\\Development\\Launcher\\ConfigData\\Settings.w2l";

		// Token: 0x0400012F RID: 303
		private const string ConfigSettingsManifestResourceName = "Launcher.ConfigData.Settings.w2l";

		// Token: 0x04000130 RID: 304
		private const string DefaultCompressedConfigSettingsFilename = "..\\..\\..\\Main\\Development\\Launcher\\ConfigData\\DebugOnly_SettingsCompressedBinaryProtobuf.w2l";

		// Token: 0x04000131 RID: 305
		private const string DefaultUncompressedConfigSettingsFilename = "..\\..\\..\\Main\\Development\\Launcher\\ConfigData\\DebugOnly_SettingsUncompressedBinaryProtobuf.pb";

		// Token: 0x04000132 RID: 306
		private const string SelectedGpuConfigKey = "SelectedGpu";

		// Token: 0x04000133 RID: 307
		private const string SelectedGpuLocKey = "SelectedGpu";

		// Token: 0x04000134 RID: 308
		public const string ToggleOptionsLocSub = "Game";

		// Token: 0x04000135 RID: 309
		public const string ToggleOptionsLocSection = "WillowGFxMenu";

		// Token: 0x04000136 RID: 310
		public const string ToggleOptionsLocArray = "SystemOptions";

		// Token: 0x04000137 RID: 311
		public const string MenuNamesLocSub = "Menu";

		// Token: 0x04000138 RID: 312
		public const string MenuNamesLocSection = "FEaudiovideo";

		// Token: 0x04000139 RID: 313
		public const string MenuMinSpecErrorLocSection = "FEBelowMinSpec";

		// Token: 0x0400013A RID: 314
		public const string MiscTextLocSection = "MiscText";

		// Token: 0x0400013B RID: 315
		private static string m_sGameName;

		// Token: 0x0400013C RID: 316
		private const string GraphicsPresetConfigKey = "GraphicsLevel";

		// Token: 0x0400013D RID: 317
		private const string AutodetectedPresetConfigKey = "AutodetectedPreset";

		// Token: 0x0400013E RID: 318
		private const string AutodetectedLowResXConfigKey = "AutodetectedLowResX";

		// Token: 0x0400013F RID: 319
		private const string AutodetectedLowResYConfigKey = "AutodetectedLowResY";

		// Token: 0x04000140 RID: 320
		private const string AutodetectedUseLowResConfigKey = "AutodetectedUseLowRes";

		// Token: 0x04000141 RID: 321
		private const string ConfigSectionRootName = "AppCompatBucket";

		// Token: 0x04000142 RID: 322
		private const string AutoDetectSettingsConfigKey = "bAutoDetectSettings";

		// Token: 0x04000143 RID: 323
		private const string DisableWindowsKeyConfigKey = "DisableWindowsKey";

		// Token: 0x04000144 RID: 324
		private MainWindow.GraphicsPresetType m_autoDetectedGraphicsPreset = MainWindow.GraphicsPresetType.eLow;

		// Token: 0x04000145 RID: 325
		private readonly Dictionary<MainWindow.ConfigPresetKeys, MainWindow.GraphicsOptionType> m_graphicsConfigAndPresetKeys = new Dictionary<MainWindow.ConfigPresetKeys, MainWindow.GraphicsOptionType>
		{
			{
				new MainWindow.ConfigPresetKeys("Resolution", "ResX", "Resolution"),
				MainWindow.GraphicsOptionType.eResX
			},
			{
				new MainWindow.ConfigPresetKeys("Resolution", "ResY", "Resolution"),
				MainWindow.GraphicsOptionType.eResY
			},
			{
				new MainWindow.ConfigPresetKeys("AmbientOcclusion", "AmbientOcclusion", "AO"),
				MainWindow.GraphicsOptionType.eAmbientOcclusion
			},
			{
				new MainWindow.ConfigPresetKeys("Bloom", "Bloom", "Bloom"),
				MainWindow.GraphicsOptionType.eBloom
			},
			{
				new MainWindow.ConfigPresetKeys("DepthOfField", "DepthOfField", "DOF"),
				MainWindow.GraphicsOptionType.eDepthOfField
			},
			{
				new MainWindow.ConfigPresetKeys("MaxAnisotropy", "MaxAnisotropy", "Anisotropy"),
				MainWindow.GraphicsOptionType.eAnisotropicFiltering
			},
			{
				new MainWindow.ConfigPresetKeys("FoliageDrawRadiusMultiplier", "FoliageDrawRadiusMultiplier", "FoliageDistance"),
				MainWindow.GraphicsOptionType.eFoliageQuality
			},
			{
				new MainWindow.ConfigPresetKeys("TextureQuality", "TextureQuality", "TextureQuality"),
				MainWindow.GraphicsOptionType.eTextureQuality
			},
			{
				new MainWindow.ConfigPresetKeys("PopulationAdjustment", "PopulationAdjustment", "GameDetail"),
				MainWindow.GraphicsOptionType.eGameDetail
			},
			{
				new MainWindow.ConfigPresetKeys("bEnablePSSMShadows", "bEnablePSSMShadows", "DynamicShadows"),
				MainWindow.GraphicsOptionType.bEnablePSSMShadows
			},
			{
				new MainWindow.ConfigPresetKeys("DynamicDecals", "DynamicDecals", "DynamicDecals"),
				MainWindow.GraphicsOptionType.eDynamicDecals
			},
			{
				new MainWindow.ConfigPresetKeys("FlareOuts", "FlareOuts", "FlareOuts"),
				MainWindow.GraphicsOptionType.eFlareOuts
			}
		};

		// Token: 0x04000146 RID: 326
		public static readonly Dictionary<MainWindow.ConfigBool, string> m_configBoolLabel = new Dictionary<MainWindow.ConfigBool, string>
		{
			{
				MainWindow.ConfigBool.eTrue,
				"True"
			},
			{
				MainWindow.ConfigBool.eFalse,
				"False"
			}
		};

		// Token: 0x04000147 RID: 327
		public static readonly Dictionary<string, int> m_defaultBoolSettings = new Dictionary<string, int>
		{
			{
				MainWindow.m_configBoolLabel[MainWindow.ConfigBool.eTrue],
				0
			},
			{
				MainWindow.m_configBoolLabel[MainWindow.ConfigBool.eFalse],
				1
			}
		};

		// Token: 0x04000148 RID: 328
		// FIX: Preset bucket values map to the requested 2x/4x/8x/16x UI values.
		private static readonly Dictionary<string, int> m_defaultAnisotropicFilteringSettings = new Dictionary<string, int>
		{
			{ "0", 1 },
			{ "2", 2 },
			{ "4", 3 },
			{ "8", 3 },
			{ "16", 4 }
		};

		// Token: 0x04000149 RID: 329
		private static readonly Dictionary<string, int> m_defaultFoliageQualitySettings = new Dictionary<string, int>
		{
			{ "0.5", 0 },
			{ "1.0", 1 }
		};

		// Token: 0x0400014A RID: 330
		private static readonly Dictionary<string, int> m_defaultGameDetailSettings = new Dictionary<string, int>
		{
			{ "-40", 0 },
			{ "-20", 1 },
			{ "0", 2 }
		};

		// Token: 0x0400014B RID: 331
		private static readonly Dictionary<string, int> m_defaultTextureQuality = new Dictionary<string, int>
		{
			{ "2", 0 },
			{ "1", 1 },
			{ "0", 2 }
		};

		// Token: 0x0400014C RID: 332
		private Dictionary<string, MainWindow.GraphicsOptionType> m_configKeys;

		// Token: 0x0400014D RID: 333
		private int m_nCustomPresetIndex = -1;

		// Token: 0x0400014E RID: 334
		private int m_nCustomConfigSettingIndex = -1;

		// Token: 0x0400014F RID: 335
		private int m_nAutodetectGraphicsPresetConfigSettingIndex = -1;

		// Token: 0x04000150 RID: 336
		private int m_nGraphicsLevelConfigSettingIndex = -1;

		// Token: 0x04000151 RID: 337
		private IniFile m_graphicsPresetsIni;

		// Token: 0x04000152 RID: 338
		private const int MinGraphicsQuality = 6;

		// Token: 0x04000153 RID: 339
		private const int MaxGraphicsQuality = 9;

		// Token: 0x04000154 RID: 340
		private static readonly string UserConfigPath = Path.Combine(new string[]
		{
			Environment.GetFolderPath(Environment.SpecialFolder.Personal),
			"My Games",
			MainWindow.GameName,
			"WillowGame",
			"Config"
		});

		// Token: 0x04000155 RID: 341
		private static string m_sInstalledConfigPath;

		// Token: 0x04000156 RID: 342
		private static readonly string InstalledEngineConfigPath = Path.Combine(new string[]
		{
			AppDomain.CurrentDomain.BaseDirectory,
			"..",
			"..",
			"Engine",
			"Config"
		});

		// Token: 0x04000157 RID: 343
		private static readonly string InstalledLocPath = Path.Combine(new string[]
		{
			AppDomain.CurrentDomain.BaseDirectory,
			"..",
			"..",
			"WillowGame",
			"Localization"
		});

		// Token: 0x04000158 RID: 344
		private static readonly string GamePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "My Games", MainWindow.GameName);

		// Token: 0x04000159 RID: 345
		private const string InstalledConfigFilePattern = "Default{0}.ini";

		// Token: 0x0400015A RID: 346
		private const string UserConfigFilePattern = "Willow{0}.ini";

		// Token: 0x0400015B RID: 347
		private readonly string LocFilePattern = Path.Combine("{0}", "Willow{1}.{0}");

		// Token: 0x0400015C RID: 348
		private readonly string ReadMePath;

		// Token: 0x0400015D RID: 349
		private readonly List<string> ExesToTryStarting = new List<string>
		{
			Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WillowGame.exe"),
			Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WillowGame-PCConsole.exe"),
			Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BorderlandsGOTY.exe")
		};

		// Token: 0x0400015E RID: 350
		private int CurrentExeIndex;

		// Token: 0x0400015F RID: 351
		private Dictionary<MainWindow.GraphicsOptionType, Dictionary<string, int>> m_defaultValueToIndex = new Dictionary<MainWindow.GraphicsOptionType, Dictionary<string, int>>
		{
			{
				MainWindow.GraphicsOptionType.eAmbientOcclusion,
				MainWindow.m_defaultBoolSettings
			},
			{
				MainWindow.GraphicsOptionType.eBloom,
				MainWindow.m_defaultBoolSettings
			},
			{
				MainWindow.GraphicsOptionType.eDepthOfField,
				MainWindow.m_defaultBoolSettings
			},
			{
				MainWindow.GraphicsOptionType.bEnablePSSMShadows,
				MainWindow.m_defaultBoolSettings
			},
			{
				MainWindow.GraphicsOptionType.eDynamicDecals,
				MainWindow.m_defaultBoolSettings
			},
			{
				MainWindow.GraphicsOptionType.eFlareOuts,
				MainWindow.m_defaultBoolSettings
			},
			{
				MainWindow.GraphicsOptionType.eAnisotropicFiltering,
				MainWindow.m_defaultAnisotropicFilteringSettings
			},
			{
				MainWindow.GraphicsOptionType.eFoliageQuality,
				MainWindow.m_defaultFoliageQualitySettings
			},
			{
				MainWindow.GraphicsOptionType.eGameDetail,
				MainWindow.m_defaultGameDetailSettings
			},
			{
				MainWindow.GraphicsOptionType.eTextureQuality,
				MainWindow.m_defaultTextureQuality
			}
		};

		// Token: 0x04000160 RID: 352
		private static string m_sThreeLetterLanguageCode;

		// Token: 0x04000161 RID: 353
		internal MainWindow wndMain;

		// Token: 0x04000162 RID: 354
		internal Image imgDownloaded;

		// Token: 0x04000163 RID: 355
		internal Grid grdMessageBox;

		// Token: 0x04000164 RID: 356
		internal Label mbQuestion;

		// Token: 0x04000165 RID: 357
		internal Button mbButtonOne;

		// Token: 0x04000166 RID: 358
		internal Button mbButtonTwo;

		// Token: 0x04000167 RID: 359
		internal Button mbButtonThree;

		// Token: 0x04000168 RID: 360
		internal Grid grdHelp;

		// Token: 0x04000169 RID: 361
		internal Grid grdMain;

		// Token: 0x0400016A RID: 362
		internal Grid grdNews;

		// Token: 0x0400016B RID: 363
		internal Grid grdSettings;

		// Token: 0x0400016C RID: 364
		internal Label GPULabel;

		// Token: 0x0400016D RID: 365
		internal CustomScrollViewer svSettings;

		// Token: 0x0400016E RID: 366
		internal Label lblOverlayBackground;

		// Token: 0x0400016F RID: 367
		internal Label lblOverlayForeground;

		// Token: 0x04000170 RID: 368
		private bool _contentLoaded;

		// Token: 0x02000039 RID: 57
		// (Invoke) Token: 0x060001F7 RID: 503
		private delegate void MessageBoxButtonClicked(int ButtonIdx);

		// Token: 0x0200003A RID: 58
		private enum GraphicsPresetType
		{
			// Token: 0x04000172 RID: 370
			eCustom,
			// Token: 0x04000173 RID: 371
			eLow,
			// Token: 0x04000174 RID: 372
			eMedium,
			// Token: 0x04000175 RID: 373
			eHigh,
			// Token: 0x04000176 RID: 374
			eUltra,
			// Token: 0x04000177 RID: 375
			eNumGraphicsPresets
		}

		// Token: 0x0200003B RID: 59
		private enum GraphicsOptionType
		{
			// Token: 0x04000179 RID: 377
			eResX,
			// Token: 0x0400017A RID: 378
			eResY,
			// Token: 0x0400017B RID: 379
			eAmbientOcclusion,
			// Token: 0x0400017C RID: 380
			eBloom,
			// Token: 0x0400017D RID: 381
			eDepthOfField,
			// Token: 0x0400017E RID: 382
			eAnisotropicFiltering,
			// Token: 0x0400017F RID: 383
			eFoliageQuality,
			// Token: 0x04000180 RID: 384
			eTextureQuality,
			// Token: 0x04000181 RID: 385
			eGameDetail,
			// Token: 0x04000182 RID: 386
			bEnablePSSMShadows,
			// Token: 0x04000183 RID: 387
			eDynamicDecals,
			// Token: 0x04000184 RID: 388
			eFlareOuts
		}

		// Token: 0x0200003C RID: 60
		private struct ConfigPresetKeys
		{
			// Token: 0x060001FA RID: 506 RVA: 0x00003363 File Offset: 0x00001563
			public ConfigPresetKeys(string sConfigKey, string sPresetKey, string sLocNameKey)
			{
				this.m_sConfigKey = sConfigKey;
				this.m_sPresetKey = sPresetKey;
				this.m_sLocNameKey = sLocNameKey;
			}

			// Token: 0x04000185 RID: 389
			public string m_sConfigKey;

			// Token: 0x04000186 RID: 390
			public string m_sPresetKey;

			// Token: 0x04000187 RID: 391
			public string m_sLocNameKey;
		}

		// Token: 0x0200003D RID: 61
		public enum ConfigBool
		{
			// Token: 0x04000189 RID: 393
			eTrue,
			// Token: 0x0400018A RID: 394
			eFalse
		}
	}
}

