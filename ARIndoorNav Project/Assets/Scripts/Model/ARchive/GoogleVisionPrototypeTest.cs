using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class GoogleVisionPrototypeTest : MonoBehaviour
{

    public TextDetection _TextDetection;

    private CameraImageBytes image;
    private bool gotImage = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GetImage()
    {
        Debug.Log("Getting Image data");

        image = Frame.CameraImage.AcquireCameraImageBytes();
        if (!image.IsAvailable)
        {
            Debug.Log("Couldn't access camera image!");
        }
        else
        {
            var imageWidth = image.Width;
            var imageHeight =  image.Height; 
            var imageY = image.Y;
            var imageYRowStride = image.YRowStride;

            _TextDetection.DetectText(imageWidth, imageHeight, imageY, imageYRowStride);
        }

    }
}
