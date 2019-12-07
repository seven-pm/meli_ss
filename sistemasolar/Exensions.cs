namespace SistemaSolar
{
    public static class Exensions
    {
        public static bool Between(this double valor, double min, double max)
        {
            return (valor >= min) && (valor <= max);
        }
    }
}
