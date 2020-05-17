using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace ClientLibrary.Utils.Forms
{
    public abstract partial class PrimaryClient : Form
    {
        protected static readonly string processName = Process.GetCurrentProcess().ProcessName;
        private static readonly string settingsFilePath = AppDomain.CurrentDomain.BaseDirectory + "settings.dat";

        private System.Timers.Timer notificationTimer = new System.Timers.Timer() { AutoReset = false };
        private DateTime lastNotification = DateTime.UtcNow;

        private int receivedScreenCount = 0;
        private System.Timers.Timer fpsCounter = new System.Timers.Timer(1000) { AutoReset = true };
        private Action fpsCounterAction;

        private float screenScale = -2;

        private JObject settings = null;

        protected PrimaryClient()
        {
            InitializeComponent();
            
            this.Text = processName;
            this.nickname = defaultNickname;
            this.port = defaultPort;

            List<MenuItem> items = new List<MenuItem>();
            items.Add(new MenuItem("Open", new EventHandler(notifyIcon_Open_Click)));
            items.Add(new MenuItem("Exit", new EventHandler(notifyIcon_Exit_Click)));

            notifyIcon.Icon = Properties.Resources.icon;
            notifyIcon.ContextMenu = new ContextMenu(items.ToArray());

            items.Clear();
            items.Add(new MenuItem("Maintain aspect ratio", new EventHandler(screen_MaintainAspectRatio_Click)) { Checked = true });
            items.Add(new MenuItem("Stretch", new EventHandler(screen_Stretch_Click)) { Checked = false });
            items.Add(new MenuItem("Zoom", new EventHandler(screen_Zoom_Click)) { Checked = false });

            screen.ContextMenu = new ContextMenu(items.ToArray());

            focusEater.Dock = DockStyle.None;
            focusEater.Size = new System.Drawing.Size(0, 0);

            connectionState.GotFocus += connectionState_GotFocus;
            screen.MouseWheel += screen_MouseWheel;
            notificationTimer.Elapsed += (o, e) => toggleNotifyIcon();

            fpsCounterAction = new Action(() => { fpsLabel.Text = "FPS : " + receivedScreenCount; receivedScreenCount = 0; });
            fpsCounter.Elapsed += (o, e) => this.BeginInvoke(fpsCounterAction);

            loadSettings();
            fpsCounter.Start();
        }

        private void loadSettings()
        {
            string settingsString = string.Empty;
            string defaultSettings = string.Format(@"{{ 'host': '{0}', 'port': {1}, 'nickname': '{2}',  'screenMode': {3} }}", host, port, nickname, 0);

            if (File.Exists(settingsFilePath))
                settingsString = File.ReadAllText(settingsFilePath);
            else settingsString = defaultSettings;

            try { settings = JsonConvert.DeserializeObject<JObject>(settingsString); }
            catch { settings = JsonConvert.DeserializeObject<JObject>(defaultSettings); }

            host = settings[JsonKeys.host].ToString();
            setPort(settings[JsonKeys.port].ToString());
            if (!defaultNickname.Equals(EmitEvents.master))
                nickname = settings[JsonKeys.nickname].ToString();

            int screenMode;
            if (!int.TryParse(settings[JsonKeys.screenMode].ToString(), out screenMode) || (screenMode < 0 || screenMode > screen.ContextMenu.MenuItems.Count))
                screenMode = 0;
            uncheckScreenMenuItems(screenMode);
        }

        private void saveSettings()
        {
            settings[JsonKeys.host] = host;
            settings[JsonKeys.port] = port;
            settings[JsonKeys.nickname] = nickname;

            for (int i = 0; i < screen.ContextMenu.MenuItems.Count; i++)
            {
                if (screen.ContextMenu.MenuItems[i].Checked)
                {
                    settings[JsonKeys.screenMode] = i;
                    break;
                }
            }

            File.WriteAllText(settingsFilePath, settings.ToString());
        }

        protected void setConnectionState(string text)
        {
            Utils.Forms.Controls.setText(this.connectionState, text);
        }

        protected void setScreenImage(Bitmap image = null)
        {
            float screenScaling = this.screenScale;

            if (image == null)
            {
                image = Properties.Resources.image;
                screenScaling = -1;
            }
            else receivedScreenCount++;

            if (screenScaling > 0 && screenScaling != screenResize)
            {
                Bitmap resized = ScreenCapture.resize(image, screenScaling / screenResize);

                image.Dispose();
                image = resized;
            }

            Utils.Forms.Controls.setImage(this.screenPanel, this.screen, image, screenScaling);
        }

        protected void showNotification(int timeout, string title, string text, ToolTipIcon icon)
        {
            new Thread(() =>
            {
                toggleNotifyIcon();

                notifyIcon.ShowBalloonTip(timeout, title, text, icon);
                lastNotification = DateTime.UtcNow;

                notificationTimer.Interval = timeout;
                notificationTimer.Start();
            }) { IsBackground = true }.Start();
        }

        private void toggleNotifyIcon(int sleep = 100)
        {
            toggleNotifyIcon(false, sleep);
            toggleNotifyIcon(true, sleep);
        }

        private void toggleNotifyIcon(bool b, int sleep)
        {
            this.notifyIcon.Visible = b;
            if (sleep > 0)
                Thread.Sleep(10);
        }

        private void uncheckScreenMenuItems(int exception)
        {
            if (exception >= 0 && exception < screen.ContextMenu.MenuItems.Count)
            {
                for (int i = 0; i < screen.ContextMenu.MenuItems.Count; i++)
                    screen.ContextMenu.MenuItems[i].Checked = false;

                screen.ContextMenu.MenuItems[exception].Checked = true;
                screenScale = exception - 1;
            }
        }
    }
}
