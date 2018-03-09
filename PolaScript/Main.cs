using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolaScript.Parsing;
using PolaScript.Runtime;
using PolaScript.Object;


namespace PolaClient
{
	class MainProgram
	{

		static void Main(string[] args)
		{
			Parser parser;
			Interpreter interpreter;
			Dictionary<string, PolaObject> varl = null;
			while (true)
			{
				string dat = Console.ReadLine();


				//try
				//{
					parser = new Parser(dat);
					parser.Parse();
					
					interpreter = new Interpreter(parser);
					if (varl != null)
						interpreter.VarList = varl;
					
					interpreter.Run();
					varl = interpreter.VarList;
				//}
				//catch (Exception ex)
				/*{
					System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
					Console.Error.WriteLine("[ERROR!!!: {0}]{1}\r\nStackTrace: \r\n", ex.GetType().Name, ex.Message);
					foreach (var frame in trace.GetFrames())
					{
						Console.Error.WriteLine("{0}, {1} : {2}", frame.GetFileLineNumber(), frame.GetFileColumnNumber(), frame.GetMethod());
					}
				}*/
			}
		}


	}
}
