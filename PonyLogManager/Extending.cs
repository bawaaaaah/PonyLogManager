using System;
using System.Runtime.CompilerServices;

namespace PonyLogManager
{
	public static class Extending
	{
		/********************
		 *  default LOG to  *
		 * default instance *
		 ********************/
		public static void TRACE(this Exception e, LogManager lm = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, DateTime? dt = null)
		{
			lm = lm ?? LogManager.defaultInstance;
			lm.trace(e, memberName, sourceFilePath, sourceLineNumber, dt);
		}

		public static void DEBUG(this Exception e, LogManager lm = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, DateTime? dt = null)
		{
			lm = lm ?? LogManager.defaultInstance;
			LogManager.defaultInstance.debug(e, memberName, sourceFilePath, sourceLineNumber, dt);
		}

		public static void INFO(this Exception e, LogManager lm = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, DateTime? dt = null)
		{
			lm = lm ?? LogManager.defaultInstance;
			LogManager.defaultInstance.info(e, memberName, sourceFilePath, sourceLineNumber, dt);
		}

		public static void WARN(this Exception e, LogManager lm = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, DateTime? dt = null)
		{
			lm = lm ?? LogManager.defaultInstance;
			LogManager.defaultInstance.warn(e, memberName, sourceFilePath, sourceLineNumber, dt);
		}

		public static void ERROR(this Exception e, LogManager lm = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, DateTime? dt = null)
		{
			lm = lm ?? LogManager.defaultInstance;
			LogManager.defaultInstance.error(e, memberName, sourceFilePath, sourceLineNumber, dt);
		}

		public static void FATAL(this Exception e, LogManager lm = null, [CallerMemberName] string memberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0, DateTime? dt = null)
		{
			lm = lm ?? LogManager.defaultInstance;
			LogManager.defaultInstance.fatal(e, memberName, sourceFilePath, sourceLineNumber, dt);
		}

		public static bool errorLevelIsMinRequire(this LogManager.ErrorType min, LogManager.ErrorType err)
		{
			return LogManager.errorLevelIsMinRequire(min, err);
		}
	}
}

