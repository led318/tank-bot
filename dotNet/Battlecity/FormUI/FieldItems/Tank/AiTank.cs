using System.Drawing;
using API.Components;
using FormUI.FieldItems.Helpers;
using FormUI.Infrastructure;
using Point = API.Components.Point;

namespace FormUI.FieldItems.Tank
{
    public class AiTank : BaseTank
    {
        public override int ShotCountdown => Settings.Get.AiTicksPerShoot;

        public AiTank(Element element, Point point) : base(element, point)
        {
            if (AppSettings.DrawBaseBorders)
                BorderColor = Color.DarkGray;

            //AddNote("0", Brushes.DarkGreen);
            //AddNote("1", Brushes.Red);
        }

        protected override void SetDirection()
        {
            Direction direction;

            switch (Element)
            {
                case Element.AI_TANK_DOWN:
                    direction = Direction.Down;
                    break;
                case Element.AI_TANK_UP:
                    direction = Direction.Up;
                    break;
                case Element.AI_TANK_LEFT:
                    direction = Direction.Left;
                    break;
                case Element.AI_TANK_RIGHT:
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
