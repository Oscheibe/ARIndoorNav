using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class HoughTests : MonoBehaviour
{
    public GameObject point1;
    public GameObject point2;
    public GameObject bar;
    public bool printDebugMessages = false;

    //int width = 480, height = 640, accumulatorWidth = 180, lineWidth = 2;
    int width = 640, height = 480, accumulatorWidth = 180, lineWidth = 2;
    int canvasWidth = 1080;
    int canvasHeight = 2240;
    int xCenter, yCenter;
    float houghHeight, accumulatorHeight;
    float x = 640, y = 480, xPoint2 = 800, yPoint2 = 199;

    private Image lineImage;
    private GameObject line;
    private List<GameObject> lineList = new List<GameObject>();



    // Start is called before the first frame update
    void Start()
    {
        xCenter = width / 2;
        yCenter = height / 2;
        houghHeight = Mathf.Ceil((Mathf.Sqrt(2) * (height > width ? height : width)) / 2);
        accumulatorHeight = houghHeight * 2;

        line = new GameObject();
        lineImage = line.AddComponent<Image>();
        lineImage.color = UnityEngine.Color.black;
        ChangePointsPosition(new Vector3(x, y, 0), new Vector3(xPoint2, yPoint2, 0));
        //TestRho(printDebugMessages);
    }

    // Update is called once per frame
    void Update()
    {
        TestRho3(printDebugMessages);
    }

    private void TestRho(bool printDebugMessages)
    {

        //var theta = 45;
        xCenter = 0;
        yCenter = 0;
        x = point1.transform.position.x;
        y = point1.transform.position.y;
        xPoint2 = point2.transform.position.x;
        yPoint2 = point2.transform.position.y;

        var theta = Vector3.Angle(new Vector3(0, 1, 0), new Vector3(x - xPoint2, y - yPoint2, 0));


        var rho = ((x - xCenter) * Mathf.Cos(theta * Mathf.Deg2Rad)) +
            ((y - yCenter) * Mathf.Sin(theta * Mathf.Deg2Rad));
        rho = Mathf.Abs(rho);

        int x1, y1, x2, y2;

        if (theta >= 45 && theta <= 135)
        {
            //y = (r - x cos(t)) / sin(t)  
            x1 = 0;

            var xCos1 = x1 * Mathf.Cos(theta * Mathf.Deg2Rad);
            var sin1 = Mathf.Sin(theta * Mathf.Deg2Rad);

            y1 = (int)Mathf.Ceil((rho - xCos1) / sin1);

            x2 = canvasWidth;

            var xCos2 = x2 * Mathf.Cos(theta * Mathf.Deg2Rad);
            var sin2 = Mathf.Sin(theta * Mathf.Deg2Rad);

            y2 = (int)Mathf.Ceil((rho - xCos2) / sin2);
        }
        else
        {
            //x = (r - y sin(t)) / cos(t);  
            y1 = 0;
            var ySin1 = y1 * Mathf.Sin(theta * Mathf.Deg2Rad);
            var cos1 = Mathf.Cos(theta * Mathf.Deg2Rad);
            x1 = (int)Mathf.Ceil((rho - ySin1) / cos1);

            y2 = canvasWidth;
            var ySin2 = y2 * Mathf.Sin(theta * Mathf.Deg2Rad);
            var cos2 = Mathf.Cos(theta * Mathf.Deg2Rad);
            x2 = (int)Mathf.Ceil((rho - ySin2) / cos2);
        }

        ClearLineList();
        DrawLine(new Vector3(x1, y1, 0), new Vector3(x2, y2, 0));
        if (printDebugMessages)
        {
            Debug.Log("Running: TestRho()");
            //Debug.Log("Actual Distance to Line: " + Vector3.Distance(new Vector3(0, 0, 0), new Vector3(xPoint2 - x, yPoint2 - y, 0)));
            Debug.Log(string.Format("rho = {0} @ x: {1}, y: {2}, (theta: {3})", rho, x, y, theta));
            Debug.Log(string.Format("x1: {0}, y1: {1}, x2: {2}, y2: {3}", x1, y1, x2, y2));

        }
    }

    private void TestRho2(bool printDebugMessages)
    {

        x = bar.transform.position.x;
        y = bar.transform.position.y;


        var theta = bar.transform.rotation.eulerAngles.z;


        var rho = (x * Mathf.Cos(theta * Mathf.Deg2Rad)) +
                    (y * Mathf.Sin(theta * Mathf.Deg2Rad));

        int x1, y1, x2, y2;

        if (theta >= 45 && theta <= 135)
        {
            //y = (r - x cos(t)) / sin(t)  
            x1 = 0;

            var xCos1 = x1 * Mathf.Cos(theta * Mathf.Deg2Rad);
            var sin1 = Mathf.Sin(theta * Mathf.Deg2Rad);

            y1 = (int)Mathf.Ceil((rho - xCos1) / sin1);

            x2 = width;

            var xCos2 = x2 * Mathf.Cos(theta * Mathf.Deg2Rad);
            var sin2 = Mathf.Sin(theta * Mathf.Deg2Rad);

            y2 = (int)Mathf.Ceil((rho - xCos2) / sin2);
        }
        else
        {
            //x = (r - y sin(t)) / cos(t);  
            y1 = 0;
            var ySin1 = y1 * Mathf.Sin(theta * Mathf.Deg2Rad);
            var cos1 = Mathf.Cos(theta * Mathf.Deg2Rad);
            x1 = (int)Mathf.Ceil((rho - ySin1) / cos1);

            y2 = width;
            var ySin2 = y2 * Mathf.Sin(theta * Mathf.Deg2Rad);
            var cos2 = Mathf.Cos(theta * Mathf.Deg2Rad);
            x2 = (int)Mathf.Ceil((rho - ySin2) / cos2);
        }



        ClearLineList();
        DrawLine(new Vector3(x1, y1, 0), new Vector3(x2, y2, 0));
        if (printDebugMessages)
        {
            Debug.Log("Running: TestRho2");
            //Debug.Log("Actual Distance to Line: " + Vector3.Distance(new Vector3(0, 0, 0), new Vector3(xPoint2 - x, yPoint2 - y, 0)));
            Debug.Log(string.Format("rho = {0} @ x: {1}, y: {2}, (theta: {3})", rho, x, y, theta));
            Debug.Log(string.Format("x1: {0}, y1: {1}, x2: {2}, y2: {3}", x1, y1, x2, y2));

        }
    }


    private void TestRho3(bool printDebugMessages)
    {

        x = bar.transform.position.x;
        y = bar.transform.position.y;
        int maxLineLength = 0;

        var theta = bar.transform.rotation.eulerAngles.z;


        var rho = (x * Mathf.Cos(theta * Mathf.Deg2Rad)) +
                    (y * Mathf.Sin(theta * Mathf.Deg2Rad));

        int x1, y1, x2, y2;

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



        ClearLineList();
        DrawLine(new Vector3(x1, y1, 0), new Vector3(x2, y2, 0));
        if (printDebugMessages)
        {
            Debug.Log("Running: TestRho3");
            //Debug.Log("Actual Distance to Line: " + Vector3.Distance(new Vector3(0, 0, 0), new Vector3(xPoint2 - x, yPoint2 - y, 0)));
            Debug.Log(string.Format("rho = {0} @ x: {1}, y: {2}, (theta: {3})", rho, x, y, theta));
            Debug.Log(string.Format("x1: {0}, y1: {1}, x2: {2}, y2: {3}", x1, y1, x2, y2));

        }
    }

    private int[] Sobel2(int[,] inputImage)
    {

        int x1, x2, y1, y2;

        var houghAccumulator = new int[(int)Mathf.Ceil(accumulatorHeight * accumulatorWidth)];

        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                if ((inputImage[x, y]) > 0)
                {
                    // Hough line filter with rho = x*cos(theta) + y*sin(theta) 
                    for (int theta = 0; theta < 180; theta += 4)
                    {
                        var rho = ((x - xCenter) * Mathf.Cos(theta * Mathf.Deg2Rad)) +
                                    ((y - yCenter) * Mathf.Sin(theta * Mathf.Deg2Rad));
                        houghAccumulator[(int)Mathf.Round(rho + houghHeight) * 180 + theta]++;
                    }

                    /*
                    if (theta >= 45 && theta <= 135)
                    {
                        //y = (r - x cos(t)) / sin(t)  
                        x1 = 0;

                        y1 = (int)Mathf.Ceil(((rho - (accumulatorHeight / 2)) - ((x1 - (width / 2)) * Mathf.Cos(theta * Mathf.Deg2Rad))) /
                                           Mathf.Sin(theta * Mathf.Deg2Rad) + (height / 2));
                        x2 = width;
                        y2 = (int)Mathf.Ceil(((rho - (accumulatorHeight / 2)) - ((x2 - (width / 2)) * Mathf.Cos(theta * Mathf.Deg2Rad))) /
                                           Mathf.Sin(theta * Mathf.Deg2Rad) + (height / 2));
                    }
                    else
                    {
                        //x = (r - y sin(t)) / cos(t);  
                        y1 = 0;
                        x1 = (int)Mathf.Ceil(((rho - (accumulatorHeight / 2)) - ((y1 - (height / 2)) * Mathf.Sin(theta * Mathf.Deg2Rad))) /
                                           Mathf.Cos(theta * Mathf.Deg2Rad) + (width / 2));
                        y2 = width;
                        x2 = (int)Mathf.Ceil(((rho - (accumulatorHeight / 2)) - ((y2 - (height / 2)) * Mathf.Sin(theta * Mathf.Deg2Rad))) /
                                           Mathf.Cos(theta * Mathf.Deg2Rad) + (width / 2));
                    }
                    */

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

    private void ChangePointsPosition(Vector3 position1, Vector3 position2)
    {
        point1.transform.position = position1;
        point2.transform.position = position2;
    }

}
