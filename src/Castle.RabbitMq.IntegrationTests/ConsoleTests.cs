namespace Castle.RabbitMq.IntegrationTests
{
    using System.Linq;
    using FluentAssertions;
    using Scenarios;
    using Xunit;

    public class ConsoleTests : ConnectorFixtureBase
    {
        [Fact]
        public void GetBindingsAsync()
        {
            var task = this.Connection.Console.GetBindingsAsync();
            task.Wait();

            var bindings = task.Result.ToList();
            bindings.Should().NotBeNull();
            bindings.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public void GetExchangesAsync()
        {
            var task = this.Connection.Console.GetExchangesAsync();
            task.Wait();

            var bindings = task.Result.ToList();
            bindings.Should().NotBeNull();
            bindings.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public void GetQueuesAsync()
        {
            var task = this.Connection.Console.GetQueuesAsync();
            task.Wait();

            var bindings = task.Result.ToList();
            bindings.Should().NotBeNull();
            bindings.Count().Should().BeGreaterThan(0);
        }
    }
}
