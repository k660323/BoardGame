using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;

public class RoomPlayerInfo : MonoBehaviourPunCallbacks, IPunObservable
{
    public Text NickNameText;
    public Text WinRate;
    public PhotonView PV;
    public Image Character;
    public Image Dice;
    public Image Legacy1, Legacy2, Legacy3;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // 외부 컴퓨터에 데이터 동기화만함
    {
        if (stream.IsWriting)
        {
            stream.SendNext(PlayerInfo.playerInfo.GetEquipItem(ItemType.Character));
            stream.SendNext(PlayerInfo.playerInfo.total.ToString());
            stream.SendNext(PlayerInfo.playerInfo.win.ToString());
            stream.SendNext(PlayerInfo.playerInfo.lose.ToString());
            stream.SendNext(PlayerInfo.playerInfo.draw.ToString());
            stream.SendNext(PlayerInfo.playerInfo.GetEquipItem(ItemType.Dice));
            stream.SendNext(PlayerInfo.playerInfo.equipLegacy1);
            stream.SendNext(PlayerInfo.playerInfo.equipLegacy2);
            stream.SendNext(PlayerInfo.playerInfo.equipLegacy3);
        }
        else
        {
            Character.sprite = Resources.Load<Sprite>("CharacterImage/" + stream.ReceiveNext().ToString());
            WinRate.text = stream.ReceiveNext().ToString() + "전 " + stream.ReceiveNext().ToString() + "승 " + stream.ReceiveNext().ToString() + "패 " + stream.ReceiveNext().ToString() + "무 ";
            Dice.sprite = Resources.Load<Sprite>("DiceImage/" + stream.ReceiveNext().ToString());
            Legacy1.sprite = Resources.Load<Sprite>("LegacyImage/" + stream.ReceiveNext());
            Legacy2.sprite = Resources.Load<Sprite>("LegacyImage/" + stream.ReceiveNext());
            Legacy3.sprite = Resources.Load<Sprite>("LegacyImage/" + stream.ReceiveNext());
            
        }
    }

    void Awake()
    {
        this.transform.parent = GameObject.Find("Canvas").transform.Find("GameRoom").Find("Division");
        this.transform.position = GameObject.Find("Canvas").transform.Find("GameRoom").Find("Division").position;
        this.transform.localScale = Vector2.one;

        // 닉네임
        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? Color.green : Color.red;

        this.WinRate.text = PlayerInfo.playerInfo.total + "전 " + PlayerInfo.playerInfo.win + "승 " + PlayerInfo.playerInfo.lose + "패 " + PlayerInfo.playerInfo.draw + "무 ";
        this.Character.sprite = Resources.Load<Sprite>("CharacterImage/" + PlayerInfo.playerInfo.GetEquipItem(ItemType.Character));
        this.Dice.sprite = Resources.Load<Sprite>("DiceImage/" + PlayerInfo.playerInfo.GetEquipItem(ItemType.Dice));
        PV.RPC("CheckMaster", RpcTarget.AllBuffered);

        if (PlayerInfo.playerInfo.equipLegacy1 != "null")
        {
            this.Legacy1.sprite = Resources.Load<Sprite>("LegacyImage/" + PlayerInfo.playerInfo.equipLegacy1);
        }
        if (PlayerInfo.playerInfo.equipLegacy2 != "null")
        {
            this.Legacy2.sprite = Resources.Load<Sprite>("LegacyImage/" + PlayerInfo.playerInfo.equipLegacy2);
        }
        if (PlayerInfo.playerInfo.equipLegacy3 != "null")
        {
            this.Legacy3.sprite = Resources.Load<Sprite>("LegacyImage/" + PlayerInfo.playerInfo.equipLegacy3);
        }

        //if(PV.IsMine) //스크립트pv랑 오브젝트pv랑 같은지
        // PV.RPC("SetInfo", RpcTarget.AllBuffered);
    }


    // PV 스크립트가 들어간 오브젝트 // PV.RPC 나자신과 평행세계나에게 알립니다.
    // 나 자신과 평행세계의 나 자신도 똑같이 이 함수 실행해준다.
    [PunRPC] // 이 방에 있는 모든 사람에게 함수 실행 // 즉 나 자신과 상대방 컴퓨터의 복제된 자신을 동기화 
    void SetInfo() // 내꺼만 뜸 이유: 이 스크립트만 PV에 연동 외부 스크립트 데이터를 들고오면 동기화문제생김
    {
        /*
        this.WinRate.text = PlayerInfo.playerInfo.total + "전 " + PlayerInfo.playerInfo.win + "승 " + PlayerInfo.playerInfo.lose + "패 " + PlayerInfo.playerInfo.draw + "무 ";
        this.Character.sprite = Resources.Load<Sprite>("CharacterImage/" + PlayerInfo.playerInfo.GetEquipItem(ItemType.Character));
        this.Dice.sprite = Resources.Load<Sprite>("DiceImage/" + PlayerInfo.playerInfo.GetEquipItem(ItemType.Dice));
        if(PlayerInfo.playerInfo.equipLegacy1 != "null")
        {
            this.Legacy1.sprite = Resources.Load<Sprite>("LegacyImage/" + PlayerInfo.playerInfo.equipLegacy1);
        }
        if (PlayerInfo.playerInfo.equipLegacy2 != "null")
        {
            this.Legacy2.sprite = Resources.Load<Sprite>("LegacyImage/" + PlayerInfo.playerInfo.equipLegacy2);
        }
        if (PlayerInfo.playerInfo.equipLegacy3 != "null")
        {
            this.Legacy3.sprite = Resources.Load<Sprite>("LegacyImage/" + PlayerInfo.playerInfo.equipLegacy3);
        }*/
    }

    [PunRPC]
    void CheckMaster()
    {
        if (PV.Controller.IsMasterClient)
        {
            this.transform.Find("RoomMaster").gameObject.SetActive(true);
        }
        else
        {
            this.transform.Find("RoomMaster").gameObject.SetActive(false);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) //나가면 모두 실행
    {
        if (PV.Controller.IsMasterClient)
        {
            this.transform.Find("RoomMaster").gameObject.SetActive(true);
        }
        else
        {
            this.transform.Find("RoomMaster").gameObject.SetActive(false);
        }

    }

    public void ReadySet(bool isReady)
    {
        PV.RPC("ReadyStateImage", RpcTarget.AllBuffered, isReady);
    }

    [PunRPC]
    void ReadyStateImage(bool isReady)
    {
        if (isReady == true)
        {
            this.transform.Find("NowReady").gameObject.SetActive(false);
        }
        else if (isReady == false)
        {
            this.transform.Find("NowReady").gameObject.SetActive(true);
        }
    }
}

