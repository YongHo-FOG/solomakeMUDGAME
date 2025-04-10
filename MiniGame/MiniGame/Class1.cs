namespace MiniGame
{
    // 게임 전체를 구성하는 클래스 모음
    internal class Class1
    {
        // 게임 엔딩 클래스 , 그런데 열거형으로 만드는것이 더 도움이 된다고한다?
        public class GameManager
        {
            public bool Gameover { get; set; } = false; // 마지막 방에 도달하면 true로 변경
            public bool HiddenEnding { get; set; } = false; // 쪽지를 모두 찾을경우 true 로 변경
        }

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
            public List<Item> Inventory = new List<Item>(); // 아이템 객체 저장

            public void ShowInventory()
            {
                Console.WriteLine("\n==== [인벤토리] ====");

                if (Inventory.Count == 0)
                {
                    Console.WriteLine("인벤토리가 비어 있습니다.");
                    return;
                }

                foreach (var item in Inventory)
                {
                    Console.WriteLine($"- {item.Name}: {item.Description}");
                }
            }

            public bool HasItem(string itemName)
            {
                return Inventory.Exists(i => i.Name == itemName);
            }

            public void AddItem(Item item)
            {
                Inventory.Add(item);
                Console.WriteLine($"'{item.Name}'를(을) 얻었다!");
            }

            public void RemoveItemByName(string itemName)
            {
                Item item = Inventory.Find(i => i.Name == itemName);
                if (item != null)
                {
                    Inventory.Remove(item);
                    Console.WriteLine($"'{item.Name}'을(를) 인벤토리에서 제거했다.");
                }
            }
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
            private GameManager gameManager;

            public StartRoom(GameManager gm)
            {
                gameManager = gm;
            }
            // 상태 관리 변수들
            bool firstVisit = true; // 방문횟수가 처음인가?
            bool keyFound = false; // 잠겨있는 방을 열 수있는 아이템을 찾았는가?
            bool noteFound = false; // 진엔딩을 볼 수 있는 아이템을 찾았는가?
            int frameAttempts = 0;   // 액자 시도 횟수

            public override void Enter(Player player)
            {
                Console.WriteLine("\n[낡은 침대가 있는 어두운 방]");

                if (firstVisit)
                {
                    Console.WriteLine("내가 왜 이런 곳에 있지... 뭔가 이상해... 일단 나가야겠어.");
                    firstVisit = false;
                }

                List<string> options = new List<string>
                 {
                     "1. 문을 연다",
                     "2. 침대 밑을 확인해본다",
                     "3. 벽에 걸린 액자를 확인해본다"
                 };

                foreach (var option in options)
                {
                    Console.WriteLine(option);
                }

                Console.Write("선택: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        if (player.HasItem("쇠로 된 열쇠"))
                        {
                            Console.WriteLine("끼이익 하는 소리와 함께 문이 열렸다!");
                            player.RemoveItemByName("쇠로 된 열쇠");
                            CanMove = true;
                        }
                        else
                        {
                            Console.WriteLine("잠겨있는 듯 하다.");
                        }
                        break;
                    case "2": InspectUnderBed(player); break;
                    case "3": InspectFrame(player); break;
                    default: Console.WriteLine("잘못된 입력입니다."); break;
                }
            }

            // 침대 밑 확인
            void InspectUnderBed(Player player)
            {
                if (keyFound)
                {
                    Console.WriteLine("\n 더 이상 볼 것은 없다.");
                    return;
                }

                Console.WriteLine("1. 손을 휘저어 쥐들을 쫓아낸다\n2. 아무것도 하지 않는다");
                Console.Write("선택: ");
                string subChoice = Console.ReadLine();
                if (subChoice == "1")
                {
                    Console.WriteLine("쥐들을 쫓아냈다. '쇠로 된 열쇠'를 얻었다!");
                    player.AddItem(new Item("쇠로 된 열쇠", "녹슨 오래된 열쇠"));
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
                        {
                            Console.WriteLine("제법 단단히 붙어있어 떼어지지 않는다.");
                        }
                        else if (frameAttempts == 2)
                        {
                            Console.WriteLine("조금 헐거워진 것 같다.");
                        }
                        else if (frameAttempts == 3)
                        {
                            Console.WriteLine("벽에서 액자가 떨어졌다!\n'의문의 쪽지 -1'을 얻었다.");
                            player.AddItem(new Item("의문의 쪽지 -1", "알 수 없는 메시지가 적힌 쪽지"));
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

                    // 추가: 액자 때고 나면 자동으로 빠져나가기 때문에 추가 코드 필요 없음
                }
            }


            public override Room Move(Player player)
            {
                Console.Clear();
                return new Hallway(gameManager);
            } // 연결통로로 이동
        }


        public class Hallway : Room
        {
            private bool firstVisit = true;
            private bool Lookaround = false;
            private GameManager gameManager;
            private string lastChoice = "";

            public Hallway(GameManager gm)
            {
                gameManager = gm;
            }

            public override void Enter(Player player)
            {
                if (player.HasItem("작게 빛나는 무언가 1") && player.HasItem("작게 빛나는 무언가 2"))
                {
                    Console.WriteLine("두 아이템이 합쳐져 '동전 모양의 열쇠'를 얻었다!");
                    player.RemoveItemByName("작게 빛나는 무언가 1");
                    player.RemoveItemByName("작게 빛나는 무언가 2");
                    player.AddItem(new Item("동전 모양의 열쇠", "어딘가의 문을 열 수 있을 것 같다."));
                }

                Console.WriteLine("\n[연결통로]");

                List<string> options = Lookaround
                    ? new List<string>
                    {
                        "1. 주위를 둘러본다",
                        "2. 왼쪽 문으로 이동한다",
                        "3. 오른쪽 문으로 이동한다",
                        "4. 정면 문으로 이동한다",
                        "5. 이전 방으로 돌아간다",
                        "0. 인벤토리 보기"
                    }
                    : new List<string>
                    {
                        "1. 주위를 둘러본다",
                        "2. 이전 방으로 돌아간다",
                        "0. 인벤토리 보기"
                    };

                foreach (var option in options)
                {
                    Console.WriteLine(option);
                }

                Console.Write("선택: ");
                lastChoice = Console.ReadLine();


                if (lastChoice == "1")
                {
                    Console.WriteLine("독특한 디자인의 방이다...\n또 다른 문이 3개가 있다.");
                    Lookaround = true;
                }
                else if (lastChoice == "0")
                {
                    player.ShowInventory(); //  인벤토리 출력
                }
                else if ((lastChoice == "2" && !Lookaround) || (lastChoice == "5" && Lookaround))
                {
                    if (player.HasItem("의문의 쪽지 -1"))
                    {
                        Console.WriteLine("다시 돌아갈 필요는 없을 것 같다.");
                    }
                    else
                    {
                        CanMove = true;
                    }
                }
                else if ((lastChoice == "2" || lastChoice == "3" || lastChoice == "4") && Lookaround)
                {
                    if (lastChoice == "4" && player.HasItem("동전 모양의 열쇠"))
                    {
                        Console.WriteLine("동전 모양의 열쇠를 문에 꽂았더니 찰칵 소리와 함께 문이 열렸다.");
                        CanMove = true;
                    }
                    else if (lastChoice == "2" && Lookaround)
                    {
                        Console.WriteLine("왼쪽 문을 열어보니 서재로 연결된다.");
                        CanMove = true;
                    }
                    else if (lastChoice == "3")
                    {
                        Console.WriteLine("오른쪽 문을 열어보니 테라스로 연결된다.");
                        CanMove = true;
                    }
                    else if (lastChoice == "4")
                    {
                        Console.WriteLine("문이 잠겨있다. 자세히보니 독특한 둥근 구멍이 있다. 여기에 무언가를 꽂아야할지도?");
                    }
                }
            }

            public override Room Move(Player player)
            {
                return lastChoice switch
                {
                    "2" => new Library(gameManager),
                    "3" => new Terrace(gameManager),
                    "4" => new FinalRoom(gameManager),
                    "5" => new StartRoom(gameManager),
                    _ => this // 현재 방 유지
                };
            }
        }

        // 현재 의도한 부분은 처음 방에 도달 한 때에만 텍스트가 출력되고 이후 재방문 시에는 텍스트 출력이 되지 않아야한다. 현재 계속 출력됨.
        // 또한 연결통로의 경우 둘러보기를 한 후의 선택지가 5개가 되고 이후에 다른 방을 다녀온 이후에는 둘러본 결과가 true로 고정되고 계속 선택지는 5개가 나와야하는데...?아니다?
        // 변수로 설정한 firstVisit 의 값이 false가 될때를 수정해야할까? 조건을 분리하여 fisrtVisit 이 true일때의 선택지와 false 일때의 선택지로 구분하면?
        // examined 값이 false 에서 true로 된 이후로 다른 방에 다녀온 이후에도 유지되어야 한다...? 어떻게?
        public class Library : Room
        {
            private GameManager gameManager;

            public Library(GameManager gm)
            {
                gameManager = gm;
            }

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
                    firstVisit = false;
                }

                List<string> options = new List<string>
                {
                    "1. 이상하게 눈이 가는 책을 집는다",
                    "2. 방의 스위치를 누른다",
                    "0. 연결통로로 돌아간다"
                };

                if (lightOff && !foundShiny)
                {
                    options.Insert(2, "3. 작게 빛나는 무언가를 줍는다");
                }

                foreach (var option in options)
                {
                    Console.WriteLine(option);
                }

                Console.Write("선택: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        if (!gotNote)
                        {
                            Console.WriteLine("중간을 펼치니 '의문의 쪽지 -2'를 얻었다!");
                            player.AddItem(new Item("의문의 쪽지 -2", "또 다른 쪽지"));
                            gotNote = true;
                        }
                        else
                        {
                            Console.WriteLine("이 책에서는 더 얻을 게 없다.");
                        }
                        break;
                    case "2":
                        lightOff = !lightOff;
                        Console.WriteLine(lightOff ? "방이 어두워졌다." : "방이 밝아졌다.");
                        break;
                    case "3":
                        if (lightOff && !foundShiny)
                        {
                            Console.WriteLine("'작게 빛나는 무언가 2'를 주웠다!");
                            player.AddItem(new Item("작게 빛나는 무언가 2", "작은 빛을 내는 물건"));
                            foundShiny = true;
                        }
                        break;
                    case "0":
                        CanMove = true;
                        break;
                    default:
                        Console.WriteLine("잘못된 선택입니다.");
                        break;
                }
            }

            public override Room Move(Player player)
            {
                Console.Clear();
                return new Hallway(gameManager); // 항상 연결통로로 돌아감
            }
        }
        public class Terrace : Room
        {
            private GameManager gameManager;

            public Terrace(GameManager gm)
            {
                gameManager = gm;
            }



            bool firstVisit = true;   // 처음 입장 여부
            bool foundItem = false;   // '작게 빛나는 무언가 1'을 발견했는지

            public override void Enter(Player player)
            {
                Console.WriteLine("\n[테라스]");

                if (firstVisit)
                {
                    Console.WriteLine("시간이 한밤중인것일까 어둡다... 바깥에는 아무것도 보이지 않는다.");
                    firstVisit = false;
                }

                List<string> options = new List<string>
                {
                    "1. 난간을 살펴본다",
                    "2. 연결통로로 돌아간다"
                };

                foreach (var option in options)
                {
                    Console.WriteLine(option);
                }

                Console.Write("선택: ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    InspectRailing(player);
                }
                else if (choice == "2")
                {
                    CanMove = true;
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
                        player.AddItem(new Item("작게 빛나는 무언가 1", "빛나는 작은 물건"));
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
                Console.Clear();
                return new Hallway(gameManager); // 항상 연결통로로 돌아감
            }
        }

        public class FinalRoom : Room
        {
            private bool firstVisit = true;
            private GameManager gameManager;

            public FinalRoom(GameManager gm)
            {
                gameManager = gm;
            }

            public override void Enter(Player player)
            {
                Console.WriteLine("\n출구");

                if (firstVisit)
                {
                    Console.WriteLine("문이 열려 밖으로 나왔다.");
                    firstVisit = false;
                }

                // 플레이어 인벤토리를 검사2

                bool hasAllNotes = player.HasItem("의문의 쪽지 -1") && player.HasItem("의문의 쪽지 -2");

                if (hasAllNotes)
                {
                    Console.WriteLine("찾은 쪽지를 서로 겹쳐보니 이제는 이해할 수 있다.... 당신은 모든 비밀을 알아냈다.");
                    gameManager.HiddenEnding = true; // 히든 엔딩
                }
                else
                {
                    Console.WriteLine("내가 왜 갇혔는지 이게 대체 무슨일인지 모른체 살기위해 주변 민가를 찾아 달렸다.");
                }

                gameManager.Gameover = true; // 게임 오버

                Console.WriteLine("\n아무버튼이나 눌러서 게임종료.");
                Console.ReadKey();
                Environment.Exit(0); // 프로그램 종료
            }

            public override Room Move(Player player)
            {
                return null; // 더 이상 이동할 방이 없음
            }
        }
        // 현재 코드를 둘러보면 객체지향은 사용이 되었으나 자료구조나 알고리즘에 관한 내용이 거의 없는듯함...







































    }
}
