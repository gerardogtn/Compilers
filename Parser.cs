/*
  Javier Curiel A01020542
  Gerardo Teruel A01018057
  Angel Tellez A01022029

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

        public Node Program() {
            var result = new Program();
            while (CurrentToken == TokenCategory.VAR || CurrentToken == TokenCategory.IDENTIFIER) {
                result.Add(Def());
            }
            return result;
        }

        public Node Def() {
            switch(CurrentToken){
                case TokenCategory.VAR:
                    return VarDef();
                case TokenCategory.IDENTIFIER:
                    return FunDef();
                default:
                    throw new SyntaxError(firstOfStatement,tokenStream.Current);
            }
        }

        public Node VarDef() {
            var token = Expect(TokenCategory.VAR);
            var varList = new VarDefList();

            IdList(varList);
            varList.AnchorToken = token;

            Expect(TokenCategory.SEMICOLON);
            return varList;
        }

        public void IdList(Node result) {
            result.Add(new Identifier() { AnchorToken = Expect(TokenCategory.IDENTIFIER) });
            IdListCont(result);
        }

        public void IdListCont(Node node) {
            while (CurrentToken == TokenCategory.COMMA){
                Expect(TokenCategory.COMMA);
                node.Add(new Identifier() {
                    AnchorToken = Expect(TokenCategory.IDENTIFIER)
                });
            }
        }

        public Node FunDef() {
            var result = new FunDef() { AnchorToken = Expect(TokenCategory.IDENTIFIER) };
            Expect(TokenCategory.PARENTHESIS_OPEN);
            result.Add(ParamList());
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.CURLY_BRACES_OPEN);
            result.Add(VarDefList());
            result.Add(StmtList());
            Expect(TokenCategory.CURLY_BRACES_CLOSE);
            return result;
        }

        public Node ParamList() {
            var result = new ParamList();
            if (CurrentToken == TokenCategory.IDENTIFIER) {
                IdList(result);
            }
            return result;
        }

        public Node VarDefList() {
            var result = new VarDefList();
            while(CurrentToken == TokenCategory.VAR){
                result.Add(VarDef());
            }
            return result;
        }

        public Node StmtList() {
            var result = new StmtList();
            while(firstOfStatement.Contains(CurrentToken)){
                result.Add(Stmt());
            }
            return result;
        }

        public Node Stmt() {
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

        public Node StmtIf() {
            var result = new StmtIf() {
                AnchorToken = Expect(TokenCategory.IF)
            };
            Expect(TokenCategory.PARENTHESIS_OPEN);
            result.Add(Expr());
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.CURLY_BRACES_OPEN);
            result.Add(StmtList());
            Expect(TokenCategory.CURLY_BRACES_CLOSE);
            result.Add(ElseIfList());
            return result;
        }

        public Node ElseIfList() {
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

        public Node StmtSwitch() {
            var result = new StmtSwitch() {
                AnchorToken = Expect(TokenCategory.SWITCH)
            };
            Expect(TokenCategory.PARENTHESIS_OPEN);
            result.Add(Expr());
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.CURLY_BRACES_OPEN);
            result.Add(CaseList());
            result.Add(Default());
            Expect(TokenCategory.CURLY_BRACES_CLOSE);
            return result;
        }

        public Node CaseList() {
            var result = new CaseList();
            while(CurrentToken == TokenCategory.CASE){
                result.Add(Case());
            }
            return result;
        }

        public Node Case() {
            var result = new Case() {
                AnchorToken = Expect(TokenCategory.CASE)
            };
            result.Add(LitList());
            Expect(TokenCategory.COLON);
            result.Add(StmtList());
            return result;
        }

        public Node LitList() {
            var result = new LitList() {
                LitSimple()
            };
            LitListCont(result);
            return result;
        }

        public void LitListCont(Node result) {
            while(CurrentToken == TokenCategory.COMMA){
                Expect(TokenCategory.COMMA);
                result.Add(LitSimple());
            }
        }

        public Node LitSimple() {
            switch (CurrentToken) {
                case TokenCategory.TRUE:
                    return new True() { AnchorToken = Expect(TokenCategory.TRUE) }; 
                case TokenCategory.FALSE:
                    return new False() { AnchorToken = Expect(TokenCategory.FALSE) };
                case TokenCategory.INT_LITERAL:
                    return new IntLiteral() { AnchorToken = Expect(TokenCategory.INT_LITERAL) };
                case TokenCategory.CHAR_LITERAL:
                    return new CharLiteral() { AnchorToken = Expect(TokenCategory.CHAR_LITERAL) };
                default:
                    throw new SyntaxError(TokenCategory.TRUE, tokenStream.Current);
            }
        }

        public Node Default() {
            var result = new Default();
            if(CurrentToken == TokenCategory.DEFAULT){
                result.AnchorToken = Expect(TokenCategory.DEFAULT);
                Expect(TokenCategory.COLON);
                result.Add(StmtList());
            }
            return result;
        }

        public Node StmtWhile() {
            var result = new StmtWhile() {
                AnchorToken = Expect(TokenCategory.WHILE)
            };
            Expect(TokenCategory.PARENTHESIS_OPEN);
            result.Add(Expr());
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.CURLY_BRACES_OPEN);
            result.Add(StmtList());
            Expect(TokenCategory.CURLY_BRACES_CLOSE);
            return result;

        }

        public Node StmtDoWhile() {
            var result = new StmtDoWhile() {
                AnchorToken = Expect(TokenCategory.DO)
            };
            Expect(TokenCategory.CURLY_BRACES_OPEN);
            result.Add(StmtList());
            Expect(TokenCategory.CURLY_BRACES_CLOSE);
            Expect(TokenCategory.WHILE);
            Expect(TokenCategory.PARENTHESIS_OPEN);
            result.Add(Expr());
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.SEMICOLON);
            return result;
        }

        public Node StmtFor() {
            var result = new StmtFor() {
                AnchorToken = Expect(TokenCategory.FOR)
            };
            Expect(TokenCategory.PARENTHESIS_OPEN);
            result.Add(new Identifier() { AnchorToken = Expect(TokenCategory.IDENTIFIER) });
            Expect(TokenCategory.IN);
            result.Add(Expr());
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            Expect(TokenCategory.CURLY_BRACES_OPEN);
            result.Add(StmtList());
            Expect(TokenCategory.CURLY_BRACES_CLOSE);
            return result;
        }

        public Node StmtBreak() {
            var result = new StmtBreak() {
                AnchorToken = Expect(TokenCategory.BREAK)
            };
            Expect(TokenCategory.SEMICOLON);
            return result;
        }

        public Node StmtContinue() {
            var result = new StmtContinue() {
                AnchorToken = Expect(TokenCategory.CONTINUE)
            };
            Expect(TokenCategory.SEMICOLON);
            return result;
        }

        public Node StmtReturn() {
            var result = new StmtReturn() {
                AnchorToken = Expect(TokenCategory.RETURN)
            };
            result.Add(Expr());
            Expect(TokenCategory.SEMICOLON);
            return result;
        }

        public Node StmtEmpty() {
            return new StmtEmpty() { 
                AnchorToken = Expect(TokenCategory.SEMICOLON) 
            };
        }

        public Node Expr() {
            return ExprCond();
        }

        public Node ExprCond() {
            var result = ExprOr();
            if(CurrentToken == TokenCategory.QUESTION_MARK) {
                var node = new TernaryOperator() {
                    AnchorToken = Expect(TokenCategory.QUESTION_MARK)
                };
                node.Add(result);
                node.Add(Expr());
                Expect(TokenCategory.COLON);
                node.Add(Expr());
                result = node;
            }
            return result;
        }
        public Node ExprOr(){
            var result = ExprAnd();
            while(CurrentToken == TokenCategory.LOGICAL_OR) {
                var or = new LogicalOr() { AnchorToken = Expect(TokenCategory.LOGICAL_OR) };
                or.Add(result);
                or.Add(ExprAnd());
                result = or;
            }
            return result;
        }

        public Node ExprAnd() {
            var result = ExprComp();
            while(CurrentToken == TokenCategory.LOGICAL_AND) {
                var and = new LogicalAnd() { AnchorToken = Expect(TokenCategory.LOGICAL_AND) };
                and.Add(result);
                and.Add(ExprComp());
                result = and;
            }
            return result;
        }

        public Node ExprComp() {
            var result = ExprRel();
            while(CurrentToken == TokenCategory.EQUAL || CurrentToken == TokenCategory.NOT_EQUAL) {
                var node = OpComp();
                node.Add(result);
                node.Add(ExprRel());
                result = node;
            }
            return result;
        }

        public Node OpComp() {
            if (CurrentToken == TokenCategory.EQUAL) {
                return new Equal() { AnchorToken = Expect(TokenCategory.EQUAL) };
            } else if (CurrentToken == TokenCategory.NOT_EQUAL) {
                return new NotEqual() { AnchorToken = Expect(TokenCategory.NOT_EQUAL) };
            } else {
                throw new SyntaxError(TokenCategory.EQUAL, tokenStream.Current);
            }
        }

        public Node ExprRel() {
            var result = ExprBitOr();
            while(CurrentToken == TokenCategory.GREATER_THAN || CurrentToken == TokenCategory.LESS_THAN
            || CurrentToken == TokenCategory.GREATER_OR_EQUAL_THAN || CurrentToken == TokenCategory.LESS_OR_EQUAL_THAN){
                var node = OpRel();
                node.Add(result);
                node.Add(ExprBitOr());
                result = node;
            }
            return result;
        }

        public Node OpRel(){
            switch(CurrentToken) {
                case TokenCategory.GREATER_THAN:
                    return new GreaterThan() { AnchorToken = Expect(TokenCategory.GREATER_THAN) };
                case TokenCategory.LESS_THAN:
                    return new LessThan() { AnchorToken = Expect(TokenCategory.LESS_THAN) };
                case TokenCategory.GREATER_OR_EQUAL_THAN:
                    return new GreaterEqualThan() { AnchorToken = Expect(TokenCategory.GREATER_OR_EQUAL_THAN) };
                case TokenCategory.LESS_OR_EQUAL_THAN:
                    return new LessEqualThan() { AnchorToken = Expect(TokenCategory.LESS_OR_EQUAL_THAN) };
                default:
                    throw new SyntaxError(TokenCategory.GREATER_THAN, tokenStream.Current);
            }
        }

        public Node ExprBitOr() {
            var result = ExprBitAnd();
            while (CurrentToken == TokenCategory.BITWISE_OR || CurrentToken == TokenCategory.BITWISE_XOR) {
                var node = OpBitOr();
                node.Add(result);
                node.Add(ExprBitAnd());
                result = node;
            }
            return result;
        }

        public Node OpBitOr() {
            if (CurrentToken == TokenCategory.BITWISE_OR) {
                return new BitwiseOr() { AnchorToken = Expect(TokenCategory.BITWISE_OR) };
            } else if (CurrentToken == TokenCategory.BITWISE_XOR) {
                return new BitwiseXor() { AnchorToken = Expect(TokenCategory.BITWISE_XOR) };
            } else {
                throw new SyntaxError(TokenCategory.BITWISE_OR, tokenStream.Current);
            }
        }

        public Node ExprBitAnd() {
            var result = ExprBitShift();
            while(CurrentToken == TokenCategory.BITWISE_AND) {
                var node = new BitwiseAnd() { AnchorToken = Expect(TokenCategory.BITWISE_AND) };
                node.Add(result);
                node.Add(ExprBitShift());
                result = node;
            }
            return result;
        }

        public Node ExprBitShift() {
            var result = ExprAdd();
            while(firstOfBitShift.Contains(CurrentToken)){
                var node = OpBitShift();
                result.Add(node);
                result.Add(ExprAdd());
                result = node;
            }
            return result;
        }

        public Node OpBitShift() {
            switch (CurrentToken) {
                case TokenCategory.BITWISE_SHIFT_LEFT:
                    return new BitwiseShiftLeft() { AnchorToken = Expect(TokenCategory.BITWISE_SHIFT_LEFT) };
                case TokenCategory.BITWISE_SHIFT_RIGHT:
                    return new BitwiseShiftRight() { AnchorToken = Expect(TokenCategory.BITWISE_SHIFT_RIGHT) };
                case TokenCategory.BITWISE_UNSIGNED_SHIFT_RIGHT:
                    return new BitwiseUnsignedShiftRight() { 
                        AnchorToken = Expect(TokenCategory.BITWISE_UNSIGNED_SHIFT_RIGHT)
                    };
                default:
                    throw new SyntaxError(TokenCategory.BITWISE_SHIFT_LEFT, tokenStream.Current);
            }
        }

        public Node ExprAdd() {
            var result = ExprMul();
            while(firstOfAdd.Contains(CurrentToken)) { 
                var node = OpAdd();
                node.Add(result);
                node.Add(ExprMul());
                result = node;
            }
            return result;
        }

        public Node OpAdd() {
            switch (CurrentToken) {
                case TokenCategory.PLUS:
                    return new Plus() { AnchorToken = Expect(TokenCategory.PLUS) };
                case TokenCategory.MINUS:
                    return new Minus() { AnchorToken = Expect(TokenCategory.MINUS) };
                default:
                    throw new SyntaxError(TokenCategory.PLUS, tokenStream.Current);    
            }
        }

        public Node ExprMul() {
            var result = ExprPow();
            while(IsOpMul()){
                var node = OpMul();
                node.Add(result);
                node.Add(ExprPow());
                result = node;
            }
            return result;
        }

        public Node OpMul() {
            switch (CurrentToken) {
                case TokenCategory.TIMES:
                    return new Times() { AnchorToken = Expect(TokenCategory.TIMES) };
                case TokenCategory.DIVISION:
                    return new Division() { AnchorToken = Expect(TokenCategory.DIVISION) };
                case TokenCategory.REMAINDER:
                    return new Remainder() { AnchorToken = Expect(TokenCategory.REMAINDER) };
                default:
                    throw new SyntaxError(TokenCategory.TIMES, tokenStream.Current);
            }
        }

        public Node ExprPow() {
            var result = ExprUnary();
            while (CurrentToken == TokenCategory.POWER){
                var node = new Power() { AnchorToken = Expect(TokenCategory.POWER) };
                node.Add(ExprUnary());
                node.Add(result);
                result = node;
            }
            return result;
        }

        public Node ExprUnary() {
            Node result = null;
            while (firstOfUnary.Contains(CurrentToken)) {
                var node = OpUnary();
                if (result != null) {
                    node.Add(result);
                } 
                result = node;
            }

            var expr = ExprPrimary();
            if (result != null) {
                result.Add(expr);
            } else {
                result = expr;
            }
            return result;
        }

        public Node OpUnary(){
            switch (CurrentToken) {
                case TokenCategory.PLUS:
                    return new Plus() { AnchorToken = Expect(TokenCategory.PLUS) };
                case TokenCategory.MINUS:
                    return new Minus() { AnchorToken = Expect(TokenCategory.MINUS) };
                case TokenCategory.BITWISE_NOT:
                    return new BitwiseNot() { AnchorToken = Expect(TokenCategory.BITWISE_NOT) };
                case TokenCategory.LOGICAL_NOT:
                    return new LogicalNot() { AnchorToken = Expect(TokenCategory.LOGICAL_NOT) };
                default:
                    throw new SyntaxError(TokenCategory.PLUS, tokenStream.Current);
            }
        }

        public Node ExprPrimary() {
           if (CurrentToken == TokenCategory.IDENTIFIER) {
                var id = Expect(TokenCategory.IDENTIFIER);
                if (CurrentToken == TokenCategory.PARENTHESIS_OPEN) {
                    return FunCall(id);
                } else {
                    return new Identifier() { AnchorToken = id };;
                }
           } else if (IsLit()) {
                return Lit();
           } else if (CurrentToken == TokenCategory.PARENTHESIS_OPEN) {
                Expect(TokenCategory.PARENTHESIS_OPEN);
                var expr = Expr();
                Expect(TokenCategory.PARENTHESIS_CLOSE);
                return expr;
           } else {
               throw new SyntaxError(TokenCategory.IDENTIFIER,  tokenStream.Current);
           }
        }

        public bool IsLit() {
            return CurrentToken == TokenCategory.TRUE || CurrentToken == TokenCategory.FALSE ||
                CurrentToken == TokenCategory.INT_LITERAL || CurrentToken == TokenCategory.CHAR_LITERAL ||
                CurrentToken == TokenCategory.STRING_LITERAL || CurrentToken == TokenCategory.CURLY_BRACES_OPEN;
        }

        public Node StmtId() {
            var id = Expect(TokenCategory.IDENTIFIER);
            if (CurrentToken == TokenCategory.ASSIGN) {
                var result = new Assignment() { AnchorToken = Expect(TokenCategory.ASSIGN) };
                result.Add(new Identifier() { AnchorToken = id });
                result.Add(Expr());
                Expect(TokenCategory.SEMICOLON);
                return result;
            } else if (CurrentToken == TokenCategory.PARENTHESIS_OPEN) {
                var result = FunCall(id);
                Expect(TokenCategory.SEMICOLON);
                return result;
            } else {
                return new Identifier() { AnchorToken = id };
            }
        }

        public Node FunCall(Token id) {
            var result = new FunCall() { AnchorToken = id  };
            Expect(TokenCategory.PARENTHESIS_OPEN);
            ExprList(result);
            Expect(TokenCategory.PARENTHESIS_CLOSE);
            return result;
        }

        public void ExprList(Node result) {
            if (IsExpr()) {
                result.Add(Expr());
                ExprListCont(result);
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

        public void ExprListCont(Node result) {
            while (CurrentToken == TokenCategory.COMMA) {
                Expect(TokenCategory.COMMA);
                result.Add(Expr());
            }
        }

        public Node Lit() {
            if (firstOfLitSimple.Contains(CurrentToken)) {
                return LitSimple();
            } else if (CurrentToken == TokenCategory.STRING_LITERAL) {
                return new StringLiteral() { AnchorToken = Expect(TokenCategory.STRING_LITERAL) };
            } else if (CurrentToken == TokenCategory.CURLY_BRACES_OPEN) {
                return ArrayList();
            } else {
                throw new SyntaxError(TokenCategory.STRING_LITERAL, tokenStream.Current);
            }
        }
        public Node ArrayList() {
            var result = new ArrayList() { AnchorToken = Expect(TokenCategory.CURLY_BRACES_OPEN) };
            if (firstOfLitSimple.Contains(CurrentToken)) {
                result.Add(LitList());
            }
            Expect(TokenCategory.CURLY_BRACES_CLOSE);
            return result;
        }

        public Node LitBool() {
            switch (CurrentToken) {
                case TokenCategory.TRUE:
                    return new True() { AnchorToken = Expect(TokenCategory.TRUE) };
                case TokenCategory.FALSE:
                    return new False() { AnchorToken = Expect(TokenCategory.FALSE) };
                default: 
                    throw new SyntaxError(TokenCategory.TRUE, tokenStream.Current);
            }
        }
    }
}
