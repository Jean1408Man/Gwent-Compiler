namespace LogicalSide;

public class Evaluator
{
    public Evaluator(Expression expression)
    {
        root = expression;
    }
    Expression root;
    public object Evaluate()
    {
        SintaxFacts.CompilerPhase= "Evaluate";
        return root.Evaluate(null,null);
    }
}
