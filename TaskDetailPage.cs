using OpenQA.Selenium;

namespace LambdaTest
{
    public class TaskDetailPage : PageObjectBase
    {
        private readonly By _taskDetailPageTitle = By.CssSelector("div[class*='taskPage'] span[class*='title']");
        private readonly By _moreOptions = By.CssSelector("div[class*='taskPage'] div[class*='more']");

        public TaskDetailPage(IWebDriver webDriver) : base(webDriver)
        {
        }

        public bool IsTaskDetailPageDisplayed(string taskName)
        {
            return _webDriverUtility.GetText(_taskDetailPageTitle).Equals(taskName);
        }

        public void MoreActions(string action)
        {
            _webDriverUtility.ClickAndRetryUntilTimeOut(_moreOptions);
            _webDriverUtility.ClickAndRetryUntilTimeOut(By.CssSelector($"div[id*='taskHeaderMenu'] div[data-dom-id='{action}']"));
        }
    }
}