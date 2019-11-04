using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class SobelEdgeDetector : MonoBehaviour
{
    private byte[] s_ImageBuffer = new byte[0];
    private int s_ImageBufferSize = 0;

    private Image lineImage;
    private GameObject line;
    private List<GameObject> lineList = new List<GameObject>();
    private Vector3 pointA = new Vector3(400, 400, 0);
    private Vector3 pointB = new Vector3(200, 200, 0);
    private int lineWidth = 2;


    void Start()
    {
        line = new GameObject();
        lineImage = line.AddComponent<Image>();
        lineImage.color = UnityEngine.Color.black;
    }

    public void TestLineDraing()
    {
        ClearLineList();
        DrawLine(new Vector3(50, 800, 0), new Vector3(400, 20, 0));
        DrawLine(new Vector3(400, 400, 0), new Vector3(200, 200, 0));
        DrawLine(new Vector3(200, 200, 0), new Vector3(400, 400, 0));
        DrawLine(new Vector3(600, 1000, 0), new Vector3(900, 600, 0));
    }

    public int CalculateHoughLinesMedian(IntPtr inputImage, int width, int height, int rowStride)
    {
        int[,] sobelResult = Sobel(inputImage, width, height, rowStride);
        int maxValue = 0;
        if (sobelResult == null)
        {
            return -1;
        }
        List<int> allValues = new List<int>();
        //houghAccumulator[theta, Mathf.Abs(rho)] += 1;
        for (int theta = 0; theta < sobelResult.GetLength(0); theta++)
        {
            for (int rho = 0; rho < sobelResult.GetLength(1); rho++)
            {
                var houghLineCount = sobelResult[theta, rho];
                if (maxValue < houghLineCount)
                {
                    maxValue = houghLineCount;
                }
                allValues.Add(houghLineCount);
            }
        }

        //return maxValue;
        return allValues[allValues.Count / 2];

    }

    public string CalculateHoughLinesMedian2(IntPtr inputImage, int width, int height, int rowStride)
    {
        int[,] sobelResult = Sobel2(inputImage, width, height, rowStride);

        int maxValue = 0;
        if (sobelResult == null)
        {
            return "" + -1;
        }
        List<int> allValues = new List<int>();
        //houghAccumulator[theta, Mathf.Abs(rho)] += 1;
        foreach (var accu in sobelResult)
        {
            if (maxValue < accu)
            {
                maxValue = accu;
            }
            allValues.Add(accu);
        }

        //return ""+maxValue;
        return "Median: " + allValues[allValues.Count / 2] + ", Max: " + maxValue;
    }

    public string DrawHoughLines(IntPtr inputImage, int width, int height, int rowStride)
    {
        int[,] sobelResult = Sobel(inputImage, width, height, rowStride);
        string result = "";
        int houghThreshold = 40;
        ClearLineList();

        if (sobelResult == null)
        {
            return null;
        }
        for (int theta = 0; theta < sobelResult.GetLength(0); theta++)
        {
            for (int rho = 0; rho < sobelResult.GetLength(1); rho++)
            {
                if (houghThreshold > sobelResult[theta, rho])
                {
                    continue;
                }
                var a = Mathf.Cos(theta);
                var b = Mathf.Sin(theta);
                var x0 = a * rho;
                var y0 = b * rho;
                var x1 = x0 + 2000 * (-b);
                var y1 = y0 + 2000 * (a);
                var x2 = x0 - 2000 * (-b);
                var y2 = x0 - 2000 * (-b);

                //var x1 = rho / (Mathf.Cos(DegreeToRadian(theta)));
                //var y1 = 0;
                //var x2 = 0;
                //var y2 = rho / (Mathf.Sin(DegreeToRadian(theta)));

                result += x1 + ", " + y1 + " / " + x2 + ", " + y2 + "\n";
                DrawLine(new Vector3(x1, y1, 0), new Vector3(x2, y2, 0));
            }
        }


        return result;
    }

    public string DrawHoughLines2(IntPtr inputImage, int width, int height, int rowStride, int canvasWidth, int canvasHeight)
    {
        //var houghHeight = Mathf.Ceil((Mathf.Sqrt(2) * (height > width ? height : width)) / 2); //453
        //var maxHeight = (height > width ? height : width);
        var maxLineLength = (int)Mathf.Ceil(Mathf.Sqrt(Mathf.Pow(width, 2) + Mathf.Pow(height, 2)));
        string result = "";


        int[,] sobelResult = Sobel2(inputImage, width, height, rowStride); // size: 163080
        int houghThreshold = 60;
        ClearLineList();
        if (sobelResult == null)
        {
            return null;
        }

        // Iterating over Hough accumulator and recalculating points
        for (int rho = 0; rho < sobelResult.GetLength(1); rho++)
        {
            for (int theta = 0; theta < 180; theta++)
            {
                // Threshold used for line detection
                if (sobelResult[theta, rho] >= houghThreshold)
                {
                    int x1, x2, y1, y2;

                    if (theta >= 45 && theta <= 135)
                    {
                        //y = (r - x cos(t)) / sin(t)  
                        x1 = 0;

                        var xCos1 = x1 * Mathf.Cos(theta * Mathf.Deg2Rad);
                        var sin1 = Mathf.Sin(theta * Mathf.Deg2Rad);

                        y1 = (int)Mathf.Ceil((rho - maxLineLength - xCos1) / sin1);

                        x2 = canvasWidth;

                        var xCos2 = x2 * Mathf.Cos(theta * Mathf.Deg2Rad);
                        var sin2 = Mathf.Sin(theta * Mathf.Deg2Rad);

                        y2 = (int)Mathf.Ceil((rho - maxLineLength - xCos2) / sin2);
                    }
                    else
                    {
                        //x = (r - y sin(t)) / cos(t);  
                        y1 = 0;
                        var ySin1 = y1 * Mathf.Sin(theta * Mathf.Deg2Rad);
                        var cos1 = Mathf.Cos(theta * Mathf.Deg2Rad);
                        x1 = (int)Mathf.Ceil((rho - maxLineLength - ySin1) / cos1);

                        y2 = canvasHeight;
                        var ySin2 = y2 * Mathf.Sin(theta * Mathf.Deg2Rad);
                        var cos2 = Mathf.Cos(theta * Mathf.Deg2Rad);
                        x2 = (int)Mathf.Ceil((rho - maxLineLength - ySin2) / cos2);
                    }
                    result += x1 + ", " + y1 + " / " + x2 + ", " + y2 + "\n";
                    DrawLine(new Vector3(x1, y1, 0), new Vector3(x2, y2, 0));
                }
            }
        }
        return result;
    }

    private int[,] Sobel2(IntPtr inputImage, int width, int height, int rowStride)
    {
        //var houghHeight = Mathf.Ceil((Mathf.Sqrt(2) * (height > width ? height : width)) / 2);
        //var accumulatorHeight = houghHeight * 2;
        //var maxLineLength = (int)Mathf.Ceil(Mathf.Sqrt(Mathf.Pow(width, 2) + Mathf.Pow(height, 2))) * 2;
        // Width = 180 due to maximum angle = 180°
        var accumulatorWidth = 180;
        //var xCenter = width / 2;
        //var yCenter = height / 2;
        //var maxHeight = (height > width ? height : width);
        var maxLineLength = (int)Mathf.Ceil(Mathf.Sqrt(Mathf.Pow(width, 2) + Mathf.Pow(height, 2)));

        int[,] houghAccumulator = new int[accumulatorWidth, maxLineLength * 2];
        //var houghAccumulator = new int[(int)Mathf.Ceil(accumulatorHeight * accumulatorWidth)];

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
                    // Hough line filter with rho = x*cos(theta) + y*sin(theta) 
                    for (int theta = 0; theta < 180; theta++)
                    {
                        var rho = (x * Mathf.Cos(theta * Mathf.Deg2Rad)) +
                                    (y * Mathf.Sin(theta * Mathf.Deg2Rad));
                        rho += maxLineLength;
                        try
                        {
                            houghAccumulator[theta, (int)Mathf.Ceil(rho)] += 1;
                        }
                        catch (System.IndexOutOfRangeException)
                        {
                            Debug.Log(string.Format("Out of Bounds: x: {0}, y: {1}, theta: {2}, rho: {3}, height: {4}, width: {5}", x, y, theta, rho, height, width));
                            throw;
                        }

                        //houghAccumulator[(int)Mathf.Round(rho + houghHeight) * 180 + theta]++;
                    }
                }
            }
        }

        return houghAccumulator;
    }


    private int[,] Sobel(IntPtr inputImage, int width, int height, int rowStride)
    {
        var maxLineLength = (int)Mathf.Ceil(Mathf.Sqrt(Mathf.Pow(width, 2) + Mathf.Pow(height, 2)));
        int[,] houghAccumulator = new int[180, maxLineLength];

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
                    // Hough line filter with rho = x*cos(theta) + y*sin(theta) 
                    for (int theta = 30; theta <= 120; theta += 30)
                    {
                        var rho = (int)Mathf.Ceil(x * Mathf.Cos(theta) + y * Mathf.Sin(theta));
                        if (rho < 0)
                        {
                            continue;
                        }
                        houghAccumulator[theta, rho] += 1;
                    }
                }

            }
        }

        return houghAccumulator;
    }

    private void DrawLine(Vector3 pointA, Vector3 pointB)
    {
        int maxCount = 50;
        if (lineList.Count >= maxCount)
        {
            return;
        }
        Vector3 differenceVector = pointB - pointA;
        var newObject = Instantiate(line, new Vector3(400, 400, 0), Quaternion.identity, transform);
        lineList.Add(newObject);

        var newObjectRect = newObject.GetComponent<RectTransform>();
        newObjectRect.sizeDelta = new Vector2(differenceVector.magnitude, lineWidth);
        newObjectRect.pivot = new Vector2(0, 0.5f);
        newObjectRect.position = pointA;
        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        newObjectRect.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void ClearLineList()
    {
        if (lineList.Count > 0)
        {
            foreach (GameObject line in lineList)
            {
                DestroyImmediate(line);
            }
            lineList.Clear();
        }
    }


    private float DegreeToRadian(float angle)
    {
        return Mathf.PI * angle / (float)180;
    }
}
