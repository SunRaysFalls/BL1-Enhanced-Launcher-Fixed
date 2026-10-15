using System;
using System.Collections.Generic;

namespace Launcher
{
	// Token: 0x02000034 RID: 52
	internal class Parameter
	{
		// Token: 0x06000135 RID: 309 RVA: 0x00002AC9 File Offset: 0x00000CC9
		internal Parameter(string ParameterName, string ParameterValue)
		{
			this._ParameterName = ParameterName;
			this._ParameterValues.Add(ParameterValue);
		}

		// Token: 0x06000136 RID: 310 RVA: 0x00007434 File Offset: 0x00005634
		public Parameter(Parameter src)
		{
			this._ParameterName = src._ParameterName;
			this._ParameterValues = new List<string>();
			checked
			{
				for (int i = 0; i < src._ParameterValues.Count; i++)
				{
					this._ParameterValues.Add(src._ParameterValues[i]);
				}
			}
		}

		// Token: 0x06000137 RID: 311 RVA: 0x00002AEF File Offset: 0x00000CEF
		internal void AddValue(string ParameterValue)
		{
			this._ParameterValues.Add(ParameterValue);
		}

		// Token: 0x06000138 RID: 312 RVA: 0x00007498 File Offset: 0x00005698
		internal void SetValue(string ParameterValue, string sIniFileName, string sSectionName, bool bEnforceUniqueParameters = true)
		{
			if (this._ParameterValues.Count != 1 && bEnforceUniqueParameters)
			{
				MainWindow.LogException(new Exception(MainWindow.Instance.GetResourceString("ConfigFileLoadError_Text")), string.Concat(new object[]
				{
					sIniFileName,
					": section=",
					sSectionName,
					": parameterName=",
					this._ParameterName,
					": Expected exactly 1 parameter value but found ",
					this._ParameterValues.Count
				}));
			}
			this._ParameterValues[0] = ParameterValue;
		}

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x06000139 RID: 313 RVA: 0x00002AFD File Offset: 0x00000CFD
		internal string Name
		{
			get
			{
				return this._ParameterName;
			}
		}

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x0600013A RID: 314 RVA: 0x00002B05 File Offset: 0x00000D05
		internal string Value
		{
			get
			{
				if (this._ParameterValues.Count <= 0)
				{
					return "";
				}
				return this._ParameterValues[0];
			}
		}

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x0600013B RID: 315 RVA: 0x00002B27 File Offset: 0x00000D27
		internal string[] Values
		{
			get
			{
				return this._ParameterValues.ToArray();
			}
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00002B34 File Offset: 0x00000D34
		public override string ToString()
		{
			return string.Format("{0}={1}", this._ParameterName, (this._ParameterValues.Count > 0) ? "Multiple Values" : this.Value);
		}

		// Token: 0x040000E4 RID: 228
		private readonly string _ParameterName;

		// Token: 0x040000E5 RID: 229
		private readonly List<string> _ParameterValues = new List<string>();
	}
}
