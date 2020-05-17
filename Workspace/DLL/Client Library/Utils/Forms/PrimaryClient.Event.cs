using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace ClientLibrary.Utils.Forms
{
    partial class PrimaryClient
    {
        private Point startPoint = new Point(-1, -1);

        private void PrimaryClient_Load(object sender, EventArgs e)
        {
            DialogResult dialogResult;

            string[] promptText = new string[] { "호스트 주소 입력 : ", "포트 번호 입력 : ", "사용할 닉네임 입력 : " };
            object[] defaultValue = new object[] { host, port, nickname };
            bool[] disableInput = new bool[] { false, false, nickname.Equals(EmitEvents.master) };
            object[] inputs = Dialog.InputBox(processName, promptText, defaultValue, disableInput, out dialogResult);

            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                host = (string)inputs[0];
                port = (ushort)inputs[1];
                nickname = (string)inputs[2];

                new Thread(() => initClient()) { IsBackground = true }.Start();
            }
            else
            {
                MessageBox.Show("프로그램을 종료합니다.", processName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }

        private void PrimaryClient_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (client != null)
                client.Close();
            notifyIcon.Visible = false;

            saveSettings();
        }

        private void PrimaryClient_Resize(object sender, EventArgs e)
        {
            if (base.WindowState.Equals(FormWindowState.Minimized))
                base.ShowInTaskbar = false;
        }

        private void PrimaryClient_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            int verticalChange = (int)(screenPanel.VerticalScroll.LargeChange * (this.Height / (this.screen.Image.Height / screenResize)));
            int horizontalChange = (int)(screenPanel.HorizontalScroll.LargeChange * (this.Width / (this.screen.Image.Width / screenResize)));

            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (screenPanel.VerticalScroll.Value - verticalChange <= screenPanel.VerticalScroll.Minimum)
                        verticalChange = screenPanel.VerticalScroll.Value - screenPanel.VerticalScroll.Minimum;
                    screenPanel.VerticalScroll.Value -= verticalChange; break;

                case Keys.Down:
                    if (screenPanel.VerticalScroll.Value + verticalChange >= screenPanel.VerticalScroll.Maximum)
                        verticalChange = screenPanel.VerticalScroll.Maximum - screenPanel.VerticalScroll.Value;
                    screenPanel.VerticalScroll.Value += verticalChange; break;

                case Keys.Left:
                    if (screenPanel.HorizontalScroll.Value - horizontalChange <= screenPanel.HorizontalScroll.Minimum)
                        horizontalChange = screenPanel.HorizontalScroll.Value - screenPanel.HorizontalScroll.Minimum;
                    screenPanel.HorizontalScroll.Value -= horizontalChange; break;

                case Keys.Right:
                    if (screenPanel.HorizontalScroll.Value + horizontalChange >= screenPanel.HorizontalScroll.Maximum)
                        horizontalChange = screenPanel.HorizontalScroll.Maximum - screenPanel.HorizontalScroll.Value;
                    screenPanel.HorizontalScroll.Value += horizontalChange; break;
            }
        }

        protected virtual void connectionState_GotFocus(object sender, EventArgs e)
        {
            focusEater.Focus();
            focusEater.Select();
            focusEater.SelectAll();
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            notifyIcon_Open_Click(sender, e);
        }

        private void notifyIcon_Open_Click(object sender, EventArgs e)
        {
            base.ShowInTaskbar = true;
            base.WindowState = FormWindowState.Normal;
        }

        private void notifyIcon_Exit_Click(object sender, EventArgs e)
        {
            base.Close();
        }

        private void screen_MaintainAspectRatio_Click(object sender, EventArgs e)
        {
            uncheckScreenMenuItems(0);
        }

        private void screen_Stretch_Click(object sender, EventArgs e)
        {
            uncheckScreenMenuItems(1);
        }

        private void screen_Zoom_Click(object sender, EventArgs e)
        {
            uncheckScreenMenuItems(2);
        }

        private void screen_DoubleClick(object sender, EventArgs e)
        {
            JToken json = new JObject();
            json[JsonKeys.nickname] = nickname;
            json[JsonKeys.selectedMember] = selectedMember;

            client.Emit(EmitEvents.mouseclick, json);
        }

        private void screen_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (startPoint.X >= 0 && startPoint.Y >= 0)
                {
                    Point changePoint = new Point(e.Location.X - startPoint.X, e.Location.Y - startPoint.Y);
                    screenPanel.AutoScrollPosition = new Point(-screenPanel.AutoScrollPosition.X - changePoint.X, -screenPanel.AutoScrollPosition.Y - changePoint.Y);
                }
                else startPoint = e.Location;
            }
        }

        private void screen_MouseUp(object sender, MouseEventArgs e)
        {
            startPoint.X = startPoint.Y = -1;
        }

        private void screen_MouseWheel(object sender, MouseEventArgs e)
        {
            if (screenScale > 0 && (ModifierKeys == Keys.Control || ModifierKeys == Keys.ControlKey || ModifierKeys == Keys.LControlKey))
            {
                ((HandledMouseEventArgs)e).Handled = true;

                if (e.Delta > 0 && screenScale < 1.5)
                    screenScale = (float)Math.Round(screenScale + 0.1F, 1);
                else if (e.Delta < 0 && screenScale > 0.5)
                    screenScale = (float)Math.Round(screenScale - 0.1F, 1);
            }
        }
    }
}
