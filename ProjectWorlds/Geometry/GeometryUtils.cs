namespace ProjectWorlds.Geometry
{
    public static class GeometryUtils
    {
        /// <summary>
        /// Square root of 0.5
        /// </summary>
        public const float Sqrt05 = 0.7071067811865475244f;
        /// <summary>
        /// Square root of 2
        /// </summary>
        public const float Sqrt2 = 1.4142135623730950488f;
        /// <summary>
        /// Square root of 5
        /// </summary>
        public const float Sqrt5 = 2.2360679774997896964f;
        /// <summary>
        /// Golden angle in radians
        /// </summary>
        public const float GoldenAngle = UnityEngine.Mathf.PI * (3 - Sqrt5);

        /// <summary>
        /// Swaps values of <paramref name="left"/> and <paramref name="right"/>
        /// </summary>
        public static void Swap<T>(ref T left, ref T right)
        {
            T temp = left;
            left = right;
            right = temp;
        }
    }
}
