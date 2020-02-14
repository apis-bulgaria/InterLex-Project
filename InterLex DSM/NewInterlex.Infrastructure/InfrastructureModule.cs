namespace NewInterlex.Infrastructure
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Auth;
    using Autofac;
    using Autofac.Core.Activators.Reflection;
    using Core.Interfaces.Gateways.Repositories;
    using Core.Interfaces.Services;
    using Data.Repositories;
    using Interfaces;
    using Services;
    using Module = Autofac.Module;

    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerLifetimeScope();
            builder.RegisterType<GraphConnectionTypeRepository>().As<IGraphConnectionTypeRepository>()
                .InstancePerLifetimeScope();
            builder.RegisterType<LanguageRepository>().As<ILanguageRepository>().InstancePerLifetimeScope();
            builder.RegisterType<LinkTypeRepository>().As<ILinkTypeRepository>().InstancePerLifetimeScope();
            builder.RegisterType<GraphRepository>().As<IGraphRepository>().InstancePerLifetimeScope();
            builder.RegisterType<MasterGraphRepository>().As<IMasterGraphRepository>().InstancePerLifetimeScope();
            
            
            builder.RegisterType<JwtFactory>().As<IJwtFactory>().SingleInstance()
                .FindConstructorsWith(new InternalConstructorFinder());
            builder.RegisterType<LinkInsertService>().As<ILinkInsertService>().SingleInstance();
            builder.RegisterType<JwtTokenHandler>().As<IJwtTokenHandler>().SingleInstance()
                .FindConstructorsWith(new InternalConstructorFinder());
            builder.RegisterType<TokenFactory>().As<ITokenFactory>().SingleInstance();
            builder.RegisterType<JwtTokenValidator>().As<IJwtTokenValidator>().SingleInstance()
                .FindConstructorsWith(new InternalConstructorFinder());
        }
    }

    public class InternalConstructorFinder : IConstructorFinder
    {
        public ConstructorInfo[] FindConstructors(Type targetType)
        {
            return targetType.GetTypeInfo().DeclaredConstructors.Where(c => !c.IsPrivate && !c.IsPublic).ToArray();
        }
    }
}