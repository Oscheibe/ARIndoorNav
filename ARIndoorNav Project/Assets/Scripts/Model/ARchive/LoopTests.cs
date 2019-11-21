using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopTests : MonoBehaviour
{
    private int height = 480;
    private int width = 640;
    private int loopCount = 0;
    public int endX = 100;
    public int endY = 200;
    public int difference = 10;
    int startX, startY;

    // Start is called before the first frame update
    void Start()
    {
        if(difference == 0)
        {
            startX = 0;
            startY = 0;
        }
        else 
        {
        startX = endX - difference;
        startY = endY - difference;
        }


        TestLoop5();
        Debug.Log("Loop: " + loopCount);

    }

    // Update is called once per frame
    void Update()
    {

    }


    void TestLoop()
    {
        Debug.Log("Testing: TestLoop");
        for (int y = 620; y <= height; y++)
        {
            for (int x = 260; x <= width; x++)
            {
                // Hough line filter with rho = x*cos(theta) + y*sin(theta) 
                for (int theta = 30; theta <= 120; theta += 30)
                {
                    var r = x * Mathf.Cos(theta * Mathf.Deg2Rad) + y * Mathf.Sin(theta * Mathf.Deg2Rad);
                    var rho = (int)Mathf.Ceil(x * Mathf.Cos(theta) + y * Mathf.Sin(theta));
                    ++loopCount;
                    //Debug.Log("Loop: " + loopCount);
                    if (rho < 0)
                    {
                        Debug.Log("Rho: " + rho);
                    }
                    //houghAccumulator[theta, Mathf.Abs(rho)] += 1;
                }
            }
        }
    }

    void TestLoop2()
    {
        Debug.Log("Testing: TestLoop2");
        var houghHeight = (Mathf.Sqrt(2) * (height > width ? height : width)) / 2;
        var accumulatorHeight = houghHeight * 2;
        var xCenter = width / 2;
        var yCenter = height / 2;
        var smallesRho = int.MaxValue;
        var biggestRho = 0;
        for (int y = 0; y <= height; y++)
        {
            for (int x = 0; x <= width; x++)
            {
                // Hough line filter with rho = x*cos(theta) + y*sin(theta) 
                // Hough line filter with rho = x*cos(theta) + y*sin(theta) 
                for (int theta = 0; theta < 180; theta++)
                {
                    loopCount++;
                    var rho = ((x - xCenter) * Mathf.Cos(theta * Mathf.Deg2Rad)) +
                                ((y - yCenter) * Mathf.Sin(theta * Mathf.Deg2Rad));
                    var rhoIndex = (int)Mathf.Round(rho + houghHeight) * 180 + theta;
                    if (rhoIndex < 0)
                    {
                        Debug.Log(string.Format("rho: {0}, x: {1}, y: {2}, theta: {3}", rho, x, y, theta));
                    }
                    if (smallesRho > rhoIndex)
                    {
                        smallesRho = rhoIndex;
                    }
                    if (biggestRho < rhoIndex)
                    {
                        biggestRho = rhoIndex;
                    }
                }
            }
        }
        Debug.Log("smallest rho: " + smallesRho);
        Debug.Log("BIGGEST rho: " + biggestRho);
    }

    void TestLoop3()
    {
        Debug.Log("Testing: TestLoop3");
        var houghHeight = Mathf.Ceil((Mathf.Sqrt(2) * (height > width ? height : width)) / 2); // 453
        var accumulatorHeight = houghHeight * 2; // 905
        var accumulatorWidth = 180;
        var houghAccumulator = new int[(int)Mathf.Ceil(accumulatorHeight * accumulatorWidth)];
        int x1 = 0, x2 = 0, y1 = 0, y2 = 0;

        // Iterating over Hough accumulator and recalculating points
        for (int rho = 0; rho < accumulatorHeight; rho++)
        {
            for (int theta = 0; theta < accumulatorWidth; theta++)
            {
                {
                    loopCount++;


                    if (theta >= 45 && theta <= 135)
                    {
                        //y = (r - x cos(t)) / sin(t)  
                        x1 = 0;
                        y1 = (int)((rho - (accumulatorHeight / 2)) - ((x1 - (width / 2))) * Mathf.Cos(theta * Mathf.Deg2Rad)) /
                                           (int)(Mathf.Sin(theta * Mathf.Deg2Rad) + (height / 2));
                        x2 = width;
                        y2 = (int)((rho - (accumulatorHeight / 2)) - ((x2 - (width / 2))) * Mathf.Cos(theta * Mathf.Deg2Rad)) /
                                           (int)(Mathf.Sin(theta * Mathf.Deg2Rad) + (height / 2));
                    }
                    else
                    {
                        //x = (r - y sin(t)) / cos(t);  
                        y1 = 0;
                        x1 = (int)((rho - ((accumulatorHeight / 2)) - ((y1 - (height / 2)))) * Mathf.Sin(theta * Mathf.Deg2Rad)) /
                                           (int)(Mathf.Cos(theta * Mathf.Deg2Rad) + (width / 2));
                        y2 = width;
                        x2 = (int)((rho - (accumulatorHeight / 2)) - ((y2 - (height / 2))) * Mathf.Sin(theta * Mathf.Deg2Rad)) /
                                           (int)(Mathf.Cos(theta * Mathf.Deg2Rad) + (width / 2));
                    }
                    if (x1 == 0)
                    {
                        Debug.Log(string.Format("x1: {0}, y1: {1}, x2: {2}, y2: {3} / rho: {4}, theta: {5}", x1, y1, x2, y2, rho, theta));
                    }
                }
            }
        }
    }

    void TestLoop4()
    {
        Debug.Log("Testing: TestLoop4");
        var houghHeight = Mathf.Ceil((Mathf.Sqrt(2) * (height > width ? height : width)) / 2); // 452.5
        var accumulatorHeight = houghHeight * 2; // 905
        var accumulatorWidth = 180;
        var houghAccumulator = new int[(int)Mathf.Ceil(accumulatorHeight * accumulatorWidth)];
        var maxValue = 0;

        // Iterating over Hough accumulator and recalculating points
        for (int rho = 0; rho < accumulatorHeight; rho++)
        {
            for (int theta = 0; theta < accumulatorWidth; theta++)
            {
                {
                    var index = (rho * 180) + theta;
                    if (maxValue < index)
                    {
                        maxValue = index;
                    }
                    if (index > houghAccumulator.Length)
                    {
                        Debug.Log(string.Format("index: {0}, rho: {1}, theta: {2}", index, rho, theta));
                    }
                    //houghAccumulator[index]++;
                    loopCount++;


                }
            }
        }
        Debug.Log("Hough height: " + houghHeight);
        Debug.Log("Hough size: " + houghAccumulator.Length);
        Debug.Log("Max Index: " + maxValue);
    }

    void TestLoop5()
    {
        Debug.Log("Testing: TestLoop5");
        //var accumulatorWidth = 180;
        var smallesRho = int.MaxValue;
        var biggestRho = 0;
        var maxHeight = (height > width ? height : width);
        //var maxLineLength = maxHeight * 2;//(int)Mathf.Ceil(Mathf.Sqrt(Mathf.Pow(width, 2) + Mathf.Pow(height, 2))) * 2;
        var maxLineLength = (int)Mathf.Ceil(Mathf.Sqrt(Mathf.Pow(width, 2) + Mathf.Pow(height, 2))) * 2;
        // Width = 180 due to maximum angle = 180°


        for (int y = startY; y <= endY; y++)
        {
            for (int x = startX; x <= endX; x++)
            {
                // Hough line filter with rho = x*cos(theta) + y*sin(theta) 
                // Hough line filter with rho = x*cos(theta) + y*sin(theta) 
                for (int theta = 0; theta < 180; theta++)
                {
                    loopCount++;
                    var rho =   (x * Mathf.Cos(theta * Mathf.Deg2Rad)) +
                                (y * Mathf.Sin(theta * Mathf.Deg2Rad));
                    rho += maxLineLength/2;

                    if(x == 638 && y == 124 && theta == 1)
                    {
                        Debug.Log(string.Format("Out of Bounds: x: {0}, y: {1}, theta: {2}, rho: {3}, height: {4}, width: {5}", x, y, theta, rho, height, width));
                    }

                    if ((int)Mathf.Ceil(rho) < 0 || (int)Mathf.Ceil(rho) > maxLineLength)
                    {
                        Debug.Log(string.Format("rho: {0}, x: {1}, y: {2}, theta: {3}", (int)Mathf.Ceil(rho), x, y, theta));
                    }
                    if (smallesRho > rho)
                    {
                        smallesRho = (int)rho;
                    }
                    if (biggestRho < rho)
                    {
                        biggestRho = (int)rho;
                    }
                }
            }
        }
        Debug.Log("smallest rho: " + smallesRho);
        Debug.Log("BIGGEST rho: " + biggestRho);
        Debug.Log("MAX rho: " + maxLineLength);
    }


    void TestCalculation()
    {
        Debug.Log("Testing: TestCalculation");
        var houghHeight = (Mathf.Sqrt(2) * (height > width ? height : width)) / 2;
        var accumulatorHeight = houghHeight * 2;
        var xCenter = width / 2;
        var yCenter = height / 2;
        var x = 280;
        var y = 640;
        var theta = 179;
        var rho = ((x - xCenter) * Mathf.Cos(theta * Mathf.Deg2Rad)) +
            ((y - yCenter) * Mathf.Sin(theta * Mathf.Deg2Rad));
        Debug.Log(string.Format("houghHeight: {0}, accumHeight: {1}\nxCenter: {2}, yCenter: {3}\nrho: {4}"
                                , houghHeight, accumulatorHeight, xCenter, yCenter, rho));
        Debug.Log("X Calc: " + ((x - xCenter) * Mathf.Cos(theta)));
        Debug.Log("Y Calc: " + ((y - yCenter) * Mathf.Sin(theta)));
        Debug.Log("Deg2Rad: " + Mathf.Deg2Rad + " Deb2Rad * theta: " + theta * Mathf.Deg2Rad);
        Debug.Log("COS1: " + Mathf.Cos(theta * Mathf.Deg2Rad));
        Debug.Log("COS2: " + Mathf.Cos(theta));

        Debug.Log(string.Format("Mathf.Cos({0}) = {1}", (theta * Mathf.Deg2Rad), Mathf.Cos(theta * Mathf.Deg2Rad)));
        Debug.Log(string.Format("Mathf.Cos({0}) = {1}", (theta * Mathf.Rad2Deg), Mathf.Cos(theta * Mathf.Rad2Deg)));
        Debug.Log(string.Format("Mathf.Cos({0}) = {1}", (theta), Mathf.Cos(theta)));

        Debug.Log("RAD2DEG::::::: " + ((x - xCenter) * Mathf.Cos(theta * Mathf.Rad2Deg)) +
            ((y - yCenter) * Mathf.Sin(theta * Mathf.Rad2Deg)));

    }

    void TestCalculation2()
    {
        Debug.Log("Testing: TestCalculation2");
        var rho = 333;
        var accumulatorHeight = 905;
        var x1 = 0;
        var theta = 102;


        int result = (int)Mathf.Ceil(((rho - (accumulatorHeight / 2)) - ((x1 - (width / 2)) * Mathf.Cos(theta * Mathf.Deg2Rad))) /
                                           Mathf.Sin(theta * Mathf.Deg2Rad) + (height / 2));

        Debug.Log("Result: " + result);

        /*
        Debug.Log(string.Format("(({0} - ({1} / 2)) - (({2} - ({3} / 2))) * Mathf.Cos({4} * Mathf.Deg2Rad)) = {5}", rho, accumulatorHeight, x1, width, theta,  ((rho - (accumulatorHeight / 2)) - ((x1 - (width / 2))) * Mathf.Cos(theta * Mathf.Deg2Rad))  ));
        Debug.Log(string.Format("Mathf.Sin({0}) + ({1} / 2)) = {2}", (theta * Mathf.Deg2Rad),height, (Mathf.Sin(theta * Mathf.Deg2Rad) + (height / 2)) ));

        var part1 = ((rho - (accumulatorHeight / 2)) - (((x1 - (width / 2))) * Mathf.Cos(theta * Mathf.Deg2Rad)));
        var part2 = (Mathf.Sin(theta * Mathf.Deg2Rad) + (height / 2));
        var result2 = part1/part2;
        Debug.Log(string.Format("Part1: {0}, Part2: {1}, Result2: {2}", part1, part2, result2));

        Debug.Log("Calc1: " + ((rho - (accumulatorHeight / 2)) - ((x1 - (width / 2)))) + ", Calc2: " + Mathf.Cos(theta * Mathf.Deg2Rad));
        */
    }
}
