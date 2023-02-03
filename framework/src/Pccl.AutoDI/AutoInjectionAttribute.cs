using Microsoft.Extensions.DependencyInjection;
using System;

namespace Pccl.AutoDI
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoInjectionAttribute : Attribute
    {
        public virtual ServiceLifetime Lifetime { get; set; }

        public virtual bool TryAdd { get; set; }

        public virtual bool Replace { get; set; }

        public AutoInjectionAttribute(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
        }
    }
}
