using System.Drawing;
using Newtonsoft.Json;

namespace FormUI.FieldItems.Helpers
{
    public class Note
    {
        public string Text { get; set; }
        [JsonIgnore]
        public Brush Color { get; set; }

        public NoteType Type { get; set; }

        public Note()
        {

        }

        public Note(int text, Brush color, NoteType type = NoteType.Other) : this(text.ToString(), color, type)
        {
        }

        public Note(string text, Brush color, NoteType type)
        {
            Text = text;
            Color = color;
            Type = type;
        }
    }

    public enum NoteType
    {
        Other,
        Health,
        ShotCountdown
    }
}
