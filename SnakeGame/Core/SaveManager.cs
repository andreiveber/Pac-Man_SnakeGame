using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SnakeGame.Core;

// Класс для сохранения данных игры
public class GameSave
{
    public List<Vector2> SnakeBody { get; set; } = new();
    public Vector2 SnakeDirection { get; set; }
    public Vector2 FoodPosition { get; set; }
    public bool IsPowerPellet { get; set; }
    public int Score { get; set; }
    public string SaveDate { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    public string SaveName { get; set; } = "Безымянное сохранение";
}

public class SaveManager
{
    private const string SavesDirectory = "Assets/Data/saves";
    // Сохраняет текущее состояние игры в JSON файл
    public void SaveGame(Entities.Snake snake, Entities.Food food, string saveName = null)
    {
        try
        {
            Directory.CreateDirectory(SavesDirectory);

            string fileName = saveName ?? $"save_{DateTime.Now:yyyyMMdd_HHmmss}";
            string savePath = Path.Combine(SavesDirectory, $"{fileName}.json");

            var save = new GameSave
            {
                SnakeBody = new List<Vector2>(snake.Body),
                SnakeDirection = new Vector2(snake.Direction.X, snake.Direction.Y),
                FoodPosition = new Vector2(food.Position.X, food.Position.Y),
                IsPowerPellet = food.IsPowerPellet,
                Score = snake.Score,
                SaveName = saveName ?? $"Сохранение от {DateTime.Now:HH:mm}"
            };
            // Сериализация в JSON
            var json = JsonConvert.SerializeObject(save, Formatting.Indented);
            File.WriteAllText(savePath, json);
            Console.WriteLine($"Игра сохранена: {savePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка сохранения игры: {ex.Message}");
        }
    }
    // Загружает сохранение из файла
    public GameSave LoadGame(string saveFile)
    {
        string savePath = Path.Combine(SavesDirectory, saveFile);

        if (!File.Exists(savePath))
        {
            Console.WriteLine($"Файл сохранения не найден: {savePath}");
            return null;
        }

        try
        {
            var json = File.ReadAllText(savePath);
            var save = JsonConvert.DeserializeObject<GameSave>(json);

            if (save != null)
            {
                Console.WriteLine($"Игра загружена (сохранено: {save.SaveDate})");
            }

            return save;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки игры: {ex.Message}");
            return null;
        }
    }
    // Возвращает список файлов сохранений
    public List<string> GetSaveFiles()
    {
        var saveFiles = new List<string>();

        if (!Directory.Exists(SavesDirectory))
        {
            Directory.CreateDirectory(SavesDirectory);
            return saveFiles;
        }

        var files = Directory.GetFiles(SavesDirectory, "*.json");
        foreach (var file in files)
        {
            saveFiles.Add(Path.GetFileName(file));
        }

        return saveFiles;
    }
    // Проверяет, существуют ли сохранения
    public bool SaveExists()
    {
        return Directory.Exists(SavesDirectory) && Directory.GetFiles(SavesDirectory, "*.json").Length > 0;
    }
    // Удаляет файл сохранения
    public void DeleteSave(string saveFile)
    {
        string savePath = Path.Combine(SavesDirectory, saveFile);
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Console.WriteLine($"Сохранение удалено: {saveFile}");
        }
    }
}