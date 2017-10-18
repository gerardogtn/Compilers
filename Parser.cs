/*
  Buttercup compiler - This class performs the syntactic analysis,
  (a.k.a. parsing).
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

namespace Buttercup {

    class Parser {

        static readonly ISet<TokenCategory> firstOfDeclaration =
            new HashSet<TokenCategory>() {
                TokenCategory.INT,
                TokenCategory.BOOL
            };

        static readonly ISet<TokenCategory> firstOfStatement =
            new HashSet<TokenCategory>() {
                TokenCategory.IDENTIFIER,
                TokenCategory.PRINT,
                TokenCategory.IF
            };

        static readonly ISet<TokenCategory> firstOfOperator =
            new HashSet<TokenCategory>() {
                TokenCategory.AND,
                TokenCategory.LESS,
                TokenCategory.PLUS,
                TokenCategory.MUL
            };

        static readonly ISet<TokenCategory> firstOfSimpleExpression =
            new HashSet<TokenCategory>() {
                TokenCategory.IDENTIFIER,
                TokenCategory.INT_LITERAL,
                TokenCategory.TRUE,
                TokenCategory.FALSE,
                TokenCategory.PARENTHESIS_OPEN,
                TokenCategory.NEG
            };

        IEnumerator<Token> tokenStream;

        public Parser(IEnumerator<Token> tokenStream) {
            this.tokenStream = tokenStream;
            this.tokenStream.MoveNext();
        }

        public TokenCategory CurrentToken {
            get { return tokenStream.Current.Category; }
        }

        public Token Expect(TokenCategory category) {
            if (CurrentToken == category) {
                Token current = tokenStream.Current;
                tokenStream.MoveNext();
                return current;
            } else {
                throw new SyntaxError(category, tokenStream.Current);
            }
        }

        public void Program() {
            DefList();
            // while (firstOfDeclaration.Contains(CurrentToken)) {
            //     Declaration();
            // }
            //
            // while (firstOfStatement.Contains(CurrentToken)) {
            //     Statement();
            // }
            //
            // Expect(TokenCategory.EOF);
        }
        public void DefList(){}


        public void Def(){
            switch(CurrentToken){
                case TokenCategory.VAR:
                    Var_Def();
                    break;
                case TokenCategory.IDENTIFIER:
                    Fun_Def();
                    break;
                default:
                    throw new SyntaxError(firstOfStatement,tokenStream.Current);
            }
        }

        public void VarDef(){
            Expect(TokenCategory.VAR);
            VarList();
            Expect(TokenCategory.SEMICOLON);
        }

        public void VarList(){
            IdList();
        }

        public void IdList(){
            Expect(TokenCategory.IDENTIFIER);
            IdListCont();
        }

        public void IdListCont(){
            while (Current == TokenCategory.PARENTHESIS_OPEN){
                Expect(TokenCategory.PARENTHESIS_OPEN);
                Expect(TokenCategory.COMMA);
                Expect(TokenCategory.IDENTIFIER);
                Expect(TokenCategory.PARENTHESIS_CLOSE);
            }
        }

        public void FunDef(){
            Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.PARENTHESIS_OPEN);
            ParamList();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.CURLY_BRACES_OPEN);
            VarDefList();
            StmtList();
            Expect(TokenCategory.CURLY_BRACES_CLOSE);
        }

        public void ParamList(){
            if(CurrentToken == TokenCategory.IDENTIFIER){
                IdList();
            }
        }

        public void VarDefList(){
            while(CurrentToken == TokenCategory.VAR){
                VarDef();
            }
        }
        public void StmtList(){
            while(firstOfStatement.Contains(CurrentToken)){
                Stmt();
            }
        }
        public void Stmt(){
            switch (CurrentToken) {
                case TokenCategory.IDENTIFIER:
                    // TODO
                    break;
                case TokenCategory.IF:
                    StmtIf();
                    break;
                case TokenCategory.SWITCH:
                    StmtSwitch();
                    break;
                case TokenCategory.WHILE:
                    StmtWhile();
                    break;
                case TokenCategory.DO:
                    StmtDoWhile();
                    break;
                case TokenCategory.FOR:
                    StmtFor();
                    break;
                case TokenCategory.BREAK:
                    StmtBreak();
                    break;
                case TokenCategory.CONTINUE:
                    StmtContinue();
                    break;
                case TokenCategory.RETURN:
                    StmtReturn();
                    break;
                case TokenCategory.SEMICOLON:
                    StmtEmpty();
                    break;
            }
        }

        public void StmtIf(){
            Expect(TokenCategory.IF);
            Expect(TokenCategory.PARENTHESIS_OPEN);
            Expr();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.CURLY_BRACES_OPEN);
            StmtList();
            Expect(TokenCategory.CURLY_BRACES_CLOSE);
            ElseIfList();
            Else();
        }
        public void ElseIfList(){
            while(CurrentToken == TokenCategory.ELSE){
                Expect(TokenCategory.ELSE);
                Expect(TokenCategory.IF);
                Expect(TokenCategory.PARENTHESIS_OPEN);
                Expr();
                Expect(TokenCategory.PARENTHESIS_CLOSE);
                Expect(TokenCategory.CURLY_BRACES_OPEN);
                StmtList();
                Expect(TokenCategory.CURLY_BRACES_CLOSE);
            }
        }
        public void Else(){
            if(CurrentToken == TokenCategory.ELSE){
                Expect(TokenCategory.ELSE);
                Expect(TokenCategory.CURLY_BRACES_OPEN);
                StmtList();
                Expect(TokenCategory.CURLY_BRACES_CLOSE);
            }
        }
        public void StmtSwitch(){
            Expect(TokenCategory.SWITCH);
            Expect(TokenCategory.PARENTHESIS_OPEN);
            Expr();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.CURLY_BRACES_OPEN);
            CaseList();
            Default();
            Expect(TokenCategory.CURLY_BRACES_CLOSE);
        }
        public void CaseList(){
            while(CurrentToken == TokenCategory.CASE){
                Case();
            }
        }
        public void Case(){
            Expect(TokenCategory.CASE);
            LitList();
            Expect(TokenCategory.COLON);
            StmtList();
        }
        public void LitList(){
            LitSimple();
            LitListCont();
        }
        public void LitListCont(){
            Expect(TokenCategory.PARENTHESIS_OPEN);
            Expect(TokenCategory.COMMA);
            LitSimple();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
        }
        public void LitSimple(){
            switch (CurrentToken) {
                case TokenCategory.TRUE:
                    Expect(TokenCategory.TRUE);
                    break;
                case TokenCategory.FALSE:
                    Expect(TokenCategory.FALSE);
                    break;
                case TokenCategory.INT_LITERAL:
                    Expect(TokenCategory.INT_LITERAL);
                    break;
                case TokenCategory.CHAR_LITERAL:
                    Expect(TokenCategory.CHAR_LITERAL);
                    break;
            }
        }
        public void Default(){
            if(CurrentToken == TokenCategory.DEFAULT){
                Expect(TokenCategory.DEFAULT);
                Expect(TokenCategory.COLON);
                StmtList();
            }
        }
        public void StmtWhile(){
            Expect(TokenCategory.WHILE);
            Expect(TokenCategory.PARENTHESIS_OPEN);
            Expr();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.CURLY_BRACES_OPEN);
            StmtList();
            Expect(TokenCategory.CURLY_BRACES_CLOSE);

        }
        public void StmtDoWhile(){
            Expect(TokenCategory.DO);
            Expect(TokenCategory.CURLY_BRACES_OPEN);
            StmtList();
            Expect(TokenCategory.CURLY_BRACES_CLOSE);
            Expect(TokenCategory.WHILE);
            Expect(TokenCategory.CURLY_BRACES_OPEN);
            Expr();
            Expect(TokenCategory.CURLY_BRACES_CLOSE);
            Expect(TokenCategory.SEMICOLON);
        }
        public void StmtFor(){
            Expect(TokenCategory.FOR);
            Expect(TokenCategory.PARENTHESIS_OPEN);
            Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.IN);
            Expr();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.CURLY_BRACES_OPEN);
            StmtList();
            Expect(TokenCategory.CURLY_BRACES_CLOSE);
        }
        public void StmtBreak(){
            Expect(TokenCategory.BREAK);
            Expect(TokenCategory.SEMICOLON);
        }
        public void StmtContinue(){
            Expect(TokenCategory.CONTINUE);
            Expect(TokenCategory.SEMICOLON);
        }
        public void StmtReturn(){
            Expect(TokenCategory.RETURN);
            Expr();
            Expect(TokenCategory.SEMICOLON);
        }
        public void StmtEmpty(){
            Expect(TokenCategory.SEMICOLON);
        }

        public void Expr(){
            ExprCond();
        }
        public void ExprCond(){
            ExprOr();
            if(CurrentToken == Category.QUESTION_MARK){
                Expect(TokenCategory.QUESTION_MARK);
                Expr();
                Expect(TokenCategory.COLON);
                Expr();
            }
        }
        public void ExprOr(){
            ExprAnd();
            while(CurrentToken == TokenCategory.LOGICAL_OR){
                Expect(TokenCategory.LOGICAL_OR);
                ExprAnd();
            }
        }

        public void ExprAnd(){
            ExprComp();
            while(CurrentToken == TokenCategory.LOGICAL_AND){
                Expect(TokenCategory.LOGICAL_AND);
                ExprComp();
            }
        }

        public void ExprComp(){
            ExprRel();
            while(CurrentToken == TokenCategory.EQUAL || CurrentToken == TokenCategory.NOT_EQUAL){
                ExprComp();
                ExprRel();
            }
        }

        public void ExprComp(){
            if (CurrentToken == TokenCategory.EQUAL) {
                Expect(TokenCategory.EQUAL);
            } else if (CurrentToken == TokenCategory.NOT_EQUAL) {
                Expect(TokenCategory.NOT_EQUAL);
            }
        }

        public void ExprRel(){
            ExprBitOr();
            while(CurrentToken == TokenCategory.GREATER_THAN || CurrentToken == TokenCategory.LESS_THAN || CurrentToken == TokenCategory.GREATER_OR_EQUAL_THAN || CurrentToken == TokenCategory.LESS_OR_EQUAL_THAN){
                ExprRel();
                ExprBitOr();
            }
        }


        public void Declaration() {
            Type();
            Expect(TokenCategory.IDENTIFIER);
        }

        public void Statement() {

            switch (CurrentToken) {

            case TokenCategory.IDENTIFIER:
                Assignment();
                break;

            case TokenCategory.PRINT:
                Print();
                break;

            case TokenCategory.IF:
                If();
                break;

            default:
                throw new SyntaxError(firstOfStatement,
                                      tokenStream.Current);
            }
        }

        public void Type() {
            switch (CurrentToken) {

            case TokenCategory.INT:
                Expect(TokenCategory.INT);
                break;

            case TokenCategory.BOOL:
                Expect(TokenCategory.BOOL);
                break;

            default:
                throw new SyntaxError(firstOfDeclaration,
                                      tokenStream.Current);
            }
        }

        public void Assignment() {
            Expect(TokenCategory.IDENTIFIER);
            Expect(TokenCategory.ASSIGN);
            Expression();
        }

        public void Print() {
            Expect(TokenCategory.PRINT);
            Expression();
        }

        public void If() {
            Expect(TokenCategory.IF);
            Expression();
            Expect(TokenCategory.THEN);
            while (firstOfStatement.Contains(CurrentToken)) {
                Statement();
            }
            Expect(TokenCategory.END);
        }

        public void Expression() {
            SimpleExpression();
            while (firstOfOperator.Contains(CurrentToken)) {
                Operator();
                SimpleExpression();
            }
        }

        public void SimpleExpression() {


            switch (CurrentToken) {

            case TokenCategory.IDENTIFIER:
                Expect(TokenCategory.IDENTIFIER);
                break;

            case TokenCategory.INT_LITERAL:
                Expect(TokenCategory.INT_LITERAL);
                break;

            case TokenCategory.TRUE:
                Expect(TokenCategory.TRUE);
                break;

            case TokenCategory.FALSE:
                Expect(TokenCategory.FALSE);
                break;

            case TokenCategory.PARENTHESIS_OPEN:
                Expect(TokenCategory.PARENTHESIS_OPEN);
                Expression();
                Expect(TokenCategory.PARENTHESIS_CLOSE);
                break;

            case TokenCategory.NEG:
                Expect(TokenCategory.NEG);
                SimpleExpression();
                break;

            default:
                throw new SyntaxError(firstOfSimpleExpression,
                                      tokenStream.Current);
            }
        }

        public void Operator() {
            var firstOperator = TokenCategory.ASSIGN;
            var lastOperator = TokenCategory.CURLY_BRACES_CLOSE;

            if (CurrentToken >= firstOperator && CurrentToken <= lastOperator){
                Expect(CurrentToken);
            }
            else{
                throw new SyntaxError(firstOfOperator,tokenStream.Current);
            }
            }
        }
    }
}
