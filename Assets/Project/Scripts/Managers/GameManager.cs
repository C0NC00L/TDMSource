using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPun
{
    /*
    private int experience;
    //Experience is a basic property
    public int Experience
    {
        get
        {
            //Some other code
            return experience;
        }
        set
        {
            //Some other code
            experience = value;
        }
    }
    */
    [Header("Player Stuff")]
    public List<GameObject> playerObjects = new List<GameObject>();
    public PlayerScript localPlayer;

    [Header("Game Stuff")]
    public int pointsToWin;
    [HideInInspector]public int bluePoints;
    [HideInInspector]public int redPoints;
    [HideInInspector]public bool gameOver;
    [HideInInspector]public int wonTeam;
    public GameObject curPlayerGun;

    public static GameManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        Transform[] spawn;
        if (TeamManager.instance.teamNum == 0)
            spawn = GameObject.Find("BlueSpawnPoints").GetComponentsInChildren<Transform>();
        else
            spawn = GameObject.Find("RedSpawnPoints").GetComponentsInChildren<Transform>();

        int num = Random.Range(0, spawn.Length);
        PhotonNetwork.Instantiate("Player", spawn[num].position, Quaternion.identity);
    }
    public void AddPoints(int teamNum)
    {
        if(teamNum == 0)
        {
            bluePoints++;
            if (bluePoints >= pointsToWin)
                photonView.RPC("WinGame", RpcTarget.All, 0);
        }
        else
        {
            redPoints++;
            if (redPoints >= pointsToWin)
                photonView.RPC("WinGame", RpcTarget.All, 1);
        }
        photonView.RPC("UpdateScoreUI", RpcTarget.All, bluePoints, redPoints);
    }
    [PunRPC]
    public void UpdateScoreUI(int newBluePoints, int newRedPoints)
    {
        bluePoints = newBluePoints;
        redPoints = newRedPoints;

        localPlayer.blueScoreText.text = bluePoints + " / " + pointsToWin;
        localPlayer.redScoreText.text = redPoints + " / " + pointsToWin;
        localPlayer.blueSlider.value = (float)bluePoints / pointsToWin;
        localPlayer.redSlider.value = (float)redPoints / pointsToWin;
    }
    [PunRPC]
    void WinGame(int teamNum)
    {
        wonTeam = teamNum;
        gameOver = true;
        localPlayer.DisplayeWinText(teamNum);
        StartCoroutine(SendBackToMenu());
    }
    IEnumerator SendBackToMenu()
    {
        yield return new WaitForSeconds(7);
        SceneManager.LoadScene("Menu");
    }
}
