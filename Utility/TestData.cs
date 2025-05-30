using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalAssessment.Utility
{
    public class TestData
    {
        public const string BaseURL = "https://automationexercise.com/";
        public const string Category = "Category";
        public const string MenCategory = "Men";
        public const string TShirtsMenCategory = "Tshirts";
        public const string Brands = "Brands";
        public const string FeaturesItems = "Features Items";
        public static readonly string[] ExpectedMenuItems = new[]
        {
            "Home",
            "Products",
            "Cart",
            "Signup / Login",
            "Test Cases",
            "API Testing",
            "Video Tutorials",
            "Contact us"
        };
        public const string Brand = "Brand:";
        public const string Currency = "Rs. ";
        public static readonly string[] Countries =
        {
            "India",
            "United States",
            "Canada",
            "Australia",
            "Israel",
            "New Zealand",
            "Singapore"
        };
        public const string EmailDomain = "@example.com";
        public const string ViewProductText = "View Product";
        public const string BibaLinkText = "BIBA";
        public const string AddToCartText = "Add to cart";
        public const string ProductsLinkText = "Products";
        public const string CartModalAddedText = "Added";
        public const string ProductAddedText = "Your product has been added to cart.";
        public const string ViewCartText = "View Cart";
        public const string ContinueShoppingText = "Continue Shopping";
        public const string ProceedToCheckoutText = "Proceed To Checkout";
        public const int ExpectedProductCountAfterDeletion = 2;
        public const decimal MinPriceFilter = 700m;
        public const decimal MaxPriceFilter = 1200m;
        public const string BrandFilter = "Polo";
        public const string DressOne = "Fancy Green Top";
        public const string DressOnePrice = "Rs. 700";
        public const string DressOneQuantity = "1";
        public const string DressOneTotal = "Rs. 700";
        public const string SareeName = "Rust Red Linen Saree";
        public const string SareePrice = "Rs. 3500";
        public const string SareeQuantity = "3";
        public const string SareeTotal = "Rs. 10500";
        public const string JeansText = "Jeans";
        public const string JeansName = "Grunt Blue Slim Fit Jeans";
        public const string JeansPrice = "Rs. 1400";
        public const string JeansQuantity = "4";
        public const string JeansTotal = "Rs. 5600";
    }
}
