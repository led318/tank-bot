using System.Drawing;
using API.Components;
using FormUI.Infrastructure;
using Point = API.Components.Point;

namespace FormUI.FieldItems.Wall
{
    public class BaseWall : BaseItem
    {
        public override bool CanShootThrough => false;
        public override bool CanMove => false;

        public virtual bool IsDestroyable => true;

        public BaseWall(Element element, Point point) : base(element, point)
        {
            if (AppSettings.DrawBaseBorders)
                BorderColor = Color.Brown;
        }
    }
}
