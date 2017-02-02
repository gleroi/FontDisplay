using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FontsDisplay
{
    /// <summary>
    ///     Interaction logic for FontView.xaml
    /// </summary>
    public partial class FontView : UserControl
    {
        public FontView()
        {
            InitializeComponent();
        }

        private void ExportToPngClicked(object sender, RoutedEventArgs e)
        {
            var bitmap = this.RenderToBitmap(this.CharactersList);
            var fontpath = this.FontPath.Text;
            var picPath = Path.ChangeExtension(fontpath, "png");
            this.SaveToPng(bitmap, picPath);
        }

        private void SaveToPng(RenderTargetBitmap bitmap, string picPath)
        {
            using (var output = new FileStream(picPath, FileMode.Create))
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(output);
            }
        }

        private RenderTargetBitmap RenderToBitmap(Control view)
        {
            const int dpi = 96;
            const double dpiratio = dpi / 96d;
            Size size = new Size(Math.Ceiling(view.ActualWidth * dpiratio), Math.Ceiling(view.ActualHeight * dpiratio));

            DrawingVisual drawingvisual = new DrawingVisual();
            using (DrawingContext context = drawingvisual.RenderOpen())
            {
                context.DrawRectangle(new VisualBrush(view), null, new Rect(new Point(), size));
                context.Close();
            }

            var bitmap = new RenderTargetBitmap((int)size.Width, (int)size.Height, dpi, dpi, PixelFormats.Pbgra32);
            bitmap.Render(drawingvisual);
            return bitmap;
        }
    }
}