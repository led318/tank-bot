using System;
using System.Drawing;
using API.Components;
using FormUI.Infrastructure;
using Point = API.Components.Point;

namespace FormUI.FieldItems
{
    public class BaseItem
    {
        public Point Point { get; set; }
        public Element Element { get; set; }

        public string Sprite => $"./Sprites/{Element}.png";

        public Image Image =>  ItemImageProvider.GetItemImage(this);
    }
}
