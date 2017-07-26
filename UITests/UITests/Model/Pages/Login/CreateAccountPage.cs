using OpenQA.Selenium;
using UITests.WebDriverLib;

namespace UITests.Model.Pages
{
    
    public class CreateAccountPage : BasePage
    {
        private By firstNameInput = By.Id("FirstName");
        private By lastNameInput = By.Id("LastName");
        private By emailInput = By.Id("Email");
        private By passwordInput = By.Id("Password");
        private static By submit = By.Id("createaccountsubmit");


        public CreateAccountPage(IWebDriver driver1)
            : base(driver1, submit)
        {            
        }

        
        public void FillInForm(string firstname, string lastname, string email, string password)
        {
            WebDriver.GetElement(firstNameInput).SetText(firstname);
            WebDriver.GetElement(lastNameInput).SetText(lastname);
            WebDriver.GetElement(emailInput).SetText(email);
            WebDriver.GetElement(passwordInput).SetText(password);          
        }
        
                
        public void Submit()
        {
            WebDriver.ClickIt(submit);
        }

        public class AccountConfirmPage : BasePage
        {

            public AccountConfirmPage(IWebDriver driver1)
                : base(driver1)
            {
            }
            
        }
    }
}
