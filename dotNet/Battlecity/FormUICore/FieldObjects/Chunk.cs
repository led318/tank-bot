using System.Collections.Generic;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldItems.Tank;

namespace FormUICore.FieldObjects
{
    public class Chunk
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }
        public List<AiTank> AiTanks { get; set; } = new List<AiTank>();
        public List<Empty> EmptyItems { get; set; } = new List<Empty>();
    }
}
