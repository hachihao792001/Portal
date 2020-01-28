using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    public int Kind;
    public Camera ThisPortalCamera;
    public LayerMask GothroughPortal, Everything, SameStickingSurface, DifferentStickingSurface;
    public Transform FPS;
    public Transform OtherCamera;
    public float playerRadius;
    public Collider StickingSurface;
    public bool justTeleported = false;
    public Transform TargetProp;

    private void Start()
    {
        InvokeRepeating("ChangeStickingSurfaceLayer", 0f, 1f);
        InvokeRepeating("CheckSameStickingSurfaceWithOtherPortal", 0f, 1f);
    }
    private void ChangeStickingSurfaceLayer()
    {
        if(Kind==0)
            StickingSurface.gameObject.layer = LayerMask.NameToLayer("BlueStickingSurface");
        else if(Kind==1)
            StickingSurface.gameObject.layer = LayerMask.NameToLayer("OrangeStickingSurface");
    }

    void CheckSameStickingSurfaceWithOtherPortal()
    {
        if(StickingSurface.GetInstanceID() == OtherCamera.parent.GetComponent<PortalScript>().StickingSurface.GetInstanceID())
            ThisPortalCamera.cullingMask = SameStickingSurface;
        else
            ThisPortalCamera.cullingMask = DifferentStickingSurface;
    }

    // Update is called once per frame
    void Update()
    {
        //Set pos of the other camera
        Vector3 PlayerPos_ThisPortal = transform.InverseTransformPoint(FPS.position);
        PlayerPos_ThisPortal = Vector3.Scale(PlayerPos_ThisPortal, new Vector3(-1, 1, -1));
        OtherCamera.localPosition = PlayerPos_ThisPortal;

        //Set look rotation of the other camera
        Vector3 FPSLookRota_ThisPortal = transform.InverseTransformDirection(FPS.forward);
        FPSLookRota_ThisPortal = Vector3.Scale(FPSLookRota_ThisPortal, new Vector3(-1, 1, -1));
        OtherCamera.localRotation = Quaternion.LookRotation(FPSLookRota_ThisPortal);

        if (TargetProp != null)
        {
            if (GetComponent<Collider>().bounds.Contains(TargetProp.TransformPoint(TargetProp.GetComponent<BoxCollider>().center)))
            {
                //Before teleport
                OtherCamera.parent.SendMessage("JustTeleported");
                OtherCamera.parent.GetComponent<PortalScript>().StickingSurface.enabled = false;
                Debug.Log(Kind + "stickingsurface false 58");

                //Set Props rotation
                Vector3 PropForward_ThisPortal = transform.InverseTransformDirection(TargetProp.forward);
                PropForward_ThisPortal = Vector3.Scale(PropForward_ThisPortal, new Vector3(-1, 1, -1));
                TargetProp.forward = OtherCamera.parent.TransformDirection(PropForward_ThisPortal);

                //Set Props position (Teleport)
                Vector3 Prop_ThisPortal = transform.InverseTransformPoint(TargetProp.position);
                Prop_ThisPortal = Vector3.Scale(Prop_ThisPortal, new Vector3(-1, 1, -1));
                TargetProp.position = OtherCamera.parent.TransformPoint(Prop_ThisPortal);

                //Set Props velocity
                Vector3 PropV_ThisPortal = transform.InverseTransformDirection(TargetProp.GetComponent<Rigidbody>().velocity);
                PropV_ThisPortal = Vector3.Scale(PropV_ThisPortal, new Vector3(-1, 1, -1));
                TargetProp.GetComponent<Rigidbody>().velocity = OtherCamera.parent.TransformDirection(PropV_ThisPortal);

                

                TargetProp = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StickingSurface.enabled = false;
            Debug.Log(Kind + "stickingsurface false 88");
        }

        if (other.tag == "PlayerCenter" && !justTeleported)
        {
            //Before teleport
            OtherCamera.parent.SendMessage("JustTeleported");
            OtherCamera.parent.GetComponent<PortalScript>().StickingSurface.enabled = false;
            Debug.Log(Kind + "stickingsurface false 95");

            //Set Player look rotation
            if (Mathf.Abs(OtherCamera.parent.forward.y) < 0.1f && Mathf.Abs(transform.forward.y) < 0.1f)
            {
                Vector3 PlayerLookDir_ThisPortal = transform.InverseTransformDirection(FPS.parent.forward);
                PlayerLookDir_ThisPortal = Vector3.Scale(PlayerLookDir_ThisPortal, new Vector3(-1, 0, -1));
                FPS.parent.forward = OtherCamera.parent.TransformDirection(PlayerLookDir_ThisPortal);
            }
            else
            {
                //FPS.SendMessage("PauseLerping");
                StartCoroutine(LiePortal_FPSEuler());
                Vector3 PlayerLookDir_ThisPortal = transform.InverseTransformDirection(FPS.parent.forward);
                PlayerLookDir_ThisPortal = Vector3.Scale(PlayerLookDir_ThisPortal, new Vector3(-1, 1, -1));
                FPS.parent.forward = OtherCamera.parent.TransformDirection(PlayerLookDir_ThisPortal);
                
                FPS.parent.GetComponent<Rigidbody>().AddForce(OtherCamera.parent.forward.normalized * 300);
            }

            //Set Player position (Teleport)
            Vector3 Player_ThisPortal = transform.InverseTransformPoint(FPS.parent.position);
            Player_ThisPortal = Vector3.Scale(Player_ThisPortal, new Vector3(-1, 1, -1));
            FPS.parent.position = OtherCamera.parent.TransformPoint(Player_ThisPortal);
            
            //Set Player velocity
            Vector3 PlayerV_ThisPortal = transform.InverseTransformDirection(FPS.parent.GetComponent<Rigidbody>().velocity);
            PlayerV_ThisPortal = Vector3.Scale(PlayerV_ThisPortal, new Vector3(-1, 1, -1));
            FPS.parent.GetComponent<Rigidbody>().velocity = OtherCamera.parent.TransformDirection(PlayerV_ThisPortal);

            
        }

        if (other.tag == "Props" && !justTeleported)
        {
            TargetProp = other.transform;
        }
    }

    IEnumerator LiePortal_FPSEuler()
    {
        Vector3 FPSLookDir_ThisPortal = transform.InverseTransformDirection(FPS.forward);
        FPSLookDir_ThisPortal = Vector3.Scale(FPSLookDir_ThisPortal, new Vector3(-1, 1, -1));
        yield return new WaitForSeconds(0.1f);
        FPS.forward = OtherCamera.parent.TransformDirection(FPSLookDir_ThisPortal);
    }

    public void FlyingPropDetected()
    {
        if (StickingSurface.enabled == true)
        {
            StickingSurface.enabled = false;
            Debug.Log(Kind + "stickingsurface false 128");
            StartCoroutine(TurnBackStickingSurfaceOnAfterDetectedProp());
        }
    }
    IEnumerator TurnBackStickingSurfaceOnAfterDetectedProp()
    {
        yield return new WaitForSeconds(1f);
        if (StickingSurface.enabled == false)
        {
            StickingSurface.enabled = true;
            Debug.Log(Kind + "stickingsurface true 137");
        }
    }

    public void JustTeleported()
    {
        justTeleported = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "PlayerCenter")
        {
            justTeleported = false;
            FPS.GetComponent<Camera>().cullingMask = Everything;
        }
        if(other.tag == "Props")
        {
            justTeleported = false;
            StickingSurface.enabled = true;
            Debug.Log(Kind + "stickingsurface true 156");
        }
        if (other.tag == "Player")
        {
            StickingSurface.enabled = true;
            Debug.Log(Kind + "stickingsurface true 162");

        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "PlayerCenter")
            FPS.GetComponent<Camera>().cullingMask = GothroughPortal;
    }
}
