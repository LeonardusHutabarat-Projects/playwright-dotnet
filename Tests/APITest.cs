using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;
using System.Text.Json;
using Microsoft.Playwright.NUnit;

namespace TechnicalAssessment.Tests;

[TestFixture]
public class API_Tests : PageTest
{
    private IAPIRequestContext _apiContext = null!;

    [SetUp]
    public async Task SetUp()
    {
        _apiContext = await Playwright.APIRequest.NewContextAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        await _apiContext.DisposeAsync();
    }

    [Test]
    public async Task ProductsList_ShouldReturnAllProducts()
    {
        // Send GET request
        var response = await _apiContext.GetAsync("https://automationexercise.com/api/productsList");

        TestContext.WriteLine($"Status: {response.Status}");
        var body = await response.TextAsync();
        TestContext.WriteLine($"Body: {body}");

        // Parse and prettify JSON
        using var jsonDoc = JsonDocument.Parse(body);
        var options = new JsonSerializerOptions { WriteIndented = true };
        string prettyJson = JsonSerializer.Serialize(jsonDoc.RootElement, options);


        // Save to local file
        var filePath = Path.Combine(Environment.CurrentDirectory, "productsList_pretty.json");
        await File.WriteAllTextAsync(filePath, prettyJson);
        TestContext.WriteLine($"Prettified JSON saved at: {filePath}");


        // Assert status code
        Assert.IsTrue(response.Ok, $"Expected status 200, got {response.Status}");

        // Parse JSON response
        var json = await response.JsonAsync();

        // Optional: Assert that the JSON contains a "products" array
        Assert.IsTrue(json.Value.TryGetProperty("products", out var products), "Response JSON does not contain 'products' property.");
        Assert.IsTrue(products.ValueKind == JsonValueKind.Array, "'products' is not an array.");

        // Optional: Print the number of products
        TestContext.WriteLine($"Total products: {products.GetArrayLength()}");
    }

    [Test]
    public async Task ProductsList_PostShouldReturn405()
    {
        // Send POST request
        var response = await _apiContext.PostAsync("https://automationexercise.com/api/productsList");

        // Output status and body for debugging
        var status = response.Status;
        var body = await response.TextAsync();
        TestContext.WriteLine($"Status: {status}");
        TestContext.WriteLine($"Body: {body}");

        // Assert HTTP status code is 200
        Assert.AreEqual(200, status, "Expected HTTP status 200");

        // Parse JSON and check responseCode/message
        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;

        Assert.IsTrue(root.TryGetProperty("responseCode", out var code), "No responseCode in JSON");
        Assert.AreEqual(405, code.GetInt32(), "Expected responseCode 405 in JSON");

        Assert.IsTrue(root.TryGetProperty("message", out var msg), "No message in JSON");
        Assert.IsTrue(msg.GetString()!.Contains("This request method is not supported"),
            $"Expected error message, got: {msg.GetString()}");
    }
}
