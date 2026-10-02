using System;
using System.IO;
using System.Linq;
using BarisCemant.Verimor.Tests.Support;
using Xunit;

namespace BarisCemant.Verimor.Tests
{
    public sealed class DocumentationContractTests
    {
        private static string Root()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory != null && !File.Exists(Path.Combine(directory.FullName, "VERSION")))
            {
                directory = directory.Parent;
            }

            return directory!.FullName;
        }

        private static string Read(string relative) => File.ReadAllText(Path.Combine(Root(), relative));

        [Theory]
        [InlineData("tr")]
        [InlineData("en")]
        public void The_operation_tables_list_every_service_method(string language)
        {
            var table = Read(Path.Combine("docs", language, "operations.md"));
            foreach (var operation in Contract.Load())
            {
                Assert.Contains("`" + operation.Proxy + "`", table);
                Assert.Contains("`" + operation.OperationId + "`", table);
            }
        }

        [Fact]
        public void Both_readmes_state_the_unofficial_and_offline_status()
        {
            var turkish = Read("README.md");
            var english = Read("README.en.md");
            Assert.Contains("resmî değildir", turkish);
            Assert.Contains("canlı Verimor servisine karşı henüz doğrulanmamıştır", turkish);
            Assert.Contains("unofficial", english);
            Assert.Contains("has not been validated against the live Verimor services", english);
        }

        [Fact]
        public void No_document_calls_the_product_pbx()
        {
            var files = Directory.GetFiles(Path.Combine(Root(), "docs"), "*.md", SearchOption.AllDirectories)
                .Concat(new[] { Path.Combine(Root(), "README.md"), Path.Combine(Root(), "README.en.md") });
            foreach (var file in files)
            {
                Assert.DoesNotContain("PBX", File.ReadAllText(file), StringComparison.OrdinalIgnoreCase);
            }
        }

        [Fact]
        public void Every_guide_exists_in_both_languages()
        {
            var turkish = Directory.GetFiles(Path.Combine(Root(), "docs", "tr")).Select(Path.GetFileName).OrderBy(n => n);
            var english = Directory.GetFiles(Path.Combine(Root(), "docs", "en")).Select(Path.GetFileName).OrderBy(n => n);
            Assert.Equal(turkish, english);
            var examples = Path.Combine(Root(), "examples");
            Assert.Equal(6, Directory.GetFiles(examples, "*.cs").Count(file => Path.GetFileName(file) != "Program.cs"));
            Assert.Equal(72, Directory.GetDirectories(Path.Combine(examples, "Operations"))
                .Sum(product => Directory.GetFiles(product, "*.cs").Length));
        }
    }
}
