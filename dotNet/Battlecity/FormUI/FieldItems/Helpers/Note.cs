using System.Drawing;

namespace FormUI.FieldItems.Helpers
{
    public class Note
    {
        public string Text { get; set; }
        public Brush Color { get; set; }

        public Note()
        {

        }

        public Note(string text, Brush color)
        {
            Text = text;
            Color = color;
        }
    }
}
