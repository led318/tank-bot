using System.Collections.Generic;
using System.Drawing;
using API.Components;
using FormUI.FieldItems.Helpers;
using FormUI.Infrastructure;
using FormUICore.Infrastructure;
using Point = API.Components.Point;

namespace FormUI.FieldItems.Tank
{
    public class EnemyTank : BaseTank
    {
        public int HiddenRoundsInRow { get; set; }
        public bool IsShoting { get; set; }

        public int StuckIndex { get; set; }

        public override int ShotCountdownDefault => ServerSettings.Settings.TankTicksPerShoot;

        private static readonly HashSet<Element> _enemyTankElements = new HashSet<Element>
        {
            Element.OTHER_TANK_DOWN,
            Element.OTHER_TANK_LEFT,
            Element.OTHER_TANK_RIGHT,
            Element.OTHER_TANK_UP
        };

        public EnemyTank(Element element, Point point) : base(element, point)
        {
            if (AppSettings.DrawBaseBorders)
                BorderColor = Color.Green;
        }

        protected override void InitIsStuck()
        {
            if (!State.HasPrevRound)
                return;

            var prevStepItem = State.PrevRound.Items[Point.X, Point.Y];

            if (prevStepItem.Element == Element)
            {
                var prevEnemyTank = (EnemyTank) prevStepItem;
                StuckIndex = prevEnemyTank.StuckIndex + 1;

                if (StuckIndex > 1)
                {
                    IsStuck = true;
                    AddNote("S", Brushes.GreenYellow, NoteType.Other);
                }
            }
        }

        protected override void SetDirection()
        {
            Direction direction;

            switch (Element)
            {
                case Element.OTHER_TANK_DOWN:
                    direction = API.Components.Direction.Down;
                    break;
                case Element.OTHER_TANK_UP:
                    direction = API.Components.Direction.Up;
                    break;
                case Element.OTHER_TANK_LEFT:
                    direction = API.Components.Direction.Left;
                    break;
                case Element.OTHER_TANK_RIGHT:
                    direction = API.Components.Direction.Right;
                    break;

                default:
                    direction = API.Components.Direction.Down;
                    break;
            }

            Direction = direction;
        }

        public void UpdateElementByDirection()
        {
            switch (Direction)
            {
                case API.Components.Direction.Down:
                    Element = Element.OTHER_TANK_DOWN;
                    break;
                case API.Components.Direction.Up:
                    Element = Element.OTHER_TANK_UP;
                    break;
                case API.Components.Direction.Left:
                    Element = Element.OTHER_TANK_LEFT;
                    break;
                case API.Components.Direction.Right:
                    Element = Element.OTHER_TANK_RIGHT;
                    break;
            }
        }

        public override void Shot()
        {
            base.Shot();
            //IsStuck = false;
            IsShoting = true;
            HiddenRoundsInRow = 0;
        }

        public static bool IsEnemyTank(Element element)
        {
            return _enemyTankElements.Contains(element);
        }
    }
}
