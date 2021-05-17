using System;

namespace gimperbot {

	internal class message {

		public static void send(string prefix, string message) {
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine($"{prefix}: {message}");
			Console.ResetColor();
		}

		public static void send_information(string prefix, string message) {
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.WriteLine($"{prefix}: {message}");
			Console.ResetColor();
		}

		public static void send_success(string prefix, string message) {
			Console.ForegroundColor = ConsoleColor.DarkGreen;
			Console.WriteLine($"{prefix}: {message}");
			Console.ResetColor();
		}

		public static void send_error(string prefix, string message) {
			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.WriteLine($"{prefix}: {message}");
			Console.ResetColor();
		}

		public static void clear_chrome_logging_crap() {
			for (int i = 0; i < 3; ++i) {
				Console.SetCursorPosition(0, Console.CursorTop - i);
				Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
			}
		}

		public static void watermark(string app_version) {
			Console.ForegroundColor = ConsoleColor.Magenta;
			Console.WriteLine($"> gimperbot {app_version} by Taiga#5769");
			Console.ResetColor();
		}
	}
}