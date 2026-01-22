using SFML.Graphics;
using SFML.System;
using SnakeGame.Core;
using SnakeGame.Entities;
using System;
using Color = SFML.Graphics.Color;
using Font = SFML.Graphics.Font;

namespace SnakeGame.States;
// Состояние, представляющее основной игровой процесс
// Управляет логикой игры, взаимодействием змейки и еды, UI игрового экрана
public class GameState : IGameState
{
    private readonly GameStateManager _stateManager;  // Управление состояниями игры
    private readonly AssetManager _assetManager;      // Управление ресурсами (текстуры, шрифты)
    private readonly InputManager _inputManager;      // Обработка пользовательского ввода
    private readonly SaveManager _saveManager;        // Управление сохранениями игры
    private readonly AudioManager _audioManager;      // Управление звуком и музыкой
    private Snake _snake;                             // Экземпляр змейки
    private Food _food;                               // Экземпляр еды
    private Text _scoreText;                          // Текст для отображения текущего счета
    private Text _speedText;                          // Текст для отображения текущей скорости
    private Text _pauseText;                          // Текст "ПАУЗА"
    private Text _saveText;                           // Текст "Игра сохранена!"
    private Text _controlHintText;                    // Подсказки по управлению внизу экрана
    private Text _powerPelletText;                    // Текст "СУПЕР СКОРОСТЬ!"
    // Таймеры и состояние UI
    private SFML.System.Clock _saveTextClock;         // Таймер для отображения сообщения о сохранении
    private SFML.System.Clock _powerPelletTextClock;  // Таймер для отображения эффекта супер-горошины
    private bool _showSaveText;                       // Флаг: показывать ли сообщение о сохранении
    private bool _showPauseText;                      // Флаг: показывать ли текст паузы 
    private bool _showPowerPelletText;                // Флаг: показывать ли эффект супер-горошины
    private RectangleShape[] _walls;                  // Декоративные стены по краям игрового поля
    private float _saveTextAlpha;                     // Прозрачность текста сохранения (для плавного исчезновения)
    // Состояние загрузки игры
    private bool _isGameLoaded = false;               // Флаг: была ли игра загружена из сохранения
    private string _saveFileToLoad = null;            // Имя файла сохранения для загрузки


    public GameState(GameStateManager stateManager, AssetManager assetManager,
                    InputManager inputManager, SaveManager saveManager,
                    AudioManager audioManager)
    {
        _stateManager = stateManager;
        _assetManager = assetManager;
        _inputManager = inputManager;
        _saveManager = saveManager;
        _audioManager = audioManager;
    }

    public void Initialize()
    {
        Console.WriteLine("=== ИНИЦИАЛИЗАЦИЯ GameState ===");

        // Создаем UI элементы
        CreateWalls();
        CreateUI();

        _saveTextClock = new SFML.System.Clock();
        _powerPelletTextClock = new SFML.System.Clock();
        _showSaveText = false;          // Сообщение о сохранении скрыто
        _showPauseText = false;         // Текст паузы скрыт
        _showPowerPelletText = false;   // Эффект супер-горошины скрыт
        _saveTextAlpha = 255;           // Полная непрозрачность (если потребуется)

        Console.WriteLine("GameState инициализирован (UI создан)");
    }
    // Устанавливает имя файла сохранения для загрузки
    // Вызывается из GameStateManager при переходе из LoadGameState
    public void SetSaveFileToLoad(string saveFile)
    {
        _saveFileToLoad = saveFile;
        Console.WriteLine($"Установлен файл для загрузки: {saveFile}");
    }
    // Начинает новую игру
    public void StartNewGame()
    {
        Console.WriteLine("=== НАЧАЛО НОВОЙ ИГРЫ ===");
        _isGameLoaded = false;   // Сбрасывает флаг загрузки
        _saveFileToLoad = null;  // Очищает имя файла для загрузки

        _snake = new Snake(_assetManager);
        _food = new Food(_assetManager);
        UpdateUI(0); // Обновляет UI с нулевым счетом
        Console.WriteLine("Новая игра начата");
    }
    // Загружает игру из файла сохранения
    public void LoadGame(string saveFile)
    {
        Console.WriteLine($"=== ЗАГРУЗКА СОХРАНЕНИЯ ===");
        Console.WriteLine($"Файл: {saveFile}");
        // Загружает данные сохранения через SaveManager
        var save = _saveManager.LoadGame(saveFile);
        if (save == null)
        {
            Console.WriteLine("Ошибка: не удалось загрузить сохранение, начинаем новую игру");
            StartNewGame();
            return;
        }

        Console.WriteLine($"Данные сохранения:");
        Console.WriteLine($"- Дата: {save.SaveDate}");
        Console.WriteLine($"- Счет: {save.Score}");
        Console.WriteLine($"- Длина змеи: {save.SnakeBody.Count}");
        Console.WriteLine($"- Направление: {save.SnakeDirection}");
        Console.WriteLine($"- Позиция еды: {save.FoodPosition}");
        Console.WriteLine($"- Супер-еда: {save.IsPowerPellet}");

        // Создает новую змею и еду
        _snake = new Snake(_assetManager);
        _food = new Food(_assetManager);

        // Загружаем тело змеи из сохранения
        _snake.Body.Clear();
        foreach (var segment in save.SnakeBody)
        {
            // Конвертирует Vector2 в System.Numerics.Vector2
            _snake.Body.Add(new System.Numerics.Vector2(segment.X, segment.Y));
        }

        // Устанавливает направление движения из сохранения
        _snake.ChangeDirection(new System.Numerics.Vector2(
            save.SnakeDirection.X,
            save.SnakeDirection.Y));

        // Устанавливает позицию и тип еды из сохранения
        _food.SetPosition(
            new System.Numerics.Vector2(save.FoodPosition.X, save.FoodPosition.Y),
            save.IsPowerPellet);

        // Рассчитывает скорость на основе счета из сохранения
        float speedReduction = Math.Min(
            save.Score * Configuration.SpeedIncrement,
            Configuration.InitialGameSpeed - Configuration.MinGameSpeed);

        _snake.CurrentSpeed = Configuration.InitialGameSpeed - speedReduction;

        Console.WriteLine($"Рассчитанная скорость: {_snake.CurrentSpeed}");

        // Обновляет UI с загруженным счетом
        UpdateUI(save.Score);

        _isGameLoaded = true;
        Console.WriteLine("=== ЗАГРУЗКА ЗАВЕРШЕНА ===");
    }
    // Создает UI элементы (текстовые поля)
    private void CreateUI()
    {
        // Получает шрифт или использует системный по умолчанию
        Font font = _assetManager.HasFont("main")
            ? _assetManager.GetFont("main")
            : new Font("C:/Windows/Fonts/arial.ttf");
        // Текст для отображения счета 
        _scoreText = new Text("Горошины: 0", font, Configuration.ScoreFontSize);
        _scoreText.Position = new Vector2f(Configuration.WallWidth + 10, Configuration.WallWidth + 10);
        _scoreText.FillColor = new Color(255, 255, 255);  // Белый цвет
        // Текст для отображения скорости
        _speedText = new Text("Скорость: 1.0x", font, Configuration.SmallFontSize);
        _speedText.Position = new Vector2f(Configuration.WallWidth + 10, Configuration.WallWidth + 45);
        _speedText.FillColor = new Color(200, 200, 255);  // Светло-синий
        // Текст "ПАУЗА"
        _pauseText = new Text("ПАУЗА", font, 72);
        _pauseText.Position = new Vector2f(
            Configuration.WindowWidth / 2 - 140,
            Configuration.WindowHeight / 2 - 40);
        _pauseText.FillColor = new Color(255, 255, 0, 200);  // Желтый с альфа-каналом
        _pauseText.Style = Text.Styles.Bold;
        // Всплывающий текст "Игра сохранена!"
        _saveText = new Text("Игра сохранена!", font, Configuration.SmallFontSize);
        _saveText.Position = new Vector2f(Configuration.WindowWidth - 200, Configuration.WallWidth + 10);
        _saveText.FillColor = new Color(0, 255, 0);  // Зеленый
        // Текст эффекта супер-горошины
        _powerPelletText = new Text("СУПЕР СКОРОСТЬ!", font, 32);
        _powerPelletText.Position = new Vector2f(
            Configuration.WindowWidth / 2 - 150,
            Configuration.WindowHeight / 2 - 16);
        _powerPelletText.FillColor = new Color(255, 255, 0);  // Желтый
        _powerPelletText.Style = Text.Styles.Bold;
        _powerPelletText.OutlineColor = new Color(255, 0, 0);  // Красная обводка
        _powerPelletText.OutlineThickness = 3;
        // Подсказки по управлению внизу экрана
        _controlHintText = new Text("ESC: Меню  F5: Сохранить  P: Пауза  M: Музыка", font, Configuration.SmallFontSize);
        _controlHintText.Position = new Vector2f(
            Configuration.WindowWidth / 2 - 250,
            Configuration.WindowHeight - Configuration.WallWidth - 40);
        _controlHintText.FillColor = new Color(200, 200, 200, 180);  // Серый полупрозрачный
    }
    // Создает декоративные стены по краям игрового поля
    private void CreateWalls()
    {
        _walls = new RectangleShape[4];
        // Верхняя стена
        var wall1 = new RectangleShape(new Vector2f(Configuration.WindowWidth, Configuration.WallWidth));
        wall1.Position = new Vector2f(0, 0);
        wall1.FillColor = Configuration.WallColor;
        _walls[0] = wall1;
        // Нижняя стена
        var wall2 = new RectangleShape(new Vector2f(Configuration.WindowWidth, Configuration.WallWidth));
        wall2.Position = new Vector2f(0, Configuration.WindowHeight - Configuration.WallWidth);
        wall2.FillColor = Configuration.WallColor;
        _walls[1] = wall2;
        // Левая стена
        var wall3 = new RectangleShape(new Vector2f(Configuration.WallWidth, Configuration.WindowHeight));
        wall3.Position = new Vector2f(0, 0);
        wall3.FillColor = Configuration.WallColor;
        _walls[2] = wall3;
        // Правая стена
        var wall4 = new RectangleShape(new Vector2f(Configuration.WallWidth, Configuration.WindowHeight));
        wall4.Position = new Vector2f(Configuration.WindowWidth - Configuration.WallWidth, 0);
        wall4.FillColor = Configuration.WallColor;
        _walls[3] = wall4;
    }
    // Обновляет UI элементы (счет и скорость)
    private void UpdateUI(int score)
    {
        // Обновляем текст счета
        _scoreText.DisplayedString = $"Горошины: {score}";
        // Обновляем текст скорости
        if (_snake != null)
        {
            float speedMultiplier = Configuration.InitialGameSpeed / _snake.CurrentSpeed;
            _speedText.DisplayedString = $"Скорость: {speedMultiplier:F1}x";
        }
    }
    // Обрабатывает пользовательский ввод во время игры
    public void HandleInput()
    {
        // Проверяет, есть ли игра для загрузки при первом входе в состояние
        if (_saveFileToLoad != null && !_isGameLoaded)
        {
            LoadGame(_saveFileToLoad); // Загружает игру
            _saveFileToLoad = null;    // Очищает имя файла
        }

        if (_snake == null) return;  // Если змейка не создана, выходит
        // Управление змейкой (только если не на паузе)
        if (!_snake.IsPaused)
        {
            if (_inputManager.IsKeyPressed(SFML.Window.Keyboard.Key.Up))
                _snake.ChangeDirection(new System.Numerics.Vector2(0, -1));  
            if (_inputManager.IsKeyPressed(SFML.Window.Keyboard.Key.Down))
                _snake.ChangeDirection(new System.Numerics.Vector2(0, 1));
            if (_inputManager.IsKeyPressed(SFML.Window.Keyboard.Key.Left))
                _snake.ChangeDirection(new System.Numerics.Vector2(-1, 0));
            if (_inputManager.IsKeyPressed(SFML.Window.Keyboard.Key.Right))
                _snake.ChangeDirection(new System.Numerics.Vector2(1, 0));
        }
        // ESC - возврат в главное меню
        if (_inputManager.IsKeyJustPressed(SFML.Window.Keyboard.Key.Escape))
        {
            // _soundEffectManager.PlaySoundEffect("click", 30f);
            _stateManager.ChangeState(Enums.GameStateType.Menu);
        }
        // P - пауза/продолжение игры
        if (_inputManager.IsKeyJustPressed(SFML.Window.Keyboard.Key.P))
        {
            _snake.IsPaused = !_snake.IsPaused;
            _showPauseText = _snake.IsPaused;

            //_soundEffectManager.PlaySoundEffect("click", 40f);
            // Управление музыкой при паузе
            if (_snake.IsPaused)
            {
                _audioManager.PauseMusic();  // Приостанавливаем музыку на паузе
            }
            else
            {
                _audioManager.ResumeMusic();  // Возобновляем музыку при продолжении
            }
        }
        // M - вкл/выкл музыку
        if (_inputManager.IsKeyJustPressed(SFML.Window.Keyboard.Key.M))
        {
            var status = _audioManager.GetMusicStatus();
            if (status == SFML.Audio.SoundStatus.Playing)
                _audioManager.PauseMusic();                   // Если играет - пауза
            else if (status == SFML.Audio.SoundStatus.Paused)
                _audioManager.ResumeMusic();                  // Если на паузе - возобновить
        }
        // F5 - быстрое сохранение игры
        if (_inputManager.IsKeyJustPressed(SFML.Window.Keyboard.Key.F5))
        {
            if (_snake != null && _food != null)
            {
                // Сохраняем игру с именем, содержащим время
                _saveManager.SaveGame(_snake, _food, $"save_{DateTime.Now:HHmmss}");
                // Настраиваем отображение сообщения о сохранении
                _showSaveText = true;     // Показываем сообщение
                _saveTextAlpha = 255;     // Полная непрозрачность
                _saveTextClock.Restart(); // Запускаем таймер
                _saveText.DisplayedString = "Игра сохранена!";
                //_soundEffectManager.PlaySoundEffect("click", 40f);
            }
        }
        // F9 - переход в экран загрузки сохранений
        if (_inputManager.IsKeyJustPressed(SFML.Window.Keyboard.Key.F9))
        {
            //_soundEffectManager.PlaySoundEffect("menu_select", 30f);
            _stateManager.ChangeState(Enums.GameStateType.LoadGame);
        }
    }
    // Обновляет логику игры (вызывается каждый кадр)
    public void Update(float deltaTime)
    {
        if (_snake == null) return;  // Если змейка не создана, выходим
        // Управление отображением сообщения о сохранении
        if (_showSaveText)
        {
            float elapsed = _saveTextClock.ElapsedTime.AsSeconds();
            if (elapsed > 1.5f)  // Через 1.5 секунды начинаем исчезновение
            {
                // Уменьшает прозрачность для плавного исчезновения
                _saveTextAlpha = Math.Max(0, _saveTextAlpha - deltaTime * 200);
                if (_saveTextAlpha <= 0)
                {
                    _showSaveText = false;  // Полностью скрываем сообщение
                }
            }
            // Применяет новую прозрачность к цвету текста
            _saveText.FillColor = new Color(_saveText.FillColor.R, _saveText.FillColor.G, _saveText.FillColor.B, (byte)_saveTextAlpha);
        }
        // Управление отображением эффекта супер-горошины (1 секунда)
        if (_showPowerPelletText && _powerPelletTextClock.ElapsedTime.AsSeconds() > 1)
        {
            _showPowerPelletText = false;
        }
        // Обновление игровой логики (только если не на паузе)
        if (!_snake.IsPaused)
        {
            // Обновляет состояние змейки и еды
            _snake.Update(deltaTime);
            _food.Update(deltaTime);
            // Проверяет столкновение змейки с едой
            if (_snake.CheckFoodCollision(_food.Position))
            {
                _snake.Grow();  // Увеличиваем змейку

                if (_food.IsPowerPellet)  // Если это супер-горошина
                {
                    _snake.ActivatePowerPellet();  // Активируем эффект скорости
                    _showPowerPelletText = true;   // Показываем текст эффекта
                    _powerPelletTextClock.Restart();
                    //_soundEffectManager.PlaySoundEffect("powerup", 50f);
                }
                else  // Обычная горошина
                {
                    _snake.IncreaseSpeed();  // Увеличиваем скорость
                    //_soundEffectManager.PlaySoundEffect("eat", 60f);
                }

                _food.Respawn();  // Генерируем новую еду

                // Обновляет UI после съедения еды
                UpdateUI(_snake.Score);
            }
            // Проверяет, жива ли змейка
            if (!_snake.IsAlive)
            {
                // Переходит в состояние завершения игры с финальным счетом
                _stateManager.ChangeState(Enums.GameStateType.GameOver, _snake.Score);
            }
        }
    }
    // Отрисовывает игровое состояние на экране
    public void Draw(RenderTarget target)
    {
        // Рисует черный фон игровой области
        var gameArea = new RectangleShape(new Vector2f(
            Configuration.WindowWidth - Configuration.WallWidth * 2,
            Configuration.WindowHeight - Configuration.WallWidth * 2));
        gameArea.Position = new Vector2f(Configuration.WallWidth, Configuration.WallWidth);
        gameArea.FillColor = Color.Black;
        target.Draw(gameArea);
        // Рисует декоративные стены
        foreach (var wall in _walls)
        {
            target.Draw(wall);
        }
        // Рисует змейку (если создана)
        if (_snake != null)
        {
            target.Draw(_snake);
        }
        // Рисует еду (если создана)
        if (_food != null)
        {
            target.Draw(_food);
        }
        // Рисует основные UI элементы
        target.Draw(_scoreText);
        target.Draw(_speedText);
        target.Draw(_controlHintText);
        // Рисует сообщение о сохранении (если нужно)
        if (_showSaveText)
        {
            target.Draw(_saveText);
        }
        // Рисует эффект супер-горошины (если нужно)
        if (_showPowerPelletText)
        {
            // Создает пульсирующий эффект прозрачности
            float alpha = 200 + (int)(Math.Sin(Environment.TickCount * 0.01) * 55);
            _powerPelletText.FillColor = new Color(
                _powerPelletText.FillColor.R,
                _powerPelletText.FillColor.G,
                _powerPelletText.FillColor.B,
                (byte)alpha);
            // Рисует полупрозрачный фон для текста
            var background = new RectangleShape(new Vector2f(320, 60));
            background.Position = new Vector2f(
                Configuration.WindowWidth / 2 - 160,
                Configuration.WindowHeight / 2 - 30);
            background.FillColor = new Color(0, 0, 0, 150);
            target.Draw(background);  // Рисует текст эффекта
            target.Draw(_powerPelletText);
        }
        // Рисует экран паузы (если игра на паузе)
        if (_showPauseText)
        {
            // Полупрозрачный фон для текста паузы
            var pauseBackground = new RectangleShape(new Vector2f(300, 100));
            pauseBackground.Position = new Vector2f(
                Configuration.WindowWidth / 2 - 150,
                Configuration.WindowHeight / 2 - 50);
            pauseBackground.FillColor = new Color(0, 0, 0, 150);
            target.Draw(pauseBackground);
            target.Draw(_pauseText);  // Текст "ПАУЗА"
        }
    }
}