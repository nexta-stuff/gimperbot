using System;
using System.Diagnostics;
using System.Reflection;

namespace gimperbot {

	internal class utils {

		public static string get_uptime( ) {
			return ( DateTime.Now - Process.GetCurrentProcess( ).StartTime ).ToString( @"hh\:mm\:ss" );
		}

		public static string get_version( ) {
			return Assembly.GetExecutingAssembly( ).GetCustomAttribute<AssemblyInformationalVersionAttribute>( ).InformationalVersion.ToString( );
		}
	}
}