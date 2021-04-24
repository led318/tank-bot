using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FormUI.FieldObjects;

namespace FormUI.Controls
{
    public class MyPictureBox : PictureBox
    {
        private Color? _myBorderColor;

        private readonly Cell _cell;

        private int _noteXStart = -1;
        private int _noteXStep = 8;

        private Font _font;

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
                _font = new Font(Font, FontStyle.Bold);

                var x = _noteXStart;

                foreach (var note in _cell.Notes)
                {
                    
                    e.Graphics.DrawString(note.Text, _font, note.Color, x, 0);
                    x += _noteXStep;
                }
            }

            base.OnPaint(e);
        }

        public void Change()
        {
            var isChanged = _cell.IsDirty;

            if (BackgroundImage != _cell.Item.Image)
            {
                BackgroundImage = _cell.Item.Image;
                isChanged = true;
            }

            if (_myBorderColor != _cell.BorderColor)
            {
                _myBorderColor = _cell.BorderColor;
                isChanged = true;
            }

            if (isChanged)
            {
                Refresh();
            }
        }
    }
}
