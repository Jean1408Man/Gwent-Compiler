 namespace Compiler;

 public abstract class Expression
 {
     
 }
public class Equal: Expression{
    public Expression left;
    public Expression right;
    public Equal(Expression left, Expression right)
    {
        this.left = left;
        this.right = right;
    }
}
 #region Binary Operator
 public abstract class BinaryOperator : Expression
 {
     public Expression? Left;
     public Expression? Right;
     public double result;

     public BinaryOperator(Expression? left, Expression? right)
     {
         Left = left;
         Right = right;
     }
 }

 public class Plus : BinaryOperator
 {
     public Plus(Expression? left, Expression? right) : base(left, right) { }
 }
     
 public class Minus : BinaryOperator
 {
     public Minus(Expression? left, Expression? right) : base(left, right) { }
 }

 public class Multiply : BinaryOperator
 {
     public Multiply(Expression? left, Expression? right) : base(left, right) { }
 }

 public class Divide : BinaryOperator
 {
    public Divide(Expression? left, Expression? right) : base(left, right) { }
 }
 public class Pow : BinaryOperator
 {
    public Pow(Expression? left, Expression? right) : base(left, right) { }
 }
 public class Number: Expression{
    int value;
 }


 #endregion