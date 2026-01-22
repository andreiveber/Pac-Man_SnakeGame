using SFML.Graphics;

namespace SnakeGame.Entities;

public abstract class Entity : Transformable, Drawable
{
    // Обновляет состояние сущности (вызывается каждый кадр)
    public abstract void Update(float deltaTime);
    // Отрисовывает сущность на экране
    public abstract void Draw(RenderTarget target, RenderStates states);
}