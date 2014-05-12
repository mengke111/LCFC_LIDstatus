using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using System.Management;
using System.Security.Principal;
using System.Diagnostics;

using Microsoft.Win32;
using System.Timers;
using System.Runtime.InteropServices;


namespace LCFC_LIDstatus
{
    public partial class Form1 : Form
    {
        [DllImport(@"User32", SetLastError = true,
          EntryPoint = "RegisterPowerSettingNotification",
          CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr RegisterPowerSettingNotification(
           IntPtr hRecipient,
           ref Guid PowerSettingGuid,
           Int32 Flags);

        
            

                static Guid GUID_LIDSWITCH_STATE_CHANGE =
            new Guid(0xBA3E0F4D, 0xB817, 0x4094, 0xA2, 0xD1, 
                     0xD5, 0x63, 0x79, 0xE6, 0xA0, 0xF3);

        static Guid GUID_LIDCLOSE_ACTION =
            new Guid(0xBA3E0F4D, 0xB817, 0x4094, 0xA2, 0xD1,
                     0xD5, 0x63, 0x79, 0xE6, 0xA0, 0xF3);

        private const int WM_POWERBROADCAST = 0x0218;
        private const int DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000;
        const int PBT_POWERSETTINGCHANGE = 0x8013; // DPPE

        //[StructLayout(LayoutKind.Sequential, Pack = 4)]
        //internal struct POWERBROADCAST_SETTING
        //{
        //    public Guid PowerSetting;
        //    public uint DataLength;
        //    public byte Data;
        //}

       
        public Form1()
        {
            InitializeComponent();
            RegisterForPowerNotifications();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        internal struct POWERBROADCAST_SETTING
        {
            public Guid PowerSetting;
            public uint DataLength;
            public byte Data;    // (*1)
        }

        private void RegisterForPowerNotifications()
        {
            IntPtr hWnd = this.Handle;
            IntPtr ret = RegisterPowerSettingNotification(hWnd,
                                   ref GUID_LIDCLOSE_ACTION,
                                   DEVICE_NOTIFY_WINDOW_HANDLE);

  
        }


        void  OpenOrClose(bool k)
        {
            if(k){
          label2.Visible=false;
          label3.Visible=true;
          richTextBox1.AppendText(System.DateTime.Now.ToString() + " LID  Close\n");
            }else{
          label2.Visible = true;
          label3.Visible=false;
          richTextBox1.AppendText(System.DateTime.Now.ToString() + " LID  Open\n");
            }
        }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
          //  MessageBox.Show("Message Receivedlllll!!!!!");
            // Listen for operating system messages.
            switch (m.Msg)
            {
                // The WM_ACTIVATEAPP message occurs when the application
                // becomes the active application or becomes inactive.
                case WM_POWERBROADCAST:
                    //   m.LParam
                    if ( m.WParam.ToInt32() == PBT_POWERSETTINGCHANGE)
                    {
                        // Extract data from message
                        POWERBROADCAST_SETTING ps =
                         (POWERBROADCAST_SETTING)Marshal.PtrToStructure(
                             m.LParam, typeof(POWERBROADCAST_SETTING));
                        IntPtr pData = (IntPtr)(m.LParam.ToInt32() + Marshal.SizeOf(ps));  // (*1)
                        if (ps.PowerSetting == GUID_LIDCLOSE_ACTION)
                        {
                            int i = ps.Data;
                            
                            if (i == 0)
                            {
                                OpenOrClose(true);

                            }
                            else {
                                OpenOrClose(false);
                            }
                            //int k;
                            //MessageBox.Show("GUID_LIDchanged_ACTION"+i);

                        }


                    }

                   // MessageBox.Show("Message Received!!!!!");
                    // The WParam value identifies what is occurring.
                 //   appActive = (((int)m.WParam != 0));

                    // Invalidate to get new text painted.
                    this.Invalidate();

                    break;
            }
            base.WndProc(ref m);
        }


    }
}
