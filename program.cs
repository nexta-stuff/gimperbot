using gimperbot.config;
using System;
using System.Collections.Generic;
using System.Threading;

namespace gimperbot {

	internal class program {
		private static List<string> config;
		private static string app_version = utils.get_version();
		private const string pf = "[gimperbot-facebook]";
		private const string pm = "[gimperbot-main]";

		private static void initialize_config(string config_file) {
			if (System.IO.File.Exists(config_file)) {
				config = loader.load_config(config_file);
			}
			else {
				loader.write_default_config(config_file);
				initialize_config(config_file); // recursive o nie!!
			}
		}

		private static void update_window(seleniumwrapper webdriver) { // This only updates the uptime. Not very useful, might get deprecated
			while (true) {
				Thread.Sleep(1000);
				Console.Title = $"gimperbot {app_version} / uptime: {utils.get_uptime()} / page: {webdriver.webdriver_url}";
			}
		}

		private static void update_facebook(seleniumwrapper webdriver) { // Facebook thread, used for posting the comment
			message.send_success(pf, "started facebook thread");
			int timeout = (int.Parse(config[(int) loader.type.TIMEOUT])) * 1000; // config

			while (true) {
				message.send(pf, $"checking in {timeout / 1000} seconds");
				Thread.Sleep(timeout);

				message.send(pf, $"newest post:\n\"{webdriver.get_latest_post()}\"");
				if (webdriver.check_for_new_posts()) {
					message.send_information(pf, $"new post is different, posting comment:\n\"{webdriver.comment_message}\"");
					webdriver.post_comment();
				}
				else {
					message.send_error(pf, $"post already exists in post history");
				}
			}
		}

		private static void Main(string[] args) {
			message.watermark(app_version); // da bo$$' most importante part

			/* load config - important */
			initialize_config("gimperbot_config.toml");

			/* initialize facebook */
			seleniumwrapper facebook = new seleniumwrapper();
			facebook.comment_message = config[(int) loader.type.MESSAGE]; // config
			facebook.webdriver_url = config[(int) loader.type.URL]; // config
			facebook.initialize();

			message.clear_chrome_logging_crap();

			/* start window thread */
			Thread window_thread = new Thread(() => update_window(facebook));
			window_thread.Start();

			Thread.Sleep(250); // Small delay because login message appears before the version string

			/* wait for login */
			message.send_information(pm, "login with your facebook account to continue");
			while (facebook.check_if_on_login_page()) {
				Thread.Sleep(2000);
			}

			/* start facebook thread */
			Thread facebook_thread = new Thread(() => update_facebook(facebook));
			facebook_thread.Start();

			/* this is my final message change da world goodbye */
			message.send_success(pm, "login was successful, starting facebook thread");
		}
	}
}