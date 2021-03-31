---
layout: post
title:  "Inbox/Outbox Pattern"
author: Andreas Rudischhauser
categories: [ Development, Patterns ]
image: 
description: "Inbox/Outbox Pattern"
---
# Inbox / Outbox Pattern

The Inbox (and Outbox) Patterns are an alternative way to make sure data flows correctly between services without using any sort of transaction system.

## How does it work?

The idea is, that in your service you have a outbox queue and a inbox queue to handle incoming and outgoing data. Think of it like a email application with inboxes and outboxes. An external system sends you a message and the first thing you do is, you receive it and put it into the inbox. Later then you may process the message. The same goes for the outbox. When you have a new message ready you put it into the outbox and it will be send out later.

## Why do I need it?

Whenever you want to send information to another system, you need to make sure the message is sent and the integrity of your system maintains. Inside one system you can use transactions to make sure, that everything is either done or failed and there is never a situation where only half of the work is done. This is what a transaction guarantees.

Distributed Transactions

The problem is, that transactions over multiple systems (distributed transactions) are complicated. There have been many systems (and there still are) that make use of distributed transactions to ensure over multiple systems, that a transaction is either completed or failed (see [Two Phase Commit Protocol](https://en.wikipedia.org/wiki/Two-phase_commit_protocol)).

The Inbox/Outbox Pattern is an alternative and easier way. Instead of taking care of the whole transaction over all systems, the pattern only uses transactions inside each service to do internal work and then put the message into the outbox. The status of the whole process has to be part of the message itself. Each system retrieves the message and continues with it's work and eventually creates new messages delivered to other systems. 

Inbox/Outbox Pattern

## When should I use it?


## How to not do it?

The following snippet is a typical example of NOT using the outbox pattern but still sending events to other systems

```csharp
public void AddCustomer(Customer customer) {

    _context.Customers.Add(customer);
    _context.SaveChanges();

    //At this point the record is saved to the local system and now
    //we want to send the record to other systems using the rabbit mq

    RabbitMQ.Send(customer.ToMessage());
}
```

Although it is already good to make use of a message broker like RabbitMQ or Kafka, the way its used here is dangerous. Lets think for a moment that the RabbitMQ cannot receive the message (network issue, to much data, etc.). With the current code, the record would be saved to the local system, but sending it to queue will fail and there is no way the lost information can be recovered.

## How do I do it?

In this example we use the Outbox Pattern to make sure, that the message is first written to the Outbox, and then processed

```csharp
public void AddCustomer(Customer customer) {

    _context.Customers.Add(customer);

    var outboxMessage = customer.ToMessage()
    _context.OutboxMessages.Add(outboxMessage);

    _context.SaveChanges();
        
    Hangfire.BackgroundJob.Enqueue<Outbox>(o => o.Process(outboxMessage.Id));
}

... in Outbox.cs

public void Process(Guid messageId) {
    var message = _context.OutboxMessages.Find(messageId);
    RabbitMQ.Send(customer);
    message.Processed = DateTime.Now;
    _context.SaveChanges();
}

```

This ensures, that whenever the customer is saved, we also save a message TO BE SENT in the outbox. In this case we also enqueue a task using hangfire to now send the message to Rabbit. The advantage is, that if the message sending fails, it will be retried.

## Do not use Redis as Inbox/Outbox Storage

Unless you need very high speeds and can live with lost information, you should not use Redis as storage for inbox and outbox messages. If Redis is failing and the Data is flushed you will lose all your information. Make sure that you always have your inbox/outbox stored to disk.

## Do process messages in order (but maybe parallelize)
https://medium.com/incognia-tech/ensuring-data-consistency-across-services-with-the-transactional-outbox-pattern-90be4d735cb0


## Links and Resources

https://event-driven.io/en/outbox_inbox_patterns_and_delivery_guarantees_explained/

https://github.com/oskardudycz/PostgresOutboxPatternWithCDC.NET
https://github.com/oskardudycz/EventSourcing.NetCore#104-event-processing
http://www.kamilgrzybek.com/design/the-outbox-pattern/