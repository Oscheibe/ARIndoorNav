using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This script makes every child GameObject invisible
 */
public class InvisibleScript : MonoBehaviour
{
    public Material invisibleMaterial;
    // Start is called before the first frame update
    void Start()
    {
        ChangeChildrenMaterial(invisibleMaterial);
    }

    private void ChangeChildrenMaterial(Material invisMat)
    {
        List<Material> invisMaterialList = new List<Material>();
        invisMaterialList.Add(invisMat);

        int numOfChildren = transform.childCount;
        for (int i = 0; i < numOfChildren; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            child.GetComponent<Renderer>().materials = invisMaterialList.ToArray();
        }
    }
}
