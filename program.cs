using System;
using System.Threading;

namespace gimperbot {

	internal class program {

		public static void update_window( seleniumwrapper webdriver ) {
			string app_version = utils.get_version( );
			Console.WriteLine( $"gimperbot {app_version} by Taiga#5769" );

			while( true ) {
				Thread.Sleep( 1000 );
				Console.Title = $"gimperbot {app_version} / uptime: {utils.get_uptime( )} / page: {webdriver.webdriver_url}";
			}
		}

		public static void update_facebook( seleniumwrapper webdriver ) {
			const string p = "[gimperbot-facebook]";
			message.send_success( p, "started facebook thread" );

			while( true ) {
				int timeout = 60000; // CONFIG
				message.send( p, $"checking in {timeout / 1000} seconds" ); // TODO bo bedzie config
				Thread.Sleep( timeout ); // todo kurwa

				message.send( p, $"newest post:\n\"{webdriver.get_latest_post( )}\"" );
				if( webdriver.check_for_new_posts( ) ) {
					message.send_information( p, $"new post is different, posting comment:\n\"{webdriver.comment_message}\"" );
					webdriver.post_comment( );
				}
				else {
					message.send_error( p, $"post already exists in post history" );
				}
			}
		}

		private static void Main( string[ ] args ) {
			Console.Title = "gimperbot initializing";
			const string p = "[gimperbot-main]";

			/* KURWA JEBAC INI zmien na toml pozniej */

			/* initialize facebook */
			seleniumwrapper facebook = new seleniumwrapper( );
			facebook.comment_message = "Gimper Gimper sraka cię robiła gimperowa sraka cię robiła tata gimpera śpiewa tak kto ma na nazwisko Gimper wypierdala z teamuuuuuuuuuuuu uuuuuuuuuuuuuuuu"; // TODO CONFIG
			facebook.webdriver_url = "https://mbasic.facebook.com/GimperOfficial?v=timeline"; // TODO CONFIG
			facebook.initialize( );

			Console.Clear( ); // hide the logging stuff

			/* start window thread */
			Thread window_thread = new Thread( ( ) => update_window( facebook ) );
			window_thread.Start( );

			Thread.Sleep( 250 ); // Small delay because login message appears before the version string

			/* wait for login */
			message.send_information( p, "log in to your facebook account to continue" );
			while( facebook.check_if_on_login_page( ) ) {
				Thread.Sleep( 2000 );
			}

			/* start facebook thread */
			Thread facebook_thread = new Thread( ( ) => update_facebook( facebook ) );
			facebook_thread.Start( );

			/* final message */
			message.send_success( p, "login was successful, starting facebook thread" );
		}
	}
}