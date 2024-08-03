using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicalSide
{
    public interface IContext
    {
        List<ICard> Find(Expression exp);
        List<ICard> Deck{get; set;}
        List<ICard> DeckOfPlayer(IPlayer player); 
        List<ICard> GraveYard{get; set;}

        List<ICard> GraveYardOfPlayer(IPlayer player);
        List<ICard> Field{get; set;}
        List<ICard> FieldOfPlayer(IPlayer player);
        List<ICard> Hand{get; set;}

        List<ICard> HandOfPlayer(IPlayer player);
        List<ICard> Board{get; set;}
        List<ICard> TriggerPlayer{get; set;}
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
    }
    public interface IPlayer
    {

    }
    public interface IEffect
    {
        EffectDeclarationExpr effect{get; set;}

        SelectorExpression Selector{get; set;}
    }
    public class MyEffect: IEffect
    {
        public MyEffect(EffectDeclarationExpr eff, SelectorExpression Sel)
        {
            effect = eff;
            Selector = Sel;
        }
        public EffectDeclarationExpr effect{get; set;}

        public SelectorExpression Selector{get; set;}
    }
    

}