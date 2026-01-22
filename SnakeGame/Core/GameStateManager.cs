using SnakeGame.Enums;
using SnakeGame.States;
using SFML.Graphics;

namespace SnakeGame.Core;

public class GameStateManager
{
    private readonly System.Collections.Generic.Dictionary<GameStateType, IGameState> _states;
    private IGameState _currentState;
    private readonly SaveManager _saveManager;
    private readonly AudioManager _audioManager;
    private RenderWindow _window;

    public GameStateManager(AssetManager assetManager, InputManager inputManager,
                           SaveManager saveManager, AudioManager audioManager,
                           RenderWindow window)
    {
        _saveManager = saveManager;
        _audioManager = audioManager;
        _window = window;

        _states = new System.Collections.Generic.Dictionary<GameStateType, IGameState>
        {
            [GameStateType.Menu] = new MenuState(this, assetManager, inputManager, _audioManager),
            [GameStateType.Game] = new GameState(this, assetManager, inputManager, saveManager, _audioManager),
            [GameStateType.GameOver] = new GameOverState(this, assetManager, inputManager, _audioManager),
            [GameStateType.Help] = new HelpState(this, assetManager, inputManager),
            [GameStateType.LoadGame] = new LoadGameState(this, assetManager, inputManager, saveManager)
        };

        _currentState = _states[GameStateType.Menu];
        _currentState.Initialize();
    }
    // Обновляет ссылку на окно (используется при переключении полноэкранного режима)
    public void UpdateWindow(RenderWindow window)
    {
        _window = window;
        _currentState.Initialize();
    }
    // Изменяет текущее состояние игры
    public void ChangeState(GameStateType stateType, object data = null)
    {
        Console.WriteLine($"=== СМЕНА СОСТОЯНИЯ ===");
        Console.WriteLine($"Новое состояние: {stateType}");
        Console.WriteLine($"Данные: {data}");

        if (_states.TryGetValue(stateType, out var state))
        {
            // Управление музыкой при смене состояний
            if (stateType == GameStateType.Game)
            {
                // Музыка уже играет с момента запуска, просто продолжаем
                _audioManager.ResumeMusic();
            }

            _currentState = state;

            // Особые обработки для разных состояний
            if (stateType == GameStateType.Game)
            {
                if (state is GameState gameState)
                {
                    // Инициализируем состояние (создает UI)
                    gameState.Initialize();

                    if (data is string saveFile)
                    {
                        Console.WriteLine($"Загрузка игры из файла: {saveFile}");
                        // Устанавливаем файл для загрузки
                        gameState.SetSaveFileToLoad(saveFile);
                    }
                    else
                    {
                        Console.WriteLine("Начинается НОВАЯ игра");
                        // Создаем новую игру
                        gameState.StartNewGame();
                    }
                }
            }
            else if (stateType == GameStateType.GameOver && data is int score)
            {
                if (state is GameOverState gameOverState)
                {
                    gameOverState.SetFinalScore(score);
                }
                _currentState.Initialize();
            }
            else
            {
                // Для других состояний просто инициализируем
                _currentState.Initialize();
            }
        }
        else
        {
            Console.WriteLine($"Ошибка: состояние {stateType} не найдено");
        }

        Console.WriteLine($"=== СОСТОЯНИЕ ИЗМЕНЕНО ===");
    }
    // Возвращает состояние по типу
    public IGameState GetState(GameStateType stateType)
    {
        return _states.ContainsKey(stateType) ? _states[stateType] : null;
    }
    // Делегирует вызовы текущему состоянию
    public void HandleInput() => _currentState.HandleInput();
    public void Update(float deltaTime) => _currentState.Update(deltaTime);
    public void Draw(RenderTarget target) => _currentState.Draw(target);
}