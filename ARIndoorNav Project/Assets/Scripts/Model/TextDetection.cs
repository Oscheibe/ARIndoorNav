using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;
using System.Runtime.InteropServices;
using SimpleJSON;
using UnityEngine.UI;

public class TextDetection : MonoBehaviour
{
    public GoogleVisionAPIConnector _GoogleVisionAPIConnector;
    public MarkerDetection _MarkerDetection;

    private Texture2D imageTexture = null;

    public void DetectText(int imageWidth, int imageHeight, System.IntPtr imageY, int imageYRowStride)
    {
        byte[] jpg = ConvertToJPG(imageWidth, imageHeight, imageY, imageYRowStride);
        Debug.Log("Sending JPG");
        _GoogleVisionAPIConnector.SendJPG(jpg);
    }

    /*
        Method called by Google Vision API connector when the Google Cloud Vision server
        responded with the scanned text
    */
    public void ReceiveTextList(List<string> textList)
    {
        _MarkerDetection.ReceiveResponse(textList);
    }

    /*  Converts CamerImageBytes array YUV format into a JPG array using only the Y channel of the image.
        1. The image y-channel is loaded into an byte array using Marshal.copy. 
        2. The byte array is loaded into a Texture2D (to convert it later)
        3. The Texture2D needs to be flipped on the horizontal axis to maintain readability of the text
        4. The Texture is then converted into a jpg array using Texture2D.EncodeToJPG()
    
    */
    private byte[] ConvertToJPG(int imageWidth, int imageHeight, System.IntPtr imageY, int imageYRowStride)
    {
        if (imageTexture == null) imageTexture = new Texture2D(imageWidth, imageHeight, TextureFormat.R8, false, false);

        int arrayLength = (imageHeight * imageYRowStride);
        byte[] managedArray = new byte[arrayLength];
        Marshal.Copy(imageY, managedArray, 0, arrayLength);
        imageTexture.LoadRawTextureData(managedArray);

        var flippedTexture = HelperFunctions.FlipTexture2D(imageTexture, true, false);
        imageTexture = flippedTexture;

        imageTexture.Apply();
        return imageTexture.EncodeToJPG();
    }
}
