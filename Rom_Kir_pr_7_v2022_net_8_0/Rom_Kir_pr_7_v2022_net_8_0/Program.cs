class Program
{
    // Инициализация переменных
    static int playerHP = 350; // Здоровье героя
    static int playerMana = 250; // Мана героя
    static char[,] maze; // Лабиринт
    static int mazeWidth, mazeHeight; // Размеры лабиринта
    static int heroX, heroY, bossX, bossY, exitX, exitY; // Позиции героя, босса и выхода

    static void Main()
    {
        // Загрузка лабиринта из файла
        LoadMaze();

        // Основной цикл игры
        bool running = true;
        while (running)
        {
            Console.Clear();
            PrintMaze(); // Отрисовка лабиринта
            PrintStats(); // Отображение статистики (здоровья и маны)

            // Проверка условий проигрыша
            if (playerHP <= 0)
            {
                Console.WriteLine("Вы проиграли.");
                running = false;
                continue;
            }

            // Обработка ввода пользователя
            ConsoleKeyInfo keyInfo = Console.ReadKey();
            char key = keyInfo.KeyChar;

            // Вызов функции поиска пути при нажатии 'E'
            if (key == 'e')
            {
                FindPath();
                continue;
            }

            // Перемещение героя и босса
            MovePlayer(key);
            MoveBoss();

            // Проверка условий победы и проигрыша
            if (maze[heroY, heroX] == 'B')
            {
                Console.WriteLine("Герой повержен.");
                running = false;
            }

            if (maze[heroY, heroX] == 'X')
            {
                Console.WriteLine("Вы выиграли!");
                running = false;
            }

            Thread.Sleep(100); // Задержка для симуляции передвижения героя и босса
        }

        Console.ReadLine(); // Ждем нажатия клавиши для завершения
    }

    // Загрузка лабиринта из файла
    static void LoadMaze()
    {
        string[] lines = File.ReadAllLines("maze.txt");
        mazeWidth = lines[0].Length;
        mazeHeight = lines.Length;
        maze = new char[mazeHeight, mazeWidth];

        // Инициализация лабиринта и определение начальных позиций
        for (int y = 0; y < mazeHeight; y++)
        {
            for (int x = 0; x < mazeWidth; x++)
            {
                maze[y, x] = lines[y][x];

                // Определение начальных позиций героя, босса и выхода
                if (maze[y, x] == 'I')
                {
                    heroX = x;
                    heroY = y;
                }
                else if (maze[y, x] == 'B')
                {
                    bossX = x;
                    bossY = y;
                }
                else if (maze[y, x] == 'X')
                {
                    exitX = x;
                    exitY = y;
                }
            }
        }
    }

    // Отрисовка лабиринта с возможностью использования маршрута
    static void PrintMaze(char[,] mazeToPrint = null)
    {
        char[,] mazeToUse = mazeToPrint ?? maze; // Используем переданный массив лабиринта

        for (int y = 0; y < mazeHeight; y++)
        {
            for (int x = 0; x < mazeWidth; x++)
            {
                // Отображение героя вместо его позиции
                if (y == heroY && x == heroX)
                {
                    Console.Write('I');
                }
                else
                {
                    Console.Write(mazeToUse[y, x]); // Отображение элементов лабиринта
                }
            }
            Console.WriteLine();
        }
    }


    // Отображение статистики (здоровья и маны)
    static void PrintStats()
    {
        Console.WriteLine();

        // Отображение HP-бара
        Console.Write("HP: [");
        int hpBars = playerHP / 50;
        for (int i = 0; i < hpBars; i++)
        {
            Console.Write('#');
        }
        for (int i = hpBars; i < 7; i++)
        {
            Console.Write('_');
        }
        Console.WriteLine(']');

        // Отображение MP-бара
        Console.Write("MP: [");
        int mpBars = playerMana / 25;
        for (int i = 0; i < mpBars; i++)
        {
            Console.Write('#');
        }
        for (int i = mpBars; i < 10; i++)
        {
            Console.Write('_');
        }
        Console.WriteLine(']');
    }

    // Перемещение героя
    static void MovePlayer(char direction)
    {
        int newHeroX = heroX;
        int newHeroY = heroY;

        // Вычисление новой позиции героя в зависимости от направления
        if (direction == 'w' && heroY > 0 && maze[heroY - 1, heroX] != '#')
        {
            newHeroY--;
        }
        else if (direction == 'a' && heroX > 0 && maze[heroY, heroX - 1] != '#')
        {
            newHeroX--;
        }
        else if (direction == 's' && heroY < mazeHeight - 1 && maze[heroY + 1, heroX] != '#')
        {
            newHeroY++;
        }
        else if (direction == 'd' && heroX < mazeWidth - 1 && maze[heroY, heroX + 1] != '#')
        {
            newHeroX++;
        }

        // Перемещение героя, если новая позиция доступна
        if (newHeroX != heroX || newHeroY != heroY)
        {
            if (maze[newHeroY, newHeroX] == ' ' || maze[newHeroY, newHeroX] == 'X' || maze[newHeroY, newHeroX] == 'B')
            {
                maze[heroY, heroX] = ' ';
                maze[newHeroY, newHeroX] = 'I';
                heroX = newHeroX;
                heroY = newHeroY;
                playerHP -= 1; // Каждый ход героя теряет  1 HP

                if (maze[newHeroY, newHeroX] == maze[11,1])
                {
                    Console.WriteLine("Вы выиграли!");
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                }

                if (maze[newHeroY, newHeroX] == maze[bossY, bossX])
                {
                    playerHP -= 50; // Каждое столкновение с боссом наносит урон
                    Console.WriteLine("Вы потеряли 50 хп!");
                    Thread.Sleep(1000);
                }

            }
        }
    }

    // Перемещение босса
    static void MoveBoss()
    {
        int newBossX = bossX;
        int newBossY = bossY;

        Random random = new Random();
        int direction = random.Next(1, 5);

        // Выбор случайного направления движения босса
        if (direction == 1 && bossY > 0 && maze[bossY - 1, bossX] == ' ')
        {
            newBossY--;
        }
        else if (direction == 2 && bossX > 0 && maze[bossY, bossX - 1] == ' ')
        {
            newBossX--;
        }
        else if (direction == 3 && bossY < mazeHeight - 1 && maze[bossY + 1, bossX] == ' ')
        {
            newBossY++;
        }
        else if (direction == 4 && bossX < mazeWidth - 1 && maze[bossY, bossX + 1] == ' ')
        {
            newBossX++;
        }

        // Перемещение босса, если новая позиция доступна
        if (newBossX != bossX || newBossY != bossY)
        {
            maze[bossY, bossX] = ' ';
            maze[newBossY, newBossX] = 'B';
            bossX = newBossX;
            bossY = newBossY;
        }
    }

    // Поиск пути от героя до выхода
    static void FindPath()
    {
        char[,] path = new char[mazeHeight, mazeWidth];
        Array.Copy(maze, path, mazeHeight * mazeWidth);

            // Пометка пути от героя до выхода символом '*'
            for (int y = 0; y < 2; y++) { path[y, 1] = '*'; }
            for (int x = 1; x < 13; x++) { path[2, x] = '*'; }
            for (int y = 2; y < 5; y++) { path[y, 12] = '*'; }
            for (int x = 12; x > 8; x--) { path[4, x] = '*'; }
            for (int y = 4; y < 7; y++) { path[y, 9] = '*'; }
            for (int x = 9; x > 5; x--) { path[6, x] = '*'; }
            for (int y = 6; y < 11; y++) { path[y, 6] = '*'; }
            for (int x = 6; x >0 ; x--) { path[10, x] = '*'; }
           
            Console.Clear();
            PrintMaze(path); // Отрисовка лабиринта с путем
            PrintStats(); // Отображение статистики
            Console.WriteLine("Маршрут найден.");
            Thread.Sleep(2000); // Задержка для отображения маршрута
    }
}

