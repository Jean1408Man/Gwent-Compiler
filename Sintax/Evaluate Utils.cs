namespace LogicalSide;

public static class EvaluateUtils
{
    //Dictionary that will associate the names and the params of the effects that are already checked
    public static Dictionary<string, List<IdentifierExpression>> ParamsRequiered= new Dictionary<string, List<IdentifierExpression>>();
    public static Dictionary<string, EffectDeclarationExpr> Effects= new Dictionary<string, EffectDeclarationExpr>();
    
    private static string? NameFinder(List<IdentifierExpression> expressions)
    {
        for(int i = 0; i< expressions.Count ; i++)
        {//Its using an effect without params
            if(expressions[i] is IdentifierExpression id && (id.ValueAsToken.Type == TokenType.Name|| id.ValueAsToken.Type == TokenType.EFFECTASSIGNMENT))
            {
                expressions.RemoveAt(i);
                return (string)id.Value;
            }
        }
        return null;
    }
    public static EffectDeclarationExpr Finder(List<IdentifierExpression> expressions)
    {
        string? name= NameFinder(expressions);
        if(name== null)
            throw new Exception("Evaluate Error, There is not name given for the Effect of the Card");
        
        if(!Effects.ContainsKey(name))
            throw new Exception($"Evaluate Error, there is not effect named {name} declared previusly");
        if(InternalFinder(ParamsRequiered[name], expressions))
        {
            return Effects[name];
        }
        else
        {
            throw new Exception("Unexpected code Entrance");
        }
    }
    public static void SetUpParams(List<IdentifierExpression> Values, List<Expression> Params)
    {
        foreach(Expression ex in Params)
        {
            if(ex is BinaryOperator bin)
            {
                if(bin.Left is IdentifierExpression ide)
                foreach(IdentifierExpression id in Values)
                {
                    if(ide.ValueAsToken.Value== id.ValueAsToken.Value)
                    {
                        ide.Value= id.Value;
                        bin.Value= id.Value;
                        break;
                    }
                }
            }
        }
    } 
    private static bool InternalFinder(List<IdentifierExpression> Declared, List<IdentifierExpression> Asked)
    {
        if(Declared.Count!= Asked.Count)
        {
            throw new Exception($"You must declare exactly {Declared.Count} Params at the effect, you declared {Asked.Count}");
        }
        int conta=0;
        foreach(IdentifierExpression bin in Asked)
        {
            
                foreach(IdentifierExpression id in Declared)
                {
                    if(bin.Equals(id))
                    {//Una variable coincide en nombre
                        if(bin.Type== id.Type)
                        {
                            conta++;
                        }
                        else
                        {
                            throw new Exception("Evaluate Error, the Params must coincide in type with the ones declarated");
                        }
                    }
                }
            
        }
        if(conta== Declared.Count)
             return true;
        else
        {
            throw new Exception("The params you declared doesnt coincide with the effect");
        }
    } 
    public static void ActualizeScope(Expression expression, EvaluateScope scope)
    {
        if(scope!= null)
        if (expression is IdentifierExpression ide && expression.Type!= ValueType.Card && expression.Type!= ValueType.Context && expression.Type!= ValueType.ListCard)
        {
            scope.AddVar(ide, ide.Value);
        }
    }
}
