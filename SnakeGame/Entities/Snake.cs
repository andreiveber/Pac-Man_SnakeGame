using SFML.Graphics;
using SFML.System;
using System.Collections.Generic;
using System.Numerics;
using SnakeGame.Core;
using System;
using Color = SFML.Graphics.Color;

namespace SnakeGame.Entities;

public class Snake : Entity
{
    public List<Vector2> Body { get; private set; }
    public Vector2 Direction { get; private set; }
    public int Score => Body.Count - Configuration.InitialSnakeLength;
    public bool IsAlive { get; private set; }
    public bool IsPaused { get; set; }
    public float CurrentSpeed { get; set; }  
    public int PowerPelletTimer { get; private set; }
    public bool HasPowerPelletEffect => PowerPelletTimer > 0;

    private Vector2 _nextDirection;
    private float _moveTimer;
    private int _animationFrame;
    private float _animationTimer;
    private AssetManager _assetManager;

    public Snake(AssetManager assetManager)
    {
        _assetManager = assetManager;
        Body = new List<Vector2>();
        Direction = new Vector2(1, 0);
        _nextDirection = Direction;
        IsAlive = true;
        IsPaused = false;
        _moveTimer = 0;
        _animationFrame = 0;
        _animationTimer = 0;
        CurrentSpeed = Configuration.InitialGameSpeed;
        PowerPelletTimer = 0;

        Reset();
    }
    // Сбрасывает змейку в начальное состояние
    public void Reset()
    {
        Body.Clear();
        for (int i = 0; i < Configuration.InitialSnakeLength; i++)
        {
            Body.Add(new Vector2(
                Configuration.StartPosition.X - i,
                Configuration.StartPosition.Y));
        }
        Direction = new Vector2(1, 0);
        _nextDirection = Direction;
        IsAlive = true;
        IsPaused = false;
        _moveTimer = 0;
        _animationFrame = 0;
        _animationTimer = 0;
        CurrentSpeed = Configuration.InitialGameSpeed;
        PowerPelletTimer = 0;
    }
    // Изменяет направление движения (с проверкой на разворот)
    public void ChangeDirection(Vector2 newDirection)
    {
        if (newDirection.X != -Direction.X || newDirection.Y != -Direction.Y)
        {
            _nextDirection = newDirection;
        }
    }
    // Обновляет состояние змейки
    public override void Update(float deltaTime)
    {
        if (!IsAlive || IsPaused) return;

        _moveTimer += deltaTime;
        _animationTimer += deltaTime;
        // Обновление анимации
        if (_animationTimer >= Configuration.PacManAnimationSpeed)
        {
            _animationTimer = 0;
            _animationFrame++;
        }
        // Уменьшение таймера эффекта супер-горошины
        if (PowerPelletTimer > 0)
        {
            PowerPelletTimer--;
        }
        // Расчет текущей скорости(ускорение от супер-горошины)
        float actualSpeed = CurrentSpeed;
        if (HasPowerPelletEffect)
        {
            actualSpeed = Math.Max(Configuration.MinGameSpeed,
                actualSpeed - Configuration.PowerPelletSpeedBoost);
        }
        // Движение змейки по таймеру
        if (_moveTimer >= actualSpeed)
        {
            _moveTimer = 0;
            Direction = _nextDirection;

            Move();
            CheckWallCollisions();
            CheckSelfCollisions();
        }
    }
    // Перемещает змейку на одну клетку
    private void Move()
    {
        Vector2 previousPosition = Body[0];
        Vector2 currentPosition;
        // Двигаем голову
        Body[0] += Direction;
        // Перемещаем сегменты тела
        for (int i = 1; i < Body.Count; i++)
        {
            currentPosition = Body[i];
            Body[i] = previousPosition;
            previousPosition = currentPosition;
        }
    }
    // Увеличивает скорость змейки (после съедения обычной горошины)
    public void IncreaseSpeed()
    {
        CurrentSpeed = Math.Max(
            Configuration.MinGameSpeed,
            CurrentSpeed - Configuration.SpeedIncrement
        );
    }
    // Активирует эффект супер-горошины
    public void ActivatePowerPellet()
    {
        PowerPelletTimer = Configuration.PowerPelletDuration;
    }
    // Проверяет столкновение со стенами
    private void CheckWallCollisions()
    {
        var head = Body[0];
        int maxX = (int)((Configuration.WindowWidth - Configuration.WallWidth * 2) / Configuration.GridSize);
        int maxY = (int)((Configuration.WindowHeight - Configuration.WallWidth * 2) / Configuration.GridSize);

        if (head.X < 0 || head.X >= maxX ||
            head.Y < 0 || head.Y >= maxY)
        {
            IsAlive = false;
        }
    }
    // Проверяет столкновение головы с телом
    private void CheckSelfCollisions()
    {
        for (int i = 1; i < Body.Count; i++)
        {
            if (Body[0] == Body[i])
            {
                IsAlive = false;
                return;
            }
        }
    }
    // Увеличивает длину змейки на один сегмент
    public void Grow()
    {
        if (Body.Count > 0)
        {
            Body.Add(Body[^1]);
        }
    }
    // Проверяет, съела ли змейка еду
    public bool CheckFoodCollision(Vector2 foodPosition)
    {
        return Body[0] == foodPosition;
    }
    // Отрисовывает змейку на экране
    public override void Draw(RenderTarget target, RenderStates states)
    {
        // Рисуем тело змеи (желтые круги)
        for (int i = 1; i < Body.Count; i++)
        {
            var segment = new CircleShape(Configuration.GridSize / 2 - 1)
            {
                Position = new Vector2f(
                    Body[i].X * Configuration.GridSize + Configuration.WallWidth,
                    Body[i].Y * Configuration.GridSize + Configuration.WallWidth),
                FillColor = new Color(255, 255, 0),
                OutlineColor = new Color(200, 200, 0),
                OutlineThickness = 1
            };
            target.Draw(segment, states);
        }

        // Рисуем голову (Pac-Man с анимацией)
        if (Body.Count > 0)
        {
            var pacmanHead = _assetManager.GetSimplePacManHeadSprite(_animationFrame, Direction);

            if (pacmanHead != null)
            {
                float posX = Body[0].X * Configuration.GridSize + Configuration.WallWidth;
                float posY = Body[0].Y * Configuration.GridSize + Configuration.WallWidth;

                pacmanHead.Position = new Vector2f(posX, posY);
                target.Draw(pacmanHead, states);
            }
            else
            {
                // Запасной вариант - желтый круг
                var head = new CircleShape(Configuration.GridSize / 2 - 1)
                {
                    Position = new Vector2f(
                        Body[0].X * Configuration.GridSize + Configuration.WallWidth,
                        Body[0].Y * Configuration.GridSize + Configuration.WallWidth),
                    FillColor = new Color(255, 255, 0)
                };
                target.Draw(head, states);
            }
        }
    }
}