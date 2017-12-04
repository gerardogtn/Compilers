/*
  Javier Curiel A01020542
  Gerardo Teruel A01018057
  Ángel Téllez A01022029

  Buttercup compiler - Program driver.
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
using System.IO;
using System.Text;

namespace int64 {

    public class Driver {

        const string VERSION = "0.4";

        //-----------------------------------------------------------
        static readonly string[] ReleaseIncludes = {
            "Lexical analysis",
            "Syntactic analysis",
            "AST construction",
            "Semantic Analysis"
        };

        //-----------------------------------------------------------
        void PrintAppHeader() {
            Console.WriteLine("int64 compiler, version " + VERSION);
            Console.WriteLine("Copyright \u00A9 2017 by G. Teruel,  J. Curiel. & Á. Téllez"
            );
            Console.WriteLine("This program is free software; you may "
                + "redistribute it under the terms of");
            Console.WriteLine("the GNU General Public License version 3 or "
                + "later.");
            Console.WriteLine("This program has absolutely no warranty.");
        }

        //-----------------------------------------------------------
        void PrintReleaseIncludes() {
            Console.WriteLine("Included in this release:");
            foreach (var phase in ReleaseIncludes) {
                Console.WriteLine("   * " + phase);
            }
        }

        //-----------------------------------------------------------
        void Run(string[] args) {

            PrintAppHeader();
            Console.WriteLine();
            PrintReleaseIncludes();
            Console.WriteLine();

            if (args.Length != 1) {
                Console.Error.WriteLine(
                    "Please specify the name of the input file.");
                Environment.Exit(1);
            }

            try {
                var inputPath = args[0];
                var input = File.ReadAllText(inputPath);
                var parser = new Parser(new Scanner(input).Start().GetEnumerator());
                var program = parser.Program();
                Console.WriteLine("Syntax OK.");

                var semanticAnalyzer = new SemanticAnalyzer();
                Console.WriteLine(program.ToStringTree());
                semanticAnalyzer.Run(program);

                Console.WriteLine("Semantics OK.");

                Console.WriteLine("\nGlobal variables");
                Console.WriteLine("================");
                foreach (var entry in semanticAnalyzer.GlobalVariablesNamespace) {
                    Console.WriteLine(entry);
                }

                var globalVariables = semanticAnalyzer.GlobalVariablesNamespace;
                var functions = semanticAnalyzer.FunctionNamespace;

                // Console.WriteLine("\nFunctions table");
                // Console.WriteLine("======================================================================");
                // Console.WriteLine($"{"Name",-30}{"Arity",-9}{"Parameters",-15}{"Local variables",-15}");
                // Console.WriteLine("======================================================================");
                // foreach (var entry in semanticAnalyzer.FunctionNamespace) {
                //     Console.WriteLine("{0,-30}{1,-9}{2,-15}{3,-15}",
                //         entry.Key,
                //         entry.Value.Arity,
                //         string.Join(", ", entry.Value.Parameters),
                //         string.Join(", ", entry.Value.LocalVars));
                // }

                var CilGenerator = new CilGenerator(globalVariables, functions);

                var outputPath = inputPath.Replace(".int64", ".il");
                if (!outputPath.Equals(inputPath)) {
                    File.WriteAllText(outputPath, CilGenerator.Run(program));
                    Console.WriteLine(
                    "Generated CIL code to '" + outputPath + "'.");
                Console.WriteLine();
                } else {
                    Console.Error.WriteLine("Incorrect input file");
                    Environment.Exit(1);
                }

            } catch (Exception e) {
                if (e is FileNotFoundException || e is SyntaxError || e is SemanticError) {
                    Console.Error.WriteLine(e.Message);
                    Environment.Exit(1);
                } else {
                  throw e;
                }
            }
        }

        //-----------------------------------------------------------
        public static void Main(string[] args) {
            new Driver().Run(args);
        }
    }
}
