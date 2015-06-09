/** Plotter **/
#include <CmdMessenger.h>  // CmdMessenger
#include <AccelStepper.h>
#include <Servo.h> 

#define UNIPOLARSTEP 1

#define motorXPin1  7   //
#define motorXPin2  6   //  
#define motorYPin1  5   //
#define motorYPin2  4   // 

 
Servo myservo;  // create servo object to control a servo 
 
int posPenDown = 20;    
int posPenUp = 50;
 
// Attach a new CmdMessenger object to the default Serial port
CmdMessenger cmdMessenger = CmdMessenger(Serial);

AccelStepper stepperX(UNIPOLARSTEP, motorXPin1, motorXPin2);
AccelStepper stepperY(UNIPOLARSTEP, motorYPin1, motorYPin2);

volatile int stepperXPosition = 0;
volatile int stepperYPosition = 0;


// This is the list of recognized commands. These can be commands that can either be sent or received. 
// In order to receive, attach a callback function to these events
enum
{
  kAcknowledge,
  kError,
  kMoveToX,                
  kMoveToY,       
  kSetPen,
  kStatus
};

// Callbacks define on which received commands we take action
void attachCommandCallbacks()
{
  // Attach callback methods
  cmdMessenger.attach(OnUnknownCommand);
  cmdMessenger.attach(kMoveToX, OnMoveToX);
  cmdMessenger.attach(kMoveToY, OnMoveToY);
  cmdMessenger.attach(kSetPen, OnSetPen);
  cmdMessenger.attach(kStatus, OnStatus);
}

// Called when a received command has no attached function
void OnUnknownCommand()
{
  cmdMessenger.sendCmd(kError,"Command without attached callback");
}

// Callback function that sets led on or off
void OnMoveToX()
{
  stepperXPosition = cmdMessenger.readInt32Arg();
  stepperX.moveTo(stepperXPosition);
  
  cmdMessenger.sendCmd(kMoveToX, stepperXPosition);
}

void OnMoveToY()
{
  stepperYPosition = cmdMessenger.readInt32Arg();
  stepperY.moveTo(stepperYPosition);
  
  cmdMessenger.sendCmd(kMoveToY, stepperYPosition);
}

void OnSetPen()
{
  bool penDown = cmdMessenger.readBoolArg();
  if(penDown)
    myservo.write(posPenDown);
  else
    myservo.write(posPenUp);
  cmdMessenger.sendCmd(kSetPen, (penDown ? "Down" : "Up"));
}

void OnStatus()
{
  cmdMessenger.sendCmd(kStatus, stepperX.currentPosition(), stepperY.currentPosition());
}

// Setup function
void setup() 
{
  // Listen on serial connection for messages from the PC
  Serial.begin(9600); 
myservo.attach(9);
  //Stepper setup
   stepperX.setMaxSpeed(800);
  stepperX.setAcceleration(800.0);
  stepperX.setSpeed(800);
  
   stepperY.setMaxSpeed(800);
  stepperY.setAcceleration(800.0);
  stepperY.setSpeed(800);
  
  // Adds newline to every command
  //cmdMessenger.printLfCr();   

  // Attach my application's user-defined callback methods
  attachCommandCallbacks();

  // Send the status to the PC that says the Arduino has booted
  // Note that this is a good debug function: it will let you also know 
  // if your program had a bug and the arduino restarted  
  cmdMessenger.sendCmd(kAcknowledge,"Arduino has started!");
  //stepperY.moveTo(3600); //debug 
  //stepperX.moveTo(1800); debug 
  
  myservo.write(posPenUp);//Start with pen up
}

// Loop function
void loop() 
{
  // Process incoming serial data, and perform callbacks
  cmdMessenger.feedinSerialData();
  //Run steppers
  //stepperX.setSpeed(800);
  stepperX.run();
  stepperY.setSpeed(600);
  stepperY.runSpeedToPosition();
}
