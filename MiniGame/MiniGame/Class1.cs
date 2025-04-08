using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniGame
{
    // 게임 전체를 구성하는 클래스 모음
    internal class Class1
    {
        // ========== 1. 아이템 클래스 ==========
        public class Item
        {
            public string Name { get; private set; }  // 아이템 이름
            public string Description { get; private set; } // 아이템 설명
            public bool IsKeyItem { get; private set; }  // 열쇠 아이템 여부 (잠긴 문을 여는 데 필요)

            public Item(string name, string description, bool isKeyItem = false)
            {
                Name = name;
                Description = description;
                IsKeyItem = isKeyItem;
            }
        }

        // ========== 2. 플레이어 클래스 ==========
        // 플레이어 클래스 (아이템 인벤토리 보유)
        public class Player
        {
            public List<string> Inventory = new List<string>();
        }


        // ========== 3. 방 (Room) 클래스 ==========
        public abstract class Room
        {
            public bool CanMove { get; protected set; } = false; // 방을 이동할 수 있는지
            public abstract void Enter(Player player); // 방에 입장했을 때 실행
            public abstract Room Move(Player player);  // 방을 이동할 때 실행
        }

        public class StartRoom : Room
        {
            // 상태 관리 변수들
            bool firstVisit = true;
            bool keyFound = false;
            bool noteFound = false;
            int frameAttempts = 0;   // 액자 시도 횟수

            public override void Enter(Player player)
            {
                
                Console.WriteLine("\n[낡은 침대가 있는 어두운 방]");

                if (firstVisit)
                {
                    Console.WriteLine("내가 왜 이런 곳에 있지... 뭔가 이상해... 일단 나가야겠어.");
                    firstVisit = false; // 처음 입장했을 때만 출력
                }

                // 플레이어 행동 선택지
                Console.WriteLine("1. 문을 연다\n2. 침대 밑을 확인해본다\n3. 벽에 걸린 액자를 확인해본다");
                Console.Write("선택: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": // 문을 연다
                        if (player.Inventory.Contains("쇠로 된 열쇠"))
                        {
                            Console.WriteLine("끼이익 하는 소리와 함께 문이 열렸다!");
                            CanMove = true; // 이동 가능 상태로 전환
                        }
                        else
                        {
                            Console.WriteLine("잠겨있는 듯 하다.");
                        }
                        break;
                    case "2": InspectUnderBed(player); break; // 침대 밑 확인
                    case "3": InspectFrame(player); break;    // 액자 확인
                    default: Console.WriteLine("잘못된 입력입니다."); break;
                }
            }

            // 침대 밑 확인
            void InspectUnderBed(Player player)
            {
                if (keyFound)
                {
                    Console.WriteLine("더 이상 볼 것은 없다.");
                    return;
                }

                Console.WriteLine("1. 손을 휘저어 쥐들을 쫓아낸다\n2. 아무것도 하지 않는다");
                Console.Write("선택: ");
                string subChoice = Console.ReadLine();
                if (subChoice == "1")
                {
                    Console.WriteLine("쥐들을 쫓아냈다. '쇠로 된 열쇠'를 얻었다!");
                    player.Inventory.Add("쇠로 된 열쇠");
                    keyFound = true;
                }
                else
                {
                    Console.WriteLine("아무 일도 일어나지 않았다.");
                }
            }

            // 액자 확인
            void InspectFrame(Player player)
            {
                if (noteFound)
                {
                    Console.WriteLine("이미 확인한 액자다.");
                    return;
                }

                Console.WriteLine("1. 액자를 떼어내려 한다\n2. 아무것도 하지 않는다");
                Console.Write("선택: ");
                string subChoice = Console.ReadLine();
                if (subChoice == "1")
                {
                    frameAttempts++;
                    if (frameAttempts == 1)
                        Console.WriteLine("제법 단단히 붙어있어 떼어지지 않는다.");
                    else if (frameAttempts == 2)
                        Console.WriteLine("조금 헐거워진 것 같다.");
                    else if (frameAttempts == 3)
                    {
                        Console.WriteLine("벽에서 액자가 떨어졌다!\n'의문의 쪽지 -1'을 얻었다.");
                        player.Inventory.Add("의문의 쪽지 -1");
                        noteFound = true;
                    }
                }
                else
                {
                    Console.WriteLine("아무 일도 일어나지 않았다.");
                }
            }

            public override Room Move(Player player) => new Hallway(); // 연결통로로 이동
        }
    }
}
