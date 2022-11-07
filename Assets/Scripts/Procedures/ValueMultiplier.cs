namespace Procedures
{
    public abstract class ValueMultiplier<T>
    {
        public abstract T Multiply(T a, T b);
    }

    public class AndMultiplier : ValueMultiplier<bool>
    {
        public override bool Multiply(bool a, bool b)
        {
            return a && b;
        }
    }

    public class FloatMultiplier : ValueMultiplier<float>
    {
        public override float Multiply(float a, float b)
        {
            return a * b;
        }
    }
}