using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Components;
using FormUI.FieldItems;

namespace FormUI.Infrastructure
{
    public static class ItemImageProvider
    {
        private static IDictionary<Element, Image> _dictionary = new Dictionary<Element, Image>();

        public static Image GetItemImage(BaseItem baseItem)
        {
            if (!_dictionary.ContainsKey(baseItem.Element))
            {
                _dictionary[baseItem.Element] = Image.FromFile(baseItem.Sprite);
            }

            return _dictionary[baseItem.Element];
        }
    }
}
