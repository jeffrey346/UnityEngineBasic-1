﻿namespace Array2D
{
    internal class Program
    {
        // Map
        // 0 : 지나갈 수 있는 길
        // 1 : 지나갈 수 없는 벽
        // 2 : 도착지점
        // 5 : 플레이어
        static int[,] map = new int[6, 5]
        {
           { 0, 0, 0, 0, 1 },
           { 0, 1, 1, 1, 1 },
           { 0, 0, 0, 1, 1 },
           { 1, 1, 0, 1, 1 },
           { 1, 1, 0, 1, 1 },
           { 1, 1, 0, 0, 2 },
        };
        //플레이어 생성
        static int x, y;

        static void Main(string[] args)
        {
            map[y, x] = 5;
            int goalY = map.GetLength(0) - 1;
            int goalX = map.GetLength(1) - 1;
            string userInput = string.Empty;
            while (map[goalY, goalX] != 5)
            {
                userInput = Console.ReadLine();
                userInput = userInput.ToUpper();
                if (userInput == "L") { MoveLeft(); }
                else if (userInput == "R") { MoveRight(); }
                else if (userInput == "U") { MoveUp(); }
                else if (userInput == "D") { MoveDown(); }
                else Console.WriteLine("잘못된 입력입니다");
            }
            DisplayMap();
        }

        static void DisplayMap()
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == 0)
                        Console.Write("□");
                    else if (map[i, j] == 1)
                        Console.Write("■");
                    else if (map[i, j] == 2)
                        Console.Write("☆");
                    else if (map[i, j] == 5)
                        Console.Write("◈");
                }
                Console.WriteLine();
            }
        }

        static void MoveRight()
        {
            // 맵의 경계를 벗어나는지 체크
            if (x >= map.GetLength(1) - 1)
            {
                Console.WriteLine("해당 방향으로 움직일 수 없습니다. 맵의 경계를 벗어납니다");
                return;
            }

            // 지나갈수 없는 타일인지 체크
            if (map[y, x + 1] == 1)
            {
                Console.WriteLine("해당 방향은 막혀있습니다.");
                return;
            }


            map[y, x++] = 0;
            map[y, x] = 5;
            DisplayMap();
        }
        static void MoveLeft()
        {
            if (x <= 0)
            {
                Console.WriteLine("해당 방향으로 움직일 수 없습니다. 맵의 경계를 벗어납니다");
                return;
            }

            if (map[y, x - 1] == 1)
            {
                Console.WriteLine("해당 방향은 막혀있습니다.");
                return;
            }

            map[y, x--] = 0;
            map[y, x] = 5;
            DisplayMap();
        }
        static void MoveUp()
        {
            if (y <= 0)
            {
                Console.WriteLine("해당 방향으로 움직일 수 없습니다. 맵의 경계를 벗어납니다");
                return;
            }

            if (map[y - 1, x] == 1)
            {
                Console.WriteLine("해당 방향은 막혀있습니다.");
                return;
            }

            map[y, x] = 5;
            map[y, x] = 0;
            DisplayMap();
        }
        static void MoveDown()
        {
            if (y >= -1)
            {
                Console.WriteLine("해당 방향으로 움직일 수 없습니다. 맵의 경계를 벗어납니다");
                return;
            }

            if (map[y - 1, x] == 1)
            {
                Console.WriteLine("해당 방향은 막혀있습니다.");
                return;
            }

            map[y, x] = 5;
            map[y--, x] = 0;
            DisplayMap();
        }
    }

}
                                                                                                                                                                                                                                                                    