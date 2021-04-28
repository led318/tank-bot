using System.Collections.Generic;
using System.Drawing;
using API.Components;
using FormUI.Infrastructure;
using FormUICore.FieldItems;
using FormUICore.Infrastructure;
using Point = API.Components.Point;

namespace FormUI.FieldItems.Wall
{
    public class BaseWall : BaseItem
    {
        public override bool CanShootThrough => false;
        public override bool CanMove => false;

        public virtual bool IsDestroyable => true;

        public int Durability => DurabilityDictionary.ContainsKey(Element) ? DurabilityDictionary[Element] : 1000;

        public BaseWall(Element element, Point point) : base(element, point)
        {
            if (AppSettings.DrawBaseBorders)
                BorderColor = Color.Brown;
        }

        public static IDictionary<Element, int> DurabilityDictionary { get; } = new Dictionary<Element, int>
        {
            {Element.WALL, 3},
            {Element.WALL_DESTROYED_DOWN, 2},
            {Element.WALL_DESTROYED_UP, 2},
            {Element.WALL_DESTROYED_LEFT, 2},
            {Element.WALL_DESTROYED_RIGHT, 2},
            {Element.WALL_DESTROYED_DOWN_TWICE, 1},
            {Element.WALL_DESTROYED_UP_TWICE, 1},
            {Element.WALL_DESTROYED_LEFT_TWICE, 1},
            {Element.WALL_DESTROYED_RIGHT_TWICE, 1},
            {Element.WALL_DESTROYED_LEFT_RIGHT, 1},
            {Element.WALL_DESTROYED_UP_DOWN, 1},
            {Element.WALL_DESTROYED_UP_LEFT, 1},
            {Element.WALL_DESTROYED_RIGHT_UP, 1},
            {Element.WALL_DESTROYED_DOWN_LEFT, 1},
            {Element.WALL_DESTROYED_DOWN_RIGHT, 1}
        };
    }
}
