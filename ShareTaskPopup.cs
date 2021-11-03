using OpenQA.Selenium;

namespace LambdaTest
{
    public class ShareTaskPopup : PageObjectBase
    {
        private readonly By _shareTaskTitle =
            By.CssSelector("div[id*='_modalContent_modalRootPortal'] div[class*='title']");

        private readonly By _sendButton =
            By.CssSelector("div[id*='_modalContent_modalRootPortal'] button[type='submit']");

        private readonly By _cancelButton =
            By.CssSelector("div[id*='_modalContent_modalRootPortal'] button[type='reset']");
        

        public ShareTaskPopup(IWebDriver webDriver) : base(webDriver)
        {
        }


        public void ClickSendButton()
        {
            _webDriverUtility.ClickAndRetryUntilTimeOut(_sendButton);
        }

        public void ClickCancelButton()
        {
            _webDriverUtility.ClickAndRetryUntilTimeOut(_cancelButton);
        }

        public bool IsShareTaskPopupDisplayed()
        {
            return _webDriverUtility.GetText(_shareTaskTitle).Equals("Share Task");
        }

        public string GetShareTaskUrl()
        {
            return _webDriverUtility.GetElementAttribute(
                By.CssSelector("div[id*='_modalContent_modalRootPortal'] input[value*='https://']"), "value");
        }
    }
}