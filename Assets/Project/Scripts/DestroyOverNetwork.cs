using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DestroyOverNetwork : MonoBehaviourPun
{
    [Header("Times")]
    public float timeToDestroy;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyThis", timeToDestroy);
    }

    void DestroyThis()
    {
        if(photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
