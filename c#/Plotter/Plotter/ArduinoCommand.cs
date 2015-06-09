// This 1st example will make the PC toggle the integrated led on the arduino board. 
// It demonstrates how to:
// - Define commands
// - Set up a serial connection
// - Send a command with a parameter to the Arduino


using System;
using System.Threading;
using CommandMessenger;
using CommandMessenger.TransportLayer;
using System.Collections.Generic;
using System.Drawing;

namespace ArduinoCommand
{
    enum CommandType
    {
        Acknowledge,            // Command to acknowledge a received command
        Error,                  // Command to message that an error has occurred
        MoveToX,                 // Command to move on OX 
        MoveToY,         // Command to move on OY 
        SetPen,               // Command to move on move pen
        Status
    };

    public struct CommandData
    {
        public Point point;
        public bool isPenDown;

        public CommandData(Point p, bool isPD)
        {
            point = p;
            isPenDown = isPD;
        }
    }

    public class Command
    {
        public const int SCALE_X = 2;
        public const int SCALE_Y = 2 ;
        public const int TRESHOLD_X = 2;
        public const int TRESHOLD_Y = 2;

        public bool RunLoop { get; set; }
        private SerialTransport _serialTransport;
        private CmdMessenger _cmdMessenger;
        private Point _stepperPosition;
        private bool _isPenDown = false;

        public List<CommandData> commands = new List<CommandData>();
        public  int lastCommandSend = 0;

        public string PortName { get; set; }
        public int BaundRate { get; set; }

        //TODO
        public void setPosition(Point value)
        {
            addCommandToSend(value, false);
        }

        // Setup function
        public void Setup()
        {
            _stepperPosition = new Point(0,0);

            // Create Serial Port object
            _serialTransport = new SerialTransport();
            _serialTransport.CurrentSerialSettings.PortName = PortName;//"COM10";    // Set com port
            _serialTransport.CurrentSerialSettings.BaudRate = BaundRate;//9600;     // Set baud rate
            _serialTransport.CurrentSerialSettings.DtrEnable = false;     // For some boards (e.g. Sparkfun Pro Micro) DtrEnable may need to be true.
            
            // Initialize the command messenger with the Serial Port transport layer
            _cmdMessenger = new CmdMessenger(_serialTransport);

            // Tell CmdMessenger if it is communicating with a 16 or 32 bit Arduino board
            _cmdMessenger.BoardType = BoardType.Bit16;
            
            // Attach the callbacks to the Command Messenger
            AttachCommandCallBacks();
            
            // Start listening
            _cmdMessenger.StartListening();                                
        }

        private Point scalePoint(Point p)
        {
            return new Point(p.X * SCALE_X, p.Y * SCALE_Y);
        }

        // Loop function
        public void Loop()
        {
            CommandData commandData = getCommandToSend();

            if (_stepperPosition != scalePoint(commandData.point))
            {
                _stepperPosition = scalePoint(commandData.point);

                // Create command
                var command = new SendCommand(
                        (int)CommandType.MoveToX
                    );
                command.AddArgument(_stepperPosition.X);
                // Send commands
                _cmdMessenger.SendCommand(command);
                Thread.Sleep(100);

                var commandY = new SendCommand(
                        (int)CommandType.MoveToY
                    );
                commandY.AddArgument(_stepperPosition.Y);

                _cmdMessenger.SendCommand(commandY);
                Thread.Sleep(100);
            }
            if (_isPenDown != commandData.isPenDown)
            {
                _isPenDown = commandData.isPenDown;
                var commandPen = new SendCommand(
                        (int)CommandType.SetPen
                    );
                commandPen.AddArgument(_isPenDown);

                _cmdMessenger.SendCommand(commandPen);
                Thread.Sleep(200);
            }
        }

        public CommandData getCommandToSend()
        {
            if (this.commands.Count == 0)
            {
                lastCommandSend = 0;
                CommandData cmd = new CommandData(new Point(0,0), false);
                return cmd;
            }
            CommandData cd = this.commands[lastCommandSend];
            if (this.commands.Count > (lastCommandSend + 2))
            {
                this.lastCommandSend++;
            }
            else
            {
                cd.isPenDown = false;
            }
            //else keep last command but pen up

            return cd;
        }

        public bool isPenDown()
        {
            return this._isPenDown;
        }

        public bool addCommandToSend(Point p, bool isPenDown = false)
        {
            if (this.commands.Count == 0)
            {
                lastCommandSend = 0;
                CommandData cmd = new CommandData(new Point(0, 0), false);
                this.commands.Add(cmd);
                return true;
            }
            if (
                    (Math.Abs(p.X - this.commands[this.commands.Count - 1].point.X) > TRESHOLD_X)
                    || (Math.Abs(p.Y - this.commands[this.commands.Count - 1].point.Y) > TRESHOLD_Y)
                )
            {
                this.commands.Add(new CommandData(p, isPenDown));
                return true;
            }
            return false;
        }

        // Exit function
        public void Exit()
        {
            // Stop listening
            _cmdMessenger.StopListening();

            // Dispose Command Messenger
            _cmdMessenger.Dispose();

            // Dispose Serial Port object
            _serialTransport.Dispose();
        }

        /// Attach command call backs. 
        private void AttachCommandCallBacks()
        {
            _cmdMessenger.Attach(OnUnknownCommand);

        }

        // Callback function that prints that the Arduino has acknowledged
        void OnAcknowledge(ReceivedCommand arguments)
        {
         //  System.Windows.Forms.MessageBox.Show(" Arduino is ready");
        }

        // Callback function that prints that the Arduino has experienced an error
        void OnError(ReceivedCommand arguments)
        {
            System.Windows.Forms.MessageBox.Show(" Arduino has experienced an error");
        }
        /// Executes when an unknown command has been received.
        void OnUnknownCommand(ReceivedCommand arguments)
        {
           // System.Windows.Forms.MessageBox.Show("Command without attached callback received");
        }

        // Callback function that prints the Arduino status to the console
        void OnStatus(ReceivedCommand arguments)
        {
            System.Windows.Forms.MessageBox.Show("Arduino status: ");
            System.Windows.Forms.MessageBox.Show(arguments.ReadStringArg());
        }
    }
}
