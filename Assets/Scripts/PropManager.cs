using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropManager : MonoBehaviour
{
    public float PickUpDist;
    public Transform HoldingObj;
    public Vector3 PickUpPos;
    public float throwForce;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (HoldingObj == null)
            {
                RaycastHit rch;
                if (Physics.Raycast(transform.position, transform.forward, out rch, PickUpDist))
                {
                    Transform hit = rch.collider.transform;
                    if (hit.tag == "Props")
                    {
                        HoldingObj = hit;
                        hit.parent = transform;
                        hit.rotation = Quaternion.identity;
                        hit.localPosition = PickUpPos;
                        hit.GetComponent<Rigidbody>().isKinematic = true;
                        hit.GetComponent<Collider>().enabled = false;
                    }
                }
            }
            else
            {
                HoldingObj.parent = null;
                HoldingObj.GetComponent<Rigidbody>().isKinematic = false;
                HoldingObj.GetComponent<Collider>().enabled = true;
                HoldingObj = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && HoldingObj != null)
        {
            HoldingObj.parent = null;
            HoldingObj.GetComponent<Rigidbody>().isKinematic = false;
            HoldingObj.GetComponent<Collider>().enabled = true;
            HoldingObj.GetComponent<Rigidbody>().AddForce(transform.forward.normalized * throwForce);
            HoldingObj = null;
        }
    }
}
