 namespace Compiler;

 public abstract class Expression
 {
    public string? printed;
    public virtual void Print(int indentLevel = 0)
    {
        Console.WriteLine(new string(' ', indentLevel * 4) + printed);
    }
 }
public class Assignment: Expression{
    public Expression left;
    public Expression right;
    public Assignment(Expression left, Expression right)
    {
        this.left = left;
        this.right = right;
    }
    public override string ToString()
    {
        return "Hola";
    }
}
 #region Binary Operator
public class BinaryOperator : Expression
{
    public Expression Left { get; set; }
    public Expression Right { get; set; }
    public TokenType Operator { get; set; }

    public BinaryOperator(Expression left, Expression right, TokenType Op)
    {
        Left = left;
        Right = right;
        Operator = Op;
        this.printed = Op.ToString();
    }
}
public class Terminal: Expression
{
    public string? ValueForPrint;
    public Token Value { get; }
    public Terminal(Token token)
    {
        this.ValueForPrint = token.Value;
        Value= token;
    }
}

public class UnaryOperator : Expression
{
    public Expression Operand { get; set; }
    public TokenType Operator { get; set; }

    public UnaryOperator(Expression operand, TokenType Op)
    {
        Operand = operand;
        Operator = Op;
    }
}
public class Number: Terminal
{
    public Number(Token token): base(token)
    {
        this.printed= "Number";
    }
}
public class BooleanLiteral : Terminal
{
    

    public BooleanLiteral(Token token): base(token)
    {
        this.printed = "Boolean";
    }
}

 #endregion

public class IdentifierExpression : Terminal
{
    public IdentifierExpression(Token token):base(token)
    {
        this.printed = "ID"; // O alguna otra forma de representar el identificador visualmente
    }
}

