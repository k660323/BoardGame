using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacterController : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView PV;
    public Rigidbody rb;
    Animator anim;
    // 목표 위치
    Vector3 target;
    Vector3 DestPos;
    Quaternion DestRot;

    // 플레이어 움직임
    public bool isMove;
    public bool isTel;
    
    // 방향
    public  bool isDir; //true 양 false 음
    public bool isX, isZ;
    
    // 거리
    public int moveDistance;

    // 완주
    public int laps;
    
    // 스피드
    public float speed = 5f;
    
    // 카드
    public int MagicCard = -1;

    // 플레이어 카드
    public GameObject playerInfo;

    // 돈
    public int havingMoney;
    public int totalMoney;

    public bool isbankRuptcy; //파산 여부
    public bool isloan; // 대출 여부

    GameController gameController;

    GameObject Maker;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(new Vector3(38.85f, 32.099f, -8.897f));

            stream.SendNext(havingMoney);
            stream.SendNext(totalMoney);

            stream.SendNext(havingMoney);
            stream.SendNext(totalMoney);

            stream.SendNext(isbankRuptcy);
            stream.SendNext(isloan);

            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            this.transform.GetChild(0).rotation = Quaternion.Euler((Vector3)stream.ReceiveNext());

            havingMoney = (int)stream.ReceiveNext();
            totalMoney = (int)stream.ReceiveNext();

            playerInfo.transform.Find("HavingMoney").GetComponent<Text>().text = "보유 자산 : " + stream.ReceiveNext().ToString();
            playerInfo.transform.Find("TotalMoney").GetComponent<Text>().text = "총 자산 : " + stream.ReceiveNext().ToString();

            isbankRuptcy = (bool)stream.ReceiveNext();
            isloan = (bool)stream.ReceiveNext();

            DestPos = (Vector3)stream.ReceiveNext();
            DestRot = (Quaternion)stream.ReceiveNext();
        }
    }

    void Awake()
    {
        if (PV.IsMine)
        {
            PV.RPC("setID_PlayerInfo", RpcTarget.All, GameObject.Find("GameController").GetComponent<GameController>().myturn.ToString(), PlayerInfo.playerInfo.GetEquipItem(ItemType.Character), PhotonNetwork.NickName);
            // 플레이어 이름과 정보

            // 닉네임
            playerInfo.transform.Find("PlayerNickName").Find("Text").GetComponent<Text>().color = Color.green;
            playerInfo.transform.Find("Myturn").Find("Text").GetComponent<Text>().color = Color.green;


            // 닉네임 정보 OnPhotonSerializeView        
            havingMoney = 5000000;
            totalMoney = 5000000;
            playerInfo.transform.Find("HavingMoney").GetComponent<Text>().text = "보유 자산 : " + havingMoney.ToString();
            playerInfo.transform.Find("TotalMoney").GetComponent<Text>().text = "총 자산 : " + totalMoney.ToString();

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //움직임 여부
        isMove = false;
        isTel = false;
        // 움직일 방향
        isX = false;
        isZ = true;
        //움직임 방향 //true 양 false 음
        isDir = true;

        // 움직일 거리
        moveDistance = 0;

        // 몇바퀴
        laps = 0;


        // 파산 여부
        isbankRuptcy = false;
        // 대출 여부
        isloan = false;


        anim = gameObject.GetComponent<Animator>();
        gameController = GameObject.Find("GameController").GetComponent<GameController>();
        Maker = GameObject.Find("Quad");



        rb = gameObject.GetComponent<Rigidbody>();
        this.gameObject.layer = LayerMask.NameToLayer("Player" + gameObject.name);
        this.transform.Find("PlayerText").GetComponent<TextMesh>().color = SetTextColor(transform.name);

        this.transform.parent = GameObject.Find("GamePlayerGroup").transform;
        this.transform.position = new Vector3(0, 1, 0);
    }

    [PunRPC]
    void setID_PlayerInfo(string myturn ,string characterImage, string playerNickName)
    {
        this.transform.name = myturn;
        this.transform.Find("PlayerText").GetComponent<TextMesh>().text = playerNickName;
        playerInfo = GameObject.Find("Canvas").transform.Find("PlayerInfoDivision").Find(myturn).gameObject;
        playerInfo.transform.Find("CharacterImage").Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("CharacterImage/" + characterImage);
        playerInfo.transform.Find("PlayerNickName").Find("Text").GetComponent<Text>().text = playerNickName;
        playerInfo.SetActive(true);
    }

    Color SetTextColor(string name)
    {
        switch (name)
        {
            case "1":
                return Color.blue;
            case "2":
                return Color.yellow;
            case "3":
                return Color.grey;
            case "4":
                return Color.red;
            default:
                return Color.white;
        }
    }

    public void lapsMoney(int m) // 한바퀴 돌때마다 돈지급
    {
        havingMoney += m;
        totalMoney += m;
        playerInfo.transform.Find("HavingMoney").GetComponent<Text>().text = "보유 자산 : " + havingMoney.ToString();
        playerInfo.transform.Find("TotalMoney").GetComponent<Text>().text = "총 자산 : " + totalMoney.ToString();
    }

    public void htMoneyCaculation(int m) // 돈 계산 // 상대방에게 비용 지불
    {
        havingMoney += m;
        totalMoney += m;
        playerInfo.transform.Find("HavingMoney").GetComponent<Text>().text = "보유 자산 : " + havingMoney.ToString();
        playerInfo.transform.Find("TotalMoney").GetComponent<Text>().text = "총 자산 : " + totalMoney.ToString();
    }

    public void MoneyCaculation(int m) // 돈 계산 // 상대방에게 비용 지불
    {
        havingMoney += m;
        totalMoney += m;
        playerInfo.transform.Find("HavingMoney").GetComponent<Text>().text = "보유 자산 : " + havingMoney.ToString();
        playerInfo.transform.Find("TotalMoney").GetComponent<Text>().text = "총 자산 : " + totalMoney.ToString();
    }

    public void havingMoneyCaculation(int m) // 건물 구입시
    {
        havingMoney += m;
        playerInfo.transform.Find("HavingMoney").GetComponent<Text>().text = "보유 자산 : " + havingMoney.ToString();
    }

    public void totalMoneyCaculation(int m) // 전재산 // 건물 양도 받을시
    {
        totalMoney += m;
        playerInfo.transform.Find("HavingMoney").GetComponent<Text>().text = "보유 자산 : " + havingMoney.ToString();
        playerInfo.transform.Find("TotalMoney").GetComponent<Text>().text = "총 자산 : " + totalMoney.ToString();
    }

    #region 이동
    void Update()
    {
        if(PV.IsMine)
        {
            if (isMove)
            {
                anim.SetBool("Walk", true);
                if (isX)
                {
                    this.transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                }
                else if (isZ)
                {
                    this.transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
                }
            }
        }
        else
        {
            float distance = (DestPos - transform.position).sqrMagnitude;
            if (distance > 50)
            {
                transform.position = DestPos;
                transform.rotation = DestRot;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, DestPos, speed * Time.deltaTime);
                float t = Mathf.Clamp(20f * Time.deltaTime, 0f, 0.99999f);
                transform.rotation = Quaternion.Lerp(transform.rotation, DestRot, t);
            }
        }    
    }

    public void Move(int[] dice,float _speed) //주사위 나온 값 받아옴
    {
        // 움직일 거리
        moveDistance = (dice[0] + dice[1]) * 2; // 칸크기가 2이므로
        speed = _speed;

        MakerMove(isX, isZ, isDir, moveDistance);

        // 움직일 방향 설정과 목표지점 설정
        if (isX)
        {
            if (isDir)
                target = new Vector3(transform.position.x + moveDistance, 0, transform.position.z);
            else
                target = new Vector3(transform.position.x - moveDistance, 0, transform.position.z);
        }
        else if (isZ)
        {
            if (isDir)
                target = new Vector3(transform.position.x, 0, transform.position.z + moveDistance);
            else
                target = new Vector3(transform.position.x, 0, transform.position.z - moveDistance);
        }
        // 움직임
        isMove = true;
      
    }

    public void MakerMove(bool _isX,bool _isZ, bool _isDir,int _moveDistance)
    {
        Maker.GetComponent<PhotonView>().RequestOwnership();

        Vector3 startPosition = gameObject.transform.position;

        if(startPosition.x ==0 && startPosition.z >=0 && startPosition.z < 20) // 1번째 라인에서 시작
        {
            if((int)Math.Round(startPosition.z) + _moveDistance < 20)
            {
                Maker.transform.position = new Vector3(0, 0.15f, startPosition.z + _moveDistance);
            }
            else
            {
                _moveDistance -= 20 - (int)Math.Round(startPosition.z);
                Maker.transform.position = new Vector3(_moveDistance, 0.15f, 20);
            }
        }
        else if (startPosition.x >= 0 && startPosition.x < 20 && startPosition.z == 20) // 2번째 라인에서 시작
        {
            if ((int)Math.Round(startPosition.x) + _moveDistance < 20)
            {
                Maker.transform.position = new Vector3(startPosition.x + _moveDistance, 0.15f, 20);
            }
            else
            {
                _moveDistance -= 20 - (int)Math.Round(startPosition.x);
                Maker.transform.position = new Vector3(20, 0.15f, 20 - _moveDistance);
            }
        }
        else if (startPosition.x == 20 && startPosition.z > 0 && startPosition.z <= 20) // 3번째 라인에서 시작
        {
            if ((int)Math.Round(startPosition.z) - _moveDistance > 0)
            {
                Maker.transform.position = new Vector3(20, 0.15f, (int)Math.Round(startPosition.z) - _moveDistance);
            }
            else
            {
                _moveDistance -= (int)Math.Round(startPosition.z);
                Maker.transform.position = new Vector3(20-_moveDistance, 0.15f, 0);
            }
        }
        else if (startPosition.x > 0 && startPosition.x <= 20 && startPosition.z == 0) // 4번째 라인에서 시작
        {
            if ((int)Math.Round(startPosition.x) - _moveDistance > 0)
            {
                Maker.transform.position = new Vector3((int)Math.Round(startPosition.z) - _moveDistance, 0.15f, 0);
            }
            else
            {
                _moveDistance -= (int)Math.Round(startPosition.x);
                Maker.transform.position = new Vector3(0, 0.15f, _moveDistance);
            }
        }
    }

    void OnTriggerEnter(Collider colGround)
    {
        if (isMove || isTel) // 움직일때
        {
            if (colGround.gameObject.layer == LayerMask.NameToLayer("Corner1")) // 방향 전환 레이어 //z축 양의방향
            {
                laps++;
                isX = false;
                isZ = true;
                isDir = true;

                transform.position = new Vector3(0, 0, 0);

                moveDistance -= 2;

                if (moveDistance != 0)
                {
                    target = new Vector3(transform.position.x, 0, transform.position.z + moveDistance);

                }
                else if (moveDistance == 0)
                {
                    isMove = false;
                    anim.SetBool("Walk", false);
                    transform.position = colGround.gameObject.transform.position;
                  //  RoundPosition();
                    gameController.specialground(colGround.gameObject, gameObject);
                }
                Turn(0);
                lapsMoney(4000000); // 300만원 입금
            }
            else if (colGround.gameObject.layer == LayerMask.NameToLayer("Corner2")) //x축 양의방향
            {
                isX = true;
                isZ = false;
                isDir = true;

                transform.position = new Vector3(0, 0, 20);

                moveDistance -= 2;



                if (moveDistance != 0)
                {
                    target = new Vector3(transform.position.x + moveDistance, 0, transform.position.z);
                }
                else if (moveDistance == 0)
                {
                    isMove = false;
                    anim.SetBool("Walk", false);
                    transform.position = colGround.gameObject.transform.position;
                  //  RoundPosition();
                    gameController.specialground(colGround.gameObject, gameObject);
                }
                Turn(90);

            }
            else if (colGround.gameObject.layer == LayerMask.NameToLayer("Corner3")) // z축 음의 방향
            {
                isX = false;
                isZ = true;
                isDir = false;

                transform.position = new Vector3(20, 0, 20);
                moveDistance -= 2;


                if (moveDistance != 0)
                {
                    target = new Vector3(transform.position.x, 0, transform.position.z - moveDistance);
                }
                else if (moveDistance == 0)
                {
                    isMove = false;
                    anim.SetBool("Walk", false);
                    transform.position = colGround.gameObject.transform.position;
                  //  RoundPosition();
                    gameController.specialground(colGround.gameObject, gameObject);
                }
                Turn(180);
            }

            else if (colGround.gameObject.layer == LayerMask.NameToLayer("Corner4")) // x축 음의방향
            {
                isX = true;
                isZ = false;

                transform.position = new Vector3(20, 0, 0);
                moveDistance -= 2;

                isDir = false;
                if (moveDistance != 0)
                {
                    target = new Vector3(transform.position.x - moveDistance, 0, transform.position.z);
                }
                else if (moveDistance == 0)
                {
                    isMove = false;
                    anim.SetBool("Walk", false);
                    transform.position = colGround.gameObject.transform.position;
                  //  RoundPosition();
                    gameController.specialground(colGround.gameObject, gameObject);
                }
                Turn(270);
            }
            else if (colGround.gameObject.layer == LayerMask.NameToLayer("PLand"))// 살수 있는 땅
            {
                moveDistance -= 2;
                if (moveDistance == 0)
                {
                    isMove = false;
                    isTel = false;
                    anim.SetBool("Walk", false);
                    transform.position = colGround.gameObject.transform.position;
                   // RoundPosition();
                    gameController.GroundInfo(colGround.gameObject, gameObject, laps);
                }
            }
            else if (colGround.gameObject.layer == LayerMask.NameToLayer("EpicLand"))//특수지역
            {
                moveDistance -= 2;
                if (moveDistance == 0)
                {
                    isMove = false;
                    isTel = false;
                    anim.SetBool("Walk", false);
                    transform.position = colGround.gameObject.transform.position;
                   // RoundPosition();
                    gameController.specialground(colGround.gameObject, transform.gameObject);
                }
            }
        }
        if (Maker.transform.GetComponent<PhotonView>().IsMine)
            if (colGround.gameObject.layer == LayerMask.NameToLayer("Maker"))
            {
                Maker.transform.position = new Vector3(Maker.transform.position.x, -5, Maker.transform.position.z);
            }
    }

 
    /*
    void RoundPosition()
    {
        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
    }
    */

    public void Turn(int YRotation)
    {
        this.transform.rotation = Quaternion.Euler(new Vector3(0, YRotation, 0));
        this.transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(38.85f, 32.099f, -8.897f));
    }

    #endregion
}
