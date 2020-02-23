using System;

namespace shopping_list_client.Model
{
    class Item : IComparable
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool Bought { get; set; }

        public int CompareTo(object obj)
        {
            Item item = obj as Item;
            if (item.Bought == Bought)
            {
                return 0;
            }
            else
            {
                if (Bought)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
        }
    }
}
