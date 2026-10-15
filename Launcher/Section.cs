using System;
using System.Collections.Generic;

namespace Launcher
{
	// Token: 0x02000035 RID: 53
	public class Section
	{
		// Token: 0x0600013D RID: 317 RVA: 0x00002B61 File Offset: 0x00000D61
		internal Section(string SectionName)
		{
			this._SectionName = SectionName;
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00007528 File Offset: 0x00005728
		internal Section(Section src)
		{
			this._SectionName = src._SectionName;
			this._Parameters = new List<Parameter>();
			checked
			{
				for (int i = 0; i < src._Parameters.Count; i++)
				{
					this._Parameters.Add(new Parameter(src._Parameters[i]));
				}
			}
		}

		// Token: 0x0600013F RID: 319 RVA: 0x00007590 File Offset: 0x00005790
		internal Parameter GetParameter(string ParameterName)
		{
			foreach (Parameter parameter in this._Parameters)
			{
				if (parameter.Name.Equals(ParameterName, StringComparison.InvariantCultureIgnoreCase))
				{
					return parameter;
				}
			}
			return null;
		}

		// Token: 0x06000140 RID: 320 RVA: 0x000075F4 File Offset: 0x000057F4
		internal void AddParameter(Parameter NewParameter)
		{
			Parameter parameter = this.GetParameter(NewParameter.Name);
			if (parameter != null)
			{
				parameter.AddValue(NewParameter.Value);
				return;
			}
			this._Parameters.Add(NewParameter);
		}

		// Token: 0x06000141 RID: 321 RVA: 0x0000762C File Offset: 0x0000582C
		internal void UpdateParameter(string sParameterName, string sParameterValue, string sIniFilename)
		{
			Parameter parameter = this.GetParameter(sParameterName);
			if (parameter != null)
			{
				parameter.SetValue(sParameterValue, sIniFilename, this.Name, true);
				return;
			}
			this.AddParameter(new Parameter(sParameterName, sParameterValue));
		}

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000142 RID: 322 RVA: 0x00002B7B File Offset: 0x00000D7B
		internal string Name
		{
			get
			{
				return this._SectionName;
			}
		}

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000143 RID: 323 RVA: 0x00002B83 File Offset: 0x00000D83
		internal Parameter[] Parameters
		{
			get
			{
				return this._Parameters.ToArray();
			}
		}

		// Token: 0x06000144 RID: 324 RVA: 0x00002B90 File Offset: 0x00000D90
		public override string ToString()
		{
			return string.Format("[{0}]", this._SectionName);
		}

		// Token: 0x06000145 RID: 325 RVA: 0x00007664 File Offset: 0x00005864
		public string[] GetParameterValues(string ParameterName)
		{
			Parameter parameter = this.GetParameter(ParameterName);
			if (parameter != null)
			{
				return parameter.Values;
			}
			return new string[0];
		}

		// Token: 0x17000044 RID: 68
		public string this[string param]
		{
			get
			{
				foreach (Parameter parameter in this._Parameters)
				{
					if (parameter.Name.Equals(param, StringComparison.InvariantCultureIgnoreCase))
					{
						return parameter.Value.Trim();
					}
				}
				return "";
			}
		}

		// Token: 0x040000E6 RID: 230
		private readonly string _SectionName;

		// Token: 0x040000E7 RID: 231
		private readonly List<Parameter> _Parameters = new List<Parameter>();
	}
}
