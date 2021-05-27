using gimperbot.system;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;

namespace gimperbot.selenium {
	// I still can't do OOP properly :(
	internal class seleniumwrapper {
		private IWebDriver webdriver;
		public string webdriver_url { get; set; }
		public string webdriver_title { get; set; }
		public string comment_message { get; set; }

		private List<string> post_history = new List<string>();
		private string p = "[gimperbot-facebook]";

		public void initialize() {
			try {
				/* turn off logging */
				ChromeDriverService service = ChromeDriverService.CreateDefaultService();
				service.SuppressInitialDiagnosticInformation = true;
				service.EnableVerboseLogging = false;

				/* turn off logging pt2 */
				ChromeOptions options = new ChromeOptions();
				options.PageLoadStrategy = PageLoadStrategy.Normal;
				options.AddArgument("--disable-gpu");
				options.AddArgument("--disable-crash-reporter");
				options.AddArgument("--disable-extensions");
				options.AddArgument("--disable-in-process-stack-traces");
				options.AddArgument("--disable-logging");
				options.AddArgument("--disable-dev-shm-usage");
				options.AddArgument("--log-level=3");

				/* startup chrome with parameters specified above */
				webdriver = new ChromeDriver(service, options);
				webdriver.Url = webdriver_url;
				webdriver_title = webdriver.Title;
			}
			catch (Exception ex) {
				message.send_error(p, $"fatal crash: initialize() failed: {ex.Message}");
				System.Threading.Thread.Sleep(5000);
				Environment.Exit(-1);
			}
		}

		public bool check_if_on_login_page() {
			try {
				return (webdriver.Url.Contains("login") || webdriver.Url.Contains("cookie/consent-page"));
			}
			catch (Exception ex) {
				message.send_error(p, $"check_if_on_login_page() failed: {ex.Message}");
				return false;
			}
		}

		// doesn't work
		public bool is_open() {
			return (webdriver.Title != null);
		}

		public void post_comment() {
			try {
				post_history.Add(get_latest_post()); // add the current post to the post history to avoid commenting on a post more than once

				webdriver.FindElement(By.PartialLinkText("Liczba komentarzy")).Click(); // click on the first "liczba komentarzy: xx", 
													// i should change this bc it doesnt work on the english version of the page
				webdriver.FindElement(By.CssSelector("textarea[name='comment_text']")).SendKeys(comment_message); // insert the text into the textarea
				webdriver.FindElement(By.CssSelector("input[type='submit']")).Click(); // click on the 'send' button
				webdriver.Url = webdriver_url; // go back to the main page (kind of a refresh I guess?)

				message.send_success(p, "post_comment(): posted comment successfully!"); // logging
			}
			catch (Exception ex) {
				message.send_error(p, $"post_comment() failed: {ex.Message}");
			}
		}

		public string get_latest_post() {
			try {
				// this is bad
				return webdriver.FindElement(By.XPath("/html/body/div/div/div[2]/div/div[1]/div[2]/div[2]/div/section/article[1]/div/div[1]/div/span/p")).Text.ToString();
			}
			catch (Exception ex) {
				message.send_error(p, $"get_latest_post() failed: {ex.Message}");
				return null;
			}
		}

		public bool check_for_new_posts() { // this was painful to get working
			try {
				for (int i = 0; i < post_history.Count; ++i) {
					if (post_history[i] == get_latest_post()) {
						return false;
					}
				}

				return true;
			}
			catch (Exception ex) {
				message.send_error(p, $"check_for_new_posts() failed: {ex.Message}");
				return false;
			}
		}
	}
}
