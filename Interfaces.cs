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
    }
    public interface IPlayer
    {

    }
}