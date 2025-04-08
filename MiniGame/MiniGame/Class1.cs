using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGame
{
    internal class Class1
    {
        public class Item
        {
            public string Name { get; private set; }
            public string Description { get; private set; }
            public bool IsKeyItem { get; private set; }  // 문을 열 수 있는 특수 아이템 여부

            public Item(string name, string description, bool isKeyItem = false)
            {
                Name = name;
                Description = description;
                IsKeyItem = isKeyItem;
            }
        }
        public class Player
        {
            public Room CurrentRoom { get; set; }
            public List<Item> Inventory { get; private set; }
            public int MaxInventorySize { get; } = 4;

            public Player(Room startingRoom)
            {
                CurrentRoom = startingRoom;
                Inventory = new List<Item>();
            }

            public void AddItem(Item item) { /* 인벤토리에 아이템 추가 */ }
            public void ShowInventory() { /* 인벤토리 출력 */ }
        }
    }
}
