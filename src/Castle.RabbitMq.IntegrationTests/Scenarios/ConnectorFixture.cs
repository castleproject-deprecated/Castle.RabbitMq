namespace Castle.RabbitMq.IntegrationTests.Scenarios
{
    using System;

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

    public abstract class ConnectorFixtureBase : Xunit.IUseFixture<ConnectorFixture>
    {
        public IRabbitConnection Connection { get; set; }

        public void SetFixture(ConnectorFixture data)
        {
            Connection = data.Connection;
        }
    }
}