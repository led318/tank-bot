using System.Collections.Generic;
using System.Linq;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldItems.Helpers;
using FormUI.Infrastructure;

namespace FormUI.FieldObjects
{
    public class Cell
    {
        public List<BaseItem> Items = new List<BaseItem>();
        public Point Point { get; set; }

        public bool IsBorderBattleWall => Point.X == 0 || Point.X == Constants.FieldWidth - 1 ||
                                          Point.Y == 0 || Point.Y == Constants.FieldHeight - 1;

        public List<Note> Notes => Items.SelectMany(x => x.Notes).ToList();

        public Cell(int i, int j)
        {
            Point = new Point(i, j);
        }
    }
}
