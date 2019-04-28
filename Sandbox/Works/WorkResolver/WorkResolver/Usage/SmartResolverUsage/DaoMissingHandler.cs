namespace WorkResolver.Usage.SmartResolverUsage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Smart.ComponentModel;
    using Smart.Resolver.Bindings;
    using Smart.Resolver.Handlers;
    using Smart.Resolver.Providers;

    using WorkResolver.Library;

    public sealed class DaoMissingHandler : IMissingHandler
    {
        public IEnumerable<IBinding> Handle(IComponentContainer components, IBindingTable table, Type type)
        {
            if (type.GetCustomAttribute<DaoAttribute>() != null)
            {
                return new[]
                {
                    new Binding(type, new CallbackProvider(type, kernel => kernel.Get<IAccessorFactory>().Create(type)))
                };
            }

            return Enumerable.Empty<IBinding>();
        }
    }
}
