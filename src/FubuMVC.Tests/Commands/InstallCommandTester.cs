using System;
using Fubu;
using FubuCore;
using FubuMVC.Core.Packaging;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using System.IO;

namespace FubuMVC.Tests.Commands
{
    [TestFixture]
    public class InstallCommandTester : InteractionContext<InstallCommand>
    {
        private InstallInput theInput;
        private ApplicationManifest theManifest;

        protected override void beforeEach()
        {
            theInput = new InstallInput()
            {
                AppFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "folder1")
            };

            theManifest = new ApplicationManifest();
            Services.PartialMockTheClassUnderTest();
        }


        private void theManifestFileDoesNotExist()
        {
            MockFor<IFileSystem>().Stub(x => x.FileExists(theInput.AppFolder, ApplicationManifest.FILE)).Return(false);
        }

        private void theManifestFileExists()
        {
            MockFor<IFileSystem>().Stub(x => x.FileExists(theInput.AppFolder, ApplicationManifest.FILE)).Return(true);
            MockFor<IFileSystem>().Stub(x => x.LoadFromFile<ApplicationManifest>(theInput.AppFolder, ApplicationManifest.FILE)).Return(theManifest);
        }


        private void execute()
        {
            ClassUnderTest.Execute(theInput, MockFor<IFileSystem>());
        }

        [Test]
        public void tell_the_user_that_the_command_cannot_continue_if_the_manifest_does_not_exist()
        {
            theManifestFileDoesNotExist();

            ClassUnderTest.Expect(x => x.WriteApplicationManifestDoesNotExist(theInput.AppFolder));

            execute();

            ClassUnderTest.VerifyAllExpectations();
        }

        [Test]
        public void create_environment_run_uses_web_config_by_default_if_it_is_not_specified_in_the_manifest()
        {
            theManifest.ConfigurationFile = null;

            var run = InstallCommand.CreateEnvironmentRun(theInput, theManifest);

            run.ConfigurationFile.ShouldEqual(Path.Combine(theInput.AppFolder, "web.config"));
        }

        [Test]
        public void create_environment_run_uses_the_specific_config_file_if_one_is_given()
        {
            theManifest.ConfigurationFile = "different.config";

            var run = InstallCommand.CreateEnvironmentRun(theInput, theManifest);

            run.ConfigurationFile.ShouldEqual(Path.Combine(theInput.AppFolder, "different.config"));
        }

        [Test]
        public void create_environment_sets_all_the_assembly_and_class_name_properties()
        {
            theManifest.EnvironmentClassName = "some class";
            theManifest.EnvironmentAssembly = "some assembly";

            var run = InstallCommand.CreateEnvironmentRun(theInput, theManifest);

            run.EnvironmentClassName.ShouldEqual(theManifest.EnvironmentClassName);
            run.AssemblyName.ShouldEqual(theManifest.EnvironmentAssembly);
        }

        [Test]
        public void create_environment_defaults_the_application_base_to_the_bin_directory_underneath_the_app_folder()
        {		
            InstallCommand.CreateEnvironmentRun(theInput, theManifest).ApplicationBase
				.ShouldEqual(Path.Combine(theInput.AppFolder, "bin"));
        }


    }
}