using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pccl.UnitTests
{
    public abstract class TestBase
    {
        public virtual void WriteServices(IServiceCollection services)
        {
            foreach (var service in services)
            {
                System.Diagnostics.Debug.WriteLine($"Service: {service.ServiceType.FullName}\tLifetime: {service.Lifetime}\tInstance:{service.ImplementationType?.FullName}");
            }

        }
    }
}
