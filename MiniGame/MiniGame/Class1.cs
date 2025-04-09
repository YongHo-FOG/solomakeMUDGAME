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

                bool keepInspecting = true; // 반복 여부 체크 변수

                while (keepInspecting) // 액자 메뉴 반복
                {
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
                            keepInspecting = false; // 성공했으면 반복 종료
                        }
                    }
                    else if (subChoice == "2")
                    {
                        Console.WriteLine("아무 일도 일어나지 않았다.");
                        keepInspecting = false; // 아무것도 안 하면 반복 종료
                    }
                    else
                    {
                        Console.WriteLine("잘못된 입력입니다."); // 잘못 입력해도 계속 반복
                    }

                    // 추가: 액자 떨어뜨리고 나면 자동으로 빠져나가기 때문에 추가 코드 필요 없음
                }
            }

            public override Room Move(Player player) => new Hallway(); // 연결통로로 이동
        }

        public class Hallway : Room
        {
            public bool firstVisit = true;
            bool examined = false; // 주위를 둘러봤는지

            // 현재 의도한 부분은 처음 방에 도달 한 때에만 텍스트가 출력되고 이후 재방문 시에는 텍스트 출력이 되지 않아야한다. 현재 계속 출력됨.
            // 또한 연결통로의 경우 둘러보기를 한 후의 선택지가 5개가 되고 이후에 다른 방을 다녀온 이후에는 둘러본 결과가 true로 고정되고 계속 선택지는 5개가 나와야하는데...?아니다?
            // 변수로 설정한 firstVisit 의 값이 false가 될때를 수정해야할까? 조건을 분리하여 fisrtVisit 이 true일때의 선택지와 false 일때의 선택지로 구분하면?
            // examined 값이 false 에서 true로 된 이후로 다른 방에 다녀온 이후에도 유지되어야 한다...? 어떻게?
            public override void Enter(Player player)
            {
                // 아이템 자동 조합
                if (player.Inventory.Contains("작게 빛나는 무언가 1") && player.Inventory.Contains("작게 빛나는 무언가 2"))
                {
                    Console.WriteLine("두 아이템이 사라졌다.\n'동전 모양의 열쇠'를 얻었다!");
                    player.Inventory.Remove("작게 빛나는 무언가 1");

                    player.Inventory.Remove("작게 빛나는 무언가 2");

                    player.Inventory.Add("동전 모양의 열쇠");
                }

                Console.WriteLine("\n[연결통로]");

                Console.WriteLine("1. 주위를 둘러본다"); 

                if (examined)
                {
                    // 주위를 이미 둘러봤으면 무조건 5개 선택지 고정
                    Console.WriteLine("2. 왼쪽 문으로 이동한다");
                    Console.WriteLine("3. 오른쪽 문으로 이동한다");
                    Console.WriteLine("4. 정면 문으로 이동한다");
                    Console.WriteLine("5. 이전 방으로 돌아간다");
                }
                else
                {
                    // 아직 주위를 둘러보지 않았으면 2번만
                    Console.WriteLine("2. 이전 방으로 돌아간다");
                }

                Console.Write("선택: ");
                string choice = Console.ReadLine();

                // 행동 처리
                if (choice == "1")
                {
                    Console.WriteLine("독특한 디자인의 방이다...\n또 다른 문이 3개가 있다.");
                    examined = true;
                }
                else if ((choice == "2" && !examined) || (choice == "5" && examined))
                {
                    if (player.Inventory.Contains("의문의 쪽지 -1"))
                        Console.WriteLine("다시 돌아갈 필요는 없을 것 같다.");
                    else
                        CanMove = true;
                }
                else if ((choice == "2" || choice == "3" || choice == "4") && examined)
                {
                    if (choice == "4" && !player.Inventory.Contains("동전 모양의 열쇠"))
                        Console.WriteLine("독특한 구멍이 있다. 아직 열 수 없다.");
                    else
                        CanMove = true;
                }
            }

            // 현재 연결통로에서 서재와 테라스로 이동 할 때에 선택지 번호를 두번 입력해야한다.
            // 선택지 입력에서 바로 이동하게 해야하는가? 그렇다면 여기에서는 오버라이드를 쓰지못하는것인가?
            // 그런데 추상클래스로 구현한것이라서 구현하지않으면 오류가 날탠데? 그럼 어떻게 바꿔야하지?
            // 여기서 마찬가지로 시작방으로 돌아갈때 또한 텍스트가 제출력된다. 근본적인 수정이 필요함.
            public override Room Move(Player player)
            {
                Console.Write("이동할 방 번호를 입력하세요: ");
                string input = Console.ReadLine();
          
                return input switch
                {
                    "2" => new Library(),
                    "3" => new Terrace(),
                    
                    "5" => new StartRoom(),
                };
            }
        }
        public class Library : Room
        {
            bool firstVisit = true;   // 처음 입장 여부
            bool gotNote = false;     // 의문의 쪽지 -2를 얻었는지
            bool lightOff = false;    // 방 조명 꺼짐 여부
            bool foundShiny = false;  // '작게 빛나는 무언가 2'를 발견했는지

            public override void Enter(Player player)
            {

                Console.WriteLine("\n[서재]");

                if (firstVisit)
                {
                    Console.WriteLine("으... 먼지야... 알아보지도 못하는 제목의 책들이 엄청 많네...");
                    firstVisit = false; // 첫 방문 시 대사 출력
                }

                // 행동 선택지
                Console.WriteLine("1. 이상한 책을 펼친다\n2. 방의 스위치를 누른다");
                if (lightOff && !foundShiny)
                    Console.WriteLine("3. '작게 빛나는 무언가'를 줍는다"); // 방이 어두우면 빛나는 무언가 등장
                Console.WriteLine("4. 연결통로로 돌아간다");

                Console.Write("선택: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": // 이상한 책을 펼친다
                        if (!gotNote)
                        {
                            Console.WriteLine("중간을 펼치니 '의문의 쪽지 -2'를 얻었다!");
                            player.Inventory.Add("의문의 쪽지 -2");
                            gotNote = true;
                        }
                        else
                        {
                            Console.WriteLine("이 책에서는 더 얻을 게 없다.");
                        }
                        break;

                    case "2": // 방의 스위치를 누른다
                        lightOff = !lightOff;
                        Console.WriteLine(lightOff ? "방이 어두워졌다." : "방이 밝아졌다.");
                        break;

                    case "3": // 어두워졌을 때만 나타나는 아이템
                        if (lightOff && !foundShiny)
                        {
                            Console.WriteLine("'작게 빛나는 무언가 2'를 주웠다!");
                            player.Inventory.Add("작게 빛나는 무언가 2");
                            foundShiny = true;
                        }
                        break;

                    case "4": // 연결통로로 돌아간다
                        CanMove = true;
                        break;
                        // 이 부분도 기본적으로 케이스로 구분되어 1~4번의 선택지가 지정되지만 방이 어두워지기 전에는 1~3번의 선택지로 책,스위치,통로돌아가기 이고
                        //이후에 불이 어두워지면 1~4번으로 선택지가 늘어나고 다시 불이 켜지면 선택지가 줄어들도록 할 수있어야 하는것이 의도인데...어떻게?
                        // 변수를 추가하여 그 변수가 true 일때와 false 일때 나오는 선택지를 각각 구분해면 될까?
                    default:
                        Console.WriteLine("잘못된 선택입니다.");
                        break;
                }
            }

            public override Room Move(Player player)
            {
                return new Hallway(); // 항상 연결통로로 돌아감
            }
        }
        public class Terrace : Room
        {
            bool firstVisit = true;   // 처음 입장 여부
            bool foundItem = false;   // '작게 빛나는 무언가 1'을 발견했는지

            public override void Enter(Player player)
            {
                Console.WriteLine("\n[테라스]");

                if (firstVisit)
                {
                    Console.WriteLine("시간이 한밤중이라 그런가... 바깥에는 아무것도 보이지 않는다.");
                    firstVisit = false; // 첫 방문 시 대사 출력
                }

                // 행동 선택지
                Console.WriteLine("1. 난간을 살펴본다 2. 연결통로로 돌아간다");
                Console.Write("선택: ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    InspectRailing(player); // 난간 살펴보기
                }
                else if (choice == "2")
                {
                    CanMove = true; // 연결통로로 돌아감
                }
                else
                {
                    Console.WriteLine("잘못된 선택입니다.");
                }
            }

            // 난간 탐색
            void InspectRailing(Player player)
            {
                Console.WriteLine("1. 난간 위를 살펴본다 2. 난간 아래를 살펴본다");
                Console.Write("선택: ");
                string input = Console.ReadLine();

                if (input == "1")
                {
                    Console.WriteLine("아무것도 없는 것 같다."); // 위쪽은 아무것도 없음
                }
                else if (input == "2")
                {
                    if (!foundItem)
                    {
                        Console.WriteLine("'작게 빛나는 무언가 1'을 주웠다!");
                        player.Inventory.Add("작게 빛나는 무언가 1");
                        foundItem = true; // 아이템 1회성
                    }
                    else
                    {
                        Console.WriteLine("이미 아이템을 찾았다.");
                    }
                }
            }

            public override Room Move(Player player)
            {
                return new Hallway(); // 항상 연결통로로 돌아감
            }
        }

       // // ========== 7. 플레이어 행동 (PlayerAction) 클래스 ==========
       // public class PlayerAction
       // {
       //     public string Description { get; private set; }  // 행동에 대한 설명
       //     public List<InteractableObject> InteractableObjects { get; private set; }
       //     // 상호작용할 수 있는 오브젝트 목록
       //
       //     public PlayerAction(string description)
       //     {
       //         Description = description;
       //         InteractableObjects = new List<InteractableObject>();
       //     }
       //
       //     public void Perform(Player player)
       //     {
       //         // 행동을 수행하고 오브젝트와 상호작용하는 로직 작성 예정
       //     }
       // }
       //
       // // ========== 8. 상호작용 가능한 오브젝트 (InteractableObject) 클래스 ==========
       // public class InteractableObject
       // {
       //     public string Name { get; private set; }  // 오브젝트 이름
       //     public string Description { get; private set; } // 오브젝트 설명
       //     public Item ItemInside { get; private set; }  // 오브젝트 안에 들어있는 아이템
       //     public bool IsActive { get; private set; } = true;  // 오브젝트가 활성 상태인지 여부
       //
       //     public InteractableObject(string name, string description, Item itemInside)
       //     {
       //         Name = name;
       //         Description = description;
       //         ItemInside = itemInside;
       //     }
       //
       //     public void Interact(Player player)
       //     {
       //         // 플레이어가 이 오브젝트와 상호작용할 때 실행할 로직 작성 예정
       //     }
       // }
    }
}
