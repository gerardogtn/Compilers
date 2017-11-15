/*
  Buttercup compiler - Specific node subclasses for the AST (Abstract
  Syntax Tree).
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

namespace int64 {

    class Program: Node {}
    class ParamList : Node {}

    class FunDef : Node {}
    class VarDefList : Node {}
    class StmtList : Node {}
    class StmtIf : Node {}
    class ElseIfList : Node {}
    class ElseIf : Node {}
    class Else : Node {}

    class StmtSwitch : Node {}
    class CaseList : Node {}
    class Case : Node {}
    class LitList : Node {}
    class Default : Node {}

    class StmtWhile : Node {}
    class StmtDoWhile : Node {}
    class StmtFor : Node {}
    class StmtBreak : Node {}
    class StmtContinue : Node {}
    class StmtReturn : Node {}
    class StmtEmpty : Node {}

    class TernaryOperator : Node {}
    class LogicalOr : Node {}
    class LogicalAnd : Node {}
    class Equal : Node {}
    class NotEqual : Node {}

    class GreaterThan : Node {}
    class GreaterEqualThan : Node {}
    class LessThan : Node {}
    class LessEqualThan : Node {}

    class BitwiseOr : Node {}
    class BitwiseXor : Node {}
    class BitwiseAnd : Node {}
    class BitwiseShiftLeft : Node {}
    class BitwiseShiftRight : Node {}
    class BitwiseUnsignedShiftRight : Node {}

    class Plus : Node {}
    class Minus : Node {}

    class Times : Node {}
    class Division : Node {}
    class Remainder : Node {}

    class Power : Node {}

    class BitwiseNot : Node {}
    class LogicalNot : Node {}

    class FunCall: Node {}
    class ArrayList : Node {}

    class True: Node {}
    class False: Node {}

    class Identifier : Node {}

    class IntLiteral : Node {}
    class CharLiteral : Node {}
    class StringLiteral : Node {}

    class Assignment: Node {}

}
