---
layout: post-sidebar
date: 2021-04-05
title: "The Inbox and Outbox Patterns"
categories: coding patterns
author_name : Andi
author_url : /author/andi
author_avatar: andi
show_avatar : true
read_time : 30
feature_image: feature-laptop
show_related_posts: true
square_related: recommend-spain
---


The Inbox and Outbox Patterns are a good way to make sure data flows reliably between services. Let's have a deep look how it works.

To get started have a look at this simple scenario without any inbox and outbox patterns applied:

![Without Inbox](/assets/2021-04-05-InboxOutboxPattern/Without.drawio.svg)

The above scenario is using a direct (or synchronous) connection to the other service. If we want to use messages the scenario would look a bit different:

![With Queue](/assets/2021-04-05-InboxOutboxPattern/WithQueue.drawio.svg)

But also this example can be broken down to the synchronous pattern above because the first call to the RabbitMQ is still synchronous.

![With Queue 2](/assets/2021-04-05-InboxOutboxPattern/WithQueue2.drawio.svg)

The following snippet is a typical example of this scenario. We first save a customer in the local system and then we send a message to another system.

In the end this means, whatever we do, there will be a synchronous call from our service to another system. Now let's have a look how we would send data to this system. 

In the following example, we are saving a customer in our service and then we want to send it to another system (using RabbitMQ). That means, after saving the local customer, we would send it like this:

```csharp
public void AddCustomer(Customer customer) {

    _context.Customers.Add(customer);
    _context.SaveChanges();

    //At this point the record is saved to the local system and now
    //we want to send the record to other systems using the rabbit mq

    RabbitMQ.Send(customer.ToMessage());
}
```

**Can you see the issue with this code?**

Let's think for a moment that RabbitMQ cannot receive the message (network issue, to much data, etc.). With the current code, the record would be saved to the local services database, but sending it to queue will fail and there is no way the lost information can be recovered. Therefore we will have different information on the local system and all other systems.

This is where the Outbox Pattern can help.

## The Outbox Pattern: How does it work?

The idea of this pattern is, that in your local service you have a outbox queue to handle outgoing data. Think of it like a email application with outboxes.

![Outbox](/assets/2021-04-05-InboxOutboxPattern/Outbox.drawio.svg)

There are different ways how you can model the outbox pattern. Whatever pattern you choose, you need to make sure, that both actions (saving locally and adding to the outbox) are handled in one transaction. One pattern is to use hangfire (a background processing library) to deal with this. That means with saving your your local record, you also put a task to send it to another system to Hangfire, which will deal with that later.

```csharp
public void AddCustomer(Customer customer) {

    using (var transaction = new TransactionScope())
    {
        _context.Customers.Add(customer);

        var outboxMessage = customer.ToMessage()
        _context.OutboxMessages.Add(outboxMessage);

        _context.SaveChanges();
        
        Hangfire.BackgroundJob.Enqueue<Outbox>(o => o.Process(outboxMessage.Id));

        transaction.Complete();
    }
}

... in Outbox.cs

public void Process(Guid messageId) {

    var message = _context.OutboxMessages.Find(messageId);
    RabbitMQ.Send(customer);

    message.Processed = DateTime.Now;
    _context.SaveChanges();

}
```

**But what if the messages in the Outbox are not sent?**

This is exactly the reason we have this box. If the other service is not available, the outbox will store all outgoing messages until the error is resolved.

The trick is, that we have separated the problems. As a developer you will have some time to fix the issue, while your local service is still working. If you won't have the outbox, then your local service would be erroring as well.

## The Inbox Pattern: How does it work?

The Inbox Pattern is similar to the Outbox Pattern. The first step is to get the message from the external system and store it. The second step is to loop through the inbox (in order) and process the messages.

![Inbox](/assets/2021-04-05-InboxOutboxPattern/Inbox.drawio.svg)

Here is some pseudo code how this could look like using Hangfire as an Inbox.

```csharp

public void QueueReceiver(Data data) {

    using (var transaction = new TransactionScope())
    {
        var inboxMessage = new Inbox(data);
        _context.InboxMessages.Add(inboxMessage);
        _context.SaveChanges();
       
        data.Ack();
        transaction.Complete();
    }
}

... in Inbox.cs

public void Run(Guid messageId) {

    var message = _context.inboxMessages.Find(messageId);
    if (message.Type == "Customer") 
    {
        //Here we do something with the received data
        _context.Customers.Add(customer.FromMessage(message));
        _context.SaveChanges();
    }
   
}
```

In this case we make sure, that when we receive the message from the queue, we save it to the local database, add a task to process it and acknowledge the message to the queue in one transaction transaction.

The second step is then to process the received data later.

## Full picture

If your service receives data from other services and also sends data to other services, you will probably have an inbox and an outbox. Just to finish this article here the full picture how this looks like.

![Inbox](/assets/2021-04-05-InboxOutboxPattern/Fullpicture.drawio.svg)

## Complete Example

While writing this article I realized, that I want to have a fully working example. So I wrote a [console application](https://github.com/AndiRudi/InboxOutboxPattern) to support the article. I didn't want to use RabbitMQ so I wrote a fake MessageQueue and using Hangfire was also quite tricky so it is not really used, but simulated.

## Known issues

### Do not use In-Memory Store as Inbox/Outbox

An in memory store like Redis used as a backing store can be problematic. You usually don't want to loose the outbox under all circumstances. But if you use Redis and some is flushing the cache you will. So make sure that you always have your inbox/outbox stored to disk.

### Process the inbox/outbox in order

For Inbox: Make sure that you process items in the order they came in (FIFO), otherwise you may mixing up and overwriting data. If the incoming message has a unique key attached use it make sure your not processing it twice. If there is a timestamp use that to order the messages.

For Outbox: Always add a timestamp and unique key to your messages to ensure the receiver can also check for order and duplicates easily. 

### Be careful about multiple workers

If you use background workers like Hangfire to process your outbox, you may want to parallelize the work to speed up the process. Although you can do this, you still need to ensure the order. You may introduce a AggregateId or PartitionId to make sure, that even when you have multiple workers, each worker only always works on a (by AggregateId or PartitionId) - ordered list of all messages.

https://medium.com/incognia-tech/ensuring-data-consistency-across-services-with-the-transactional-outbox-pattern-90be4d735cb0

## FAQ

### Is there another possible option instead of a recurring task polling for changes?

Multiple databases have triggers than can be used to start a worker once new data is incoming.

### What about Distributed Transactions?

Distributed transactions are another way how to ensure transactional safety between two systems. It is usually more complicated. There have been many systems (and there still are) that make use of distributed transactions (see [Two Phase Commit Protocol](https://en.wikipedia.org/wiki/Two-phase_commit_protocol)).

The Inbox/Outbox Pattern is an alternative and easier way. Instead of taking care of the whole transaction over all systems, the pattern only uses transactions inside each service to do internal work and then put the message into the outbox. The status of the whole process has to be part of the message itself. Each system retrieves the message and continues with it's work and eventually creates new messages delivered to other systems.



## Links and Resources

https://event-driven.io/en/outbox_inbox_patterns_and_delivery_guarantees_explained/

https://github.com/oskardudycz/PostgresOutboxPatternWithCDC.NET
https://github.com/oskardudycz/EventSourcing.NetCore#104-event-processing
http://www.kamilgrzybek.com/design/the-outbox-pattern/

Inbox & Outbox pattern - transactional message processing [Microservices .NET]
https://www.youtube.com/watch?v=ebyR5RPKciw

Reliably Save State & Publish Events (Outbox Pattern)
https://www.youtube.com/watch?v=u8fOnxAxKHk

