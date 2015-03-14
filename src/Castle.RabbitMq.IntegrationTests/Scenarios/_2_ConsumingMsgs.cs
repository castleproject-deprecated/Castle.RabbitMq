namespace Castle.RabbitMq.IntegrationTests.Scenarios
{
    using System;
    using System.Threading;
    using FluentAssertions;
    using Xunit;

    public class _2_ConsumingMsgs : ConnectorFixtureBase
    {
        [Fact]
        public void ConsumingMsgFromQueueBoundToDefaultExchange()
        {
            var channel = this.Connection.CreateChannel();

            var exchange = channel.DefaultExchange;

            const string queueName = "namedqueue";

            var queue = channel.DeclareQueue(queueName);

            // exchange.Bind(queue, "");

            var @event = new AutoResetEvent(false);
            var msgReceived = 0;

            queue.Consume<MyDumbMessage>((env, ack) =>
            {
                msgReceived++;
                ack.Ack();
                @event.Set();
            }, new ConsumerOptions() { });

            exchange.Send(new MyDumbMessage(), queueName);

            @event.WaitOne(TimeSpan.FromSeconds(2));

            msgReceived.Should().Be(1);
        }

        [Fact]
        public void ConsumingMsgFromQueueBoundToDefaultExchange_DeclaredByDefaultExchange()
        {
            var channel = this.Connection.CreateChannel();

            var exchange = channel.DefaultExchange;

            const string queueName = "namedqueue2";

            var queue = exchange.DeclareQueue(queueName);

            // exchange.Bind(queue, "");

            var @event = new AutoResetEvent(false);
            var msgReceived = 0;

            queue.Consume<MyDumbMessage>((env, ack) =>
            {
                msgReceived++;
                ack.Ack();
                @event.Set();
            });

            exchange.Send(new MyDumbMessage(), queueName);

            @event.WaitOne(TimeSpan.FromSeconds(2));

            msgReceived.Should().Be(1);
        }

        [Fact]
        public void ConsumingFromEphemeralQueue_WontWorkBeforeBinding()
        {
            var channel = this.Connection.CreateChannel();
            var exchange = channel.DeclareExchange("exchange2", RabbitExchangeType.Direct);

            var queue = channel.DeclareEphemeralQueue();

            var @event = new AutoResetEvent(false);
            var msgReceived = 0;

            queue.Consume<MyDumbMessage>((env, ack) =>
            {
                msgReceived++;
                ack.Ack();
                @event.Set();
            });

            exchange.Send(new MyDumbMessage(), queue.Name);
            @event.WaitOne(TimeSpan.FromSeconds(2));

            msgReceived.Should().Be(0);
        }
       
    }
}