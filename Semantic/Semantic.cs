using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogicalSide
{
    public class Semantic
    {
        public Semantic(Expression root)
        {
            SintaxFacts.CompilerPhase= "Semantic";
            root.Semantic(null!);
        }
    }
}