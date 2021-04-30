using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Gun : MonoBehaviourPun
{
    [Header("Gun Stats")]
    public int gunDamage;
    public int Range;
    public float fireSpeed;
    public float bulletSpread;
    public float bulletSpreadReduction;
    public float zoomedCrossHairSize;
    [HideInInspector]public float fireSpeedTimer;
    public LayerMask ignoredMask;

    [Header("Hand Alignment")]
    public Transform start;
    public Transform end;
    public Transform startPos;

    [Header("Components")]
    [HideInInspector]public PlayerScript localPlayer;
    [HideInInspector]public RectTransform crossHair;
    [HideInInspector]public Camera myCam;

    [HideInInspector] public Vector3 shootTarget;

    //can add keyword virtual before the return type then add in that method with an ovveride in a subclass to add more functionality
    private void Start()
    {
        localPlayer = GameManager.instance.localPlayer;
        crossHair = localPlayer.crossHair;
        myCam = localPlayer.myCam;
        transform.localPosition = startPos.localPosition;
    }
    public void Fire()
    {
    }
    public void Aim()
    {
    }
}
