using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Player Stuff")]
    public int maxPlayer = 8;
    public TeamManager[] playerTeams;
    //instance
    public static NetworkManager instance;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        //connect to master server
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }
    public void CreateRoom(string roomName)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)maxPlayer;
        PhotonNetwork.CreateRoom(roomName,options);
    }
    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }
    // called when we disconnect from the photon server
    public override void OnDisconnected(DisconnectCause cause)
    {
        // load the menu scene
        SceneManager.LoadScene("Menu");
    }
}
