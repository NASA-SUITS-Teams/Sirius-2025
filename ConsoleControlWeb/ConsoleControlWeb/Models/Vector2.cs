namespace ConsoleControlWeb.Models
{
    public struct Vector2
    {
        public float X { get; set; }
        public float Y { get; set; }
        public Vector2(float x, float y) { X = x; Y = y; }
        public float Magnitude => (float)System.Math.Sqrt(X * X + Y * Y);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.X - b.X, a.Y - b.Y);
    }
}
