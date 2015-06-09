using System;
using CommandMessenger;
using CommandMessenger.TransportLayer;
using Plotter;

namespace ArduinoController
{
    enum Command
    {
        Acknowledge,            // Command to acknowledge a received command
        Error,                  // Command to message that an error has occurred
        MoveToX,                 
        MoveToY,        
        SetPen,
        Status
    };

    public class ArduinoController
    {
        // This class (kind of) contains presentation logic, and domain model.
        // ChartForm.cs contains the view components 

        private SerialTransport   _serialTransport;
        private CmdMessenger      _cmdMessenger;
        private ControllerForm    _controllerForm;
        public string portName = "COM10"; //Default
        public int baud = 9600;//Default

        // ------------------ MAIN  ----------------------

        // Setup function
        public void Setup(ControllerForm controllerForm)
        {
            // storing the controller form for later reference
            _controllerForm = controllerForm;

            // Create Serial Port object
            // Note that for some boards (e.g. Sparkfun Pro Micro) DtrEnable may need to be true.
            _serialTransport = new SerialTransport
            {
                CurrentSerialSettings = { PortName = this.portName, BaudRate = this.baud, DtrEnable = false } // object initializer
            };

            // Initialize the command messenger with the Serial Port transport layer
            _cmdMessenger = new CmdMessenger(_serialTransport)
            {
                BoardType = BoardType.Bit16 // Set if it is communicating with a 16- or 32-bit Arduino board
            };

            // Tell CmdMessenger to "Invoke" commands on the thread running the WinForms UI
            _cmdMessenger.SetControlToInvokeOn(_controllerForm);

            // Attach the callbacks to the Command Messenger
            AttachCommandCallBacks();

            // Attach to NewLinesReceived for logging purposes
            _cmdMessenger.NewLineReceived += NewLineReceived;

            // Attach to NewLineSent for logging purposes
            _cmdMessenger.NewLineSent += NewLineSent;                       

            // Start listening
            _cmdMessenger.StartListening();

            _controllerForm.setMotorXPosition(0);
            _controllerForm.setMotorYPosition(0);
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
            _cmdMessenger.Attach((int)Command.Acknowledge, OnAcknowledge);
            _cmdMessenger.Attach((int)Command.Error, OnError);
            _cmdMessenger.Attach((int)Command.MoveToX, OnMoveX);
            _cmdMessenger.Attach((int)Command.MoveToY, OnMoveY);
            _cmdMessenger.Attach((int)Command.SetPen, OnSetPen);
            _cmdMessenger.Attach((int)Command.Status, OnStatus);
        }

        // ------------------  CALLBACKS ---------------------

        // Called when a received command has no attached function.
        // In a WinForm application, console output gets routed to the output panel of your IDE
        void OnUnknownCommand(ReceivedCommand arguments)
        {            
            this._controllerForm.log(@"Command without attached callback received");
        }

        // Callback function that prints that the Arduino has acknowledged
        void OnAcknowledge(ReceivedCommand arguments)
        {
            this._controllerForm.log(@" Arduino is ready");
        }

        // Callback function that prints that the Arduino has experienced an error
        void OnError(ReceivedCommand arguments)
        {
            this._controllerForm.log(@"Arduino has experienced an error");
        }

        // Callback function that prints that the Arduino has experienced an error
        void OnMoveX(ReceivedCommand arguments)
        {
            this._controllerForm.log(@"MovedX ");
        }
        // Callback function that prints that the Arduino has experienced an error
        void OnMoveY(ReceivedCommand arguments)
        {
            this._controllerForm.log(@"MoveY ");
        }
        // Callback function that prints that the Arduino has experienced an error
        void OnSetPen(ReceivedCommand arguments)
        {
            this._controllerForm.log(@"SetPen " + arguments.ReadStringArg());
        }

        void OnStatus(ReceivedCommand arguments)
        {
            this._controllerForm.log(@"X pos " + arguments.ReadInt16Arg());
            this._controllerForm.log(@"Y pos " + arguments.ReadInt16Arg());
        }

        // Log received line to console
        private void NewLineReceived(object sender, NewLineEvent.NewLineArgs e)
        {
            this._controllerForm.log(@"Received > " + e.Command.CommandString());
        }

        // Log sent line to console
        private void NewLineSent(object sender, NewLineEvent.NewLineArgs e)
        {
            this._controllerForm.log(@"Sent > " + e.Command.CommandString());
        }

        // Sent command to change led blinking frequency
        public void SendMotorXPosition(int value)
        {
            // Create command to start sending data
            var command = new SendCommand((int)Command.MoveToX, value);

            // Put the command on the queue and wrap it in a collapse command strategy
            // This strategy will avoid duplicates of this certain command on the queue: if a SetLedFrequency command is
            // already on the queue when a new one is added, it will be replaced at its current queue-position. 
            // Otherwise the command will be added to the back of the queue. 
            // 
            // This will make sure that when the slider raises a lot of events that each send a new blink frequency, the 
            // embedded controller will not start lagging.
            _cmdMessenger.QueueCommand(new CollapseCommandStrategy(command));
        }


        // Sent command to change led on/of state
        public void SendMotorYPosition(int value)
        {
            // Create command to start sending data
            var command = new SendCommand((int)Command.MoveToY, value);

            // Send command
            _cmdMessenger.SendCommand(new SendCommand((int)Command.MoveToY, value));         
        }
        // Sent command to change led on/of state
        public void SendPenDown(bool value)
        {
            // Create command to start sending data
            var command = new SendCommand((int)Command.SetPen, value);

            // Send command
            _cmdMessenger.SendCommand(command);
        }

    }
}
