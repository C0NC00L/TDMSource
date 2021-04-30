using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class Health : MonoBehaviourPun
{
    [Header("HP Stats")]
    public int maxHP;
    [SerializeField]int curHP;
    bool dead;

    [Header("UI Stuff")]
    public Slider blueSlider;
    public Slider redSlider;
    public TextMeshProUGUI blueScoreText;
    public TextMeshProUGUI redScoreText;

    // Start is called before the first frame update
    void Start()
    {
        curHP = maxHP;
    }

    public void TakeDamage(int damage, int id, int teamNum)
    {
        curHP -= damage;
        photonView.RPC("UpdateHealth", RpcTarget.All, photonView.ViewID, curHP);
        if (curHP <= 0 && !dead)
        {
            dead = true;
            StartCoroutine(CallClientDie(id));
            photonView.RPC("Die", RpcTarget.All, photonView.ViewID);
            GameManager.instance.AddPoints(teamNum);//this team num is from the other team
        }
    }
    IEnumerator CallClientDie(int id)
    {
        yield return new WaitForSeconds(3);
        photonView.RPC("ClientDie", PhotonView.Find(id).Owner);
    }
    [PunRPC]
    void ClientDie()
    {
        Transform[] spawn;
        if (TeamManager.instance.teamNum == 0)
            spawn = GameObject.Find("BlueSpawnPoints").GetComponentsInChildren<Transform>();
        else
            spawn = GameObject.Find("RedSpawnPoints").GetComponentsInChildren<Transform>();
        int num = Random.Range(0, spawn.Length);
        PhotonNetwork.Instantiate("Player", spawn[num].position, Quaternion.identity);
    }
    [PunRPC]
    void Die(int id)
    {
        GameManager.instance.playerObjects.Remove(PhotonView.Find(id).gameObject);
        PlayerScript playerScript = PhotonView.Find(id).GetComponent<PlayerScript>();
        playerScript.anim.SetTrigger("Die");
        playerScript.anim.SetBool("Dead", true);
        playerScript.LeftArmRig.weight = 0;
        playerScript.ribsRig.weight = 0;
        playerScript.myCam.GetComponent<CameraController>().enabled = false;
        //playerScript.myCam.GetComponent<AudioListener>().enabled = false;//maybe put an audio listen on a game manager or somthin
        //playerScript.myCam.enabled = false;
        playerScript.anim.SetTrigger("Die");
        playerScript.enabled = false;
        Invoke("Despawn", 3.5f);
    }
    void Despawn()
    {
        if (photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    public void UpdateHealth(int id, int HP)
    {
        PhotonView.Find(id).GetComponent<Health>().curHP = HP;
    }
}
