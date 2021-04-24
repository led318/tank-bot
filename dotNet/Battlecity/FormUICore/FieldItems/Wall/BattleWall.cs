using System.Drawing;
using API.Components;
using FormUI.Infrastructure;
using Point = API.Components.Point;

namespace FormUI.FieldItems.Wall
{
    public class BattleWall : BaseWall
    {
        public override bool IsDestroyable => false;

        public BattleWall(Element element, Point point) : base(element, point)
        {
            if (AppSettings.DrawBaseBorders)
                BorderColor = Color.White;
        }
        
    }
}
