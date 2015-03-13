namespace Castle.RabbitMq
{
    static class ExtHelpers
    {
        public static string ToStr(this RabbitExchangeType source)
        {
            return source.ToString().ToLowerInvariant();
        }
    }
}