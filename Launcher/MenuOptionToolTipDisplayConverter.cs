using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Launcher
{
	// Token: 0x02000030 RID: 48
	public class MenuOptionToolTipDisplayConverter : IValueConverter
	{
		// Token: 0x06000120 RID: 288 RVA: 0x00007124 File Offset: 0x00005324
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Setting setting = value as Setting;
			if (setting != null)
			{
				string text = MainWindow.MenuNamesLocFile["FEaudiovideo"][setting.LocNameKey + "Desc"].Trim(new char[] { '"' });
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
				return null;
			}
			else
			{
				if (DesignerProperties.GetIsInDesignMode(MenuOptionToolTipDisplayConverter.obj))
				{
					return null;
				}
				MainWindow.LogException(new Exception(MainWindow.Instance.GetResourceString("ConfigFileLoadError_Text")), "Did not provide a valid setting. Provided " + value.GetType().ToString());
				return null;
			}
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00002A61 File Offset: 0x00000C61
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		// Token: 0x040000E1 RID: 225
		private static DependencyObject obj = new DependencyObject();
	}
}
