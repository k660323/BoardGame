using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun.UtilityScripts;
using System.Net;
using System.CodeDom.Compiler;

public class NetworkController : MonoBehaviourPunCallbacks
{
    //public static NetworkController networkController = null;
    public GameObject GameStart, GameReady, MapImageBtn, CreateRoomHold;
    bool Ready = true;
    public static int turn = 15;
    public static string mod = "";
    public static int people = 0;

    [Header("LobbyPanel")]
    public Button[] CellBtn;
    public Button PreviousBtn;
    public Button NextBtn;

    [Header("RoomPanel")]
    public GameObject GameRoomPanel;
    public GameObject LobbyRoomPanel;
    public GameObject SearchRoomPanel;
    public GameObject RoomInfo;
    public GameObject CreateRoomInfo;
    public Text ListText; // 방 접속한 닉네임 출력
    public Text RoomInfoText; // 방 인원수 출력
    public Text[] ChatText;
    public InputField ChatInput;
    List<RoomInfo> myList = new List<RoomInfo>();
    List<string[]> myListPW = new List<string[]>();
    int currentPage = 1, maxPage, multiple;

    [Header("ETC")]
    public PhotonView PV;

    [Header("오디오")]
    public AudioClip ChatClip;

    #region 채팅창 갱신
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (ChatInput.text != "")
            {
                Send();
                ChatInput.Select(); // inputfield 포커스 해제 글 못적음
                ChatInput.transform.GetChild(1).GetComponent<Text>().text = "채팅을 입력하세요";
            }
            else
            {
                ChatInput.ActivateInputField(); // inputfield 포커스 글적을수있음
                ChatInput.transform.GetChild(1).GetComponent<Text>().text = "";
            }
        }
    }

    //채팅
    [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다 // 방에 속해 있어야 가능 //  ChatText 채팅 목록
    void ChatRPC(string msg)
    {
        SoundManager.instance.SFXPlay("ChatSound", ChatClip);
        if (ChatText[ChatText.Length - 1].text != "")
        {
            for (int i = 1; i < ChatText.Length; i++)
            {
                if (ChatText[i].text == "")
                {
                    continue;
                }
                else
                {
                    ChatText[i - 1].text = ChatText[i].text;
                }
            }
            ChatText[ChatText.Length - 1].text = msg;
        }
        else
        {
            ChatText[ChatText.Length - 1].text = msg;
        }
    }

    //채팅
    public void Send() // ChatInput 채팅 입력칸 에 메시지 적고 pv.rpc(chatrpc) 전송
    {
        // PhotonNetwork.NickName + " : " + ChatInput.text
        string msg = PhotonNetwork.NickName + " : " + ChatInput.text;
        PV.RPC("ChatRPC", RpcTarget.All, msg);
        ChatInput.text = "";
    }
    #endregion

    #region 방 리스트 갱신
    // 방 리스트 갱신
    // 왼쪽버튼-2,오른쪽버튼-1,셀 숫자
    public void MyListClick(int num)
    {
        if (num == -2) --currentPage;
        else if (num == -1) ++currentPage;
        else PhotonNetwork.JoinRoom(myList[multiple + num].Name);
        MyListRenewal();
    }

    void MyListRenewal()
    {
        //최대 페이지
        maxPage = (myList.Count % CellBtn.Length == 0) ? myList.Count / CellBtn.Length : myList.Count / CellBtn.Length + 1;

        // 이전, 다음 버튼
        PreviousBtn.interactable = (currentPage <= 1) ? false : true;
        NextBtn.interactable = (currentPage >= maxPage) ? false : true;

        // 페이지에 맞는 리스트 대입
        multiple = (currentPage - 1) * CellBtn.Length; //페이지 시작 인덱스
        for(int i=0; i < CellBtn.Length; i++)
        {
            CellBtn[i].interactable = (multiple + i < myList.Count) ? true : false;
            CellBtn[i].transform.Find("RoomName").Find("Text").GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].Name : "";
            CellBtn[i].transform.Find("NowPeople").Find("Text").GetComponent<Text>().text = (multiple + i < myList.Count) ? myList[multiple + i].PlayerCount + "/" + myList[multiple + i].MaxPlayers : "";
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList) //방이 생성될시 모든 클라이언트 동기화 해줌
    {
        int roomCount = roomList.Count;
        for(int i=0; i < roomCount; i++)
        {
            if (!roomList[i].RemovedFromList) //false라면 현재 존재하는 방
            {
                if (!myList.Contains(roomList[i])) myList.Add(roomList[i]);
                else myList[myList.IndexOf(roomList[i])] = roomList[i];
            }
            else if (myList.IndexOf(roomList[i]) != -1) myList.RemoveAt(myList.IndexOf(roomList[i]));
        }
        MyListRenewal();
    }
    #endregion

    #region 서버 연결 후 로비 입장
    // 서버 연결

    private void Start()
    {
        turn = 15;
        mod = "";
        people = 0;
        PhotonNetwork.AutomaticallySyncScene = false;
        Connect();
    }

    public void Connect()//서버에 접속 필수
    {
        Debug.Log("접속 완료");
        PhotonNetwork.ConnectUsingSettings();  // 이 함수가 호출되고 성공적으로 호출되면 on커넥트마스터 콜백함수 호출
    }

    public override void OnConnectedToMaster() // 연결되면 콜백함수 서버 접속 완료 On으로 시작 // 여기 호출1
    {
        PhotonNetwork.LocalPlayer.NickName = PlayerInfo.playerInfo.playerName;
        PhotonNetwork.JoinLobby();
        Debug.Log("접속 완료");
    }

    public void JoinLobby()
    {
        Debug.Log("접속 완료");
        PhotonNetwork.JoinLobby(); // 로비접속 //필수 호출1
    }

    public override void OnJoinedLobby() // 로비 접속하면 콜백함수 호출
    {
        myList.Clear();
        Debug.Log("접속 완료");
    }
    #endregion

    #region 방 생성
    //방
    public void CreateRoom()
    {
        CreateRoomHold.SetActive(true);
        if (PhotonNetwork.InLobby)
        {
            string roomName = GameObject.Find("Canvas").transform.Find("LobbyRoom").Find("CreateRoomWindow").Find("RoomNameInputField").Find("Text").GetComponent<Text>().text;
            //Text pw = GameObject.Find("Canvas").transform.Find("CreateRoomWindow").Find("RoomPwdInputField").Find("Text").GetComponent<Text>();
            int people = int.Parse(GameObject.Find("Canvas").transform.transform.Find("LobbyRoom").Find("CreateRoomWindow").Find("RoomPeopleDropdown").Find("Label").GetComponent<Text>().text);


            RoomOptions ros = new RoomOptions();
            ros.MaxPlayers = (byte)people;
            ros.IsVisible = true;

            if (roomName != "")
            {
                PhotonNetwork.CreateRoom(roomName, ros);
            }
            else
            {
                roomName = "Room" + Random.Range(0, 100);
                GameObject.Find("Canvas").transform.Find("LobbyRoom").Find("CreateRoomWindow").Find("RoomNameInputField").GetComponent<InputField>().text = roomName;
                PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = (byte)people });
            }
        }
        else
        {
            CreateRoomHold.SetActive(false);
            GameObject.Find("Canvas").transform.Find("LobbyRoom").Find("BgErrorWindow").gameObject.SetActive(true);
            Connect();
        }
    }

    public void JoinOrCreateRoom() // 방 찾기 없으면 방 생성
    {
        Text roomName = GameObject.Find("Canvas").transform.Find("CreateRoomWindow").Find("RoomNameInputField").Find("Text").GetComponent<Text>();
        //Text pw = GameObject.Find("Canvas").transform.Find("CreateRoomWindow").Find("RoomPwdInputField").Find("Text").GetComponent<Text>();
        int people = int.Parse(GameObject.Find("Canvas").transform.Find("CreateRoomWindow").Find("RoomPeopleDropdown").Find("Label").GetComponent<Text>().text);
        //bool observer = GameObject.Find("Canvas").transform.Find("CreateRoomWindow").Find("isObserver").GetComponent<Toggle>().isOn;
        // Text gamemod = GameObject.Find("Canvas").transform.Find("CreateRoomWindow").Find("RoomGameModeDropdown").Find("Label").GetComponent<Text>();
        //Text map = GameObject.Find("Canvas").transform.Find("CreateRoomWindow").Find("RoomGameMapDropdown").Find("Label").GetComponent<Text>();
        //int turn = int.Parse(GameObject.Find("Canvas").transform.Find("CreateRoomWindow").Find("RoomGameTurnDropdown").Find("Label").GetComponent<Text>().text);

        RoomOptions ros = new RoomOptions();
        ros.MaxPlayers = (byte)people;
        ros.IsVisible = true;

        PhotonNetwork.JoinOrCreateRoom(roomName.text, new RoomOptions { MaxPlayers = 2 },null);
    }

    public override void OnCreatedRoom() // 방 만들고 초기화
    {
        GameRoomPanel.SetActive(true);
        LobbyRoomPanel.SetActive(false);

        string[] RoomInfoList = new string[6];
        RoomInfoList[0] = CreateRoomInfo.transform.Find("RoomNameInputField").Find("Text").GetComponent<Text>().text; //방이름
        RoomInfoList[1] = CreateRoomInfo.transform.Find("RoomPwdInputField").Find("Text").GetComponent<Text>().text; //방비번
        RoomInfoList[2] = CreateRoomInfo.transform.Find("RoomPeopleDropdown").Find("Label").GetComponent<Text>().text; //인원수
        RoomInfoList[3] = CreateRoomInfo.transform.Find("RoomGameModeDropdown").Find("Label").GetComponent<Text>().text; // 게임모드
        RoomInfoList[4] = CreateRoomInfo.transform.Find("RoomGameMapDropdown").Find("Label").GetComponent<Text>().text; // 맵
        RoomInfoList[5] = CreateRoomInfo.transform.Find("RoomGameTurnDropdown").Find("Label").GetComponent<Text>().text; // 턴
        PV.RPC("SetRoomInfo", RpcTarget.AllBuffered, RoomInfoList);
        //GameObject.Find("Canvas").transform.Find("GameRoom").gameObject.SetActive(true);
        //GameObject.Find("Canvas").transform.Find("LobbyRoom").gameObject.SetActive(false);
        Debug.Log("방 만들기 완료");
    }

    [PunRPC]
    void SetRoomInfo(string[] roomListInfo) // 방 만들때 세팅값 초기 세팅
    {
        RoomInfo.transform.Find("RoomName").Find("Text").GetComponent<Text>().text = roomListInfo[0]; // 이름 
        RoomInfo.transform.Find("RoomPWD").Find("Text").GetComponent<Text>().text = roomListInfo[1]; // 비번
        RoomInfo.transform.Find("MaxPeople").Find("Text").GetComponent<Text>().text = roomListInfo[2]; // 인원수
        RoomInfo.transform.Find("GameMode").Find("Text").GetComponent<Text>().text = roomListInfo[3]; // 게임모드
        GameRoomPanel.transform.Find("MapImage").Find("Text").GetComponent<Text>().text = roomListInfo[4]; // 맵 
        RoomInfo.transform.Find("Round").Find("Text").GetComponent<Text>().text = roomListInfo[5]; // 턴

        GameObject.Find("Canvas").transform.Find("GameRoom").Find("MapImage").GetComponent<Image>().sprite = Resources.Load<Sprite>("MapImage/" + roomListInfo[4]);
        GameObject.Find("Canvas").transform.Find("GameRoom").Find("MapImage").Find("Text").GetComponent<Text>().text = roomListInfo[4];
    }

    #endregion

    #region 방 입장
    public void JoinRoom() // 방 찾기
    {
        PhotonNetwork.JoinRoom(GameObject.Find("Canvas").transform.Find("SearchRoom").Find("RoomName").Find("InputField").Find("Text").GetComponent<Text>().text);
    }

    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom() // 방 입장후 초기화
    {
        GameRoomPanel.SetActive(true);
        SearchRoomPanel.SetActive(false);
        if (PhotonNetwork.IsMasterClient) // 초기방 세팅 방장/논방장
        {
            GameObject.Find("Canvas").transform.Find("GameRoom").Find("GameStartButton").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("GameRoom").Find("GameReadyButton").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("GameRoom").Find("MapImage").Find("Button").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("GameRoom").Find("RoomInfo").Find("Button").gameObject.SetActive(true);
        }
        else
        {
            GameObject.Find("Canvas").transform.Find("GameRoom").Find("GameStartButton").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("GameRoom").Find("GameReadyButton").Find("CompleteImage").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("GameRoom").Find("GameReadyButton").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("GameRoom").Find("MapImage").Find("Button").gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("GameRoom").Find("RoomInfo").Find("Button").gameObject.SetActive(false);
        }
        //RoomRenewal();
        PlayerSpawn();
        //채팅 초기화
        ChatInput.text = "";
        for (int i = 0; i < ChatText.Length; i++) ChatText[i].text = "";

        Debug.Log("방 참가 완료");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //RoomRenewal();
        PV.RPC("ChatRPC", RpcTarget.All, "<color=yellow>" + newPlayer.NickName + "님이 참가하셨습니다.</color>");
    }

    public void PlayerSpawn()
    {
        PhotonNetwork.Instantiate("Player", new Vector2(1, 1), Quaternion.identity); // 리소스 폴더안의 복제할 오브젝트 이름  
    }

    #endregion

    #region 로비퇴장 방퇴장 연결끊기
    public void LeaveLobby()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        else
            SceneManager.LoadScene("MainScene");
    }

    public override void OnLeftLobby()
    {
        Disconnected();
        SceneManager.LoadScene("MainScene");
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            GameRoomPanel.SetActive(false);
            LobbyRoomPanel.SetActive(true);
        }

    }
    public override void OnLeftRoom()
    {
        CreateRoomHold.SetActive(false);
        GameRoomPanel.SetActive(false);
        LobbyRoomPanel.SetActive(true);
        PV.RPC("MasterChange", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) //나가면 모두 실행
    {
        //RoomRenewal();

        //PV.RPC("MasterChange", RpcTarget.All);
        PV.RPC("ChatRPC", RpcTarget.All, "<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다.</color>");

    }

    public void Disconnected()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("연결 끊김");
    }

   

    #endregion

    #region 생성 입장 랜덤입장 오류
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("방 만들기 실패");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        GameObject.Find("Canvas").transform.Find("SearchRoom").Find("NoSearchSignWindow").gameObject.SetActive(true);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        GameObject.Find("Canvas").transform.Find("SearchRoom").Find("NoSearchSignWindow").gameObject.SetActive(true);
    }
    #endregion

    #region 방 대기실 인원 실시간감지
    void RoomRenewal() // 현재 방 인원수 출력
    {
       // ListText.text = ""; // 닉네임 출력
      //  for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
       //     ListText.text += PhotonNetwork.PlayerList[i].NickName + ((i + 1 == PhotonNetwork.PlayerList.Length) ? "" : ", ");
        //RoomInfoText.text = PhotonNetwork.CurrentRoom.Name + " / " + PhotonNetwork.CurrentRoom.PlayerCount + "명 / " + PhotonNetwork.CurrentRoom.MaxPlayers + "최대";

        //RoomPanel[_RoomCount].transform.Find("RoomName").Find("Text").GetComponent<Text>().text = PhotonNetwork.CurrentRoom.Name;
        // RoomPanel[_RoomCount].transform.Find("NowPeople").Find("Text").GetComponent<Text>().text = PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers;
    }
    #endregion

    #region 방장 변경
    public void ChangeMasterClient()
    {
        if (Ready == false)
        {
            GameReady.transform.Find("CompleteImage").gameObject.SetActive(false);
            MapImageBtn.gameObject.SetActive(true);
            Ready = true;
            findmylist();
        }
        GameStart.SetActive(true);
        GameReady.SetActive(false);

        GameObject.Find("Canvas").transform.Find("GameRoom").Find("GameStartButton").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("GameRoom").Find("GameReadyButton").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("GameRoom").Find("MapImage").Find("Button").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("GameRoom").Find("RoomInfo").Find("Button").gameObject.SetActive(true);
    }

    [PunRPC]
    void MasterChange()
    {
        if (PhotonNetwork.IsMasterClient == true)
            ChangeMasterClient();
    }

    #endregion

    #region 준비상태 동기화
    public void ReadyState() // 레디 비레디
    {
        if (Ready == false) // 준비
        {
            GameReady.transform.Find("CompleteImage").gameObject.SetActive(false);
            Ready = true;
            findmylist();
        }
        else if (Ready == true) // 준비 완료
        {
            GameReady.transform.Find("CompleteImage").gameObject.SetActive(true);
            Ready = false;
            findmylist();
        }
    }

    void findmylist() // 준비상태 동기화
    {
        for (int i = 0; i < GameObject.Find("Canvas").transform.Find("GameRoom").Find("Division").childCount; i++)
        {
            PhotonView tPV = GameObject.Find("Canvas").transform.Find("GameRoom").Find("Division").GetChild(i).GetComponent<PhotonView>();
            if (tPV.Controller.IsLocal)
            {
                GameObject tgo = GameObject.Find("Canvas").transform.Find("GameRoom").Find("Division").GetChild(i).gameObject;
                tgo.GetComponent<RoomPlayerInfo>().ReadySet(Ready);
                break;
            }
        }
    }

    #endregion

    #region 맵 변경
    public void ChangeMap(string mapName)
    {
        PV.RPC("AllChangeMap", RpcTarget.AllBuffered, mapName);
    }

    [PunRPC]
    void AllChangeMap(string mapName)
    {
        GameObject.Find("Canvas").transform.Find("GameRoom").Find("MapImage").GetComponent<Image>().sprite = Resources.Load<Sprite>("MapImage/" + mapName);
        GameObject.Find("Canvas").transform.Find("GameRoom").Find("MapImage").Find("Text").GetComponent<Text>().text = mapName;
    }
    #endregion

    #region 방 정보 변경
    public void RoomInfoEdit() // 방 수정창 열때 초기화
    {
        GameObject roomEdit = GameObject.Find("Canvas").transform.Find("GameRoom").Find("RoomInfoMask").Find("RoomInfoWindow").gameObject;
        GameObject roomInfo = GameObject.Find("Canvas").transform.Find("GameRoom").Find("RoomInfo").gameObject;

        roomEdit.transform.Find("RoomNameInputField").GetComponent<InputField>().text = roomInfo.transform.Find("RoomName").Find("Text").GetComponent<Text>().text; // 이름 
        roomEdit.transform.Find("RoomPwdInputField").GetComponent<InputField>().text = roomInfo.transform.Find("RoomPWD").Find("Text").GetComponent<Text>().text; // 비번
        roomEdit.transform.Find("RoomPeopleDropdown").Find("Label").GetComponent<Text>().text = roomInfo.transform.Find("MaxPeople").Find("Text").GetComponent<Text>().text; // 인원수
        roomEdit.transform.Find("RoomGameModeDropdown").Find("Label").GetComponent<Text>().text = roomInfo.transform.Find("GameMode").Find("Text").GetComponent<Text>().text; // 게임모드
        roomEdit.transform.Find("RoomGameTurnDropdown").Find("Label").GetComponent<Text>().text = roomInfo.transform.Find("Round").Find("Text").GetComponent<Text>().text; // 턴
    }

    public void CommitRoomInfo() // 방 수정 내용 적용
    {
        GameObject roomEdit = GameObject.Find("Canvas").transform.Find("GameRoom").Find("RoomInfoMask").Find("RoomInfoWindow").gameObject;
        string[] roomEditInfo = new string[5];
        roomEditInfo[0] = roomEdit.transform.Find("RoomNameInputField").Find("Text").GetComponent<Text>().text; // 이름 
        roomEditInfo[1] = roomEdit.transform.Find("RoomPwdInputField").Find("Text").GetComponent<Text>().text; // 비번
        roomEditInfo[2] = roomEdit.transform.Find("RoomPeopleDropdown").Find("Label").GetComponent<Text>().text; // 인원수
        roomEditInfo[3] = roomEdit.transform.Find("RoomGameModeDropdown").Find("Label").GetComponent<Text>().text; // 게임모드
        roomEditInfo[4] = roomEdit.transform.Find("RoomGameTurnDropdown").Find("Label").GetComponent<Text>().text; // 턴

        PV.RPC("CommitAllRoomInfo", RpcTarget.AllBuffered, roomEditInfo);
    }

    [PunRPC]
    public void CommitAllRoomInfo(string[] roomEditInfo) // 모든 유저에게 내용 적용
    {
        GameObject roomInfo = GameObject.Find("Canvas").transform.Find("GameRoom").Find("RoomInfo").gameObject;
        roomInfo.transform.Find("RoomName").Find("Text").GetComponent<Text>().text = roomEditInfo[0]; // 이름 
        roomInfo.transform.Find("RoomPWD").Find("Text").GetComponent<Text>().text = roomEditInfo[1]; // 비번
        roomInfo.transform.Find("MaxPeople").Find("Text").GetComponent<Text>().text = roomEditInfo[2]; // 인원수
        roomInfo.transform.Find("GameMode").Find("Text").GetComponent<Text>().text = roomEditInfo[3]; // 게임모드
        roomInfo.transform.Find("Round").Find("Text").GetComponent<Text>().text = roomEditInfo[4]; // 턴
    }
    #endregion

    #region 게임시작
    public void StartGame()
    {
        int readycount = 0;

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {

            if (GameObject.Find("Canvas").transform.Find("GameRoom").Find("Division").GetChild(i).Find("NowReady").gameObject.activeInHierarchy == true)
            {
                readycount++;
            }// 방장은 레디여부 확인x 시작주체이므로 카운터                               
            else if (GameObject.Find("Canvas").transform.Find("GameRoom").Find("Division").GetChild(i).Find("RoomMaster").gameObject.activeInHierarchy == true)
            {
                readycount++;
            }
        }

        if (readycount == PhotonNetwork.PlayerList.Length && readycount != 1 && PhotonNetwork.IsMasterClient == true)
        {
            GameStart.SetActive(false);
            PV.RPC("GameStartState", RpcTarget.AllBuffered);
        }
        else if (readycount != PhotonNetwork.PlayerList.Length)
        {
            GameObject.Find("Canvas").transform.Find("GameRoom").Find("BgErrorWindow").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("GameRoom").Find("BgErrorWindow").Find("ErrorWindow").Find("Text").GetComponent<Text>().text = "모두 레디하지 않았어요";
        }
        else if (readycount == 1)
        {
            GameObject.Find("Canvas").transform.Find("GameRoom").Find("BgErrorWindow").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("GameRoom").Find("BgErrorWindow").Find("ErrorWindow").Find("Text").GetComponent<Text>().text = "혼자서는 못해요";
        }
    }

    [PunRPC]
    void GameStartState()
    {
        GameObject roomInfo = GameObject.Find("Canvas").transform.Find("GameRoom").Find("RoomInfo").gameObject;
        NetworkController.turn = int.Parse(roomInfo.transform.Find("Round").Find("Text").GetComponent<Text>().text);
        NetworkController.mod = roomInfo.transform.Find("GameMode").Find("Text").GetComponent<Text>().text;
        NetworkController.people = int.Parse(roomInfo.transform.Find("MaxPeople").Find("Text").GetComponent<Text>().text);
        PhotonNetwork.LoadLevel(GameObject.Find("Canvas").transform.Find("GameRoom").Find("MapImage").Find("Text").GetComponent<Text>().text);
    }

    #endregion

    #region 변수 동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
    #endregion

    #region 스크립트 정보
    [ContextMenu("정보")]
    void Info()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("현재 방 이름 : " + PhotonNetwork.CurrentRoom.Name);
            Debug.Log("현재 방 인원수 : " + PhotonNetwork.CurrentRoom.PlayerCount);
            Debug.Log("현재 방 최대 인원수 : " + PhotonNetwork.CurrentRoom.MaxPlayers);

            string playerStr = "방에 있는 플레이어 목록 : ";
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++) playerStr += PhotonNetwork.PlayerList[i].NickName + ", ";
            Debug.Log(playerStr);
        }
        else
        {
            Debug.Log("접속한 인원수 : " + PhotonNetwork.CountOfPlayers);
            Debug.Log("방 개수 : " + PhotonNetwork.CountOfRooms);
            Debug.Log("모든 방에 있는 인원 수 : " + PhotonNetwork.CountOfPlayersInRooms);
            Debug.Log("로비에 있는지? : " + PhotonNetwork.InLobby);
            Debug.Log("연결됐는지? : " + PhotonNetwork.IsConnected);
        }
    }
    #endregion
}



#region 더미코드
/*
public class NetworkController : MonoBehaviourPunCallbacks
{
    static NetworkController nc;
    bool createRoom = false;
    #region Private Serializable Fields


    #endregion


    #region Private Fields

    [SerializeField]
    private string gameVersion = "1.0";

    #endregion


    #region MonoBehaviour CallBacks

    // Start is called before the first frame update
    void Awake()
    {
        // 같은 버전의 클라이언트 끼리 로드할 수 있다.
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #endregion


    #region Public Methods

    public void RoomConnect() // 방 찾아 연결
    {
        if (PhotonNetwork.IsConnected) // 방찾기 취소후 다시 방찾을때 실행 되는 함수
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    #endregion

    #region Public Methods
    public void Start()
    {
        createRoom = false;
        if (nc == null)
        {
            nc = this;
            DontDestroyOnLoad(this);
            PhotonNetwork.NickName = PlayerInfo.playerInfo.playerName;
        }
    }

    public override void OnCreatedRoom() //방생성
    {
        if (PhotonNetwork.IsConnected) // 방생성후 다음 다시 방만들기 할때 실행 되는 함수
        {
            Text roomName = GameObject.Find("Canvas").transform.Find("CreateRoomWindow").Find("RoomNameInputField").Find("Text").GetComponent<Text>();
            //Text pw = GameObject.Find("Canvas").transform.Find("CreateRoomWindow").Find("RoomPwdInputField").Find("Text").GetComponent<Text>();
            int people = int.Parse(GameObject.Find("Canvas").transform.Find("CreateRoomWindow").Find("RoomPeopleDropdown").Find("Label").GetComponent<Text>().text);
            //bool observer = GameObject.Find("Canvas").transform.Find("CreateRoomWindow").Find("isObserver").GetComponent<Toggle>().isOn;
            // Text gamemod = GameObject.Find("Canvas").transform.Find("CreateRoomWindow").Find("RoomGameModeDropdown").Find("Label").GetComponent<Text>();
            //Text map = GameObject.Find("Canvas").transform.Find("CreateRoomWindow").Find("RoomGameMapDropdown").Find("Label").GetComponent<Text>();
            //int turn = int.Parse(GameObject.Find("Canvas").transform.Find("CreateRoomWindow").Find("RoomGameTurnDropdown").Find("Label").GetComponent<Text>().text);

            RoomOptions ros = new RoomOptions();
            ros.MaxPlayers = (byte)people;
            ros.IsVisible = true;
            PhotonNetwork.CreateRoom(roomName.text, ros);
        }
        else
        {
            createRoom = true;
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }
     

    }

    #endregion

    #region MonoBehaviourPunCallbacks Callbacks

    public override void OnConnectedToMaster() // 방장권환으로 방 만들기 // 방 찾기
    {
        if(createRoom == true)
        {
            Text roomName = GameObject.Find("Canvas").transform.Find("CreateRoomWindow").Find("RoomNameInputField").Find("Text").GetComponent<Text>();
            //Text pw = GameObject.Find("Canvas").transform.Find("CreateRoomWindow").Find("RoomPwdInputField").Find("Text").GetComponent<Text>();
            int people = int.Parse(GameObject.Find("Canvas").transform.Find("CreateRoomWindow").Find("RoomPeopleDropdown").Find("Label").GetComponent<Text>().text);
            //bool observer = GameObject.Find("Canvas").transform.Find("CreateRoomWindow").Find("isObserver").GetComponent<Toggle>().isOn;
            // Text gamemod = GameObject.Find("Canvas").transform.Find("CreateRoomWindow").Find("RoomGameModeDropdown").Find("Label").GetComponent<Text>();
            //Text map = GameObject.Find("Canvas").transform.Find("CreateRoomWindow").Find("RoomGameMapDropdown").Find("Label").GetComponent<Text>();
            //int turn = int.Parse(GameObject.Find("Canvas").transform.Find("CreateRoomWindow").Find("RoomGameTurnDropdown").Find("Label").GetComponent<Text>().text);

            RoomOptions ros = new RoomOptions();
            ros.MaxPlayers = (byte)people;
            ros.IsVisible = true;
            PhotonNetwork.CreateRoom(roomName.text, ros);

            createRoom = false;
            Debug.Log("어케 드러옴");
        }
        else
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnDisconnected(DisconnectCause cause) // 연결 끊기
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    #endregion

    public override void OnJoinRandomFailed(short returnCode, string message) // 방 없음
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        GameObject.Find("Canvas").transform.Find("NoSearchSignWindow").gameObject.SetActive(true);
    }


    public override void OnJoinedRoom() // 성공적으로 방에 들어갔을때 호출
    {
        LoadingSceneManager.LoadScene("WaitingScene");
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
    }
}
    //채팅
    [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
    void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = msg;
                break;
            }
        if (!isInput) // 꽉차면 한칸씩 위로 올림
        {
            for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
    }

*/
#endregion