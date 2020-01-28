using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{
    Rigidbody rb;

    [Header("Mouse")]
    public float sentivity;
    public float yaw, pitch;

    [Space]
    [Header("Walking & Running")]
    public Vector2 input;
    public float walkSpeed;
    public bool running;

    [Space]
    [Header("Jumping")]
    public float groundDistance;
    public bool grounded;
    public float jumpForce;

    bool lerping = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = transform.parent.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (lerping)
        {
            transform.parent.rotation = Quaternion.Lerp(transform.parent.rotation, Quaternion.Euler(0, transform.parent.eulerAngles.y, 0), 0.2f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(transform.localEulerAngles.x, 0, 0), 0.2f);
        }
        else
        {
            Debug.Log("no lerp");
        }
        //Mouse
        /*
        yaw -= Input.GetAxis("Mouse Y") * sentivity;
        pitch += Input.GetAxis("Mouse X") * sentivity;

        transform.parent.eulerAngles = new Vector3(0, pitch, 0);
        transform.eulerAngles = new Vector3(yaw, pitch, 0);
        */
        transform.parent.Rotate(Vector3.up * Input.GetAxis("Mouse X") * sentivity);
        transform.Rotate(Vector3.left * Input.GetAxis("Mouse Y") * sentivity);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);

        //Walking & Running
        running = Input.GetKey(KeyCode.LeftControl);
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        input *= walkSpeed * (running ? 2 : 1);
        rb.velocity = transform.parent.TransformDirection(new Vector3(input.x, rb.velocity.y, input.y));

        //Jumping
        grounded = Physics.Raycast(transform.parent.position, Vector3.down, groundDistance);
        if(grounded && Input.GetKeyDown(KeyCode.Space))
            rb.AddForce(Vector3.up * jumpForce);
    }

    public void PauseLerping()
    {
        Debug.Log("paseu lerpng");
        StartCoroutine(pauselerping());
    }
    IEnumerator pauselerping()
    {
        lerping = false;
        yield return new WaitForSeconds(0.1f);
        lerping = true;
    }
}
