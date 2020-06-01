using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This script makes every child GameObject invisible
 */
public class InvisibleScript : MonoBehaviour
{
    public Material _invisibleMaterial;
    public Material _visibleMaterial;
    public Material _ConcreteMaterial;

    public bool _invisibleMaterialBool;
    public bool _visibleMaterialBool;
    public bool _ConcreteMaterialBool;

    public List<GameObject> _walls;

    private Material activeMaterial;
    private bool isInvisible = false;

    // Start is called before the first frame update
    void Start()
    {
        if (_invisibleMaterialBool)
            activeMaterial = _invisibleMaterial;

        if (_visibleMaterialBool)
            activeMaterial = _visibleMaterial;

        if (_ConcreteMaterialBool)
            activeMaterial = _ConcreteMaterial;

        foreach (var wall in _walls)
        {
            ChangeChildrenMaterial(activeMaterial, wall);
        }
    }

    private void ChangeChildrenMaterial(Material invisMat, GameObject wall)
    {
        List<Material> invisMaterialList = new List<Material>();
        invisMaterialList.Add(invisMat);

        int numOfChildren = wall.transform.childCount;
        for (int i = 0; i < numOfChildren; i++)
        {
            GameObject child = wall.transform.GetChild(i).gameObject;
            child.GetComponent<Renderer>().materials = invisMaterialList.ToArray();
        }
    }

    /**
     * makes the GameObjects invisible / visible
     */
    public void InvisibleWallsOnOff()
    {
        if (isInvisible)
        {
            foreach (var wall in _walls)
            {
                ChangeChildrenMaterial(activeMaterial, wall);
            }
            isInvisible = false;
        }
        else
        {
            foreach (var wall in _walls)
            {
                ChangeChildrenMaterial(_invisibleMaterial, wall);
            }
            isInvisible = true;
        }
    }
}
