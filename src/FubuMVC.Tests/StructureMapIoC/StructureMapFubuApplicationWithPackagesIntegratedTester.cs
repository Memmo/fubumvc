using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web.Routing;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.Diagnostics.HtmlWriting;
using FubuMVC.Core.Packaging;
using FubuMVC.Core.Registration;
using FubuMVC.StructureMap.Bootstrap;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;
using System.Linq;

namespace FubuMVC.Tests.StructureMapIoC
{
    [TestFixture, Explicit]
    public class StructureMapFubuApplicationWithPackagesIntegratedTester
    {
        private BehaviorGraph graph;

        [SetUp]
        public void SetUp()
        {
            var root = Path.Combine(Path.GetFullPath("."), "../../../FubuTestApplication");

            FubuMvcPackageFacility.PhysicalRootPath = Path.GetFullPath(root);

            new FakeApplication().Bootstrap(new List<RouteBase>());

            

            graph = ObjectFactory.GetInstance<BehaviorGraph>();

            graph.Behaviors.Each(x => Debug.WriteLine(x.FirstCallDescription()));
        }

        public void TearDown()
        {
            FubuMvcPackageFacility.PhysicalRootPath = null;
        }



        [Test]
        public void should_be_able_to_find_extensions()
        {
            var extensions = FubuExtensionFinder.FindAllExtensions();

            extensions.Each(x => Debug.WriteLine(x.GetType().FullName));

            extensions.Any().ShouldBeTrue();
        }

        [Test]
        public void should_have_actions_for_the_package_assembly()
        {
            graph.Behaviors.Count(
                x => x.FirstCall().HandlerType.Assembly == typeof (TestPackage1.StringController).Assembly)
                .ShouldEqual(3);
        }

        
        public class FakeApplication : FubuStructureMapApplication
        {
            public override FubuRegistry GetMyRegistry()
            {
                return new FubuTestApplicationRegistry();
            }
        }

        public class FubuTestApplicationRegistry : FubuRegistry
        {
            public FubuTestApplicationRegistry()
            {
                IncludeDiagnostics(true);
            }
        }
    }
}