using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;

public class MenuController : MonoBehaviourPunCallbacks, ILobbyCallbacks
{

    #region Variables_________________________________________________________

    public GameObject loadingImg;
    public TextMeshProUGUI message;

    [Header("Menu Screens")]
    public GameObject mainScreen;
    public GameObject createRoomScreen;
    public GameObject roomLobbyScreen;
    public GameObject roomSelectLobbyScreen;

    [Header("Main Screen Components")]
    public Button createRoomBttn_ms;
    public Button findRoomBttn_ms;
    public TMP_InputField NameInput_ms;

    [Header("Create Room Screen")]
    public TMP_InputField roomNameInput_crs;
    public Button createRoomBttn_crs;

    [Header("Room Lobby Screen")]
    public Button startGameButton_rls;
    public TextMeshProUGUI playerListTextBox;
    public TextMeshProUGUI roomNameText_rls;
    public TMP_Dropdown levelSelect;

    [Header("Room Selection Lobby Screen")]
    public RectTransform roomListContainer;
    public GameObject roomBttnPrefab;

    [SerializeField]
    private List<GameObject> roomBttnList = new List<GameObject>();
    [SerializeField]
    private List<RoomInfo> roomInfoList = new List<RoomInfo>();


    public string name;
    public string roomName;
    public string selectedLevel;

    #endregion

    #region // unity methods___________________________________________

    void Start()
    {
        // disabling buttons until network is established
        createRoomBttn_ms.interactable = false;
        findRoomBttn_ms.interactable = false;
        NameInput_ms.interactable = false;
        loadingImg.SetActive(true);
        showMessage("Connecting to Server");

        //unhide cursor
        Cursor.lockState = CursorLockMode.None;

        // if we are in a game
        if (PhotonNetwork.InRoom)
        {
            // go to the lobby

            //make the room visible
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.CurrentRoom.IsOpen = true;

        }

    }
    #endregion

    #region // all screen methods_________________________________________


    void setScreen(GameObject screen)
    {
        mainScreen.SetActive(false);
        createRoomScreen.SetActive(false);
        roomLobbyScreen.SetActive(false);
        roomSelectLobbyScreen.SetActive(false);

        screen.SetActive(true);

        if(screen == mainScreen)
        {
            EventSystem.current.SetSelectedGameObject(NameInput_ms.gameObject);
        }

        if (screen == createRoomScreen)
        {
            EventSystem.current.SetSelectedGameObject(roomNameInput_crs.gameObject);
        }

        if (screen == roomSelectLobbyScreen)
        {
            updateRoomSelectLobbyUI();
        }
    }

    public void showMessage(string text)
    {
        message.text = text;
        message.gameObject.SetActive(true);
    }

    public void hideMessage()
    {
        message.text = "";
        message.gameObject.SetActive(false);
    }

    public void onBkBttn()
    {
        setScreen(mainScreen);
    }

    #endregion


    #region // main Screen________________________________________________

    public override void OnConnectedToMaster()
    {
        NameInput_ms.interactable = true;
        EventSystem.current.SetSelectedGameObject(NameInput_ms.gameObject);
        loadingImg.SetActive(false);
        hideMessage();
    }


    public void onplayerNameValueChanged()
    {
        name = NameInput_ms.text;

        if (name.Length > 2 && name.Length < 12)
        {
            if (!createRoomBttn_ms.IsInteractable())
            {
                createRoomBttn_ms.interactable = true;
                findRoomBttn_ms.interactable = true;
                hideMessage();
            }
        }
        else
        {
            createRoomBttn_ms.interactable = false;
            findRoomBttn_ms.interactable = false;
            showMessage("Name must be between 3 and 11 characters.");
        }

        
       
    }

    public void OnCreateRoomBttn_ms()
    {
        PhotonNetwork.NickName = name;
        roomNameInput_crs.text = null;
        createRoomBttn_crs.interactable = false;
        setScreen(createRoomScreen);
    }

    public void OnFindRoomBttn_ms()
    {
        PhotonNetwork.NickName = name;
        setScreen(roomSelectLobbyScreen);
    }


    #endregion

    #region // create Room________________________________________________

    public void onRoomNameValueChanged()
    {
        roomName = roomNameInput_crs.text;

        if (roomName.Length > 2 && roomName.Length < 12)
        {
            if (!createRoomBttn_crs.IsInteractable())
            {
                createRoomBttn_crs.interactable = true;
                hideMessage();
            }
        }
        else
        {
            createRoomBttn_crs.interactable = false;
            showMessage("Room name must be between 3 and 11 characters.");
        }

    }

    public void onCreateRoomBttn_crs()
    {
        NetworkManager.instance.createRoom(roomName);
    }

    #endregion

    #region // room Lobby________________________________________________

    public override void OnJoinedRoom()
    {
        setScreen(roomLobbyScreen);
        photonView.RPC("updateRoomLobbyUI", RpcTarget.All);
    }
    [PunRPC]
    void updateRoomLobbyUI()
    {
        //update room name
        roomNameText_rls.text = "<b>" + PhotonNetwork.CurrentRoom.Name + "</b>";

        // update player list
        playerListTextBox.text = "";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerListTextBox.text += player.NickName + "\n";
        }

        // disable or enable buttons
        startGameButton_rls.interactable = PhotonNetwork.IsMasterClient;
        levelSelect.interactable = PhotonNetwork.IsMasterClient;

        selectedLevel = "Level 1";
        selectedLevel = levelSelect.options[0].text;

    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        updateRoomLobbyUI();
    }

    public void onLeaveRoombttn_rl()
    {
        PhotonNetwork.LeaveRoom();
        setScreen(mainScreen);
    }

    public void onSelectLevelChange()
    {
        selectedLevel = levelSelect.options[levelSelect.value].text;
    }

    public void onStartGameBttn_rl()
    {
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        ;
        NetworkManager.instance.photonView.RPC("changeScenes", RpcTarget.All, selectedLevel);
    }

    #endregion

    #region // room select________________________________________________

    public void onCreateRoomBttn_rsl()
    {
        roomNameInput_crs.text = null;
        createRoomBttn_crs.interactable = false;
        setScreen(createRoomScreen);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        roomInfoList = roomList;
    }

    public GameObject createNewRoomBttn()
    {
        GameObject bttn = Instantiate(roomBttnPrefab, roomListContainer.transform);
        roomBttnList.Add(bttn);
        return bttn;
    }

    public void onJoinRoomBttn_rls(string roomName)
    {
        foreach (RoomInfo roomInfo in roomInfoList)
        {
            if (roomInfo.Name == roomName)
            {
                NetworkManager.instance.joinRoom(roomName);
                break;
            }
        }
        
    }

    public void onRefreshBttn()
    {
        updateRoomSelectLobbyUI();
    }

    public void updateRoomSelectLobbyUI()
    {
        foreach (GameObject bttn in roomBttnList)
        {
            bttn.SetActive(false);
        }

        foreach(RoomInfo room in roomInfoList)
        {
            if(room.PlayerCount <= 0)
            {
                roomInfoList.Remove(room);
            }
        }

        for (int i = 0; i < roomInfoList.Count; i++)
        {
            GameObject bttn = i >= roomBttnList.Count ? createNewRoomBttn() : roomBttnList[i];
            bttn.SetActive(true);
            bttn.transform.Find("roomNameText").GetComponent<TextMeshProUGUI>().text = roomInfoList[i].Name;
            bttn.transform.Find("playerCountText").GetComponent<TextMeshProUGUI>().text = roomInfoList[i].PlayerCount.ToString() + "/" + roomInfoList[i].MaxPlayers.ToString();

            string rn = roomInfoList[i].Name;
            Button bttncomp = bttn.GetComponent<Button>();
            bttncomp.onClick.RemoveAllListeners();
            bttncomp.onClick.AddListener( () => { onJoinRoomBttn_rls(rn); } );
        }
    }



    #endregion


}
