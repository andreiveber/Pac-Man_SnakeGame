using SnakeGame.Core;

namespace SnakeGame;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("=== ЗАПУСК PAC-MAN ЗМЕЙКИ ===");
            Console.WriteLine("Создание игрового окна...");

            var game = new Game();
            Console.WriteLine("✓ Игра успешно инициализирована");
            Console.WriteLine("=== УПРАВЛЕНИЕ ===");
            Console.WriteLine("F11 - Полноэкранный режим");
            Console.WriteLine("M - Музыка вкл/выкл");
            Console.WriteLine("ESC - Меню/Выход");
            Console.WriteLine("F5 - Сохранить игру");
            Console.WriteLine("F9 - Загрузить игру");
            Console.WriteLine("P - Пауза");
            Console.WriteLine("==============================");

            game.Run();

            Console.WriteLine("Игра завершена. Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"=== ОШИБКА ЗАПУСКА ИГРЫ ===");
            Console.WriteLine($"Сообщение: {ex.Message}");
            Console.WriteLine($"Тип ошибки: {ex.GetType().Name}");
            Console.WriteLine($"Детали: {ex.StackTrace}");
            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}