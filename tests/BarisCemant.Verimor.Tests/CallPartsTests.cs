using System.Collections.Generic;
using System.Globalization;
using BarisCemant.Verimor.Http;
using Xunit;

namespace BarisCemant.Verimor.Tests
{
    public sealed class CallPartsTests
    {
        [Fact]
        public void Skips_null_values_and_formats_scalars_invariantly()
        {
            var call = new CallParts();
            call.AddQuery("skip", null);
            call.AddQuery("n", 42L);
            call.AddQuery("t", true);
            call.AddQuery("f", false);
            call.AddQuery("s", "text");
            call.AddQuery("ids", new List<long> { 1, 2, 3 });
            call.AddQuery("d", 1.5d);

            Assert.False(call.Query.ContainsKey("skip"));
            Assert.Equal("42", call.Query["n"]);
            Assert.Equal("true", call.Query["t"]);
            Assert.Equal("false", call.Query["f"]);
            Assert.Equal("text", call.Query["s"]);
            Assert.Equal("1,2,3", call.Query["ids"]);
            Assert.Equal("1.5", call.Query["d"]);
        }

        [Fact]
        public void Formatting_does_not_depend_on_the_current_culture()
        {
            var previous = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("tr-TR");
                var call = new CallParts();
                call.AddQuery("d", 1.5d);
                Assert.Equal("1.5", call.Query["d"]);
            }
            finally
            {
                CultureInfo.CurrentCulture = previous;
            }
        }
    }
}
