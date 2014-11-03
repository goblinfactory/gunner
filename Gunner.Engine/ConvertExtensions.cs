namespace Gunner.Engine
{
    public static class ConvertExtensions
    {
        public static decimal ToMegabytes(this long bytes)
        {
            return bytes/1048576M;
        }
    }
}