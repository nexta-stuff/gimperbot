using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;

namespace gimperbot {

	internal class browser {
		private IWebDriver webdriver;
		public string webdriver_url { get; set; }
		public string webdriver_title { get; set; }
		public string comment_message { get; set; }

		private List<string> post_history = new List<string>( );
		private string p = "[gimperbot-facebook]";

		public void initialize( ) {
			try {
				/* wylacz logging */
				ChromeDriverService service = ChromeDriverService.CreateDefaultService( );
				service.SuppressInitialDiagnosticInformation = true;
				service.EnableVerboseLogging = false;

				/* wylacz wiecej pierdol */
				ChromeOptions options = new ChromeOptions( );
				options.PageLoadStrategy = PageLoadStrategy.Normal;
				options.AddArgument( "--disable-gpu" );
				options.AddArgument( "--disable-crash-reporter" );
				options.AddArgument( "--disable-extensions" );
				options.AddArgument( "--disable-in-process-stack-traces" );
				options.AddArgument( "--disable-logging" );
				options.AddArgument( "--disable-dev-shm-usage" );
				options.AddArgument( "--log-level=3" );

				/* wlacz chrome z opcjami */
				webdriver = new ChromeDriver( service, options );
				webdriver.Url = webdriver_url;
				webdriver_title = webdriver.Title;
			}
			catch( Exception ex ) {
				message.send_error( p, $"initialize() failed: {ex.Message}" );
			}
		}

		public bool check_if_on_login_page( ) {
			try {
				return ( webdriver.Url.Contains( "login" ) || webdriver.Url.Contains( "cookie/consent-page" ) );
			}
			catch( Exception ex ) {
				message.send_error( p, $"check_if_on_login_page() failed: {ex.Message}" );
				return false;
			}
		}

		// chujoza jakas nie dziala pierdole to
		public bool check_if_open( ) {
			return ( String.IsNullOrEmpty( webdriver.CurrentWindowHandle ) );
		}

		public void post_comment( ) {
			try {
				post_history.Add( get_latest_post( ) ); // najpierw dodaj post zebys juz na nim nie komentowal pozniej

				webdriver.FindElement( By.PartialLinkText( "Liczba komentarzy" ) ).Click( ); // kliknij na pierwsze "liczba komentarzy: xx"
				webdriver.FindElement( By.CssSelector( "textarea[name='comment_text']" ) ).SendKeys( comment_message ); // wpierdol do text area nasz tekst pogchamp
				webdriver.FindElement( By.CssSelector( "input[type='submit']" ) ).Click( ); // kliknij na "wyslij"
				webdriver.Url = webdriver_url; // wroc spowrotem na strone (tak jakby odswieza tutaj zamiast w check_for_new_posts( ) bo inaczej nie wykrywa postu na stronie)

				message.send_success( p, "post_comment(): posted comment successfully!" ); // wiadomosc:)
			}
			catch( Exception ex ) {
				message.send_error( p, $"post_comment() failed: {ex.Message}" );
			}
		}

		public string get_latest_post( ) {
			try {
				// XDDDD tak inspector powiedzial to tak jest
				return webdriver.FindElement( By.XPath( "/html/body/div/div/div[2]/div/div[1]/div[2]/div[2]/div/section/article[1]/div/div[1]/div/span/p" ) ).Text.ToString( );
			}
			catch( Exception ex ) {
				message.send_error( p, $"get_latest_post() failed: {ex.Message}" );
				return null;
			}
		}

		public bool check_for_new_posts( ) { // panie boze pomuz mi prosze
			try {
				for (int i = 0; i < post_history.Count; ++i) {
					if( post_history[i] == get_latest_post( ) ) {
						return false;
					}
				}

				return true;
			}
			catch( Exception ex ) {
				message.send_error( p, $"check_for_new_posts() failed: {ex.Message}" );
				return false;
			}
		} // chyba dzialaaXDDD jebana kurwica
	}
}