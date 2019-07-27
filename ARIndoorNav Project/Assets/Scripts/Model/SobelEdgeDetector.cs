using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SobelEdgeDetector : MonoBehaviour
{    
    private static byte[] s_ImageBuffer = new byte[0];
    private static int s_ImageBufferSize = 0;
    private static int[,] houghAccumulator;
    
    public static int[,] Sobel(byte[] outputImage, IntPtr inputImage, int width, int height, int rowStride)
    {
        var maxLineLength =  (int)Mathf.Ceil(Mathf.Sqrt(Mathf.Pow(width, 2) + Mathf.Pow(height, 2)));
        houghAccumulator = new int[180, maxLineLength];

        if (outputImage.Length < width * height)
        {
            Debug.Log("Input buffer is too small!");
            return null;
        }

        // Adjust buffer size if necessary.
        int bufferSize = rowStride * height;
        if (bufferSize != s_ImageBufferSize || s_ImageBuffer.Length == 0)
        {
            s_ImageBufferSize = bufferSize;
            s_ImageBuffer = new byte[bufferSize];
        }

        // Move raw data into managed buffer.
        System.Runtime.InteropServices.Marshal.Copy(inputImage, s_ImageBuffer, 0, bufferSize);

        // Detect edges.
        int threshold = 128 * 128;

        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                // Offset of the pixel at [i, j] of the input image.
                int offset = (y * rowStride) + x;

                // Neighbour pixels around the pixel at [i, j].
                int a00 = s_ImageBuffer[offset - rowStride - 1];
                int a01 = s_ImageBuffer[offset - rowStride];
                int a02 = s_ImageBuffer[offset - rowStride + 1];
                int a10 = s_ImageBuffer[offset - 1];
                int a12 = s_ImageBuffer[offset + 1];
                int a20 = s_ImageBuffer[offset + rowStride - 1];
                int a21 = s_ImageBuffer[offset + rowStride];
                int a22 = s_ImageBuffer[offset + rowStride + 1];

                // Sobel X filter:
                //   -1, 0, 1,
                //   -2, 0, 2,
                //   -1, 0, 1
                int xSum = -a00 - (2 * a10) - a20 + a02 + (2 * a12) + a22;

                // Sobel Y filter:
                //    1, 2, 1,
                //    0, 0, 0,
                //   -1, -2, -1
                int ySum = a00 + (2 * a01) + a02 - a20 - (2 * a21) - a22;

                if ((xSum * xSum) + (ySum * ySum) > threshold)
                {
                    for (int theta = 30; theta <= 120; theta += 30)
                    {
                        var rho = (int)Mathf.Ceil(x * Mathf.Cos(theta) + y * Mathf.Sin(theta));
                        houghAccumulator[theta, Mathf.Abs(rho)] += 1;
                    }

                    outputImage[(y * width) + x] = 0xFF;
                }
                else
                {
                    outputImage[(y * width) + x] = 0x1F;
                }
            }
        }
        
        return houghAccumulator;
    }
}
