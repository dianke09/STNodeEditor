using System;
using System.Drawing;
using SkiaSharp;

namespace ST.Library.UI.NodeEditor {
    public static class SkiaDrawingHelper {
        public static SKColor ToSKColor(Color color) {
            return new SKColor(color.R, color.G, color.B, color.A);
        }

        public static SKRect ToSKRect(Rectangle rect) {
            return new SKRect(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        public static void RenderToGraphics(Graphics graphics, Size size, Action<SKCanvas> renderAction) {
            using (var bitmap = new SKBitmap(Math.Max(size.Width, 1), Math.Max(size.Height, 1), true))
            using (var canvas = new SKCanvas(bitmap)) {
                canvas.Clear(SKColors.Transparent);
                renderAction(canvas);
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var ms = new System.IO.MemoryStream(data.ToArray()))
                using (var gdiImage = Image.FromStream(ms)) {
                    graphics.DrawImageUnscaled(gdiImage, 0, 0);
                }
            }
        }
    }
}
