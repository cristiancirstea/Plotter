using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArduinoCommand;
using System.Threading;

namespace Plotter
{
    public partial class Form1 : Form
    {
        static SerialPort port;
        static ArduinoCommand.Command arduinoCommand;
        Thread loopThread;
        public static bool _RunThread;
        bool startPaint = false;
        Graphics g;
        //nullable int for storing Null value
        int? initX = null;
        int? initY = null;

        public Form1()
        {
            InitializeComponent();
            populatePorts();
            g = pnl_Draw.CreateGraphics();
        }

        public int getBaud()
        {
            return (int)edBaud.Value;
        }

        public string[] getPorts()
        {
            return SerialPort.GetPortNames();
        }

        public string getSelectedPort()
        {
            return cbPorts.Items[cbPorts.SelectedIndex].ToString();
        }

        private void populatePorts()
        {
            string[] ports = getPorts();
            cbPorts.Items.AddRange(ports);
            if(ports.Length > 0)
                cbPorts.SelectedIndex = 0;
        }

        public static void runLoop()
        {
            while ((arduinoCommand.RunLoop))
            {
                arduinoCommand.Loop();
            }
            if (!Form1._RunThread)
                arduinoCommand.Exit();
            Form1._RunThread = false;
        }

        public bool connect()
        {
            try
            {
                string portName = getSelectedPort();
                int baud = getBaud();
                arduinoCommand = new ArduinoCommand.Command { RunLoop = true, PortName = portName, BaundRate = baud };
                arduinoCommand.Setup();

                // Create the thread object, passing in the Alpha.Beta method
                // via a ThreadStart delegate. This does not start the thread.
                loopThread = new Thread(new ThreadStart(Form1.runLoop));
                Form1._RunThread = true;
                // Start the thread 
                loopThread.Start();
                // Spin for a while waiting for the started thread to become
                // alive:
                 while (!loopThread.IsAlive) ;
                // Put the Main thread to sleep for 1 millisecond to allow oThread
                // to do some work:
                Thread.Sleep(1);

                return true;
            }
            catch(Exception exception)
            {
                MessageBox.Show(exception.Message);
                return false;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            bool connected = connect();
            if(connected)
            {
                btnConnect.Visible = false;
                btnDisconnect.Visible = true; 
            }
        }

        //delegate void AddTextCallback(string text);

        //private void AddTextRecived(string text)
        //{
        //    // InvokeRequired required compares the thread ID of the
        //    // calling thread to the thread ID of the creating thread.
        //    // If these threads are different, it returns true.
        //    if (this.tbRecived.InvokeRequired)
        //    {
        //        AddTextCallback d = new AddTextCallback(AddTextRecived);
        //        this.Invoke(d, new object[] { text });
        //    }
        //    else
        //    {
        //        this.tbRecived.AppendText(text);
                
        //    }
        //}

        private void btnDisconnect_Click(object sender, EventArgs e)
        {

            disconnect();
            btnConnect.Visible = true;
            btnDisconnect.Visible = false; 
        }

        public void disconnect()
        {
            if (arduinoCommand != null)
            {
                arduinoCommand.RunLoop = false;
               // arduinoCommand = null;
            }
            //loopThread.Join();
            Form1._RunThread = false;
            arduinoCommand.Exit();
            //loopThread.Join();
           // loopThread = null;
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            disconnect();
        }

        private void edPosition_ValueChanged(object sender, EventArgs e)
        {
            if (arduinoCommand != null)
                arduinoCommand.setPosition(new Point(((int)edPosition.Value), ((int)numericUpDown1.Value)));
        }

        private void pnl_Draw_MouseUp(object sender, MouseEventArgs e)
        {
            startPaint = false;
            initX = null;
            initY = null;
        }

        private void pnl_Draw_MouseMove(object sender, MouseEventArgs e)
        {
            if (startPaint)
            {
                //Setting the Pen BackColor and line Width
                Pen p = new Pen(Color.Black, 1);
                //Drawing the line.
                g.DrawLine(p, new Point(initX ?? e.X, initY ?? e.Y), new Point(e.X, e.Y));
                initX = e.X;
                initY = e.Y;

                addPoint(new Point(e.X, e.Y));
            }
        }

        public void addPoint(Point p)
        {
            if (arduinoCommand.addCommandToSend(p, true))
                cbPoints.Items.Add("" + p.X + " " + p.Y);
        }

        private void pnl_Draw_MouseDown(object sender, MouseEventArgs e)
        {
            startPaint = true;
        }

        private void btnClearCanvas_Click(object sender, EventArgs e)
        {
            g.Clear(pnl_Draw.BackColor);
            //Setting the BackColor of pnl_draw and btn_CanvasColor to White on Clicking New under File Menu
            pnl_Draw.BackColor = Color.White;
            arduinoCommand.commands.Clear();
            arduinoCommand.setPosition(new Point(0,0));
            cbPoints.Items.Clear();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (arduinoCommand != null)
                arduinoCommand.setPosition(new Point(((int)edPosition.Value), ((int)numericUpDown1.Value)));
        }
    }
}
