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
	}
}