using API.Components;
using FormUI.FieldItems;

namespace FormUI.Infrastructure
{
    public static class ItemFactory
    {
        public static BaseItem GetItem(Element element, Point point)
        {
            var item = new BaseItem();
            item.Element = element;
            item.Point = point;

            return item;
        }
    }
}
