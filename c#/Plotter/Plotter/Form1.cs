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

                // Create the thread object, 
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

        private void btnDisconnect_Click(object sender, EventArgs e)
        {

            disconnect();
            btnConnect.Visible = true;
            btnDisconnect.Visible = false; 
        }

        public void disconnect()
        {
            //TODO some bugs on disconnect!!! threads and stuff
            if (arduinoCommand != null)
            {
                if(arduinoCommand.RunLoop)
                    arduinoCommand.Exit();
                arduinoCommand.RunLoop = false;
                Form1._RunThread = false;
                
               // arduinoCommand = null;
            }
            //loopThread.Join();
            
            //loopThread.Join();
           // loopThread = null;
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            disconnect();
        }

        private void edPosition_ValueChanged(object sender, EventArgs e)
        {
           
        }

        private void pnl_Draw_MouseUp(object sender, MouseEventArgs e)
        {
            startPaint = false;
            initX = null;
            initY = null;
            if (arduinoCommand == null)
                return;
            arduinoCommand.addCommandToSend(e.Location, false); //only move to that
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
            if (arduinoCommand == null)
                return;
            if (arduinoCommand.addCommandToSend(p, true))
                cbPoints.Items.Add("" + p.X + " " + p.Y);
        }

        private void pnl_Draw_MouseDown(object sender, MouseEventArgs e)
        {
            startPaint = true;
            if (arduinoCommand == null)
                return;
            if (arduinoCommand.isPenDown())
                return;
            arduinoCommand.addCommandToSend(e.Location, false); //only move to that
        }

        private void btnClearCanvas_Click(object sender, EventArgs e)
        {
            g.Clear(pnl_Draw.BackColor);
            pnl_Draw.BackColor = Color.White;
            cbPoints.Items.Clear();
            if (arduinoCommand == null)
                return;
            arduinoCommand.commands.Clear();
            arduinoCommand.setPosition(new Point(0,0));
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            btnDisconnect_Click(sender, e);

            using(ControllerForm cf = new ControllerForm(cbPorts.SelectedItem.ToString(), getBaud()))
            {
                cf.ShowDialog();
            }
        }

        private void pnl_Draw_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
