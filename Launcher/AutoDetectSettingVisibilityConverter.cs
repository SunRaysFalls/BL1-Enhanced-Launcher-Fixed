using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Launcher
{
	// Token: 0x02000032 RID: 50
	public class AutoDetectSettingVisibilityConverter : IValueConverter
	{
		// Token: 0x0600012A RID: 298 RVA: 0x00007244 File Offset: 0x00005444
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (DesignerProperties.GetIsInDesignMode(AutoDetectSettingVisibilityConverter.obj))
			{
				return Visibility.Visible;
			}
			if (!(value is Setting) || targetType != typeof(Visibility))
			{
				throw new InvalidOperationException("Must pass a setting to return a visibility for. Passed " + value.ToString() + targetType.ToString());
			}
			if ((value as Setting).bIsAutoSetSetting)
			{
				return (Application.Current.MainWindow as MainWindow).IsAutoDetectEnabled() ? Visibility.Collapsed : Visibility.Visible;
			}
			return Visibility.Visible;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x00002A61 File Offset: 0x00000C61
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		// Token: 0x040000E3 RID: 227
		private static DependencyObject obj = new DependencyObject();
	}
}
