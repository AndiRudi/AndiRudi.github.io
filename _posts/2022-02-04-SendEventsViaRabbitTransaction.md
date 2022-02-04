
# Send Events via RabbitMQ (Transaction Safe)

Recently I came across one of the videos about event driven architectures and it got me thinking what happens when the message bus aka RabbitMQ is not available? It's very dangerous if the data gets saved, but the even is not sent to the message bus, because other services would stay in wrong state. So I thought let's check it out.

Let's begin to replicate a simple example that stores a customer and sends out that the customer was created.

```cs
```

Once this works, lets modify the code to simulate an error when sending the event to rabbit.

```cs
```

As we can see...

Now let's see what options we have

Save to DB
Publish Event

with / without Transaction
