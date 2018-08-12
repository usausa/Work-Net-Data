namespace Smart.Data.Accessor.Resolver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Smart.ComponentModel;
    using Smart.Data.Accessor.Attributes;
    using Smart.Resolver.Bindings;
    using Smart.Resolver.Handlers;
    using Smart.Resolver.Providers;

    public sealed class AccessorMissingHandler : IMissingHandler
    {
        public IEnumerable<IBinding> Handle(IComponentContainer components, IBindingTable table, Type type)
        {
            if ((type.GetCustomAttribute<DaoAttribute>() != null) ||
                (type.GetCustomAttribute<BuilderAttribute>() != null))
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
