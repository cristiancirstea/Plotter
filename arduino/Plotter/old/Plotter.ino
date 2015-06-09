#include <CmdMessenger.h>  
#include <AccelStepper.h>
/*-----( Declare Constants and Pin Numbers )-----*/
#define FULLSTEP 4
#define HALFSTEP 8
// motor pins
#define motorPin1  2     // Blue   - 28BYJ48 pin 1
#define motorPin2  3     // Pink   - 28BYJ48 pin 2
#define motorPin3  4    // Yellow - 28BYJ48 pin 3
#define motorPin4  5     // Orange - 28BYJ48 pin 4
                        // Red    - 28BYJ48 pin 5 (VCC)
/*-----( Declare objects )-----*/
// NOTE: The sequence 1-3-2-4 is required for proper sequencing of 28BYJ48
//AccelStepper stepper(HALFSTEP, motorPin1, motorPin3, motorPin2, motorPin4);
AccelStepper stepper(1, 11, 10);

// Attach a new CmdMessenger object to the default Serial port
CmdMessenger cmdMessenger = CmdMessenger(Serial);
int stepperPosition = 0;

enum
{
  kMoveTo, // Command to move stepper specific... steps (of course)
};

// Callbacks define on which received commands we take action 
void attachCommandCallbacks()
{
  cmdMessenger.attach(kMoveTo, OnMoveTo);
}

// Callback function that moves stepper
void OnMoveTo()
{
  
  stepperPosition = cmdMessenger.readInt32Arg();
  // Set led
  stepper.moveTo(stepperPosition);
}

void setup() {
  Serial.begin(9600);
  
  stepper.setMaxSpeed(1000);
  stepper.setAcceleration(800.0);
  stepper.setSpeed(800);
  
   // Adds newline to every command
  cmdMessenger.printLfCr();  
  
  attachCommandCallbacks();
}

void loop() {
  cmdMessenger.feedinSerialData();
  stepper.run();
}
