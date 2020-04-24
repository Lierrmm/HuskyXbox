﻿namespace Husky
{
    /// <summary>
    /// Vertex Normal Unpacking Methods
    /// </summary>
    class VertexNormalUnpacking
    {
        /// <summary>
        /// Unpacks a Vertex Normal from: WaW, MW2, MW3, Bo1
        /// </summary>
        /// <param name="packedNormal">Packed 4 byte Vertex Normal</param>
        /// <returns>Resulting Vertex Normal</returns>
        public static Vector3 MethodA(PackedUnitVector packedNormal)
        {
            // Decode the scale of the vector
            float decodeScale = ((float)(packedNormal.Byte4 - -192.0) / 32385.0f);

            // Return decoded vector
            return new Vector3(
                (float)(packedNormal.Byte1 - 127.0) * decodeScale,
                (float)(packedNormal.Byte2 - 127.0) * decodeScale,
                (float)(packedNormal.Byte3 - 127.0) * decodeScale);
        }

        /// <summary>
        /// Unpacks a Vertex Normal from: Ghosts, AW, MWR
        /// </summary>
        /// <param name="packedNormal">Packed 4 byte Vertex Normal</param>
        /// <returns>Resulting Vertex Normal</returns>
        public static Vector3 MethodB(PackedUnitVector packedNormal)
        {
            // Return decoded vector
            return new Vector3(
                (float)(((packedNormal.Value & 0x3FF) / 1023.0) * 2.0 - 1.0),
                (float)((((packedNormal.Value >> 10) & 0x3FF) / 1023.0) * 2.0 - 1.0),
                (float)((((packedNormal.Value >> 20) & 0x3FF) / 1023.0) * 2.0 - 1.0));
        }

        /// <summary>
        /// Unpacks a Vertex Normal from: Bo2
        /// </summary>
        /// <param name="packedNormal">Packed 4 byte Vertex Normal</param>
        /// <returns>Resulting Vertex Normal</returns>
        public static Vector3 MethodC(PackedUnitVector packedNormal)
        {
            // Resulting values
            var builtX = new FloatToInt { Integer = (uint)((packedNormal.Value & 0x3FF) - 2 * (packedNormal.Value & 0x200) + 0x40400000) };
            var builtY = new FloatToInt { Integer = (uint)((((packedNormal.Value >> 10) & 0x3FF) - 2 * ((packedNormal.Value >> 10) & 0x200) + 0x40400000)) };
            var builtZ = new FloatToInt { Integer = (uint)((((packedNormal.Value >> 20) & 0x3FF) - 2 * ((packedNormal.Value >> 20) & 0x200) + 0x40400000)) };

            // Return decoded vector
            return new Vector3(
                (builtX.Float - 3.0) * 8208.0312f,
                (builtY.Float - 3.0) * 8208.0312f,
                (builtZ.Float - 3.0) * 8208.0312f);
        }
    }
}
