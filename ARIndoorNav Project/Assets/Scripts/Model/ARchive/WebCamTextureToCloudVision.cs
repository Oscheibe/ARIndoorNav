// Source: https://github.com/comoc/UnityCloudVision
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GoogleARCore;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System.IO;
using SimpleJSON;
//using UnityEngine.Windows;

public class WebCamTextureToCloudVision : MonoBehaviour
{
    public TextDetection _TextDetection;

    public string url = "https://vision.googleapis.com/v1/images:annotate?key=";
    public string apiKey = "";
    public float captureIntervalSeconds = 2.0f;
    public FeatureType featureType = FeatureType.TEXT_DETECTION;
    public int maxResults = 10;

    public RawImage _texturePlane;
    public RectTransform _scanRect;
    public RawImage _testImage;
    public LineRenderer _lineRenderer;

    private string apiKeyResourceLocation = "APIKeys/CloudVision";
    private bool runVision = false;
    private CameraImageBytes image;
    private Texture2D imageTexture = null;
    private int imageWidth, imageHeight;
    private byte[] imageBytes;

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
        TextAsset apiKeyText = Resources.Load(apiKeyResourceLocation) as TextAsset;
        apiKey = apiKeyText.text;


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

            yield return new WaitForEndOfFrame();
            var success = SetWebCamTexture();
            if (!success) continue;

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
                        //Debug.Log("Parsing Data");
                        //Debug.Log("WWW Text: " + www.text.Replace("\n", "").Replace(" ", ""));
                        var response = JSON.Parse(www.text);
                        var nameList = GetDescriptionListFromJSON(response);

                        _TextDetection.ReceiveTextList(nameList);

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

    
    private List<string> GetDescriptionListFromJSON(JSONNode response)
    {
        List<string> descriptionList = new List<string>();
        int listLength = response["responses"][0]["textAnnotations"].Count;

        var description = response["responses"][0]["textAnnotations"][0]["description"].Value;
        var descriptionLines = description.Split('\n');

        foreach (var line in descriptionLines)
        {
            descriptionList.Add(line);
        }

        return descriptionList;
    }



    private void DisplayJSONTextResult(JSONNode response)
    {
        var locale = response["responses"][0]["textAnnotations"][0]["locale"].Value;
        var description1 = response["responses"][0]["textAnnotations"][0]["description"].Value;
        var description2 = response["responses"][0]["textAnnotations"][1]["description"].Value;

        var textBoxCount = 17;
        var sumVector = new Vector2[textBoxCount * 4];

        for (int i = 0; i < textBoxCount; i++)
        {
            var tmpVector = GetVector2FromJSON(response, i);
            DrawPath(tmpVector);

            for (int k = 0; k < 4; k++)
            {
                //sumVector[4*i+k] = tmpVector[k];
            }
        }

        //DrawPath(sumVector);
    }

    private void DrawPath(Vector2[] path)
    {
        var newLineRendererGO = new GameObject("LineRenderer");


        newLineRendererGO.transform.SetParent(_lineRenderer.transform);
        newLineRendererGO.AddComponent<LineRenderer>();
        var newLineRenderer = newLineRendererGO.GetComponent<LineRenderer>();
        newLineRenderer.material = _lineRenderer.material;
        newLineRenderer.widthMultiplier = _lineRenderer.widthMultiplier;
        newLineRenderer.loop = _lineRenderer.loop;
        newLineRenderer.useWorldSpace = _lineRenderer.useWorldSpace;


        if (path.Length < 2) return;
        newLineRenderer.positionCount = path.Length;
        for (int i = 0; i < path.Length; i++)
        {
            //Debug.Log("Draw line " + i + ": " + path[i]);
            newLineRenderer.SetPosition(i, path[i]);
        }

        newLineRendererGO.transform.position = Vector3.zero;
        newLineRendererGO.transform.rotation = new Quaternion();
        //_Line.sortingLayerName = "Foreground";
    }

    private Vector2[] GetVector2FromJSON(JSONNode response, int vertextNumber)
    {
        Vector2[] textBox = new Vector2[4];
        var v1 = new Vector2(response["responses"][0]["textAnnotations"][vertextNumber]["boundingPoly"]["vertices"][0]["x"].AsFloat,
                            -response["responses"][0]["textAnnotations"][vertextNumber]["boundingPoly"]["vertices"][0]["y"].AsFloat);
        var v2 = new Vector2(response["responses"][0]["textAnnotations"][vertextNumber]["boundingPoly"]["vertices"][1]["x"].AsFloat,
                            -response["responses"][0]["textAnnotations"][vertextNumber]["boundingPoly"]["vertices"][1]["y"].AsFloat);
        var v3 = new Vector2(response["responses"][0]["textAnnotations"][vertextNumber]["boundingPoly"]["vertices"][2]["x"].AsFloat,
                            -response["responses"][0]["textAnnotations"][vertextNumber]["boundingPoly"]["vertices"][2]["y"].AsFloat);
        var v4 = new Vector2(response["responses"][0]["textAnnotations"][vertextNumber]["boundingPoly"]["vertices"][3]["x"].AsFloat,
                            -response["responses"][0]["textAnnotations"][vertextNumber]["boundingPoly"]["vertices"][3]["y"].AsFloat);

        textBox[0] = v1;
        textBox[1] = v2;
        textBox[2] = v3;
        textBox[3] = v4;

        return textBox;
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
        public Vertex(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

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
