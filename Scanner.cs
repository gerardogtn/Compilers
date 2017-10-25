/*
Javier Curiel A01020542
Gerardo Teruel A01018057

  Buttercup compiler - This class performs the lexical analysis,
  (a.k.a. scanning).
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
using System.Text;
using System.Text.RegularExpressions;

namespace int64 {

    class Scanner {

        readonly string input;

        static readonly Regex regex = new Regex(
            @"
                (?<Comment>                   [/][/].*                                         )
              | (?<MultilineComment>          ([/][*][^*]*[*]+([^*/][^*]*[*]+)*[/])            )
              | (?<Identifier>                [a-zA-Z][a-zA-Z0-9_]*                            )
              | (?<IntLiteral>                0[bB][01]+|0[oO][0-7]+|0[xX][0-9a-fA-f]+|\d+     )
              | (?<CharLiteral>               [']([^""\\]|\\n|\\r|\\t|\\\\|\\'|\\""|\\u[0-9a-zA-Z]{6})?[']       )
              | (?<StringLiteral>             ""(\'|\\""|[^""\n])*""                           )
              | (?<Equal>                     [=]{2}                                           )
              | (?<NotEqual>                  [!][=]                                           )
              | (?<GreaterOrEqualThan>        [>][=]                                           )
              | (?<BitwiseUnsignedShiftRight> [>]{3}                                           )
              | (?<BitwiseShiftRight>         [>]{2}                                           )
              | (?<GreaterThan>               [>]                                              )
              | (?<LessOrEqualThan>           [<][=]                                           )
              | (?<BitwiseShiftLeft>          [<]{2}                                           )
              | (?<LessThan>                  [<]                                              )
              | (?<Assign>                    [=]                                              )
              | (?<Minus>                     [-]                                              )
              | (?<Plus>                      [+]                                              )
              | (?<Power>                     [*]{2}                                           )
              | (?<Times>                     [*]                                              )
              | (?<Division>                  [/]                                              )
              | (?<Remainder>                 [%]                                              )
              | (?<BitwiseNot>                [~]                                              )
              | (?<LogicalAnd>                [&]{2}                                           )
              | (?<BitwiseAnd>                [&]                                              )
              | (?<LogicalOr>                 [|]{2}                                           )
              | (?<BitwiseOr>                 [|]                                              )
              | (?<BitwiseXor>                [\^]                                             )
              | (?<LogicalNot>                [!]                                              )
              | (?<QuestionMark>              [?]                                              )
              | (?<Colon>                     [:]                                              )
              | (?<ParenthesisOpen>           [(]                                              )
              | (?<ParenthesisClose>          [)]                                              )
              | (?<CurlyBracesOpen>           [{]                                              )
              | (?<CurlyBracesClose>          [}]                                              )
              | (?<Comma>                     [,]                                              )
              | (?<SemiColon>                 [;]                                              )
              | (?<Newline>                   \n                                               )
              | (?<WhiteSpace>                \s                                               )     # Must go anywhere after Newline.
              | (?<Other>                     .                                                )     # Must be last: match any other character.
            ",
            RegexOptions.IgnorePatternWhitespace
                | RegexOptions.Compiled
                | RegexOptions.Multiline
            );

        static readonly IDictionary<string, TokenCategory> keywords =
            new Dictionary<string, TokenCategory>() {
                {"break", TokenCategory.BREAK},
                {"case", TokenCategory.CASE},
                {"continue", TokenCategory.CONTINUE},
                {"default", TokenCategory.DEFAULT},
                {"do", TokenCategory.DO},
                {"else", TokenCategory.ELSE},
                {"false", TokenCategory.FALSE},
                {"for", TokenCategory.FOR},
                {"if", TokenCategory.IF},
                {"in", TokenCategory.IN},
                {"return", TokenCategory.RETURN},
                {"switch", TokenCategory.SWITCH},
                {"true", TokenCategory.TRUE},
                {"while", TokenCategory.WHILE},
                {"var", TokenCategory.VAR}
            };

        static readonly IDictionary<string, TokenCategory> nonKeywords =
            new Dictionary<string, TokenCategory>() {
                {"IntLiteral", TokenCategory.INT_LITERAL},
                {"CharLiteral", TokenCategory.CHAR_LITERAL},
                {"StringLiteral", TokenCategory.STRING_LITERAL},
                {"Equal", TokenCategory.EQUAL},
                {"NotEqual", TokenCategory.NOT_EQUAL},
                {"GreaterOrEqualThan", TokenCategory.GREATER_OR_EQUAL_THAN},
                {"BitwiseUnsignedShiftRight", TokenCategory.BITWISE_UNSIGNED_SHIFT_RIGHT},
                {"BitwiseShiftRight", TokenCategory.BITWISE_SHIFT_RIGHT},
                {"GreaterThan", TokenCategory.GREATER_THAN},
                {"LessOrEqualThan", TokenCategory.LESS_OR_EQUAL_THAN},
                {"BitwiseShiftLeft", TokenCategory.BITWISE_SHIFT_LEFT},
                {"LessThan", TokenCategory.LESS_THAN},
                {"Assign", TokenCategory.ASSIGN},
                {"Minus", TokenCategory.MINUS},
                {"Plus", TokenCategory.PLUS},
                {"Power", TokenCategory.POWER},
                {"Times", TokenCategory.TIMES},
                {"Division", TokenCategory.DIVISION},
                {"Remainder", TokenCategory.REMAINDER},
                {"BitwiseNot", TokenCategory.BITWISE_NOT},
                {"LogicalAnd", TokenCategory.LOGICAL_AND},
                {"BitwiseAnd", TokenCategory.BITWISE_AND},
                {"LogicalOr", TokenCategory.LOGICAL_OR},
                {"BitwiseOr", TokenCategory.BITWISE_OR},
                {"BitwiseXor", TokenCategory.BITWISE_XOR},
                {"LogicalNot", TokenCategory.LOGICAL_NOT},
                {"QuestionMark", TokenCategory.QUESTION_MARK},
                {"ParenthesisOpen", TokenCategory.PARENTHESIS_OPEN},
                {"ParenthesisClose", TokenCategory.PARENTHESIS_CLOSE},
                {"CurlyBracesOpen", TokenCategory.CURLY_BRACES_OPEN},
                {"CurlyBracesClose", TokenCategory.CURLY_BRACES_CLOSE},
                {"Colon", TokenCategory.COLON},
                {"SemiColon", TokenCategory.SEMICOLON},
                {"Comma", TokenCategory.COMMA}
            };

        public Scanner(string input) {
            this.input = input;
        }

        public IEnumerable<Token> Start() {

            var row = 1;
            var columnStart = 0;

            Func<Match, TokenCategory, Token> newTok = (m, tc) =>
                new Token(m.Value, tc, row, m.Index - columnStart + 1);

            foreach (Match m in regex.Matches(input)) {

                if (m.Groups["Newline"].Success) {

                    // Found a new line.
                    row++;
                    columnStart = m.Index + m.Length;

                } else if (m.Groups["WhiteSpace"].Success
                    || m.Groups["Comment"].Success) {
                    // Skip white space and comments.

                } else if (m.Groups["MultilineComment"].Success) {
                  String value = m.Value;
                  row += value.Length - value.Replace("\n", "").Length;
                } else if (m.Groups["Identifier"].Success) {

                    if (keywords.ContainsKey(m.Value)) {

                        // Matched string is a Buttercup keyword.
                        yield return newTok(m, keywords[m.Value]);

                    } else {

                        // Otherwise it's just a plain identifier.
                        yield return newTok(m, TokenCategory.IDENTIFIER);
                    }

                } else if (m.Groups["Other"].Success) {

                    // Found an illegal character.
                    yield return newTok(m, TokenCategory.ILLEGAL_CHAR);

                } else {

                    // Match must be one of the non keywords.
                    foreach (var name in nonKeywords.Keys) {
                        if (m.Groups[name].Success) {
                            yield return newTok(m, nonKeywords[name]);
                            break;
                        }
                    }
                }
            }

            yield return new Token(null,
                                   TokenCategory.EOF,
                                   row,
                                   input.Length - columnStart + 1);
        }
    }
}
