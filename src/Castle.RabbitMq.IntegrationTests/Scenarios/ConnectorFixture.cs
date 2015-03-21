namespace Castle.RabbitMq.IntegrationTests.Scenarios
{
    using System;
    using Xunit;

    public class ConnectorFixture : IDisposable
    {
        public ConnectorFixture()
        {
            this.Connection = RabbitConnector.Connect("localhost", vhost: "castle");
        }

        public IRabbitConnection Connection { get; set; }

        public void Dispose()
        {
            if (this.Connection != null)
            {
                this.Connection.Dispose();
                this.Connection = null;
            }
        }
    }

    public class ChannelFixture : ConnectorFixture
    {
        public ChannelFixture()
        {
            this.Channel = this.Connection.CreateChannel();
        }

        public IRabbitChannel Channel { get; set; }
    }

//    public abstract class ConnectorFixtureBase : IClassFixture<ConnectorFixture>
//    {
//        public IRabbitConnection Connection { get; private set; }
//
//        public virtual void SetFixture(ConnectorFixture data)
//        {
//            Connection = data.Connection;
//        }
//    }
//
//    public abstract class ChannelFixtureBase : ConnectorFixtureBase
//    {
//        public IRabbitChannel Channel { get; private set; }
//
//        public override void SetFixture(ConnectorFixture data)
//        {
//            base.SetFixture(data);
//
//            this.Channel = this.Connection.CreateChannel();
//        }
//    }
}