using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RibTargetMover : MonoBehaviourPun
{
    [Header("Stats")]
    public float sensy;
    public float maxy;
    public float miny;
    float roty;

    public void LateUpdate()
    {
        if(photonView.IsMine)
            photonView.RPC("LookFunc", RpcTarget.All, photonView.ViewID, Input.GetAxis("Mouse Y") * sensy);
    }
    [PunRPC]
    public void LookFunc(int id, float _rotY)
    {
        //get mouse inputs
        roty += _rotY;
        //clamp rotation
        roty = Mathf.Clamp(roty, miny, maxy);
        //rotate camera vertically
        PhotonView.Find(id).transform.localRotation = Quaternion.Euler(-roty, 0, 0);
    }
}
