// Source: https://github.com/comoc/UnityCloudVision
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System;
using SimpleJSON;

public class WebCamTextureToCloudVision : MonoBehaviour
{

    public string url = "https://vision.googleapis.com/v1/images:annotate?key=";
    public string apiKey = "";
    public float captureIntervalSeconds = 2.0f;
    //public int requestedWidth = 640;
    //public int requestedHeight = 480;
    public FeatureType featureType = FeatureType.TEXT_DETECTION;
    public int maxResults = 10;

    public RawImage _texturePlane;
    public RectTransform _scanRect;

    private bool runVision = false;
    private CameraImageBytes image;
    private Texture2D imageTexture = null;
    private int imageWidth, imageHeight;
    private byte[] imageBytes;

    //WebCamTexture webcamTexture;
    //Texture2D texture2D;
    Dictionary<string, string> headers;

    public void StartStop()
    {
        // Stop
        if (runVision)
        {
            runVision = false;
        }
        // Start
        else
        {
            runVision = true;
            StartCoroutine("Capture");
        }
    }


    // Use this for initialization
    void Start()
    {
        headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json; charset=UTF-8");

        if (apiKey == null || apiKey == "")
            Debug.LogError("No API key. Please set your API key into the \"Web Cam Texture To Cloud Vision(Script)\" component.");

    }

    private bool SetWebCamTexture()
    {
        image = Frame.CameraImage.AcquireCameraImageBytes();
        if (!image.IsAvailable)
        {
            Debug.Log("Couldn't set texture");
            return false;
        }
        if (imageTexture == null) imageTexture = new Texture2D(image.Width, image.Height, TextureFormat.R8, false, false);

        /*
        WebCamDevice[] devices = WebCamTexture.devices;
        WebCamTexture webcamTexture;
        webcamTexture = new WebCamTexture(devices[0].name, requestedWidth, requestedHeight);
        webcamTexture.Play();
        Color[] pixels = webcamTexture.GetPixels();
        imageTexture.SetPixels(pixels);
        webcamTexture.Stop();

        _canvas.enabled = false;
        new WaitForEndOfFrame();
        imageTexture.ReadPixels(_canvas.GetComponent<Rect>(), 0, 0);
        _canvas.enabled = true;

        imageTexture.ReadPixels(_scanRect.GetComponent<Rect>(), 0, 0);


        */

        
        int arrayLength = (image.Height * image.YRowStride);
        byte[] managedArray = new byte[arrayLength];
        Marshal.Copy(image.Y, managedArray, 0, arrayLength);
        imageTexture.LoadRawTextureData(managedArray);
        
        var flippedTexture = HelperFunctions.FlipTexture2D(imageTexture, true, false);
        imageTexture = flippedTexture;

        imageTexture.Apply();
        _texturePlane.texture = imageTexture;

        return true;
    }

    private IEnumerator Capture()
    {
        while (runVision)
        {
            if (this.apiKey == null)
                yield return null;
            /*
            UnityEngine.Color[] pixels = webcamTexture.GetPixels();
            if (pixels.Length == 0)
                yield return null;
            if (texture2D == null || webcamTexture.width != texture2D.width || webcamTexture.height != texture2D.height)
            {
                texture2D = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGBA32, false);
            }

            texture2D.SetPixels(pixels);
            // texture2D.Apply(false); // Not required. Because we do not need to be uploaded it to GPU
            */
            yield return new WaitForEndOfFrame();
            var success = SetWebCamTexture();
            if (!success) continue;
            
            //yield return new WaitForSeconds(captureIntervalSeconds);
            //continue;
            


            byte[] jpg = imageTexture.EncodeToJPG();
            string base64 = System.Convert.ToBase64String(jpg);

#if UNITY_WEBGL
			Application.ExternalCall("post", this.gameObject.name, "OnSuccessFromBrowser", "OnErrorFromBrowser", this.url + this.apiKey, base64, this.featureType.ToString(), this.maxResults);
#else

            AnnotateImageRequests requests = new AnnotateImageRequests();
            requests.requests = new List<AnnotateImageRequest>();

            AnnotateImageRequest request = new AnnotateImageRequest();
            request.image = new Image();
            request.image.content = base64;
            request.features = new List<Feature>();

            Feature feature = new Feature();
            feature.type = this.featureType.ToString();
            feature.maxResults = this.maxResults;

            request.features.Add(feature);

            requests.requests.Add(request);

            string jsonData = JsonUtility.ToJson(requests, false);
            if (jsonData != string.Empty)
            {
                Debug.Log("Sending Request");
                string url = this.url + this.apiKey;
                byte[] postData = System.Text.Encoding.Default.GetBytes(jsonData);
                using (WWW www = new WWW(url, postData, headers))
                {
                    yield return www;
                    if (string.IsNullOrEmpty(www.error))
                    {
                        //Debug.Log("WWW Text: "+www.text.Replace("\n", "").Replace(" ", ""));
                        AnnotateImageResponses responses = JsonUtility.FromJson<AnnotateImageResponses>(www.text);
                        // SendMessage, BroadcastMessage or someting like that.
                        var response = JSON.Parse(www.text);
                        
                        TestTextRecognition(responses);
                    }
                    else
                    {
                        Debug.Log("Error: " + www.error);
                    }
                }
            }
#endif

            yield return new WaitForSeconds(captureIntervalSeconds);
        }
    }

#if UNITY_WEBGL
	void OnSuccessFromBrowser(string jsonString) {
		Debug.Log(jsonString);	
		AnnotateImageResponses responses = JsonUtility.FromJson<AnnotateImageResponses>(jsonString);
		Sample_OnAnnotateImageResponses(responses);
	}

	void OnErrorFromBrowser(string jsonString) {
		Debug.Log(jsonString);	
	}
#endif


    private void DisplayJSONTextResult(JSONNode response)
    {
        Debug.Log("Locale: " + response["responses"]["textAnnotations"]["locale"].Value);
        Debug.Log("Description: " + response["responses"]["textAnnotations"]["description"].Value);
    }
    /// <summary>
    /// A sample implementation.
    /// </summary>
    void Sample_OnAnnotateImageResponses(AnnotateImageResponses responses)
    {
        if (responses.responses.Count > 0)
        {
            if (responses.responses[0].faceAnnotations != null && responses.responses[0].faceAnnotations.Count > 0)
            {
                Debug.Log("joyLikelihood: " + responses.responses[0].faceAnnotations[0].joyLikelihood);
            }
        }
    }

    private void TestTextRecognition(AnnotateImageResponses responses)
    {
        if (responses.responses.Count > 0)
        {
            if (responses.responses[0].textAnnotations != null && responses.responses[0].textAnnotations.Count > 0)
            {
                Debug.Log("Response count: " + responses.responses.Count);
                int i = 0;
                foreach (var text in responses.responses[0].textAnnotations)
                {
                    Debug.Log(i+++": "+ text);
                }
                //Debug.Log("Locations0: "+responses.responses[0].textAnnotations[0].locations[0].ToString());
            }
        }
    }




    [System.Serializable]
    public class AnnotateImageRequests
    {
        public List<AnnotateImageRequest> requests;
    }

    [System.Serializable]
    public class AnnotateImageRequest
    {
        public Image image;
        public List<Feature> features;
    }

    [System.Serializable]
    public class Image
    {
        public string content;
    }

    [System.Serializable]
    public class Feature
    {
        public string type;
        public int maxResults;
    }

    [System.Serializable]
    public class ImageContext
    {
        public LatLongRect latLongRect;
        public List<string> languageHints;
    }

    [System.Serializable]
    public class LatLongRect
    {
        public LatLng minLatLng;
        public LatLng maxLatLng;
    }

    [System.Serializable]
    public class AnnotateImageResponses
    {
        public List<AnnotateImageResponse> responses;
    }

    [System.Serializable]
    public class AnnotateImageResponse
    {
        public List<FaceAnnotation> faceAnnotations;
        public List<EntityAnnotation> landmarkAnnotations;
        public List<EntityAnnotation> logoAnnotations;
        public List<EntityAnnotation> labelAnnotations;
        public List<TextAnnotation> textAnnotations;
    }

    [System.Serializable]
    public class FaceAnnotation
    {
        public BoundingPoly boundingPoly;
        public BoundingPoly fdBoundingPoly;
        public List<Landmark> landmarks;
        public float rollAngle;
        public float panAngle;
        public float tiltAngle;
        public float detectionConfidence;
        public float landmarkingConfidence;
        public string joyLikelihood;
        public string sorrowLikelihood;
        public string angerLikelihood;
        public string surpriseLikelihood;
        public string underExposedLikelihood;
        public string blurredLikelihood;
        public string headwearLikelihood;
    }

    [System.Serializable]
    public class TextAnnotation
    {
        public string locale;
        public List<string> description;
        public List<BoundingPoly> boundingPoly;
    }

    [System.Serializable]
    public class EntityAnnotation
    {
        public string mid;
        public string locale;
        public string description;
        public float score;
        public float confidence;
        public float topicality;
        public BoundingPoly boundingPoly;
        public List<LocationInfo> locations;
        public List<Property> properties;
    }

    [System.Serializable]
    public class BoundingPoly
    {
        public List<Vertex> vertices;
    }

    [System.Serializable]
    public class Landmark
    {
        public string type;
        public Position position;
    }

    [System.Serializable]
    public class Position
    {
        public float x;
        public float y;
        public float z;
    }

    [System.Serializable]
    public class Vertex
    {
        public float x;
        public float y;
    }

    [System.Serializable]
    public class LocationInfo
    {
        LatLng latLng;
    }

    [System.Serializable]
    public class LatLng
    {
        float latitude;
        float longitude;
    }

    [System.Serializable]
    public class Property
    {
        string name;
        string value;
    }

    public enum FeatureType
    {
        TYPE_UNSPECIFIED,
        FACE_DETECTION,
        LANDMARK_DETECTION,
        LOGO_DETECTION,
        LABEL_DETECTION,
        TEXT_DETECTION,
        SAFE_SEARCH_DETECTION,
        IMAGE_PROPERTIES
    }

    public enum LandmarkType
    {
        UNKNOWN_LANDMARK,
        LEFT_EYE,
        RIGHT_EYE,
        LEFT_OF_LEFT_EYEBROW,
        RIGHT_OF_LEFT_EYEBROW,
        LEFT_OF_RIGHT_EYEBROW,
        RIGHT_OF_RIGHT_EYEBROW,
        MIDPOINT_BETWEEN_EYES,
        NOSE_TIP,
        UPPER_LIP,
        LOWER_LIP,
        MOUTH_LEFT,
        MOUTH_RIGHT,
        MOUTH_CENTER,
        NOSE_BOTTOM_RIGHT,
        NOSE_BOTTOM_LEFT,
        NOSE_BOTTOM_CENTER,
        LEFT_EYE_TOP_BOUNDARY,
        LEFT_EYE_RIGHT_CORNER,
        LEFT_EYE_BOTTOM_BOUNDARY,
        LEFT_EYE_LEFT_CORNER,
        RIGHT_EYE_TOP_BOUNDARY,
        RIGHT_EYE_RIGHT_CORNER,
        RIGHT_EYE_BOTTOM_BOUNDARY,
        RIGHT_EYE_LEFT_CORNER,
        LEFT_EYEBROW_UPPER_MIDPOINT,
        RIGHT_EYEBROW_UPPER_MIDPOINT,
        LEFT_EAR_TRAGION,
        RIGHT_EAR_TRAGION,
        LEFT_EYE_PUPIL,
        RIGHT_EYE_PUPIL,
        FOREHEAD_GLABELLA,
        CHIN_GNATHION,
        CHIN_LEFT_GONION,
        CHIN_RIGHT_GONION
    };

    public enum Likelihood
    {
        UNKNOWN,
        VERY_UNLIKELY,
        UNLIKELY,
        POSSIBLE,
        LIKELY,
        VERY_LIKELY
    }
}
