using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using OpenTelemetry;
using OpenTelemetry.Instrumentation.AspNet;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace AspNetWcfDemo
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private MeterProvider? meterProvider;
        private TracerProvider? tracerProvider;

        protected void Application_Start()
        {
            var originalOnRequestStartedCallback = TelemetryHttpModule.Options.OnRequestStartedCallback;
            TelemetryHttpModule.Options.OnRequestStartedCallback = (activity, httpContext) =>
            {
                if (originalOnRequestStartedCallback != null)
                {
                    originalOnRequestStartedCallback(activity, httpContext);
                }
                
                var requestContext = httpContext.Request.RequestContext;
                var routeData = requestContext.RouteData;
                string template = null;

                if (routeData.Route is Route route)
                {
                    var vpd = route.GetVirtualPath(requestContext, routeData.Values);
                    template = "/" + vpd.VirtualPath;
                }

                if (template != null)
                {
                    activity.DisplayName = template;
                    activity.SetTag("http.route", template);
                }
            };

            this.meterProvider = Sdk.CreateMeterProviderBuilder()
                .AddAspNetInstrumentation()
                .AddOtlpExporter()
                .Build();
            this.tracerProvider = Sdk.CreateTracerProviderBuilder()
                .SetSampler(new AlwaysOnSampler())
                .AddAspNetInstrumentation()
                .AddWcfInstrumentation()
                .AddOtlpExporter()
                .Build();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_End()
        {
            this.meterProvider?.Dispose();
            this.tracerProvider?.Dispose();
        }
    }
}
