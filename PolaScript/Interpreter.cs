using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolaScript.Parsing.Nodes;
using System.Reflection;
using PolaScript.Parsing;
using PolaScript.Object;

namespace PolaScript
{
	namespace Runtime
	{
		public class Interpreter
		{
			Dictionary<string, PolaObject> varlist = new Dictionary<string, PolaObject>();

			List<INode> statements;

			public Dictionary<string, PolaObject> VarList
			{
				get => varlist;
				set => varlist = value;
			}
			

			public Interpreter(Parser parser)
			{
				statements = parser.Statements;
				
			}

			public Interpreter(List<INode> node)
			{
				statements = node;
			}

			public void Run()
			{
				foreach (INode node in statements)
				{
					Calc(node);
				}
			}

			PolaObject Calc(INode arg)
			{


				if (arg is PolaObjectNode)
					return ((PolaObjectNode)arg).constant;
				else if (arg is MethodCallNode)
				{
					MethodCallNode mcn = (MethodCallNode)arg;
					object[] objects = new object[mcn.childs.Count];
					Type[] types = new Type[mcn.childs.Count];
					for (int i = 0; i < mcn.childs.Count; i++)
					{
						PolaObject pobj = Calc(mcn.childs[i]);
						switch (pobj.type)
						{
							case Types.Number:
								types[i] = typeof(double);
								break;
							case Types.String:
								types[i] = typeof(string);
								break;
							case Types.Boolean:
								types[i] = typeof(bool);
								break;
							case Types.Variable:
								Console.WriteLine(pobj.value);
								pobj = varlist[(string)pobj.value];
								switch (pobj.type)
								{
									case Types.Number:
										types[i] = typeof(double);
										break;
									case Types.String:
										types[i] = typeof(string);
										break;
									case Types.Boolean:
										types[i] = typeof(bool);
										break;
								}
								break;
							default:
								throw new Exception("メソッドのパラメーターに " + pobj.type + " を指定することはできません。");
						}
						objects[i] = pobj.value;
					}

					if (mcn.childs.Count == 0)
						objects = null;
					MethodInfo mi = typeof(PolaLibrary).GetMethod(mcn.name, types);
					if (mi == null)
						throw new Exception(mcn.name + "() メソッドはありません。");
					PolaMethodAttribute pma = mi.GetCustomAttribute<PolaMethodAttribute>();
					if (mi != null && pma != null)
					{
						object hoge = null;
						try
						{
							hoge = mi.Invoke(null, objects);
						}
						catch (TargetInvocationException ex)
						{
							throw ex.InnerException;
						}
						switch (pma.ResultTypes)
						{
							case Types.Null:	//値を返さないメソッド
								return new PolaObject(Types.Null, null);
							default:
								return new PolaObject(pma.ResultTypes, hoge);
						}
					}
					else
					{
						throw new Exception(mcn.name + "() メソッドはありません。");
					}
				}

				ExpressionNode pnode = (ExpressionNode)arg;

				PolaObject p1, p2;
				PolaObject _p1, _p2;
				switch (pnode.childs.Count)
				{
					case 2:
						p1 = Calc(pnode.childs[0]);
						p2 = Calc(pnode.childs[1]);



						if (p1.type == Types.Variable)
							_p1 = varlist[(string)p1.value];
						else
							_p1 = p1;
						if (p2.type == Types.Variable && p1.type != Types.Type)
							_p2 = varlist[(string)p2.value];
						else
							_p2 = p2;


						if (pnode.name == "+")
						{

							if (_p1.type == Types.String && _p2.type == Types.Number)
								return new PolaObject(Types.String, (string)_p1.value + ((double)_p2.value).ToString());
							else if (_p1.type == Types.String && _p2.type == Types.String)
								return new PolaObject(Types.String, (string)_p1.value + (string)_p2.value);
							else if (_p1.type == Types.Number && _p2.type == Types.Number)
								return new PolaObject(Types.Number, (double)_p1.value + (double)_p2.value);
							else
								throw new Exception(string.Format("{0} 演算子を、 {1} 型と {2} 型の演算に使用できません。", pnode.name, _p1.type, _p2.type));
						}
						else if (pnode.name == "-")
							return new PolaObject(Types.Number, (double)_p1.value - (double)_p2.value);
						else if (pnode.name == "*")
						{
							if (_p1.type == Types.String && _p2.type == Types.Number)
								return new PolaObject(Types.String, (RepeatString((string)_p1.value, (int)(double)_p2.value)));
							return new PolaObject(Types.Number, (double)_p1.value * (double)_p2.value);
						}
						else if (pnode.name == "/")
							return new PolaObject(Types.Number, (double)_p1.value / (double)_p2.value);
						else if (pnode.name == "%")
							return new PolaObject(Types.Number, (double)_p1.value % (double)_p2.value);
						else if (pnode.name == "<")
							return new PolaObject(Types.Boolean, ((double)_p1.value < (double)_p2.value) ? true : false);
						else if (pnode.name == ">")
							return new PolaObject(Types.Boolean, ((double)_p1.value > (double)_p2.value) ? true : false);
						else if (pnode.name == "<=")
							return new PolaObject(Types.Boolean, ((double)_p1.value <= (double)_p2.value) ? true : false);
						else if (pnode.name == ">=")
							return new PolaObject(Types.Boolean, ((double)_p1.value >= (double)_p2.value) ? true : false);
						else if (pnode.name == "==")
							return new PolaObject(Types.Boolean, ((double)_p1.value == (double)_p2.value) ? true : false);
						else if (pnode.name == "!=")
							return new PolaObject(Types.Boolean, ((double)_p1.value != (double)_p2.value) ? true : false);
						else if (pnode.name == "<<")
							try
							{
								if ((double)_p1.value - (int)_p1.value != 0 || (double)_p2.value - (int)_p2.value != 0)
									throw new Exception(" << 演算は、整数のみ行うことができます。");
								else
									return new PolaObject(Types.Number, (int)_p1.value << (int)_p2.value);
							}
							catch (InvalidCastException)
							{
								throw new Exception(string.Format("{0} 演算子を、 {1} 型と {2} 型の演算に使用できません。", pnode.name, _p1.type, _p2.type));
							}
						else if (pnode.name == ">>")
							if ((double)_p1.value - (int)_p1.value != 0 || (double)_p2.value - (int)_p2.value != 0)
								throw new Exception(" >> 演算は、整数のみ行うことができます。");
							else
								return new PolaObject(Types.Number, (int)_p1.value >> (int)_p2.value);
						else if (pnode.name == "&")
							if ((double)_p1.value - (int)_p1.value != 0 || (double)_p2.value - (int)_p2.value != 0)
								throw new Exception(" & 演算は、整数のみ行うことができます。");
							else
								return new PolaObject(Types.Number, (int)_p1.value & (int)_p2.value);
						else if (pnode.name == "|")
							if ((double)_p1.value - (int)_p1.value != 0 || (double)_p2.value - (int)_p2.value != 0)
								throw new Exception(" | 演算は、整数のみ行うことができます。");
							else
								return new PolaObject(Types.Number, (int)_p1.value | (int)_p2.value);
						else if (pnode.name == "^")
							if ((double)_p1.value - (int)_p1.value != 0 || (double)_p2.value - (int)_p2.value != 0)
								throw new Exception(" ^ 演算は、整数のみ行うことができます。");
							else
								return new PolaObject(Types.Number, (int)_p1.value ^ (int)_p2.value);
						else if (pnode.name == "=")
						{


							if (p1.type == Types.Variable)
							{
								CheckVarExists(p1);
								return varlist[(string)p1.value].SetValue(_p2.type, _p2.value);
							}
						}
						else if (pnode.name == "+=")
						{
							//複合代入演算子に、変数が参照できないときの処理を追加する(キーが存在しないとき)

							CheckVarExists(p1);
							if (varlist[(string)p1.value].type == Types.String && _p2.type == Types.String)
								return varlist[(string)p1.value].SetValue(_p2.type, (string)(varlist[(string)p1.value].value) + (string)_p2.value);
							else if (varlist[(string)p1.value].type == Types.String && _p2.type == Types.Number)
								return varlist[(string)p1.value].SetValue(varlist[(string)p1.value].type, (string)(varlist[(string)p1.value].value) + (double)_p2.value);
							else if (varlist[(string)p1.value].type == Types.Number && _p2.type == Types.Number)
								return varlist[(string)p1.value].SetValue(_p2.type, (double)(varlist[(string)p1.value].value) + (double)_p2.value);
							else
								throw new Exception(string.Format("{0} 演算子を、 {1} 型と {2} 型の演算に使用できません。", pnode.name, _p1.type, _p2.type));
						}
						else if (pnode.name == "-=")
						{
							CheckVarExists(p1);
							if (varlist[(string)p1.value].type == Types.Number && p2.type == Types.Number)
								return varlist[(string)p1.value].SetValue(p2.type, (double)(varlist[(string)p1.value].value) - (double)p2.value);
							else
								throw new Exception(string.Format("{0} 演算子を、 {1} 型と {2} 型の演算に使用できません。", pnode.name, p1.type, p2.type));
						}
						else if (pnode.name == "*=")
						{
							CheckVarExists(p1);
							if (varlist[(string)p1.value].type == Types.Number && p2.type == Types.Number)
								return varlist[(string)p1.value].SetValue(p2.type, (double)(varlist[(string)p1.value].value) + (double)p2.value);
							else if (varlist[(string)p1.value].type == Types.String && p2.type == Types.Number)
								return varlist[(string)p1.value].SetValue(varlist[(string)p1.value].type, RepeatString((string)(varlist[(string)p1.value].value), (int)p2.value));
							else
								throw new Exception(string.Format("{0} 演算子を、 {1} 型と {2} 型の演算に使用できません。", pnode.name, p1.type, p2.type));
						}
						else if (pnode.name == "/=")
						{
							CheckVarExists(p1);
							if (varlist[(string)p1.value].type == Types.Number && p2.type == Types.Number)
								return varlist[(string)p1.value].SetValue(p2.type, (double)(varlist[(string)p1.value].value) / (double)p2.value);
							else
								throw new Exception(string.Format("{0} 演算子を、 {1} 型と {2} 型の演算に使用できません。", pnode.name, p1.type, p2.type));
						}
						else if (pnode.name == "let")
						{
							if (p1.type != Types.Type)
								throw new Exception("型指定子がありません。");
							if (p2.type != Types.Variable)
								throw new Exception((string)p2.value + " は変数名として不適切です。");
							if (varlist.ContainsKey((string)p2.value))
								throw new Exception("変数 " + (string)p2.value + " はすでに存在します。");
							varlist[(string)p2.value] = new PolaObject(((Types)p1.value), null);
							return p2;
						}
						break;
					case 1:
						p1 = Calc(pnode.childs[0]);
						if (p1.type == Types.Variable)
							_p1 = varlist[(string)p1.value];
						else
							_p1 = p1;


						if (pnode.name == "-")
							return new PolaObject(Types.Number, -(double)p1.value);
						else if (pnode.name == "~")
							if ((double)p1.value - (int)p1.value != 0)
								throw new Exception("~ 演算は、整数のみ行うことができます。");
							else
								return new PolaObject(Types.Number, ~(int)p1.value);
						else if (pnode.name == "!")
						{
							if ((double)p1.value != 0)
								return new PolaObject(Types.Number, 1);
							else
								return new PolaObject(Types.Number, 0);
						}
						else if (pnode.name == ";")
							return p1;
						break;
					default:
						throw new Exception(string.Format("{0}つの項をとる演算子はありません。", pnode.childs.Count));
				}
				throw new Exception(string.Format("{0}つの項をとる、\"{1}\" 演算子はありません。", pnode.childs.Count, pnode.name));

			}

			public static string RepeatString(string s, int count)
			{
				System.Text.StringBuilder buf =
					new System.Text.StringBuilder(s.Length * count);
				for (int i = 0; i < count; i++)
				{
					buf.Append(s);
				}
				return buf.ToString();
			}

			/// <summary>
			/// 指定した Constant が変数を表すか、表すならその変数が存在するかどうか確認し、問題があれば例外を投げます。
			/// </summary>
			/// <param name="vardata">チェックする Constant 構造体。</param>
			void CheckVarExists(PolaObject vardata)
			{
				if (vardata.type != Types.Variable)
					throw new Exception("左辺は変数を表しません。");
				if (!varlist.ContainsKey((string)vardata.value))
					throw new Exception(string.Format("変数 {0} は宣言されていません。", (string)vardata.value));
			}

		}
	}
}
