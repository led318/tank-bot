using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using FormUI.FieldItems;

namespace FormUI.Controls
{
    public class MyPictureBox : PictureBox
    {
        private Color? _myBorderColor;


        public MyPictureBox()
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_myBorderColor.HasValue)
            {
                ControlPaint.DrawBorder(e.Graphics, ClientRectangle, _myBorderColor.Value, ButtonBorderStyle.Solid);
            }

            base.OnPaint(e);
        }

        public void Change(BaseItem item)
        {
            var isChanged = false;

            if (BackgroundImage != item.Image)
            {
                BackgroundImage = item.Image;
                isChanged = true;
            }

            if (_myBorderColor != item.BorderColor)
            {
                _myBorderColor = item.BorderColor;
                isChanged = true;
            }



            if (isChanged)
            {
                Refresh();
            }
        }
    }
}
