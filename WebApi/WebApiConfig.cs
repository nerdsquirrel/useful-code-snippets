public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Automatically configures the log4net system based on the application's configuration settings.
            XmlConfigurator.Configure();

            // unity configuration
            var container = new UnityContainer();
            container.RegisterType<IRepository<tblname>, Repository<tblname>>(new HierarchicalLifetimeManager());  
            // ...                       
            config.DependencyResolver = new UnityResolver(container);
            // if signalr is used
            GlobalHost.DependencyResolver = new SignalRDependencyInjection(container);

            //Register the filter injector            
            var defaultprovider = config.Services.GetFilterProviders().Single(i=>i is ActionDescriptorFilterProvider);
            config.Services.Remove(typeof(IFilterProvider), defaultprovider);
            config.Services.Add(typeof(IFilterProvider), new UnityFilterProvider(container));

            config.EnableCors(new EnableCorsAttribute(ConfigurationManager.AppSettings[AppSettingsKeys.APPSETTINGS_ACCESS_CONTROL_ALLOW_ORIGIN] , "*", "*", "*"));
            config.Filters.Add(new ArpModelValidationAttribute());
            config.Filters.Add(new AppExceptionFilterAttribute());
            config.Filters.Add(new RequireHttpsAttribute());

            config.MessageHandlers.Add(new AntiForgeryHandler());
            config.MessageHandlers.Add(new ResponseWrappingHandler());
            
            config.MapHttpAttributeRoutes(new CentralizedPrefixProvider("api"));

            // formatting
            var settings = config.Formatters.JsonFormatter.SerializerSettings;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.Formatting = Formatting.Indented; 

            // adding new custom type formatter
            config.Formatters.Add(new CsvMediaTypeFormatter(new QueryStringMapping("format", "csv", "text/csv")));

            // supporting only json
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
          
           // for fluent based validation
            FluentValidationModelValidatorProvider.Configure(config);
        }

    }