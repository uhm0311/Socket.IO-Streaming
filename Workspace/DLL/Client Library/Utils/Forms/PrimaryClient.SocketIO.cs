using Newtonsoft.Json.Linq;
using SocketIOSharp.Client;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace ClientLibrary.Utils.Forms
{
    partial class PrimaryClient
    {
        protected SocketIOClient client = null;
        protected string host = IPAddress.Loopback.ToString();
        
        protected const ushort defaultPort = 9104;
        protected ushort port = 0;

        protected abstract string defaultNickname { get; }
        protected string nickname = null;

        protected string selectedMember = null;
        protected System.Timers.Timer timer = new System.Timers.Timer(1) { AutoReset = false };

        protected Dictionary<string, DateTime> mouseAppearances = new Dictionary<string, DateTime>();

        private float screenResize = 0.75F;
        private ImageCodecInfo jpgEncoder = ScreenCapture.getEncoder(ImageFormat.Jpeg);
        private EncoderParameters encoderParams = ScreenCapture.getQualityEncoderParams(0.5);

        private void initClient()
        {
            this.setConnectionState("연결중...");

            client = new SocketIOClient(SocketIOClient.Scheme.ws, host, port, false);
            initCallbacks();
            client.Connect();
        }

        private void initCallbacks()
        {
            initPrimaryCallbacks();
            initCustomCallbacks();
            initExtraCallbacks();
        }

        private void initPrimaryCallbacks()
        {
            if (client == null)
                initClient();

            client.On(SocketIOClient.Event.CONNECTION, (Data) =>
            {
                this.setConnectionState("서버에 연결되었습니다.");

                JToken json = new JObject();
                json[JsonKeys.nickname] = nickname;

                client.Emit(EmitEvents.login + (defaultNickname.Equals(EmitEvents.master) ? EmitEvents.master : string.Empty), json);

                timer.Elapsed += (o, e) =>
                {
                    json[JsonKeys.selectedMember] = selectedMember;
                    client.Emit(EmitEvents.screenrequest, json);
                };

                timer.Start();
            });

            client.On(SocketIOClient.Event.DISCONNECT, (Data) =>
            {
                this.setConnectionState("연결이 해제되었습니다. 서버의 상태를 확인하세요.");
                this.clearMemberSelection();
            });

            client.On(SocketIOClient.Event.ERROR, (Data) =>
            {
                string Message = string.Empty;
                if (Data != null && Data.Length > 0 && Data[0] != null)
                    Message = Data[0].ToString();

                if (!string.IsNullOrWhiteSpace(Message))
                {
                    this.setConnectionState(Message);
                    MessageBox.Show(Message, processName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        private void initCustomCallbacks()
        {
            if (client == null)
                initClient();

            client.On(EmitEvents.logout, (Data) =>
            {
                if (Data != null && Data.Length > 0 && Data[0] != null && !string.IsNullOrWhiteSpace(Data[0].ToString()))
                {
                    this.client.AutoReconnect = false;
                    this.client.Close();

                    MessageBox.Show(Data[0].ToString() + "\r\n연결을 해제합니다.", processName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.setConnectionState(Data[0].ToString());
                }
            });

            client.On(EmitEvents.screenrequest, (Data) =>
            {
                using (MemoryStream stream = new MemoryStream())
                using (Bitmap screenshot = ScreenCapture.shoot())
                using (Bitmap compressed = ScreenCapture.compress(screenshot, screenResize, false, 1.25F))
                {
                    compressed.Save(stream, jpgEncoder, encoderParams);

                    JToken json = new JObject();
                    json[JsonKeys.nickname] = nickname;
                    json[JsonKeys.image] = stream.ToArray();

                    client.Emit(EmitEvents.screen, json);
                }
            });

            client.On(EmitEvents.screen, (Data) =>
            {
                if (Data != null && Data.Length > 0 && Data[0] != null)
                {
                    if (!string.IsNullOrWhiteSpace(selectedMember) && Data[0][JsonKeys.nickname].ToString().Equals(selectedMember))
                    {
                        if (Data[0][JsonKeys.image] != null)
                        {
                            using (MemoryStream stream = new MemoryStream((byte[])Data[0][JsonKeys.image]))
                                this.setScreenImage(new Bitmap(stream));
                        }
                    }
                }

                timer.Start();
            });

            client.On(EmitEvents.mouseclick, (Data) =>
            {
                if (Data != null && Data.Length > 0 && Data[0] != null && Data[0][JsonKeys.nickname] != null)
                {
                    string temp = Data[0][JsonKeys.nickname].ToString();
                    const int timeout = 2000;

                    lock (mouseAppearances)
                    {
                        bool showBalloon = true;

                        if (!mouseAppearances.ContainsKey(temp))
                            mouseAppearances.Add(temp, DateTime.UtcNow);
                        else if (Math.Abs(mouseAppearances[temp].Subtract(DateTime.UtcNow).TotalMilliseconds) <= timeout / 2)
                            showBalloon = false;

                        if (showBalloon)
                        {
                            mouseAppearances[temp] = DateTime.UtcNow;

                            StringBuilder builder = new StringBuilder();
                            foreach (string key in mouseAppearances.Keys)
                            {
                                if (Math.Abs(mouseAppearances[key].Subtract(DateTime.UtcNow).TotalMilliseconds) <= timeout)
                                    builder.AppendLine(key);
                            }

                            showNotification(timeout, "Mouse Click", builder.ToString(), ToolTipIcon.Info);
                        }
                    }
                }
            });
        }

        protected abstract void initExtraCallbacks();

        protected virtual void clearMemberSelection()
        {
            this.setScreenImage();
        }

        protected void setPort(string portString)
        {
            if (!ushort.TryParse(portString, out port))
                port = defaultPort;
        }
    }
}