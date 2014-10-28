using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolaScript.Parsing.Nodes;
using PolaScript.Object;

namespace PolaScript
{
	namespace Parsing
	{
		public class Parser
		{

			string[] token;

			int ix = 0;

			public INode RootNode = null;

			public Parser(string code)
			{
				token = GetTokenByExpression(code);
			}
			
			public string GetAST(INode node)
			{
				StringBuilder lastdata = new StringBuilder();
				if (node is ExpressionNode)
					lastdata.AppendFormat(" ({0}", ((ExpressionNode)node).name);
				else if (node is PolaObjectNode)
				{
					lastdata.Append(" (");
					PolaObjectNode cnode = (PolaObjectNode)node;
					lastdata.Append(cnode.constant.type);
					lastdata.Append(" ");
					switch (cnode.constant.type)
					{
						case Types.Number:
							lastdata.Append((double)cnode.constant.value);
							break;
						case Types.Boolean:
							lastdata.Append((bool)cnode.constant.value);
							break;
						case Types.String:
							lastdata.Append((string)cnode.constant.value);
							break;
						case Types.Variable:
							lastdata.Append((string)cnode.constant.value);
							break;
						case Types.Type:
							lastdata.Append((Types)cnode.constant.value);
							break;
						case Types.Null:
							lastdata.Append("null");
							break;
					}
				}
				else if (node is MethodCallNode)
				{
					lastdata.AppendFormat(" (method name=\"{0}\" ", ((MethodCallNode)node).name);					
				}
				for (int n = 0; n < node.childs.Count; n++)
				{
					INode pchild = node.childs[n];

					lastdata.Append(GetAST(pchild));
				}
				lastdata.Append(")");

				return lastdata.ToString();
			}

			public void Parse()
			{
				foreach (string data in token)
					Console.Write(data + " ");
				Console.Write(" => ");

				ix = 0;
				INode expr = SetStatement();
				Console.Write(GetAST(expr));
				Console.WriteLine();
				this.RootNode = expr;
			}

			INode SetStatement()
			{
				ExpressionNode mainnode = new ExpressionNode(";");

				while(ix < token.Length && token[ix] != ";")
				{
					INode child = SetAssignment();
					
					mainnode.AddChild(child);
					ix++;
				}
				Console.WriteLine("ix: " + ix);
				if (ix > token.Length && token[token.Length - 1] != ";")
					throw new ParseException("; が必要です。");
				
				return mainnode;
			}

			INode SetAssignment()
			{
				INode pleft = SetLogic();

				while (ix < token.Length && (token[ix] == "=" || token[ix] == "+=" || token[ix] == "-=" || token[ix] == "*=" || token[ix] == "/=" || token[ix] == "&=" || token[ix] == "|=" || token[ix] == "^="))
				{
					ExpressionNode pnode = new ExpressionNode(token[ix++]);
					
					INode pright = SetAssignment();
					Console.WriteLine(pnode.name);
					pnode.AddTwoChildren(pleft, pright);
					pleft = pnode;
				}
				return pleft;
			}

			INode SetLogic()
			{
				INode pleft = SetBit();
				while (ix < token.Length && (token[ix] == "<" || token[ix] == ">" || token[ix] == "==" || token[ix] == "!=" || token[ix] == "<=" || token[ix] == ">="))
				{
					ExpressionNode pnode = new ExpressionNode(token[ix++]);
					Console.WriteLine(pnode.name);
					INode pright = SetShift();
					pnode.AddTwoChildren(pleft, pright);
					pleft = pnode;
				}
				
				return pleft;
			}

			INode SetBit()
			{
				INode pleft = SetShift();
				while (ix < token.Length && (token[ix] == "&" || token[ix] == "|" || token[ix] == "^"))
				{
					ExpressionNode pnode = new ExpressionNode(token[ix++]);
					Console.WriteLine(pnode.name);
					INode pright = SetLogic();
					pnode.AddTwoChildren(pleft, pright);
					pleft = pnode;
				}
				return pleft;
			}

			INode SetShift()
			{
				INode pleft = SetAddSub();
				while (ix < token.Length && (token[ix] == "<<" || token[ix] == ">>"))
				{
					ExpressionNode pnode = new ExpressionNode(token[ix++]);
					Console.WriteLine(pnode.name);
					INode pright = SetAddSub();
					pnode.AddTwoChildren(pleft, pright);
					pleft = pnode;
				}
				return pleft;
			}

			INode SetAddSub()
			{
				INode pleft = SetMulDiv();
				while (ix < token.Length && (token[ix] == "+" || token[ix] == "-"))
				{
					ExpressionNode pnode = new ExpressionNode(token[ix++]);
					Console.WriteLine(pnode.name);
					INode pright = SetMulDiv();
					pnode.AddTwoChildren(pleft, pright);
					pleft = pnode;
				}
				return pleft;
			}

			INode SetMulDiv()
			{
				INode pleft = SetFactor();
				while (ix < token.Length && (token[ix] == "*" || token[ix] == "/" || token[ix] == "%"))
				{
					ExpressionNode pnode = new ExpressionNode(token[ix++]);
					Console.WriteLine(pnode.name);
					INode pright = SetFactor();
					pnode.AddTwoChildren(pleft, pright);
					pleft = pnode;
				}
				return pleft;
			}



			INode SetFactor()
			{
				INode pnode;
				if (token[ix] == "(")
				{
					ix++;
					pnode = SetAssignment();
					if (token[ix++] != ")")
						throw new ParseException("\")\"がありません。");
				}
				else if (token[ix] == "-" || token[ix] == "~" || token[ix] == "!")
				{
					pnode = new ExpressionNode(token[ix++]);
					INode pright = SetAssignment();
					Console.WriteLine(((ExpressionNode)pnode).name);
					((ExpressionNode)pnode).AddChild(pright);
				}
				else if (token[ix] == "num" || token[ix] == "string" || token[ix] == "bool" || token[ix] == "Number" || token[ix] == "String" || token[ix] == "Boolean")
				{
					INode pleft = null;
					switch (token[ix])
					{
						case "num":
							pleft = new PolaObjectNode(Types.Type, Types.Number);
							break;
						case "Number":
							pleft = new PolaObjectNode(Types.Type, Types.Number);
							break;
						case "string":
							pleft = new PolaObjectNode(Types.Type, Types.String);
							break;
						case "String":
							pleft = new PolaObjectNode(Types.Type, Types.String);
							break;
						case "bool":
							pleft = new PolaObjectNode(Types.Type, Types.Boolean);
							break;
						case "Boolean":
							pleft = new PolaObjectNode(Types.Type, Types.Boolean);
							break;
						default:
							throw new ParseException(string.Format("{0} という型指定子はありません。"));

					}
					pnode = new ExpressionNode("let");
					ix++;
					INode pright = SetFactor();
					Console.WriteLine(((PolaObjectNode)pleft).constant.value);
					((ExpressionNode)pnode).AddTwoChildren(pleft, pright);
					pright = pnode;
				}
				else
				{
					string tmp = token[ix++];


					if (isNumber(tmp))	//数値
					{
						pnode = new PolaObjectNode(Types.Number, double.Parse(tmp));
					}
					
					else if (tmp[0] == '"' && tmp[tmp.Length - 1] == '"' && tmp.Length > 1)	//文字列
					{
						pnode = new PolaObjectNode(Types.String, tmp.Substring(1, tmp.Length - 2).Replace("\\\"", "\"").Replace("\\\\","\\"));
					}
					else if (tmp == "true" || tmp == "false")	//ブール
					{
						pnode = new PolaObjectNode(Types.Boolean, bool.Parse(tmp));
					}
					else if (isVarName(tmp))	//変数
					{

						if (ix < token.Length && token[ix] == "(")
						{
							ix++;
							//INode pleft = new PolaObjectNode(Types.Variable, tmp);

							if (ix < token.Length && token[ix] != ")")
							{
								INode pright = SetAssignment();
								pnode = new MethodCallNode(tmp);
								tmplist = new List<INode>();
								GetArguments(pright.childs);
								pnode.childs = tmplist;
							}
							else
							{
								
								pnode = new MethodCallNode(tmp);
							}
							//((ExpressionNode)pnode).AddTwoChildren(pleft, pright);

						}
						else
							pnode = new PolaObjectNode(Types.Variable, tmp);

					}
					else
					{
						throw new ParseException(tmp + " を理解できません。");
					}

					if (ix < token.Length && token[ix] == ",")
					{
						PolaObjectNode pn = new PolaObjectNode(((PolaObjectNode)pnode).constant.type, ((PolaObjectNode)pnode).constant.value);
						pnode = new ExpressionNode(",");
						ix++;
						INode pright = SetAssignment();
						((ExpressionNode)pnode).AddTwoChildren(pn, pright);

						
					}

				}
				return pnode;
			}

			List<INode> tmplist = null;

			void GetArguments(List<INode> list)
			{
				foreach (INode node in list)
				{
					if (node is ExpressionNode && ((ExpressionNode)node).name == ",")
					{
						tmplist.Add(node.childs[0]);
						if (node.childs[1] is ExpressionNode && ((ExpressionNode)node.childs[1]).name == ",")
						{
							GetArguments(node.childs[1].childs);
						}
						else
						{
							tmplist.Add(node.childs[1]);
						}
					}
					else
					{
						tmplist.Add(node);
					}
				}
			}

			static bool isNumber(string val)
			{
				double d;
				return double.TryParse(val, out d);
			}

			static bool isVarName(string val)
			{
				bool lastdata = true;
				int cnt = 0;
				foreach (char c in val)
				{
					if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || (cnt > 0 && c >= '0' && c <= '9') || (c == '_')))
					{
						lastdata = false;
					}
					cnt++;
				}
				return lastdata;
			}

			static string[] GetTokenByExpression(string source)
			{
				TokenMode tm = TokenMode.Null;
				string tmp = "";
				List<string> lastdata = new List<string>();
				bool isString = false;
				int c = 0;
				foreach (char chr in source)
				{
					if (!isString)
					{
						if (char.IsSeparator(chr))
						{
							if (tmp != "")
							{
								lastdata.Add(tmp);
								tmp = "";
								c = 0;
							}
							continue;
						}

						TokenMode nowtm = TokenMode.Null;
						if (char.IsNumber(chr) || chr == '.' || chr == '\\')
							nowtm = TokenMode.Value;
						else if (
								 chr == '+' || chr == '-'
								 || chr == '*' || chr == '/' || chr == '%'
								 || chr == '(' || chr == ')'
								 || chr == '<' || chr == '>' || chr == '='
								 || chr == '&' || chr == '|' || chr == '^'
								 || chr == '!' || chr == '~' || chr == ';'
								 || chr == ','
								)
						{
							nowtm = TokenMode.Operation;
						}
						if ((nowtm != tm || (nowtm == TokenMode.Operation && tm == TokenMode.Operation)))
						{
							tm = nowtm;
							if (tmp != "" && tmp + chr != "<=" && tmp + chr != ">=" && tmp + chr != "==" && tmp + chr != "!=" && tmp + chr != "<<" && tmp + chr != ">>"
								&& tmp + chr != "+=" && tmp + chr != "-=" && tmp + chr != "*=" && tmp + chr != "/=" && tmp + chr != "&=" && tmp + chr != "|=" && tmp + chr != "^=")
							{
								lastdata.Add(tmp);
								tmp = "";
								c = 0;
							}
						}
					}

					if (c > 0)
					{
						if (chr == '"' && (tmp + chr).Substring(tmp.Length - 1) != "\\\"")
						{
							isString = !isString;
						}
						if (chr == '"' && c > 1 && (tmp + chr).Substring(tmp.Length - 2) == "\\\\\"")
							isString = !isString;
					
					}
					else if (chr == '"')
							isString = !isString;
					tmp += chr;
					c++;
				}
				if (tmp != "")
				{
					lastdata.Add(tmp);
				}
				return lastdata.ToArray();
			}
		}

		public enum TokenMode
		{
			Null, Value, Operation
		}

		public class ParseException : Exception
		{

			public int ProblemLine { get; private set; }
			public int ProblemColumn { get; private set; }

			public ParseException()
				: base("構文解析に失敗しました。") { }

			public ParseException(int line, int column)
				: this()
			{
				ProblemLine = line;
				ProblemColumn = column;
			}

			public ParseException(string message)
				: base(message) { }

			public ParseException(string message, int line, int column)
				: base(message)
			{
				ProblemLine = line;
				ProblemColumn = column;
			}

		}

	}

}