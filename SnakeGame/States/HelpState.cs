using SFML.Graphics;
using SFML.System;
using SnakeGame.Core;
using System;
using System.Collections.Generic;

namespace SnakeGame.States;
// Состояние, представляющее экран справки/управления
// Показывает информацию об управлении и игровой механике
public class HelpState : IGameState
{
    private readonly GameStateManager _stateManager;
    private readonly AssetManager _assetManager;
    private readonly InputManager _inputManager;

    private Text _titleText;
    private List<Text> _helpTexts;

    public HelpState(GameStateManager stateManager, AssetManager assetManager,
                     InputManager inputManager)
    {
        _stateManager = stateManager;
        _assetManager = assetManager;
        _inputManager = inputManager;
    }

    public void Initialize()
    {
        SFML.Graphics.Font font;
        if (_assetManager.HasFont("main"))
        {
            font = _assetManager.GetFont("main");
        }
        else
        {
            font = new SFML.Graphics.Font("C:/Windows/Fonts/arial.ttf");
        }
        // Заголовок экрана справки
        string titleStr = "Управление и функции";
        _titleText = new Text(titleStr, font, 48)
        {
            Position = new Vector2f(
                Configuration.WindowWidth / 2 - GetTextWidth(titleStr, font, 48) / 2,
                50),
            FillColor = new SFML.Graphics.Color(0, 255, 0),
            Style = Text.Styles.Bold
        };

        _helpTexts = new List<Text>();

        // Создает все текстовые элементы справки
        // Управление змейкой
        var text1 = new Text("Управление змейкой:", font, 28);
        text1.Position = new Vector2f(100, 120);
        text1.FillColor = new SFML.Graphics.Color(255, 255, 0);
        _helpTexts.Add(text1);

        var text2 = new Text("↑ - Движение вверх", font, 24);
        text2.Position = new Vector2f(150, 160);
        text2.FillColor = Configuration.TextColor;
        _helpTexts.Add(text2);

        var text3 = new Text("↓ - Движение вниз", font, 24);
        text3.Position = new Vector2f(150, 190);
        text3.FillColor = Configuration.TextColor;
        _helpTexts.Add(text3);

        var text4 = new Text("← - Движение влево", font, 24);
        text4.Position = new Vector2f(150, 220);
        text4.FillColor = Configuration.TextColor;
        _helpTexts.Add(text4);

        var text5 = new Text("→ - Движение вправо", font, 24);
        text5.Position = new Vector2f(150, 250);
        text5.FillColor = Configuration.TextColor;
        _helpTexts.Add(text5);

        var text6 = new Text("Функции игры:", font, 28);
        text6.Position = new Vector2f(100, 300);
        text6.FillColor = new SFML.Graphics.Color(255, 255, 0);
        _helpTexts.Add(text6);

        var text7 = new Text("ESC - Возврат в меню", font, 24);
        text7.Position = new Vector2f(150, 340);
        text7.FillColor = Configuration.TextColor;
        _helpTexts.Add(text7);

        var text8 = new Text("F5 - Сохранить игру", font, 24);
        text8.Position = new Vector2f(150, 370);
        text8.FillColor = Configuration.TextColor;
        _helpTexts.Add(text8);

        var text9 = new Text("F9 - Загрузить игру", font, 24);
        text9.Position = new Vector2f(150, 400);
        text9.FillColor = Configuration.TextColor;
        _helpTexts.Add(text9);

        var text10 = new Text("P - Пауза во время игры", font, 24);
        text10.Position = new Vector2f(150, 430);
        text10.FillColor = Configuration.TextColor;
        _helpTexts.Add(text10);

        var text11 = new Text("M - Вкл/выкл музыку", font, 24);
        text11.Position = new Vector2f(150, 460);
        text11.FillColor = Configuration.TextColor;
        _helpTexts.Add(text11);

        var text12 = new Text("F11 - Полноэкранный режим", font, 24);
        text12.Position = new Vector2f(150, 490);
        text12.FillColor = Configuration.TextColor;
        _helpTexts.Add(text12);

        var text13 = new Text("Цель игры:", font, 28);
        text13.Position = new Vector2f(600, 120);
        text13.FillColor = new SFML.Graphics.Color(255, 255, 0);
        _helpTexts.Add(text13);

        var text14 = new Text("• Собирайте белые горошины", font, 24);
        text14.Position = new Vector2f(650, 160);
        text14.FillColor = Configuration.TextColor;
        _helpTexts.Add(text14);

        var text15 = new Text("• Увеличивайте длину змейки", font, 24);
        text15.Position = new Vector2f(650, 190);
        text15.FillColor = Configuration.TextColor;
        _helpTexts.Add(text15);

        var text16 = new Text("• Избегайте столкновений", font, 24);
        text16.Position = new Vector2f(650, 220);
        text16.FillColor = Configuration.TextColor;
        _helpTexts.Add(text16);

        var text17 = new Text("• Супер-горошины дают скорость", font, 24);
        text17.Position = new Vector2f(650, 250);
        text17.FillColor = Configuration.TextColor;
        _helpTexts.Add(text17);
        // Инструкция для возврата
        var text18 = new Text("Нажмите ESC для возврата в меню", font, 28);
        text18.Position = new Vector2f(
            Configuration.WindowWidth / 2 - GetTextWidth("Нажмите ESC для возврата в меню", font, 28) / 2,
            Configuration.WindowHeight - 100);
        text18.FillColor = new SFML.Graphics.Color(0, 255, 0);
        text18.Style = Text.Styles.Bold;
        _helpTexts.Add(text18);
    }
    // Вспомогательный метод для расчета ширины текста
    private float GetTextWidth(string text, SFML.Graphics.Font font, uint fontSize)
    {
        var tempText = new Text(text, font, fontSize);
        return tempText.GetLocalBounds().Width;
    }
    // Обрабатывает пользовательский ввод на экране справки
    public void HandleInput()
    {
        if (_inputManager.IsKeyJustPressed(SFML.Window.Keyboard.Key.Escape))
        {
            // ESC - возврат в главное меню
            _stateManager.ChangeState(Enums.GameStateType.Menu);
        }
    }
    // Обновляет анимации на экране справки
    public void Update(float deltaTime)
    {
        // Добавляем пульсирующий эффект к последнему тексту (инструкции возврата)
        if (_helpTexts.Count > 0)
        {
            var lastText = _helpTexts[_helpTexts.Count - 1];
            float alpha = 200 + (int)(Math.Sin(Environment.TickCount * 0.005) * 55);
            lastText.FillColor = new SFML.Graphics.Color(
                lastText.FillColor.R,
                lastText.FillColor.G,
                lastText.FillColor.B,
                (byte)alpha);
        }
    }
    // Отрисовывает экран справки
    public void Draw(RenderTarget target)
    {
        // Синий фон для экрана справки
        target.Clear(new SFML.Graphics.Color(0, 0, 60));

        target.Draw(_titleText);
        // Рисуем все текстовые элементы
        foreach (var text in _helpTexts)
        {
            target.Draw(text);
        }
    }
}