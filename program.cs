using gimperbot.config;
using System;
using System.Collections.Generic;
using System.Threading;

namespace gimperbot {

	internal class program {
		private static List<string> config; // Config properties. Gets its proper values when initialize_config() is called
		private static string app_version = utils.get_version(); // Gets the version from the assembly info
		private const string pf = "[gimperbot-facebook]"; // Prefix for the facebook thread
		private const string pm = "[gimperbot-main]"; // Prefix for the main thread

		// Loads the config if it exists, creates a new one if it doesn't
		private static void initialize_config(string config_file) {
			if (System.IO.File.Exists(config_file)) {
				config = loader.load_config(config_file);
			}
			else {
				loader.write_default_config(config_file);
				initialize_config(config_file); // might get in a horrible loop if there's no write permissions
			}
		}

		// This only updates the uptime. Not very useful, might get deprecated
		private static void update_window(seleniumwrapper webdriver) { // Updates every second
			while (true) {
				Thread.Sleep(1000);
				Console.Title = $"gimperbot {app_version} / uptime: {utils.get_uptime()} / page: {webdriver.webdriver_url}";
			}
		}

		/* Facebook thread, used for checking for new posts & posting the comment in case there's a new post that wasn't seen yet @ facebook.post_history */
		private static void update_facebook(seleniumwrapper webdriver) { 
			message.send_success(pf, "started facebook thread");
			int timeout = (int.Parse(config[(int) loader.type.TIMEOUT])) * 1000; // Loaded from config

			while (true) {
				message.send(pf, $"checking for a new post in {timeout / 1000} seconds");
				Thread.Sleep(timeout); // Waits for x seconds, as specified in the configuration file. Default is 60

				message.send(pf, $"newest post:\n\"{webdriver.get_latest_post()}\""); // string.Concat this bitch ???
				if (webdriver.check_for_new_posts()) {
					message.send_information(pf, $"new post is different, posting comment:\n\"{webdriver.comment_message}\"");
					webdriver.post_comment(); // Posts the comment
				}
				else {
					message.send_error(pf, $"post already exists in post history");
				}
			}
		}

		private static void Main(string[] args) {
			/* da bo$$' most importante part */
			message.watermark(app_version); 

			/* load config - important */
			initialize_config("gimperbot_config.toml");

			/* start initialize selenium webdriver */
			Console.WriteLine("--------------------------------\n[gimperbot-main] initializing chromedriver");
			seleniumwrapper facebook = new seleniumwrapper();
			facebook.comment_message = config[(int) loader.type.MESSAGE]; // Loaded from config
			facebook.webdriver_url = config[(int) loader.type.URL]; // Loaded from config
			facebook.initialize();
			Console.WriteLine("[gimperbot-main] finished\n--------------------------------");
			/* end initialize selenium webdriver */

			/* start window thread */
			Thread window_thread = new Thread(() => update_window(facebook));
			window_thread.Start();

			/* check every 2 seconds if user has logged in */
			message.send_information(pm, "login with your facebook account to continue");
			while (facebook.check_if_on_login_page()) {
				Thread.Sleep(2000);
			}
			
			/* this is my final message change da world goodbye */
			message.send_success(pm, "login was successful, starting facebook thread");

			/* start facebook thread */
			Thread facebook_thread = new Thread(() => update_facebook(facebook));
			facebook_thread.Start();
		}
	}
}