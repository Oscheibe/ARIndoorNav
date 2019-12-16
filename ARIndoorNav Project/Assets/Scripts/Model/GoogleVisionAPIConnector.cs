using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using SimpleJSON;
using UnityEngine.UI;

public class GoogleVisionAPIConnector : MonoBehaviour
{
    //public TextDetection _TextDetection;
    public string url = "https://vision.googleapis.com/v1/images:annotate?key=";
    public string apiKey = "";

    private Dictionary<string, string> headers;
    private FeatureType featureType = FeatureType.TEXT_DETECTION;
    private int maxResults = 10;

    // Start is called before the first frame update
    void Start()
    {
        headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json; charset=UTF-8");

        if (apiKey == null || apiKey == "")
            Debug.LogError("No API key. Please set your API key into the \"Web Cam Texture To Cloud Vision(Script)\" component.");

    }


    public IEnumerator SendJPG(byte[] jpg)
    {
        if (this.apiKey == null)
            yield return null;

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
                    Debug.Log("Parsing Data");
                    Debug.Log("WWW Text: " + www.text.Replace("\n", "").Replace(" ", ""));
                    //AnnotateImageResponses responses = JsonUtility.FromJson<AnnotateImageResponses>(www.text);
                    // SendMessage, BroadcastMessage or someting like that.
                    var response = JSON.Parse(www.text);


                    //DisplayJSONTextResult(response);
                }
                else
                {
                    Debug.Log("Error: " + www.error);
                }
            }
        }
#endif


    }

    //private IEnumerator Capture(byte[] jpg) {}

    public class AnnotateImageRequests
    {
        public List<AnnotateImageRequest> requests;
    }
    public class AnnotateImageRequest
    {
        public Image image;
        public List<Feature> features;
    }
    public class Feature
    {
        public string type;
        public int maxResults;
    }
    public class Image
    {
        public string content;
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

    // Functions to test this script below: 

    public void TestConnector()
    {
        var tmpTexture = new Texture2D(640, 480, TextureFormat.R8, false, false);
        var fileData = File.ReadAllBytes("Assets/Resources/3.219.jpg");
        tmpTexture.LoadImage(fileData);
        tmpTexture.Apply();

        byte[] jpg = tmpTexture.EncodeToJPG();
        SendJPG(jpg);
    }

    private void SendResponse(List<string> textList)
    {
        foreach (var text in textList)
        {
            Debug.Log("Text; " + text);
        }
    }
}
