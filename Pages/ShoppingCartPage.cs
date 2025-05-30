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
            //var productCard = _page.Locator($".productinfo:has-text('{productName}')").First;
            await productCard.HoverAsync();
            var addToCartSelector = string.Format(TestLocator.ButtonByText, TestData.AddToCartText);
            await productCard.Locator(addToCartSelector).ClickAsync();
            //await productCard.Locator("text=Add to cart").ClickAsync();
        }

        public async Task<bool> VerifyAddToCartPopupAsync()
        {
            await _page.WaitForSelectorAsync(
                TestLocator.CartModal,
                new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible }
            );

            //await _page.WaitForSelectorAsync("div#cartModal", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });

            //bool hasTitle = await _page.Locator("div#cartModal .modal-title:has-text('Added')").IsVisibleAsync();

            var cartModalTitleSelector = string.Format(TestLocator.CartModalTitleHasText, TestData.CartModalAddedText);
            bool hasTitle = await _page.Locator(cartModalTitleSelector).IsVisibleAsync();


            //bool hasAddedText = await _page.Locator("div#cartModal:has-text('Your product has been added to cart.')").IsVisibleAsync();

            var cartModalProductSelector = string.Format(TestLocator.CartModalHasText, TestData.ProductAddedText);
            bool hasAddedText = await _page.Locator(cartModalProductSelector).IsVisibleAsync();


            //bool hasViewCartLink = await _page.Locator("div#cartModal a:has-text('View Cart')").IsVisibleAsync();

            var cartModalViewSelector = string.Format(TestLocator.CartModalViewCartLink, TestData.ViewCartText);
            bool hasViewCartLink = await _page.Locator(cartModalViewSelector).IsVisibleAsync();

             //bool hasContinueShoppingButton = await _page.Locator("div#cartModal button:has-text('Continue Shopping')").IsVisibleAsync();

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
            //await Expect(_page).ToHaveURLAsync(new Regex("view_cart"));

            var viewCartRegex = new Regex(TestLocator.ViewCartUrlPattern);
            await Expect(_page).ToHaveURLAsync(viewCartRegex);

            //await Expect(_page.Locator("a:has-text('Proceed To Checkout')")).ToBeVisibleAsync();

            var proceedToCheckoutSelector = string.Format(TestLocator.AnchorHasText, TestData.ProceedToCheckoutText);
            await Expect(_page.Locator(proceedToCheckoutSelector)).ToBeVisibleAsync();



        }

        public async Task VerifyCartRowAsync(string productName, string price, string quantity, string total)
        {
            //var cartRow = _page.Locator($"tr:has-text('{productName}')");

            var cartRowSelector = string.Format(TestLocator.CartRowByProductName, productName);
            var cartRow = _page.Locator(cartRowSelector);

            //await Expect(cartRow.Locator("img")).ToBeVisibleAsync();

            await Expect(cartRow.Locator(TestLocator.ProductImage)).ToBeVisibleAsync();

            //await Expect(cartRow.Locator($"td:has-text('{productName}')")).ToBeVisibleAsync();

            var cartCellSelector = string.Format(TestLocator.CartCellByProductName, productName);
            await Expect(cartRow.Locator(cartCellSelector)).ToBeVisibleAsync();

            //await Expect(cartRow.Locator($"td.cart_price:has-text('{price}')")).ToBeVisibleAsync();

            var cartPriceCellSelector = string.Format(TestLocator.CartPriceCellByPrice, price);
            await Expect(cartRow.Locator(cartPriceCellSelector)).ToBeVisibleAsync();


            //await Expect(cartRow.Locator($"td.cart_quantity:has-text('{quantity}')")).ToBeVisibleAsync();

            var cartQuantityCellSelector = string.Format(TestLocator.CartQuantityCellByQuantity, quantity);
            await Expect(cartRow.Locator(cartQuantityCellSelector)).ToBeVisibleAsync();


            //await Expect(cartRow.Locator($"td.cart_total p.cart_total_price:has-text('{total}')")).ToBeVisibleAsync();

            var cartTotalPriceCellSelector = string.Format(TestLocator.CartTotalPriceCellByTotal, total);
            await Expect(cartRow.Locator(cartTotalPriceCellSelector)).ToBeVisibleAsync();


            //await Expect(cartRow.Locator("a.cart_quantity_delete, button.cart_quantity_delete")).ToBeVisibleAsync();

            await Expect(cartRow.Locator(TestLocator.CartDeleteButton)).ToBeVisibleAsync();
        }

        public async Task DeleteCartRowAsync(string productName)
        {
            // 1. Locate the cart row for "Fancy Green Top"
            //var cartRow = _page.Locator($"tr:has-text('{productName}')");

            var cartRowSelector = string.Format(TestLocator.CartRowByProductName, productName);
            var cartRow = _page.Locator(cartRowSelector);


            // 2. Within that row, locate and click the delete button
            //await cartRow.Locator("a.cart_quantity_delete").ClickAsync();
            await cartRow.Locator(TestLocator.CartDeleteButtonOnly).ClickAsync();
            await Expect(cartRow).Not.ToBeVisibleAsync();
        }

        public async Task ConfirmAfterDeletionAsync(string productName)
        {
            // Select all cart rows with id starting with 'product-'
            //var productRows = _page.Locator("tr[id^='product-']");

            var productRows = _page.Locator(TestLocator.ProductRows);

            // Count the number of such rows
            int rowCount = await productRows.CountAsync();

            // Assert there are 2 products
            //Assert.That(rowCount, Is.EqualTo(2), "Cart should have 2 products after deletion.");

            Assert.That(rowCount, Is.EqualTo(TestData.ExpectedProductCountAfterDeletion), TestMessages.CartAfterDeletion);

            //await Expect(_page.Locator($"tr:has-text('{productName}')")).ToBeVisibleAsync();

            var cartRowSelector = string.Format(TestLocator.CartRowByProductName, productName);
            await Expect(_page.Locator(cartRowSelector)).ToBeVisibleAsync();

        }

    }
}
