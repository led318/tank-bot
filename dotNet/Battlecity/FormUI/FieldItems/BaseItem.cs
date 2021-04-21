using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using API.Components;
using FormUI.FieldItems.Helpers;
using FormUI.Infrastructure;
using Point = API.Components.Point;

namespace FormUI.FieldItems
{
    public abstract class BaseItem
    {
        public Point Point { get; set; }
        public Element Element { get; set; }

        public List<Note> Notes { get; set; } = new List<Note>();

        public string Sprite => $"./Sprites/{Element}.png";

        public Image Image =>  ItemImageProvider.GetItemImage(this);
        public Color? BorderColor { get; set; } = null;

        public virtual bool CanShootThrough => true;
        public virtual bool CanMove => true;

        protected BaseItem(Element element, Point point)
        {
            Element = element;
            Point = point;
        }

        public virtual void ProcessRound()
        {
            
        }
        
        public void AddNote<T>(T text, Brush color, NoteType type = NoteType.Other)
        {
            if (type != NoteType.Other)
            {
                Notes = Notes.Where(n => n.Type != type).ToList();
            }

            Notes.Add(new Note(text.ToString(), color, type));
        }

        public virtual void Tick()
        {

        }
    }
}
