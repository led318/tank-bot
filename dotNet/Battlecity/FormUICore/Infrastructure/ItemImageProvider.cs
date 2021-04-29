using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using API.Components;
using FormUICore.FieldItems;

namespace FormUICore.Infrastructure
{
    public static class ItemImageProvider
    {
        private static readonly IDictionary<Element, Image> _dictionary = new ConcurrentDictionary<Element, Image>();

        public static Image GetItemImage(BaseItem baseItem)
        {

            return GetElementImage(baseItem.Element, baseItem.Sprite);
        }

        public static Image GetElementImage(Element element, string sprite)
        {
            if (!_dictionary.ContainsKey(element))
            {
                //var imgStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(sprite);
                //_dictionary[element] = new Bitmap(imgStream);

                _dictionary[element] = Image.FromFile(sprite);
            }

            return _dictionary[element];
        }
    }
}
