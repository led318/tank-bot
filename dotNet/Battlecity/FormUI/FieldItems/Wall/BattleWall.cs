using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormUI.FieldItems.Wall
{
    public class BattleWall : BaseWall
    {
        public override bool IsDestroyable => false;

        public BattleWall()
        {
            BorderColor = Color.White;
        }
    }
}
