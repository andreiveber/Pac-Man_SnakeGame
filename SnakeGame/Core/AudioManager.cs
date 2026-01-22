using SFML.Audio;
using System.Collections.Generic;

namespace SnakeGame.Core;

public class AudioManager
{
    private readonly Dictionary<string, Sound> _sounds;
    private readonly Dictionary<string, Music> _music;
    private Sound _currentSound;
    private Music _currentMusic;

    public AudioManager()
    {
        _sounds = new Dictionary<string, Sound>();
        _music = new Dictionary<string, Music>();
        _currentMusic = null;
    }
    // Загружает звуковой эффект из файла
    public void LoadSound(string name, string filename)
    {
        try
        {
            var soundBuffer = new SoundBuffer(filename);
            var sound = new Sound(soundBuffer);
            _sounds[name] = sound;
            Console.WriteLine($"Звук загружен: {name} - {filename}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки звука {filename}: {ex.Message}");
        }
    }
    // Загружает музыкальный файл (потоковое воспроизведение)
    public void LoadMusic(string name, string filename)
    {
        try
        {
            var music = new Music(filename);
            _music[name] = music;
            Console.WriteLine($"Музыка загружена: {name} - {filename}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка загрузки музыки {filename}: {ex.Message}");
        }
    }
    // Воспроизводит звуковой эффект
    public void PlaySound(string name, float volume = 100f)
    {
        if (_sounds.TryGetValue(name, out var sound))
        {
            sound.Volume = volume;
            sound.Play();
            _currentSound = sound;
        }
    }
    // Воспроизводит фоновую музыку
    public void PlayMusic(string name, bool loop = true, float volume = 50f)
    {
        StopMusic();

        if (_music.TryGetValue(name, out var music))
        {
            _currentMusic = music;
            _currentMusic.Volume = volume;
            _currentMusic.Loop = loop;
            _currentMusic.Play();
        }
    }
    // Останавливает музыку
    public void StopMusic()
    {
        if (_currentMusic != null)
        {
            _currentMusic.Stop();
            _currentMusic = null;
        }
    }
    // Приостанавливает музыку
    public void PauseMusic()
    {
        if (_currentMusic != null && _currentMusic.Status == SoundStatus.Playing)
        {
            _currentMusic.Pause();
        }
    }
    // Возобновляет музыку после паузы
    public void ResumeMusic()
    {
        if (_currentMusic != null && _currentMusic.Status == SoundStatus.Paused)
        {
            _currentMusic.Play();
        }
    }
    // Устанавливает громкость музыки
    public void SetMusicVolume(float volume)
    {
        if (_currentMusic != null)
        {
            _currentMusic.Volume = volume;
        }
    }
    // Останавливает все звуки и музыку
    public void StopAll()
    {
        StopMusic();

        if (_currentSound != null)
        {
            _currentSound.Stop();
            _currentSound = null;
        }
    }
    // Очищает ресурсы
    public void Dispose()
    {
        StopAll();

        foreach (var sound in _sounds.Values)
        {
            sound.Dispose();
        }
        _sounds.Clear();

        foreach (var music in _music.Values)
        {
            music.Dispose();
        }
        _music.Clear();
    }
    // Возвращает текущий статус музыки
    public SoundStatus GetMusicStatus()
    {
        return _currentMusic?.Status ?? SoundStatus.Stopped;
    }
}