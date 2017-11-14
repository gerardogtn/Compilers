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


    class SemanticAnalyzer {

        class FunctionTable{
            int arity;
            bool predefined;
            SymbolTable localTable;

            public FunctionTable(int arity,bool predefined,SymbolTable localTable){
                 this.arity = arity;
                 this.predefined = predefined;
                 this.localTable = localTable;
            }
        }

        class LocalTable{
            string kind;
            int position;

            public LocalTable(string kind, int position){
                this.kind = kind;
                this.position = position;
            }

        }


        //-----------------------------------------------------------
        public SymbolTable Table {
            get;
            private set;
        }

        //-----------------------------------------------------------
        public SemanticAnalyzer() {
            GlobalVars = new SymbolTable();
            Functions = new SymbolTable();
        }

        //-----------------------------------------------------------
        public Type Visit(Program node) {
            Visit((dynamic) node[0]);
            Visit((dynamic) node[1]);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(DefList node) {
            VisitChildren(node);
            return Type.VOID;
        }
        public Type Visit(Def node) {
            VisitChildren(node);
            return Type.VOID;
        }
        public Type Visit(VarDef node) {
            VisitChildren(node);
            return Type.VOID;
        }
        public Type Visit(VarList node) {
            VisitChildren(node);
            return Type.VOID;
        }
        public Type Visit(IdList node) {
            foreach (var n in node) {
                var variableName = n.AnchorToken.Lexeme;
                if (GlobalVars.Contains(variableName)) {
                    throw new SemanticError(
                        "Duplicated variable: " + variableName, n.AnchorToken);
                } else {
                    GlobalVars[variableName] = null;
                }
            }
            return Type.VOID;
        }

        public Type Visit(ParamList node) {
            VisitChildren(node);
            return Type.VOID;
        }
        public Type Visit(VarDefList node) {
            VisitChildren(node);
            return Type.VOID;
        }
        public Type Visit(StmtList node) {
            VisitChildren(node);
            return Type.VOID;
        }
        public Type Visit(CaseList node) {
            VisitChildren(node);
            return Type.VOID;
        }
        public Type Visit(LitList node) {
            VisitChildren(node);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(Declaration node) {

            var variableName = node[0].AnchorToken.Lexeme;

            if (Table.Contains(variableName)) {
                throw new SemanticError(
                    "Duplicated variable: " + variableName,
                    node[0].AnchorToken);

            } else {
                Table[variableName] =
                    typeMapper[node.AnchorToken.Category];
            }

            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(StatementList node) {
            VisitChildren(node);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(Assignment node) {

            var variableName = node.AnchorToken.Lexeme;

            if (Table.Contains(variableName)) {

                var expectedType = Table[variableName];

                if (expectedType != Visit((dynamic) node[0])) {
                    throw new SemanticError(
                        "Expecting type " + expectedType
                        + " in assignment statement",
                        node.AnchorToken);
                }

            } else {
                throw new SemanticError(
                    "Undeclared variable: " + variableName,
                    node.AnchorToken);
            }

            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(Print node) {
            node.ExpressionType = Visit((dynamic) node[0]);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(If node) {
            if (Visit((dynamic) node[0]) != Type.BOOL) {
                throw new SemanticError(
                    "Expecting type " + Type.BOOL
                    + " in conditional statement",
                    node.AnchorToken);
            }
            VisitChildren(node[1]);
            return Type.VOID;
        }

        //-----------------------------------------------------------
        public Type Visit(Identifier node) {

            var variableName = node.AnchorToken.Lexeme;

            if (Table.Contains(variableName)) {
                return Table[variableName];
            }

            throw new SemanticError(
                "Undeclared variable: " + variableName,
                node.AnchorToken);
        }

        //-----------------------------------------------------------
        public Type Visit(IntLiteral node) {

            var intStr = node.AnchorToken.Lexeme;

            try {
                Convert.ToInt32(intStr);

            } catch (OverflowException) {
                throw new SemanticError(
                    "Integer literal too large: " + intStr,
                    node.AnchorToken);
            }

            return Type.INT;
        }

        //-----------------------------------------------------------
        public Type Visit(True node) {
            return Type.BOOL;
        }

        //-----------------------------------------------------------
        public Type Visit(False node) {
            return Type.BOOL;
        }

        //-----------------------------------------------------------
        public Type Visit(Neg node) {
            if (Visit((dynamic) node[0]) != Type.INT) {
                throw new SemanticError(
                    "Operator - requires an operand of type " + Type.INT,
                    node.AnchorToken);
            }
            return Type.INT;
        }

        //-----------------------------------------------------------
        public Type Visit(And node) {
            VisitBinaryOperator('&', node, Type.BOOL);
            return Type.BOOL;
        }

        //-----------------------------------------------------------
        public Type Visit(Less node) {
            VisitBinaryOperator('<', node, Type.INT);
            return Type.BOOL;
        }

        //-----------------------------------------------------------
        public Type Visit(Plus node) {
            VisitBinaryOperator('+', node, Type.INT);
            return Type.INT;
        }

        //-----------------------------------------------------------
        public Type Visit(Mul node) {
            VisitBinaryOperator('*', node, Type.INT);
            return Type.INT;
        }

        //-----------------------------------------------------------
        void VisitChildren(Node node) {
            foreach (var n in node) {
                Visit((dynamic) n);
            }
        }

        //-----------------------------------------------------------
        void VisitBinaryOperator(char op, Node node, Type type) {
            if (Visit((dynamic) node[0]) != type ||
                Visit((dynamic) node[1]) != type) {
                throw new SemanticError(
                    String.Format(
                        "Operator {0} requires two operands of type {1}",
                        op,
                        type),
                    node.AnchorToken);
            }
        }
    }
}
