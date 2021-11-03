using OpenQA.Selenium;
using PMUITestBase.Utilities;

namespace LambdaTest
{
    public class GenericPopup : PageObjectBase
    {
        private readonly By _genericPopupMessageText =
            By.XPath("//div[contains(@id,'undefined_modalContent_modalRootPortal')]//div[contains(@class,'text')]");
        
        
        public GenericPopup(IWebDriver webDriver) : base(webDriver)
        {
        }

        public bool GetPopupMessage(string expectedText)
        {
            return _webDriverUtility.WaitUntilAnyTextContains(_genericPopupMessageText, expectedText);
        }

        public void ClickOnPopupMessage(string buttonText)
        {
            _webDriverUtility.ClickAndRetryUntilTimeOut(By.XPath($"//button[contains(text(),'{buttonText}')]"));
            //When we take actions, it won't be processed straightaway. Need to give our engine sometime.
            WebDriverUtility.WaitForSeconds(3);
        }
    }
}