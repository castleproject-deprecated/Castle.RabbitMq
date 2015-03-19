namespace Castle.RabbitMq.IntegrationTests
{
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Scenarios;
    using Xunit;

    public class ConsoleTests : ConnectorFixtureBase
    {
        [Fact]
        public async Task GetBindingsAsync()
        {
            var bindings = await this.Connection.Console.GetBindingsAsync();
            bindings.Should().NotBeNull();
//            bindings.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetExchangesAsync()
        {
            var exchanges = await this.Connection.Console.GetExchangesAsync();

            exchanges.Should().NotBeNull();
//            exchanges.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetQueuesAsync()
        {
            var queues = await this.Connection.Console.GetQueuesAsync();

            queues.Should().NotBeNull();
//            queues.Count().Should().BeGreaterThan(0);
        }
    }
}
