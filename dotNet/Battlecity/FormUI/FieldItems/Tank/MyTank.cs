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

        protected override void SetDirection()
        {
            Direction direction;

            switch (Element)
            {
                case Element.TANK_DOWN:
                    direction = Direction.Down;
                    break;
                case Element.TANK_UP:
                    direction = Direction.Up;
                    break;
                case Element.TANK_LEFT:
                    direction = Direction.Left;
                    break;
                case Element.TANK_RIGHT:
                    direction = Direction.Right;
                    break;

                default:
                    direction = Direction.Down;
                    break;
            }

            CurrentDirection = direction;
        }
    }
}
