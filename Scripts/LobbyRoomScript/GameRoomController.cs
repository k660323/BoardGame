using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRoomController : MonoBehaviourPunCallbacks
{
    public PhotonView PV;
    
    public NetworkController NC;
    public GameObject GameStart, GameReady,MapImageBtn;
   
    bool Ready = true;
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (NC.ChatInput.text != "")
            {
                NC.Send();
                NC.ChatInput.Select(); // inputfield 포커스 해제 글 못적음
                NC.ChatInput.transform.GetChild(1).GetComponent<Text>().text = "채팅을 입력하세요";
            }
            else
            {
                NC.ChatInput.ActivateInputField(); // inputfield 포커스 글적을수있음
                NC.ChatInput.transform.GetChild(1).GetComponent<Text>().text = "";
            }
        }
    }

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

    public void ReadyState() // 레디 비레디
    {
        if(Ready == false) // 준비
        {
            GameReady.transform.Find("CompleteImage").gameObject.SetActive(false);
            Ready = true;
            findmylist();
        }
        else if(Ready == true) // 준비 완료
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

        Debug.Log(readycount);
        Debug.Log(PhotonNetwork.PlayerList.Length);
        if (readycount == PhotonNetwork.PlayerList.Length && readycount != 1)
        {
            SceneController sc = new SceneController();
            sc.LoadScene(GameObject.Find("Canvas").transform.Find("GameRoom").Find("MapImage").Find("Text").GetComponent<Text>().text);
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }


}
