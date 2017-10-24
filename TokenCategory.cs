/*
Javier Curiel A01020542
Gerardo Teruel A01018057

  Buttercup compiler - Token categories for the scanner.
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

    enum TokenCategory {
        /* Identifier */
        IDENTIFIER,

        /* Keywords*/
        BREAK,
        CASE,
        CONTINUE,
        DEFAULT,
        DO,
        ELSE,
        FALSE,
        FOR,
        IF,
        IN,
        RETURN,
        SWITCH,
        TRUE,
        WHILE,
        VAR,

        /* Literals */
        INT_LITERAL,
        CHAR_LITERAL,
        STRING_LITERAL,

        /* Operators */
        ASSIGN,
        MINUS,
        PLUS,
        TIMES,
        DIVISION,
        REMAINDER,
        POWER,
        BITWISE_NOT,
        BITWISE_AND,
        BITWISE_OR,
        BITWISE_XOR,
        BITWISE_SHIFT_LEFT,
        BITWISE_SHIFT_RIGHT,
        BITWISE_UNSIGNED_SHIFT_RIGHT,
        LOGICAL_NOT,
        LOGICAL_AND,
        LOGICAL_OR,
        EQUAL,
        NOT_EQUAL,
        GREATER_THAN,
        LESS_THAN,
        GREATER_OR_EQUAL_THAN,
        LESS_OR_EQUAL_THAN,
        QUESTION_MARK,
        PARENTHESIS_OPEN,
        PARENTHESIS_CLOSE,
        CURLY_BRACES_OPEN,
        CURLY_BRACES_CLOSE,

        /* Separators */
        COMMENT,
        COMMA,
        COLON,
        SEMICOLON,
        EOF,

        /* Other */
        ILLEGAL_CHAR
    }
}
