using SFML.Graphics;
using SFML.System;
using System.Numerics;
using SnakeGame.Core;
using System;
using Color = SFML.Graphics.Color;

namespace SnakeGame.Entities;

public class Food : Entity
{
    public Vector2 Position { get; private set; }
    public bool IsPowerPellet { get; private set; }

    private readonly Random _random;
    private AssetManager _assetManager;
    private float _animationTimer;
    private int _animationFrame;

    public Food(AssetManager assetManager)
    {
        _assetManager = assetManager;
        _random = new Random();
        _animationTimer = 0;
        _animationFrame = 0;

        Respawn();
    }
    // Перемещает еду в случайную позицию
    public void Respawn()
    {
        int maxX = (int)((Configuration.WindowWidth - Configuration.WallWidth * 2) / Configuration.GridSize);
        int maxY = (int)((Configuration.WindowHeight - Configuration.WallWidth * 2) / Configuration.GridSize);

        Position = new Vector2(
            _random.Next(1, maxX - 1),
            _random.Next(1, maxY - 1)
        );
        // 10% шанс создать супер-горошину
        IsPowerPellet = _random.Next(100) < 10;
    }

    // Устанавливает позицию и тип еды (используется при загрузке сохранения)
    public void SetPosition(Vector2 position, bool isPowerPellet)
    {
        Position = position;
        IsPowerPellet = isPowerPellet;
    }
    // Обновляет анимацию еды
    public override void Update(float deltaTime)
    {
        _animationTimer += deltaTime;
        if (_animationTimer >= 0.1f)
        {
            _animationTimer = 0;
            _animationFrame++;
        }
    }
    // Отрисовывает еду на экране
    public override void Draw(RenderTarget target, RenderStates states)
    {
        if (IsPowerPellet)
        {
            // Отрисовка пульсирующей супер-горошины
            var powerPellet = _assetManager.GetPowerPelletSprite(_animationFrame);
            if (powerPellet != null)
            {
                powerPellet.Position = new Vector2f(
                    Position.X * Configuration.GridSize + Configuration.WallWidth,
                    Position.Y * Configuration.GridSize + Configuration.WallWidth);
                target.Draw(powerPellet, states);
            }
            else
            {
                // Запасной вариант - белый круг
                var shape = new CircleShape(Configuration.GridSize / 2 - 2)
                {
                    Position = new Vector2f(
                        Position.X * Configuration.GridSize + Configuration.WallWidth + 2,
                        Position.Y * Configuration.GridSize + Configuration.WallWidth + 2),
                    FillColor = new Color(255, 255, 255)
                };
                target.Draw(shape, states);
            }
        }
        else
        {
            // Отрисовка обычной горошины
            var pellet = _assetManager.GetSprite("pellet");
            if (pellet != null)
            {
                float offset = Configuration.GridSize / 4;
                pellet.Position = new Vector2f(
                    Position.X * Configuration.GridSize + Configuration.WallWidth + offset,
                    Position.Y * Configuration.GridSize + Configuration.WallWidth + offset);
                target.Draw(pellet, states);
            }
            else
            {
                // Запасной вариант - маленький белый круг
                var shape = new CircleShape(Configuration.GridSize / 4)
                {
                    Position = new Vector2f(
                        Position.X * Configuration.GridSize + Configuration.WallWidth + Configuration.GridSize / 4,
                        Position.Y * Configuration.GridSize + Configuration.WallWidth + Configuration.GridSize / 4),
                    FillColor = new Color(255, 255, 255)
                };
                target.Draw(shape, states);
            }
        }
    }
}
