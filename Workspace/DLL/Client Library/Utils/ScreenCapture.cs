using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ClientLibrary.Utils
{
    public static class ScreenCapture
    {
        private static readonly float[][] colorMatrix = new float[][] { new float[] { .3f, .3f, .3f, 0, 0 }, new float[] { .59f, .59f, .59f, 0, 0 }, new float[] { .11f, .11f, .11f, 0, 0 }, new float[] { 0, 0, 0, 1, 0 }, new float[] { 0, 0, 0, 0, 1 } };

        public static Bitmap shoot()
        {
            using (Graphics g1 = Graphics.FromHwnd(IntPtr.Zero))
            {
                IntPtr desktop = g1.GetHdc();

                int logicalScreenWidth = GetDeviceCaps(desktop, (int)DeviceCap.HORZRES);
                int logicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);

                int physicalScreenWidth = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPHORZRES);
                int physicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

                float screenWidthScalingFactor = (float)Math.Round(physicalScreenWidth / (float)logicalScreenWidth, 2);
                float screenHeightScalingFactor = (float)Math.Round(physicalScreenHeight / (float)logicalScreenHeight, 2);

                Bitmap screenshot = new Bitmap(physicalScreenWidth, physicalScreenHeight, PixelFormat.Format16bppRgb555);
                using (Graphics g2 = Graphics.FromImage(screenshot))
                {
                    g2.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, new Size(physicalScreenWidth, physicalScreenHeight), CopyPixelOperation.SourceCopy);
                    Cursors.Default.Draw(g2, new Rectangle(new Point((int)(Cursor.Position.X * screenWidthScalingFactor), (int)(Cursor.Position.Y * screenHeightScalingFactor)), Cursors.Default.Size));
                }

                return screenshot;
            }
        }

        public static Bitmap compress(Bitmap image, float resize, bool grayscale, float gamma)
        {
            Bitmap resized = ScreenCapture.resize(image, resize);

            if (grayscale)
            {
                Bitmap graysacled = getGrayscale(resized);
                
                resized.Dispose();
                resized = graysacled;
            }

            if (gamma != 1)
            {
                Bitmap gammaCorrected = getGammaCorrection(resized, gamma);

                resized.Dispose();
                resized = gammaCorrected;
            }

            return resized;
        }

        public static Bitmap resize(Bitmap image, Size resize)
        {
            if (image.Size != resize)
            {
                Bitmap resized = new Bitmap(resize.Width, resize.Height, image.PixelFormat);
                using (Graphics g = Graphics.FromImage(resized))
                    g.DrawImage(image, 0, 0, resize.Width, resize.Height);

                return resized;
            }
            else return image.Clone(new Rectangle(0, 0, image.Width, image.Height), image.PixelFormat);
        }

        public static Bitmap resize(Bitmap image, float resize)
        {
            Size newSize = new Size((int)(image.Width * resize), (int)(image.Height * resize));
            return ScreenCapture.resize(image, newSize);
        }

        public static Bitmap getGrayscale(Bitmap original)
        {
            Bitmap grayscaled = new Bitmap(original.Width, original.Height, original.PixelFormat);

            using (Graphics g = Graphics.FromImage(grayscaled))
            {
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(new ColorMatrix(colorMatrix));

                g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            }

            return grayscaled;
        }

        public static Bitmap getGammaCorrection(Bitmap original, float gamma)
        {
            if (gamma != 1)
            {
                Bitmap gammaCorrected = new Bitmap(original.Width, original.Height, original.PixelFormat);

                using (Graphics g = Graphics.FromImage(gammaCorrected))
                {
                    ImageAttributes attributes = new ImageAttributes();
                    attributes.SetGamma(gamma);

                    g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height), 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
                }

                return gammaCorrected;
            }
            else return original.Clone(new Rectangle(0, 0, original.Width, original.Height), original.PixelFormat);
        }

        public static ImageCodecInfo getEncoder(ImageFormat format)
        {
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
            {
                if (codec.FormatID.Equals(format.Guid))
                    return codec;
            }

            return null;
        }

        public static EncoderParameters getQualityEncoderParams(double quality)
        {
            return new EncoderParameters() { Param = new EncoderParameter[] { new EncoderParameter(Encoder.Quality, (long)(quality * 100)) } };
        }

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        private enum DeviceCap
        {
            HORZRES = 8,
            VERTRES = 10,
            DESKTOPVERTRES = 117,
            DESKTOPHORZRES = 118,

            // http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
        }
    }
}
