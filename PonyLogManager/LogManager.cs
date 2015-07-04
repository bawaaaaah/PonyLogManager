using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PonyLogManager
{
	public class LogManager
	{

		public static LogManager defaultInstance = new LogManager();

		[Flags]
		public enum ErrorType
		{
			OFF = 0,
			TRACE = 1,
			DEBUG = 2,
			INFO = 4,
			WARN = 8,
			ERROR = 16,
			FATAL = 32,
			ALL = 63
		}
		public bool writeDateTime;
		public String logPath;
		public ErrorType writeToConsole;
		public ErrorType writeToFile;

		public delegate void StringExceptCatched(ErrorType eType, String message, string memberName, string sourceFilePath, int sourceLineNumber, DateTime? dt);
		public StringExceptCatched stringExceptCatched = null;
		public delegate void ExceptCatched(ErrorType eType, Exception except, string memberName, string sourceFilePath, int sourceLineNumber, DateTime? dt);
		public ExceptCatched exceptCatched = null;
		public delegate void ObjectExceptCatched(ErrorType eType, Object except, string memberName, string sourceFilePath, int sourceLineNumber, DateTime? dt);
		public ObjectExceptCatched objectExceptCatched = null;

		private static Object loggingConsoleLocker = new Object();
		private Object loggingFileLocker = new Object();

		public LogManager(ErrorType writeToConsole = ErrorType.ERROR, ErrorType writeToFile = ErrorType.ERROR, String logPath = null, bool writeDateTime = false)
		{
			this.logPath = logPath ?? AppDomain.CurrentDomain.BaseDirectory + "logFile.log";
			this.writeToConsole = writeToConsole;
			this.writeToFile = writeToFile;
			this.writeDateTime = writeDateTime;
		}

		public void trace(Object exceptObject, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, DateTime? dt = null)
		{
			log(ErrorType.TRACE, exceptObject, memberName, sourceFilePath, sourceLineNumber, dt);
		}

		public void debug(Object exceptObject, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, DateTime? dt = null)
		{
			log(ErrorType.DEBUG, exceptObject, memberName, sourceFilePath, sourceLineNumber, dt);
		}

		public void info(Object exceptObject, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, DateTime? dt = null)
		{
			log(ErrorType.INFO, exceptObject, memberName, sourceFilePath, sourceLineNumber, dt);
		}

		public void warn(Object exceptObject, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, DateTime? dt = null)
		{
			log(ErrorType.WARN, exceptObject, memberName, sourceFilePath, sourceLineNumber, dt);
		}

		public void error(Object exceptObject, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, DateTime? dt = null)
		{
			log(ErrorType.ERROR, exceptObject, memberName, sourceFilePath, sourceLineNumber, dt);
		}

		public void fatal(Object exceptObject, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, DateTime? dt = null)
		{
			log(ErrorType.FATAL, exceptObject, memberName, sourceFilePath, sourceLineNumber, dt);
		}

		public void log(ErrorType eType, Object exceptObject, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, DateTime? dt = null)
		{
			DateTime dateTime = dt ?? DateTime.Now;
			if (exceptObject.GetType() == typeof(String)) {
				log(eType, (String)exceptObject, memberName, sourceFilePath, sourceLineNumber, dateTime);
			} else if (typeof(Exception).IsAssignableFrom(exceptObject.GetType())) {
				log(eType, (Exception)exceptObject, memberName, sourceFilePath, sourceLineNumber, dateTime);
			} else {
				if (objectExceptCatched != null)
				{
					foreach (ObjectExceptCatched objCatched in objectExceptCatched.GetInvocationList())
					{
						var t = new Thread(new ThreadStart(delegate() { objCatched(eType, exceptObject, memberName, sourceFilePath, sourceLineNumber, dateTime); }));
						t.SetApartmentState(ApartmentState.STA);
						t.Start();
					}
				}
			}
		}

		public void log(ErrorType eType, Exception except, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, DateTime? dt = null)
		{
			DateTime dateTime = dt ?? DateTime.Now;
			logging(eType, (writeDateTime ? "Except datetime : " + dateTime.ToLongDateString() + " at " + dateTime.ToLongTimeString() + Environment.NewLine : "") + parseException(except));
			if (exceptCatched != null)
			{
				foreach (ExceptCatched exptCatched in exceptCatched.GetInvocationList())
				{
					var t = new Thread(new ThreadStart(delegate() { exptCatched(eType, except, memberName, sourceFilePath, sourceLineNumber, dt); }));
					t.SetApartmentState(ApartmentState.STA);
					t.Start();
				}
			}
		}

		public void log(ErrorType eType, String message, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, DateTime? dt = null)
		{
			DateTime dateTime = dt ?? DateTime.Now;
			logging(eType, (writeDateTime ? "Except datetime : " + dateTime.ToLongDateString() + " at " + dateTime.ToLongTimeString() + Environment.NewLine : "") + generateStringException(message, memberName, sourceFilePath, sourceLineNumber));
			if (stringExceptCatched != null)
			{
				foreach (StringExceptCatched strCatched in stringExceptCatched.GetInvocationList())
				{
					var t = new Thread(new ThreadStart(delegate() { strCatched(eType, message, memberName, sourceFilePath, sourceLineNumber, dt); }));
					t.SetApartmentState(ApartmentState.STA);
					t.Start();
				}
			}
		}

		private void logging(ErrorType eType, String message)
		{
			lock (loggingConsoleLocker)
				if (errorLevelIsMinRequire(writeToConsole, eType))
					Console.Error.WriteLine(message);

			lock (loggingFileLocker)
				if (errorLevelIsMinRequire(writeToFile, eType) && logPath != "")
				{
					try
					{
						var tmp = logPath;
						DirectoryInfo dir = new FileInfo(tmp).Directory;
						if (!dir.Exists)
							dir.CreateSubdirectory(new FileInfo(tmp).Directory.FullName);

						System.IO.File.AppendAllText(tmp, message);
					}
					catch (Exception e)
					{
						Console.Error.WriteLine(parseException(e));
					}
				}
		}

		public static bool errorLevelIsMinRequire(ErrorType min, ErrorType err)
		{
			if (min != ErrorType.OFF && ( min <= err || min == ErrorType.ALL))
				return true;
			return false;
		}

		private String parseException(Exception e)
		{
			var seperator = "";
			for (int i = 0; i < e.Source.Length; i++)
				seperator += "#";

			String rturn = "####" + seperator + Environment.NewLine
				+ "# " + e.Source + " #" + Environment.NewLine
				+ "####" + seperator + Environment.NewLine
				+ e.Message + Environment.NewLine;

			for (int i = 0; i < e.Message.Split('\n').Reverse().ToList()[0].Length; i++)
				rturn += "_";

			rturn += Environment.NewLine + e.StackTrace + Environment.NewLine;

			for (int i = 0; i < e.StackTrace.Split('\n').Reverse().ToList()[0].Length; i++)
				rturn += "-";

			return rturn+ Environment.NewLine;
		}

		private String generateStringException(String message, string memberName, string sourceFilePath, int sourceLineNumber)
		{
			var seperator = "";

			for (int i = 0; i < memberName.Length; i++)
				seperator += "#";

			String rturn = "####" + seperator + Environment.NewLine
				+ "# " + memberName + " #" + Environment.NewLine
				+ "####" + seperator + Environment.NewLine
				+ message + Environment.NewLine;

			for (int i = 0; i < message.Split('\n').Reverse().ToList()[0].Length; i++)
				rturn += "_";
			
			var lastTrace = "File: " + sourceFilePath + " line: " + sourceLineNumber;
			rturn += Environment.NewLine + lastTrace + Environment.NewLine;

			for (int i = 0; i < lastTrace.Split('\n').Reverse().ToList()[0].Length; i++)
				rturn += "-";

			return rturn + Environment.NewLine;
		}
	}
}