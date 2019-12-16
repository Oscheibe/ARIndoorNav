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

    public void DetectText(CameraImageBytes image)
    {
        byte[] jpg = ConvertToJPG(image);
        _GoogleVisionAPIConnector.SendJPG(jpg);
    }

    public void ReceiveTextList(List<string> textList)
    {
        // TODO
    }

    /*  Converts CamerImageBytes array YUV format into a JPG array using only the Y channel of the image.
        1. The image y-channel is loaded into an byte array using Marshal.copy. 
        2. The byte array is loaded into a Texture2D (to convert it later)
        3. The Texture2D needs to be flipped on the horizontal axis to maintain readability of the text
        4. The Texture is then converted into a jpg array using Texture2D.EncodeToJPG()
    
    */
    private byte[] ConvertToJPG(CameraImageBytes image)
    {
        if (imageTexture == null) imageTexture = new Texture2D(image.Width, image.Height, TextureFormat.R8, false, false);

        int arrayLength = (image.Height * image.YRowStride);
        byte[] managedArray = new byte[arrayLength];
        Marshal.Copy(image.Y, managedArray, 0, arrayLength);
        imageTexture.LoadRawTextureData(managedArray);

        var flippedTexture = HelperFunctions.FlipTexture2D(imageTexture, true, false);
        imageTexture = flippedTexture;

        imageTexture.Apply();

        return imageTexture.EncodeToJPG();
    }
}
