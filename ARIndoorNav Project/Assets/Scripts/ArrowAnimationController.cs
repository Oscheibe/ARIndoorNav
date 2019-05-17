using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAnimationController : MonoBehaviour
{

    private List<GameObject> arrows = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        
        int children = this.transform.childCount;
        for (int i = 0; i < children; i++)
        {           
            arrows.Add(this.transform.GetChild(i).gameObject);
        }

        InvokeRepeating("AnimateArrow0", 1.0f, 1f);
        InvokeRepeating("AnimateArrow1", 1.25f, 1f);
        InvokeRepeating("AnimateArrow2", 1.5f, 1f);
        arrows[2].transform.localScale += new Vector3(0.3F, 0.3F, 0.3F);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void AnimateArrow0()
    {
        arrows[2].transform.localScale -= new Vector3(0.3F, 0.3F, 0.3F);
        arrows[0].transform.localScale += new Vector3(0.3F, 0.3F, 0.3F);
    }

    
    void AnimateArrow1()
    {
        arrows[0].transform.localScale -= new Vector3(0.3F, 0.3F, 0.3F);
        arrows[1].transform.localScale += new Vector3(0.3F, 0.3F, 0.3F);
    }

    
    void AnimateArrow2()
    {
        arrows[1].transform.localScale -= new Vector3(0.3F, 0.3F, 0.3F);
        arrows[2].transform.localScale += new Vector3(0.3F, 0.3F, 0.3F);
    }

}
