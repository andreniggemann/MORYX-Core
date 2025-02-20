// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Moryx.Communication;
using Moryx.Communication.Endpoints;
using Moryx.Logging;
using Moryx.Tools.Wcf;
using Endpoint = Moryx.Tools.Wcf.Endpoint;

namespace Moryx.Runtime.Wcf
{
    internal class ConfiguredServiceHost : IConfiguredServiceHost, IEndpointHost
    {
        /// <summary>
        /// Di container of parent plugin for component resolution
        /// </summary>
        private readonly EndpointCollector _collector;
        private readonly PortConfig _portConfig;
        private readonly IModuleLogger _logger;

        private ServiceHost _service;
        private readonly ITypedHostFactory _factory;

        /// <summary>
        /// Endpoint information
        /// </summary>
        private HostConfig _hostConfig;
        private Type _contract;
        private string _endpointAddress;
        private EndpointAttribute _endpointAttribute;

        public ConfiguredServiceHost(ITypedHostFactory factory, IModuleLogger parentLogger, EndpointCollector endpointCollector, PortConfig portConfig)
        {
            _factory = factory;
            _collector = endpointCollector;
            _portConfig = portConfig;

            if (parentLogger != null)
                _logger = parentLogger.GetChild("WcfHosting", GetType());
        }

        /// <summary>
        /// Setup service for given config
        /// </summary>
        /// <param name="contract">Contract type of the host</param>
        /// <param name="config">Config of component</param>
        public void Setup(Type contract, HostConfig config)
        {
            // Create base address depending on chosen binding
            string protocol;
            switch (config.BindingType)
            {
                case ServiceBindingType.NetTcp:
                    protocol = "net.tcp";
                    break;
                default:
                    protocol = "http";
                    break;
            }

            // Create service host
            _service = _factory.CreateServiceHost(contract);
            _contract = contract;
            _endpointAttribute = _contract.GetCustomAttribute<EndpointAttribute>();

            // Configure host
            _service.CloseTimeout = TimeSpan.Zero;

            // Set binding
            Binding binding;
            switch (config.BindingType)
            {
                case ServiceBindingType.BasicHttp: binding = BindingFactory.CreateBasicHttpBinding(config.RequiresAuthentification);
                    break;
                case ServiceBindingType.NetTcp: binding = BindingFactory.CreateNetTcpBinding(config.RequiresAuthentification);
                    break;
                default: binding = BindingFactory.CreateWebHttpBinding();
                    break;
            }

            // Set default timeouts
            binding.OpenTimeout = _portConfig.OpenTimeout != PortConfig.InfiniteTimeout
                ? TimeSpan.FromSeconds(_portConfig.OpenTimeout)
                : TimeSpan.MaxValue;

            binding.CloseTimeout = _portConfig.CloseTimeout != PortConfig.InfiniteTimeout
                ? TimeSpan.FromSeconds(_portConfig.CloseTimeout)
                : TimeSpan.MaxValue;

            binding.SendTimeout = _portConfig.SendTimeout != PortConfig.InfiniteTimeout
                ? TimeSpan.FromSeconds(_portConfig.SendTimeout)
                : TimeSpan.MaxValue;

            binding.ReceiveTimeout = _portConfig.ReceiveTimeout != PortConfig.InfiniteTimeout
                ? TimeSpan.FromSeconds(_portConfig.ReceiveTimeout)
                : TimeSpan.MaxValue;

            // Create endpoint address from config
            var port = config.BindingType == ServiceBindingType.NetTcp ? _portConfig.NetTcpPort : _portConfig.HttpPort;

            // Override binding timeouts if necessary
            if (config is ExtendedHostConfig extendedConfig && extendedConfig.OverrideFrameworkConfig)
            {
                // Override binding timeouts if necessary
                port = extendedConfig.Port;
                binding.OpenTimeout = extendedConfig.OpenTimeout != PortConfig.InfiniteTimeout
                    ? TimeSpan.FromSeconds(extendedConfig.OpenTimeout)
                    : TimeSpan.MaxValue;

                binding.CloseTimeout = extendedConfig.CloseTimeout != PortConfig.InfiniteTimeout
                    ? TimeSpan.FromSeconds(extendedConfig.CloseTimeout)
                    : TimeSpan.MaxValue;

                binding.SendTimeout = extendedConfig.SendTimeout != PortConfig.InfiniteTimeout
                    ? TimeSpan.FromSeconds(extendedConfig.SendTimeout)
                    : TimeSpan.MaxValue;

                binding.ReceiveTimeout = extendedConfig.ReceiveTimeout != PortConfig.InfiniteTimeout
                    ? TimeSpan.FromSeconds(extendedConfig.ReceiveTimeout)
                    : TimeSpan.MaxValue;
            }

            _endpointAddress = $"{protocol}://{_portConfig.Host}:{port}/{config.Endpoint}/";

            var endpoint = _service.AddServiceEndpoint(contract, binding, _endpointAddress);

            // Add  behaviors
            endpoint.Behaviors.Add(new CultureBehavior());

            if (config.BindingType == ServiceBindingType.WebHttp)
            {
                endpoint.Behaviors.Add(new WebHttpBehavior());
                endpoint.Behaviors.Add(new CorsBehaviorAttribute());
            }

            if (config.MetadataEnabled)
            {
                var serviceMetadataBehavior = new ServiceMetadataBehavior
                {
                    HttpGetEnabled = true,
                    HttpGetUrl = new Uri($"http://{_portConfig.Host}:{_portConfig.HttpPort}/Metadata/{config.Endpoint}")
                };
                _service.Description.Behaviors.Add(serviceMetadataBehavior);
            }

            if (config.HelpEnabled)
            {
                var serviceDebugBehavior = _service.Description.Behaviors.Find<ServiceDebugBehavior>();
                serviceDebugBehavior.IncludeExceptionDetailInFaults = true;
                serviceDebugBehavior.HttpHelpPageEnabled = true;
                serviceDebugBehavior.HttpHelpPageUrl = new Uri($"http://{_portConfig.Host}:{_portConfig.HttpPort}/Help/{config.Endpoint}");
            }

            _hostConfig = config;
        }

        #region IConfiguredServiceHost

        /// <inheritdoc />
        public void Start()
        {
            _logger?.Log(LogLevel.Info, "Starting wcf service {0} with version {1}", _endpointAddress,
                _endpointAttribute?.Version ?? "1.0.0.0");

            _service.Open();

            if (_endpointAttribute != null)
            {
                _collector.AddEndpoint(_endpointAddress, new Endpoint
                {
                    Service = _endpointAttribute.Name ?? _contract.Name,
                    Path = _hostConfig.Endpoint,
                    Address = _endpointAddress,
                    Binding = _hostConfig.BindingType,
                    Version = _endpointAttribute.Version,
                    RequiresAuthentication = _hostConfig.RequiresAuthentification
                });
            }
        }

        /// <inheritdoc />
        public void Stop()
        {
            _collector.RemoveEndpoint(_endpointAddress);

            _logger?.Log(LogLevel.Info, "Stopping service {0}", _endpointAddress);

            if (_service != null && _service.State != CommunicationState.Faulted)
            {
                _service.Close();
                _service = null;
            }
        }

        #endregion
    }
}
