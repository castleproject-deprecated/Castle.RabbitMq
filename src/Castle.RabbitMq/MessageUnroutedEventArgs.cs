namespace Castle.RabbitMq
{
    public class MessageUnroutedEventArgs
    {
        public MessageEnvelope MessageEnvelope { get; internal set; }
        public string Exchange { get; internal set; }
        public ushort ReplyCode { get; internal set; }
        public string ReplyText { get; internal set; }
        public string RoutingKey { get; internal set; }
    }
}