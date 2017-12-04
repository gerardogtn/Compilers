using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Int64 {
	class CilGenerator : INodeVisitor {

		private StringBuilder Builder;
		private int IndentLevel;
		private static int SPACES_PER_INDENT = 2;
		private String FileName;
		private String CurrentFunction;
		private int labelCounter = 0;

		private Stack<String> ExitLabels;
		private Stack<String> ContinueLabels;
		private Stack<String> BreakLabels;

		private Dictionary<String, FunctionDefinition> ApiFunctions = new Dictionary<String, FunctionDefinition>() {
                {"printi", new FunctionDefinition("printi",  1, new HashSet<String>() { "i" } , new HashSet<String>())},
                {"printc", new FunctionDefinition("printc",  1, new HashSet<String>() { "c" } , new HashSet<String>())},
                {"prints", new FunctionDefinition("prints",  1, new HashSet<String>() { "s" } , new HashSet<String>())},
                {"println", new FunctionDefinition("println", 0, new HashSet<String>(), new HashSet<String>())},
                {"readi", new FunctionDefinition("readi",   0, new HashSet<String>(), new HashSet<String>())},
                {"reads", new FunctionDefinition("reads",   0, new HashSet<String>(), new HashSet<String>())},
                {"new", new FunctionDefinition("new",     1, new HashSet<String>() { "n" } , new HashSet<String>())},
                {"size", new FunctionDefinition("size",    1, new HashSet<String>() { "h" } , new HashSet<String>())},
                {"add", new FunctionDefinition("add",     2, new HashSet<String>() { "h", "x" } , new HashSet<String>())},
                {"get", new FunctionDefinition("get",     2, new HashSet<String>() { "h", "i" } , new HashSet<String>())},
                {"set", new FunctionDefinition("set",     3, new HashSet<String>() { "h", "i", "x" }, new HashSet<String>())}
            };

        public ISet<String> GlobalVariablesNamespace {
            get;
            private set;
        }

        public Dictionary<String, FunctionDefinition> FunctionNamespace {
            get;
            private set;
        }

        private string GenerateLabel() {
            return String.Format("${0:000000}", labelCounter++);
        }

		public CilGenerator(ISet<String> GlobalVariablesNamespace,
				Dictionary<String, FunctionDefinition> FunctionNamespace) {
			this.Builder = new StringBuilder();
			this.FileName = "Test";
			this.GlobalVariablesNamespace = GlobalVariablesNamespace;
			this.FunctionNamespace = FunctionNamespace;
			this.ExitLabels = new Stack<String>();
			this.ContinueLabels = new Stack<String>();
			this.BreakLabels = new Stack<String>();
		}

		public String Run(Program node) {
			Visit(node);
			return Builder.ToString();
		}

		private void WriteLine(String s) {
			Indent();
			this.Builder.Append(s);
			this.Builder.Append("\n");
		}

		private void WriteLine(params String[] strings) {
			Indent();
			foreach (String s in strings) {
				this.Builder.Append(s);
			}
			this.Builder.Append("\n");
		}

		private void Indent() {
			this.Builder.Append(new String(' ', IndentLevel * SPACES_PER_INDENT));
		}

		private void Write(params String[] strings ) {
			foreach (String s in strings) {
				this.Builder.Append(s);
			}
		}

		public void Visit(Program node) {
			WriteLine(".assembly 'output' { }\n");
			WriteLine(".assembly extern 'int64lib' { }\n");
			WriteLine(".class public '", FileName, "' extends ['mscorlib']'System'.'Object' {\n");
			IndentLevel++;
			foreach (var n in node) {
				if (n.GetType() == typeof(VarDefList)) {
					VisitGlobalVariables((VarDefList) node[0]);
					this.Builder.AppendLine();
				} else {
					Visit((dynamic) n);
				}
			}
			IndentLevel--;
			WriteLine("}");
		}

		private void VisitGlobalVariables(VarDefList node) {
			foreach (Node c in node) {
				Identifier Identifier = (Identifier) c;
				WriteLine(".field public static int64 '", c.AnchorToken.Lexeme, "'");
			}
		}


		public void Visit(FunDef node) {
			var PrevFunction = CurrentFunction;
			CurrentFunction = node.AnchorToken.Lexeme;
			Indent();
			Write(".method public static default int64 '", CurrentFunction, "'");
			Visit((ParamList) node[0]);
			Write(" {\n");
			IndentLevel++;
			if (node.AnchorToken.Lexeme.Equals("main")) {
				WriteLine(".entrypoint");
			}
			Visit((dynamic) node[1]);
			Visit((dynamic) node[2]);
			WriteLine("ldc.i8 0");
			WriteLine("ret");

			IndentLevel--;
			CurrentFunction = PrevFunction;
			WriteLine("}\n");
		}


		public void Visit(ParamList node) {
			Write("(");
			if (node.Any()) {
				Node last = node.Last();
				foreach (var n in node) {
					Write("int64 '", n.AnchorToken.Lexeme, "'");
					if (!n.Equals(last)) {
						Write(", ");
					}
				}
			}

			Write(")");
		}

		// Only called inside a fundef.
		public void Visit(VarDefList node) {
			if (node.Any()) {
				WriteLine(".locals init (");
				IndentLevel++;
				Node last = node.Last();
				foreach (var n in node) {
					Indent();
					Write("int64 '", n.AnchorToken.Lexeme, "'");
					if (!n.Equals(last)) {
						Write(",\n");
					}
				}
				IndentLevel--;
				Write(")\n");
			}
		}

		public void Visit(StmtList node) {
			VisitChildren(node);
		}

		public void Visit(StmtIf node) {
			var consequentLabel = GenerateLabel();
			var exitLabel = GenerateLabel();

			ExitLabels.Push(exitLabel);

			// If positive, go to the consequent.
			Visit((dynamic) node[0]);
			WriteLine("brtrue ", consequentLabel, "\n");

			// Go to elseif
			var ElseIfList = (ElseIfList) node[2];
			Visit(ElseIfList);

			// Assert consequent and exit.
			WriteLabel(consequentLabel);
			Visit((dynamic) node[1]);
			WriteLabel(exitLabel);

			ExitLabels.Pop();
		}

		private void WriteLabel(String label) {
			WriteLine(label, ": ");
		}

		public void Visit(ElseIfList node) {
			// Else if labels.
			var labels = new List<String>();
			for (int i = 0; i < node.Count(); i++) {
				var child = node[i];
				if (child is ElseIf) {
					var label = GenerateLabel();
					labels.Add(label);
					Visit((dynamic) child[0]);
					WriteLine("brtrue ", label, "\n");
				}
			}

			// Else statement.
			if (node.Any() && node.Last() is Else) {
				Visit((dynamic) node.Last()[0]);
			}
			WriteLine("br ", ExitLabels.Peek(), "\n");

			// Else if bodies.
			for (int i = 0; i < labels.Count; i++) {
				WriteLabel(labels[i]);
				var child = node[i];
				if (child is ElseIf) {
					Visit((dynamic) child[1]);
					WriteLine("br ", ExitLabels.Peek(), "\n");
				}
			}
		}

		public void Visit(ElseIf node) {
			throw new InvalidOperationException("ElseIf should already be handled");
		}

		public void Visit(Else node) {
			throw new InvalidOperationException("Else should already be handled");
		}

		public void Visit(StmtSwitch node) {
			CaseList CaseList = (CaseList) node[1];
			Default Default = (Default) node[2];

			var labels = new List<String>();

			foreach (var _case in CaseList) {
				var label = GenerateLabel();
				labels.Add(label);

				foreach (var lit in _case[0]) {
					Visit((dynamic) node[0]);
					Visit((dynamic) lit);
					WriteLine("brtrue ", label);
				}
			}

			var exitLabel = GenerateLabel();

			Visit((StmtList) Default[0]);
			WriteLine("br ", exitLabel);

			for (int i = 0; i < labels.Count; i++) {
				WriteLabel(labels[i]);
				Visit((StmtList) CaseList[i][1]);
				WriteLine("br ", exitLabel);
			}
			WriteLabel(exitLabel);

		}

		public void Visit(CaseList node) {
			throw new InvalidOperationException("CaseList Should be handled in StmtSwitch");
		}

		public void Visit(Case node) {
			throw new InvalidOperationException("Case Should be handled in StmtSwitch");
		}

		public void Visit(LitList node) {
			throw new InvalidOperationException("LitList Should be handled in StmtSwitch");
		}

		public void Visit(Default node) {
			throw new InvalidOperationException("Default Should be handled in StmtSwitch");
		}

		public void Visit(StmtWhile node) {
			var bodyLabel = GenerateLabel();
			var conditionLabel = GenerateLabel();
			var exitLabel = GenerateLabel();

			ContinueLabels.Push(conditionLabel);
			BreakLabels.Push(exitLabel);

			// Visit body.
			WriteLine("br ", conditionLabel);
			WriteLabel(bodyLabel);
			Visit((dynamic) node[1]);

			// Visit condition.
			WriteLabel(conditionLabel);
			Visit((dynamic) node[0]);

			// Jump back
			WriteLine("brtrue ", bodyLabel);

			WriteLabel(exitLabel);
			ContinueLabels.Pop();
			BreakLabels.Pop();
		}

		public void Visit(StmtDoWhile node) {
			var bodyLabel = GenerateLabel();
			var conditionLabel = GenerateLabel();
			var exitLabel = GenerateLabel();

			ContinueLabels.Push(conditionLabel);
			BreakLabels.Push(exitLabel);

			// Visit body.
			WriteLabel(bodyLabel);
			Visit((dynamic) node[1]);

			// Visit condition.
			WriteLabel(conditionLabel);
			Visit((dynamic) node[0]);

			// Jump back
			WriteLine("brtrue ", bodyLabel);

			WriteLabel(exitLabel);
			ContinueLabels.Pop();
			BreakLabels.Pop();
		}

		public void Visit(StmtFor node) {

		}

		public void Visit(StmtBreak node) {
			WriteLine("br ", BreakLabels.Peek());
		}

		public void Visit(StmtContinue node) {
			WriteLine("br ", ContinueLabels.Peek());
		}

		public void Visit(StmtReturn node) {
			VisitChildren(node);
			WriteLine("ret");
		}

		public void Visit(StmtEmpty node) {
			// Do nothing.
		}

		public void Visit(TernaryOperator node) {
			var consequentLabel = GenerateLabel();
			var exitLabel = GenerateLabel();

			Visit((dynamic) node[0]);
			WriteLine("brtrue ", consequentLabel);

			Visit((dynamic) node[2]);
			WriteLine("br ", exitLabel);

			WriteLabel(consequentLabel);
			Visit((dynamic) node[1]);
			WriteLabel(exitLabel);
		}

		public void Visit(LogicalOr node) {
			var label = GenerateLabel();
			var exitLabel = GenerateLabel();
			Visit((dynamic) node[0]);
			WriteLine("brtrue ", label);
			Visit((dynamic) node[1]);
			WriteLine("br ", exitLabel);
			WriteLabel(label);
            WriteLine("ldc.i4.1");
            WriteLine("conv.i8");
            WriteLabel(exitLabel);
		}

		public void Visit(LogicalAnd node) {
			var label = GenerateLabel();
			var exitLabel = GenerateLabel();
			Visit((dynamic) node[0]);
			WriteLine("brfalse ", label);
			Visit((dynamic) node[1]);
			WriteLine("br ", exitLabel);
			WriteLabel(label);
            WriteLine("ldc.i4.0");
            WriteLine("conv.i8");
            WriteLabel(exitLabel);
		}

		public void Visit(Equal node) {
			VisitChildren(node);
			WriteLine("ceq");
		}

		public void Visit(NotEqual node) {
			VisitChildren(node);
			WriteLine("ceq");
			WriteLine("ldc.i4 0");
			WriteLine("ceq");
		}

		public void Visit(GreaterThan node) {
			VisitChildren(node);
			WriteLine("cgt");
		}

		public void Visit(GreaterEqualThan node) {
			VisitChildren(node);
			WriteLine("clt");
			WriteLine("ldc.i4 0");
			WriteLine("ceq");
		}

		public void Visit(LessThan node) {
			VisitChildren(node);
			WriteLine("clt");
		}

		public void Visit(LessEqualThan node) {
			VisitChildren(node);
			WriteLine("cgt");
			WriteLine("ldc.i4 0");
			WriteLine("ceq");
		}

		public void Visit(BitwiseOr node) {
			VisitChildren(node);
			WriteLine("not");
		}

		public void Visit(BitwiseXor node) {
			VisitChildren(node);
			WriteLine("xor");
		}

		public void Visit(BitwiseAnd node) {
			VisitChildren(node);
			WriteLine("and");
		}

		public void Visit(BitwiseShiftLeft node) {
			VisitChildren(node);
			WriteLine("conv.i4");
			WriteLine("shl");
		}

		public void Visit(BitwiseShiftRight node) {
			VisitChildren(node);
			WriteLine("conv.i4");
			WriteLine("shr");
		}

		public void Visit(BitwiseUnsignedShiftRight node) {
			VisitChildren(node);
			WriteLine("conv.i4");
			WriteLine("shr.un");
		}

		public void Visit(Plus node) {
			VisitChildren(node);
			if (node.Count() > 1) {
				WriteLine("add.ovf");
			}
		}

		public void Visit(Minus node) {
			if (node.Count() == 1) {
				WriteLine("ldc.i4 0");
				WriteLine("conv.u8");
			}
			VisitChildren(node);
			WriteLine("sub.ovf");

		}

		public void Visit(Times node) {
			VisitChildren(node);
			WriteLine("mul.ovf");
		}

		public void Visit(Division node) {
			VisitChildren(node);
			WriteLine("div");

		}

		public void Visit(Remainder node) {
			VisitChildren(node);
			WriteLine("rem");
		}


		public void Visit(Power node) {
			VisitChildren(node);
			WriteLine("call int64 class ['int64lib']'Int64'.'Utils'::'Pow'(int64, int64)");
		}


		public void Visit(BitwiseNot node) {
			VisitChildren(node);
			WriteLine("neg");
		}

		public void Visit(LogicalNot node) {
			VisitChildren(node);
			WriteLine("ldc.i4.0");
			WriteLine("ceq");
			WriteLine("conv.i8");
		}

		public void Visit(StmtFunCall node) {
			VisitFunCall(node);
			WriteLine("pop");
		}

		public void Visit(FunCall node) {
			VisitFunCall(node);
		}

		private void VisitFunCall(Node node) {
			var functionName = node.AnchorToken.Lexeme;

			VisitChildren(node);

			Indent();

			if (!ApiFunctions.ContainsKey(functionName)) {
				Write("call int64 class '", FileName, "'::'", functionName, "'(");
			} else {
				Write("call int64 class ['int64lib']'Int64'.'Utils'::'");
				Write(functionName.First().ToString().ToUpper());
				Write(functionName.Substring(1));
				Write("'(");
			}

			for (int i = 0; i < FunctionNamespace[functionName].Arity; i++) {
				Write("int64");
				if (i != FunctionNamespace[functionName].Arity - 1) {
					Write(", ");
				}
			}
			Write(")\n");
		}

		public IList<Node> Visit(ArrayList node) {
			return null;
		}


		public void Visit(True node) {
            WriteLine("ldc.i4.1");
            WriteLine("conv.i8");
		}

		public void Visit(False node) {
            WriteLine("ldc.i4.0");
            WriteLine("conv.i8");
		}

		public void Visit(Identifier node) {
			var lexeme = node.AnchorToken.Lexeme;
			if (FunctionNamespace[CurrentFunction].LocalVars.Contains(lexeme)) {
				WriteLine("ldloc '", lexeme, "'");
			} else if (FunctionNamespace[CurrentFunction].Parameters.Contains(lexeme)) {
				WriteLine("ldarg '", lexeme, "'");
			} else if (GlobalVariablesNamespace.Contains(lexeme)) {
				WriteLine("ldsfld int64 '", FileName, "'::'", lexeme, "'");
			} else {
				throw new InvalidOperationException("Found a variable that is not in parameters, or locals, or globals");
			}
		}

		public void Visit(IntLiteral node) {
			String number = node.AnchorToken.Lexeme;
            int radix = 10;
            if (number.StartsWith("0b") || number.StartsWith("0B")) {
                number = number.Replace("0b", "").Replace("0B", "");
                radix = 2;
            }
            if (number.StartsWith("0o") || number.StartsWith("0O")) {
                radix = 8;
            }
            if (number.StartsWith("0x") || number.StartsWith("0X")) {
                radix = 16;
            }
            long value = checked (Convert.ToInt64(number, radix));
			WriteLine("ldc.i8 ", value.ToString());
		}

		public void Visit(CharLiteral node) {
			string char_literal = node.AnchorToken.Lexeme.Replace("'","").Replace("\\","");

            Write("\t" + toCodePoints(char_literal));
		}

		public string toCodePoints(string char_literal) {
			if (char_literal == "n") {
                return "ldc.i8 10\n";
            }
            else if (char_literal == "r") {
                return "ldc.i8 13\n";
            }
            else if (char_literal == "t") {
                return "ldc.i8 9\n";
            }
            else if (char_literal == "\\") {
                return "ldc.i8 92\n";
            }
            else if (char_literal == "'") {
                return "ldc.i8 39\n";
            }
            else if (char_literal == "\"") {
                return "ldc.i8 34\n";
            }
            else if (char_literal.StartsWith("u")) {
                return "ldc.i8 0x" + char_literal.Replace("u", "") + "\n";
            }
            else {
				Console.WriteLine("What!!");
                return "ldc.i8 " + char.ConvertToUtf32(char_literal, 0) + "\n";
            }
		}

		public void Visit(StringLiteral node) {
			String s = node.AnchorToken.Lexeme.Substring(1, node.AnchorToken.Lexeme.Length - 2);
			var size = s.Length;

			var count = Utils.AsCodePoints(s).Count();
			var sb = new StringBuilder();

			int i = 0;
			bool flag = false;
			bool flagx = false;
			short j = 0;
			foreach (var l in Utils.AsCodePoints(s)) {

				if (flagx) {
					sb.Append(l.ToString());
					j++;
					count--;
					if (j >= 6) {
						sb.Append("\n");
						flagx = false;
						i++;
					}
					continue;
				}

				if (l == '\\') {
					flag = true;
					count--;
					continue;
				}

				sb.Append("\tdup\n");
				sb.Append("\tldc.i8 " + i.ToString() + "\n");
				if (flag) {
					if (l == 110) {
		                sb.Append("\tldc.i8 10\n");
		            }
		            else if (l == 114) {
		                sb.Append("\tldc.i8 13\n");
		            }
		            else if (l == 116) {
		                sb.Append("\tldc.i8 9\n");
		            }
		            else if (l == 92) {
		                sb.Append("\tldc.i8 92\n");
		            }
		            else if (l == 39) {
		                sb.Append("\tldc.i8 39\n");
		            }
		            else if (l == 34) {
		                sb.Append("\tldc.i8 34\n");
		            }
		            else if (l == 117) {
						sb.Append("\tldc.i8 0x");
		                flagx = true;
						flag = false;
						continue;
		            }
					flag = false;
				}
				else {
					sb.Append("\tldc.i8 " + l.ToString() + "\n");
				}
				sb.Append("\tcall int64 class ['int64lib']'Int64'.'Utils'::'Set'(int64, int64, int64)\n");
				sb.Append("\tpop\n");
				i += 1;
			}

			WriteLine("ldc.i8 ", count.ToString());
			WriteLine("call int64 class ['int64lib']'Int64'.'Utils'::'New'(int64)");
			WriteLine(sb.ToString());
		}

		public void Visit(Assignment node) {
			Visit((dynamic) node[1]);

			String lexeme = node[0].AnchorToken.Lexeme;
			if (FunctionNamespace[CurrentFunction].LocalVars.Contains(lexeme)) {
				WriteLine("stloc '", lexeme, "'");
			} else if (FunctionNamespace[CurrentFunction].Parameters.Contains(lexeme)) {
				WriteLine("starg '", lexeme, "'");
			} else if (GlobalVariablesNamespace.Contains(lexeme)) {
				WriteLine("stsfld int64 '", FileName, "'::'", lexeme, "'");
			} else {
				throw new InvalidOperationException("Found a variable that is not in parameters, or locals, or globals");
			}
			WriteLine();
		}

		private void VisitChildren(Node node) {
            foreach (var n in node) {
                Visit((dynamic) n);
            }
        }

	}
}
