using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;

namespace Bottles.Deployment.Parsing
{
    public class RecipeReader
    {
        public static Recipe ReadFrom(string directory)
        {
            return new RecipeReader(directory).Read();
        }

        private readonly string _directory;
        private readonly IFileSystem _fileSystem = new FileSystem();

        public RecipeReader(string directory)
        {
            _directory = directory;
        }

        public Recipe Read()
        {
            var recipeName = Path.GetFileName(_directory);
            var recipe = new Recipe(recipeName);

            // need to read the recipe control file
            // need to read each host file

            
            _fileSystem.FindFiles(_directory, new FileSet(){
                Include = "*.host"
            }).Each(file =>
            {
                var host = HostReader.ReadFrom(file);
                recipe.RegisterHost(host);
            });

            return recipe;
        }
    }

    public static class HostReader
    {
        public static HostManifest ReadFrom(string fileName)
        {
            var parser = new SettingsParser(fileName);
            new FileSystem().ReadTextFile(fileName, parser.ParseText);

            var hostName = Path.GetFileNameWithoutExtension(fileName);
            var host = new HostManifest(hostName);

            host.RegisterSettings(parser.Settings);
            host.RegisterBottles(parser.References);

            return host;
        }
    }
}