using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalBullet : MonoBehaviour
{
    public Color color;

    private void Start()
    {
        StartCoroutine(WaitForDisappear());
    }
    IEnumerator WaitForDisappear()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    void Update()
    {
        ParticleSystem.MainModule main = GetComponent<ParticleSystem>().main;
        main.startColor = color;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
