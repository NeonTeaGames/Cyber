
using System;
using UnityEngine;

namespace Cyber.Networking {

    /// <summary>
    /// Class that contains a few functions to help de/serialize some arrays.
    /// </summary>
    public class NetworkHelper {
        
        /// <summary>
        /// Divides an int-array into four byte-arrays.
        /// </summary>
        /// <param name="intArray">Int-array to divide</param>
        /// <returns>Four byte-arrays</returns>
        public static byte[][] SerializeIntArray(int[] intArray) {
            byte[][] ByteArray = new byte[4][];
            for (int i = 0; i < 4; i++) {
                ByteArray[i] = new byte[intArray.Length];
            }

            for (int top = 0; top < 4; top++) {
                for (int i = 0; i < intArray.Length; i++) {
                    byte B = (byte) ((intArray[i] & (0xFF << (top * 8))) >> (top * 8));
                    ByteArray[top][i] = B;
                }
            }

            return ByteArray;
        }

        /// <summary>
        /// Deserializes an int-array from four byte-arrays
        /// </summary>
        /// <param name="byteArray">Four byte arrays</param>
        /// <returns>An int array</returns>
        public static int[] DeserializeIntArray(byte[][] byteArray) {
            if (byteArray.GetLength(0) != 4) {
                throw new ArgumentException("Specified byte-array is invalid (First dimension should be of length 4).");
            } else if (byteArray.Length <= 0 || byteArray[0] == null) {
                return new int[0];
            }
            int[] IntArray = new int[byteArray[0].Length];
            
            for (int i = 0; i < byteArray[0].Length; i++) {
                int I = byteArray[0][i] + (byteArray[1][i] << 8) + (byteArray[2][i] << 16) + (byteArray[3][i] << 24);
                IntArray[i] = I;
            }

            return IntArray;
        }

        /// <summary>
        /// Returns the current system time in seconds.
        /// </summary>
        /// <returns>The system time in seconds.</returns>
        public static double GetTime() {
            return DateTime.Now.ToUniversalTime().Subtract(
                   new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
                   .TotalMilliseconds * 1.0 / 1000;
        }

    }
}
