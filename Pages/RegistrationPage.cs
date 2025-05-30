using Bogus;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicalAssessment.Utility;

namespace TechnicalAssessment.Pages
{
    public class RegistrationPage
    {
        private readonly IPage _page;
        private readonly Faker _faker = new();

        public RegistrationPage(IPage page)
        {
            _page = page;
        }

        public async Task<(string FullName, string Email, string Password)> RegisterNewUserAsync()
        {
            string firstName = _faker.Name.FirstName();
            string lastName = _faker.Name.LastName();
            string fullName = $"{firstName} {lastName}";
            string randomEmail = $"user_{Guid.NewGuid()}@example.com";
            string password = _faker.Internet.Password();

            await _page.Locator(TestLocator.SignupLoginButton).ClickAsync();
            await _page.FillAsync(TestLocator.NameInput, fullName);
            await _page.FillAsync(TestLocator.SignupEmailInput, randomEmail);
            await _page.ClickAsync(TestLocator.SignupButton);
            await _page.WaitForSelectorAsync(TestLocator.EnterAccountInfoHeader);

            var gender = new[] { Bogus.DataSets.Name.Gender.Male, Bogus.DataSets.Name.Gender.Female }[DateTime.Now.Second % 2];
            await _page.CheckAsync(gender == Bogus.DataSets.Name.Gender.Male
                ? TestLocator.MaleGenderRadio
                : TestLocator.FemaleGenderRadio
            );
            await _page.FillAsync(TestLocator.PasswordInput, password);

            await _page.SelectOptionAsync(TestLocator.DaysDropdown, _faker.Random.Int(1, 31).ToString());
            await _page.SelectOptionAsync(TestLocator.MonthsDropdown, _faker.Random.Int(1, 12).ToString());
            await _page.SelectOptionAsync(TestLocator.YearsDropdown, _faker.Random.Int(1900, 2021).ToString());

            await _page.FillAsync(TestLocator.FirstNameInput, firstName);
            await _page.FillAsync(TestLocator.LastNameInput, lastName);
            await _page.FillAsync(TestLocator.AddressInput, _faker.Address.StreetAddress());

            await _page.SelectOptionAsync(TestLocator.CountryDropdown, _faker.PickRandom(TestData.Countries));
            await _page.FillAsync(TestLocator.StateInput, _faker.Address.State());
            await _page.FillAsync(TestLocator.CityInput, _faker.Address.City());
            await _page.FillAsync(TestLocator.ZipcodeInput, _faker.Address.ZipCode());
            await _page.FillAsync(TestLocator.MobileNumberInput, _faker.Phone.PhoneNumber());

            await _page.ClickAsync(TestLocator.CreateAccountButton);
            await _page.WaitForSelectorAsync(TestLocator.AccountCreatedMessage);
            await _page.ClickAsync(TestLocator.ContinueButton);

            // Wait for login confirmation
            var loggedInText = string.Format(TestMessages.LoggedInAs, fullName);
            await _page.WaitForSelectorAsync($"text={loggedInText}");

            return (fullName, randomEmail, password);
        }
    }
}
