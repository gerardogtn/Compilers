/*
  Javier Curiel A01020542
  Gerardo Teruel A01018057
  Angel Tellez A01022029

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

   class Compiler {
      public static void Main(string[] args) {

         if (args.Length != 1) {
            Console.Error.WriteLine(
            "Please specify the name of the input file.");
            Environment.Exit(1);
         }

         try {
            var inputPath = args[0];
            var input = File.ReadAllText(inputPath);

            Console.WriteLine(String.Format(
            "===== Tokens from: \"{0}\" =====", inputPath)
            );
            var count = 1;
            foreach (Token token in new Scanner(input).Start()) {
               Console.WriteLine(String.Format("[{0}] {1}",
               count++, token)
               );
            }

         } catch (FileNotFoundException e) {
            Console.Error.WriteLine(e.Message);
            Environment.Exit(1);
         }
      }
   }
}
