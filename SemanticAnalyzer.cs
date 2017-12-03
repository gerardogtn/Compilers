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

/******************* FIRST PASS VISITOR *******************/

    class FirstPassVisitor : INodeVisitor {

        public ISet<String> GlobalVariablesNamespace {
            get;
            private set;
        }

        public Dictionary<String, FunctionDefinition> FunctionNamespace {
            get;
            private set;
        }

        public FirstPassVisitor(ISet<String> GlobalVariablesNamespace, Dictionary<String, FunctionDefinition> FunctionNamespace) {
            this.GlobalVariablesNamespace = GlobalVariablesNamespace;
            this.FunctionNamespace = FunctionNamespace;
        }

        void VisitChildren(Node node) {
            foreach (var n in node) {
                Visit((dynamic) n);
            }
        }

        /////////////////////////////////////////////////

        public void Visit(Program node) {
            VisitChildren(node);
        }

        public void Visit(ParamList node) {

        }

        public void Visit(FunDef node) {
            var functionName = node.AnchorToken.Lexeme;

            // Add parameters
            var parameters = new HashSet<String>();
            foreach(var n in node[0]){
                var variableName = n.AnchorToken.Lexeme;
                if(parameters.Contains(variableName)){
                    throw new SemanticError("Duplicated variable: " + variableName, n.AnchorToken);
                } else {
                    parameters.Add(variableName);
                }
            }

            // Add local variables
            var localVariables = new HashSet<String>();
            foreach(var n in node[1]){
                var variableName = n.AnchorToken.Lexeme;
                if(parameters.Contains(variableName)){
                   Console.WriteLine("WARNING:");
                   Console.WriteLine("\tLocal variable \"" + variableName +
                        "\" will shadow the paremeter with the same name.");
                }
                localVariables.Add(variableName);
            }

            var functionDefinition = new FunctionDefinition(functionName, parameters.Count, parameters, localVariables);
            if(FunctionNamespace.ContainsKey(functionName)){
                throw new SemanticError("Duplicated function: " + functionName, node.AnchorToken);
            } else{
                if(functionName == "main" && parameters.Count > 0){
                    throw new SemanticError("Main function has more than 0 parameters ");
                } else{
                    FunctionNamespace.Add(functionName,functionDefinition);
                }
            }
        }

        public void Visit(VarDefList node) {
            foreach (var n in node) {
                var variableName = n.AnchorToken.Lexeme;
                if (GlobalVariablesNamespace.Contains(variableName)) {
                    throw new SemanticError("Duplicated variable: " + variableName, n.AnchorToken);
                } else {
                    GlobalVariablesNamespace.Add(variableName);
                }
            }
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


/******************* SECOND PASS VISITOR *******************/

    class SecondPassVisitor : INodeVisitor {

        private int NestedLoopCount;

        public ISet<String> localParameters {
            get;
            private set;
        }

        public ISet<String> localVars {
            get;
            private set;
        }

        public ISet<String> GlobalVariablesNamespace {
            get;
            private set;
        }

        public Dictionary<String, FunctionDefinition> FunctionNamespace {
            get;
            private set;
        }



        public SecondPassVisitor(ISet<String> GlobalVariablesNamespace, Dictionary<String, FunctionDefinition> FunctionNamespace) {
            this.NestedLoopCount = 0;
            this.GlobalVariablesNamespace = GlobalVariablesNamespace;
            this.FunctionNamespace = FunctionNamespace;
        }

        void VisitChildren(Node node) {
            foreach (var n in node) {
                Visit((dynamic) n);
            }
        }

        //////////////////////////////////////////////////

        public void Visit(Program node) {
            foreach(var n in node) {
                if (!(n is VarDefList)) {
                    Visit((dynamic) n);
                }
            }
        }

        // public ISet<String> getChildren(ParamList node){
        //     // Already validate duplicates on first pass
        //     var children = new HashSet<String>();
        //     foreach (var n in node) {
        //         children.Add(n.AnchorToken.Lexeme);
        //     }
        //     return children;
        // }

        public ISet<String> getChildren(VarDefList node){
            var children = new HashSet<String>();
            foreach (var n in node) {
                var variableName = n.AnchorToken.Lexeme;
                if(children.Contains(variableName)){
                    throw new SemanticError("Duplicated variable: " + variableName, n.AnchorToken);
                } else{
                    children.Add(variableName);
                }

            }
            return children;
        }

        public void Visit(FunDef node) {
            localParameters = FunctionNamespace[node.AnchorToken.Lexeme].Parameters;
            localVars = getChildren((dynamic) node[1]);
            VisitChildren(node);

        }

        public void Visit(ParamList node) {


        }

        public void Visit(VarDefList node) {

        }

        public void Visit(StmtList node) {
            VisitChildren(node);
        }

        public void Visit(StmtIf node) {
            VisitChildren(node);
        }

        public void Visit(ElseIfList node) {
            VisitChildren(node);
        }

        public void Visit(ElseIf node) {
            VisitChildren(node);
        }

        public void Visit(Else node) {
            VisitChildren(node);
        }

        public void Visit(StmtSwitch node) {
            VisitChildren(node);
        }

        public void Visit(CaseList node) {
            VisitChildren(node);
        }

        public void Visit(Case node) {
            VisitChildren(node);
        }

        public void Visit(LitList node) {
            VisitChildren(node);
        }

        public void Visit(Default node) {
            VisitChildren(node);
        }

        public void Visit(StmtWhile node) {
            NestedLoopCount++;
            VisitChildren(node);
            NestedLoopCount--;
        }

        public void Visit(StmtDoWhile node) {
            NestedLoopCount++;
            VisitChildren(node);
            NestedLoopCount--;
        }

        public void Visit(StmtFor node) {
            NestedLoopCount++;
            VisitChildren(node);
            NestedLoopCount--;
        }

        public void Visit(StmtBreak node) {
            if (NestedLoopCount <= 0) {
                throw new SemanticError("Break outside of For, While or DoWhile ", node.AnchorToken);
            }
        }

        public void Visit(StmtContinue node) {
            if (NestedLoopCount <= 0) {
                throw new SemanticError("Continue outside of For, While or DoWhile ", node.AnchorToken);
            }
        }

        public void Visit(StmtReturn node) {

        }

        public void Visit(StmtEmpty node) {

        }

        public void Visit(TernaryOperator node) {
            VisitChildren(node);
        }

        public void Visit(LogicalOr node) {
            VisitChildren(node);
        }

        public void Visit(LogicalAnd node) {
            VisitChildren(node);
        }

        public void Visit(Equal node) {
            VisitChildren(node);
        }

        public void Visit(NotEqual node) {
            VisitChildren(node);
        }

        public void Visit(GreaterThan node) {
            VisitChildren(node);
        }

        public void Visit(GreaterEqualThan node) {
            VisitChildren(node);
        }

        public void Visit(LessThan node) {
            VisitChildren(node);
        }

        public void Visit(LessEqualThan node) {
            VisitChildren(node);
        }

        public void Visit(BitwiseOr node) {
            VisitChildren(node);
        }

        public void Visit(BitwiseXor node) {
            VisitChildren(node);
        }

        public void Visit(BitwiseAnd node) {
            VisitChildren(node);
        }

        public void Visit(BitwiseShiftLeft node) {
            VisitChildren(node);
        }

        public void Visit(BitwiseShiftRight node) {
            VisitChildren(node);
        }

        public void Visit(BitwiseUnsignedShiftRight node) {
            VisitChildren(node);
        }

        public void Visit(Plus node) {
            VisitChildren(node);
        }

        public void Visit(Minus node) {
            VisitChildren(node);
        }

        public void Visit(Times node) {
            VisitChildren(node);
        }

        public void Visit(Division node) {
            VisitChildren(node);
        }

        public void Visit(Remainder node) {
            VisitChildren(node);
        }

        public void Visit(Power node) {
            VisitChildren(node);
        }

        public void Visit(BitwiseNot node) {
            VisitChildren(node);
        }

        public void Visit(LogicalNot node) {
            VisitChildren(node);
        }

        public void Visit(FunCall node) {
            var functionName = node.AnchorToken.Lexeme;
            var size = 0;
            foreach (var n in node) {
                size++;
            }
            if(FunctionNamespace.ContainsKey(functionName) && FunctionNamespace[functionName].Arity == size){

            }
            else{
                throw new SemanticError("Invalid function call! " + functionName, node.AnchorToken);
            }
            VisitChildren(node);
        }

        public IList<Node> Visit(ArrayList node) {
            return null;
        }

        public void Visit(True node) {

        }

        public void Visit(False node) {

        }

        public void Visit(Identifier node) {
            var variableName = node.AnchorToken.Lexeme;
            //Console.Write(variableName);
            if(!localVars.Contains(variableName) && !localParameters.Contains(variableName) && !GlobalVariablesNamespace.Contains(variableName)){
                throw new SemanticError("Variable not defined -> " + variableName, node.AnchorToken);
            }
        }

        public void Visit(IntLiteral node) {
            try {
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
                Convert.ToInt64(number, radix);
            } catch (OverflowException) {
                throw new SemanticError("Cannot convert literal to int64. ", node.AnchorToken);
            }
        }

        public void Visit(CharLiteral node) {

        }

        public void Visit(StringLiteral node) {

        }

        public void Visit(Assignment node) {
            VisitChildren(node);
        }
    }


/******************* FUNCTION DEFINITION *******************/

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

        public ISet<String> LocalVars {
            get;
            set;
        }

        public FunctionDefinition(String Name, int Arity, ISet<String> Parameters, ISet<String> LocalVars) {
            this.Name = Name;
            this.Arity = Arity;
            this.Parameters = Parameters;
            this.LocalVars = LocalVars;
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


/******************* SEMANTIC ANALYZER *******************/

    class SemanticAnalyzer {

        //-----------------------------------------------------------
        public ISet<String> GlobalVariablesNamespace {
            get;
            private set;
        }

        public Dictionary<String, FunctionDefinition> FunctionNamespace {
            get;
            private set;
        }

        //-----------------------------------------------------------
        public SemanticAnalyzer() {
            GlobalVariablesNamespace = new HashSet<String>();
            FunctionNamespace = new Dictionary<String, FunctionDefinition>() {
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
        }

        public void Run(Program node) {
            FirstPassVisitor firstPassVisitor = new FirstPassVisitor(GlobalVariablesNamespace, FunctionNamespace);
            firstPassVisitor.Visit(node);

            CheckMain();

            SecondPassVisitor secondPassVisitor = new SecondPassVisitor(GlobalVariablesNamespace, FunctionNamespace);
            secondPassVisitor.Visit(node);
        }

        private void CheckMain() {
            if(!FunctionNamespace.ContainsKey("main")){
                throw new SemanticError("No main function defined. ");
            }
        }

    }
}
