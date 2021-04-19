using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Components;

namespace Solver.FieldItems
{
    public class BaseItem
    {
        public string Symbol { get; set; }
        public Element Element { get; set; }

        public string Sprite => $"{Element}.png";
    }
}
