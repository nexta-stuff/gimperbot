using System.Collections.Generic;
using System.IO;

using Tommy;

namespace gimperbot.config {
	internal class loader {
		private static string p = "[gimperbot.config-loader]";

		public enum type {
			MESSAGE = 0,
			URL = 1,
			TIMEOUT = 2
		};

		public static void write_default_config(string config_file) {
			message.send_information(p, "no configuration file found - creating new configuration file...");

			TomlTable default_config = new TomlTable {
				["gimperbot"] =
				{
					[ "message" ] = "Gimpeeer, gimpeeer, gimpa sraka cię robiła, gimperowa sraka cię robiła, tata Gimpera teraz śpiewa, hej, kto ma na nazwisko Gimper wypierdala z teamu huuu uuuu uuuu",
					[ "url" ] = "https://mbasic.facebook.com/GimperOfficial?v=timeline",
					[ "timeout" ] = 60,
				},
			};

			using (StreamWriter writer = new StreamWriter(File.OpenWrite(config_file))) {
				default_config.WriteTo(writer);
				writer.Flush();
			}
		}

		public static List<string> load_config(string config_file) {
			List<string> result = new List<string>();
			TomlTable configuration;

			using (StreamReader file = new StreamReader(File.OpenRead(config_file))) {
				try {
					configuration = TOML.Parse(file);
				}
				catch (TomlParseException ex) {
					configuration = ex.ParsedTable;

					foreach (TomlSyntaxException syntax_ex in ex.SyntaxErrors) {
						message.send_error(p, $"error in configuration file: {syntax_ex.Column}:{syntax_ex.Line}: {syntax_ex.Message}");
						message.send_information(p, "attempting to load configuration file either way");
					}
				}

				// i cant figure this fucking api out, so...
				result.Add(configuration["gimperbot"]["message"]);
				result.Add(configuration["gimperbot"]["url"]);
				result.Add(configuration["gimperbot"]["timeout"]);
			}

			message.send_success(p, $"loaded configuration file \"{config_file}\" successfully!");
			return result;
		}
	}
}