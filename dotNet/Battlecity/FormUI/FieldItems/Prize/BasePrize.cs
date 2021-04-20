using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormUI.FieldItems.Prize
{
    public class BasePrize : BaseItem
    {
        public int Duration { get; set; }


        public BasePrize()
        {
            BorderColor = Color.Orange;
        }
    }
}
