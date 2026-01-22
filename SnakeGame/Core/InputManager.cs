using SFML.Window;
using System.Collections.Generic;

namespace SnakeGame.Core;

public class InputManager
{
    private readonly HashSet<Keyboard.Key> _pressedKeys = new();
    // Проверяет, нажата ли клавиша в данный момент
    public bool IsKeyPressed(Keyboard.Key key) => Keyboard.IsKeyPressed(key);
    // Проверяет, была ли клавиша только что нажата (одиночное нажатие)
    public bool IsKeyJustPressed(Keyboard.Key key)
    {
        if (Keyboard.IsKeyPressed(key))
        {
            if (!_pressedKeys.Contains(key))
            {
                _pressedKeys.Add(key);
                return true;
            }
        }
        else
        {
            _pressedKeys.Remove(key);
        }
        return false;
    }
    // Очищает список нажатых клавиш
    public void ClearPressedKeys()
    {
        _pressedKeys.Clear();
    }
}