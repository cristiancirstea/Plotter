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
    
    // This is the list of recognized commands. These can be commands that can either be sent or received. 
    // In order to receive, attach a callback function to these events
    // 
    // Default commands
    // Note that commands work both directions:
    // - All commands can be sent
    // - Commands that have callbacks attached can be received
    // 
    // This means that both sides should have an identical command list:
    // one side can either send it or receive it (sometimes both)
 
    // Commands
    enum CommandType
    {
        MoveTo, 
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
        public const int SCALE_X = 10;
        public const int SCALE_Y = 10;

        public bool RunLoop { get; set; }
        private SerialTransport _serialTransport;
        private CmdMessenger _cmdMessenger;
        private Point _stepperPosition;
        private bool _isPenDown = false;

        //public  List<Point> points = new List<Point>();
        //public int lastPointSend = 0;

        public List<CommandData> commands = new List<CommandData>();
        public  int lastCommandSend = 0;

        public string PortName { get; set; }
        public int BaundRate { get; set; }

        //TODO
        public void setPosition(Point value)
        {
            this._stepperPosition = value;
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


            //TODO send x and y
            _stepperPosition = scalePoint(commandData.point);
            _isPenDown = commandData.isPenDown;

            // Create command
            //var command = new SendCommand(
            //    (int)CommandType.MoveToX,
            //    _stepperPosition.X
            //);
            
            // Send commands
            var command = new SendCommand(
                    (int)CommandType.MoveTo
                );
            command.AddArgument(_stepperPosition.X);
            //command.AddArgument(_stepperPosition.Y);

            _cmdMessenger.SendCommand(command);
            
            // Wait for 200 milisecond and repeat
            Thread.Sleep(200);           
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
            //else keep last command

            return cd;
        }

        public bool addCommandToSend(Point p, bool isPenDown = false)
        {
            //TODO check last isPenDown
            if (this.commands.Count == 0)
            {
                lastCommandSend = 0;
                CommandData cmd = new CommandData(new Point(0, 0), false);
                this.commands.Add(cmd);
                return true;
            }
            if (
                    (p.X != this.commands[this.commands.Count - 1].point.X)
                    || (p.Y != this.commands[this.commands.Count - 1].point.Y)
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
           System.Windows.Forms.MessageBox.Show(" Arduino is ready");
        }

        // Callback function that prints that the Arduino has experienced an error
        void OnError(ReceivedCommand arguments)
        {
            System.Windows.Forms.MessageBox.Show(" Arduino has experienced an error");
        }
        /// Executes when an unknown command has been received.
        void OnUnknownCommand(ReceivedCommand arguments)
        {
            System.Windows.Forms.MessageBox.Show("Command without attached callback received");
        }

        // Callback function that prints the Arduino status to the console
        void OnStatus(ReceivedCommand arguments)
        {
            System.Windows.Forms.MessageBox.Show("Arduino status: ");
            System.Windows.Forms.MessageBox.Show(arguments.ReadStringArg());
        }
    }
}
