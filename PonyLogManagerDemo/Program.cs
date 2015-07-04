using System;
using PonyLogManager;

namespace PonyLogManagerDemo
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			LogManager customLogManager = new LogManager(LogManager.ErrorType.OFF, LogManager.ErrorType.FATAL, "customlogfile.log", true);
			// all delegate where execute in thread
			LogManager.defaultInstance.objectExceptCatched += objLog;
			LogManager.defaultInstance.writeToConsole = LogManager.ErrorType.TRACE;

			DemoClass demoClass = new DemoClass(){ jobs = "Demo for PonyLogManager" };
			demoClass.sw.Start();

			LogManager.defaultInstance.trace("Start demo");
			try{
				int a = 0, b = 0;
				a /= b;
			} catch (Exception e) {
				e.ERROR();
				// you can also
				LogManager.defaultInstance.debug(e);
				// or
				e.DEBUG(customLogManager);
				customLogManager.debug(e);
			}
			demoClass.sw.Stop();
			LogManager.defaultInstance.debug(demoClass);
			LogManager.defaultInstance.trace("End demo");
			Console.Read();
		}

		private static void objLog(LogManager.ErrorType eType, Object except, string memberName, string sourceFilePath, int sourceLineNumber, DateTime? dt){
			if(!LogManager.ErrorType.DEBUG.errorLevelIsMinRequire(eType))
				return;
			if (except.GetType().IsAssignableFrom(typeof(DemoClass))) {
				DemoClass demo = (DemoClass)except;
				Console.WriteLine(demo.jobs + " in " + demo.sw.ElapsedMilliseconds + "ms");
			}
		}

		private class DemoClass{
			public String jobs = "";
			public System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
		}
	}
}
