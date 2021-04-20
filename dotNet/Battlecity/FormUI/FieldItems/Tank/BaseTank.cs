using System.Collections.Generic;
using System.Linq;

using FormUI.Infrastructure;

namespace FormUI.FieldItems.Tank
{
    public abstract class BaseTank : BaseMobile
    {
        public override bool CanShootThrough => false;

        public override int Speed => 1;

        public virtual int Health { get; set; } = 1;

        public abstract int ShotCountdown { get; }
        public int ShotCountdownLeft { get; set; } = 0;


        

        
    }
}
