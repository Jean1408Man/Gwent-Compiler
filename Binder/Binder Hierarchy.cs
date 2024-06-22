// namespace Compiler;

// public abstract class BoundNode
// {
//     public abstract BoundKind Kind{get;}
// }
// public abstract class BoundExpression: BoundNode
// {
//     public abstract Type Type{get;}
// }
// public class BoundLiteralExpressions: BoundExpression
// {
//     public BoundLiteralExpressions(object value){
//         Value = value; 
//     }
//     public override BoundKind Kind => BoundKind.LiteralExpressions;
//     public override Type Type => Value.GetType();
//     public object Value {get;}
// }
// public class BoundUnaryExpression: BoundExpression
// {
//     public BoundUnaryExpression(BoundUnaryOperatorKind operatorKind, BoundExpression operand){
//         OperatorKind = operatorKind;
//         Operand = operand;
//     }
//     public override BoundKind Kind => BoundKind.UnaryExpressions;
//     public override Type Type => Operand.GetType();
//     public BoundUnaryOperatorKind OperatorKind {get;}
//     public BoundExpression Operand {get;}
// }
// internal sealed class Boundinaryexpression : Boundexpression
// {
// public BoundBinaryExpression(BoundExpression left, BoundBinaryOperatorKind operatorKind, BoundExpression right){
// Left = left;
// OperatorKind = operatorKind;
// Right = right
// }

// public override BoundKind Kind => BoundKind.UnaryExpressions;
// public override Type Type => Left. Type;
// public BoundExpression Left { get; }
// public BoundBinaryOperatorKind OperatorKind( get;)
// public BoundExpression Right {get;}

// }




// #region Enums
// public enum BoundUnaryOperatorKind
// {
//     Negation
// }
// public enum BoundKind
// {
//     LiteralExpressions,
//     UnaryExpressions,
//     BinaryExpressions
// };
// #endregion
