using ClientLibrary.Utils;
using ClientLibrary.Utils.Forms;
using Newtonsoft.Json.Linq;
using SocketIOSharp.Client;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace StreamingClient.GUI
{
    public class StreamingClient : PrimaryClient
    {
        protected override string defaultNickname { get { return Environment.UserName; } }

        public StreamingClient() : base()
        {
            base.selectedMember = EmitEvents.master;
        }

        protected override void initExtraCallbacks()
        {
            client.On(EmitEvents.login + EmitEvents.master, (Data) =>
            {
                timer.Start();
            });

            client.On(EmitEvents.logout + EmitEvents.master, (Data) =>
            {
                base.setScreenImage();
            });
        }
    }
}
