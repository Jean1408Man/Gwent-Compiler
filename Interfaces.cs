using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicalSide
{
    public interface IContext
    {
        bool Turn{get; set;}
        List<ICard> Find(Expression exp);
        List<ICard> Deck{get; set;}
        List<ICard> OtherDeck{get; set;}
        List<ICard> DeckOfPlayer(IPlayer player);
        List<ICard> GraveYard{get; set;}
        
        List<ICard> GraveYardOfPlayer(IPlayer player);
        
        List<ICard> Field{get; set;}
        List<ICard> OtherField{get; set;}

        List<ICard> FieldOfPlayer(IPlayer player);
        List<ICard> Hand{get; set;}
        List<ICard> OtherHand{get; set;}


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
    }
    public interface IPlayer
    {

    }
    public interface IEffect
    {
        EffectDeclarationExpr effect{get; set;}
        List<IdentifierExpression> Params{get; set;}
        SelectorExpression Selector{get; set;}

        void Execute(IContext context)
        {
            List<ICard> targets= Selector.Execute(context);
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
    }
    

}