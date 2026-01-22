using SFML.Graphics;
using SFML.System;
using SnakeGame.Core;
using SnakeGame.Enums;
using System;
using System.Collections.Generic;

namespace SnakeGame.States;
// Состояние, представляющее главное меню игры
// Первый экран, который видит пользователь при запуске игры
public class MenuState : IGameState
{
    private readonly GameStateManager _stateManager;
    private readonly AssetManager _assetManager;
    private readonly InputManager _inputManager;
    private readonly AudioManager _audioManager;

    private Text _titleText;
    private Text _subtitleText;
    private List<Text> _menuTexts;     // Все текстовые элементы меню
    private int _selectedOption;       // Индекс выбранного пункта меню
    private readonly string[] _options = { "Новая игра", "Загрузить игру", "Справка", "Выход" };
    private RectangleShape[] _walls;  // Декоративные стены
    private Sprite _background;       // Фоновое изображение

    public MenuState(GameStateManager stateManager, AssetManager assetManager,
                     InputManager inputManager, AudioManager audioManager)
    {
        _stateManager = stateManager;
        _assetManager = assetManager;
        _inputManager = inputManager;
        _audioManager = audioManager;
        _walls = new RectangleShape[4];
    }

    public void Initialize()
    {
        // Загружаем фон меню
        _background = _assetManager.GetBackground("menu");

        SFML.Graphics.Font font;
        if (_assetManager.HasFont("main"))
        {
            font = _assetManager.GetFont("main");
        }
        else
        {
            font = new SFML.Graphics.Font("C:/Windows/Fonts/arial.ttf");
        }

        // Главный заголовок "PAC-MAN"
        string mainTitleStr = "PAC-MAN";
        _titleText = new Text(mainTitleStr, font, Configuration.TitleFontSize);
        _titleText.Position = new Vector2f(
            Configuration.WindowWidth / 2 - GetTextWidth(mainTitleStr, font, Configuration.TitleFontSize) / 2,
            40);
        _titleText.FillColor = new SFML.Graphics.Color(255, 255, 0); // Классический желтый Pac-Man
        _titleText.Style = Text.Styles.Bold;
        _titleText.OutlineColor = new SFML.Graphics.Color(200, 150, 0);
        _titleText.OutlineThickness = 4;

        // Подзаголовок "ЗМЕЙКА"
        string subtitleStr = "ЗМЕЙКА";
        _subtitleText = new Text(subtitleStr, font, Configuration.MenuTitleFontSize);
        _subtitleText.Position = new Vector2f(
            Configuration.WindowWidth / 2 - GetTextWidth(subtitleStr, font, Configuration.MenuTitleFontSize) / 2,
            130);
        _subtitleText.FillColor = new SFML.Graphics.Color(255, 255, 0);
        _subtitleText.Style = Text.Styles.Bold;
        _subtitleText.OutlineColor = new SFML.Graphics.Color(200, 150, 0);
        _subtitleText.OutlineThickness = 3;

        CreateWalls();
        UpdateMenuTexts(font);
    }
    // Вспомогательный метод для расчета ширины текста
    private float GetTextWidth(string text, SFML.Graphics.Font font, uint fontSize)
    {
        var tempText = new Text(text, font, fontSize);
        return tempText.GetLocalBounds().Width;
    }
    // Создает декоративные стены по краям экрана
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
    // Обновляет текстовые элементы меню
    private void UpdateMenuTexts(SFML.Graphics.Font font)
    {
        _menuTexts = new List<Text>();

        float startY = Configuration.WindowHeight / 2 - 50;
        float spacing = 60;
        // Создает элементы меню
        for (int i = 0; i < _options.Length; i++)
        {
            string optionText = _options[i];
            float textWidth = GetTextWidth(optionText, font, Configuration.MenuFontSize);

            var text = new Text(optionText, font, Configuration.MenuFontSize);
            text.Position = new Vector2f(
                Configuration.WindowWidth / 2 - textWidth / 2,
                startY + i * spacing);
            // Выделяет выбранный элемент
            if (i == _selectedOption)
            {
                text.FillColor = new SFML.Graphics.Color(255, 255, 0);  // Ярко-желтый для выбранного
                text.OutlineColor = new SFML.Graphics.Color(150, 150, 0);
                text.OutlineThickness = 3;
                text.Style = Text.Styles.Bold;
            }
            else
            {
                text.FillColor = new SFML.Graphics.Color(200, 200, 0);  // Бледно-желтый для остальных
                text.OutlineColor = new SFML.Graphics.Color(50, 50, 50);
                text.OutlineThickness = 1;
                text.Style = Text.Styles.Regular;
            }

            _menuTexts.Add(text);
        }

        // Добавляет подсказки управления внизу экрана
        string hint1 = "Используйте ↑↓ для выбора, ENTER для подтверждения";
        var hintText1 = new Text(hint1, font, Configuration.SmallFontSize);
        hintText1.Position = new Vector2f(
            Configuration.WindowWidth / 2 - GetTextWidth(hint1, font, Configuration.SmallFontSize) / 2,
            Configuration.WindowHeight - 80);
        hintText1.FillColor = new SFML.Graphics.Color(180, 180, 255);
        _menuTexts.Add(hintText1);

        string hint2 = "F11 - Полный экран  |  M - Музыка вкл/выкл";
        var hintText2 = new Text(hint2, font, Configuration.SmallFontSize);
        hintText2.Position = new Vector2f(
            Configuration.WindowWidth / 2 - GetTextWidth(hint2, font, Configuration.SmallFontSize) / 2,
            Configuration.WindowHeight - 50);
        hintText2.FillColor = new SFML.Graphics.Color(180, 180, 255);
        _menuTexts.Add(hintText2);
    }
    // Обрабатывает пользовательский ввод в главном меню
    public void HandleInput()
    {
        bool selectionChanged = false;
        // Стрелка вверх - выбор предыдущего пункта
        if (_inputManager.IsKeyJustPressed(SFML.Window.Keyboard.Key.Up))
        {
            _selectedOption = (_selectedOption - 1 + _options.Length) % _options.Length;
            selectionChanged = true;
        }
        // Стрелка вниз - выбор следующего пункта
        if (_inputManager.IsKeyJustPressed(SFML.Window.Keyboard.Key.Down))
        {
            _selectedOption = (_selectedOption + 1) % _options.Length;
            selectionChanged = true;
        }

        if (selectionChanged)
        {
            //_soundEffectManager.PlaySoundEffect("menu_select", 30f);
            UpdateMenuTexts(_assetManager.HasFont("main") ? _assetManager.GetFont("main") : new SFML.Graphics.Font("C:/Windows/Fonts/arial.ttf"));
        }

        if (_inputManager.IsKeyJustPressed(SFML.Window.Keyboard.Key.Enter))
        {
            // Воспроизводим звук при подтверждении выбора
            //_soundEffectManager.PlaySoundEffect("click", 40f);
            HandleMenuSelection();
        }

        if (_inputManager.IsKeyJustPressed(SFML.Window.Keyboard.Key.Space))
        {
            //_soundEffectManager.PlaySoundEffect("click", 40f);
            _stateManager.ChangeState(GameStateType.Game);
        }
    }
    // Обрабатывает выбор пункта меню
    private void HandleMenuSelection()
    {
        switch ((MenuOptionType)_selectedOption)
        {
            case MenuOptionType.NewGame:
                _stateManager.ChangeState(GameStateType.Game);
                break;
            case MenuOptionType.LoadGame:
                _stateManager.ChangeState(GameStateType.LoadGame);
                break;
            case MenuOptionType.Help:
                _stateManager.ChangeState(GameStateType.Help);
                break;
            case MenuOptionType.Exit:
                Environment.Exit(0);
                break;
        }
    }
    // Обновляет анимации в главном меню
    public void Update(float deltaTime)
    {
        // Добавляет эффект пульсации к выбранному элементу меню
        if (_menuTexts != null && _selectedOption < _menuTexts.Count - 2) // -2 потому что последние 2 элемента - подсказки
        {
            var selectedText = _menuTexts[_selectedOption];
            float scale = 1.0f + (float)Math.Sin(Environment.TickCount * 0.003) * 0.1f;
            selectedText.Scale = new Vector2f(scale, scale);
        }
    }
    // Отрисовывает главное меню
    public void Draw(RenderTarget target)
    {
        // Рисует фон если есть
        if (_background != null)
        {
            target.Draw(_background);
        }
        else
        {
            target.Clear(new SFML.Graphics.Color(0, 0, 60));
        }

        // Добавляет затемнение для лучшей читаемости текста
        if (_background != null)
        {
            var overlay = new RectangleShape(new Vector2f(Configuration.WindowWidth, Configuration.WindowHeight));
            overlay.FillColor = new SFML.Graphics.Color(0, 0, 0, 150);
            target.Draw(overlay);
        }

        // Рисует стены
        foreach (var wall in _walls)
        {
            if (wall != null)
                target.Draw(wall);
        }

        // Рисует заголовки
        target.Draw(_titleText);
        target.Draw(_subtitleText);

        // Рисует все элементы меню
        if (_menuTexts != null)
        {
            foreach (var text in _menuTexts)
            {
                if (text != null)
                    target.Draw(text);
            }
        }
    }
}
