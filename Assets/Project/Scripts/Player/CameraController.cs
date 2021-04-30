using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraController : MonoBehaviourPun
{
    [Header("Stats")]
    public float sensx;
    public float sensy;
    public float maxy;
    public float miny;
    public bool isSpectator;
    public float SpectatorMoveSpeed;

    float rotx;
    float roty;

    public void LateUpdate()
    {
        //get mouse inputs
        rotx += Input.GetAxis("Mouse X") * sensx;
        roty += Input.GetAxis("Mouse Y") * sensy;
        //clamp rotation
        roty = Mathf.Clamp(roty, miny, maxy);
        //are we spectating?
        if (isSpectator)
        {
            //rotate camera vertically
            transform.rotation = Quaternion.Euler(-roty, rotx, 0);
            //move camera
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            float y = 0;
            if (Input.GetKey(KeyCode.E))
                y = 1;
            else if (Input.GetKey(KeyCode.Q))
                y = -1;
            Vector3 dir = transform.right * x + transform.up * y + transform.forward * z;
            transform.position += dir * SpectatorMoveSpeed * Time.deltaTime;
        }
        else
        {
            //rotate camera vertically
            transform.localRotation = Quaternion.Euler(-roty, 0, 0);
            //rotate player horizontally
            transform.parent.rotation = Quaternion.Euler(0, rotx, 0);
        }
    }
}
