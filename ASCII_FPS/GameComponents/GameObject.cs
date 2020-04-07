namespace ASCII_FPS.GameComponents
{
    public abstract class GameObject
    {
        public bool Destroy { get; protected set; }

        public abstract void Update(float deltaTime);
    }
}
