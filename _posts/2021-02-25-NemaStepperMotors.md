---
layout: post
title:  "NEMA 8 vs. NEMA 11 Comparison"
author: john
categories: [ IoT, Stepper ]
image: ../assets/2021-02-25-NemaStepperMotors/header.jpg
description: "Nema Stepper Motor with Arduino and A4988 Driver"
---

# Nema Stepper Motor with Arduino and A4988 Driver

For one of my next projects I wanted to use the 28BYJ-48 Stepper motor, but while trying out, I came to realize that this motor does not have enough torque for the project. This was the time when I figured there are a bunch of other stepper motors, called the NEMA Motors. They come in different sizes and shapes and I just bought a NEMA 8 and a NEMA 11 motor to compare them.

To drive a NEMA stepper, you need:

- Stepper Motor
- Arduino Nano (or any other)
- A A4988 driver board
- Development board
- Cables 
- USB Cable
- A 3D Printed hinch

## Winch

I decided to quickly spin up Tinkercad and draw an winch. Tinkercad is quite easy to use and I recommend it for beginners. It has everything needed to make lots of projects and it's free.

I designed the winch for both motors. You can download them from Thingiverse:

Thingiverse Hinches for Nema 8 and 11

## Schema

This is how the things need to be connected

![Schematic View](../assets/2021-02-25-NemaStepperMotors/schema.svg)

## Bread Board

This is how to set things up on a bread board.

![BreadBoard](../assets/2021-02-25-NemaStepperMotors/breadboard.png)

## Code

This is a pretty simple version of the code to run the motors in both directions. Just copy this to your Arduino IDE, select the correct Arduino board and upload it. Be careful, the motors will start right after upload.

```c
int stepCounter;
int steps = 2000;

void setup()
{
   pinMode(6, OUTPUT); // enable
   pinMode(5, OUTPUT); // step
   pinMode(4, OUTPUT); // direction
   digitalWrite(6,LOW);
}

void loop()
{
   digitalWrite(4,HIGH); // counterwise
   for(stepCounter = 0; stepCounter < steps; stepCounter++)
   {
      digitalWrite(5,HIGH);
      delayMicroseconds(500);
      digitalWrite(5,LOW);
      delayMicroseconds(500);
   }

   delay(1000);

   digitalWrite(4,LOW); // counterclockwise
   
   for(stepCounter = 0; stepCounter < steps; stepCounter++)
   {
      digitalWrite(5,HIGH);
      delayMicroseconds(500);
      digitalWrite(5,LOW);
      delayMicroseconds(500);
   }

   delay(1000);
}
```

## Assembly

First mount the winch onto the motor. You may need some filing and pushing to get it onto the shaft. Then get your breadboard and some cables and put everything together like in the breadboard. Last but not least plug the USB cable into the Arduino and upload the code. Once uploaded, connect the main power source to the motor and it should immediately start spinning. If not head over to the FAQ below.

This is how it looks like in my setup


And here you can see a little video of the final result:

## Results




## FAQ

### The stepper motor is just making sounds, but is not moving

In my experiments this happenes when the motor does not have enough power (voltage). I first tried to use a converter which creates 5V out off the USB port, but this was not enough current to power the motor. So please use an external power supply to drive the motor or some extra batteries.

I am using a laboratory power supply because it can give different current and has additional usb power. [Click here to see an recommendation](https://amzn.to/3kSzGa1)

### What is 28BYJ-48?

This is a very cheap stepper motor. Beacause of that ot is used a lot in IoT projects. The downside of the motor is, that it does not have a lot of torque

28BYJ-48 35 mm x 30 mm, 0,034 Nm -> That means around 200 gramms

### What is NEMA?

NEMA (National Electrical Manufacturers Association) is a group that has standardized the motors (and their torque)

* NEMA 08, 20 mm × 20 mm, 0,036 Nm

That means around 200 gramms

* NEMA 11, 28 mm × 28 mm, 0,1 Nm

You can really feel the difference both in weight (around 100 gramms) and torque compared to the NEMA 8 or 28BYJ-48.
That means around 400 gramms (but also it weighs around 100 gramms)

* NEMA 14, 35 mm × 35 mm, 0,3 Nm
* NEMA 17, 42 mm × 42 mm, 0,5 Nm
* NEMA 23, 56 mm × 56 mm, 2,0 – 4,0 Nm
* NEMA 34, 86 mm × 86 mm, 4,5 – 8,0 Nm


