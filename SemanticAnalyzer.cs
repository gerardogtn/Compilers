/*
  Buttercup compiler - Semantic analyzer.
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
using System.Collections.Generic;

namespace int64 {

    class FirstPassVisitor : INodeVisitor {

        public ISet<String> GlobalVariablesNamespace {
            get;
            private set;
        }

        public ISet<FunctionDefinition> FunctionNamespace {
            get;
            private set;
        }

        public FirstPassVisitor(ISet<String> GlobalVariablesNamespace, ISet<FunctionDefinition> FunctionNamespace) {
            this.GlobalVariablesNamespace = GlobalVariablesNamespace;
            this.FunctionNamespace = FunctionNamespace;
        }

        void VisitChildren(Node node) {
            foreach (var n in node) {
                Visit((dynamic) n);
            }
        }

        public void Visit(Program node) {
            VisitChildren(node);
        }


        public IList<Identifier> Visit(IdList node) {
            return null;
        }

        public void Visit(ParamList node) {

        }

        public void Visit(FunDef node) {

        }

        public void Visit(VarDefList node) {
            IList<Identifier> vars = Visit((dynamic) node[0]);
        }

        public void Visit(StmtList node) {

        }

        public void Visit(StmtIf node) {

        }

        public void Visit(ElseIfList node) {

        }

        public void Visit(ElseIf node) {

        }

        public void Visit(Else node) {

        }

        public void Visit(StmtSwitch node) {

        }

        public void Visit(CaseList node) {

        }

        public void Visit(Case node) {

        }

        public void Visit(LitList node) {

        }

        public void Visit(Default node) {

        }

        public void Visit(StmtWhile node) {

        }

        public void Visit(StmtDoWhile node) {

        }

        public void Visit(StmtFor node) {

        }

        public void Visit(StmtBreak node) {

        }

        public void Visit(StmtContinue node) {

        }

        public void Visit(StmtReturn node) {

        }

        public void Visit(StmtEmpty node) {

        }

        public void Visit(TernaryOperator node) {

        }

        public void Visit(LogicalOr node) {

        }

        public void Visit(LogicalAnd node) {

        }

        public void Visit(Equal node) {

        }

        public void Visit(NotEqual node) {

        }

        public void Visit(GreaterThan node) {

        }

        public void Visit(GreaterEqualThan node) {

        }

        public void Visit(LessThan node) {

        }

        public void Visit(LessEqualThan node) {

        }

        public void Visit(BitwiseOr node) {

        }

        public void Visit(BitwiseXor node) {

        }

        public void Visit(BitwiseAnd node) {

        }

        public void Visit(BitwiseShiftLeft node) {

        }

        public void Visit(BitwiseShiftRight node) {

        }

        public void Visit(BitwiseUnsignedShiftRight node) {

        }

        public void Visit(Plus node) {

        }

        public void Visit(Minus node) {

        }

        public void Visit(Times node) {

        }

        public void Visit(Division node) {

        }

        public void Visit(Remainder node) {

        }

        public void Visit(Power node) {

        }

        public void Visit(BitwiseNot node) {

        }

        public void Visit(LogicalNot node) {

        }

        public void Visit(FunCall node) {

        }

        public IList<Node> Visit(ArrayList node) {
            return null;
        }

        public void Visit(True node) {

        }

        public void Visit(False node) {

        }


        public void Visit(Identifier node) {

        }

        public void Visit(IntLiteral node) {

        }

        public void Visit(CharLiteral node) {

        }

        public void Visit(StringLiteral node) {

        }

        public void Visit(Assignment node) {

        }
    }

    class SecondPassVisitor : INodeVisitor {
        public void Visit(Program node) {

        }

        public IList<Identifier> Visit(IdList node) {
            return null;
        }

        public void Visit(ParamList node) {

        }

        public void Visit(FunDef node) {

        }

        public void Visit(VarDefList node) {

        }

        public void Visit(StmtList node) {

        }

        public void Visit(StmtIf node) {

        }

        public void Visit(ElseIfList node) {

        }

        public void Visit(ElseIf node) {

        }

        public void Visit(Else node) {

        }

        public void Visit(StmtSwitch node) {

        }

        public void Visit(CaseList node) {

        }

        public void Visit(Case node) {

        }

        public void Visit(LitList node) {

        }

        public void Visit(Default node) {

        }

        public void Visit(StmtWhile node) {

        }

        public void Visit(StmtDoWhile node) {

        }

        public void Visit(StmtFor node) {

        }

        public void Visit(StmtBreak node) {

        }

        public void Visit(StmtContinue node) {

        }

        public void Visit(StmtReturn node) {

        }

        public void Visit(StmtEmpty node) {

        }

        public void Visit(TernaryOperator node) {

        }

        public void Visit(LogicalOr node) {

        }

        public void Visit(LogicalAnd node) {

        }

        public void Visit(Equal node) {

        }

        public void Visit(NotEqual node) {

        }

        public void Visit(GreaterThan node) {

        }

        public void Visit(GreaterEqualThan node) {

        }

        public void Visit(LessThan node) {

        }

        public void Visit(LessEqualThan node) {

        }

        public void Visit(BitwiseOr node) {

        }

        public void Visit(BitwiseXor node) {

        }

        public void Visit(BitwiseAnd node) {

        }

        public void Visit(BitwiseShiftLeft node) {

        }

        public void Visit(BitwiseShiftRight node) {

        }

        public void Visit(BitwiseUnsignedShiftRight node) {

        }

        public void Visit(Plus node) {

        }

        public void Visit(Minus node) {

        }

        public void Visit(Times node) {

        }

        public void Visit(Division node) {

        }

        public void Visit(Remainder node) {

        }

        public void Visit(Power node) {

        }

        public void Visit(BitwiseNot node) {

        }

        public void Visit(LogicalNot node) {

        }

        public void Visit(FunCall node) {

        }

        public IList<Node> Visit(ArrayList node) {
            return null;
        }

        public void Visit(True node) {

        }

        public void Visit(False node) {

        }


        public void Visit(Identifier node) {

        }

        public void Visit(IntLiteral node) {

        }

        public void Visit(CharLiteral node) {

        }

        public void Visit(StringLiteral node) {

        }

        public void Visit(Assignment node) {

        }
    }

    class FunctionDefinition {
        public String Name {
            get;
            set;
        }

        public int Arity {
            get;
            set;
        }

        public ISet<String> Parameters {
            get;
            set;
        }

        public FunctionDefinition(String Name, int Arity, ISet<String> Parameters) {
            this.Name = Name;
            this.Arity = Arity;
            this.Parameters = Parameters;
        }

        public override bool Equals(Object obj) {
            if (obj == null || GetType() != obj.GetType()) return false;
            FunctionDefinition functionDefinition = obj as FunctionDefinition;
            return Name.Equals(functionDefinition.Name);
        }

        public override int GetHashCode() {
            return Name.GetHashCode();
        }
    }

    class SemanticAnalyzer {

        //-----------------------------------------------------------
        public ISet<String> GlobalVariablesNamespace {
            get;
            private set;
        }

        public ISet<FunctionDefinition> FunctionNamespace {
            get;
            private set;
        }

        //-----------------------------------------------------------
        public SemanticAnalyzer() {
            GlobalVariablesNamespace = new HashSet<String>();
            FunctionNamespace = new HashSet<FunctionDefinition>();
        }

        public void Run(Program node) {
            FirstPassVisitor firstPassVisitor = new FirstPassVisitor(GlobalVariablesNamespace, FunctionNamespace);
            firstPassVisitor.Visit(node);

            CheckMain();

            SecondPassVisitor secondPassVisitor = new SecondPassVisitor();
            secondPassVisitor.Visit(node);
        }

        private void CheckMain() {
            if (!FunctionNamespace.Contains(new FunctionDefinition("main", 0, null))) {
                 throw new SemanticError("No main function defined.");
            }
        }

    }
}
