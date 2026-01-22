using SFML.Graphics;
using SFML.System;
using SnakeGame.Core;
using System;
using System.Collections.Generic;

namespace SnakeGame.States;
// Состояние, представляющее экран завершения игры
// Показывается, когда змейка умирает (столкновение со стеной или собой)
public class GameOverState : IGameState
{
    private readonly GameStateManager _stateManager;
    private readonly AssetManager _assetManager;
    private readonly InputManager _inputManager;
    private readonly AudioManager _audioManager;
    private int _finalScore;

    private Text _gameOverText;
    private Text _scoreText;
    private Text _restartText;
    private Text _menuText;
    private RectangleShape[] _walls;
    private Sprite _background;

    public GameOverState(GameStateManager stateManager, AssetManager assetManager,
                    InputManager inputManager, AudioManager audioManager)
    {
        _stateManager = stateManager;
        _assetManager = assetManager;
        _inputManager = inputManager;
        _audioManager = audioManager;
    }
    // Устанавливает финальный счет, полученный из GameState
    public void SetFinalScore(int score)
    {
        _finalScore = score;
    }
    // Инициализирует экран завершения игры
    public void Initialize()
    {
        // Загружает фон для Game Over
        _background = _assetManager.GetBackground("gameover");

        CreateWalls();

        SFML.Graphics.Font font = _assetManager.HasFont("main")
            ? _assetManager.GetFont("main")
            : new SFML.Graphics.Font("C:/Windows/Fonts/arial.ttf");

        string gameOverStr = "ИГРА ОКОНЧЕНА";
        _gameOverText = new Text(gameOverStr, font, Configuration.GameOverFontSize);
        _gameOverText.Position = new Vector2f(
            Configuration.WindowWidth / 2 - GetTextWidth(gameOverStr, font, Configuration.GameOverFontSize) / 2,
            80);
        _gameOverText.FillColor = new SFML.Graphics.Color(255, 50, 50);
        _gameOverText.Style = Text.Styles.Bold;
        _gameOverText.OutlineColor = new SFML.Graphics.Color(150, 0, 0);
        _gameOverText.OutlineThickness = 4;

        string scoreStr = $"Ваш счет: {_finalScore}";
        _scoreText = new Text(scoreStr, font, Configuration.ScoreFontSize);
        _scoreText.Position = new Vector2f(
            Configuration.WindowWidth / 2 - GetTextWidth(scoreStr, font, Configuration.ScoreFontSize) / 2,
            220);
        _scoreText.FillColor = new SFML.Graphics.Color(255, 215, 0);
        _scoreText.OutlineColor = new SFML.Graphics.Color(100, 100, 0);
        _scoreText.OutlineThickness = 2;

        string restartStr = "Нажмите R для перезапуска";
        _restartText = new Text(restartStr, font, Configuration.MenuFontSize);
        _restartText.Position = new Vector2f(
            Configuration.WindowWidth / 2 - GetTextWidth(restartStr, font, Configuration.MenuFontSize) / 2,
            300);
        _restartText.FillColor = new SFML.Graphics.Color(0, 255, 0);

        string menuStr = "Нажмите ESC для выхода в меню";
        _menuText = new Text(menuStr, font, Configuration.MenuFontSize);
        _menuText.Position = new Vector2f(
            Configuration.WindowWidth / 2 - GetTextWidth(menuStr, font, Configuration.MenuFontSize) / 2,
            370);
        _menuText.FillColor = new SFML.Graphics.Color(200, 200, 200);
    }
    // Вспомогательный метод для расчета ширины текста
    private float GetTextWidth(string text, SFML.Graphics.Font font, uint fontSize)
    {
        var tempText = new Text(text, font, fontSize);
        return tempText.GetLocalBounds().Width;
    }
    // Создает декоративные стены по краям экрана (визуальный элемент)
    private void CreateWalls()
    {
        _walls = new RectangleShape[4];
        // Верхняя стена
        var wall1 = new RectangleShape(new Vector2f(Configuration.WindowWidth, Configuration.WallWidth));
        wall1.Position = new Vector2f(0, 0);
        wall1.FillColor = new SFML.Graphics.Color(139, 0, 0);
        _walls[0] = wall1;
        // Нижняя стена
        var wall2 = new RectangleShape(new Vector2f(Configuration.WindowWidth, Configuration.WallWidth));
        wall2.Position = new Vector2f(0, Configuration.WindowHeight - Configuration.WallWidth);
        wall2.FillColor = new SFML.Graphics.Color(139, 0, 0);
        _walls[1] = wall2;
        // Левая стена
        var wall3 = new RectangleShape(new Vector2f(Configuration.WallWidth, Configuration.WindowHeight));
        wall3.Position = new Vector2f(0, 0);
        wall3.FillColor = new SFML.Graphics.Color(139, 0, 0);
        _walls[2] = wall3;
        // Правая стена
        var wall4 = new RectangleShape(new Vector2f(Configuration.WallWidth, Configuration.WindowHeight));
        wall4.Position = new Vector2f(Configuration.WindowWidth - Configuration.WallWidth, 0);
        wall4.FillColor = new SFML.Graphics.Color(139, 0, 0);
        _walls[3] = wall4;
    }
    // Обрабатывает пользовательский ввод на экране завершения игры
    public void HandleInput()
    {
        // R - перезапустить игру
        if (_inputManager.IsKeyJustPressed(SFML.Window.Keyboard.Key.R))
        {
            //_soundEffectManager.PlaySoundEffect("click", 40f);
            _stateManager.ChangeState(Enums.GameStateType.Game);
        }
        // ESC - вернуться в главное меню
        if (_inputManager.IsKeyJustPressed(SFML.Window.Keyboard.Key.Escape))
        {
            //_soundEffectManager.PlaySoundEffect("menu_select", 30f);
            _stateManager.ChangeState(Enums.GameStateType.Menu);
        }
    }
    // Обновляет анимации на экране завершения игры
    public void Update(float deltaTime)
    {
        // Создаем пульсирующий эффект для текста перезапуска
        float alpha = 200 + (int)(Math.Sin(Environment.TickCount * 0.005) * 55);
        _restartText.FillColor = new SFML.Graphics.Color(
            _restartText.FillColor.R,
            _restartText.FillColor.G,
            _restartText.FillColor.B,
            (byte)alpha);
    }
    // Отрисовывает экран завершения игры
    public void Draw(RenderTarget target)
    {
        // Рисует фон если есть
        if (_background != null)
        {
            target.Draw(_background);
        }
        else
        {
            target.Clear(new SFML.Graphics.Color(30, 0, 0));
        }

        // Добавляет затемнение для лучшей читаемости текста
        if (_background != null)
        {
            var overlay = new RectangleShape(new Vector2f(Configuration.WindowWidth, Configuration.WindowHeight));
            overlay.FillColor = new SFML.Graphics.Color(0, 0, 0, 180);
            target.Draw(overlay);
        }
        // Рисует стены
        foreach (var wall in _walls)
        {
            target.Draw(wall);
        }
        // Рисует все текстовые элементы
        target.Draw(_gameOverText);
        target.Draw(_scoreText);
        target.Draw(_restartText);
        target.Draw(_menuText);
        // Добавляет мотивирующую подсказку
        var font = _assetManager.HasFont("main")
            ? _assetManager.GetFont("main")
            : new SFML.Graphics.Font("C:/Windows/Fonts/arial.ttf");

        var hintText = new Text("Соберите больше горошин в следующий раз!", font, 24);
        hintText.Position = new Vector2f(
            Configuration.WindowWidth / 2 - GetTextWidth("Соберите больше горошин в следующий раз!", font, 24) / 2,
            430);
        hintText.FillColor = new SFML.Graphics.Color(200, 200, 200, 150);
        target.Draw(hintText);
    }
}