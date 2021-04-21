using System.Drawing;
using API.Components;
using FormUI.Infrastructure;
using Point = API.Components.Point;

namespace FormUI.FieldItems.Tank
{
    public class EnemyTank : BaseTank
    {
        public override int ShotCountdownDefault => Settings.Get.TankTicksPerShoot;

        public EnemyTank(Element element, Point point) : base(element, point)
        {
            if (AppSettings.DrawBaseBorders)
                BorderColor = Color.Green;
        }

        protected override void SetDirection()
        {
            Direction direction;

            switch (Element)
            {
                case Element.OTHER_TANK_DOWN:
                    direction = Direction.Down;
                    break;
                case Element.OTHER_TANK_UP:
                    direction = Direction.Up;
                    break;
                case Element.OTHER_TANK_LEFT:
                    direction = Direction.Left;
                    break;
                case Element.OTHER_TANK_RIGHT:
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
