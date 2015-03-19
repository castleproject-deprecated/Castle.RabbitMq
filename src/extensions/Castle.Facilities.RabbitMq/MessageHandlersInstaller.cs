namespace Castle.RabbitMq.Extensions.MessageHandler
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Core.Internal;
    using Core.Logging;
    using Messaging;
    using MicroKernel.Registration;
    using MicroKernel.SubSystems.Configuration;
    using Windsor;

    /// <summary>
    /// Maps messages from a queue to instances of <see cref="IMessageHandler{TMessage}"/>
    /// </summary>
    public class MessageHandlersInstaller : IWindsorInstaller
    {
        private static readonly Type GenericHandler = typeof(IMessageHandler<>);

        private readonly Assembly[] _assembliesWithHandlers;

        public MessageHandlersInstaller(ILogger logger = null, 
                                        MessageHandlingStrategy handlingStrategy = null,
                                        params Assembly[] assembliesWithHandlers)
        {
            this._assembliesWithHandlers = assembliesWithHandlers;

            this.HandlingStrategy = handlingStrategy ?? new DefaultMessageHandlingStrategy();

            this.Logger = logger ?? NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        /// <summary>
        /// Name this host current scope of message handlers. 
        /// </summary>
        public string CurrentScopeName { get; set; }
//        public string ExchangeNamePrefix { get; set; }
//        public string QueueNamePrefix { get; set; }

        public MessageHandlingStrategy HandlingStrategy { get; set; }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            if (!container.Kernel.HasComponent(typeof(CastleRabbitMqBus)))
            {
                container.Register(
                    Component.For<IBus, CastleRabbitMqBus>()
                             .ImplementedBy<CastleRabbitMqBus>()
                );
            }

            foreach (var asm in _assembliesWithHandlers)
            {
                InstallHandlersInAssembly(asm, container);
            }
        }

        private void InstallHandlersInAssembly(Assembly asm, IWindsorContainer container)
        {
            var handlers = asm
                    .GetExportedTypes()
                    .Where(t => !t.IsAbstract && IsAssignableToGenericType(t, GenericHandler))
                    .Where(MatchesOurScope);

            foreach (var handlerImpl in handlers)
            {
                try
                {
                    RegisterHandlerAndHandlerContracts(handlerImpl, container);
                }
                catch (Exception e)
                {
                    this.Logger.Error("Error registering msg handler type: " + handlerImpl, e);
                }
            }
        }

        private void RegisterHandlerAndHandlerContracts(Type handlerHolder, IWindsorContainer container)
        {
            var holder = handlerHolder;

            container.Register(Component.For(holder));

            Func<object> handler = () => container.Resolve(holder);

            var handlerContracts = holder
                .GetInterfaces()
                .Where(t => t.Name.StartsWith("IMessageHandler", StringComparison.Ordinal));

            foreach (var handlerContract in handlerContracts)
            {
                HandlingStrategy.Register(handlerContract, handler);
            }
        }

        /// <summary>
        /// Accepts a handler that either has no
        /// scope set through <see cref="MessagingScopeAttribute"/>, 
        /// or the explicitly set scope matches <see cref="CurrentScopeName"/>
        /// </summary>
        private bool MatchesOurScope(Type handlerType)
        {
            var attr = handlerType.GetAttribute<MessagingScopeAttribute>();

            return attr == null || attr.GetScopes().Any(s => s == this.CurrentScopeName);
        }

        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            var baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }
    }
}
