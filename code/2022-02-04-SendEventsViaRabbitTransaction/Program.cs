// See https://aka.ms/new-console-template for more information
using System.Net.Mail;
using System.Text;
using System.Transactions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

//Setup Database
var context = new TestDbContext();

//Setup Rabbit MQ
var factory = new ConnectionFactory() { HostName = "localhost" };
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();
channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

//Setup Receiver
var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine(" [x] Received {0}", message);
};
channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);

var customer = new Customer { Name = "John Doe" };

Console.WriteLine("Simulation started");
//WithoutTrans(fail: false);
WithTrans(fail: false);

void WithoutTrans(bool fail)
{
    //Store to Database
    context.Customers.Add(customer);
    context.SaveChanges();
    Console.WriteLine("SaveChanges done");

    if (fail) throw new Exception("Simulated failure");

    //Send to Rabbit
    channel.BasicPublish(exchange: "",
                         routingKey: "hello",
                         basicProperties: null,
                         body: Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(customer)));

    Console.ReadKey();
}

void WithTrans(bool fail)
{
    using var transactionScope = new TransactionScope();

    //Store to Database
    context.Customers.Add(customer);
    context.SaveChanges();
    Console.WriteLine("SaveChanges done");

    if (fail) throw new Exception("Simulated failure");

    //Send to Rabbit
    channel.BasicPublish(exchange: "",
                         routingKey: "hello",
                         basicProperties: null,
                         body: Encoding.UTF8.GetBytes(System.Text.Json.JsonSerializer.Serialize(customer)));

    Console.ReadKey();

    transactionScope.Complete();

}