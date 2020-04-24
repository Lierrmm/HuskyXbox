// ------------------------------------------------------------------------
using System.Runtime.InteropServices;

namespace Husky
{
    /// <summary>
    /// A class that contains both a float and an int in the same bytes, to mimick C++ Unions
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    class FloatToInt
    {
        /// <summary>
        /// Integer Value
        /// </summary>
        [FieldOffset(0)]
        public uint Integer;

        /// <summary>
        /// Floating Point Value
        /// </summary>
        [FieldOffset(0)]
        public float Float;
    }
}