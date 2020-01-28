using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPortalBullet : MonoBehaviour
{
    public LayerMask WhatPortalShouldStick;
    public Collider PlayerCollider;
    public GameObject PortalBullet;
    public float BulletSpeed;
    public Color bluePortalColor, orangePortalColor;
    public Transform BluePortalInScene, OrangePortalInScene;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject NewBullet = Instantiate(PortalBullet, transform.position, Quaternion.identity);
            Physics.IgnoreCollision(NewBullet.GetComponent<Collider>(), PlayerCollider);
            NewBullet.GetComponent<Rigidbody>().velocity = transform.forward.normalized * BulletSpeed;
            NewBullet.GetComponent<PortalBullet>().color = bluePortalColor;

            RaycastHit rch;
            if (Physics.Raycast(transform.position, transform.forward, out rch, 999f, WhatPortalShouldStick))
                StartCoroutine(WaitForAppearPortal(rch, 0));
        }

        if (Input.GetMouseButtonDown(1))
        {
            GameObject NewBullet = Instantiate(PortalBullet, transform.position, Quaternion.identity);
            Physics.IgnoreCollision(NewBullet.GetComponent<Collider>(), PlayerCollider);
            NewBullet.GetComponent<Rigidbody>().velocity = transform.forward.normalized * BulletSpeed;
            NewBullet.GetComponent<PortalBullet>().color = orangePortalColor;

            RaycastHit rch;
            if (Physics.Raycast(transform.position, transform.forward, out rch, 999f, WhatPortalShouldStick))
                StartCoroutine(WaitForAppearPortal(rch, 1));
        }
    }

    IEnumerator WaitForAppearPortal(RaycastHit rch, int portal)
    {
        yield return new WaitForSeconds(Vector3.Distance(transform.position, rch.point) / BulletSpeed);
        if (portal == 0)
        {
            BluePortalInScene.position = rch.point + rch.normal.normalized * 0.01f;
            BluePortalInScene.forward = rch.normal;
            BluePortalInScene.GetComponent<PortalScript>().StickingSurface.gameObject.layer = LayerMask.NameToLayer("Stickable");
            BluePortalInScene.GetComponent<PortalScript>().StickingSurface = rch.collider;
        }
        else
        {
            OrangePortalInScene.position = rch.point + rch.normal.normalized * 0.01f;
            OrangePortalInScene.forward = rch.normal;
            OrangePortalInScene.GetComponent<PortalScript>().StickingSurface.gameObject.layer = LayerMask.NameToLayer("Stickable");
            OrangePortalInScene.GetComponent<PortalScript>().StickingSurface = rch.collider;
            }
    }
}
