using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
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
                _dictionary[element] = Image.FromFile(sprite);
            }

            return _dictionary[element];
        }
    }
}
