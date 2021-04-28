using API.Components;
using FormUI.FieldItems.Prize;
using FormUI.FieldItems.Tank;
using FormUI.FieldItems.Wall;
using FormUICore.FieldItems;
using FormUICore.FieldItems.Tank;

namespace FormUI.FieldItems
{
    public static class ItemFactory
    {
        public static BaseItem GetItem(Element element, Point point)
        {
            var item = GetBaseItem(element, point);

            return item;
        }

        private static BaseItem GetBaseItem(Element element, Point point)
        {
            switch (element)
            {
                case Element.NONE:
                    return new Empty(element, point);

                case Element.AI_TANK_DOWN:
                case Element.AI_TANK_LEFT:
                case Element.AI_TANK_RIGHT:
                case Element.AI_TANK_UP:
                    return new AiTank(element, point);

                case Element.AI_TANK_PRIZE:
                    return new AiPrizeTank(element, point);

                case Element.PRIZE:
                case Element.PRIZE_IMMORTALITY:
                case Element.PRIZE_BREAKING_WALLS:
                case Element.PRIZE_WALKING_ON_WATER:
                case Element.PRIZE_VISIBILITY:
                case Element.PRIZE_NO_SLIDING:
                    return new BasePrize(element, point);

                case Element.OTHER_TANK_DOWN:
                case Element.OTHER_TANK_LEFT:
                case Element.OTHER_TANK_RIGHT:
                case Element.OTHER_TANK_UP:
                    return new EnemyTank(element, point);

                case Element.TANK_DOWN:
                case Element.TANK_LEFT:
                case Element.TANK_RIGHT:
                case Element.TANK_UP:
                    return new MyTank(element, point);

                case Element.BULLET:
                    return new Bullet(element, point);
                case Element.BANG:
                    return new Blast(element, point);

                case Element.RIVER:
                    return new River(element, point);
                case Element.TREE:
                    return new Tree(element, point);
                case Element.ICE:
                    return new Ice(element, point);


                case Element.WALL:
                //case Element.WALL_DESTROYED:
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
                case Element.WALL_DESTROYED_UP_TWICE:
                case Element.WALL_DESTROYED_UP_DOWN:
                case Element.WALL_DESTROYED_UP_LEFT:
                    return new BaseWall(element, point);

                case Element.BATTLE_WALL:
                    return new BattleWall(element, point);
            }


            return new Empty(element, point);
        }
    }
}
