using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserPositionIdicator_Map : MonoBehaviour
{
    public GameObject _UserIndicatorCylinder;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        AdjustCylinderPos();
    }

    private void AdjustCylinderPos()
    {
        var CylinderHeight = _UserIndicatorCylinder.transform.localScale.y;
        var newPos = new Vector3(this.transform.position.x, this.transform.position.y + CylinderHeight, this.transform.position.z);

        _UserIndicatorCylinder.transform.position = newPos;
    }
}
