using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolaScript.Object;

namespace PolaScript
{
	namespace Parsing
	{
		namespace Nodes
		{
			/// <summary>
			/// ノードを表します。
			/// </summary>
			public abstract class INode
			{
				//string name {get; set;}
				/// <summary>
				/// ノードの子要素です。
				/// </summary>
				public List<INode> childs { get; set; }

				public void AddChild(INode pchild)
				{
					//if (pnode.name == "=" || pnode.name == "+=" || pnode.name == "-=" || pnode.name == "*=" || pnode.name == "/=" || pnode.name == "&=" || pnode.name == "|=" || pnode.name == "^=")
					//	pnode.childs.Insert(0, pchild);
					//else
					this.childs.Add(pchild);
				}


				public void AddTwoChildren(INode pchild1, INode pchild2)
				{
					AddChild(pchild1);
					AddChild(pchild2);
				}
			}


			public interface IStatementNode
			{
				
			}

			public class MethodCallNode : INode
			{
				public string name { get; set; }

				public MethodCallNode(string name)
				{
					this.name = name;
					this.childs = new List<INode>();
				}

			}

			public class DeclerationNode : IStatementNode
			{
				Type type { get; set; }
				string varname { get; set; }
				
			}
			


			/// <summary>
			/// 式を表すノードです。
			/// </summary>
			public class ExpressionNode : INode
			{
				public string name { get; set; }
				//public string value;

				public ExpressionNode(string opname)
				{
					childs = new List<INode>();
					name = opname;
				}

				

			}


			/// <summary>
			/// PolaScript オブジェクトを表すノードです。
			/// </summary>
			public class PolaObjectNode : INode
			{
				//public string name { get; set; }
				public PolaObject constant { get; set; }
				public PolaObjectNode(PolaObject pobj)
				{
					this.childs = new List<INode>();
					this.constant = pobj;
				}

				public PolaObjectNode(Types t, object val)
					: this(new PolaObject(t, val)) { }

			}
		}
	}
}