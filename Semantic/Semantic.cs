using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compiler
{
    public class Semantic
    {
        public Semantic(Expression root)
        {
            root.Semantic(null);
        }
    }
}