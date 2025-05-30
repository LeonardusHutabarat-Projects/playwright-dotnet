using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalAssessment.Utility
{
    public class TestLocator
    {
        public const string ButtonText = "button:has-text('{0}')";
        public const string ImgAltSelector = "img[alt='{0}']";
        public const string H2Text = "h2:has-text('{0}')";
        public const string CarouselText = ".carousel";
        public const string SliderCarouselLi = "li[data-target='#slider-carousel']";
        public const string SliderIndicator = "li[data-target='#slider-carousel'][data-slide-to='{0}']";
        public const string NavBarSelector = "ul.nav.navbar-nav";
        public const string NavbarItemSelector = "ul.nav.navbar-nav >> text={0}";
        public const string FeaturesItemsSelector = ".features_items";

        public const string SignupLoginButton = "text= Signup / Login";
        public const string NameInput = "input[name='name']";
        public const string SignupEmailInput = "input[data-qa='signup-email']";
        public const string SignupButton = "button[data-qa='signup-button']";
        public const string EnterAccountInfoHeader = "h2:has-text('Enter Account Information')";
        public const string MaleGenderRadio = "input[id='id_gender1']";
        public const string FemaleGenderRadio = "input[id='id_gender2']";
        public const string PasswordInput = "input[name='password']";
        public const string DaysDropdown = "#days";
        public const string MonthsDropdown = "#months";
        public const string YearsDropdown = "#years";
        public const string FirstNameInput = "input[name='first_name']";
        public const string LastNameInput = "input[name='last_name']";
        public const string AddressInput = "#address1";
        public const string CountryDropdown = "#country";
        public const string StateInput = "#state";
        public const string CityInput = "#city";
        public const string ZipcodeInput = "#zipcode";
        public const string MobileNumberInput = "#mobile_number";
        public const string CreateAccountButton = "[data-qa='create-account']";
        public const string AccountCreatedMessage = "[data-qa='account-created']";
        public const string ContinueButton = "[data-qa='continue-button']";
        public const string ViewProductLink = "a:has-text('{0}')";
        public const string AnchorHasText = "a:has-text('{0}')";
        public const string FeaturesItemsSection = ".features_items";
        public const string SingleProductHasText = ".single-products:has-text('{0}')";
        public const string ViewProductLinkXpath = "xpath=following-sibling::div[@class='choose']//a[contains(text(),'View Product')]";
        public const string QuantityInput = "input[name='quantity']";
        public const string ButtonHasText = "button:has-text('{0}')";
        public const string LinkHasText = "a:has-text('{0}')";
        public const string ProductInfoHasText = ".productinfo:has-text('{0}')";
        public const string ButtonByText = "text={0}";
        public const string CartModal = "div#cartModal";
        public const string CartModalTitleHasText = "div#cartModal .modal-title:has-text('{0}')";
        public const string CartModalHasText = "div#cartModal:has-text('{0}')";
        public const string CartModalViewCartLink = "div#cartModal a:has-text('{0}')";
        public const string CartModalButtonHasText = "div#cartModal button:has-text('{0}')";
        public const string ViewCartUrlPattern = "view_cart";
        public const string CartRowByProductName = "tr:has-text('{0}')";
        public const string ProductImage = "img";
        public const string CartCellByProductName = "td:has-text('{0}')";
        public const string CartPriceCellByPrice = "td.cart_price:has-text('{0}')";
        public const string CartQuantityCellByQuantity = "td.cart_quantity:has-text('{0}')";
        public const string CartTotalPriceCellByTotal = "td.cart_total p.cart_total_price:has-text('{0}')";
        public const string CartDeleteButton = "a.cart_quantity_delete, button.cart_quantity_delete";
        public const string CartDeleteButtonOnly = "a.cart_quantity_delete";
        public const string ProductRows = "tr[id^='product-']";
        public const string CategoryLink = "a:text-is('{0}')";
        public const string ProductImageWrapper = ".product-image-wrapper";
        public const string ProductInformationPrefix = "div.product-information";
        public const string SpanText = "span:has-text('{0}')";
        public const string PText = "p:has-text('{0}')";
    }
}
