using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArduinoController;

namespace Plotter
{
    public partial class ControllerForm : Form
    {
        private readonly ArduinoController.ArduinoController _arduinoController;

        public ControllerForm()
        {
            InitializeComponent();
            _arduinoController = new ArduinoController.ArduinoController();
            _arduinoController.Setup(this);
        }

        public ControllerForm(string portName, int baud)
        {

            InitializeComponent();
            _arduinoController = new ArduinoController.ArduinoController();
            _arduinoController.portName = portName;
            _arduinoController.baud = baud;
            _arduinoController.Setup(this);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public void setMotorXPosition(int value)
        {
            edPositionX.Value = value;
        }

        public void setMotorYPosition(int value)
        {
            edPositionY.Value = value;
        }

        private void edPositionX_ValueChanged(object sender, EventArgs e)
        {
            _arduinoController.SendMotorXPosition((int)edPositionX.Value);
        }

        private void edPositionY_ValueChanged(object sender, EventArgs e)
        {
            _arduinoController.SendMotorYPosition((int)edPositionY.Value);
        }

        public void log(String message)
        {
            tbLog.AppendText(message + "\n");
        }

        private void ControllerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _arduinoController.Exit();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _arduinoController.SendPenDown(checkBox1.Checked);
        }

        private void ControllerForm_Load(object sender, EventArgs e)
        {

        }
    }
}
