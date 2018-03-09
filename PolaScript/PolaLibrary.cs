using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolaScript.Object;

namespace PolaScript
{
	namespace Runtime
	{
		/// <summary>
		/// PolaScript から利用できる .NET メソッドを定義します。
		/// </summary>
		public static class PolaLibrary
		{
			//文字列出力
			[PolaMethod(Types.Null)]
			public static void print(string msg)
			{
				Console.Write(msg);
			}

			//文字列出力と改行
			[PolaMethod(Types.Null)]
			public static void println(string msg)
			{
				Console.WriteLine(msg);
			}

			//文字列出力と改行
			[PolaMethod(Types.Null)]
			public static void numprintln(double msg)
			{
				Console.WriteLine(msg);
			}
			//文字列入力
			[PolaMethod(Types.String)]
			public static string scanln(string prompt)
			{
				Console.WriteLine(prompt);
				return Console.ReadLine();
			}




		}
		
		/// <summary>
		/// PolaScript から利用できるメソッドであることを表します。
		/// </summary>
		[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
		sealed class PolaMethodAttribute : Attribute
		{
			readonly Types resultTypes;

			/// <summary>
			/// 返り値の型を指定して、 PolaMethodAttribute クラスの新しいインスタンスを初期化します。
			/// </summary>
			/// <param name="result">返り値の型。返り値がない場合、 PolaScript.Object.Types.Null を指定します。</param>
			public PolaMethodAttribute(Types result)
			{
				resultTypes = result;
			}

			/// <summary>
			/// 返り値の型を取得します。
			/// </summary>
			public Types ResultTypes
			{
				get { return resultTypes; }
			}
		}


	}
}
