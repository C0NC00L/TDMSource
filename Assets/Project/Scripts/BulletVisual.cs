using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BulletVisual : MonoBehaviourPun
{
    [Header("Components")]
    public Rigidbody rig;
    public float speed;
    public float timeTillDestroy;


    // Start is called before the first frame update
    void Start()
    {
        //rig.velocity = transform.forward * speed;
        rig.velocity = transform.forward * speed;
        Invoke("DestroyThis", timeTillDestroy);
    }
    void DestroyThis()
    {
        if(photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
