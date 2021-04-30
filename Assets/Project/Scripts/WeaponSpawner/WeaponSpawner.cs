using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class WeaponSpawner : MonoBehaviourPun
{
    [Header("Guns")]
    public GameObject[] gunList;
    GameObject displayedGun;
    public GameObject curDisplayedGun;
    public int respawnTime;

    [Header("Objects")]
    public GameObject gunContainer;
    public GameObject pickUpText;//maybe change the text to say pick up the gun name
    //public TextMeshPro pickUpTMP;
    public float textRotSpeed;

    bool playerIsHere;
    PlayerScript nearbyPlayer;

    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsMasterClient)
            SpawnNewGun();
    }
    private void Update()
    {
        if (playerIsHere)
        {
            if (Input.GetKeyDown(KeyCode.E) && curDisplayedGun != null)
            {
                string name = curDisplayedGun.name.Replace("Disp ", "");
                name = name.Replace("(Clone)", "");
                nearbyPlayer.SpawnGun(name);
                photonView.RPC("RemoveDisplayGun", RpcTarget.MasterClient);
                pickUpText.SetActive(false);
            }
        }
    }
    [PunRPC]
    void RemoveDisplayGun()
    {
        PhotonNetwork.Destroy(curDisplayedGun);
        Invoke("SpawnNewGun", respawnTime);
    }
    public void SpawnNewGun()
    {
        displayedGun = gunList[Random.Range(0, gunList.Length)];
        GameObject obj = PhotonNetwork.Instantiate(displayedGun.name, gunContainer.transform.position, Quaternion.identity);
        photonView.RPC("SetChild", RpcTarget.All, obj.GetPhotonView().ViewID, gunContainer.GetPhotonView().ViewID, photonView.ViewID);
    }
    [PunRPC]
    void SetChild(int weaponID, int containerID, int weaponSpawnerID)
    {
        PhotonView.Find(weaponID).transform.parent = PhotonView.Find(containerID).transform;
        PhotonView.Find(weaponSpawnerID).GetComponent<WeaponSpawner>().curDisplayedGun = PhotonView.Find(weaponID).gameObject;
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.parent.gameObject.GetPhotonView().IsMine && curDisplayedGun != null)
        {
            pickUpText.SetActive(true);
            nearbyPlayer = other.transform.parent.parent.GetComponent<PlayerScript>();
            playerIsHere = true;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.parent.parent.gameObject.GetPhotonView().IsMine)
        {
            Vector3 lookPos = other.transform.position - pickUpText.transform.position;
            Quaternion rot = Quaternion.LookRotation(lookPos);
            pickUpText.transform.rotation = Quaternion.Slerp(pickUpText.transform.rotation, rot, Time.deltaTime * textRotSpeed);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.parent.gameObject.GetPhotonView().IsMine)
        {
            pickUpText.SetActive(false);
            playerIsHere = false;
        }
    }
}
