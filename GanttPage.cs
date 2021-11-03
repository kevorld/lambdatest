using System;
using OpenQA.Selenium;

namespace LambdaTest
{
    public class GanttPage : PageObjectBase
    {
        private readonly By _taskNameColumn = By.CssSelector("td[data-column-id='taskName']");
        private readonly By _deleteButtonFromRibbonBar = By.Id("taskRibbonItem_deleteTask");
        private readonly By _addTaskButtonFromRibbonBar = By.Id("taskRibbonItem_addTask");
        private readonly By _taskNameContent = By.CssSelector("td[data-column-id='taskName']");

        private readonly By _activeRow = By.CssSelector(".row-headers .active");
        private readonly By _taskNameInput = By.CssSelector("#Editor .pm-itemeditor-taskname input");

        public GanttPage(IWebDriver webDriver) : base(webDriver)
        {
        }
        
        public void SelectProjectFromLeftPanel(string projectName)
        {
            _webDriverUtility.WaitUntilPageIsFullyLoaded();
            _webDriverUtility.ClickAndRetryUntilTimeOut(By.XPath($"//span[contains(text(),'{projectName}')]"), 10);
        }
        
        public bool RemoveAllTasks()
        {
            _webDriverUtility.SwitchToIframe(By.Id("app_iframe_project_[object Object]"), WebDriverUtility.LONGER_WAIT_TIME);

            if (!string.IsNullOrEmpty(_webDriverUtility.WaitUntilAllTextsReturn(_taskNameColumn)))
            {
                _webDriverUtility.ClickAndRetryUntilTimeOut(By.XPath("//span[contains(text(),'All')]"));

                _webDriverUtility.WebElementDynamicWait(_taskNameColumn, element =>
                {
                    _webDriverUtility.ClickAndRetryUntilTimeOut(_deleteButtonFromRibbonBar);
                    return string.IsNullOrEmpty(_webDriverUtility.GetTexts(_taskNameColumn));
                });
            }

            _webDriverUtility.SwitchBackToMainFrame();
            return true;
        }
        
        public void CreateOneTaskFromToolbar(string taskName)
        {
            _webDriverUtility.SwitchToIframe(By.Id("app_iframe_project_[object Object]"), WebDriverUtility.LONGER_WAIT_TIME);

            _webDriverUtility.ClickAndRetryUntilTimeOut(_addTaskButtonFromRibbonBar);
            int activeRowNumber = Int32.Parse(_webDriverUtility.GetText(_activeRow));
            _webDriverUtility.TypeText(_taskNameInput, taskName);
            _webDriverUtility.PressKeyFromKeyboard(_taskNameInput, Keys.Enter);
            _webDriverUtility.WaitUntilTextBecome(_activeRow, Convert.ToString(++activeRowNumber));
            WebDriverUtility.WaitForMilliSeconds(10);
            
            _webDriverUtility.SwitchBackToMainFrame();
        }
        
        public bool GanttPageIsLoaded()
        {
            _webDriverUtility.SwitchToIframe(By.Id("app_iframe_project_[object Object]"), WebDriverUtility.LONGER_WAIT_TIME);
            bool isDisplayed = _webDriverUtility.WaitUntilElementIsVisible(_taskNameContent, WebDriverUtility.NORMAL_WAIT_TIME);
            _webDriverUtility.SwitchBackToMainFrame();
            return isDisplayed;
        }
        
        public void SelectSingleTask(string taskName)
        {
            _webDriverUtility.SwitchToIframe(By.Id("app_iframe_project_[object Object]"), WebDriverUtility.LONGER_WAIT_TIME);

            _webDriverUtility.ClickAndRetryUntilTimeOut(By.XPath($"//span[contains(text(), '{taskName}')]"));

            _webDriverUtility.SwitchBackToMainFrame();
        }
        
        public void ClickOnButtonFromToolBar(string buttonName)
        {
            _webDriverUtility.SwitchToIframe(By.Id("app_iframe_project_[object Object]"), WebDriverUtility.LONGER_WAIT_TIME);

            By button = By.Id($"taskRibbonItem_{buttonName}");
            
            WebDriverUtility.WaitForMilliSeconds(100);
            _webDriverUtility.WebElementDynamicWait(button,
                element =>
                {
                    _webDriverUtility.ClickAndRetryUntilTimeOut(element);
                    return true;
                });
            
            _webDriverUtility.SwitchBackToMainFrame();
        }
    }
}