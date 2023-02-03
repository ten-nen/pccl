using Pccl.AutoDI;
using Microsoft.Extensions.DependencyInjection;

namespace Pccl.UnitTests
{
    public class InjectTest : TestBase
    {
        public InjectTest()
        {
        }

        [Fact]
        public void TestInjectUility()
        {
            var services = new ServiceCollection();
            services.AddServices<ITestService>(ServiceLifetime.Scoped,  null, assemblies: typeof(ITestService).Assembly);

            WriteServices(services);

            var builder = services.BuildServiceProvider();
            var testServs = builder.GetServices<ITestService>();

            Assert.Equal(testServs.Count(), 1);
        }

        [Fact]
        public void TestAutoInjectByInterface()
        {
            var services = new ServiceCollection();
            services.AddAutoRegisterServices(typeof(ITestService1).Assembly);

            WriteServices(services);

            var builder = services.BuildServiceProvider();
            var testServs = builder.GetServices<ITestService1>();

            Assert.Equal(testServs.Count(), 1);
        }

        [Fact]
        public void TestAutoInjectByAttribute()
        {
            var services = new ServiceCollection();
            services.AddAutoRegisterServices(typeof(ITestService2).Assembly);

            WriteServices(services);

            var builder = services.BuildServiceProvider();
            var testServs = builder.GetServices<ITestService2>();

            Assert.Equal(1, testServs.Count());
        }

        [Fact]
        public void TestAutoInjectGenericService()
        {
            var services = new ServiceCollection();
            services.AddServices(typeof(ITestGenericService<>), ServiceLifetime.Scoped,  null, assemblies: typeof(ITestService).Assembly);

            WriteServices(services);

            var builder = services.BuildServiceProvider();
            var testServs = builder.GetServices(typeof(ITestGenericService<string>));
            Assert.Equal(testServs.Count(), 1);


            testServs = builder.GetServices(typeof(ITestGenericServiceDefined));
            Assert.Equal(testServs.Count(), 1);
        }
    }


    public interface ITestService { }
    public class TestImplementation : ITestService { }

    public interface ITestService1 { }
    public class TestImplementation1 : ITestService1, IScopedService { }

    public interface ITestService2 { }
    [AutoInjection(ServiceLifetime.Scoped)]
    public class TestImplementation2 : ITestService2 { }


    public interface ITestGenericService<T> where T : class { }
    public interface ITestGenericServiceDefined : ITestGenericService<string> { }
    public class TestGenericService : ITestGenericServiceDefined { }
}
