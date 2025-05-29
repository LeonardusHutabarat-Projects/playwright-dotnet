using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Microsoft.Playwright.Assertions;

namespace TechnicalAssessment.Pages
{
    public class ShoppingCartPage
    {
        private readonly IPage _page;
        public ShoppingCartPage(IPage page) => _page = page;

        public async Task AddProductToCartAsync(string productName)
        {
            var productCard = _page.Locator($".productinfo:has-text('{productName}')").First;
            await productCard.HoverAsync();
            await productCard.Locator("text=Add to cart").ClickAsync();
        }

        public async Task<bool> VerifyAddToCartPopupAsync()
        {
            await _page.WaitForSelectorAsync("div#cartModal", new PageWaitForSelectorOptions { State = WaitForSelectorState.Visible });

            bool hasTitle = await _page.Locator("div#cartModal .modal-title:has-text('Added')").IsVisibleAsync();
            bool hasAddedText = await _page.Locator("div#cartModal:has-text('Your product has been added to cart.')").IsVisibleAsync();
            bool hasViewCartLink = await _page.Locator("div#cartModal a:has-text('View Cart')").IsVisibleAsync();
            bool hasContinueShoppingButton = await _page.Locator("div#cartModal button:has-text('Continue Shopping')").IsVisibleAsync();

            return hasTitle && hasAddedText && hasViewCartLink && hasContinueShoppingButton;
        }

        public async Task ClickViewCartOnPopupAsync()
        {
            await _page.Locator("div#cartModal a:has-text('View Cart')").ClickAsync();
        }

        public async Task VerifyCartPageIsDisplayedAsync()
        {
            await Expect(_page).ToHaveURLAsync(new Regex("view_cart"));
            await Expect(_page.Locator("a:has-text('Proceed To Checkout')")).ToBeVisibleAsync();
        }

        public async Task VerifyCartRowAsync(string productName, string price, string quantity, string total)
        {
            var cartRow = _page.Locator($"tr:has-text('{productName}')");

            await Expect(cartRow.Locator("img")).ToBeVisibleAsync();
            await Expect(cartRow.Locator($"td:has-text('{productName}')")).ToBeVisibleAsync();
            await Expect(cartRow.Locator($"td.cart_price:has-text('{price}')")).ToBeVisibleAsync();
            await Expect(cartRow.Locator($"td.cart_quantity:has-text('{quantity}')")).ToBeVisibleAsync();
            await Expect(cartRow.Locator($"td.cart_total p.cart_total_price:has-text('{total}')")).ToBeVisibleAsync();
            await Expect(cartRow.Locator("a.cart_quantity_delete, button.cart_quantity_delete")).ToBeVisibleAsync();
        }

        public async Task DeleteCartRowAsync(string productName)
        {
            // 1. Locate the cart row for "Fancy Green Top"
            var cartRow = _page.Locator($"tr:has-text('{productName}')");

            // 2. Within that row, locate and click the delete button
            await cartRow.Locator("a.cart_quantity_delete").ClickAsync();

            await Expect(cartRow).Not.ToBeVisibleAsync();
        }

        public async Task ConfirmAfterDeletionAsync(string productName)
        {
            // Select all cart rows with id starting with 'product-'
            var productRows = _page.Locator("tr[id^='product-']");

            // Count the number of such rows
            int rowCount = await productRows.CountAsync();

            // Assert there are 2 products
            Assert.That(rowCount, Is.EqualTo(2), "Cart should have 2 products after deletion.");

            await Expect(_page.Locator($"tr:has-text('{productName}')")).ToBeVisibleAsync();

        }

    }
}
