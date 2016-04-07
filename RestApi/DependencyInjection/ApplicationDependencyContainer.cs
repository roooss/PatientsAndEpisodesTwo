using System.Reflection;
using System.Web.Http.Dependencies;
using Autofac;
using Autofac.Integration.WebApi;
using RestApi.Interfaces;
using RestApi.Models;

namespace RestApi.DependencyInjection
{
    public class ApplicationDependencyContainer
    {
        private readonly ContainerBuilder _autofacContainerBuilder = new ContainerBuilder();

        private IContainer _container;

        public ApplicationDependencyContainer(Assembly apiAssembly)
        {
            _autofacContainerBuilder.RegisterApiControllers(apiAssembly);
            RegisterDependencies();
        }

        private void RegisterDependencies()
        {
            _autofacContainerBuilder.RegisterType<PatientContext>().As<IDatabaseContext>();
        }

        public IDependencyResolver WebApiDependencyResolver
        {
            get { return new AutofacWebApiDependencyResolver(_container); }
        }

        public void OverrideRegistration<TInterface, TClass>() where TInterface : class where TClass : class, TInterface
        {
            _autofacContainerBuilder.RegisterType<TClass>().As<TInterface>().InstancePerLifetimeScope();
        }

        public void Build()
        {
            _container = _autofacContainerBuilder.Build();
        }

        public ILifetimeScope BeginLifetimeScope()
        {
            return _container.BeginLifetimeScope();
        }
    }
}