using System.Configuration;
using System.Drawing;
using API.Components;
using FormUI.Infrastructure;
using Point = API.Components.Point;

namespace FormUI.FieldItems.Tank
{
    public class MyTank : BaseTank
    {
        public override int ShotCountdownDefault => Settings.Get.TankTicksPerShoot;

        public MyTank(Element element, Point point) : base(element, point)
        {
            if (AppSettings.DrawBaseBorders)
                BorderColor = Color.Yellow;
        }

        public void UpdateElementByDirection()
        {
            switch (Direction)
            {
                case API.Components.Direction.Down:
                    Element = Element.TANK_DOWN;
                    break;
                case API.Components.Direction.Up:
                    Element = Element.TANK_UP;
                    break;
                case API.Components.Direction.Left:
                    Element = Element.TANK_LEFT;
                    break;
                case API.Components.Direction.Right:
                    Element = Element.TANK_RIGHT;
                    break;
            }
        }

        protected override void SetDirection()
        {
            Direction direction;

            switch (Element)
            {
                case Element.TANK_DOWN:
                    direction = API.Components.Direction.Down;
                    break;
                case Element.TANK_UP:
                    direction = API.Components.Direction.Up;
                    break;
                case Element.TANK_LEFT:
                    direction = API.Components.Direction.Left;
                    break;
                case Element.TANK_RIGHT:
                    direction = API.Components.Direction.Right;
                    break;

                default:
                    direction = API.Components.Direction.Down;
                    break;
            }

            Direction = direction;
        }
    }
}
