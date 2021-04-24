using System.Drawing;
using API.Components;
using FormUI.Infrastructure;
using Point = API.Components.Point;

namespace FormUI.FieldItems.Prize
{
    public class BasePrize : BaseItem
    {
        public override bool CanShootThrough => true;

        public int Duration { get; set; }

        public BasePrize(Element element, Point point) : base(element, point)
        {
            if (AppSettings.DrawBaseBorders)
                BorderColor = Color.Orange;
        }
    }
}
