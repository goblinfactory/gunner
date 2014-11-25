using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Gunner.Engine;
using Gunner.Tests.Internal;
using Ionic.Zip;
using NUnit.Framework;

namespace Gunner.Tests.Build
{
    [TestFixture]
    public class Packager
    {
        // ------------------------------------------------------------------------------------------
        //                                           PACKAGE SETTINGS
        // ------------------------------------------------------------------------------------------

        private string[] _packageFileList = new[] {     
            @"{gunner}\bin\{build}\Gunner.Engine.dll",
            @"{gunner}\bin\{build}\CommandLine.dll",
            @"{gunner}\bin\{build}\Gunner.exe",
            @"{gunner}\bin\{build}\NetMQ.dll",
            @"{gunner}\bin\{build}\Newtonsoft.Json.dll",
            @"{gunner}\bin\{build}\Gunner.exe.config",
            @"{gunner}\bin\{build}\Test404.bat",
            @"{gunner}\bin\{build}\TestLocal.bat",
            @"{mechanic}\bin\{build}\mechanic.exe",
            @"..\..\..\readme.md"
        };

        string _version =  "Gunner." + BuildHelper.GetVersion() + ".zip";

        // ------------------------------------------------------------------------------------------
        //
        //                                           CREATE PACKAGE 
        //
        // ------------------------------------------------------------------------------------------

        [Test]
        public void CreateGunnerPackage()
        {
            Test.TraceFeature();
            GivenAllTheCorrectFilesAreInTheReleaseFolders();
            WhenICreateAPackage();
            ThenThePackageIsCreated();
            AndThePackageShouldBeNamedAccordingToTheBuildNumber();
            AndThePackageLastModifiedDateTimeShouldBeCurrent();
        }


        // ------------------------------------------------------------------------------------------
        //
        //                                          STEP DEFINITIONS
        //
        // ------------------------------------------------------------------------------------------


        private List<FileInfo> _packageFIs;
        private DirectoryInfo _buildOutputPath;
        private DirectoryInfo _binOutputPath;
        private FileInfo _package;

        public void GivenAllTheCorrectFilesAreInTheReleaseFolders()
        {
            Test.TraceStep();
            var mechanic = @"..\..\..\Gunner.Mechanic";
            var gunner = @"..\..\..\Gunner.Console";
            var files = _packageFileList.Select(f => 
                    f.Replace("{build}", Release.CurrentBuild)
                    .Replace("{mechanic}", mechanic)
                    .Replace("{gunner}", gunner)
                ).Select(f => new FileInfo(f)
            ).ToList();
            files.ForEach(fi => fi.Exists.Should().BeTrue(fi.FullName + " not exist."));
            _packageFIs = files;
        }

        private void WhenICreateAPackage()
        {
            Test.TraceStep();
            _buildOutputPath = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, @"..\..\..\build"));
            _binOutputPath = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, @"..\..\..\build\bin"));
            if (!_binOutputPath.Exists) _binOutputPath.Create();
            Console.WriteLine( _buildOutputPath.FullName);
            var packagePath = _buildOutputPath + @"\" + _version;
            _package = new FileInfo(packagePath);
            _binOutputPath.GetFiles().ToList().ForEach(f=> f.Delete());
            using (ZipFile zip = new ZipFile())
            {
                _packageFIs.ForEach(f =>
                    {
                        zip.AddFile(f.FullName, "");
                        File.Copy(f.FullName,Path.Combine(_binOutputPath.FullName,f.Name));
                    });
                
                zip.Save(packagePath);
            }
        }

        private void ThenThePackageIsCreated()
        {
            Test.TraceStep();
            _buildOutputPath.GetFiles().Where(f => f.Name.Contains(_version)).Should().NotBeNull();
        }

        private void AndThePackageShouldBeNamedAccordingToTheBuildNumber()
        {
            Test.TraceStep();
            var zip = _buildOutputPath.GetFiles().Where(f => f.Name.EndsWith(".zip")).OrderBy(f => f.CreationTime).Last();
            zip.Name.Should().Be(_version);
        }

        private void AndThePackageLastModifiedDateTimeShouldBeCurrent()
        {
            Test.TraceStep();
            DateTime.Now.Subtract(_package.LastWriteTime).Should().BeLessOrEqualTo(TimeSpan.FromSeconds(5));
        }

    }
}
