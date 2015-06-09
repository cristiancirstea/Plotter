#include <CmdMessenger.h>  
#include <AccelStepper.h>
/*-----( Declare Constants and Pin Numbers )-----*/
//#define FULLSTEP 4
//#define HALFSTEP 8
// motor pins
//#define motorPin1  2     // Blue   - 28BYJ48 pin 1
//#define motorPin2  3     // Pink   - 28BYJ48 pin 2
//#define motorPin3  4    // Yellow - 28BYJ48 pin 3
//#define motorPin4  5     // Orange - 28BYJ48 pin 4
                        // Red    - 28BYJ48 pin 5 (VCC)
/*-----( Declare objects )-----*/
// NOTE: The sequence 1-3-2-4 is required for proper sequencing of 28BYJ48
//AccelStepper stepper(HALFSTEP, motorPin1, motorPin3, motorPin2, motorPin4);
AccelStepper stepperX(1, 5, 4);
AccelStepper stepperY(1, 7, 6);

// Attach a new CmdMessenger object to the default Serial port
CmdMessenger cmdMessenger = CmdMessenger(Serial);
volatile int stepperXPosition = 0;
volatile int stepperYPosition = 0;

enum
{
  // Commands
  kAcknowledge         , // Command to acknowledge that cmd was received
  kError               , // Command to report errors
  kMoveTo, // Command to move stepperX specific... steps (of course)
  kStatus
};

// Callbacks define on which received commands we take action 
void attachCommandCallbacks()
{
  // Attach callback methods
  cmdMessenger.attach(OnUnknownCommand);
  cmdMessenger.attach(kMoveTo, OnMoveTo);
  cmdMessenger.attach(kStatus, OnStatus);
}

// Callback function that responds that Arduino is ready (has booted up)
void OnArduinoReady()
{
  cmdMessenger.sendCmd(kAcknowledge,"Arduino ready");
}


// Called when a received command has no attached function
void OnUnknownCommand()
{
  cmdMessenger.sendCmd(kError,"Command without attached callback");
}
// Callback function that moves stepper to OX
void OnMoveTo()
{
  stepperXPosition = cmdMessenger.readInt32Arg();
  stepperX.moveTo(stepperXPosition);
  
   stepperYPosition = cmdMessenger.readInt32Arg();
  stepperY.moveTo(stepperYPosition);
}

void OnStatus()
{
  //TODO
}

void setup() {
  Serial.begin(9600);
  
  stepperX.setMaxSpeed(1000);
  stepperX.setAcceleration(800.0);
  stepperX.setSpeed(800);
  
  stepperY.setMaxSpeed(1000);
  stepperY.setAcceleration(200.0);
  stepperY.setSpeed(200);
  
   // Adds newline to every command
  cmdMessenger.printLfCr();  
  
  attachCommandCallbacks();
  
  // Send the status to the PC that says the Arduino has booted
  cmdMessenger.sendCmd(kAcknowledge,"Arduino has started!");
}

void loop() {
  cmdMessenger.feedinSerialData();
  //Run only if it has to !!!! TODO !!!!
  stepperX.run();
  stepperY.run();
}
