using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormUI.FieldItems
{
    public class River : BaseItem
    {
        public override bool CanMove => false;

        public River()
        {
            BorderColor = Color.Blue;
        }
    }
}
