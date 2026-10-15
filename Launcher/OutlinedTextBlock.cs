using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace Launcher
{
	// Token: 0x02000021 RID: 33
	[ContentProperty("Text")]
	public class OutlinedTextBlock : FrameworkElement
	{
		// Token: 0x0600008F RID: 143 RVA: 0x00002499 File Offset: 0x00000699
		public OutlinedTextBlock()
		{
			this.TextDecorations = new TextDecorationCollection();
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000090 RID: 144 RVA: 0x000024AC File Offset: 0x000006AC
		// (set) Token: 0x06000091 RID: 145 RVA: 0x000024BE File Offset: 0x000006BE
		public Brush Fill
		{
			get
			{
				return (Brush)base.GetValue(OutlinedTextBlock.FillProperty);
			}
			set
			{
				base.SetValue(OutlinedTextBlock.FillProperty, value);
			}
		}

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000092 RID: 146 RVA: 0x000024CC File Offset: 0x000006CC
		// (set) Token: 0x06000093 RID: 147 RVA: 0x000024DE File Offset: 0x000006DE
		public FontFamily FontFamily
		{
			get
			{
				return (FontFamily)base.GetValue(OutlinedTextBlock.FontFamilyProperty);
			}
			set
			{
				base.SetValue(OutlinedTextBlock.FontFamilyProperty, value);
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000094 RID: 148 RVA: 0x000024EC File Offset: 0x000006EC
		// (set) Token: 0x06000095 RID: 149 RVA: 0x000024FE File Offset: 0x000006FE
		[TypeConverter(typeof(FontSizeConverter))]
		public double FontSize
		{
			get
			{
				return (double)base.GetValue(OutlinedTextBlock.FontSizeProperty);
			}
			set
			{
				base.SetValue(OutlinedTextBlock.FontSizeProperty, value);
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000096 RID: 150 RVA: 0x00002511 File Offset: 0x00000711
		// (set) Token: 0x06000097 RID: 151 RVA: 0x00002523 File Offset: 0x00000723
		public FontStretch FontStretch
		{
			get
			{
				return (FontStretch)base.GetValue(OutlinedTextBlock.FontStretchProperty);
			}
			set
			{
				base.SetValue(OutlinedTextBlock.FontStretchProperty, value);
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000098 RID: 152 RVA: 0x00002536 File Offset: 0x00000736
		// (set) Token: 0x06000099 RID: 153 RVA: 0x00002548 File Offset: 0x00000748
		public FontStyle FontStyle
		{
			get
			{
				return (FontStyle)base.GetValue(OutlinedTextBlock.FontStyleProperty);
			}
			set
			{
				base.SetValue(OutlinedTextBlock.FontStyleProperty, value);
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600009A RID: 154 RVA: 0x0000255B File Offset: 0x0000075B
		// (set) Token: 0x0600009B RID: 155 RVA: 0x0000256D File Offset: 0x0000076D
		public FontWeight FontWeight
		{
			get
			{
				return (FontWeight)base.GetValue(OutlinedTextBlock.FontWeightProperty);
			}
			set
			{
				base.SetValue(OutlinedTextBlock.FontWeightProperty, value);
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x0600009C RID: 156 RVA: 0x00002580 File Offset: 0x00000780
		// (set) Token: 0x0600009D RID: 157 RVA: 0x00002592 File Offset: 0x00000792
		public Brush Stroke
		{
			get
			{
				return (Brush)base.GetValue(OutlinedTextBlock.StrokeProperty);
			}
			set
			{
				base.SetValue(OutlinedTextBlock.StrokeProperty, value);
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x0600009E RID: 158 RVA: 0x000025A0 File Offset: 0x000007A0
		// (set) Token: 0x0600009F RID: 159 RVA: 0x000025B2 File Offset: 0x000007B2
		public double StrokeThickness
		{
			get
			{
				return (double)base.GetValue(OutlinedTextBlock.StrokeThicknessProperty);
			}
			set
			{
				base.SetValue(OutlinedTextBlock.StrokeThicknessProperty, value);
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x000025C5 File Offset: 0x000007C5
		// (set) Token: 0x060000A1 RID: 161 RVA: 0x000025D7 File Offset: 0x000007D7
		public string Text
		{
			get
			{
				return (string)base.GetValue(OutlinedTextBlock.TextProperty);
			}
			set
			{
				base.SetValue(OutlinedTextBlock.TextProperty, value);
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000A2 RID: 162 RVA: 0x000025E5 File Offset: 0x000007E5
		// (set) Token: 0x060000A3 RID: 163 RVA: 0x000025F7 File Offset: 0x000007F7
		public TextAlignment TextAlignment
		{
			get
			{
				return (TextAlignment)base.GetValue(OutlinedTextBlock.TextAlignmentProperty);
			}
			set
			{
				base.SetValue(OutlinedTextBlock.TextAlignmentProperty, value);
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000A4 RID: 164 RVA: 0x0000260A File Offset: 0x0000080A
		// (set) Token: 0x060000A5 RID: 165 RVA: 0x0000261C File Offset: 0x0000081C
		public TextDecorationCollection TextDecorations
		{
			get
			{
				return (TextDecorationCollection)base.GetValue(OutlinedTextBlock.TextDecorationsProperty);
			}
			set
			{
				base.SetValue(OutlinedTextBlock.TextDecorationsProperty, value);
			}
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x0000262A File Offset: 0x0000082A
		// (set) Token: 0x060000A7 RID: 167 RVA: 0x0000263C File Offset: 0x0000083C
		public TextTrimming TextTrimming
		{
			get
			{
				return (TextTrimming)base.GetValue(OutlinedTextBlock.TextTrimmingProperty);
			}
			set
			{
				base.SetValue(OutlinedTextBlock.TextTrimmingProperty, value);
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x0000264F File Offset: 0x0000084F
		// (set) Token: 0x060000A9 RID: 169 RVA: 0x00002661 File Offset: 0x00000861
		public TextWrapping TextWrapping
		{
			get
			{
				return (TextWrapping)base.GetValue(OutlinedTextBlock.TextWrappingProperty);
			}
			set
			{
				base.SetValue(OutlinedTextBlock.TextWrappingProperty, value);
			}
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00002674 File Offset: 0x00000874
		protected override void OnRender(DrawingContext drawingContext)
		{
			this.EnsureGeometry();
			drawingContext.DrawGeometry(null, this.pen, this.textGeometry);
		}

		// Token: 0x060000AB RID: 171 RVA: 0x00005A20 File Offset: 0x00003C20
		protected override Size MeasureOverride(Size availableSize)
		{
			this.EnsureFormattedText();
			if (this.formattedText == null)
			{
				return new Size(10.0, 10.0);
			}
			this.formattedText.MaxTextWidth = Math.Min(3579139.0, availableSize.Width);
			this.formattedText.MaxTextHeight = availableSize.Height;
			return new Size(this.formattedText.Width, this.formattedText.Height);
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00005AA0 File Offset: 0x00003CA0
		protected override Size ArrangeOverride(Size finalSize)
		{
			this.EnsureFormattedText();
			if (this.formattedText == null)
			{
				return finalSize;
			}
			this.formattedText.MaxTextWidth = finalSize.Width;
			this.formattedText.MaxTextHeight = Math.Max(1.0, finalSize.Height);
			this.textGeometry = null;
			return finalSize;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x0000268F File Offset: 0x0000088F
		private static void OnFormattedTextInvalidated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			OutlinedTextBlock outlinedTextBlock = (OutlinedTextBlock)dependencyObject;
			outlinedTextBlock.formattedText = null;
			outlinedTextBlock.textGeometry = null;
			outlinedTextBlock.InvalidateMeasure();
			outlinedTextBlock.InvalidateVisual();
		}

		// Token: 0x060000AE RID: 174 RVA: 0x000026B0 File Offset: 0x000008B0
		private static void OnFormattedTextUpdated(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
		{
			OutlinedTextBlock outlinedTextBlock = (OutlinedTextBlock)dependencyObject;
			outlinedTextBlock.UpdateFormattedText();
			outlinedTextBlock.textGeometry = null;
			outlinedTextBlock.InvalidateMeasure();
			outlinedTextBlock.InvalidateVisual();
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00005AF8 File Offset: 0x00003CF8
		private void EnsureFormattedText()
		{
			if (this.formattedText != null || this.Text == null)
			{
				return;
			}
			this.formattedText = new FormattedText(this.Text, CultureInfo.CurrentUICulture, base.FlowDirection, new Typeface(this.FontFamily, this.FontStyle, this.FontWeight, FontStretches.Normal), this.FontSize, Brushes.Black);
			this.UpdateFormattedText();
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x00005B60 File Offset: 0x00003D60
		private void UpdateFormattedText()
		{
			if (this.formattedText == null)
			{
				return;
			}
			this.formattedText.MaxLineCount = ((this.TextWrapping == TextWrapping.NoWrap) ? 1 : int.MaxValue);
			this.formattedText.TextAlignment = this.TextAlignment;
			this.formattedText.Trimming = this.TextTrimming;
			this.formattedText.SetFontSize(this.FontSize);
			this.formattedText.SetFontStyle(this.FontStyle);
			this.formattedText.SetFontWeight(this.FontWeight);
			this.formattedText.SetFontFamily(this.FontFamily);
			this.formattedText.SetFontStretch(this.FontStretch);
			this.formattedText.SetTextDecorations(this.TextDecorations);
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x00005C1C File Offset: 0x00003E1C
		private void EnsureGeometry()
		{
			if (this.textGeometry != null || this.formattedText == null)
			{
				return;
			}
			this.EnsureFormattedText();
			this.textGeometry = this.formattedText.BuildGeometry(new Point(0.0, 0.0));
			this.pen = new Pen(this.Stroke, this.StrokeThickness);
			this.pen.LineJoin = PenLineJoin.Round;
			this.pen.MiterLimit = 10.0;
		}

		// Token: 0x04000099 RID: 153
		public static readonly DependencyProperty FillProperty = DependencyProperty.Register("Fill", typeof(Brush), typeof(OutlinedTextBlock), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

		// Token: 0x0400009A RID: 154
		public static readonly DependencyProperty StrokeProperty = DependencyProperty.Register("Stroke", typeof(Brush), typeof(OutlinedTextBlock), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));

		// Token: 0x0400009B RID: 155
		public static readonly DependencyProperty StrokeThicknessProperty = DependencyProperty.Register("StrokeThickness", typeof(double), typeof(OutlinedTextBlock), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

		// Token: 0x0400009C RID: 156
		public static readonly DependencyProperty FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner(typeof(OutlinedTextBlock), new FrameworkPropertyMetadata(new PropertyChangedCallback(OutlinedTextBlock.OnFormattedTextUpdated)));

		// Token: 0x0400009D RID: 157
		public static readonly DependencyProperty FontSizeProperty = TextElement.FontSizeProperty.AddOwner(typeof(OutlinedTextBlock), new FrameworkPropertyMetadata(new PropertyChangedCallback(OutlinedTextBlock.OnFormattedTextUpdated)));

		// Token: 0x0400009E RID: 158
		public static readonly DependencyProperty FontStretchProperty = TextElement.FontStretchProperty.AddOwner(typeof(OutlinedTextBlock), new FrameworkPropertyMetadata(new PropertyChangedCallback(OutlinedTextBlock.OnFormattedTextUpdated)));

		// Token: 0x0400009F RID: 159
		public static readonly DependencyProperty FontStyleProperty = TextElement.FontStyleProperty.AddOwner(typeof(OutlinedTextBlock), new FrameworkPropertyMetadata(new PropertyChangedCallback(OutlinedTextBlock.OnFormattedTextUpdated)));

		// Token: 0x040000A0 RID: 160
		public static readonly DependencyProperty FontWeightProperty = TextElement.FontWeightProperty.AddOwner(typeof(OutlinedTextBlock), new FrameworkPropertyMetadata(new PropertyChangedCallback(OutlinedTextBlock.OnFormattedTextUpdated)));

		// Token: 0x040000A1 RID: 161
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(OutlinedTextBlock), new FrameworkPropertyMetadata(new PropertyChangedCallback(OutlinedTextBlock.OnFormattedTextInvalidated)));

		// Token: 0x040000A2 RID: 162
		public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(OutlinedTextBlock), new FrameworkPropertyMetadata(new PropertyChangedCallback(OutlinedTextBlock.OnFormattedTextUpdated)));

		// Token: 0x040000A3 RID: 163
		public static readonly DependencyProperty TextDecorationsProperty = DependencyProperty.Register("TextDecorations", typeof(TextDecorationCollection), typeof(OutlinedTextBlock), new FrameworkPropertyMetadata(new PropertyChangedCallback(OutlinedTextBlock.OnFormattedTextUpdated)));

		// Token: 0x040000A4 RID: 164
		public static readonly DependencyProperty TextTrimmingProperty = DependencyProperty.Register("TextTrimming", typeof(TextTrimming), typeof(OutlinedTextBlock), new FrameworkPropertyMetadata(new PropertyChangedCallback(OutlinedTextBlock.OnFormattedTextUpdated)));

		// Token: 0x040000A5 RID: 165
		public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register("TextWrapping", typeof(TextWrapping), typeof(OutlinedTextBlock), new FrameworkPropertyMetadata(TextWrapping.NoWrap, new PropertyChangedCallback(OutlinedTextBlock.OnFormattedTextUpdated)));

		// Token: 0x040000A6 RID: 166
		private FormattedText formattedText;

		// Token: 0x040000A7 RID: 167
		private Geometry textGeometry;

		// Token: 0x040000A8 RID: 168
		private Pen pen;
	}
}
