int stepCounter;
int steps = 2000;

void setup()
{
   // Motor 1
   
  pinMode(6, OUTPUT); // enable
  pinMode(5, OUTPUT); // step
  pinMode(4, OUTPUT); // direction
  digitalWrite(6,LOW);

   // Motor 2

  pinMode(7, OUTPUT); // enable
  pinMode(8, OUTPUT); // step
  pinMode(9, OUTPUT); // direction
  digitalWrite(7,LOW);

   // Servo
  pinMode(3, OUTPUT); // pulse
   
   
}

void loop()
{
   digitalWrite(4,HIGH); // clockwise Motor 1    
   for(stepCounter = 0; stepCounter < steps; stepCounter++)
   {
      // Motor 1
      digitalWrite(5,HIGH);
      delayMicroseconds(500);
      digitalWrite(5,LOW);
      delayMicroseconds(500);
   }

   delay(1000);

   digitalWrite(9,HIGH); // clockwise Motor 2      
   for(stepCounter = 0; stepCounter < steps; stepCounter++)
   {
      digitalWrite(8,HIGH);
      delayMicroseconds(500);
      digitalWrite(8,LOW);
      delayMicroseconds(500);    
   }


   delay(1000);
}