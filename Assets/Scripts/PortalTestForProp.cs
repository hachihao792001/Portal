using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTestForProp : MonoBehaviour
{
    private void Update()
    {
        Collider[] inside = Physics.OverlapBox(transform.position, new Vector3(2, 2, 2));
        foreach(Collider c in inside)
        {
            if (c.tag == "Props")
            {
                transform.parent.SendMessage("FlyingPropDetected");
            }
        }
    }
}
