using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MainMenu : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [Header("Objects")]
    public GameObject networkManager;
    public GameObject teamManager;
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject createRoomScreeen;
    public GameObject lobbyScreen;
    public GameObject TeamTexts;
    public GameObject lobbyBrowserScreen;
    [Header("Main Screen")]
    public Button createRoomButton;
    public Button findRoomButton;
    [Header("Lobby")]
    public TextMeshPro bluePlayerListText;
    public TextMeshPro redPlayerListText;
    public TextMeshProUGUI roomInfoText;
    public Button startGameButton;
    [Header("Lobby Browser")]
    public RectTransform roomListContainer;
    public GameObject roomButtonPrefab;

    private List<GameObject> roomButtons = new List<GameObject>();
    private List<RoomInfo> roomList = new List<RoomInfo>();

    void Start()
    {
        if (!GameObject.Find("_NetworkManager(Clone)"))
            Instantiate(networkManager);
        if (!GameObject.Find("TeamManager(Clone)"))
            Instantiate(teamManager);
        //disable at the start to mot screw with the network
        createRoomButton.interactable = false;
        findRoomButton.interactable = false;
        //unlock the cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //Cursor.SetCursor(cursorSprite, new Vector2(.1f, .1f), CursorMode.ForceSoftware);
        //check if we were already in a game
        if (PhotonNetwork.InRoom)
        {
            //go to the lobby
            SetScreen(lobbyScreen);
            UpdateLobbyUI();
            TeamTexts.SetActive(true);
            //make the room visable again
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }
    }
    //changes current screen
    void SetScreen(GameObject Screen)
    {
        //disable all other screens
        mainScreen.SetActive(false);
        createRoomScreeen.SetActive(false);
        lobbyScreen.SetActive(false);
        lobbyBrowserScreen.SetActive(false);
        //activate the requested screen
        Screen.SetActive(true);
        if (Screen == lobbyBrowserScreen)
            UpdateLobbyBrowserUI();
    }
    public void OnBackButton()
    {
        SetScreen(mainScreen);
    }

    //MAIN SCREEN//////////////////////////////////////////////////////
    //called when the player name is changed
    public void OnPlayerNameChange(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }
    public override void OnConnectedToMaster()
    {
        //enable to buttons when we join the master server
        createRoomButton.interactable = true;
        findRoomButton.interactable = true;
    }
    //called when we want press create room button
    public void OnCreateRoomButton()
    {
        SetScreen(createRoomScreeen);
    }
    //called when we press find room button
    public void OnFindRoomButton()
    {
        SetScreen(lobbyBrowserScreen);
    }
    //CREATE ROOM SCREEN////////////////////////////////////////////////
    public void OnCreateRoomButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.CreateRoom(roomNameInput.text);
    }
    //LOBBY SCREEN/////////////////////////////////////////////////////
    public override void OnJoinedRoom()
    {
        SetScreen(lobbyScreen);
        TeamTexts.SetActive(true);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }
    [PunRPC]
    void UpdateLobbyUI()
    {
        bool blueStartVar = true;
        //enable or disable start game button depending on if your the host
        startGameButton.interactable = PhotonNetwork.IsMasterClient;
        //display all players
        bluePlayerListText.text = "";
        redPlayerListText.text = "";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (blueStartVar)
            {
                bluePlayerListText.text += player.NickName + "\n";
                photonView.RPC("AssignTeam", player, true);
                blueStartVar = false;
            }
            else
            {
                redPlayerListText.text += player.NickName + "\n";
                photonView.RPC("AssignTeam", player, false);
                blueStartVar = true;
            }
        }
        //set room info 
        roomInfoText.text = "<u>Room Name:</u>\n" + PhotonNetwork.CurrentRoom.Name;
    }
    [PunRPC]
    void AssignTeam(bool onBlue)
    {
        if (onBlue)
        {
            TeamManager.instance.blueTeam = true;
            TeamManager.instance.teamNum = 0;
        }
        else
        { 
            TeamManager.instance.redTeam = true;
            TeamManager.instance.teamNum = 1;
        }
}
    public void OnStartGameButton()
    {
        //hide room
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        //tell everyone to load the game
        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Map1");
    }
    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
        TeamTexts.SetActive(false);
    }
    //LOBBY BROWSER SCREEN//////////////////////////////////////////////////
    GameObject CreateRoomButton()
    {
        GameObject buttonobj = Instantiate(roomButtonPrefab,roomListContainer.transform);
        roomButtons.Add(buttonobj);
        return buttonobj;
    }
    void UpdateLobbyBrowserUI()
    {
        //disable all room buttons
        foreach (GameObject button in roomButtons)
            button.SetActive(false);
        //display all current room in master server
        for (int x = 0; x < roomList.Count; x++)
        {
            //get or create the button object
            GameObject button = x >= roomButtons.Count ? CreateRoomButton() : roomButtons[x];
            button.SetActive(true);
            //set the room's name and player count
            button.transform.Find("RoomNameText").GetComponent<TextMeshProUGUI>().text = roomList[x].Name;
            button.transform.Find("PlayerCount").GetComponent<TextMeshProUGUI>().text = roomList[x].PlayerCount + " / " + roomList[x].MaxPlayers;
            //set button on click event 
            Button buttonComp = button.GetComponent<Button>();
            string roomName = roomList[x].Name;
            buttonComp.onClick.RemoveAllListeners();
            buttonComp.onClick.AddListener(() => { OnJoinedRoomButton(roomName); });
        }
    }
    public void OnJoinedRoomButton(string roomName)
    {
        NetworkManager.instance.JoinRoom(roomName);
    }
    public void OnRefreshButton()
    {
        UpdateLobbyBrowserUI();
    }
    public override void OnRoomListUpdate(List<RoomInfo> allRooms)
    {
        roomList = allRooms;
    }
}
