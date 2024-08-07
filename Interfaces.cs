using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicalSide
{
    public interface IContext
    {
        bool Turn{get; set;}
        CustomList<ICard> Find(Expression exp);
        
        CustomList<ICard> Deck{get; set;}
        CustomList<ICard> OtherDeck{get; set;}
        CustomList<ICard> DeckOfPlayer(IPlayer player);
        
        
        CustomList<ICard> GraveYard{get; set;}

        
        CustomList<ICard> GraveYardOfPlayer(IPlayer player);
        
        CustomList<ICard> Field{get; set;}
        CustomList<ICard> OtherField{get; set;}
        CustomList<ICard> FieldOfPlayer(IPlayer player);
        
        
        CustomList<ICard> Hand{get; set;}
        CustomList<ICard> OtherHand{get; set;}


        CustomList<ICard> HandOfPlayer(IPlayer player);
        CustomList<ICard> Board{get; set;}
        CustomList<ICard> TriggerPlayer{get; set;}
    }


    public interface ICard
    {
        string Name{get; set;}
        string Type{get; set;}
        int Power{get; set;}
        string Range{get; set;}
        IPlayer Owner{get; set;}
        string Faction{get; set;}
        List<IEffect> Effects{get; set;}
        

          

        void Execute(IContext context)
        {
            foreach(IEffect effect in Effects)
            {
                effect.Execute(context);
            }
        }
    }
    public class MyCard: ICard
    {
        public string Name{get; set;}
        public string Type{get; set;}
        public int Power{get; set;}
        public string Range{get; set;}
        public IPlayer Owner{get; set;}
        public string Faction{get; set;}
        public List<IEffect> Effects{get; set;}
        public override string ToString()
        {
            string result= "";
            result += "Name: " + Name + "\n";
            result += "Type: " + Type + "\n";
            result += "Power: " + Power + "\n";
            result += "Range: " + Range + "\n";
            result += "Owner: " + Owner + "\n";
            result += "Faction: " + Faction + "\n";
            result += "Efectos: \n";
            int conta = 1;
            foreach(IEffect effect in Effects)
            {
                Console.WriteLine($"{conta++}- "+ effect);
            }
            return result;
        }
    }
    public interface IPlayer
    {
        bool Turn{get; set;}
    }
    public class Player: IPlayer
    {
        public bool Turn{get; set;}
    }
    
    public interface IEffect
    {
        EffectDeclarationExpr effect{get; set;}
        List<IdentifierExpression> Params{get; set;}
        SelectorExpression Selector{get; set;}

        void Execute(IContext context)
        {
            CustomList<ICard> targets= Selector.Execute(context);
            effect.Execute(context, targets, Params);
        }
    }
    public class MyEffect: IEffect
    {
        public MyEffect(EffectDeclarationExpr eff, SelectorExpression Sel, List<IdentifierExpression> Par)
        {
            effect = eff;
            Selector = Sel;
            Params= Par;
        }
        public List<IdentifierExpression> Params{get; set;}

        public EffectDeclarationExpr effect{get; set;}

        public SelectorExpression Selector{get; set;}
        public override string ToString()
        {
            string s= "Efecto: " + (string)effect.Name.Value+ "\n";
            foreach(IdentifierExpression identifier in Params)
            {
                s+= identifier.ValueAsToken.Value+ "\n";
            }
            return s;
        }
    }
    


}