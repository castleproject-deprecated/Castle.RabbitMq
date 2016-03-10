namespace Castle.RabbitMq
{
	using System;
	using System.Diagnostics;


	///	<summary>
	///	In order to	not	carry a	dependency on a	log	lib...
	///	</summary>
	public static class LogAdapter
	{
		public static bool LogEnabled;

		static LogAdapter()
		{
			LogDebugFn = (c, m, ex) =>
			{
				var msg = string.Format("{0}: {1}", c, m);
				Console.Out.WriteLine(msg);
				if (Debugger.IsLogging())
				{
					Debugger.Log(1, "DEBUG", msg);
				}
			};

			LogErrorFn = (c, m, ex) =>
			{
				var msg = string.Format("{0}: {1}", c, m);
				Console.Error.WriteLine(msg);
				if (Debugger.IsLogging())
				{
					Debugger.Log(1, "ERROR", msg);
				}
			};

			LogWarnFn = (c, m, ex) =>
			{
				var msg = string.Format("{0}: {1}", c, m);
				Console.Error.WriteLine(msg);
				if (Debugger.IsLogging())
				{
					Debugger.Log(1, "WARN", msg);
				}
			};
		}

		public static Action<string, string, Exception> LogDebugFn { get; set; }
		public static Action<string, string, Exception> LogErrorFn { get; set; }
		public static Action<string, string, Exception> LogWarnFn { get; set; }

		public static void LogDebug(string context, string message)
		{
			LogDebugFn(context, message, null);
		}

		public static void LogDebug(string context, string message, Exception ex)
		{
			LogDebugFn(context, message, ex);
		}

		public static void LogError(string context, string message, Exception ex)
		{
			LogErrorFn(context, message, ex);
		}

		public static void LogWarn(string context, string message, Exception ex = null)
		{
			LogWarnFn(context, message, ex);
		}
	}
}
