using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Diagnostics.Behaviors;
using FubuMVC.Diagnostics.Configuration;
using FubuMVC.Diagnostics.Configuration.Policies;
using FubuMVC.Diagnostics.Configuration.SparkPolicies;
using FubuMVC.Diagnostics.Endpoints;
using FubuMVC.Diagnostics.Grids;
using FubuMVC.Diagnostics.Grids.Columns;
using FubuMVC.Diagnostics.Infrastructure;
using FubuMVC.Diagnostics.Models;
using FubuMVC.Diagnostics.Models.Requests;
using FubuMVC.Diagnostics.Navigation;
using FubuMVC.Diagnostics.Notifications;
using FubuMVC.Diagnostics.Partials;
using Spark.Web.FubuMVC;

namespace FubuMVC.Diagnostics
{
    public class FubuDiagnosticsRegistry : FubuPackageRegistry
    {
        public FubuDiagnosticsRegistry()
        {
			this.IncludeDiagnostics(true);
            this.ApplyEndpointConventions(typeof(DiagnosticsEndpointMarker));
            this.Spark(spark => spark
                                    .Policies
                                    .Add(new DiagnosticsEndpointSparkPolicy(typeof (DiagnosticsEndpointMarker)))
                                    .Add<PartialActionSparkPolicy>()
									.Add<NotificationActionSparkPolicy>());

            Services(x =>
                         {
                             // Typically you'd do this in your container but we're keeping this IoC-agnostic
                             x.SetServiceIfNone<IHttpRequest, HttpRequest>();
                             x.SetServiceIfNone<IHttpConstraintResolver, HttpConstraintResolver>();
							 x.SetServiceIfNone<IRequestCacheModelBuilder, RequestCacheModelBuilder>();
                             x.SetServiceIfNone<INavigationMenuBuilder, NavigationMenuBuilder>();
                             x.SetServiceIfNone<IAuthorizationDescriptor, AuthorizationDescriptor>();
                             x.SetServiceIfNone(typeof(IGridService<,>), typeof(GridService<,>));
                             x.SetServiceIfNone(typeof(IGridRowBuilder<,>), typeof(GridRowBuilder<,>));
							 x.SetServiceIfNone(typeof(IGridColumnBuilder<>), typeof(GridColumnBuilder<>));
							 x.SetServiceIfNone<IGridRowProvider<BehaviorGraph, BehaviorChain>, BehaviorGraphRowProvider>();
							 x.SetServiceIfNone<IGridRowProvider<RequestCacheModel, RecordedRequestModel>, RequestCacheRowProvider>();

							 x.Scan(scan =>
							        	{
							        		scan
												.Applies
												.ToThisAssembly()
												.ToAllPackageAssemblies();

											scan
												.AddAllTypesOf<INavigationItemAction>()
												.AddAllTypesOf<INotificationPolicy>();

							        		scan
							        			.ConnectImplementationsToTypesClosing(typeof (IPartialDecorator<>))
							        			.ConnectImplementationsToTypesClosing(typeof (IGridColumnBuilder<>))
												.ConnectImplementationsToTypesClosing(typeof(IGridColumn<>))
												.ConnectImplementationsToTypesClosing(typeof(IGridFilter<>));
							        	});
                         });



        	Policies
        		.ConditionallyWrapBehaviorChainsWith<UnknownObjectBehavior>(
        			call => call.InputType() == typeof (ChainRequest));

            Actions
                .FindWith<PartialActionSource>()
				.FindWith<NotificationActionSource>();

            Output
                .ToJson
                .WhenCallMatches(call => call.OutputType().Name.StartsWith("Json"));
        }
    }
}