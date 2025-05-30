using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TechnicalAssessment.Utility;
using static Microsoft.Playwright.Assertions;

namespace TechnicalAssessment.Pages
{
    public class ShoppingCartPage
    {
        private readonly IPage _page;
        public ShoppingCartPage(IPage page) => _page = page;

        public async Task AddProductToCartAsync(string productName)
        {
            var productCardSelector = string.Format(TestLocator.ProductInfoHasText, productName);
            var productCard = _page.Locator(productCardSelector).First;

            await productCard.HoverAsync();
            var addToCartSelector = string.Format(TestLocator.ButtonByText, TestData.AddToCartText);
            await productCard.Locator(addToCartSelector).ClickAsync();
        }

        public async Task<bool> VerifyAddToCartPopupAsync()
        {
            await _page.WaitForSelectorAsync(
                TestLocator.CartModal,
                new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible }
            );

            var cartModalTitleSelector = string.Format(TestLocator.CartModalTitleHasText, TestData.CartModalAddedText);
            bool hasTitle = await _page.Locator(cartModalTitleSelector).IsVisibleAsync();

            var cartModalProductSelector = string.Format(TestLocator.CartModalHasText, TestData.ProductAddedText);
            bool hasAddedText = await _page.Locator(cartModalProductSelector).IsVisibleAsync();

            var cartModalViewSelector = string.Format(TestLocator.CartModalViewCartLink, TestData.ViewCartText);
            bool hasViewCartLink = await _page.Locator(cartModalViewSelector).IsVisibleAsync();

            var cartModalButtonSelector = string.Format(TestLocator.CartModalButtonHasText, TestData.ContinueShoppingText);
            bool hasContinueShoppingButton = await _page.Locator(cartModalButtonSelector).IsVisibleAsync();

            return hasTitle && hasAddedText && hasViewCartLink && hasContinueShoppingButton;
        }

        public async Task ClickViewCartOnPopupAsync()
        {
            await _page.Locator("div#cartModal a:has-text('View Cart')").ClickAsync();
        }

        public async Task VerifyCartPageIsDisplayedAsync()
        {

            var viewCartRegex = new Regex(TestLocator.ViewCartUrlPattern);
            await Expect(_page).ToHaveURLAsync(viewCartRegex);

            var proceedToCheckoutSelector = string.Format(TestLocator.AnchorHasText, TestData.ProceedToCheckoutText);
            await Expect(_page.Locator(proceedToCheckoutSelector)).ToBeVisibleAsync();
        }

        public async Task VerifyCartRowAsync(string productName, string price, string quantity, string total)
        {
            var cartRowSelector = string.Format(TestLocator.CartRowByProductName, productName);
            var cartRow = _page.Locator(cartRowSelector);

            await Expect(cartRow.Locator(TestLocator.ProductImage)).ToBeVisibleAsync();

            var cartCellSelector = string.Format(TestLocator.CartCellByProductName, productName);
            await Expect(cartRow.Locator(cartCellSelector)).ToBeVisibleAsync();

            var cartPriceCellSelector = string.Format(TestLocator.CartPriceCellByPrice, price);
            await Expect(cartRow.Locator(cartPriceCellSelector)).ToBeVisibleAsync();

            var cartQuantityCellSelector = string.Format(TestLocator.CartQuantityCellByQuantity, quantity);
            await Expect(cartRow.Locator(cartQuantityCellSelector)).ToBeVisibleAsync();

            var cartTotalPriceCellSelector = string.Format(TestLocator.CartTotalPriceCellByTotal, total);
            await Expect(cartRow.Locator(cartTotalPriceCellSelector)).ToBeVisibleAsync();

            await Expect(cartRow.Locator(TestLocator.CartDeleteButton)).ToBeVisibleAsync();
        }

        public async Task DeleteCartRowAsync(string productName)
        {
            var cartRowSelector = string.Format(TestLocator.CartRowByProductName, productName);
            var cartRow = _page.Locator(cartRowSelector);
            await cartRow.Locator(TestLocator.CartDeleteButtonOnly).ClickAsync();
            await Expect(cartRow).Not.ToBeVisibleAsync();
        }

        public async Task ConfirmAfterDeletionAsync(string productName)
        {
            var productRows = _page.Locator(TestLocator.ProductRows);

            // Count the number of such rows
            int rowCount = await productRows.CountAsync();

            // Assert there are 2 products
            Assert.That(rowCount, Is.EqualTo(TestData.ExpectedProductCountAfterDeletion), TestMessages.CartAfterDeletion);

            var cartRowSelector = string.Format(TestLocator.CartRowByProductName, productName);
            await Expect(_page.Locator(cartRowSelector)).ToBeVisibleAsync();

        }

    }
}
