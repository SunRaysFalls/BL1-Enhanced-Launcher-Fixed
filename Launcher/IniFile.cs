using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Launcher
{
	// Token: 0x02000036 RID: 54
	public class IniFile
	{
		// Token: 0x17000045 RID: 69
		// (get) Token: 0x06000147 RID: 327 RVA: 0x00002BA2 File Offset: 0x00000DA2
		// (set) Token: 0x06000148 RID: 328 RVA: 0x00002BAA File Offset: 0x00000DAA
		public string FileName { get; private set; }

		// Token: 0x06000149 RID: 329 RVA: 0x00002BB3 File Offset: 0x00000DB3
		private IniFile(string InFileName, List<Section> Sections)
		{
			this._Sections = Sections;
			this.FileName = InFileName;
		}

		// Token: 0x0600014A RID: 330 RVA: 0x000076FC File Offset: 0x000058FC
		[Conditional("DEBUG")]
		internal void DebugDump()
		{
			checked
			{
				foreach (Section section in this._Sections)
				{
					Console.WriteLine(section);
					foreach (Parameter parameter in section.Parameters)
					{
						Console.Write("Parameter: " + parameter.Name + "=");
						int num = parameter.Values.Length;
						for (int j = 0; j < num; j++)
						{
							Console.Write(parameter.Values[j] + ((j < num - 1) ? ", " : ""));
						}
						Console.WriteLine("");
					}
				}
			}
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00002BC9 File Offset: 0x00000DC9
		internal bool HasSection(string SectionName)
		{
			return this.GetSections(SectionName).Count<Section>() > 0;
		}

		// Token: 0x0600014C RID: 332 RVA: 0x00002BDA File Offset: 0x00000DDA
		internal void AddSection(string SectionName)
		{
			if (this.GetSections(SectionName).Count<Section>() == 0)
			{
				this._Sections.Add(new Section(SectionName));
			}
		}

		// Token: 0x0600014D RID: 333 RVA: 0x000077D4 File Offset: 0x000059D4
		internal Section GetSection(string SectionName)
		{
			IEnumerable<Section> sections = this.GetSections(SectionName);
			if (sections.Count<Section>() != 1)
			{
				MainWindow.LogException(new Exception(MainWindow.Instance.GetResourceString("ConfigFileLoadError_Text")), string.Format("{0}: Expected exactly 1 section named [{1}] and found {2}.", this.FileName, SectionName, sections.Count<Section>()));
			}
			return sections.First<Section>();
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00002BFB File Offset: 0x00000DFB
		internal IEnumerable<Section> GetSections(string SectionName)
		{
			foreach (Section section in this._Sections)
			{
				if (section.Name.Equals(SectionName, StringComparison.InvariantCultureIgnoreCase))
				{
					yield return section;
				}
			}
			List<Section>.Enumerator enumerator = default(List<Section>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00007830 File Offset: 0x00005A30
		internal void Save(string SaveAsFileName = null, bool bUpdateFilename = false)
		{
			using (StreamWriter streamWriter = new StreamWriter(string.IsNullOrWhiteSpace(SaveAsFileName) ? this.FileName : SaveAsFileName))
			{
				foreach (Section section in this._Sections)
				{
					streamWriter.WriteLine(string.Format("[{0}]", section.Name));
					foreach (Parameter parameter in section.Parameters)
					{
						foreach (string text in parameter.Values)
						{
							streamWriter.WriteLine(string.Format("{0}={1}", parameter.Name, text));
						}
					}
					streamWriter.WriteLine();
				}
			}
			if (SaveAsFileName != null && bUpdateFilename)
			{
				this.FileName = SaveAsFileName;
			}
		}

		// Token: 0x17000046 RID: 70
		public Section this[string parm]
		{
			get
			{
				return this.GetSection(parm);
			}
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00007934 File Offset: 0x00005B34
		internal static IniFile Parse(string FileName)
		{
			if (!File.Exists(FileName))
			{
				return null;
			}
			List<Section> list = new List<Section>();
			int num = 0;
			checked
			{
				using (TextReader textReader = new StreamReader(FileName))
				{
					for (string text = textReader.ReadLine(); text != null; text = textReader.ReadLine())
					{
						num++;
						text = text.Trim();
						if (text.Length > 0 && !text.StartsWith("#", StringComparison.InvariantCultureIgnoreCase))
						{
							if (text.StartsWith("[") && text.EndsWith("]"))
							{
								list.Add(new Section(text.Substring(1, text.Length - 2)));
							}
							else if (!text.StartsWith(";") && !text.StartsWith("//"))
							{
								if (text.Contains('='))
								{
									string text2 = text.Substring(0, text.IndexOf('='));
									string text3 = text.Substring(text.IndexOf('=') + 1);
									StringBuilder stringBuilder = new StringBuilder();
									bool flag = false;
									char c = '\\';
									foreach (char c2 in text3)
									{
										if (c2 == '"' && c != '\\')
										{
											flag = !flag;
										}
										if (c2 == ';' && !flag)
										{
											break;
										}
										c = c2;
										stringBuilder.Append(c2);
									}
									text3 = stringBuilder.ToString().Trim();
									if (list.Count > 0)
									{
										list[list.Count - 1].AddParameter(new Parameter(text2.Trim(), text3.Trim()));
									}
									else
									{
										MainWindow.LogException(new Exception(MainWindow.Instance.GetResourceString("ConfigFileLoadError_Text")), "No section available in IniFile.Parse(): FileName=" + FileName);
									}
								}
								else
								{
									MainWindow.LogMessage(string.Concat(new object[] { "Bad formatting in ", FileName, " on line ", num, ": error==>", text }), false);
								}
							}
						}
					}
				}
				return new IniFile(FileName, list);
			}
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00007B50 File Offset: 0x00005D50
		public IniFile Copy(string sPathname)
		{
			List<Section> list = new List<Section>();
			checked
			{
				for (int i = 0; i < this._Sections.Count; i++)
				{
					list.Add(new Section(this._Sections[i]));
				}
				return new IniFile(sPathname, list);
			}
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00007B98 File Offset: 0x00005D98
		public void OverlayIni(IniFile overlayIni, bool bEnforceUniqueParameters = true)
		{
			int count = overlayIni._Sections.Count;
			checked
			{
				for (int i = 0; i < count; i++)
				{
					Section section = overlayIni.GetSection(overlayIni._Sections[i].Name);
					string name = section.Name;
					Section section2;
					if (!this.HasSection(name))
					{
						this.AddSection(name);
						section2 = this.GetSection(name);
					}
					else
					{
						section2 = this.GetSection(name);
					}
					int num = section.Parameters.Length;
					for (int j = 0; j < num; j++)
					{
						Parameter parameter = section.Parameters[j];
						Parameter parameter2 = section2.GetParameter(parameter.Name);
						if (parameter2 == null)
						{
							section2.AddParameter(parameter);
						}
						else
						{
							parameter2.SetValue(parameter.Value, this.FileName, name, bEnforceUniqueParameters);
						}
					}
				}
			}
		}

		// Token: 0x06000154 RID: 340 RVA: 0x00007C64 File Offset: 0x00005E64
		public string GetValue(string sSectionName, string sParameterName, string sDefaultValue)
		{
			if (this.HasSection(sSectionName))
			{
				Parameter parameter = this.GetSection(sSectionName).GetParameter(sParameterName);
				if (parameter != null)
				{
					return parameter.Value;
				}
			}
			return sDefaultValue;
		}

		// Token: 0x06000155 RID: 341 RVA: 0x00002C1B File Offset: 0x00000E1B
		public bool HasValue(string sSectionName, string sParameterName)
		{
			return this.HasSection(sSectionName) && this.GetSection(sSectionName).GetParameter(sParameterName) != null;
		}

		// Token: 0x06000156 RID: 342 RVA: 0x00002C38 File Offset: 0x00000E38
		public void SetValue(string sSectionName, string sParameterName, string sParameterValue)
		{
			if (this.HasSection(sSectionName))
			{
				this.GetSection(sSectionName).UpdateParameter(sParameterName, sParameterValue, this.FileName);
			}
		}

		// Token: 0x040000E8 RID: 232
		private readonly List<Section> _Sections;
	}
}
