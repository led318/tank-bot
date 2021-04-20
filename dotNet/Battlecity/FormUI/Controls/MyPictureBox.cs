using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FormUI.FieldItems;
using FormUI.FieldObjects;

namespace FormUI.Controls
{
    public class MyPictureBox : PictureBox
    {
        private Color? _myBorderColor;

        private Cell _cell;

        private int _noteXStart = -1;
        private int _noteXStep = 8;

        public MyPictureBox(Cell cell)
        {
            _cell = cell;

            if (_cell.IsBorderBattleWall)
                Visible = false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_myBorderColor.HasValue)
            {
                ControlPaint.DrawBorder(e.Graphics, ClientRectangle, _myBorderColor.Value, ButtonBorderStyle.Solid);
            }

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            if (_cell.Notes.Any())
            {
                var x = _noteXStart;

                foreach (var note in _cell.Notes)
                {
                    e.Graphics.DrawString(note.Text, Font, note.Color, x, 0);
                    x += _noteXStep;
                }
            }

            base.OnPaint(e);
        }

        public void Change()
        {
            var isChanged = false;

            var item = _cell.Items.First();

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
