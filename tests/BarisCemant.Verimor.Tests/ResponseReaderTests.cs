using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BarisCemant.Verimor.Http;
using Xunit;

namespace BarisCemant.Verimor.Tests
{
    public sealed class ResponseReaderTests
    {
        public sealed class Sample
        {
            public string? Name { get; set; }
        }

        // Mirrors the generated converters, which throw ArgumentException for a missing required property.
        [JsonConverter(typeof(StrictConverter))]
        public sealed class Strict
        {
        }

        public sealed class StrictConverter : JsonConverter<Strict>
        {
            public override Strict Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
                => throw new ArgumentException("Property is required for class Strict.");

            public override void Write(Utf8JsonWriter writer, Strict value, JsonSerializerOptions options)
                => writer.WriteStartObject();
        }

        private static TransportResponse Response(int status, string body, string? type = "application/json")
            => new TransportResponse(status, Encoding.UTF8.GetBytes(body), type, new Dictionary<string, string>());

        [Fact]
        public void Reads_a_typed_json_body()
            => Assert.Equal("a", ResponseReader.Json<Sample>(Response(200, "{\"Name\":\"a\"}")).Name);

        [Theory]
        [InlineData("")]
        [InlineData("null")]
        [InlineData("not json")]
        [InlineData("[1,2]")]
        public void Rejects_empty_non_json_and_wrong_shaped_success_bodies(string body)
            => Assert.Throws<UnexpectedResponseException>(() => ResponseReader.Json<Sample>(Response(200, body)));

        [Fact]
        public void A_converter_rejecting_the_shape_becomes_an_unexpected_response()
        {
            var error = Assert.Throws<UnexpectedResponseException>(() => ResponseReader.Json<Strict>(Response(200, "{}")));
            Assert.IsType<ArgumentException>(error.InnerException);
        }

        [Fact]
        public void Reads_text_bytes_and_elements()
        {
            Assert.Equal("12.5", ResponseReader.Text(Response(200, "12.5", "text/plain")));
            Assert.Equal(new byte[] { 0x31, 0x32 }, ResponseReader.Bytes(Response(200, "12", "application/pdf")));
            Assert.Equal(1, ResponseReader.Element(Response(200, "{\"a\":1}")).GetProperty("a").GetInt32());
            Assert.Throws<UnexpectedResponseException>(() => ResponseReader.Element(Response(200, "nope")));
        }
    }
}
