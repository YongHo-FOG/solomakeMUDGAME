using System.Threading;

namespace MiniGame
{
    internal class Program
    {
        public class Player
        {
            public string Name;
            public int HP;
            public int MaxHP;
            public int Attack;
            public List<Item> Inventory = new List<Item>();

            public void AttackMonster(Monster monster)
            {
                Console.WriteLine($"{Name}이(가) {monster.Name}을(를) 공격했습니다!");
                monster.HP -= this.Attack;
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
}
