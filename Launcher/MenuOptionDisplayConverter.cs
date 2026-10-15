using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Launcher
{
	// Token: 0x0200002F RID: 47
	public class MenuOptionDisplayConverter : IValueConverter
	{
		// Token: 0x0600011C RID: 284 RVA: 0x0000709C File Offset: 0x0000529C
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Setting setting = value as Setting;
			if (setting != null)
			{
				return MainWindow.UCWords(MainWindow.MenuNamesLocFile["FEaudiovideo"][setting.LocNameKey].Trim(new char[] { '"' }));
			}
			if (DesignerProperties.GetIsInDesignMode(MenuOptionDisplayConverter.obj))
			{
				return "Setting with a name that's way too long to ever fit.";
			}
			MainWindow.LogException(new Exception(MainWindow.Instance.GetResourceString("ConfigFileLoadError_Text")), "Did not provide a valid setting. Provided " + value.GetType().ToString());
			return null;
		}

		// Token: 0x0600011D RID: 285 RVA: 0x00002A61 File Offset: 0x00000C61
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		// Token: 0x040000E0 RID: 224
		private static DependencyObject obj = new DependencyObject();
	}
}
