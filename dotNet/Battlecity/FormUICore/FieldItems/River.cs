using System.Drawing;
using API.Components;
using FormUI.Infrastructure;
using FormUICore.FieldItems;

namespace FormUI.FieldItems
{
    public class River : BaseItem
    {
        public override bool CanMove => false;

        public River(Element element, API.Components.Point point) : base(element, point)
        {
            if (AppSettings.DrawBaseBorders)
                BorderColor = Color.Blue;
        }

    }
}
