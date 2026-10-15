using System;
using System.Globalization;
using System.Windows.Data;

namespace Launcher
{
	// Token: 0x0200002E RID: 46
	public class OpacityToBool : IValueConverter
	{
		// Token: 0x06000119 RID: 281 RVA: 0x00007010 File Offset: 0x00005210
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is double && parameter != null)
			{
				try
				{
					CultureInfo cultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();
					cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";
					return object.Equals(value, double.Parse(parameter as string, NumberStyles.Any, cultureInfo));
				}
				catch (Exception ex)
				{
					MainWindow.LogException(ex, "Unexpected value type. Value must be a double and the converter parameter must be the double that 'value' should be equal to. value.GetType()=" + value.GetType().ToString());
				}
			}
			return null;
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00002A61 File Offset: 0x00000C61
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
