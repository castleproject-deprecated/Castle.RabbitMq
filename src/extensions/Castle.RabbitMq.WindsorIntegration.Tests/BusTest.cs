namespace Castle.RabbitMq.WindsorIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Castle.RabbitMq;
    using Castle.RabbitMq.IntegrationTests.Scenarios;
    using Castle.RabbitMq.Messaging;
    using FluentAssertions;
    using Xunit;


    public class BusTest : IClassFixture<ChannelFixture>
    {
        public BusTest(ChannelFixture channelFix)
        {
            this.Channel = channelFix.Channel;
        }

        public IRabbitChannel Channel { get; set; }

        // What_When_Should

        [Fact]
        public void Bus_Send_ShouldDeclareExchangeAndCreateQueue_RespectingPrefixes()
        {
            const string vhost = "castle";

            var bus = new CastleRabbitMqBus(new ConfigSettings()
            {
                OurScope = "test_bus",
                VHost = vhost,
                ExchangeNamePrefix = "test_e_",
                QueueNamePrefix = "test_q_",
                NamespaceExchangeMapping = new Dictionary<string, string>()
                {
                    { "Castle.RabbitMq.WindsorIntegration.Tests", "test_bus" }
                }
            }, this.Channel);

            bus.Send(new MyRoutableMessage() { RoutingKey = "routing1" });

            Thread.Sleep(0);

            var queue = this.Channel.DeclareQueue("test_q_routing1", new QueueOptions()
            {
                AutoDelete = false, Durable = true, Exclusive = false
            });

            var @event = new AutoResetEvent(false);

            queue.Consume<MyRoutableMessage>((env, ack) =>
            {
                ack.Ack();

                env.Properties.Type.Should().Be("Castle.RabbitMq.WindsorIntegration.Tests.MyRoutableMessage, Castle.RabbitMq.WindsorIntegration.Tests");

                @event.Set();
            });

            if (!@event.WaitOne(TimeSpan.FromSeconds(5)))
            {
                throw new Exception("Did not consume expected message");
            }
        }
    }
}
