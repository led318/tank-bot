using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using FormUICore.FieldObjects;
using FormUICore.Infrastructure;
using Newtonsoft.Json;

namespace FormUICore.Controls
{
    public class MyPictureBox : PictureBox
    {
        private Color? _myBorderColor;

        private readonly Cell _cell;

        private int _noteXStart = -1;
        private int _noteXStep = 8;

        private Font _font;

        private ToolTip _toolTip;

        public MyPictureBox(Cell cell)
        {
            _cell = cell;

            if (_cell.IsBorderBattleWall)
                Visible = false;

            MouseHover += MyPictureBox_MouseHover;
            MouseClick += MyPictureBox_MouseClick;
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

        private void MyPictureBox_MouseHover(object sender, EventArgs e)
        {
            _toolTip ??= new ToolTip();

            _toolTip.SetToolTip(this, _cell.Point.ToString());
        }

        private void MyPictureBox_MouseClick(object sender, EventArgs e)
        {
            var cellJson = JsonConvert.SerializeObject(_cell);

            Logger.SetText(cellJson);
        }
    }
}
