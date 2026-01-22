using SFML.Graphics;
using SFML.System;
using SnakeGame.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace SnakeGame.States;
// Состояние, представляющее экран загрузки сохраненных игр
// Позволяет пользователю выбирать, просматривать и удалять сохранения
public class LoadGameState : IGameState
{
    private readonly GameStateManager _stateManager;
    private readonly AssetManager _assetManager;
    private readonly InputManager _inputManager;
    private readonly SaveManager _saveManager;

    private Text _titleText;
    private List<Text> _saveFilesTexts; // Текстовые элементы для отображения сохранений
    private List<string> _saveFiles;    // Список имен файлов сохранений
    private int _selectedSave;          // Индекс выбранного сохранения
    private RectangleShape[] _walls;    // Декоративные стены
    private Text _emptyText;            // Текст при отсутствии сохранений
    private Text _hintText;             // Подсказки управления

    public LoadGameState(GameStateManager stateManager, AssetManager assetManager,
                        InputManager inputManager, SaveManager saveManager)
    {
        _stateManager = stateManager;
        _assetManager = assetManager;
        _inputManager = inputManager;
        _saveManager = saveManager;
    }

    public void Initialize()
    {
        Console.WriteLine("=== ИНИЦИАЛИЗАЦИЯ LoadGameState ===");
        _saveFiles = _saveManager.GetSaveFiles(); // Получаем список файлов сохранени
        _selectedSave = 0; // Выбираем первый элемент по умолчанию

        CreateWalls();

        SFML.Graphics.Font font = _assetManager.HasFont("main")
            ? _assetManager.GetFont("main")
            : new SFML.Graphics.Font("C:/Windows/Fonts/arial.ttf");
        // Заголовок экрана
        string titleStr = "ВЫБЕРИТЕ СОХРАНЕНИЕ";
        _titleText = new Text(titleStr, font, 48);
        _titleText.Position = new Vector2f(
            Configuration.WindowWidth / 2 - GetTextWidth(titleStr, font, 48) / 2,
            50);
        _titleText.FillColor = new SFML.Graphics.Color(0, 255, 0);
        _titleText.Style = Text.Styles.Bold;
        // Обновляем список отображаемых сохранений
        UpdateSaveFilesList(font);
        // Подсказки управления внизу экрана
        _hintText = new Text("ENTER: Загрузить  DELETE: Удалить  ESC: Назад", font, Configuration.SmallFontSize);
        _hintText.Position = new Vector2f(
            Configuration.WindowWidth / 2 - GetTextWidth("ENTER: Загрузить  DELETE: Удалить  ESC: Назад", font, Configuration.SmallFontSize) / 2,
            Configuration.WindowHeight - 50);
        _hintText.FillColor = new SFML.Graphics.Color(200, 200, 255);
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
    // Обновляет список отображаемых сохранений
    private void UpdateSaveFilesList(SFML.Graphics.Font font)
    {
        _saveFilesTexts = new List<Text>();

        if (_saveFiles.Count == 0)
        {
            _emptyText = new Text("Нет сохраненных игр", font, Configuration.MenuFontSize);
            _emptyText.Position = new Vector2f(
                Configuration.WindowWidth / 2 - GetTextWidth("Нет сохраненных игр", font, Configuration.MenuFontSize) / 2,
                Configuration.WindowHeight / 2 - 20);
            _emptyText.FillColor = new SFML.Graphics.Color(255, 100, 100);
            return;
        }

        float startY = 150; // Начальная Y-координата
        float spacing = 50; // Расстояние между элементами
        // Создаем текстовые элементы для каждого сохранения
        for (int i = 0; i < _saveFiles.Count; i++)
        {
            string fileName = _saveFiles[i];
            string displayName = Path.GetFileNameWithoutExtension(fileName);
            // Пытаемся загрузить метаданные сохранения для красивого отображения
            try
            {
                var save = _saveManager.LoadGame(fileName);
                if (save != null)
                {
                    // Форматируем отображаемое имя: "Имя - Счет"
                    displayName = $"{save.SaveName} - Счет: {save.Score} - {save.SaveDate}";
                }
            }
            catch { }

            float textWidth = GetTextWidth(displayName, font, Configuration.MenuFontSize);

            var text = new Text(displayName, font, Configuration.MenuFontSize);
            text.Position = new Vector2f(
                Configuration.WindowWidth / 2 - textWidth / 2,
                startY + i * spacing);
            // Выделяем выбранный элемент
            if (i == _selectedSave)
            {
                text.FillColor = new SFML.Graphics.Color(255, 255, 0); // Желтый для выбранного
                text.OutlineColor = new SFML.Graphics.Color(150, 150, 0);
                text.OutlineThickness = 3;
                text.Style = Text.Styles.Bold;
            }
            else
            {
                text.FillColor = new SFML.Graphics.Color(200, 200, 0); // Бледно-желтый для остальных
                text.OutlineColor = new SFML.Graphics.Color(50, 50, 50);
                text.OutlineThickness = 1;
                text.Style = Text.Styles.Regular;
            }

            _saveFilesTexts.Add(text);
        }
    }
    // Обрабатывает пользовательский ввод на экране загрузки
    public void HandleInput()
    {
        // Если нет сохранений, только ESC работает
        if (_saveFiles.Count == 0)
        {
            if (_inputManager.IsKeyJustPressed(SFML.Window.Keyboard.Key.Escape))
            {
                _stateManager.ChangeState(Enums.GameStateType.Menu);
            }
            return;
        }
        // Стрелка вверх - выбор предыдущего сохранения
        if (_inputManager.IsKeyJustPressed(SFML.Window.Keyboard.Key.Up))
        {
            _selectedSave = (_selectedSave - 1 + _saveFiles.Count) % _saveFiles.Count;
            UpdateSaveFilesList(_assetManager.HasFont("main") ? _assetManager.GetFont("main") : new SFML.Graphics.Font("C:/Windows/Fonts/arial.ttf"));
        }
        // Стрелка вниз - выбор следующего сохранения
        if (_inputManager.IsKeyJustPressed(SFML.Window.Keyboard.Key.Down))
        {
            _selectedSave = (_selectedSave + 1) % _saveFiles.Count;
            UpdateSaveFilesList(_assetManager.HasFont("main") ? _assetManager.GetFont("main") : new SFML.Graphics.Font("C:/Windows/Fonts/arial.ttf"));
        }
        // ENTER - загрузить выбранное сохранение
        if (_inputManager.IsKeyJustPressed(SFML.Window.Keyboard.Key.Enter))
        {
            if (_selectedSave >= 0 && _selectedSave < _saveFiles.Count)
            {
                Console.WriteLine($"Выбрано сохранение: {_saveFiles[_selectedSave]}");
                _stateManager.ChangeState(Enums.GameStateType.Game, _saveFiles[_selectedSave]);
            }
        }
        // DELETE - удалить выбранное сохранение
        if (_inputManager.IsKeyJustPressed(SFML.Window.Keyboard.Key.Delete))
        {
            if (_selectedSave >= 0 && _selectedSave < _saveFiles.Count)
            {
                _saveManager.DeleteSave(_saveFiles[_selectedSave]);
                _saveFiles = _saveManager.GetSaveFiles();
                _selectedSave = Math.Min(_selectedSave, Math.Max(0, _saveFiles.Count - 1));
                UpdateSaveFilesList(_assetManager.HasFont("main") ? _assetManager.GetFont("main") : new SFML.Graphics.Font("C:/Windows/Fonts/arial.ttf"));
            }
        }
        // ESC - вернуться в главное меню
        if (_inputManager.IsKeyJustPressed(SFML.Window.Keyboard.Key.Escape))
        {
            _stateManager.ChangeState(Enums.GameStateType.Menu);
        }
    }
    // Обновляет анимации на экране загрузки
    public void Update(float deltaTime)
    {
        // Добавляем эффект пульсации к выбранному элементу
        if (_saveFilesTexts != null && _selectedSave < _saveFilesTexts.Count)
        {
            var selectedText = _saveFilesTexts[_selectedSave];
            float scale = 1.0f + (float)Math.Sin(Environment.TickCount * 0.003) * 0.1f;
            selectedText.Scale = new Vector2f(scale, scale);
        }
    }
    // Отрисовывает экран загрузки сохранений
    public void Draw(RenderTarget target)
    {
        // Темно-синий фон
        target.Clear(new SFML.Graphics.Color(0, 0, 40));
        // Рисуем стены
        foreach (var wall in _walls)
        {
            target.Draw(wall);
        }
        // Рисуем заголовок
        target.Draw(_titleText);
        // Если сохранений нет, показываем сообщение
        if (_saveFiles.Count == 0 && _emptyText != null)
        {
            target.Draw(_emptyText);
        }
        else
        {
            // Рисуем список сохранений
            foreach (var text in _saveFilesTexts)
            {
                target.Draw(text);
            }
        }
        // Рисуем подсказки
        target.Draw(_hintText);
    }
}