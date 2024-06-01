// namespace Compiler;

// public abstract class Expression
// {
//     public abstract bool IsValid();
//     public abstract void Evaluate();
// }

// #region Binary Operator
// public abstract class BinaryOperator : Expression
// {
//     public Expression? Left;
//     public Expression? Right;
//     public double result;
//     public BinaryOperator(Expression? left, Expression? right)
//     {
//         Left = left;
//         Right = right;
//     }

//     public override bool IsValid()
//     {
//         if(Left.IsValid() && Right.IsValid)
        
//     }
// }

// public class Plus : BinaryOperator
// {
//     public Plus(Expression? left, Expression? right) : base(left, right) { }

//     public override void Evaluate()
//     {
//         // TO DO: implement the evaluation logic for the Plus operator
//         // For example, you could add the values of the left and right expressions
//         // and store the result in a temporary variable or return it
//     }
// }

// public class Minus : BinaryOperator
// {
//     public Minus(Expression? left, Expression? right) : base(left, right) { }

//     public override void Evaluate()
//     {
//         // TO DO: implement the evaluation logic for the Minus operator
//     }
// }

// public class Multiply : BinaryOperator
// {
//     public Multiply(Expression? left, Expression? right) : base(left, right) { }

//     public override void Evaluate()
//     {
//         // TO DO: implement the evaluation logic for the Multiply operator
//     }
// }

// public class Divide : BinaryOperator
// {
//     public Divide(Expression? left, Expression? right) : base(left, right) { }

//     public override void Evaluate()
//     {
//         // TO DO: implement the evaluation logic for the Divide operator
//     }
// }


// #endregion