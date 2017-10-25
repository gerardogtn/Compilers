/*
Javier Curiel A01020542
Gerardo Teruel A01018057

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

namespace int64 {

    class Parser {

        static readonly ISet<TokenCategory> firstOfStatement =
            new HashSet<TokenCategory>() {
                TokenCategory.IDENTIFIER,
                TokenCategory.IF,
                TokenCategory.SWITCH,
                TokenCategory.WHILE,
                TokenCategory.DO,
                TokenCategory.FOR,
                TokenCategory.BREAK,
                TokenCategory.CONTINUE,
                TokenCategory.RETURN,
                TokenCategory.SEMICOLON
            };

        static readonly ISet<TokenCategory> firstOfBitShift =
            new HashSet<TokenCategory>() {
                TokenCategory.BITWISE_SHIFT_LEFT,
                TokenCategory.BITWISE_SHIFT_RIGHT,
                TokenCategory.BITWISE_UNSIGNED_SHIFT_RIGHT,
            };

        static readonly ISet<TokenCategory> firstOfAdd =
            new HashSet<TokenCategory>() {
                TokenCategory.PLUS,
                TokenCategory.MINUS
            };

        static readonly ISet<TokenCategory>  firstOfUnary =
            new HashSet<TokenCategory>() {
                TokenCategory.PLUS,
                TokenCategory.MINUS,
                TokenCategory.BITWISE_NOT,
                TokenCategory.LOGICAL_NOT
            };


        static readonly ISet<TokenCategory> firstOfLitSimple =
            new HashSet<TokenCategory>() {
                TokenCategory.TRUE,
                TokenCategory.FALSE,
                TokenCategory.INT_LITERAL,
                TokenCategory.CHAR_LITERAL
            };


        static readonly ISet<TokenCategory> firstOfSimpleExpression =
            new HashSet<TokenCategory>() {
                TokenCategory.IDENTIFIER,
                TokenCategory.INT_LITERAL,
                TokenCategory.TRUE,
                TokenCategory.FALSE,
                TokenCategory.PARENTHESIS_OPEN,
                TokenCategory.MINUS
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
            return new Program() { DefList() };
        }

        public Node DefList(){
            var result = new DefList();
            while (CurrentToken == TokenCategory.VAR || CurrentToken == TokenCategory.IDENTIFIER) {
                result.Add(Def());
            }
            return result;
        }

        public void Def(){
            switch(CurrentToken){
                case TokenCategory.VAR:
                    return VarDef();
                case TokenCategory.IDENTIFIER:
                    return FunDef();
                default:
                    throw new SyntaxError(firstOfStatement,tokenStream.Current);
            }
        }

        public void VarDef(){
            var token = Expect(TokenCategory.VAR);
            var varList = VarList();
            varList.AnchorToken = token;

            Expect(TokenCategory.SEMICOLON);
            return varList;
        }

        public void VarList(){
            return IdList();
        }

        public void IdList(){
            var token = Expect(TokenCategory.IDENTIFIER);
            var result = IdListCont();
            result.AnchorToken = token;
            return result;
        }

        public void IdListCont(){
            var result = new IdListCont();
            while (CurrentToken == TokenCategory.COMMA){
                Expect(TokenCategory.COMMA);
                result.Add(new Identifier() {
                    AnchorToken = Expect(TokenCategory.IDENTIFIER)
                };
            }
            return result;
        }

        public void FunDef() {
            var result = new FunDef();
            result.Add(new Identifier() {
                AnchorToken = Expect(TokenCategory.IDENTIFIER)
            };
            Expect(TokenCategory.PARENTHESIS_OPEN);
            result.Add(ParamList());
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.CURLY_BRACES_OPEN);
            result.Add(VarDefList());
            result.Add(StmtList());
            Expect(TokenCategory.CURLY_BRACES_CLOSE);
            return result;
        }

        public void ParamList(){
            var result = new ParamList();
            if (CurrentToken == TokenCategory.IDENTIFIER) {
                result.Add(IdList());
            }
            return result;
        }

        public void VarDefList(){
            var result = new VarDefList();
            while(CurrentToken == TokenCategory.VAR){
                result.Add(VarDef());
            }
            return result;
        }

        public void StmtList(){
            var result = new StmtList();
            while(firstOfStatement.Contains(CurrentToken)){
                result.Add(Stmt());
            }
            return result;
        }

        public void Stmt(){
            switch (CurrentToken) {
                case TokenCategory.IDENTIFIER:
                    return StmtId();
                case TokenCategory.IF:
                    return StmtIf();
                case TokenCategory.SWITCH:
                    return StmtSwitch();
                case TokenCategory.WHILE:
                    return StmtWhile();
                case TokenCategory.DO:
                    return StmtDoWhile();
                case TokenCategory.FOR:
                    return StmtFor();
                case TokenCategory.BREAK:
                    return StmtBreak();
                case TokenCategory.CONTINUE:
                    return StmtContinue();
                case TokenCategory.RETURN:
                    return StmtReturn();
                case TokenCategory.SEMICOLON:
                    return StmtEmpty();
                default:
                    throw new SyntaxError(firstOfStatement, tokenStream.Current);
            }
        }

        public void StmtIf(){
            var result = new StmtIf(){
                AnchorToken = Expect(TokenCategory.IF)
            }
            Expect(TokenCategory.PARENTHESIS_OPEN);
            result.Add(Expr())
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.CURLY_BRACES_OPEN);
            result.Add(StmtList());
            Expect(TokenCategory.CURLY_BRACES_CLOSE);
            result.Add(ElseIfList());
            return result;
        }

        public void ElseIfList(){
            var result = new ElseIfList();
            while (CurrentToken == TokenCategory.ELSE) {
                var token = Expect(TokenCategory.ELSE);
                if (CurrentToken == TokenCategory.IF) {
                    var node = new ElseIf() { AnchorToken = token };
                    Expect(TokenCategory.IF);
                    Expect(TokenCategory.PARENTHESIS_OPEN);
                    node.Add(Expr());
                    Expect(TokenCategory.PARENTHESIS_CLOSE);
                    Expect(TokenCategory.CURLY_BRACES_OPEN);
                    node.Add(StmtList());
                    Expect(TokenCategory.CURLY_BRACES_CLOSE);
                    result.Add(node);
                } else {
                    var node = new Else() { AnchorToken = token };
                    Expect(TokenCategory.CURLY_BRACES_OPEN);
                    node.Add(StmtList());
                    Expect(TokenCategory.CURLY_BRACES_CLOSE);
                    result.Add(node);
                    break;
                }
            }
            return result;
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
            while(CurrentToken == TokenCategory.COMMA){
                Expect(TokenCategory.COMMA);
                LitSimple();
            }

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
            Expect(TokenCategory.PARENTHESIS_OPEN);
            Expr();
            Expect(TokenCategory.PARENTHESIS_CLOSE);
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
            if(CurrentToken == TokenCategory.QUESTION_MARK){
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
                OpComp();
                ExprRel();
            }
        }

        public void OpComp(){
            if (CurrentToken == TokenCategory.EQUAL) {
                Expect(TokenCategory.EQUAL);
            } else if (CurrentToken == TokenCategory.NOT_EQUAL) {
                Expect(TokenCategory.NOT_EQUAL);
            }
        }

        public void ExprRel(){
            ExprBitOr();
            while(CurrentToken == TokenCategory.GREATER_THAN || CurrentToken == TokenCategory.LESS_THAN
            || CurrentToken == TokenCategory.GREATER_OR_EQUAL_THAN || CurrentToken == TokenCategory.LESS_OR_EQUAL_THAN){
                OpRel();
                ExprBitOr();
            }
        }

        public void OpRel(){
            switch(CurrentToken) {
                case TokenCategory.GREATER_THAN:
                    Expect(TokenCategory.GREATER_THAN);
                    break;
                case TokenCategory.LESS_THAN:
                    Expect(TokenCategory.LESS_THAN);
                    break;
                case TokenCategory.GREATER_OR_EQUAL_THAN:
                    Expect(TokenCategory.GREATER_OR_EQUAL_THAN);
                    break;
                case TokenCategory.LESS_OR_EQUAL_THAN:
                    Expect(TokenCategory.LESS_OR_EQUAL_THAN);
                    break;
            }
        }

        public void ExprBitOr() {
            ExprBitAnd();
            while (CurrentToken == TokenCategory.BITWISE_OR || CurrentToken == TokenCategory.BITWISE_XOR) {
                OpBitOr();
                ExprBitAnd();
            }
        }

        public void OpBitOr() {
            if (CurrentToken == TokenCategory.BITWISE_OR) {
                Expect(TokenCategory.BITWISE_OR);
            } else if (CurrentToken == TokenCategory.BITWISE_XOR) {
                Expect(TokenCategory.BITWISE_XOR);
            }
        }

        public void ExprBitAnd() {
            ExprBitShift();
            while(CurrentToken == TokenCategory.BITWISE_AND){
                Expect(TokenCategory.BITWISE_AND);
                ExprBitShift();
            }
        }

        public void ExprBitShift(){
            ExprAdd();
            while(firstOfBitShift.Contains(CurrentToken)){
                OpBitShift();
                ExprAdd();
            }
        }

        public void OpBitShift(){
            switch (CurrentToken) {
                case TokenCategory.BITWISE_SHIFT_LEFT:
                    Expect(TokenCategory.BITWISE_SHIFT_LEFT);
                    break;
                case TokenCategory.BITWISE_SHIFT_RIGHT:
                    Expect(TokenCategory.BITWISE_SHIFT_RIGHT);
                    break;
                case TokenCategory.BITWISE_UNSIGNED_SHIFT_RIGHT:
                    Expect(TokenCategory.BITWISE_UNSIGNED_SHIFT_RIGHT);
                    break;
            }
        }

        public void ExprAdd(){
            ExprMul();
            while(firstOfAdd.Contains(CurrentToken)){
                OpAdd();
                ExprMul();
            }
        }

        public void OpAdd(){
            switch (CurrentToken) {
                case TokenCategory.PLUS:
                    Expect(TokenCategory.PLUS);
                    break;
                case TokenCategory.MINUS:
                    Expect(TokenCategory.MINUS);
                    break;
            }
        }
        public void ExprMul(){
            ExprPow();
            while(IsOpMul()){
                OpMul();
                ExprPow();
            }
        }
        public void OpMul(){
            switch (CurrentToken) {
                case TokenCategory.TIMES:
                    Expect(TokenCategory.TIMES);
                    break;
                case TokenCategory.DIVISION:
                    Expect(TokenCategory.DIVISION);
                    break;
                case TokenCategory.REMAINDER:
                    Expect(TokenCategory.REMAINDER);
                    break;
            }
        }
        public void ExprPow(){
            ExprUnary();
            while(CurrentToken == TokenCategory.POWER){
                Expect(TokenCategory.POWER);
                ExprUnary();
            }
        }
        public void ExprUnary(){
            while(firstOfUnary.Contains(CurrentToken)){
                OpUnary();
            }
            ExprPrimary();
        }

        public void OpUnary(){
            switch (CurrentToken) {
                case TokenCategory.PLUS:
                    Expect(TokenCategory.PLUS);
                    break;
                case TokenCategory.MINUS:
                    Expect(TokenCategory.MINUS);
                    break;
                case TokenCategory.BITWISE_NOT:
                    Expect(TokenCategory.BITWISE_NOT);
                    break;
                case TokenCategory.LOGICAL_NOT:
                    Expect(TokenCategory.LOGICAL_NOT);
                    break;
            }
        }

        public void ExprPrimary(){
           if (CurrentToken == TokenCategory.IDENTIFIER) {
                StmtId();
           } else if (IsLit()) {
                Lit();
           } else if (CurrentToken == TokenCategory.PARENTHESIS_OPEN) {
                Expect(TokenCategory.PARENTHESIS_OPEN);
                Expr();
                Expect(TokenCategory.PARENTHESIS_CLOSE);
           }
           else{
               throw new SyntaxError(TokenCategory.IDENTIFIER,  tokenStream.Current);
           }
        }

        public bool IsLit() {
            return CurrentToken == TokenCategory.TRUE || CurrentToken == TokenCategory.FALSE ||
                CurrentToken == TokenCategory.INT_LITERAL || CurrentToken == TokenCategory.CHAR_LITERAL ||
                CurrentToken == TokenCategory.STRING_LITERAL || CurrentToken == TokenCategory.CURLY_BRACES_OPEN;
        }

        public void StmtId() {
            Expect(TokenCategory.IDENTIFIER);
            if (CurrentToken == TokenCategory.ASSIGN) {
                Expect(TokenCategory.ASSIGN);
                Expr();
                Expect(TokenCategory.SEMICOLON);
            } else if (CurrentToken == TokenCategory.PARENTHESIS_OPEN) {
                Expect(TokenCategory.PARENTHESIS_OPEN);
                ExprList();
                Expect(TokenCategory.PARENTHESIS_CLOSE);
            }
        }

        public void ExprList() {
            if (IsExpr()) {
                Expr();
                ExprListCont();
            }
        }

        public bool IsExpr() {
            return IsLit() || IsOpUnary() || CurrentToken == TokenCategory.IDENTIFIER || CurrentToken == TokenCategory.PARENTHESIS_OPEN
                || IsOpMul() || IsOpAdd() || IsOpBitShift() || IsOpRel() || IsOpComp();
        }

        public bool IsOpUnary() {
            return CurrentToken == TokenCategory.PLUS || CurrentToken == TokenCategory.ASSIGN || CurrentToken == TokenCategory.LOGICAL_NOT || CurrentToken == TokenCategory.BITWISE_NOT;
        }

        public bool IsOpMul() {
            return CurrentToken == TokenCategory.TIMES || CurrentToken == TokenCategory.DIVISION || CurrentToken == TokenCategory.REMAINDER;
        }

        public bool IsOpAdd() {
            return CurrentToken == TokenCategory.PLUS || CurrentToken == TokenCategory.MINUS;
        }

        public bool IsOpBitShift() {
            return CurrentToken == TokenCategory.BITWISE_SHIFT_RIGHT || CurrentToken == TokenCategory.BITWISE_SHIFT_LEFT || CurrentToken == TokenCategory.BITWISE_UNSIGNED_SHIFT_RIGHT;
        }

        public bool IsOpRel() {
            return CurrentToken == TokenCategory.LESS_THAN || CurrentToken == TokenCategory.LESS_OR_EQUAL_THAN || CurrentToken == TokenCategory.GREATER_THAN || CurrentToken == TokenCategory.GREATER_OR_EQUAL_THAN;
        }

        public bool IsOpComp() {
            return CurrentToken == TokenCategory.EQUAL || CurrentToken == TokenCategory.NOT_EQUAL;
        }

        public void ExprListCont() {
            while (CurrentToken == TokenCategory.COMMA) {
                Expect(TokenCategory.COMMA);
                Expr();
            }
        }

        public void Lit(){
            if (firstOfLitSimple.Contains(CurrentToken)){
                LitSimple();
            } else if (CurrentToken == TokenCategory.STRING_LITERAL){
                Expect(TokenCategory.STRING_LITERAL);
            } else if (CurrentToken == TokenCategory.CURLY_BRACES_OPEN){
                ArrayList();
            }
        }
        public void ArrayList(){
            Expect(TokenCategory.CURLY_BRACES_OPEN);
            if(firstOfLitSimple.Contains(CurrentToken)){
                LitList();
            }
            Expect(TokenCategory.CURLY_BRACES_CLOSE);
        }

        public void LitBool(){
            switch (CurrentToken) {
                case TokenCategory.TRUE:
                    Expect(TokenCategory.TRUE);
                    break;

                case TokenCategory.FALSE:
                    Expect(TokenCategory.FALSE);
                    break;
            }
        }
    }
}
