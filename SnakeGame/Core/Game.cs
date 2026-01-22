using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using System.IO;
using System.Numerics;

namespace SnakeGame.Core;

public class Game
{
    // Создание необходимых директорий
    private RenderWindow _window;
    private AssetManager _assetManager;
    private InputManager _inputManager;
    private SaveManager _saveManager;
    private AudioManager _audioManager;
    private GameStateManager _stateManager;
    private readonly SFML.System.Clock _clock;

    private bool _isFullscreen = false;
    private VideoMode _windowedMode;
    private VideoMode _fullscreenMode;

    public Game()
    {
        // Инициализация директорий
        Directory.CreateDirectory("Assets");
        Directory.CreateDirectory("Assets/Backgrounds");
        Directory.CreateDirectory("Assets/Music");
        Directory.CreateDirectory("Assets/Sounds");
        Directory.CreateDirectory("Assets/Data/saves");
        Directory.CreateDirectory("Assets/Fonts");

        // Сохраняет режимы отображения
        _windowedMode = new VideoMode(Configuration.WindowWidth, Configuration.WindowHeight);
        _fullscreenMode = VideoMode.DesktopMode;

        // Создает оконный режим по умолчанию
        _window = new RenderWindow(
            _windowedMode,
            Configuration.WindowTitle,
            Styles.Close | Styles.Resize | Styles.Titlebar);

        _window.SetKeyRepeatEnabled(false);
        // Подписывает на события окна
        _window.Closed += (sender, e) => _window.Close();
        _window.KeyPressed += OnKeyPressed;
        _window.SetFramerateLimit(60);
        _window.SetVerticalSyncEnabled(true);
        // Создает менеджеры
        _assetManager = new AssetManager();
        _inputManager = new InputManager();
        _saveManager = new SaveManager();
        _audioManager = new AudioManager();
        // Загружает ресурсы
        LoadResources();

        // Запускает фоновую музыку
        StartBackgroundMusic();

        // Создает менеджер состояний с 6 параметрами
        _stateManager = new GameStateManager(_assetManager, _inputManager,
            _saveManager, _audioManager, _window);

        _clock = new SFML.System.Clock();
    }
    // Обработчик нажатия клавиш на уровне окна
    private void OnKeyPressed(object sender, SFML.Window.KeyEventArgs e)
    {
        // Переключение полноэкранного режима 
        if (e.Code == Keyboard.Key.F11)
        {
            ToggleFullscreen();
        }
        // Мьютирование музыки по M
        else if (e.Code == Keyboard.Key.M)
        {
            ToggleMusic();
        }
    }
    // Переключает между оконным и полноэкранным режимом
    private void ToggleFullscreen()
    {
        _isFullscreen = !_isFullscreen;

        var oldWindow = _window;
        // Создает новое окно в нужном режиме
        if (_isFullscreen)
        {
            _window = new RenderWindow(_fullscreenMode, Configuration.WindowTitle, Styles.Fullscreen);
        }
        else
        {
            _window = new RenderWindow(_windowedMode, Configuration.WindowTitle,
                Styles.Close | Styles.Resize | Styles.Titlebar);
        }

        // Настраивает новое окно
        _window.SetKeyRepeatEnabled(false);

        _window.Closed += (sender, e) => _window.Close();
        _window.KeyPressed += OnKeyPressed;
        _window.SetFramerateLimit(60);
        _window.SetVerticalSyncEnabled(true);

        _stateManager.UpdateWindow(_window);

        oldWindow.Close();
    }
    // Обновляет ссылку на окно в менеджере состояний
    private void ToggleMusic()
    {
        if (_audioManager != null)
        {
            var music = _audioManager.GetMusicStatus();
            if (music == SFML.Audio.SoundStatus.Playing)
            {
                _audioManager.PauseMusic();
            }
            else if (music == SFML.Audio.SoundStatus.Paused)
            {
                _audioManager.ResumeMusic();
            }
        }
    }
    // Загружает все необходимые ресурсы игры
    private void LoadResources()
    {
        // Загрузка шрифтов
        string[] possibleFontPaths =
        {
            "Assets/Fonts/arial.ttf",
            "Assets/Fonts/tahoma.ttf",
            "C:/Windows/Fonts/arial.ttf",
            "C:/Windows/Fonts/tahoma.ttf"
        };

        foreach (var fontPath in possibleFontPaths)
        {
            if (File.Exists(fontPath))
            {
                try
                {
                    _assetManager.LoadFont("main", fontPath);
                    Console.WriteLine($"Шрифт загружен: {fontPath}");
                    break;
                }
                catch { continue; }
            }
        }

        // Загрузка фоновых изображений
        LoadBackgroundImages();

        // Загрузка музыки
        LoadMusic();

        // Загрузка звуковых эффектов
        //LoadSoundEffects();

        // Создает текстуры в стиле Pac-Man
        CreatePacManTextures();
    }
    // Загружает музыкальные файлы
    private void LoadMusic()
    {
        try
        {
            Console.WriteLine("=== ЗАГРУЗКА МУЗЫКИ ===");

            // Проверяет существующие пути к музыке
            string[] possibleMusicPaths =
            {
                "Assets/Music/pacman_theme.mp3",
                "Assets/Music/theme.mp3",
                "Assets/Music/game_music.mp3",
                "Assets/Music/music.mp3",
                "Assets/pacman_theme.mp3"
            };

            bool musicLoaded = false;

            foreach (var musicPath in possibleMusicPaths)
            {
                if (File.Exists(musicPath))
                {
                    try
                    {
                        _audioManager.LoadMusic("pacman_theme", musicPath);
                        Console.WriteLine($"Музыка загружена: {musicPath}");
                        musicLoaded = true;
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"✗ Ошибка загрузки музыки {musicPath}: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine($"✗ Файл не найден: {musicPath}");
                }
            }

            if (!musicLoaded)
            {
                Console.WriteLine("Предупреждение: музыкальные файлы не найдены!");
                Console.WriteLine("Поместите MP3 файл в одну из следующих папок:");
                Console.WriteLine("- Assets/Music/pacman_theme.mp3");
                Console.WriteLine("- Assets/Music/theme.mp3");
                Console.WriteLine("- Assets/Music/game_music.mp3");
                Console.WriteLine("- Assets/Music/music.mp3");
                Console.WriteLine("- Assets/pacman_theme.mp3");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки музыки: {ex.Message}");
        }

        Console.WriteLine("=== ЗАГРУЗКА МУЗЫКИ ЗАВЕРШЕНА ===");
    }
    // Запускает фоновую музыку
    private void StartBackgroundMusic()
    {
        try
        {
            Console.WriteLine("=== ЗАПУСК ФОНОВОЙ МУЗЫКИ ===");

            // Запускаем музыку для меню
            _audioManager.PlayMusic("pacman_theme", true, Configuration.MusicVolume);

            if (_audioManager.GetMusicStatus() == SFML.Audio.SoundStatus.Playing)
            {
                Console.WriteLine("Фоновая музыка запущена успешно");
            }
            else
            {
                Console.WriteLine("Музыка не проигрывается");
            }

            Console.WriteLine("=== ФОНОВАЯ МУЗЫКА ЗАПУЩЕНА ===");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка запуска музыки: {ex.Message}");
        }
    }
    // Загружает фоновые изображения или создает их программно
    private void LoadBackgroundImages()
    {
        try
        {
            // Проверяет существование файлов фонов
            string menuBgPath = "Assets/Backgrounds/menu_bg.jpg";
            string gameOverBgPath = "Assets/Backgrounds/gameover_bg.jpg";

            if (File.Exists(menuBgPath))
                _assetManager.LoadBackground("menu", menuBgPath);
           
            if (File.Exists(gameOverBgPath))
                _assetManager.LoadBackground("gameover", gameOverBgPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки фонов: {ex.Message}");
        }
    }
    // Создает текстуры в стиле Pac-Man
    private void CreatePacManTextures()
    {
        // Создает текстуру для тела змеи
        _assetManager.CreateSnakeBodyTexture();

        // Создает текстуру горошины
        _assetManager.CreatePelletTexture();

        // Создает анимацию Pac-Man для всех направлений
        System.Numerics.Vector2[] directions =
        {
            new System.Numerics.Vector2(1, 0),   // Вправо
            new System.Numerics.Vector2(-1, 0),  // Влево
            new System.Numerics.Vector2(0, 1),   // Вниз
            new System.Numerics.Vector2(0, -1)   // Вверх
        };

        // Создает 30 кадров анимации для каждого направления
        for (int frame = 0; frame < 30; frame++)
        {
            foreach (var direction in directions)
            {
                _assetManager.CreateSimplePacManHeadTexture(frame, direction);
            }
            _assetManager.CreatePowerPelletTexture(frame);
        }
    }
    // Главный игровой цикл
    public void Run()
    {
        while (_window.IsOpen)
        {
            // Обработка событий окна
            _window.DispatchEvents();
            // Расчет времени между кадрами
            var deltaTime = _clock.Restart().AsSeconds();
            // Обработка ввода, обновление состояния, отрисовка
            _stateManager.HandleInput();
            _stateManager.Update(deltaTime);

            _window.Clear(Configuration.BackgroundColor);
            _stateManager.Draw(_window);
            _window.Display();
        }

        // Очистка ресурсов при выходе
        _audioManager.Dispose();
        //_soundEffectManager.Dispose();
        _assetManager.Dispose();
    }
}