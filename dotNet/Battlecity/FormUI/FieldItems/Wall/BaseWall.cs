using System.Drawing;

namespace FormUI.FieldItems.Wall
{
    public class BaseWall : BaseItem
    {
        public override bool CanShootThrough => false;
        public override bool CanMove => false;

        public virtual bool IsDestroyable => true;

        public BaseWall()
        {
            BorderColor = Color.Brown;
        }
    }
}
