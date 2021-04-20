using System;
using System.Drawing;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldItems.Prize;
using FormUI.FieldItems.Tank;
using FormUI.FieldItems.Wall;
using Point = API.Components.Point;

namespace FormUI.Infrastructure
{
    public static class ItemFactory
    {
        private static readonly Random _random = new Random();

        public static BaseItem GetItem(Element element, Point point)
        {
            var item = GetBaseItem(element);
            item.Element = element;
            item.Point = point;

            /*
            if (_random.Next(100) > 90)
            {
                item.BorderColor = Color.Red;
            }
            */

            return item;
        }

        private static BaseItem GetBaseItem(Element element)
        {
            switch (element)
            {
                case Element.NONE:
                    return new Empty();

                case Element.AI_TANK_DOWN:
                case Element.AI_TANK_LEFT:
                case Element.AI_TANK_RIGHT:
                case Element.AI_TANK_UP:
                    return new AiTank();

                case Element.AI_TANK_PRIZE:
                    return new AiPrizeTank();

                case Element.PRIZE:
                case Element.PRIZE_IMMORTALITY:
                case Element.PRIZE_BREAKING_WALLS:
                case Element.PRIZE_WALKING_ON_WATER:
                case Element.PRIZE_VISIBILITY:
                case Element.PRIZE_NO_SLIDING:
                    return new BasePrize();

                case Element.OTHER_TANK_DOWN:
                case Element.OTHER_TANK_LEFT:
                case Element.OTHER_TANK_RIGHT:
                case Element.OTHER_TANK_UP:
                    return new EnemyTank();

                case Element.TANK_DOWN:
                case Element.TANK_LEFT:
                case Element.TANK_RIGHT:
                case Element.TANK_UP:
                    return new MyTank();

                case Element.BULLET:
                    return new Bullet();
                case Element.BANG:
                    return new Blast();

                case Element.RIVER:
                    return new River();
                case Element.TREE:
                    return new Tree();
                case Element.ICE:
                    return new Ice();


                case Element.WALL:
                case Element.WALL_DESTROYED_DOWN:
                case Element.WALL_DESTROYED_DOWN_LEFT:
                case Element.WALL_DESTROYED_DOWN_RIGHT:
                case Element.WALL_DESTROYED_DOWN_TWICE:
                case Element.WALL_DESTROYED_LEFT:
                case Element.WALL_DESTROYED_LEFT_RIGHT:
                case Element.WALL_DESTROYED_LEFT_TWICE:
                case Element.WALL_DESTROYED_RIGHT:
                case Element.WALL_DESTROYED_RIGHT_TWICE:
                case Element.WALL_DESTROYED_RIGHT_UP:
                case Element.WALL_DESTROYED_UP:
                case Element.WALL_DESTROYED_UP_DOWN:
                case Element.WALL_DESTROYED_UP_LEFT:
                    return new BaseWall();

                case Element.BATTLE_WALL:
                    return new BattleWall();
            }


            return new Empty();
        }
    }
}
