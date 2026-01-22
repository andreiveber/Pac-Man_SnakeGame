using System.Numerics;
using Color = SFML.Graphics.Color;

namespace SnakeGame.Core;

public static class Configuration
{
    // Размеры окна
    public const uint WindowWidth = 1024;
    public const uint WindowHeight = 768;
    public const string WindowTitle = "Pac-Man Змейка";

    // Игровые параметры
    public const int GridSize = 28;
    public const int InitialSnakeLength = 3;
    public const float InitialGameSpeed = 0.15f;
    public const float MinGameSpeed = 0.05f;
    public const float SpeedIncrement = 0.005f;
    public const float PowerPelletSpeedBoost = 0.02f;
    public const int PowerPelletDuration = 100;
    public const float WallWidth = 4f;

    public static readonly Vector2 StartPosition = new(15, 15);

    // Анимация
    public const int PacManAnimationFrames = 20;
    public const float PacManAnimationSpeed = 0.05f;

    // Звук
    public const float MusicVolume = 50f;
    public const float SoundVolume = 70f;

    // Цвета
    public static readonly Color BackgroundColor = Color.Black;
    public static readonly Color WallColor = new(0, 50, 100);
    public static readonly Color TextColor = new(255, 255, 255);

    // Размеры шрифтов
    public const uint TitleFontSize = 80;
    public const uint MenuTitleFontSize = 48;
    public const uint MenuFontSize = 36;
    public const uint ScoreFontSize = 28;
    public const uint SmallFontSize = 20;
    public const uint GameOverFontSize = 48;

    // Пути
    public const string AssetsPath = "Assets";
    public const string BackgroundsPath = "Assets/Backgrounds";
    public const string MusicPath = "Assets/Music";              
    public const string SavesPath = "Assets/Data/saves";
    public const string FontsPath = "Assets/Fonts";
}