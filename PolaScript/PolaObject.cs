using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolaScript
{
	namespace Object
	{
		/// <summary>
		/// PolaScript のオブジェクトを表します。
		/// </summary>
		public class PolaObject
		{
			/// <summary>
			/// オブジェクトの型を取得します。
			/// </summary>
			public Types type { get; private set; }

			/// <summary>
			/// オブジェクトの値を取得します。
			/// </summary>
			public object value { get; private set; }

			/// <summary>
			/// 型と値を指定して、 PolaObject クラスの新しいインスタンスを初期化します。
			/// </summary>
			/// <param name="t">型。</param>
			/// <param name="val">値。</param>
			public PolaObject(Types t, object val)
			{
				type = t;
				SetValue(t, val);
			}

			/// <summary>
			/// このインスタンスに、新しい値を設定します。
			/// </summary>
			/// <param name="t">型。</param>
			/// <param name="val">値。</param>
			/// <returns>このインスタンスの参照。</returns>
			/// <exception cref="System.ArgumentException">元のオブジェクトの型と、新しいオブジェクトの型が一致しないと発生。</exception>
			public PolaObject SetValue(Types t, object val)
			{
				
				if (type != t)
					throw new ArgumentException(string.Format("{0} 型と {1} 型で一致しません。", type, t));
				if (val == null)
				{
					value = val;
					return this;
				}

				switch (t)
				{
					case Types.Number:
						if (!(val is double || val is int))
							throw new ArgumentException(string.Format("{0} 型と .NET の {1} 型で一致しません。", t, val.GetType().Name));
						break;
					case Types.Boolean:
						if (!(val is bool))
							throw new ArgumentException(string.Format("{0} 型と .NET の {1} 型で一致しません。", t, val.GetType().Name));
						break;
					case Types.String:
						if (!(val is string))
							throw new ArgumentException(string.Format("{0} 型と .NET の {1} 型で一致しません。", t, val.GetType().Name));
						break;
				}
				value = val;
				return this;
				
			}


		}

		

		/// <summary>
		/// PolaObject の型を指定します。
		/// </summary>
		public enum Types
		{
			/// <summary>
			/// 未定義型 (PolaScript から定義できません)。
			/// </summary>
			Null,
			/// <summary>
			/// 数値。
			/// </summary>
			Number,
			/// <summary>
			/// 文字列。
			/// </summary>
			String,
			/// <summary>
			/// 真偽値。
			/// </summary>
			Boolean,
			/// <summary>
			/// 変数への参照 (PolaScript から定義できません)。
			/// </summary>
			Variable,
			/// <summary>
			/// 型 (PolaScript から定義できません)。
			/// </summary>
			Type
		}
	}
}