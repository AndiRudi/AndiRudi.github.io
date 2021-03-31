---
layout: post
title:  "Fully staffed teams"
author: Andreas Rudischhauser
categories: [ Development, Patterns ]
image: 
description: "Fully staffed teams"
---

# Fully staffed teams

## Example: Why Marketing should be included

## Example: Why BI should be included

The SuperDev Global Inc. develops a SAAS application and has it's development teams structured to fit in scrum teams. Each team consists of a two developers, one product manager and a half test resource. They are currently working on a video MVP which is vital for the company. The product was designed in a collaborative effort and the development is already ongoing for quite some time. With the end of the quarter the project reaches the public release and the developers are currently testing and fixing issues for the final start.

Almost at the end of the project there is a meeting coming up which has the title "Purpose meeting, all Stakeholders invited" which is initiated from other Departments that want to evaluate the success of the project and add their stakes. For the success of the project there is quite some data needed, which is why BI is involved and they want to know how to access the data of the project.

WTF?

As a developer doing an MVP you are most likely don't want to discuss this "now". First of all the project reached the last phase, the going live phase. The focus is on fast small steps, bug fixing, and doing actual runtime support. All of that requires your full attention and this is not the right time to rethink the project and find the possible points to connect with other stakeholders like BI.

Another issues is, that the project is an MVP and all the data structures will probably change with the first refactoring after release. So you don't event want a tight BI integration at this stage (which would only be possible if they connect to the database, and it is not the right time to think of an abstraction).

## Solution: Fully staffed team

If the BI Department would have been part of the team from the beginning, this would have not happened. The BI team would would most likely add their requirements early in the project phase and the correct integration would be part of the MVP.