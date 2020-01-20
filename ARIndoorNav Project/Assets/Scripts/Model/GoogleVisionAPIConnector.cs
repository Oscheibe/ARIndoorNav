using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.UI;

public class GoogleVisionAPIConnector : MonoBehaviour
{
    public TextDetection _TextDetection;


    //public TextDetection _TextDetection;
    public string url = "https://vision.googleapis.com/v1/images:annotate?key=";
    public string apiKeyResourceLocation = "APIKeys/CloudVision";

    //public RawImage _texturePlane;
    private string apiKey = "";
    private Dictionary<string, string> headers;
    private FeatureType featureType = FeatureType.TEXT_DETECTION;
    private int maxResults = 10;
    private bool receivedResult = false;

    private byte[] jpg;

    // Start is called before the first frame update
    void Start()
    {
        TextAsset apiKeyText = Resources.Load(apiKeyResourceLocation) as TextAsset;
        apiKey = apiKeyText.text;

        headers = new Dictionary<string, string>();
        headers.Add("Content-Type", "application/json; charset=UTF-8");

        if (apiKey == null || apiKey == "")
            Debug.LogError("No API key. Please set your API key into the \"GoogleVisionAPIConnector(Script)\" component.");

    }

    public void SendJPG(byte[] imageJPG)
    {
        jpg = imageJPG;
        StartCoroutine("SendAPIRequest");
    }


    public IEnumerator SendAPIRequest()
    {
        receivedResult = false;
        while (!receivedResult)
        {
            if (this.apiKey == null)
            {
                Debug.Log("No API key");
                yield return null;
            }
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
                Debug.Log("Sending Request to Google Vision API");
                string url = this.url + this.apiKey;
                byte[] postData = System.Text.Encoding.Default.GetBytes(jsonData);
                using (WWW www = new WWW(url, postData, headers))
                {
                    yield return www;
                    if (string.IsNullOrEmpty(www.error))
                    {
                        receivedResult = true;

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
            else
            {
                Debug.Log("JSON Data string is empty!");
            }
#endif

            yield return new WaitForSeconds(5.0f);
        }
    }


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
    public class Feature
    {
        public string type;
        public int maxResults;
    }
    [System.Serializable]
    public class Image
    {
        public string content;
    }
    [System.Serializable]
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


    private void SendResponse(List<string> textList)
    {
        foreach (var text in textList)
        {
            Debug.Log("Text; " + text);
        }
    }


}
