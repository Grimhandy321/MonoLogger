using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    class MessageTests : AbstractTest
    {
        [Fact]
        public async Task MessageTest_ShouldReturnAccomplished_ForValidJson()
        {
            using var client = new HttpClient();

            // Arrange
            var json = "{\"content\":\"Hello\"}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync($"{_url}/message-schema", content);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("accomplished", responseBody);
        }

        [Fact]
        public async Task MessageTest_ShouldReturnErrorMessage_ForInvalidJson()
        {
            using var client = new HttpClient();

            // Arrange
            var invalidJson = "Invalid JSON";
            var content = new StringContent(invalidJson, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync($"{_url}/message-schema", content);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Contains("Exception", responseBody);
        }

        [Fact]
        public async Task MessageTest_ShouldReturnError_ForEmptyString()
        {
            using var client = new HttpClient();

            // Arrange
            var emptyJson = "";
            var content = new StringContent(emptyJson, Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync($"{_url}/message-schema", content);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Contains("Exception", responseBody);
        }

        [Fact]
        public async Task MessageTest_ShouldReturnError_ForNullBody()
        {
            using var client = new HttpClient();

            // Arrange
            StringContent? content = null; // Simulate null body

            // Act
            var response = await client.PostAsync($"{_url}/message-schema", content ?? new StringContent(""));
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Contains("Exception", responseBody);
        }
    }
}
