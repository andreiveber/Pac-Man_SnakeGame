using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SnakeGame.Core;

public class AssetManager
{
    // Словари для хранения ресурсов
    private readonly Dictionary<string, Texture> _textures = new();
    private readonly Dictionary<string, Font> _fonts = new();
    private readonly Dictionary<string, Sprite> _sprites = new();
    private readonly Dictionary<string, SFML.Graphics.Image> _images = new();
    private readonly Dictionary<string, Texture> _backgroundTextures = new();
    private readonly Dictionary<string, Sprite> _backgroundSprites = new();

    // Загружает текстуру из файла и сохраняет под заданным именем
    public void LoadTexture(string name, string filename)
    {
        var texture = new Texture(filename);
        _textures[name] = texture;
    }
    // Загружает шрифт из файла    
    public void LoadFont(string name, string filename)
    {
        var font = new Font(filename);
        _fonts[name] = font;
    }
    // Загружает изображение для внутренней обработки
    public void LoadImage(string name, string filename)
    {
        try
        {
            var image = new SFML.Graphics.Image(filename);
            _images[name] = image;
            Console.WriteLine($"Изображение загружено: {name} - {filename}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки изображения {filename}: {ex.Message}");
        }
    }
    // Загружает фоновое изображение, масштабирует под размер окна
    public void LoadBackground(string name, string filename)
    {
        try
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"Файл фона не найден: {filename}");
                return;
            }

            var texture = new Texture(filename);
            _backgroundTextures[name] = texture;

            var sprite = new Sprite(texture);
            float scaleX = (float)Configuration.WindowWidth / texture.Size.X;
            float scaleY = (float)Configuration.WindowHeight / texture.Size.Y;
            sprite.Scale = new Vector2f(scaleX, scaleY);

            _backgroundSprites[name] = sprite;
            Console.WriteLine($"Фон загружен: {name} - {filename}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки фона {filename}: {ex.Message}");
        }
    }

    public Texture GetTexture(string name) => _textures[name];
    public Font GetFont(string name) => _fonts[name];
    public Sprite GetSprite(string name) => _sprites.ContainsKey(name) ? _sprites[name] : null;
    public Sprite GetBackground(string name) => _backgroundSprites.ContainsKey(name) ? _backgroundSprites[name] : null;

    public bool HasFont(string name) => _fonts.ContainsKey(name);
    public bool HasTexture(string name) => _textures.ContainsKey(name);
    public bool HasSprite(string name) => _sprites.ContainsKey(name);
    public bool HasBackground(string name) => _backgroundSprites.ContainsKey(name);
    // Создает текстуру для тела змейки в стиле Pac-Man
    public void CreateSnakeBodyTexture()
    {
        int size = Configuration.GridSize;
        var texture = new Texture((uint)size, (uint)size);

        using (var image = new SFML.Graphics.Image((uint)size, (uint)size, new Color(0, 0, 0, 0)))
        {
            // Рисуем круг желтого цвета (тело змейки)
            int radius = size / 2 - 1;
            int center = size / 2;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float dist = MathF.Sqrt(
                        MathF.Pow(x - center, 2) +
                        MathF.Pow(y - center, 2));

                    if (dist < radius)
                    {
                        image.SetPixel((uint)x, (uint)y, new Color(255, 255, 0));
                    }
                }
            }

            texture.Update(image);
            texture.Smooth = true;
        }

        _textures["snake_body"] = texture;
        _sprites["snake_body"] = new Sprite(texture);
    }
    // Создает анимированную текстуру головы Pac-Man
    public void CreateSimplePacManHeadTexture(int frame, Vector2 direction)
    {
        int size = Configuration.GridSize;
        var texture = new Texture((uint)size, (uint)size);

        using (var image = new SFML.Graphics.Image((uint)size, (uint)size, new Color(0, 0, 0, 0)))
        {
            int radius = size / 2 - 1;
            int center = size / 2;
            // Анимация открывания/закрывания рта
            float mouthProgress = (frame % 30) / 30.0f;
            float mouthSize = 0.2f + MathF.Sin(mouthProgress * MathF.PI * 2) * 0.4f;
            // Определяем угол поворота в зависимости от направления
            float dirAngle = 0;
            if (direction.X > 0) dirAngle = 0;
            else if (direction.X < 0) dirAngle = MathF.PI;
            else if (direction.Y < 0) dirAngle = MathF.PI * 1.5f;
            else dirAngle = MathF.PI * 0.5f;
            // Рисуем Pac-Man с открытым ртом
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float dx = x - center;
                    float dy = y - center;
                    float dist = MathF.Sqrt(dx * dx + dy * dy);

                    if (dist < radius)
                    {
                        float angle = MathF.Atan2(dy, dx);
                        float relAngle = angle - dirAngle;
                        // Нормализуем угол
                        while (relAngle > MathF.PI) relAngle -= 2 * MathF.PI;
                        while (relAngle < -MathF.PI) relAngle += 2 * MathF.PI;
                        // Не рисуем пиксели в области рта
                        if (MathF.Abs(relAngle) > mouthSize || dist < radius * 0.4f)
                        {
                            image.SetPixel((uint)x, (uint)y, new Color(255, 255, 0));
                        }
                    }
                }
            }

            texture.Update(image);
            texture.Smooth = true;
        }

        string textureName = $"pacman_{direction.X}_{direction.Y}_{frame % 30}";
        _textures[textureName] = texture;
    }

    public Sprite GetSimplePacManHeadSprite(int frame, Vector2 direction)
    {
        string textureName = $"pacman_{direction.X}_{direction.Y}_{frame % 30}";
        if (!_textures.ContainsKey(textureName))
        {
            CreateSimplePacManHeadTexture(frame, direction);
        }
        return new Sprite(_textures[textureName]);
    }
    // Создает текстуру обычной горошины
    public void CreatePelletTexture()
    {
        int size = Configuration.GridSize / 2;
        var texture = new Texture((uint)size, (uint)size);

        using (var image = new SFML.Graphics.Image((uint)size, (uint)size, new Color(0, 0, 0, 0)))
        {
            int radius = size / 2;
            int center = size / 2;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float dist = MathF.Sqrt(
                        MathF.Pow(x - center, 2) +
                        MathF.Pow(y - center, 2));

                    if (dist < radius)
                    {
                        image.SetPixel((uint)x, (uint)y, Color.White);
                    }
                }
            }

            texture.Update(image);
            texture.Smooth = true;
        }

        _textures["pellet"] = texture;
        _sprites["pellet"] = new Sprite(texture);
    }
    // Создает пульсирующую текстуру супер-горошины
    public void CreatePowerPelletTexture(int frame)
    {
        int size = Configuration.GridSize;
        var texture = new Texture((uint)size, (uint)size);

        using (var image = new SFML.Graphics.Image((uint)size, (uint)size, new Color(0, 0, 0, 0)))
        {
            // Пульсация размера
            float pulse = 0.5f + (float)Math.Sin(frame * 0.3) * 0.5f;
            int radius = (int)(size / 2 * pulse);
            int center = size / 2;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float dist = MathF.Sqrt(
                        MathF.Pow(x - center, 2) +
                        MathF.Pow(y - center, 2));

                    if (dist < radius)
                    {
                        // Пульсация яркости
                        byte brightness = (byte)(200 + pulse * 55);
                        image.SetPixel((uint)x, (uint)y, new Color(brightness, brightness, brightness));
                    }
                }
            }

            texture.Update(image);
            texture.Smooth = true;
        }

        string textureName = $"power_pellet_{frame % 20}";
        _textures[textureName] = texture;
    }

    public Sprite GetPowerPelletSprite(int frame)
    {
        string textureName = $"power_pellet_{frame % 20}";
        if (!_textures.ContainsKey(textureName))
        {
            CreatePowerPelletTexture(frame);
        }
        return new Sprite(_textures[textureName]);
    }
    // Очищает все загруженные ресурсы
    public void Dispose()
    {
        foreach (var texture in _textures.Values)
        {
            texture.Dispose();
        }
        _textures.Clear();

        foreach (var font in _fonts.Values)
        {
            font.Dispose();
        }
        _fonts.Clear();

        _sprites.Clear();

        foreach (var image in _images.Values)
        {
            image.Dispose();
        }
        _images.Clear();

        foreach (var texture in _backgroundTextures.Values)
        {
            texture.Dispose();
        }
        _backgroundTextures.Clear();

        _backgroundSprites.Clear();
    }
}