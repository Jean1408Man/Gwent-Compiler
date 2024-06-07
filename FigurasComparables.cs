using System.Formats.Asn1;

namespace Compiler;

public abstract class Figuras: IComparable<Figuras>
{
    public int CompareTo(Figuras? figura)
    {
        if(figura!.Area()> Area())
        {
            return 1;
        }
        if(figura.Area()== Area())
            return 0;
        return -1;
    }
    public abstract double Area();
}
public class Rectangulo: Figuras
{
    public double b;
    public double a;
    public override double Area()
    {
        return a*b;
    }
}
