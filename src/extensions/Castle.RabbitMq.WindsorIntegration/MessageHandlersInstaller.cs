namespace Castle.RabbitMq.WindsorIntegration
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Impl;
    using Windsor;
    using Core.Internal;
    using Core.Logging;
    using Messaging;
    using MicroKernel.Registration;
    using MicroKernel.SubSystems.Configuration;

    /// <summary>
    /// Maps messages from a queue to instances of <see cref="IMessageHandler{TMessage}"/>
    /// </summary>
    public class MessageHandlersInstaller : IWindsorInstaller
    {
        private static readonly Type GenericHandler = typeof(IMessageHandler<>);

        private readonly Assembly[] _assembliesWithHandlers;
        private ConfigSettings _config;
        private MessageHandlerMainDispatcher _mainDispatcher;

        public MessageHandlersInstaller(ConfigSettings config = null,
                                        params Assembly[] assembliesWithHandlers)
        {
            this._config = config;
            this._assembliesWithHandlers = assembliesWithHandlers;
        }

        public bool UseTransactions { get; set; }
        public ILogger Logger { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            SetUpLoggerFactory(container);
            SetUpLogger();
            SetUpBus(container);
            SetUpConfig(container);
            SetUpHandlerInvoker(container);
            SetUpMainDispatcher(container);

            // ordering is primordial here. so we have to install the handlers first

            foreach (var asm in _assembliesWithHandlers)
            {
                InstallHandlersInAssembly(asm, container);
            }

            // then we can start the bus
            StartBus(container);

            // Then the dispatcher
            StartMainDispatcher(container);
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

            Func<IMessageHandler> handler = () => (IMessageHandler) container.Resolve(holder);

            var handlerContracts = holder
                .GetInterfaces()
                .Where(t => t.Name.StartsWith("IMessageHandler", StringComparison.Ordinal));

            foreach (var handlerContract in handlerContracts)
            {
                Register(handlerContract, handler);
            }
        }

        private void Register(Type handlerContract, Func<IMessageHandler> handlerBuilder)
        {
            if (!handlerContract.IsGenericType) return;

            var msgType = handlerContract.GetGenericArguments()[0];

            _mainDispatcher.Add(msgType, handlerBuilder);
        }

        /// <summary>
        /// Accepts a handler that either has no
        /// scope set through <see cref="MessagingScopeAttribute"/>, 
        /// or the explicitly set scope matches <see cref="ConfigSettings.Id"/>
        /// </summary>
        private bool MatchesOurScope(Type handlerType)
        {
            var attr = handlerType.GetAttribute<MessagingScopeAttribute>();

            return attr == null || attr.GetScopes().Any(s => s == _config.OurScope);
        }

        private void StartBus(IWindsorContainer container)
        {
            container.Resolve<IBus>().Start();
        }

        private void StartMainDispatcher(IWindsorContainer container)
        {
            container.Resolve<MessageHandlerMainDispatcher>().Start();
        }

        private void SetUpMainDispatcher(IWindsorContainer container)
        {
            container.Register(Component.For<MessageHandlerMainDispatcher>());
            _mainDispatcher = container.Resolve<MessageHandlerMainDispatcher>();
        }

        private void SetUpHandlerInvoker(IWindsorContainer container)
        {
            if (this.UseTransactions)
            {
                container.Register(
                    Component.For<MessageHandlerInvoker>().ImplementedBy<TransactionalMessageHandlerInvoker>());
            }
            else
            {
                container.Register(Component.For<MessageHandlerInvoker>().ImplementedBy<DefaultMessageHandlerInvoker>());
            }
            
        }

        private void SetUpConfig(IWindsorContainer container)
        {
            if (_config == null)
            {
                _config = container.Resolve<ConfigSettings>();
            }
            else
            {
                container.Register(Component.For<ConfigSettings>().Instance(_config));
            }
        }

        private void SetUpLogger()
        {
            this.Logger = this.LoggerFactory.Create(typeof(MessageHandlersInstaller));
        }

        private void SetUpBus(IWindsorContainer container)
        {
            if (!container.Kernel.HasComponent(typeof(CastleRabbitMqBus)))
            {
                container.Register(
                    Component.For<IBus, CastleRabbitMqBus>()
                             .ImplementedBy<CastleRabbitMqBus>()
                );
            }
        }

        private void SetUpLoggerFactory(IWindsorContainer container)
        {
            this.LoggerFactory = 
                container.Kernel.HasComponent(typeof(ILoggerFactory)) ? 
                container.Resolve<ILoggerFactory>() : 
                new NullLogFactory();
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
