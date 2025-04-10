using static MiniGame.Class1;

namespace MiniGame
{
    class Program
    {
        static Player player = new Player(); // 플레이어 객체 생성


        static void Main(string[] args)
        {
            
            Console.WriteLine("미스테리 이스케이프");
            Console.WriteLine("시작하려면 아무 버튼이나 눌러주세요");
            Console.ReadKey(true);
            Console.Clear();
            GameManager gameManager = new GameManager();
            Room currentRoom = new StartRoom(gameManager); // 처음 방은 StartRoom
             
            

            while (true)
            {
                currentRoom.Enter(player); // 현재 방의 입장 이벤트 실행
                if (currentRoom.CanMove)   // 이동 가능한 경우
                { 
                    currentRoom = currentRoom.Move(player); 
                } // 다음 방으로 이동

                if (gameManager.Gameover)
                {
                    Console.WriteLine("당신은 탈출에 성공하였습니다.");
                    Console.WriteLine("왜 여기에 갇혀있었는지, 또 여긴 어디인지에 대한 의문이 풀리지 않았지만 말이죠...");
                }
                else if (gameManager.Gameover && gameManager.HiddenEnding)
                {
                    Console.WriteLine("당신은 이 장소의 존재 목적이 무엇인지 알게되었고 여기가 어디인지 알게되었습니다.");
                    Console.WriteLine("진엔딩 완료");
                }
            }
        }
    }
}
