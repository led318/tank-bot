using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormUI.FieldItems
{
    public class Bullet : BaseMobile
    {
        public override bool CanShootThrough => false; //todo: maybe true

        public override int Speed => 2;

        public Bullet()
        {
            BorderColor = Color.Red;
        }
    }
}
