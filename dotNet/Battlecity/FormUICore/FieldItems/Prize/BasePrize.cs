using System.Collections.Generic;
using System.Drawing;
using API.Components;
using FormUI.Infrastructure;
using Point = API.Components.Point;

namespace FormUI.FieldItems.Prize
{
    public class BasePrize : BaseItem
    {
        public override bool CanShootThrough => true;
        public override bool CanMove => true;

        private static readonly HashSet<Element> _prizeElements = new HashSet<Element>
        {
            Element.PRIZE,
            Element.PRIZE_IMMORTALITY,
            Element.PRIZE_BREAKING_WALLS,
            Element.PRIZE_WALKING_ON_WATER,
            Element.PRIZE_VISIBILITY,
            Element.PRIZE_NO_SLIDING
        };

        public int Duration { get; set; }

        public BasePrize(Element element, Point point) : base(element, point)
        {
            if (AppSettings.DrawBaseBorders)
                BorderColor = Color.Orange;
        }

        public static bool IsPrize(Element element)
        {
            return _prizeElements.Contains(element);
        }
    }
}
