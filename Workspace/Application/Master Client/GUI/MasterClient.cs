using ClientLibrary.Utils;
using ClientLibrary.Utils.Forms;
using Newtonsoft.Json.Linq;
using SocketIOSharp.Client;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MasterClient.GUI
{
    public partial class MasterClient : PrimaryClient
    {
        private delegate void SetListCallback(string[] list);

        protected override string defaultNickname { get { return EmitEvents.master; } }
        private ListBox memberList = new ListBox();

        public MasterClient(string[] args) : base()
        {
            if (args.Length > 0)
                setPort(args[0]);
                
            this.memberList.Dock = System.Windows.Forms.DockStyle.Left;
            this.memberList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.memberList.FormattingEnabled = true;
            this.memberList.ItemHeight = 12;
            this.memberList.Location = new System.Drawing.Point(0, 21);
            this.memberList.Name = "memberList";
            this.memberList.Size = new System.Drawing.Size(114, 459);
            this.memberList.TabIndex = 6;
            this.memberList.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.memberList_DrawItem);
            this.memberList.SelectedIndexChanged += new System.EventHandler(this.memberList_SelectedIndexChanged);

            this.Controls.Add(this.memberList);
        }

        private void memberList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (memberList.SelectedItem != null)
            {
                selectedMember = memberList.SelectedItem.ToString();

                if (!nickname.Equals(selectedMember))
                    timer.Start();
                else base.setScreenImage();
            }
            else this.clearMemberSelection();
        }

        private void memberList_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                e.DrawBackground();
                e.Graphics.DrawString(memberList.Items[e.Index].ToString(), e.Index == 0 ? new Font(SystemFonts.DefaultFont.FontFamily, 9, FontStyle.Bold) : new Font(SystemFonts.DefaultFont.FontFamily, 9), Brushes.Black, e.Bounds);
                e.DrawFocusRectangle();
            }
        }

        protected override void connectionState_GotFocus(object sender, EventArgs e)
        {
            if (memberList.Items.Count > 1)
            {
                memberList.Focus();
                memberList.Select();
            }
            else base.connectionState_GotFocus(sender, e);
        }

        private void setList(string[] list)
        {
            if (this.memberList.InvokeRequired)
            {
                SetListCallback d = new SetListCallback(setList);
                this.Invoke(d, new object[] { list });
            }
            else
            {
                memberList.Items.Clear();
                foreach (string item in list)
                    memberList.Items.Add(item);

                memberList.SelectedItem = selectedMember;
            }
        }

        protected override void initExtraCallbacks()
        {
            client.On(EmitEvents.members, (Data) =>
            {
                if (Data != null && Data.Length > 0 && Data[0] != null && Data[0].Type == JTokenType.Array)
                {
                    int count = Data[0].Count();

                    List<string> list = new List<string>();
                    list.Add(nickname);

                    for (int i = 0; i < count; i++)
                    {
                        string temp = Data[0][i][JsonKeys.nickname].ToString();

                        if (!list.Contains(temp))
                            list.Add(temp);
                    }

                    if (!list.Contains(selectedMember))
                        this.clearMemberSelection();

                    this.setList(list.ToArray());
                }

                timer.Start();
            });
        }

        protected override void clearMemberSelection()
        {
            selectedMember = null;
            base.clearMemberSelection();
        }
    }
}
