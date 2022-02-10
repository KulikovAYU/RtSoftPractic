using System.Collections.Generic;
using Autofac;
using Autofac.Core;

namespace ServerApp.Core.Modules
{
    public abstract class ContainerOfModulesBase
    {
        public static ILifetimeScope Ioc { get; private set; }
        protected abstract List<IModule> SubModules { get; set; }

        public ILifetimeScope Configure()
        {
            var builder = new ContainerBuilder();
            RegisterModules(builder);
            
            builder.RegisterBuildCallback(scope => Ioc = scope);
            return builder.Build();
        }
        
        void RegisterModules(ContainerBuilder builder)
        {
            foreach (var moduleOverrides in SubModules)
                builder.RegisterModule(moduleOverrides);
        }
    }
}