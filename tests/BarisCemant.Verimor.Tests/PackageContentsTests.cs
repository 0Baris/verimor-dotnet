using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Xunit;

namespace BarisCemant.Verimor.Tests
{
    /// <summary>Packs the library from a scratch copy and inspects the .nupkg like a consumer would.</summary>
    public sealed class PackageContentsTests
    {
        private static readonly string PrivateName = "verimor-sdk" + "-generator";

        [Fact]
        public void The_package_contains_only_the_public_library()
        {
            var package = Pack();
            using var zip = ZipFile.OpenRead(package);
            var names = zip.Entries.Select(e => e.FullName).ToList();

            Assert.Contains("lib/netstandard2.0/BarisCemant.Verimor.dll", names);
            Assert.Contains("README.md", names);
            Assert.Contains("LICENSE", names);
            Assert.DoesNotContain(names, n => n.StartsWith("lib/net8.0", StringComparison.Ordinal));
            Assert.DoesNotContain(names, n => n.StartsWith("lib/net10.0", StringComparison.Ordinal));
            Assert.DoesNotContain(names, n => n.Contains("tests/") || n.Contains(".github/") || n.Contains("spec/"));
            Assert.DoesNotContain(names, n => n.EndsWith(".yaml", StringComparison.Ordinal) || n.EndsWith(".yml", StringComparison.Ordinal));

            foreach (var entry in zip.Entries.Where(e => e.FullName.EndsWith(".nuspec") || e.FullName.EndsWith(".md")))
            {
                using var reader = new StreamReader(entry.Open(), Encoding.UTF8);
                Assert.DoesNotContain(PrivateName, reader.ReadToEnd());
            }
        }

        [Fact]
        public void The_nuspec_declares_the_id_version_and_only_the_allowed_dependencies()
        {
            using var zip = ZipFile.OpenRead(Pack());
            var nuspec = zip.Entries.Single(e => e.FullName.EndsWith(".nuspec", StringComparison.Ordinal));
            using var stream = nuspec.Open();
            var document = XDocument.Load(stream);
            var ns = document.Root!.Name.Namespace;
            var metadata = document.Root.Element(ns + "metadata")!;

            Assert.Equal("BarisCemant.Verimor", metadata.Element(ns + "id")!.Value);
            Assert.Equal(File.ReadAllText(Path.Combine(RepositoryRoot(), "VERSION")).Trim(), metadata.Element(ns + "version")!.Value);
            Assert.Equal("MIT", metadata.Element(ns + "license")!.Value);
            var dependencies = metadata.Descendants(ns + "dependency").Select(d => (string)d.Attribute("id")!).OrderBy(d => d).ToArray();
            Assert.Equal(new[] { "System.ComponentModel.Annotations", "System.Text.Json" }, dependencies);
        }

        [Fact]
        public void Two_packs_produce_the_same_assembly()
        {
            string AssemblyHash(string package)
            {
                using var zip = ZipFile.OpenRead(package);
                using var stream = zip.GetEntry("lib/netstandard2.0/BarisCemant.Verimor.dll")!.Open();
                using var sha = SHA256.Create();
                return Convert.ToHexString(sha.ComputeHash(stream));
            }

            Assert.Equal(AssemblyHash(Pack()), AssemblyHash(Pack()));
        }

        private static string Pack()
        {
            var root = RepositoryRoot();
            var scratch = Path.Combine(Path.GetTempPath(), "verimor-pack-" + Guid.NewGuid().ToString("N"));
            CopyDirectory(Path.Combine(root, "src", "BarisCemant.Verimor"), Path.Combine(scratch, "src", "BarisCemant.Verimor"));
            foreach (var file in new[] { "VERSION", "README.md", "LICENSE" })
            {
                File.Copy(Path.Combine(root, file), Path.Combine(scratch, file));
            }

            var output = Path.Combine(scratch, "dist");
            var dotnet = Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") ?? "dotnet";
            var start = new ProcessStartInfo(dotnet, "pack src/BarisCemant.Verimor/BarisCemant.Verimor.csproj -c Release -o dist -p:ContinuousIntegrationBuild=true")
            {
                WorkingDirectory = scratch,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };
            using var process = Process.Start(start)!;
            var log = process.StandardOutput.ReadToEnd() + process.StandardError.ReadToEnd();
            process.WaitForExit();
            Assert.True(process.ExitCode == 0, log);
            return Directory.GetFiles(output, "*.nupkg").Single(f => !f.EndsWith(".snupkg", StringComparison.Ordinal));
        }

        private static string RepositoryRoot()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null && !File.Exists(Path.Combine(directory.FullName, "VERSION")))
            {
                directory = directory.Parent;
            }

            return directory?.FullName ?? throw new InvalidOperationException("VERSION not found above the test output.");
        }

        private static void CopyDirectory(string source, string destination)
        {
            foreach (var directory in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
            {
                if (!directory.Contains(Path.DirectorySeparatorChar + "bin") && !directory.Contains(Path.DirectorySeparatorChar + "obj"))
                {
                    Directory.CreateDirectory(directory.Replace(source, destination));
                }
            }

            Directory.CreateDirectory(destination);
            foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
            {
                var relative = file.Substring(source.Length);
                if (relative.Contains(Path.DirectorySeparatorChar + "bin" + Path.DirectorySeparatorChar)
                    || relative.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar))
                {
                    continue;
                }

                var target = destination + relative;
                Directory.CreateDirectory(Path.GetDirectoryName(target)!);
                File.Copy(file, target);
            }
        }
    }
}
