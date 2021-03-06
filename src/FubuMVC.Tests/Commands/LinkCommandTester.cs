using System.IO;
using Fubu;
using FubuCore;
using FubuMVC.Core.Packaging;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Commands
{
    [TestFixture]
    public class LinkCommandTester : InteractionContext<LinkCommand>
    {
        private LinkInput theInput;
        private ApplicationManifest appManifest;
        private PackageManifest pakManifest;

        protected override void beforeEach()
        {
            theInput = new LinkInput
            {
                PackageFolder = "package",
                AppFolder = "app",
            };

            appManifest = new ApplicationManifest();
            pakManifest = new PackageManifest();
            Services.PartialMockTheClassUnderTest();
        }
        
        private void theManifestFileExists()
        {
            MockFor<IFileSystem>().Stub(x => x.PackageManifestExists(theInput.PackageFolder)).Return(true);
            MockFor<IFileSystem>().Stub(x => x.LoadPackageManifestFrom(theInput.PackageFolder)).Return(pakManifest);
            MockFor<IFileSystem>().Stub(x => x.FileExists(theInput.AppFolder, ApplicationManifest.FILE)).Return(true);
            MockFor<IFileSystem>().Stub(x => x.LoadFromFile<ApplicationManifest>(theInput.AppFolder, ApplicationManifest.FILE)).Return(appManifest);
        }

        private string oneFolderUp(string path)
        {
            return "..{0}{1}".ToFormat(Path.DirectorySeparatorChar, path);
        }

        private void execute()
        {
            ClassUnderTest.Execute(theInput, MockFor<IFileSystem>());
        }

        [Test]
        public void should_link_app_to_package()
        {
            var expectedFolder = oneFolderUp(theInput.PackageFolder);
            theManifestFileExists();

            execute();

            appManifest.LinkedFolders.ShouldContain(expectedFolder);
        }

        [Test]
        public void should_link_app_to_package_with_trailing_slash_for_app()
        {
            var expectedFolder = oneFolderUp(theInput.PackageFolder);
            theInput.AppFolder += Path.DirectorySeparatorChar;
            theManifestFileExists();

            execute();

            appManifest.LinkedFolders.ShouldContain(expectedFolder);
        }
    }
}