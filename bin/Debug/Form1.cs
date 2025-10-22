using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

// Add these two statements to all SimConnect clients
using Microsoft.FlightSimulator.SimConnect;
using System.Runtime.InteropServices;

namespace TCalc_004
{


    public partial class Form1 : Form
    {
        public Form1()
        {
            response = 1;
            output = "\n\n\n\n\n\n\n\n\n\n";
            InitializeComponent();
            setButtons(true, false);
        }

        private void button_Connect_Click(object sender, EventArgs e)
        {
            if (my_simconnect == null)
            {
                try
                {
                    my_simconnect = new Microsoft.FlightSimulator.SimConnect.SimConnect("Managed Data Request", base.Handle, 0x402, null, 0);
                    setButtons(false, true);
                    initDataRequest();
                    timer1.Enabled = true;
                }
                catch (COMException)
                {
                    label_status.Text = "Unable to connect to sim";
                }
            }
            else
            {
                label_status.Text = "Error - try again";
                closeConnection();
                setButtons(true, false);
                timer1.Enabled = false;
            }
        }

        private void button_Disconnect_Click(object sender, EventArgs e)
        {
            closeConnection();
            setButtons(true, false);
            timer1.Enabled = false;
            textBox_latitude.Text = "";
            textBox_longitude.Text = "";
            textBox_trueheading.Text = "";
            textBox_groundaltitude.Text = "";
            label_DirNumber.Text = "";
            label_FileNumber.Text = "";
        }

        private void closeConnection()
        {
            if (my_simconnect != null)
            {
                my_simconnect.Dispose();
                my_simconnect = null;
                label_status.Text = "Connection closed";
            }
        }

        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == 0x402)
            {
                if (my_simconnect != null)
                {
                    my_simconnect.ReceiveMessage();
                }
            }
            else
            {
                base.DefWndProc(ref m);
            }
        }

        private void displayText(string s)
        {
            output = output.Substring(output.IndexOf("\n") + 1);
            object obj1 = output;
            output = string.Concat(new object[] { obj1, "\n", response++, ": ", s });
            label_status.Text = output;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            closeConnection();
            timer1.Enabled = false;
        }

        private void initDataRequest()
        {
            try
            {
                my_simconnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(simconnect_OnRecvOpen);
                my_simconnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(simconnect_OnRecvQuit);
                my_simconnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(simconnect_OnRecvException);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Title", null, SIMCONNECT_DATATYPE.STRING256, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Latitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Longitude", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Plane Heading Degrees True", "degrees", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.AddToDataDefinition(DEFINITIONS.Struct1, "Ground Altitude", "meters", SIMCONNECT_DATATYPE.FLOAT64, 0.0f, SimConnect.SIMCONNECT_UNUSED);
                my_simconnect.RegisterDataDefineStruct<Struct1>(DEFINITIONS.Struct1);
                my_simconnect.OnRecvSimobjectDataBytype += new SimConnect.RecvSimobjectDataBytypeEventHandler(simconnect_OnRecvSimobjectDataBytype);
            }
            catch (COMException exception1)
            {
                displayText(exception1.Message);
            }
        }

        private void setButtons(bool bConnect, bool bDisconnect)
        {
            button_Connect.Enabled = bConnect;
            button_Disconnect.Enabled = bDisconnect;
            button_addHour.Enabled = bDisconnect;
            button_subHour.Enabled = bDisconnect;
        }

        private void simconnect_OnRecvException(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            label_status.Text = "Exception received: " + ((uint)data.dwException);
        }

        private void simconnect_OnRecvOpen(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            label_status.Text = "Connected to sim";
        }

        private void simconnect_OnRecvQuit(SimConnect sender, SIMCONNECT_RECV data)
        {
            label_status.Text = "sim has exited";
            closeConnection();
            timer1.Enabled = false;
        }

        private void simconnect_OnRecvSimobjectDataBytype(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA_BYTYPE data)
        {
            string DirNumberText = "";
            string DirNumber1Text = "";
            string DirNumber2Text = "";
            string FileNumberText = "";
            string FileNumber1Text = "";
            string FileNumber2Text = "";
            double DirNumber1 = 0;
            double DirNumber2 = 0;
            double FileNumber1 = 0;
            double FileNumber2 = 0;


            if (data.dwRequestID == 0)
            {
                Struct1 struct1 = (Struct1)data.dwData[0];
                // label_aircraft.Text = struct1.title.ToString();
                textBox_latitude.Text = struct1.latitude.ToString();
                textBox_longitude.Text = struct1.longitude.ToString();
                textBox_trueheading.Text = struct1.trueheading.ToString();
                textBox_groundaltitude.Text = struct1.groundaltitude.ToString();

                DirNumber1 = ((int)(((180.0 + (struct1.longitude)) * 12) / 360.0));
                if (DirNumber1 < 10)
                    DirNumber1Text = "0" + DirNumber1.ToString();
                else
                    DirNumber1Text = DirNumber1.ToString();
                DirNumber2 = ((int)(((90.0 - (struct1.latitude)) * 8) / 180.0));
                if (DirNumber2 < 10)
                    DirNumber2Text = "0" + DirNumber2.ToString();
                else
                    DirNumber2Text = DirNumber2.ToString();
                DirNumberText = DirNumber1Text + DirNumber2Text;

                FileNumber1 = ((int)(((180.0 + (struct1.longitude)) * 96) / 360.0));
                if (FileNumber1 < 10)
                    FileNumber1Text = "0" + FileNumber1.ToString();
                else
                    FileNumber1Text = FileNumber1.ToString();
                FileNumber2 = ((int)(((90.0 - (struct1.latitude)) * 64) / 180.0));
                if (FileNumber2 < 10)
                    FileNumber2Text = "0" + FileNumber2.ToString();
                else
                    FileNumber2Text = FileNumber2.ToString();
                FileNumberText = FileNumber1Text + FileNumber2Text;

                label_DirNumber.Text = DirNumberText;
                label_FileNumber.Text = FileNumberText;


            }
            else
            {
                label_status.Text = "Unknown request ID: " + ((uint)data.dwRequestID);
                textBox_latitude.Text = "";
                textBox_longitude.Text = "";
                textBox_trueheading.Text = "";
                textBox_groundaltitude.Text = "";
                label_DirNumber.Text = "";
                label_FileNumber.Text = "";
            }
        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
            my_simconnect.RequestDataOnSimObjectType(DATA_REQUESTS.REQUEST_1, DEFINITIONS.Struct1, 0, SIMCONNECT_SIMOBJECT_TYPE.USER);
            label_status.Text = "Request sent...";
        }


        private Microsoft.FlightSimulator.SimConnect.SimConnect my_simconnect;
        private string output;
        private int response;
        private const int WM_USER_SIMCONNECT = 0x402;


        private enum DATA_REQUESTS
        {
            REQUEST_1
        }

        private enum DEFINITIONS
        {
            Struct1
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct Struct1
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x100)]
            public string title;
            public double latitude;
            public double longitude;
            public double trueheading;
            public double groundaltitude;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true) Form1.ActiveForm.TopMost = true;
            else Form1.ActiveForm.TopMost = false;
        }

        private void button_addHour_Click(object sender, EventArgs e)
        {
            my_simconnect.MapClientEventToSimEvent((Enum)Form1.EVENTS.KEY_CLOCK_HOURS_INC, "CLOCK_HOURS_INC");
            my_simconnect.TransmitClientEvent(0U, (Enum)Form1.EVENTS.KEY_CLOCK_HOURS_INC, 1, (Enum)Form1.NOTIFICATION_GROUPS.GROUP0, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }

        private enum EVENTS
        {
            KEY_PAUSE_ON,
            KEY_PAUSE_OFF,
            KEY_CLOCK_HOURS_INC,
            KEY_CLOCK_HOURS_DEC,
        }

        private enum NOTIFICATION_GROUPS
        {
            GROUP0,
        }

        private void button_subHour_Click(object sender, EventArgs e)
        {
            my_simconnect.MapClientEventToSimEvent((Enum)Form1.EVENTS.KEY_CLOCK_HOURS_DEC, "CLOCK_HOURS_DEC");
            my_simconnect.TransmitClientEvent(0U, (Enum)Form1.EVENTS.KEY_CLOCK_HOURS_DEC, 1, (Enum)Form1.NOTIFICATION_GROUPS.GROUP0, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }
    }
}

