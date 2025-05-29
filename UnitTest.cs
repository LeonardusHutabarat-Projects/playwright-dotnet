
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Schema;
using Bogus;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using PercyIO.Playwright;
using TechnicalAssessment.Pages;
using TechnicalAssessment.Utility;

namespace PlaywrightTests;

[TestFixture]
public class TechnicalAssessment : PageTest
{

    [SetUp]
    public async Task SetUp()
    {
        // Launch a headless browser.
        var launchOptions = new BrowserTypeLaunchOptions { Headless = true };
        var browser = await Playwright.Chromium.LaunchAsync(launchOptions);
        
        // Create a context (separate environment).
        var context = await browser.NewContextAsync();

        // Open a page inside that context.
        var page = await context.NewPageAsync();

        // Open BaseURL in the current page.
        await Page.GotoAsync(TestData.BaseURL);

        // Select the "Consent" button on the page.
        var consentSelector = string.Format(TestLocator.ButtonText, TestMessages.ConsentText);
        var consentButton = Page.Locator(consentSelector);

        // When the "Consent" button is visible, the click it.
        if (await consentButton.IsVisibleAsync())
        {
            await consentButton.ClickAsync();
        }

        // Various checks:
        // (1) Verify that the attribute "Website for automation practice" text is visible.
        var imgSelector = string.Format(TestLocator.ImgAltSelector, TestMessages.AutomationPracticeAlt);
        await Expect(Page.Locator(imgSelector)).ToBeVisibleAsync();

        // Take a Percy snapshot of the entire page
        var logoLocator = Page.Locator(imgSelector);
        await Expect(logoLocator).ToBeVisibleAsync();
        Percy.Snapshot(Page, "Home Page");
        await logoLocator.ScreenshotAsync(new LocatorScreenshotOptions { Path = "logo.png" });

        // (2) Verify that "Category" section is visible.
        var h2Selector = string.Format(TestLocator.H2Text, TestData.Category);
        await Expect(Page.Locator(h2Selector)).ToBeVisibleAsync();

        // (3) Verify that "Brand" section is visible.
        var brandsSelector = string.Format(TestLocator.H2Text, TestData.Brands);
        await Expect(Page.Locator(brandsSelector)).ToBeVisibleAsync();

        // (4) Verify that "Features Items" section is visible.
        var featuresItemsSelector = string.Format(TestLocator.H2Text, TestData.FeaturesItems);
        await Expect(Page.Locator(featuresItemsSelector)).ToBeVisibleAsync();
        
        // (5) Verify that "Carousel" section is visible.
        await Page.WaitForSelectorAsync(TestLocator.CarouselText);
        var carouselIsVisible = await Page.IsVisibleAsync(TestLocator.CarouselText);
        Console.WriteLine(string.Format(TestMessages.CarouselVisiblePrefix, carouselIsVisible));

        // Count the number of carousel indicators (dots)
        int totalSlides = await Page.Locator(TestLocator.SliderCarouselLi).CountAsync();

        await Page.WaitForSelectorAsync(TestLocator.SliderCarouselLi);

        for (int i = 0; i < totalSlides; i++)
        {
            string indicatorSelector = string.Format(TestLocator.SliderIndicator, i);
            await Page.ClickAsync(indicatorSelector);

        }

        Console.WriteLine(string.Format(TestMessages.ActiveSlideText, totalSlides));

        if (totalSlides != 3)
        {
            throw new Exception(string.Format(TestMessages.CarouselSlideCount, totalSlides));
        }

        // List of expected menu items
        string[] expectedMenuItems = TestData.ExpectedMenuItems;

        // Wait for the navigation bar to be visible
        await Page.WaitForSelectorAsync(TestLocator.NavBarSelector);

        // Loop through each expected menu item and verify its presence
        foreach (var item in expectedMenuItems)
        {
            var locatorString = string.Format(TestLocator.NavbarItemSelector, item);
            var locator = Page.Locator(locatorString);
            bool itemIsVisible = await locator.IsVisibleAsync();
            Console.WriteLine($"{item}: {(itemIsVisible ? "Found" : "Not Found")}");
            if (!itemIsVisible)
            {
                throw new Exception(string.Format(TestMessages.MenuItemNotFound, item));
            }
        }

        Console.WriteLine(TestMessages.AllMenuItemsVerified);

        await Page.WaitForSelectorAsync(TestLocator.FeaturesItemsSelector);

    }

    [Test, Order(1)]
    public async Task A_UserRegistrationFlow()
    {
        var registrationPage = new RegistrationPage(Page);
        var (fullName, email, password) = await registrationPage.RegisterNewUserAsync();

        // Optionally: Assert the user is logged in
        await Expect(Page.Locator($"text=Logged in as {fullName}")).ToBeVisibleAsync();

        Console.WriteLine($"Registered user: {fullName}");
        Console.WriteLine($"Email: {email}");
        Console.WriteLine($"Password: {password}");
    }

    [Test, Order(2)]
    public async Task B_ProductSearchAndFiltering()
    {
        var productPage = new ProductPage(Page);

        // Step 1 & 2
        await productPage.ExpandMenCategoryAsync();
        await productPage.ClickTshirtsLinkAsync();

        // Step 3
        int productCount = await productPage.GetProductCountAsync();
        if (productCount != 6)
            throw new Exception($"Expected 6 products, but found {productCount}");

        // Step 4-6: Get all product details
        var products = await productPage.GetAllProductDetailsAsync();

        // Step 7-9: Filter logic
        int filteredCount = productPage.CountFilteredProducts(products, TestData.MinPriceFilter, TestData.MaxPriceFilter, TestData.BrandFilter);
        if (filteredCount != 1)
            throw new Exception($"Expected 1 filtered product, but found {filteredCount}");
    }


    [Test, Order(3)]
    public async Task C_ShoppingCartFunctionality()
    {
        var cartPage = new ShoppingCartPage(Page);

        // Add Fancy Green Top to cart
        await cartPage.AddProductToCartAsync(TestData.DressOne);
        // Verify add to cart popup
        bool popupOk = await cartPage.VerifyAddToCartPopupAsync();
        Assert.IsTrue(popupOk, TestMessages.PopupMissingElements);
        // Click "View Cart" in popup
        await cartPage.ClickViewCartOnPopupAsync();
        // Verify cart page is displayed
        await cartPage.VerifyCartPageIsDisplayedAsync();
        // Verify cart row details
        await cartPage.VerifyCartRowAsync(
            TestData.DressOne,
            TestData.DressOnePrice,
            TestData.DressOneQuantity,
            TestData.DressOneTotal
        );

        await Page.GoBackAsync();

        // Navigate to Saree product page first and verify it.
        var bibaSelector = string.Format(TestLocator.AnchorHasText, TestData.BibaLinkText);
        await Page.ClickAsync(bibaSelector);
        await Page.WaitForSelectorAsync(TestLocator.FeaturesItemsSection);
        var sareeCardSelector = string.Format(TestLocator.SingleProductHasText, TestData.SareeName);
        var sareeCard = Page.Locator(sareeCardSelector);
        var viewProductLink = sareeCard.Locator(TestLocator.ViewProductLinkXpath);
        await viewProductLink.ClickAsync();
        await Page.FillAsync(TestLocator.QuantityInput, TestData.SareeQuantity);
        var addToCartSelector = string.Format(TestLocator.ButtonHasText, TestData.AddToCartText);
        await Page.ClickAsync(addToCartSelector);
        Assert.IsTrue(await cartPage.VerifyAddToCartPopupAsync());
        await cartPage.ClickViewCartOnPopupAsync();
        await cartPage.VerifyCartRowAsync(TestData.SareeName, TestData.SareePrice, TestData.SareeQuantity, TestData.SareeTotal);

        await Page.GoBackAsync();

        // Navigate to Products page if needed.
        var productsLinkSelector = string.Format(TestLocator.LinkHasText, TestData.ProductsLinkText);
        await Page.ClickAsync(productsLinkSelector);

        await Page.Locator($"{TestLocator.CategoryLink.Replace("{0}", TestData.MenCategory)}").ClickAsync();
        var jeansSelector = string.Format(TestLocator.AnchorHasText, TestData.JeansText);
        await Page.WaitForSelectorAsync(
            jeansSelector,
            new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible }
        );
        await Page.ClickAsync(jeansSelector);
        await Page.WaitForSelectorAsync(TestLocator.FeaturesItemsSection);
        var jeansCardSelector = string.Format(TestLocator.SingleProductHasText, TestData.JeansName);
        var jeansCard = Page.Locator(jeansCardSelector);
        var viewProductXpath = string.Format(TestLocator.ViewProductLinkXpath, TestData.ViewProductText);
        await jeansCard.Locator(viewProductXpath).ClickAsync();
        await Page.FillAsync(TestLocator.QuantityInput, TestData.JeansQuantity);

        await Page.ClickAsync(addToCartSelector);

        Assert.IsTrue(
            await cartPage.VerifyAddToCartPopupAsync(),
            TestMessages.JeansPopupMissingElements
        );

        await cartPage.ClickViewCartOnPopupAsync();

        await cartPage.DeleteCartRowAsync(TestData.DressOne);
        await cartPage.ConfirmAfterDeletionAsync(TestData.SareeName);
        await cartPage.ConfirmAfterDeletionAsync(TestData.JeansName);

        await cartPage.VerifyCartPageIsDisplayedAsync();
        await cartPage.VerifyCartRowAsync(TestData.SareeName, TestData.SareePrice, TestData.SareeQuantity, TestData.SareeTotal);
        await cartPage.VerifyCartRowAsync(TestData.JeansName, TestData.JeansPrice, TestData.JeansQuantity, TestData.JeansTotal);
    }
}

