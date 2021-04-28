using System.Drawing;
using API.Components;
using FormUI.Infrastructure;
using FormUICore.FieldItems;

namespace FormUI.FieldItems
{
    public class Tree : BaseItem
    {
        public Tree(Element element, API.Components.Point point) : base(element, point)
        {
            if (AppSettings.DrawBaseBorders)
                BorderColor = Color.DarkGreen;
        }
    }
}
