
using System.Collections.Generic;

namespace int64 {

	interface INodeVisitor {
		void Visit(Program node);
		IList<Node> Visit(DefList node);
		IList<Identifier> Visit(IdList node);
		void Visit(ParamList node);

		void Visit(FunDef node);
		void Visit(VarDefList node);
		void Visit(StmtList node);
		void Visit(StmtIf node);
		void Visit(ElseIfList node);
		void Visit(ElseIf node);
		void Visit(Else node);

		void Visit(StmtSwitch node);
		void Visit(CaseList node);
		void Visit(Case node);
		void Visit(LitList node);
		void Visit(Default node);

		void Visit(StmtWhile node);
		void Visit(StmtDoWhile node);
		void Visit(StmtFor node);
		void Visit(StmtBreak node);
		void Visit(StmtContinue node);
		void Visit(StmtReturn node);
		void Visit(StmtEmpty node);

		void Visit(TernaryOperator node);
		void Visit(LogicalOr node);
		void Visit(LogicalAnd node);
		void Visit(Equal node);
		void Visit(NotEqual node);

		void Visit(GreaterThan node);
		void Visit(GreaterEqualThan node);
		void Visit(LessThan node);
		void Visit(LessEqualThan node);

		void Visit(BitwiseOr node);
		void Visit(BitwiseXor node);
		void Visit(BitwiseAnd node);
		void Visit(BitwiseShiftLeft node);
		void Visit(BitwiseShiftRight node);
		void Visit(BitwiseUnsignedShiftRight node);

		void Visit(Plus node);
		void Visit(Minus node);

		void Visit(Times node);
		void Visit(Division node);
		void Visit(Remainder node);

		void Visit(Power node);

		void Visit(BitwiseNot node);
		void Visit(LogicalNot node);

		void Visit(FunCall node);
		IList<Node> Visit(ArrayList node);

		void Visit(True node);
		void Visit(False node);

		void Visit(Identifier node);

		void Visit(IntLiteral node);
		void Visit(CharLiteral node);
		void Visit(StringLiteral node);

		void Visit(Assignment node);
	}
}