/*
  Buttercup compiler - Common Intermediate Language (CIL) code generator.
  Copyright (C) 2013 Ariel Ortiz, ITESM CEM

  This program is free software: you can redistribute it and/or modify
  it under the terms of the GNU General Public License as published by
  the Free Software Foundation, either version 3 of the License, or
  (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU General Public License for more details.

  You should have received a copy of the GNU General Public License
  along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Text;
using System.Collections.Generic;

namespace int64 {

    class CILGenerator {

        ISet<String> GlobalVariables;
        Dictionary<String, FunctionDefinition> Functions;

        public CILGenerator(ISet<String> GlobalVariables, Dictionary<String, FunctionDefinition> Functions) {
            this.GlobalVariables = GlobalVariables;
            this.Functions = Functions;
        }

        //-----------------------------------------------------------

        int labelCounter = 0;

        string GenerateLabel() {
            return String.Format("${0:000000}", labelCounter++);
        }


        //----------------------- NODES ----------------------------//

        public string Visit(Program node) {
            return @"// Code generated by the int64 compiler.
.assembly 'int64' {}
.assembly extern 'int64lib' {}
.class public 'int64Program' extends ['mscorlib']'System'.'Object' {

   //------------------ GLOBAL VARIABLES ------------------//
"  + Visit((dynamic) node[0]) + @"
   //--------------------- FUNCTIONS ---------------------//
"  + Visit((dynamic) node[1])
      + "\t\tret\n\t}\n}\n";
        }

        //-----------------------------------------------------------
        public string Visit(ParamList node) {return "";}

        //-----------------------------------------------------------
        public string Visit(FunDef node) {return "";}

        //-----------------------------------------------------------
        public string Visit(VarDefList node) {
           var sb = new StringBuilder();
           foreach (var id in node) {
               sb.Append(String.Format(
                    "\t\t.field private static int64 {0}\n",
                    Visit((dynamic) id))
               );
           }
           return sb.ToString();
          return "Returning";
        }

        //-----------------------------------------------------------
        public string Visit(StmtList node) {return "";}

        //-----------------------------------------------------------
        public string Visit(StmtIf node) {return "";}

        //-----------------------------------------------------------
        public string Visit(ElseIfList node) {return "";}

        //-----------------------------------------------------------
        public string Visit(ElseIf node) {return "";}

        //-----------------------------------------------------------
        public string Visit(Else node) {return "";}

        //-----------------------------------------------------------
        public string Visit(StmtSwitch node) {return "";}

        //-----------------------------------------------------------
        public string Visit(CaseList node) {return "";}

        //-----------------------------------------------------------
        public string Visit(Case node) {return "";}

        //-----------------------------------------------------------
        public string Visit(LitList node) {return "";}

        //-----------------------------------------------------------
        public string Visit(Default node) {return "";}

        //-----------------------------------------------------------
        public string Visit(StmtWhile node) {return "";}

        //-----------------------------------------------------------
        public string Visit(StmtDoWhile node) {return "";}

        //-----------------------------------------------------------
        public string Visit(StmtFor node) {return "";}

        //-----------------------------------------------------------
        public string Visit(StmtBreak node) {return "";}

        //-----------------------------------------------------------
        public string Visit(StmtContinue node) {return "";}

        //-----------------------------------------------------------
        public string Visit(StmtReturn node) {return "";}

        //-----------------------------------------------------------
        public string Visit(StmtEmpty node) {return "";}

        //-----------------------------------------------------------
        public string Visit(TernaryOperator node) {return "";}

        //-----------------------------------------------------------
        public string Visit(LogicalOr node) {return "";}

        //-----------------------------------------------------------
        public string Visit(LogicalAnd node) {return "";}

        //-----------------------------------------------------------
        public string Visit(Equal node) {return "";}

        //-----------------------------------------------------------
        public string Visit(NotEqual node) {return "";}

        //-----------------------------------------------------------
        public string Visit(GreaterThan node) {return "";}

        //-----------------------------------------------------------
        public string Visit(GreaterEqualThan node) {return "";}

        //-----------------------------------------------------------
        public string Visit(LessThan node) {return "";}

        //-----------------------------------------------------------
        public string Visit(LessEqualThan node) {return "";}

        //-----------------------------------------------------------
        public string Visit(BitwiseOr node) {return "";}

        //-----------------------------------------------------------
        public string Visit(BitwiseXor node) {return "";}

        //-----------------------------------------------------------
        public string Visit(BitwiseAnd node) {return "";}

        //-----------------------------------------------------------
        public string Visit(BitwiseShiftLeft node) {return "";}

        //-----------------------------------------------------------
        public string Visit(BitwiseShiftRight node) {return "";}

        //-----------------------------------------------------------
        public string Visit(BitwiseUnsignedShiftRight node) {return "";}

        //-----------------------------------------------------------
        public string Visit(Plus node) {return "";}

        //-----------------------------------------------------------
        public string Visit(Minus node) {return "";}

        //-----------------------------------------------------------
        public string Visit(Times node) {return "";}

        //-----------------------------------------------------------
        public string Visit(Division node) {return "";}

        //-----------------------------------------------------------
        public string Visit(Remainder node) {return "";}

        //-----------------------------------------------------------
        public string Visit(Power node) {return "";}

        //-----------------------------------------------------------
        public string Visit(BitwiseNot node) {return "";}

        //-----------------------------------------------------------
        public string Visit(LogicalNot node) {return "";}

        //-----------------------------------------------------------
        public string Visit(FunCall node) {return "";}

        //-----------------------------------------------------------
        public string Visit(ArrayList node) {return "";}

        //-----------------------------------------------------------
        public string Visit(True node) {return "";}

        //-----------------------------------------------------------
        public string Visit(False node) {return "";}

        //-----------------------------------------------------------
        public string Visit(Identifier node) {
           return node.AnchorToken.Lexeme;
        }

        //-----------------------------------------------------------
        public string Visit(IntLiteral node) {return "";}

        //-----------------------------------------------------------
        public string Visit(CharLiteral node) {return "";}

        //-----------------------------------------------------------
        public string Visit(StringLiteral node) {return "";}

        //-----------------------------------------------------------
        public string Visit(Assignment node) {return "";}










        //
        // public string Visit(DeclarationList node) {
        //     // The code for the local variable declarations is
        //     // generated directly from the symbol table, not from
        //     // the AST nodes.
        //     var sb = new StringBuilder();
        //     foreach (var entry in table) {
        //         sb.Append(String.Format(
        //                       "\t\t.locals init ({0} '{1}')\n",
        //                       CILTypes[entry.Value],
        //                       entry.Key)
        //                   );
        //     }
        //     return sb.ToString();
        // }
        //
        // //-----------------------------------------------------------
        // public string Visit(Declaration node) {
        //     // This method is never called.
        //     return null;
        // }
        //
        // //-----------------------------------------------------------
        // public string Visit(StatementList node) {
        //     return VisitChildren(node);
        // }
        //
        // //-----------------------------------------------------------
        // public string Visit(Assignment node) {
        //     return Visit((dynamic) node[0])
        //         + "\t\tstloc '"
        //         + node.AnchorToken.Lexeme
        //         + "'\n";
        // }
        //
        // //-----------------------------------------------------------
        // public string Visit(Print node) {
        //     return Visit((dynamic) node[0])
        //         + "\t\tcall void class ['bcuplib']'Buttercup'."
        //         + "'Utils'::'Print'("
        //         + CILTypes[node.ExpressionType]
        //         + ")\n";
        // }
        //
        // //-----------------------------------------------------------
        // public string Visit(If node) {
        //
        //     var label = GenerateLabel();
        //
        //     return String.Format(
        //         "{1}\t\tbrfalse '{0}'\n{2}\t'{0}':\n",
        //         label,
        //         Visit((dynamic) node[0]),
        //         Visit((dynamic) node[1])
        //     );
        // }
        //
        // //-----------------------------------------------------------
        // public string Visit(Identifier node) {
        //     return "\t\tldloc '"
        //         + node.AnchorToken.Lexeme
        //         + "'\n";
        // }
        //
        // //-----------------------------------------------------------
        // public string Visit(IntLiteral node) {
        //
        //     var intValue = Convert.ToInt32(node.AnchorToken.Lexeme);
        //
        //     if (intValue <= 8) {
        //         return "\t\tldc.i4."
        //             + intValue
        //             + "\n";
        //
        //     } else if (intValue <= 127) {
        //         return "\t\tldc.i4.s "
        //             + intValue
        //             + "\n";
        //
        //     } else {
        //         return "\t\tldc.i4 "
        //             + intValue
        //             + "\n";
        //     }
        // }
        //
        // //-----------------------------------------------------------
        // public string Visit(True node) {
        //     return "\t\tldc.i4.1\n";
        // }
        //
        // //-----------------------------------------------------------
        // public string Visit(False node) {
        //     return "\t\tldc.i4.0\n";
        // }
        //
        // //-----------------------------------------------------------
        // public string Visit(Neg node) {
        //     return "\t\tldc.i4.0\n"
        //         + Visit((dynamic) node[0])
        //         + "\t\tsub.ovf\n";
        // }
        //
        // //-----------------------------------------------------------
        // public string Visit(And node) {
        //     return VisitBinaryOperator("and", node);
        // }
        //
        // //-----------------------------------------------------------
        // public string Visit(Less node) {
        //     return VisitBinaryOperator("clt", node);
        // }
        //
        // //-----------------------------------------------------------
        // public string Visit(Plus node) {
        //     return VisitBinaryOperator("add.ovf", node);
        // }
        //
        // //-----------------------------------------------------------
        // public string Visit(Mul node) {
        //     return VisitBinaryOperator("mul.ovf", node);
        // }
        //
        // //-----------------------------------------------------------
        // string VisitChildren(Node node) {
        //     var sb = new StringBuilder();
        //     foreach (var n in node) {
        //         sb.Append(Visit((dynamic) n));
        //     }
        //     return sb.ToString();
        // }
        //
        // //-----------------------------------------------------------
        // string VisitBinaryOperator(string op, Node node) {
        //     return Visit((dynamic) node[0])
        //         + Visit((dynamic) node[1])
        //         + "\t\t"
        //         + op
        //         + "\n";
        // }
    }
}
