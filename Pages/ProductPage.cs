using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TechnicalAssessment.Utility;

namespace TechnicalAssessment.Pages
{
    public class ProductPage
    {
        private readonly IPage _page;

        public ProductPage(IPage page)
        {
            _page = page;
        }

        public async Task ExpandMenCategoryAsync()
        {
            await _page.Locator($"{TestLocator.CategoryLink.Replace("{0}", TestData.MenCategory)}").ClickAsync();
        }

        public async Task ClickTshirtsLinkAsync()
        {
            await _page.Locator($"{TestLocator.CategoryLink.Replace("{0}", TestData.TShirtsMenCategory)}").ClickAsync();
        }

        public async Task<int> GetProductCountAsync()
        {
            var productCard = _page.Locator(TestLocator.ProductImageWrapper);
            return await productCard.CountAsync();
        }

        public async Task<List<(decimal Price, string Brand)>> GetAllProductDetailsAsync()
        {
            int productCount = await GetProductCountAsync();
            var products = new List<(decimal Price, string Brand)>();

            for (int i = 0; i < productCount; i++)
            {
                var cards = _page.Locator(TestLocator.ProductImageWrapper);
                var card = cards.Nth(i);

                // Click "View Product"
                var viewProductSelector = string.Format(TestLocator.ViewProductLink, TestData.ViewProductText);
                await card.Locator(viewProductSelector).ClickAsync();
                await _page.WaitForSelectorAsync($"{TestLocator.ProductInformationPrefix}");

                // Extract price
                var priceLocator = $"{TestLocator.ProductInformationPrefix} " +
                    $"{string.Format(TestLocator.SpanText,
                    TestData.Currency)}";
                var priceText = await _page.Locator(priceLocator).First.InnerTextAsync();
                var priceMatch = Regex.Match(priceText, @"\d+");
                decimal price = priceMatch.Success ? decimal.Parse(priceMatch.Value, CultureInfo.InvariantCulture) : 0;

                // Extract brand
                var brandLocator = $"{TestLocator.ProductInformationPrefix} " +
                   $"{string.Format(TestLocator.PText, TestData.Brand)}";

                var brandText = await _page.Locator(brandLocator).InnerTextAsync();
                string brand = brandText.Split(':').Last().Trim();

                products.Add((price, brand));

                await _page.GoBackAsync();
                await _page.Locator(TestLocator.ProductImageWrapper).First.WaitForAsync();
            }

            return products;
        }

        public int CountFilteredProducts(List<(decimal Price, string Brand)> products, decimal minPrice, decimal maxPrice, string brand)
        {
            return products.Count(p =>
                p.Price >= minPrice &&
                p.Price <= maxPrice &&
                p.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase)
            );
        }
    }
}
