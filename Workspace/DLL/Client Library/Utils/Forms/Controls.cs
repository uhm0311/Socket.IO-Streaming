using System;
using System.Drawing;
using System.Windows.Forms;

namespace ClientLibrary.Utils.Forms
{
    public class Controls
    {
        private static Action<TextBox, string> setTextAction = new Action<TextBox, string>(setText);
        private static Action<Panel, PictureBox, Bitmap, double> setImageAction = new Action<Panel, PictureBox, Bitmap, double>(setImage);

        public static void setText(TextBox textBox, string text)
        {
            if (textBox != null)
            {
                if (!textBox.InvokeRequired)
                    textBox.Text = text;
                else textBox.Invoke(setTextAction, textBox, text);
            }
        }

        public static void setImage(Panel panel, PictureBox pictureBox, Bitmap image, double screenScaling)
        {
            if (pictureBox != null)
            {
                if (!pictureBox.InvokeRequired)
                {
                    if (pictureBox.Image != null)
                        pictureBox.Image.Dispose();

                    if (screenScaling > 0)
                    {
                        if (!(image.Width > panel.Width && image.Height > panel.Height))
                            setPictureBoxSizeMode(pictureBox, PictureBoxSizeMode.CenterImage);
                        else setPictureBoxSizeMode(pictureBox, PictureBoxSizeMode.AutoSize);
                    }
                    else if (screenScaling > -1)
                        setPictureBoxSizeMode(pictureBox, PictureBoxSizeMode.StretchImage);
                    else if (screenScaling > -2)
                        setPictureBoxSizeMode(pictureBox, PictureBoxSizeMode.Zoom);

                    if (pictureBox.SizeMode != PictureBoxSizeMode.AutoSize)
                    {
                        int width = panel.Width;
                        int height = panel.Height;

                        if (pictureBox.SizeMode == PictureBoxSizeMode.CenterImage)
                        {
                            if (image.Width > panel.Width || image.Height > panel.Height)
                            {
                                setPictureDockStyle(pictureBox, DockStyle.None);

                                width = Math.Max(image.Width, panel.Width);
                                height = Math.Max(image.Height, panel.Height);
                            }
                            else setPictureDockStyle(pictureBox, DockStyle.Fill);
                        }

                        pictureBox.Width = width;
                        pictureBox.Height = height;
                    }
                    else setPictureDockStyle(pictureBox, DockStyle.None);

                    pictureBox.Image = image;
                }
                else pictureBox.Invoke(setImageAction, panel, pictureBox, image, screenScaling);
            }
        }

        private static void setPictureBoxSizeMode(PictureBox pictureBox, PictureBoxSizeMode sizeMode)
        {
            if (pictureBox.SizeMode != sizeMode)
            {
                if (pictureBox.Image != null)
                    pictureBox.Image.Dispose();

                pictureBox.Image = null;
                pictureBox.SizeMode = sizeMode;
            }
        }

        private static void setPictureDockStyle(PictureBox pictureBox, DockStyle dockStyle)
        {
            if (pictureBox.Dock != dockStyle)
            {
                if (pictureBox.Image != null)
                    pictureBox.Image.Dispose();

                pictureBox.Image = null;
                pictureBox.Dock = dockStyle;
            }
        }
    }
}
