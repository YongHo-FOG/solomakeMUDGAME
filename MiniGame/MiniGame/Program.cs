using static MiniGame.Class1;

namespace MiniGame
{
    class Program
    {
        static Player player = new Player(); // 플레이어 객체 생성

        static void Main(string[] args)
        {
            Console.WriteLine("텍스트 어드벤처 게임에 오신 걸 환영합니다!");
            Room currentRoom = new StartRoom(); // 처음 방은 StartRoom

            while (true)
            {
                currentRoom.Enter(player); // 현재 방의 입장 이벤트 실행
                if (currentRoom.CanMove)   // 이동 가능한 경우
                    currentRoom = currentRoom.Move(player); // 다음 방으로 이동
            }
        }
    }
}
