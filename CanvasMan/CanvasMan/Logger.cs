using System;

namespace CanvasMan {
	public static class Logger {
		// Event that subscribers can listen to
		public static event Action<string> LogMessage;

		// Call this method to log a message
		public static void Log(string message) {
			string logEntry = $"{DateTime.Now:HH:mm:ss} - {message}{Environment.NewLine}";
			LogMessage?.Invoke(logEntry);
		}
	}
}
