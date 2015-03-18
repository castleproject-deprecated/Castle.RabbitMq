namespace Castle.RabbitMq.Extensions.MessageHandler
{
    using System;
    using System.Linq;
    using Core.Logging;
    using MicroKernel.Registration;
    using Windsor;

    public abstract class MessageHandlingStrategy
    {
        public abstract void Register(Type handlerImpl, Func<object> handler);
    }

    public class DefaultMessageHandlingStrategy : MessageHandlingStrategy
    {
//        private static Type _registrationType;

        public override void Register(Type handlerContract, Func<object> handler)
        {
            var messageType = handlerContract.GenericTypeArguments.First();

//            var registrationType = RegistrationType.MakeGenericType(msgType);
//            var handlerRegistration = (IMessageHandlerRegistration)
//                Activator.CreateInstance(registrationType, handler, container);
//
//            container.Register(
//                Component.For(registrationType, typeof(IMessageHandlerRegistration)).Instance(handlerRegistration),
//                Component.For(typeof(IMessageHandlerInvoker)).Instance(handlerRegistration.Invoker)
//                );

//            logger.Debug("Registered Invoker for " + msgType);
        }

//        public Type RegistrationType
//        {
//            get { return registrationType ?? typeof(MessageHandlerRegistration<>); }
//            set
//            {
//                if (!typeof(IMessageHandlerRegistration).IsAssignableFrom(value))
//                    throw new ArgumentException("Invalid type");
//
//                registrationType = value;
//            }
//        }

//        public virtual void Subscribe(IWindsorContainer windsorContainer, params IMessageHandlerRegistration[] registrations)
//        {
//            var collector = new HandlerCollection(new NullLogger());
//            var bus = windsorContainer.Resolve<RabbitMQBusAdapter>();
//
//            foreach (var registration in registrations)
//            {
//                registration.Wire(collector, bus);
//            }
//
//            bus.Subscribe(collector);
//        }
    }
}