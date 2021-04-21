using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Components;
using FormUI.Infrastructure;
using Point = API.Components.Point;

namespace FormUI.FieldItems.Prize
{
    public class BasePrize : BaseItem
    {
        public override bool CanShootThrough => false;

        public int Duration { get; set; }

        public BasePrize(Element element, Point point) : base(element, point)
        {
            if (AppSettings.DrawBaseBorders)
                BorderColor = Color.Orange;
        }
    }
}
