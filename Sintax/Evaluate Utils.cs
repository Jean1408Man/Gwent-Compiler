namespace LogicalSide;

public static class EvaluateUtils
{
    //Dictionary that will associate the names and the params of the effects that are already checked
    public static Dictionary<string, List<IdentifierExpression>> ParamsRequiered= new Dictionary<string, List<IdentifierExpression>>();
    public static Dictionary<string, EffectDeclarationExpr> Effects= new Dictionary<string, EffectDeclarationExpr>();
    
    private static string? NameFinder(List<Expression> expressions)
    {
        for(int i = 0; i< expressions.Count ; i++)
        {//Its using an effect without params
            if(expressions[i] is BinaryOperator bin)
            {
                if(bin.Left is IdentifierExpression id && (id.ValueAsToken.Type == TokenType.Name|| id.ValueAsToken.Type == TokenType.EFFECTASSIGNMENT))
                {
                    expressions.RemoveAt(i);
                    return (string)bin.Right.Value;
                }
            }
            else
            throw new Exception("Unexpected Code Entrance");
        }
        return null;
    }
    public static EffectDeclarationExpr Finder(List<Expression> expressions)
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
    private static bool InternalFinder(List<IdentifierExpression> Declared, List<Expression> Asked)
    {
        if(Declared.Count!= Asked.Count)
        {
            throw new Exception($"You must declare exactly {Declared.Count} Params at the effect, you declared {Asked.Count}");
        }
        int conta=0;
        foreach(Expression ex in Asked)
        {
            if(ex is BinaryOperator bin)
            {
                foreach(IdentifierExpression id in Declared)
                {
                    if(bin.Left.Equals(id))
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
        }
        if(conta== Declared.Count)
             return true;
        else
        {
            throw new Exception("The params you declared doesnt coincide with the effect");
        }
        return false;
    } 

}
