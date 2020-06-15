namespace NewInterlex.Core
{
    using Autofac;
    using Interfaces.UseCases;
    using UseCases;

    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ExchangeRefreshTokenUseCase>().As<IExchangeRefreshTokenUseCase>()
                .InstancePerLifetimeScope();
            builder.RegisterType<LoginUseCase>().As<ILoginUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<RegisterUserUseCase>().As<IRegisterUserUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<GetMetaInfoUseCase>().As<IGetMetaInfoUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<SaveGraphUseCase>().As<ISaveGraphUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<GetGraphUseCase>().As<IGetGraphUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<CreateMasterGraphUseCase>().As<ICreateMasterGraphUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<MasterGraphDetailsUseCase>().As<IMasterGraphDetailsUseCase>()
                .InstancePerLifetimeScope();
            builder.RegisterType<SaveGraphDataUseCase>().As<ISaveGraphDataUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<MasterGraphGetAllUseCase>().As<IMasterGraphGetAll>().InstancePerLifetimeScope();
            builder.RegisterType<InsertLinksUseCase>().As<IInsertLinksUseCase>().InstancePerLifetimeScope();
            builder.RegisterType<ExportReportUseCase>().As<IExportReportUseCase>().InstancePerLifetimeScope();
        }
    }
}