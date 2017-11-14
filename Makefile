# int64 compiler, version 0.4
# Copyright \u00A9 2017 by G. Teruel, J. Curiel. & A. Téllez
#
# This program is free software.
# You may redistribute it under the terms of
# the GNU General Public License version 3 or later.
# This program has absolutely no warranty.

PHASE1 = Token.cs TokenCategory.cs Scanner.cs
PHASE2 = Parser.cs SyntaxError.cs
PHASE3 = Node.cs SpecificNodes.cs
PHASE4 = SemanticAnalyzer.cs SymbolTable.cs SemanticError.cs
PHASE5 = CILGenerator.cs int64lib.cs

make: clean int64

phase1: Lexical.cs $(PHASE1)
	mcs -out:int64_lexical $?

phase2: Syntactic.cs $(PHASE1) $(PHASE2)
	mcs -out:int64_syntactic $?

phase3: AST.cs $(PHASE1) $(PHASE2) $(PHASE3)
	mcs -out:int64_ast $?

phase4: Semantic.cs $(PHASE1) $(PHASE2) $(PHASE3) $(PHASE4)
	mcs -out:int64_semantic $?

int64: Driver.cs $(PHASE1) $(PHASE2) $(PHASE3) $(PHASE4) #$(PHASE5)
	@mcs -nowarn:414 -out:int64 $?
	Compiling...

clean:
	@rm -f int64_lexical int64_syntactic int64_ast int64_semantic int64
