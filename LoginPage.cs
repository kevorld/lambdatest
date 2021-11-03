using OpenQA.Selenium;

namespace LambdaTest
{
    public class LoginPage : PageObjectBase
    {
        public LoginPage(IWebDriver driver) : base(driver)
        {
        }

        private IWebElement EmailTextField => _webDriver.FindElement(By.Id("ctl00_CPHBody_Email"));
        private IWebElement PasswordTextField => _webDriver.FindElement(By.Id("ctl00_CPHBody_Password"));
        private IWebElement LoginButton => _webDriver.FindElement(By.Id("ctl00_CPHBody_btnLogin"));
        
        public void EnterEmail(string email)
        {
            //Clear text box
            EmailTextField.Clear();
            //Enter text
            EmailTextField.SendKeys(email);
        }
        
        public void EnterPassword(string password)
        {
            //Clear text box
            PasswordTextField.Clear();
            //Enter text
            PasswordTextField.SendKeys(password);
        }

        public void Submit()
        {
            LoginButton.Click();
        }
    }
}