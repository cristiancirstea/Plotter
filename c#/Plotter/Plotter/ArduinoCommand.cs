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
        MoveTo, // Command to request led to be set in specific state
    };

    struct CommandData
    {
        Point point;
        bool isPenDown;
    }

    public class Command
    {
        public bool RunLoop { get; set; }
        private SerialTransport _serialTransport;
        private CmdMessenger _cmdMessenger;
        private int _stepperPosition;
        public  List<Point> points = new List<Point>();
        public  int lastPointSend = 0;

        public string PortName { get; set; }
        public int BaundRate { get; set; }

        public void setPosition(int value)
        {
            this._stepperPosition = value;
        }

        // Setup function
        public void Setup()
        {
            _stepperPosition = 0;

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
        // Loop function
        public void Loop()
        {
            //TODO send x and y
            _stepperPosition = getPointToMove().X * 10;
            // Create command
            var command = new SendCommand((int)CommandType.MoveTo,_stepperPosition); 
              

            // Send command
            _cmdMessenger.SendCommand(command);

            //Console.Write("Move to ");
            //Console.WriteLine(_stepperPosition);

            // Wait for 1 second and repeat
            Thread.Sleep(200);
            //_stepperPosition += 300;   // Toggle led state            
        }

        public Point getPointToMove()
        {
            if(this.points.Count == 0)
            {
                lastPointSend = 0;
                return new Point(0,0);
            }
            Point p = this.points[lastPointSend];
            if (this.points.Count > (lastPointSend + 2))
                this.lastPointSend++;
            return p;
        }



        // Exit function
        public void Exit()
        {
            // We will never exit the application
        }

        /// Attach command call backs. 
        private void AttachCommandCallBacks()
        {
            // No callbacks are currently needed
        }
    }
}
