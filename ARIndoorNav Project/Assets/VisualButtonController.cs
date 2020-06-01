using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualButtonController : MonoBehaviour
{

    public Sprite _EyeOpen;
    public Sprite _EyeClosed;

    private Sprite currentImage;

    // Start is called before the first frame update
    void Start()
    {
        currentImage = GetComponent<Image>().sprite;
    }

    public void SwitchImage()
    {
        if (currentImage.name == _EyeOpen.name)
        {
            currentImage = _EyeClosed;
        }
        else
            currentImage = _EyeOpen;

        GetComponent<Image>().sprite = currentImage;
    }

}
