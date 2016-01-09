using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoveoBlitz;

namespace Coveo.Bot
{
    public class PathCoord
    {
        public Pos current;
        public Pos previous;

        public int heuristic;
        public int weight;
    }
}
