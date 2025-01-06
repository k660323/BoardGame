using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System;

public class GameController : MonoBehaviourPunCallbacks
{
    public PhotonView PV;

    public Text[] ChatText;
    public InputField ChatInput;
    public Image ChatBg;

    public GameObject SequenceAppoint;
    public GameObject CardManager;
    public GameObject CardWindow;
    public GameObject CardUseWindow;
    public GameObject CardChangeRWindow;
    public GameObject TreasureWindow;

    public GameObject[] Dice;

    public GameObject DiceCheckzone;
    public CardInfo ci;
    float LimitTime;
    public Text Timer;

    bool isGame;
    bool isChangeTurn;
    bool isPause;
    bool isSale;
    bool isgambling;


    bool isTravel;
    bool isRest;
    bool isChat;

    public int myturn;
    int oddEvenDiceCount;

    public static int dicecount;

    int gameCost;
    int baetingcount;
    int currentReward;
    int RewardRatio;
    int totalRewardMoney;

    int isUseCard = -1; // 사용x 


    string nowState;

    public Text GameRoundText;
    public int GameRound;

    public int PlayerTurn;

    public int[] dice;
    public int doubleCount;

    int travelDestination;

    public List<int> playerList;

    TextMesh _TextMesh;

    public Text text, text1;

    int CardNumber;

    int rpcCount;

    //축제 땅
    public GameObject festivalGround;
    public int festival;

    Animator[] treasureAnim;
    Animator[] contentAnim;
    Animator[] starAnim;

    //땅 패널과 플레이어 정보
    [SerializeField]
    GameObject gpi;

    GameObject panel;  // 땅 오브젝트
    GameObject player; // 자신 플레이어 오브젝트

    GameObject playerCharacter;
    //GameObject playerInfo; // 플레이어 패널 스크립트
    int laps;
    PanelSettingController pSC; // 땅 스크립트

    [Header("오디오")]
    public AudioClip ChatClip;
    public AudioClip SequenceClip;
    public AudioClip MyturnClip;
    public AudioClip DoubleClip;
    public AudioClip[] DiceResultClip;
    public AudioClip SuccessClip;
    public AudioClip FailClip;
    public AudioClip GreatClip;
    public AudioClip KartStartClip;
    public AudioClip DiceRollClip;
    public AudioClip WindowActiveClip;
    public AudioClip fallWaterClip;

    #region 초기화
    void Awake()
    {
        Init();
        if (PhotonNetwork.IsMasterClient)
            MasterInit();
    }

    void Init()
    {
        isGame = false;
        isChangeTurn = false;
        isPause = false;
        isTravel = false;
        isRest = false;
        isSale = false;
        isgambling = false;
        isChat = false;

        rpcCount = 0;
        festival = 1;
        doubleCount = 0;
        dicecount = 0;
        LimitTime = 30.0f;
        GameRound = 0;
        PlayerTurn = 1;
        myturn = 0;
        travelDestination = 0;
        oddEvenDiceCount = 5;
        playerList = new List<int>();

        dice = new int[2];

        treasureAnim = new Animator[4];
        contentAnim = new Animator[4];
        starAnim = new Animator[3];

        GameObject.Find("Canvas").transform.Find("Scroll View").Find("Scrollbar Vertical").GetComponent<Scrollbar>().value = 0;

        for (int i = 0; i < 4; i++)
        {
            treasureAnim[i] = TreasureWindow.transform.Find("Treasure").Find("Treasure" + i.ToString()).GetComponent<Animator>();
            contentAnim[i] = TreasureWindow.transform.Find("Treasure").Find("Image" + i.ToString()).GetComponent<Animator>();
        }

        for (int i = 0; i < 3; i++)
            starAnim[i] = TreasureWindow.transform.Find("PlayerState").Find("CurrentProcess").GetChild(i).GetComponent<Animator>();

        GameObject element;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < GameObject.Find("Map").transform.GetChild(i).childCount; j++)
            {
                PanelSettingController psc = GameObject.Find("Map").transform.GetChild(i).GetChild(j).GetComponent<PanelSettingController>();
                if (psc.gameObject.layer == LayerMask.NameToLayer("Corner4"))
                {
                    continue;
                }
                element = Instantiate(Resources.Load<GameObject>("GroundListCase/GroundInfo"));
                element.transform.parent = GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content");
                element.transform.localScale = Vector3.one;
                element.name = psc.gameObject.name;

                element.transform.Find("Title").GetComponent<Text>().text = psc.pI.name;
                element.transform.Find("BGImage").Find("Image").GetComponent<Image>().sprite = psc.pI.image;
                element.transform.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
    }

    void MasterInit()
    {
        List<int> sequenceList = new List<int>();
        int[] sequence = new int[PhotonNetwork.PlayerList.Length];


        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            sequenceList.Add(i + 1);

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            int rand = UnityEngine.Random.Range(0, sequenceList.Count);
            sequence[i] = sequenceList[rand];
            sequenceList.RemoveAt(rand);
        }

        PV.RPC("InitSetting", RpcTarget.All, sequence);
    }

    [PunRPC]
    void InitSetting(int[] sequence)
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            SequenceAppoint.transform.GetChild(i).gameObject.SetActive(true);
            SequenceAppoint.transform.GetChild(i).Find("Number").GetComponent<Text>().text = sequence[i].ToString();
        }
        SoundManager.instance.SFXPlay("시작안내사운드", SequenceClip);
    }
    #endregion

    #region 순서 확정후 게임 세팅 초기화
    public void choiceSequence(string s) // 유니티 버튼 참조
    {
        if (myturn == 0)
        {
            myturn = int.Parse(SequenceAppoint.transform.Find(s).Find("Number").GetComponent<Text>().text);
            SequenceAppoint.transform.Find(s).GetComponent<Image>().sprite = Resources.Load<Sprite>("BackGround/bg_ColorBlock2");
            PV.RPC("choiceSync", RpcTarget.AllBufferedViaServer, s, PhotonNetwork.NickName);
        }
    }


    [PunRPC]
    void choiceSync(string s, string nickName)
    {
        int count = 0;
        SequenceAppoint.transform.Find(s).Find("Text").gameObject.SetActive(false);
        SequenceAppoint.transform.Find(s).Find("Number").gameObject.SetActive(true);
        SequenceAppoint.transform.Find(s).Find("PlayerNickName").gameObject.SetActive(true);
        SequenceAppoint.transform.Find(s).Find("PlayerNickName").GetComponent<Text>().text = nickName;

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (SequenceAppoint.transform.GetChild(i).Find("Number").gameObject.activeSelf == true)
            {
                count++;
            }
        }

        if (count == PhotonNetwork.PlayerList.Length)
        {
            Invoke("GameInit", 4f);
            LimitTime = 5.0f;
        }
    }

    void GameInit()
    {
        GameRound = 1;
        GameRoundText.text = GameRound + "턴 / " + NetworkController.turn.ToString() + "턴";
        nowState = "DiceState";
        LimitTime = 30.0f;
        SequenceAppoint.gameObject.SetActive(false);
        playerCharacter = PhotonNetwork.Instantiate("Character/" + PlayerInfo.playerInfo.GetEquipItem(ItemType.Character), new Vector3(0, 1, 0), Quaternion.identity); // 리소스 폴더안의 복제할 오브젝트 이름  
        isGame = true;
    }
    #endregion


    // Update is called once per frame
    void Update()
    {
        if (isGame)
        {
            if (LimitTime > 0)
            {
                if (PV.Controller.IsLocal)
                    PV.RPC("TimeSync", RpcTarget.AllBufferedViaServer, Time.deltaTime);

                if (!isChangeTurn)
                {
                    isChangeTurn = true;
                    PV.RPC("ActiveDicePanel", RpcTarget.All);
                }

                if (myturn == PlayerTurn && nowState == "DiceState" && isChangeTurn && GameObject.Find("Canvas").transform.Find("DiceRollButton").GetComponent<Button>().interactable == true && isChat == false)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        RollDiceButton();
                    }
                }
            }
            else
            {
                if (myturn == PlayerTurn)
                {
                    switch (nowState)
                    {
                        case "DiceState":
                            RollDiceButton();
                            break;
                        case "PurchasePanelState":
                            CloseWindow(GameObject.Find("Canvas").transform.Find("PurchaseWindow").gameObject);
                            break;
                        case "BankRuptcySelectState":
                            BankRuptcySelect(1);
                            break;
                        case "TakeOverWindowState":
                            CloseWindow(GameObject.Find("Canvas").transform.Find("TakeOverWindow").gameObject);
                            break;
                        case "SpeedTravelState":
                        case "AddFlagState":
                        case "FestiverState":
                            CloseWindow(GameObject.Find("Canvas").transform.Find("GroundList").gameObject);
                            break;
                        case "DrawCardState":
                            ChangeCard();
                            break;
                        case "ReChangeCardState":
                            CloseWindow(CardChangeRWindow);
                            break;
                        case "CardUseState":
                            AutoShowCardCancelEvent();
                            break;
                        case "TreasureHouseState":
                            PV.RPC("RPCCloseWindow", RpcTarget.AllBufferedViaServer);
                            break;
                        default:
                            Debug.Log(nowState + "현재 상태");
                            break;
                    }
                }
            }            
        }
        else
        {
            if (LimitTime > 0)
            {
                if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
                    PV.RPC("TimeSync", RpcTarget.AllBufferedViaServer, Time.deltaTime);
            }
        }

        if (Input.GetKeyUp(KeyCode.Return))
        {
            if (ChatInput.text != "")
            {
                isChat = false;
                Send();
                ChatInput.Select(); // inputfield 포커스 해제 글 못적음
                ChatInput.transform.GetChild(1).GetComponent<Text>().text = "채팅을 입력하세요";
            }
            else
            {
                isChat = true;
                ChatInput.ActivateInputField(); // inputfield 포커스 글적을수있음
                ChatInput.transform.GetChild(1).GetComponent<Text>().text = "";
            }
        }
    }

    #region 시간

    [PunRPC]
    void TimeSync(float time)
    {
        LimitTime -= time;
        Timer.text = "남은 시간\n" + Math.Round(LimitTime);
    }

    [PunRPC]
    void RPCSetTime(float _LimitTime)
    {
        LimitTime = _LimitTime;
    }

    [PunRPC]
    void TimePause(bool _isPause)
    {
        isPause = _isPause;
    }

    #endregion

    #region 주사위 and 무브
    [PunRPC]
    void ActiveDicePanel()
    {
        if (rpcCount == 1) // update문에서 한번만 호출할려 했으나 RPC속도? Update속도? 때문에 2번 호출됨 그래서 1번만 호출되도록 조건문을 검
        {
            if (PlayerTurn == myturn)
            {
                if (isRest == false && playerCharacter.GetComponent<PlayerCharacterController>().isbankRuptcy == false)
                {
                    SoundManager.instance.SFXPlay("나의턴", MyturnClip);

                    if (isTravel == false)
                    {
                        GameObject.Find("Canvas").transform.Find("DiceRollButton").gameObject.SetActive(true);
                        GameObject.Find("Canvas").transform.Find("DiceRollButton").GetComponent<Button>().interactable = true;

                        GameObject.Find("Canvas").transform.Find("OddEvenToggleGroup").GetChild(0).GetComponent<Toggle>().isOn = false;
                        GameObject.Find("Canvas").transform.Find("OddEvenToggleGroup").GetChild(1).GetComponent<Toggle>().isOn = false;
                        GameObject.Find("Canvas").transform.Find("OddEvenToggleGroup").gameObject.SetActive(true);
                        if (oddEvenDiceCount <= 0)
                        {
                            GameObject.Find("Canvas").transform.Find("OddEvenToggleGroup").GetChild(0).GetComponent<Toggle>().interactable = false;
                            GameObject.Find("Canvas").transform.Find("OddEvenToggleGroup").GetChild(1).GetComponent<Toggle>().interactable = false;
                        }
                        else
                        {
                            GameObject.Find("Canvas").transform.Find("OddEvenToggleGroup").GetChild(0).GetComponent<Toggle>().interactable = true;
                            GameObject.Find("Canvas").transform.Find("OddEvenToggleGroup").GetChild(1).GetComponent<Toggle>().interactable = true;
                        }

                        GameObject.Find("Canvas").transform.Find("OddEvenToggleGroup").GetChild(0).Find("Label").GetComponent<Text>().text = "홀x" + oddEvenDiceCount.ToString();
                        GameObject.Find("Canvas").transform.Find("OddEvenToggleGroup").GetChild(1).Find("Label").GetComponent<Text>().text = "짝x" + oddEvenDiceCount.ToString();
                    }
                    else
                    {
                        PV.RPC("RPCSetTime", RpcTarget.AllViaServer, 30.0f);
                        SpeedMove(20.0f);
                    }
                }
                else
                {
                    ChangeTurn();
                    isRest = false;
                }
            }
            else
            {
                GameObject.Find("Canvas").transform.Find("DiceRollButton").gameObject.SetActive(false);
                GameObject.Find("Canvas").transform.Find("OddEvenToggleGroup").gameObject.SetActive(false);
            }
        }
        else
        {
            rpcCount++;
        }
    }

    public void RollDiceButton()
    {
        PV.RPC("RPCDiceRollSound", RpcTarget.AllBufferedViaServer);
        PV.RPC("RPCSetTime", RpcTarget.All, 30.0f);

        GameObject.Find("Canvas").transform.Find("DiceRollButton").GetComponent<Button>().interactable = false;
        GameObject.Find("Canvas").transform.Find("OddEvenToggleGroup").GetChild(0).GetComponent<Toggle>().interactable = false;
        GameObject.Find("Canvas").transform.Find("OddEvenToggleGroup").GetChild(1).GetComponent<Toggle>().interactable = false;

        
        dicecount = 0;

        Dice[0].GetComponent<PhotonView>().RequestOwnership();
        Dice[1].GetComponent<PhotonView>().RequestOwnership();

        if (GameObject.Find("Canvas").transform.Find("OddEvenToggleGroup").GetChild(0).GetComponent<Toggle>().isOn) // 홀수
        {
            Dice[0].GetComponent<DiceRoller>().DiceRoll("OddDiceMaterial");
            Dice[1].GetComponent<DiceRoller>().DiceRoll("EvenDiceMaterial");
            oddEvenDiceCount--;
            GameObject.Find("Canvas").transform.Find("OddEvenToggleGroup").GetChild(0).Find("Label").GetComponent<Text>().text = "홀x" + oddEvenDiceCount.ToString();
            GameObject.Find("Canvas").transform.Find("OddEvenToggleGroup").GetChild(1).Find("Label").GetComponent<Text>().text = "짝x" + oddEvenDiceCount.ToString();
        }
        else if(GameObject.Find("Canvas").transform.Find("OddEvenToggleGroup").GetChild(1).GetComponent<Toggle>().isOn) // 짝수
        {
            if(UnityEngine.Random.Range(1, 2) == 1) // 홀 + 홀
            {
                Dice[0].GetComponent<DiceRoller>().DiceRoll("OddDiceMaterial");
                Dice[1].GetComponent<DiceRoller>().DiceRoll("OddDiceMaterial");
            }
            else // 짝 + 짝
            {
                Dice[0].GetComponent<DiceRoller>().DiceRoll("EvenDiceMaterial");
                Dice[1].GetComponent<DiceRoller>().DiceRoll("EvenDiceMaterial");
            }
            oddEvenDiceCount--;
            GameObject.Find("Canvas").transform.Find("OddEvenToggleGroup").GetChild(0).Find("Label").GetComponent<Text>().text = "홀x" + oddEvenDiceCount.ToString();
            GameObject.Find("Canvas").transform.Find("OddEvenToggleGroup").GetChild(1).Find("Label").GetComponent<Text>().text = "짝x" + oddEvenDiceCount.ToString();
        }
        else // 일반 주사위
        {
            Dice[0].GetComponent<DiceRoller>().DiceRoll("NormalDiceMaterial");
            Dice[1].GetComponent<DiceRoller>().DiceRoll("NormalDiceMaterial");
        }

        DiceCheckzone.GetComponent<DiceCheckZone>().InitDiceZone();
        
        /*
        dice[0] = int.Parse(text.text);
        dice[1] = int.Parse(text1.text);
        Debug.Log("주사위 1 : " + dice[0]);
        Debug.Log("주사위 2 : " + dice[1]);
        DiceMove(dice, 5.0f);*/

    }

    public void SpeedMove(float _speed)
    {
        isTravel = false;
        dice[0] = travelDestination / 2;
        dice[1] = 0;
        PV.RPC("RPCKartStartSound", RpcTarget.AllBufferedViaServer);
        for (int i = 0; i < GameObject.Find("GamePlayerGroup").transform.childCount; i++)
        {
            if (GameObject.Find("GamePlayerGroup").transform.GetChild(i).gameObject.GetComponent<PhotonView>().Controller.IsLocal)
            {
                GameObject.Find("GamePlayerGroup").transform.GetChild(i).GetComponent<PlayerCharacterController>().Move(dice, _speed);
            }
        }
    }

    public void DiceMove(int[] _dice, float _speed)
    {
        dice[0] = _dice[0];
        dice[1] = _dice[1];

        if (dice[0] == dice[1])
        {
            if (++doubleCount == 3)
            {
                StartCoroutine("GotoRoost");
            }
            else
            {
                for (int i = 0; i < GameObject.Find("GamePlayerGroup").transform.childCount; i++)
                {
                    if (GameObject.Find("GamePlayerGroup").transform.GetChild(i).gameObject.GetComponent<PhotonView>().Controller.IsLocal)
                    {
                        GameObject.Find("GamePlayerGroup").transform.GetChild(i).GetComponent<PlayerCharacterController>().Move(dice, _speed);
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < GameObject.Find("GamePlayerGroup").transform.childCount; i++)
            {
                if (GameObject.Find("GamePlayerGroup").transform.GetChild(i).gameObject.GetComponent<PhotonView>().Controller.IsLocal)
                {
                    GameObject.Find("GamePlayerGroup").transform.GetChild(i).GetComponent<PlayerCharacterController>().Move(dice, _speed);
                }
            }
        }
        PV.RPC("RPCDiceSound", RpcTarget.AllBufferedViaServer, dice[0] == dice[1], dice[0] + dice[1]);

    }

    IEnumerator GotoRoost()
    {
        doubleCount = 0;
        player.GetComponent<Rigidbody>().AddForce(transform.up * 1000);
        yield return new WaitForSeconds(1.5f);
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.transform.position = new Vector3(0, 10, 20);
        player.GetComponent<PlayerCharacterController>().isTel = true;
        player.GetComponent<PlayerCharacterController>().moveDistance = 2;
    }

    #endregion

    #region 특수 지역
    public void specialground(GameObject _panel, GameObject _player) // 미구현
    {
        this.panel = _panel;  // 땅 오브젝트
        this.player = _player; // 자신 플레이어 오브젝트     

        switch (this.panel.gameObject.layer)
        {
            case 8: // 시작 코너 추가 건물 건설 //랜드마크 건물 제외 출력
                GroundList("AddFlagState");
                break;
            case 9: // 1번째 코너
                RestLand();
                break;
            case 10: // 2번째 코너 가격 2x 상승
                GroundList("FestiverState");
                break;
            case 11: // 원하는 지점으로 이동
                GroundList("SpeedTravelState");
                break;
            case 13: //에픽랜드
                EpicLand(this.panel.gameObject.GetComponent<PanelSettingController>().name);
                break;
        }
    }


    #region 땅 문서
    void GroundList(string active)
    {
        GameObject element;
        int count = 0;
        int myGroundCount = 0;
        nowState = active;

        for (int i = 0; i < GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").childCount; i++)
        {
            GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(i).gameObject.SetActive(false);
            GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(i).Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
        }
        GameObject.Find("Canvas").transform.Find("GroundList").Find("CloseButton").GetComponent<Button>().onClick.RemoveAllListeners();


        switch (active)
        {
            case "SpeedTravelState":
                GameObject.Find("Canvas").transform.Find("GroundList").Find("Title").Find("Text").GetComponent<Text>().text = "고속 여행";
                if (player.GetComponent<PlayerCharacterController>().havingMoney >= 250000)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < GameObject.Find("Map").transform.GetChild(i).childCount; j++)
                        {
                            GameObject ground = GameObject.Find("Map").transform.GetChild(i).GetChild(j).gameObject;
                            if (ground.layer == LayerMask.NameToLayer("Corner4"))
                            {
                                continue;
                            }

                            element = GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject;
                            element.SetActive(true);
                            element.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { speedTravel(player, ground); });
                            element.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { player.GetComponent<PlayerCharacterController>().htMoneyCaculation(-250000); });
                            element.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { GameObject.Find("Canvas").transform.Find("GroundList").gameObject.SetActive(false); });
                            count++;
                        }
                    }
                    GameObject.Find("Canvas").transform.Find("GroundList").gameObject.SetActive(true);
                    GameObject.Find("Canvas").transform.Find("GroundList").Find("CloseButton").GetComponent<Button>().onClick.AddListener(() => { CloseWindow(GameObject.Find("Canvas").transform.Find("GroundList").gameObject); });
                    GameObject.Find("Canvas").transform.Find("GroundList").Find("ToggleGroup").Find("Toggle0").GetComponent<Toggle>().isOn = true;
                    PV.RPC("RPCSetTime", RpcTarget.AllBufferedViaServer, 30.0f);
                }
                else
                {
                    ChangeTurn();
                }
                break;
            case "AddFlagState":
                Debug.Log("깃발 추가");
                GameObject.Find("Canvas").transform.Find("GroundList").Find("Title").Find("Text").GetComponent<Text>().text = "추가 건설";
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < GameObject.Find("Map").transform.GetChild(i).childCount; j++)
                    {
                        if (i == 3 && GameObject.Find("Map").transform.GetChild(i).GetChild(j).gameObject.layer == LayerMask.NameToLayer("Corner4"))
                            continue;

                        if (GameObject.Find("Map").transform.GetChild(i).GetChild(j).GetComponent<PanelSettingController>().pI.owner == player.name)
                        {
                            Debug.Log(GameObject.Find("Map").transform.GetChild(i).GetChild(j).GetComponent<PanelSettingController>().pI.name.Substring(0, 2) + "땅 이름");
                            if (GameObject.Find("Map").transform.GetChild(i).GetChild(j).GetComponent<PanelSettingController>().pI.name.Substring(0, 2) != "유물")
                            {
                                myGroundCount++;
                                PanelSettingController psc = GameObject.Find("Map").transform.GetChild(i).GetChild(j).GetComponent<PanelSettingController>();

                                element = GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject;
                                element.SetActive(true);
                                Debug.Log(element + "내가 소유한 땅");

                                element.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() =>
                                { GroundInfo(psc.gameObject, player, player.GetComponent<PlayerCharacterController>().laps); });
                                element.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { GameObject.Find("Canvas").transform.Find("GroundList").gameObject.SetActive(false); });
                            }

                        }
                        count++;
                    }
                }
                if (myGroundCount != 0)
                {
                    GameObject.Find("Canvas").transform.Find("GroundList").gameObject.SetActive(true);
                    GameObject.Find("Canvas").transform.Find("GroundList").Find("CloseButton").GetComponent<Button>().onClick.AddListener(() => { CloseWindow(GameObject.Find("Canvas").transform.Find("GroundList").gameObject); });
                    GameObject.Find("Canvas").transform.Find("GroundList").Find("ToggleGroup").Find("Toggle0").GetComponent<Toggle>().isOn = true;
                    PV.RPC("RPCSetTime", RpcTarget.AllBufferedViaServer, 30.0f);
                }
                else
                {
                    ChangeTurn();
                }
                break;
            case "FestiverState":
                GameObject.Find("Canvas").transform.Find("GroundList").Find("Title").Find("Text").GetComponent<Text>().text = "축제 개최";
                if (player.GetComponent<PlayerCharacterController>().havingMoney >= 250000)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < GameObject.Find("Map").transform.GetChild(i).childCount; j++)
                        {
                            if (i == 3 && GameObject.Find("Map").transform.GetChild(i).GetChild(j).gameObject.layer == LayerMask.NameToLayer("Corner4"))
                                continue;

                            if (GameObject.Find("Map").transform.GetChild(i).GetChild(j).GetComponent<PanelSettingController>().pI.owner == player.name)
                            {
                                myGroundCount++;
                                PanelSettingController psc = GameObject.Find("Map").transform.GetChild(i).GetChild(j).GetComponent<PanelSettingController>();

                                element = GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject;
                                element.SetActive(true);

                                if (player.GetComponent<PlayerCharacterController>().havingMoney >= 250000)
                                {
                                    element.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { Festiver(psc); });
                                    element.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() => { player.GetComponent<PlayerCharacterController>().htMoneyCaculation(-250000); });
                                }
                            }
                            count++;
                        }
                    }

                    if (myGroundCount != 0)
                    {
                        GameObject.Find("Canvas").transform.Find("GroundList").gameObject.SetActive(true);
                        GameObject.Find("Canvas").transform.Find("GroundList").Find("CloseButton").GetComponent<Button>().onClick.AddListener(() => { CloseWindow(GameObject.Find("Canvas").transform.Find("GroundList").gameObject); });
                        GameObject.Find("Canvas").transform.Find("GroundList").Find("ToggleGroup").Find("Toggle0").GetComponent<Toggle>().isOn = true;
                        PV.RPC("RPCSetTime", RpcTarget.AllBufferedViaServer, 30.0f);
                    }
                    else
                    {
                        ChangeTurn();
                    }
                }
                else
                {
                    ChangeTurn();
                }
                break;
        }
    }

    public void LineActive(int Line)
    {
        for (int i = 0; i < GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").childCount; i++)
        {
            GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(i).gameObject.SetActive(false);
        }

        if (nowState == "SpeedTravelState")
        {
            switch (Line)
            {
                case 0:
                    for (int i = 0; i < GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").childCount; i++)
                    {
                        GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(i).gameObject.SetActive(true);
                    }
                    break;
                case 1:
                    for (int i = 0; i < GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").childCount; i++)
                    {
                        if (int.Parse(GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(i).gameObject.name) < 10)
                            GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(i).gameObject.SetActive(true);
                    }
                    break;
                case 2:
                    for (int i = 0; i < GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").childCount; i++)
                    {
                        if (int.Parse(GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(i).gameObject.name) >= 10 && int.Parse(GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(i).gameObject.name) < 20)
                            GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(i).gameObject.SetActive(true);
                    }
                    break;
                case 3:
                    for (int i = 0; i < GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").childCount; i++)
                    {
                        if (int.Parse(GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(i).gameObject.name) >= 20 && int.Parse(GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(i).gameObject.name) < 30)
                            GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(i).gameObject.SetActive(true);
                    }
                    break;
                case 4:
                    for (int i = 0; i < GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").childCount; i++)
                    {
                        if (int.Parse(GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(i).gameObject.name) >= 30 && int.Parse(GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(i).gameObject.name) < 40)
                            GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(i).gameObject.SetActive(true);
                    }
                    break;
            }
        }
        else if (nowState == "AddFlagState")
        {
            int count;
            switch (Line)
            {
                case 0:
                    count = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < GameObject.Find("Map").transform.GetChild(i).childCount; j++)
                        {
                            if (i == 3 && GameObject.Find("Map").transform.GetChild(i).GetChild(j).gameObject.layer == LayerMask.NameToLayer("Corner4"))
                                continue;

                            if (GameObject.Find("Map").transform.GetChild(i).GetChild(j).GetComponent<PanelSettingController>().pI.owner == player.name)
                            {
                                if (GameObject.Find("Map").transform.GetChild(i).GetChild(j).GetComponent<PanelSettingController>().pI.name.Substring(0, 2) != "유물")
                                {
                                    GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject.SetActive(true);
                                }
                            }
                            else
                            {
                                GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject.SetActive(false);
                            }
                            count++;

                        }
                    }
                    break;
                case 1:
                    count = 0;
                    for (int j = 0; j < GameObject.Find("Map").transform.GetChild(0).childCount; j++)
                    {
                        if (GameObject.Find("Map").transform.GetChild(0).GetChild(j).GetComponent<PanelSettingController>().pI.owner == player.name)
                        {
                            if (GameObject.Find("Map").transform.GetChild(0).GetChild(j).GetComponent<PanelSettingController>().pI.name.Substring(0, 2) != "유물")
                            {
                                GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject.SetActive(true);
                            }
                        }
                        else
                        {
                            GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject.SetActive(false);
                        }
                        count++;
                    }
                    break;
                case 2:
                    count = 10;
                    for (int j = 0; j < GameObject.Find("Map").transform.GetChild(1).childCount; j++)
                    {
                        if (GameObject.Find("Map").transform.GetChild(1).GetChild(j).GetComponent<PanelSettingController>().pI.owner == player.name)
                        {
                            if (GameObject.Find("Map").transform.GetChild(1).GetChild(j).GetComponent<PanelSettingController>().pI.name.Substring(0, 2) != "유물")
                            {
                                GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject.SetActive(true);
                            }
                        }
                        else
                        {
                            GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject.SetActive(false);
                        }
                        count++;
                    }
                    break;
                case 3:
                    count = 20;
                    for (int j = 0; j < GameObject.Find("Map").transform.GetChild(2).childCount; j++)
                    {
                        if (GameObject.Find("Map").transform.GetChild(2).GetChild(j).GetComponent<PanelSettingController>().pI.owner == player.name)
                        {
                            if (GameObject.Find("Map").transform.GetChild(2).GetChild(j).GetComponent<PanelSettingController>().pI.name.Substring(0, 2) != "유물")
                            {
                                GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject.SetActive(true);
                            }
                        }
                        else
                        {
                            GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject.SetActive(false);
                        }
                        count++;
                    }
                    break;
                case 4:
                    count = 30;
                    for (int j = 1; j < GameObject.Find("Map").transform.GetChild(3).childCount; j++)
                    {
                        if (GameObject.Find("Map").transform.GetChild(3).GetChild(j).GetComponent<PanelSettingController>().pI.owner == player.name)
                        {
                            if (GameObject.Find("Map").transform.GetChild(3).GetChild(j).GetComponent<PanelSettingController>().pI.name.Substring(0, 2) != "유물")
                            {
                                GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject.SetActive(true);
                            }
                        }
                        else
                        {
                            GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject.SetActive(false);
                        }
                        count++;
                    }
                    break;
            }
        }
        else if (nowState == "FestiverState")
        {
            int count;
            switch (Line)
            {
                case 0:
                    count = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        for (int j = 0; j < GameObject.Find("Map").transform.GetChild(i).childCount; j++)
                        {
                            if (i == 3 && GameObject.Find("Map").transform.GetChild(i).GetChild(j).gameObject.layer == LayerMask.NameToLayer("Corner4"))
                                continue;

                            if (GameObject.Find("Map").transform.GetChild(i).GetChild(j).GetComponent<PanelSettingController>().pI.owner == player.name)
                            {
                                GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject.SetActive(true);
                            }
                            else
                            {
                                GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject.SetActive(false);
                            }
                            count++;

                        }
                    }
                    break;
                case 1:
                    count = 0;
                    for (int j = 0; j < GameObject.Find("Map").transform.GetChild(0).childCount; j++)
                    {
                        if (GameObject.Find("Map").transform.GetChild(0).GetChild(j).GetComponent<PanelSettingController>().pI.owner == player.name)
                        {
                            GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject.SetActive(true);
                        }
                        else
                        {
                            GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject.SetActive(false);
                        }
                        count++;
                    }
                    break;
                case 2:
                    count = 10;
                    for (int j = 0; j < GameObject.Find("Map").transform.GetChild(1).childCount; j++)
                    {
                        if (GameObject.Find("Map").transform.GetChild(1).GetChild(j).GetComponent<PanelSettingController>().pI.owner == player.name)
                        {
                            GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject.SetActive(true);
                        }
                        else
                        {
                            GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject.SetActive(false);
                        }
                        count++;
                    }
                    break;
                case 3:
                    count = 20;
                    for (int j = 0; j < GameObject.Find("Map").transform.GetChild(2).childCount; j++)
                    {
                        if (GameObject.Find("Map").transform.GetChild(2).GetChild(j).GetComponent<PanelSettingController>().pI.owner == player.name)
                        {
                            GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject.SetActive(true);
                        }
                        else
                        {
                            GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject.SetActive(false);
                        }
                        count++;
                    }
                    break;
                case 4:
                    count = 30;
                    for (int j = 1; j < GameObject.Find("Map").transform.GetChild(3).childCount; j++)
                    {
                        if (GameObject.Find("Map").transform.GetChild(3).GetChild(j).GetComponent<PanelSettingController>().pI.owner == player.name)
                        {
                            GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject.SetActive(true);
                        }
                        else
                        {
                            GameObject.Find("Canvas").transform.Find("GroundList").Find("MaskArea").Find("Content").GetChild(count).gameObject.SetActive(false);
                        }
                        count++;
                    }
                    break;
            }
        }

    }


    #endregion

    #region 휴식
    void RestLand()
    {
        Debug.Log("쉬는 함수 호출");
        if (player.GetComponent<PlayerCharacterController>().MagicCard == 2)
        {
            ShowCardInfo();
        }
        else
        {
            isRest = true;
            ChangeTurn();
        }
    }
    #endregion

    #region 페스티벌
    void Festiver(PanelSettingController psc)
    {
        Debug.Log(psc.gameObject + "축제 땅 이름 ");
        GameObject.Find("Canvas").transform.Find("GroundList").gameObject.SetActive(false);

        if (festivalGround != null)
        {
            PanelSettingController bPSC = festivalGround.GetComponent<PanelSettingController>();
            int beforeMoney = bPSC.pI.price;

            if (bPSC.pI.name.Substring(0, 2) != "유물")
                for (int i = 0; i < 4; i++)
                {
                    if (psc.gameObject.transform.Find("Flag").GetChild(i).gameObject.activeInHierarchy == true)
                    {
                        beforeMoney += bPSC.pI.flagValue[i];
                    }
                }

            PV.RPC("RPCReSetFestiver", RpcTarget.All);
            //PV.RPC("RPCSetPanelValue", RpcTarget.All, int.Parse(bPSC.transform.gameObject.name), bPSC.pI.name + "\n" + (beforeMoney / 10000) / 2 + "만원");

            if (((beforeMoney / 2) / 1000).ToString().Substring(((beforeMoney / 2) / 1000).ToString().Length - 1, 1) != "0")
                PV.RPC("RPCSetPanelValue", RpcTarget.All, int.Parse(bPSC.transform.gameObject.name), bPSC.pI.name + "\n" + (beforeMoney / 10000) / 2 + "만" + (beforeMoney / 2).ToString().Substring((beforeMoney / 2).ToString().Length - 4, 4));
            else
                PV.RPC("RPCSetPanelValue", RpcTarget.All, int.Parse(bPSC.transform.gameObject.name), bPSC.pI.name + "\n" + (beforeMoney / 10000) / 2 + "만");
        }

        int money = psc.pI.price;
        if (psc.pI.name.Substring(0, 2) != "유물")
            for (int i = 0; i < 4; i++)
            {
                if (psc.gameObject.transform.Find("Flag").GetChild(i).gameObject.activeInHierarchy == true)
                {
                    money += psc.pI.flagValue[i];
                }
            }

        PV.RPC("RPCSetFestiver", RpcTarget.All, int.Parse(psc.transform.gameObject.name));
        //PV.RPC("RPCSetPanelValue", RpcTarget.All, int.Parse(psc.transform.gameObject.name), psc.pI.name + "\n" + ((money * festival) / 10000)/2 + "만원");
        if ((((money * festival) / 2) / 1000).ToString().Substring((((money * festival) / 2) / 1000).ToString().Length - 1, 1) != "0")
            PV.RPC("RPCSetPanelValue", RpcTarget.All, int.Parse(psc.transform.gameObject.name), psc.pI.name + "\n" + ((money * festival) / 10000) / 2 + "만" + ((money * festival) / 2).ToString().Substring(((money * festival) / 2).ToString().Length - 4, 4));
        else
            PV.RPC("RPCSetPanelValue", RpcTarget.All, int.Parse(psc.transform.gameObject.name), psc.pI.name + "\n" + ((money * festival) / 10000) / 2 + "만원");
        ChangeTurn();

    }

    [PunRPC]
    void RPCSetFestiver(int _panelNumber)
    {
        festival *= 2;

        GameObject ground;

        if (_panelNumber < 10)
        {
            ground = GameObject.Find("Map").transform.GetChild(0).gameObject;
        }
        else if (_panelNumber < 20)
        {
            ground = GameObject.Find("Map").transform.GetChild(1).gameObject;
        }
        else if (_panelNumber < 30)
        {
            ground = GameObject.Find("Map").transform.GetChild(2).gameObject;
        }
        else
        {
            ground = GameObject.Find("Map").transform.GetChild(3).gameObject;
        }


        ground.transform.Find(_panelNumber.ToString()).GetComponent<PanelSettingController>().pI.festiver = true;
        festivalGround = ground.transform.Find(_panelNumber.ToString()).gameObject;
    }

    [PunRPC]
    void RPCReSetFestiver()
    {
        festivalGround.GetComponent<PanelSettingController>().pI.festiver = false;
    }
    #endregion

    #region 고속 여행
    void speedTravel(GameObject mycharacter, GameObject ground)
    {
        if (ground.transform.position.x > 0 && ground.transform.position.x <= 20 && ground.transform.position.z == 0) // 4라인
        {
            travelDestination = (int)(20 - ground.transform.position.x);
        }
        else if (ground.transform.position.x == 0 && ground.transform.position.z >= 0 && ground.transform.position.z < 20) // 1라인
        {
            travelDestination = 20;
            travelDestination += (int)(ground.transform.position.z);
        }
        else if (ground.transform.position.x >= 0 && ground.transform.position.x < 20 && ground.transform.position.z == 20) // 2라인
        {
            travelDestination = 40;
            travelDestination += (int)(ground.transform.position.x);
        }
        else if (ground.transform.position.x == 20 && ground.transform.position.z > 0 && ground.transform.position.z <= 20) // 3라인
        {
            travelDestination = 60;
            travelDestination += (int)(20 - ground.transform.position.z);
        }
        isTravel = true;
        ChangeTurn();
    }
    #endregion

    #region 특수 이벤트 지역
    void EpicLand(string ColGroundName)
    {
        switch (ColGroundName)
        {
            case "마법카드":
                MagicCard();
                break;
            case "폭포수":
                waterfall();
                break;
            case "보물창고":
                TreasureHouse();
                break;
        }
    }

    #region 마법카드
    void MagicCard()
    {
        nowState = "DrawCardState";

        CardNumber = UnityEngine.Random.Range(0, CardManager.GetComponent<CardManager>().CardList.Count);

        CardManager.GetComponent<CardManager>().CardList.TryGetValue(CardNumber, out ci);
        CardWindow.transform.Find("Name").Find("Text").GetComponent<Text>().text = ci.name;
        CardWindow.transform.Find("Text").GetComponent<Text>().text = ci.ex;
        CardWindow.transform.Find("Image").GetComponent<Image>().sprite = ci.image;

        PV.RPC("RPCSetTime", RpcTarget.All, 15.0f);
        CardWindow.gameObject.SetActive(true);
    }

    public void ChangeCard()
    {
        if (player.GetComponent<PlayerCharacterController>().MagicCard != -1)
        {
            nowState = "ReChangeCardState";
            PV.RPC("RPCSetTime", RpcTarget.All, 15.0f);
            CardWindow.gameObject.SetActive(false);
            CardChangeRWindow.gameObject.SetActive(true);
        }
        else
        {
            player.GetComponent<PlayerCharacterController>().MagicCard = CardNumber;
            player.GetComponent<PlayerCharacterController>().playerInfo.transform.Find("CardImage").GetComponent<Image>().sprite = ci.image;
            player.GetComponent<PlayerCharacterController>().playerInfo.transform.Find("CardImage").gameObject.SetActive(true);
            PV.RPC("RPCCardImage", RpcTarget.Others, player.name, true);
            Debug.Log(player.name + "플레이어 이름");
            CloseWindow(CardWindow);
        }
    }

    public void ReChangeCard()
    {
        player.GetComponent<PlayerCharacterController>().MagicCard = CardNumber;
        player.GetComponent<PlayerCharacterController>().playerInfo.transform.Find("CardImage").GetComponent<Image>().sprite = ci.image;
    }

    void useCard()
    {
        player.GetComponent<PlayerCharacterController>().MagicCard = -1;
        player.GetComponent<PlayerCharacterController>().playerInfo.transform.Find("CardImage").gameObject.SetActive(false);
        PV.RPC("RPCCardImage", RpcTarget.Others, player.name, false);
    }

    [PunRPC]
    void RPCCardImage(string TargetPlayer, bool isVis)
    {
        Debug.Log(TargetPlayer + "타켓 플레이어");
        GameObject.Find("Canvas").transform.Find("PlayerInfoDivision").transform.Find(TargetPlayer).Find("CardImage").gameObject.SetActive(isVis);
    }

    void ShowCardInfo()
    {
        CardUseWindow.transform.Find("ConfirmBtn").GetComponent<Button>().onClick.RemoveAllListeners();
        CardUseWindow.transform.Find("CancelBtn").GetComponent<Button>().onClick.RemoveAllListeners();

        CardInfo cardinfo;
        nowState = "CardUseState";
        CardManager.GetComponent<CardManager>().CardList.TryGetValue(player.GetComponent<PlayerCharacterController>().MagicCard, out cardinfo);
        CardUseWindow.transform.Find("Name").Find("Text").GetComponent<Text>().text = cardinfo.name;
        CardUseWindow.transform.Find("Image").GetComponent<Image>().sprite = cardinfo.image;
        CardUseWindow.transform.Find("Text").GetComponent<Text>().text = cardinfo.ex;

        switch (player.GetComponent<PlayerCharacterController>().MagicCard)
        {
            case 0: // 천사카드              
                CardUseWindow.transform.Find("ConfirmBtn").GetComponent<Button>().onClick.AddListener(() => {
                    useCard();
                    ChangeMasterPanel();
                    GroundInfo(panel, player, laps);
                });

                CardUseWindow.transform.Find("CancelBtn").GetComponent<Button>().onClick.AddListener(() => {
                    AccessPayment();
                });
                break;
            case 1: // 방어(미구현)
                CardUseWindow.transform.Find("ConfirmBtn").GetComponent<Button>().onClick.AddListener(() => { useCard(); });
                CardUseWindow.transform.Find("CancelBtn").GetComponent<Button>().onClick.AddListener(() => { ChangeTurn(); });
                break;
            case 2: // 무인도 탈출
                CardUseWindow.transform.Find("ConfirmBtn").GetComponent<Button>().onClick.AddListener(() => {
                    useCard();
                    ChangeTurn();
                });

                CardUseWindow.transform.Find("CancelBtn").GetComponent<Button>().onClick.AddListener(() => {
                    isRest = true;
                    ChangeTurn();
                });

                break;
            case 3: // 할인 쿠폰
                CardUseWindow.transform.Find("ConfirmBtn").GetComponent<Button>().onClick.AddListener(() => {
                    useCard();
                    isSale = true;
                    AccessPayment();
                });

                CardUseWindow.transform.Find("CancelBtn").GetComponent<Button>().onClick.AddListener(() => { AccessPayment(); });
                break;
            default:
                break;
        }

        CardUseWindow.transform.Find("ConfirmBtn").GetComponent<Button>().onClick.AddListener(() => {
            CardUseWindow.gameObject.SetActive(false);
        });

        CardUseWindow.transform.Find("CancelBtn").GetComponent<Button>().onClick.AddListener(() => {
            CardUseWindow.gameObject.SetActive(false);
        });

        CardUseWindow.SetActive(true);
    }

    void AutoShowCardCancelEvent()
    {
        switch (player.GetComponent<PlayerCharacterController>().MagicCard)
        {
            case 0: // 천사카드                                   
                AccessPayment();
                break;
            case 1: // 방어(미구현)              
                ChangeTurn();
                break;
            case 2: // 무인도 탈출           
                isRest = true;
                ChangeTurn();
                break;
            case 3: // 할인 쿠폰            
                AccessPayment();
                break;
            default:
                break;
        }

        CardUseWindow.transform.Find("ConfirmBtn").GetComponent<Button>().onClick.AddListener(() => {
            CardUseWindow.gameObject.SetActive(false);
        });

        CardUseWindow.transform.Find("CancelBtn").GetComponent<Button>().onClick.AddListener(() => {
            CardUseWindow.gameObject.SetActive(false);
        });
    }
    #endregion

    #region 폭포수
    void waterfall()
    {
        int x, z, r;
        int StartLine = UnityEngine.Random.Range(0, 4);
        bool isX, isZ, isDir;

        PV.RPC("RPCFallWaterSound", RpcTarget.AllBufferedViaServer);

        if (StartLine == 0) // x가 0 // z축 상승
        {
            x = 0;
            z = UnityEngine.Random.Range(0, 10) * 2;
            r = 0;
            isX = false;
            isZ = true;
            isDir = true;
        }
        else if (StartLine == 1) // x축 상승 //z가 20 
        {
            x = UnityEngine.Random.Range(0, 10) * 2;
            z = 20;
            r = 90;
            isX = true;
            isZ = false;
            isDir = true;
        }
        else if (StartLine == 2) // x가 20 // z축 감소
        {
            z = UnityEngine.Random.Range(1, 11) * 2;
            x = 20;
            r = 180;
            isX = false;
            isZ = true;
            isDir = false;
        }
        else // x축 감소 // z가 0
        {
            z = 0;
            x = UnityEngine.Random.Range(1, 11) * 2;
            r = 270;
            isX = true;
            isZ = false;
            isDir = false;
        }

        player.GetComponent<PlayerCharacterController>().isX = isX;
        player.GetComponent<PlayerCharacterController>().isZ = isZ;
        player.GetComponent<PlayerCharacterController>().isDir = isDir;
        player.GetComponent<PlayerCharacterController>().isTel = true;
        player.GetComponent<PlayerCharacterController>().moveDistance = 2;
        player.transform.position = new Vector3(x, 5, z);
        player.GetComponent<PlayerCharacterController>().Turn(r);
    }

    #endregion

    #region 보물창고
    void TreasureHouse()
    {
        PV.RPC("RPCSetTime", RpcTarget.AllViaServer, 30.0f);
        if (player.GetComponent<PlayerCharacterController>().havingMoney >= 500000)
        {
            PV.RPC("ActiveWindow", RpcTarget.All, int.Parse(player.name));
            TreasureWindow.GetComponent<PhotonView>().RequestOwnership();
            baetingcount = 0;
            nowState = "TreasureHouseState";
            isgambling = false;
            for (int i = 0; i < 3; i++)
                starAnim[i].SetBool("ActiveStar", false);
            InitTreasure();
        }
        else
        {
            ChangeTurn();
        }
    }

    public void GameCost(int cost)
    {
        if (TreasureWindow.GetComponent<PhotonView>().IsMine)
        {
            gameCost = cost * 10000;
            PV.RPC("RPCGameCost", RpcTarget.AllViaServer, cost * RewardRatio);
        }
    }

    [PunRPC]
    void RPCGameCost(int cost)
    {
        TreasureWindow.transform.Find("GameCost").Find("ObtainMoney").Find("Text").GetComponent<Text>().text = cost.ToString() + "만원";
    }

    public void execTreasure()// 유니티에서 버튼 호출
    {
        if (currentReward != 0 && TreasureWindow.GetComponent<PhotonView>().IsMine)
            player.GetComponent<PlayerCharacterController>().htMoneyCaculation(currentReward);
        PV.RPC("RPCCloseWindow", RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    void RPCCloseWindow()
    {
        TreasureWindow.SetActive(false);
        ChangeTurn();
    }

    [PunRPC]
    void ActiveWindow(int _player)
    {
        currentReward = 0;
        RewardRatio = 2;
        totalRewardMoney = 0;
        GameObject playerInfo = GameObject.Find("Canvas").transform.Find("PlayerInfoDivision").Find(_player.ToString()).gameObject;

        //왼쪽
        TreasureWindow.transform.Find("PlayerState").Find("Title").GetComponent<Text>().text = playerInfo.transform.Find("PlayerNickName").Find("Text").GetComponent<Text>().text;
        TreasureWindow.transform.Find("PlayerState").Find("Image").GetComponent<Image>().sprite = playerInfo.transform.Find("CharacterImage").Find("Image").GetComponent<Image>().sprite;
        TreasureWindow.transform.Find("PlayerState").Find("CurrentReward").Find("Text").GetComponent<Text>().text = "50만원";

        // 중앙
        for (int i = 0; i < 4; i++)
        {
            TreasureWindow.transform.Find("Treasure").Find("Treasure" + i).GetComponent<Button>().onClick.RemoveAllListeners();
        }

        //오른쪽

        TreasureWindow.transform.Find("CloseButton").gameObject.SetActive(false);

        if (player.GetComponent<PlayerCharacterController>().havingMoney > 1000000)
        {
            TreasureWindow.transform.Find("GameCost").Find("ToggleGroup").Find("100Toggle").GetComponent<Toggle>().interactable = true;
        }
        else
        {
            TreasureWindow.transform.Find("GameCost").Find("ToggleGroup").Find("100Toggle").GetComponent<Toggle>().interactable = false;
        }

        if (player.GetComponent<PlayerCharacterController>().havingMoney > 1500000)
        {
            TreasureWindow.transform.Find("GameCost").Find("ToggleGroup").Find("150Toggle").GetComponent<Toggle>().interactable = true;
        }
        else
        {
            TreasureWindow.transform.Find("GameCost").Find("ToggleGroup").Find("150Toggle").GetComponent<Toggle>().interactable = false;
        }



        TreasureWindow.transform.Find("GameCost").Find("RewardRatio").Find("Text").GetComponent<Text>().text = "x" + RewardRatio.ToString();
        TreasureWindow.transform.Find("GameCost").Find("ObtainMoney").Find("Text").GetComponent<Text>().text = "100만원";

        TreasureWindow.SetActive(true);
    }

    void InitTreasure()
    {
        PV.RPC("RPCSetTime", RpcTarget.AllViaServer, 30.0f);
        int t = UnityEngine.Random.Range(0, 4);
        TreasureWindow.transform.Find("CloseButton").gameObject.SetActive(true);

        for (int i = 0; i < 4; i++)
        {
            if (TreasureWindow.transform.Find("Treasure").Find("Treasure" + i).name == "Treasure" + t)
            {
                int index = i;
                TreasureWindow.transform.Find("Treasure").Find("Treasure" + i).GetComponent<Button>().onClick.AddListener(() => {
                    if (!isgambling)
                    {
                        isgambling = true;

                        TreasureWindow.transform.Find("GameCost").Find("ToggleGroup").Find("50Toggle").GetComponent<Toggle>().interactable = false;
                        TreasureWindow.transform.Find("GameCost").Find("ToggleGroup").Find("100Toggle").GetComponent<Toggle>().interactable = false;
                        TreasureWindow.transform.Find("GameCost").Find("ToggleGroup").Find("150Toggle").GetComponent<Toggle>().interactable = false;

                        for (int j = 0; j < 3; j++)
                        {
                            if (TreasureWindow.transform.Find("GameCost").Find("ToggleGroup").GetChild(j).GetComponent<Toggle>().isOn)
                            {
                                switch (j)
                                {
                                    case 0:
                                        gameCost = 1500000;
                                        PV.RPC("RPCupdateRewardCost", RpcTarget.All, gameCost);
                                        break;
                                    case 1:
                                        gameCost = 1000000;
                                        PV.RPC("RPCupdateRewardCost", RpcTarget.All, gameCost);
                                        break;
                                    case 2:
                                        gameCost = 500000;
                                        PV.RPC("RPCupdateRewardCost", RpcTarget.All, gameCost);
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            TreasureWindow.transform.Find("Treasure").Find("Treasure" + j).GetComponent<Button>().onClick.RemoveAllListeners();
                        }

                        PV.RPC("RPCupdateRewardCost", RpcTarget.All, currentReward);
                    }
                    TreasureWindow.transform.Find("CloseButton").gameObject.SetActive(false);
                    StartCoroutine("FindTreasure", index);
                });
            }
            else
            {
                int index = i;
                TreasureWindow.transform.Find("Treasure").Find("Treasure" + i).GetComponent<Button>().onClick.AddListener(() => {
                    TreasureWindow.transform.Find("CloseButton").gameObject.SetActive(false);
                    StartCoroutine("FailFindTreasure", index);
                });
            }
        }
    }

    [PunRPC]
    void RPCupdateRewardCost(int beforeCurrentReward)
    {
        currentReward = beforeCurrentReward * RewardRatio; //현재 받을수 있는 금액
        RewardRatio *= 2; // 보상 배율
        totalRewardMoney = currentReward * RewardRatio; // 다음 배팅후 받을수 있는금액

        TreasureWindow.transform.Find("PlayerState").Find("CurrentReward").Find("Text").GetComponent<Text>().text = currentReward.ToString().Substring(0, 3) + "만원";
        TreasureWindow.transform.Find("GameCost").Find("RewardRatio").Find("Text").GetComponent<Text>().text = "x" + RewardRatio.ToString();
        TreasureWindow.transform.Find("GameCost").Find("ObtainMoney").Find("Text").GetComponent<Text>().text = totalRewardMoney.ToString().Substring(0, 3) + "만원";

        TreasureWindow.transform.Find("PlayerState").Find("CurrentProcess").GetChild(baetingcount).GetComponent<Image>().sprite = Resources.Load<Sprite>("GUIImage/Icon_Star1");
    }

    IEnumerator FindTreasure(int i)
    {
        SoundManager.instance.SFXPlay("성공", SuccessClip);
        PV.RPC("RPCSetTime", RpcTarget.AllBufferedViaServer, 7.0f);
        contentAnim[i].SetBool("isTreasure", true);
        treasureAnim[i].SetBool("OpenTreasure", true);
        yield return new WaitForSeconds(3f);
        treasureAnim[i].SetBool("OpenTreasure", false);
        contentAnim[i].SetBool("isTreasure", false);

        yield return new WaitForSeconds(1f);
        //배팅카운터 확인후 초과시 종료 아님 초기화
        starAnim[baetingcount++].SetBool("ActiveStar", true);
        if (baetingcount == 3)
        {
            player.GetComponent<PlayerCharacterController>().htMoneyCaculation(currentReward);
            PV.RPC("RPCCloseWindow", RpcTarget.AllViaServer);
        }
        else
        {
            InitTreasure();
        }
    }


    IEnumerator FailFindTreasure(int i)
    {
        SoundManager.instance.SFXPlay("실패", FailClip);
        PV.RPC("RPCSetTime", RpcTarget.AllViaServer, 7.0f);
        treasureAnim[i].SetBool("OpenTreasure", true);
        yield return new WaitForSeconds(3f);
        treasureAnim[i].SetBool("OpenTreasure", false);

        yield return new WaitForSeconds(1f);
        //창닫고 돈까고 턴 교체
        player.GetComponent<PlayerCharacterController>().htMoneyCaculation(-gameCost);
        PV.RPC("RPCCloseWindow", RpcTarget.AllViaServer);
        ChangeTurn();
    }

    #endregion

    #endregion

    #endregion

    #region 패널 영역
    public void GroundInfo(GameObject _panel, GameObject _player, int laps)
    {
        this.panel = _panel;
        this.player = _player;
        this.laps = laps;

        this.pSC = _panel.GetComponent<PanelSettingController>();

        SoundManager.instance.SFXPlay("WindowActive", WindowActiveClip);

        // 버튼 초기화
        GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag0").Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag1").Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag2").Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag3").Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Purchase").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Close").GetComponent<Button>().onClick.RemoveAllListeners();
        for (int i = 0; i < 4; i++) // 깃발 버튼 초기화
        {
            // 버튼 기본 세팅
            GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag" + i.ToString()).Find("Button").GetComponent<Button>().interactable = true;
            GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag" + i.ToString()).Find("Value").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag" + i.ToString()).Find("Toggle").GetComponent<Toggle>().isOn = false;
            GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag" + i.ToString()).Find("Value").Find("Text").GetComponent<Text>().text = pSC.pI.flagValue[i].ToString();

            // 버튼 활성화 여부
            if (laps == 0) // 첫바퀴 돌지 않을경우 
            {
                if (i < 2) // 깃발 1,2 활성화
                {
                    GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag" + i.ToString()).gameObject.SetActive(true);
                }
                else
                {
                    GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag" + i.ToString()).gameObject.SetActive(false);
                }
            }
            else // 첫바퀴를 돈 경우
            {
                if (i < 3) // 깃발 1,2,3 활성화
                {
                    GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag" + i.ToString()).gameObject.SetActive(true);
                }
                else
                {
                    GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag" + i.ToString()).gameObject.SetActive(false);
                }
            }
        }

        if (pSC.pI.owner == "") // 주인 없는 패널
        {
            if (player.GetComponent<PlayerCharacterController>().havingMoney >= pSC.pI.price)
            {
                PV.RPC("RPCSetTime", RpcTarget.All, 30.0f);
                nowState = "PurchasePanelState";
                GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Image").GetComponent<Image>().sprite = pSC.pI.image;
                GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Name").GetComponent<Text>().text = pSC.pI.name;
                GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Value").GetComponent<Text>().text = pSC.pI.price.ToString();
                if (pSC.pI.name.Substring(0, 2) == "유물")
                {
                    for (int i = 0; i < 3; i++)
                        GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag" + i.ToString()).gameObject.SetActive(false);
                }

                GameObject.Find("Canvas").transform.Find("PurchaseWindow").gameObject.SetActive(true);
            }
            else
            {
                CloseWindow(GameObject.Find("Canvas").transform.Find("PurchaseWindow").gameObject);
            }
            GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag0").Find("Button").GetComponent<Button>().onClick.AddListener(() => { FlagisOn(0); });
            GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag1").Find("Button").GetComponent<Button>().onClick.AddListener(() => { FlagisOn(1); });
            GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag2").Find("Button").GetComponent<Button>().onClick.AddListener(() => { FlagisOn(2); });
            GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag3").Find("Button").GetComponent<Button>().onClick.AddListener(() => { FlagisOn(3); });

            GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Purchase").GetComponent<Button>().onClick.AddListener(() => { purchasePanel(); });
            GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Close").GetComponent<Button>().onClick.AddListener(() => { CloseWindow(GameObject.Find("Canvas").transform.Find("PurchaseWindow").gameObject); });
        }
        else if (pSC.pI.owner == player.name) // 주인일때
        {
            if (pSC.pI.name.Substring(0, 2) == "유물")
            {
                ChangeTurn();
            }
            else
            {
                bool[] actvieFlag = new bool[4] { false, false, false, false };
                // 깃발 구매여부 확인
                for (int i = 0; i < 4; i++)
                {
                    if (panel.transform.Find("Flag").GetChild(i).gameObject.activeInHierarchy == true)
                    {
                        actvieFlag[i] = true;
                    }
                }
                //랜드마크 건설 안했을시
                if (actvieFlag[3] == false)
                {
                    PV.RPC("RPCSetTime", RpcTarget.All, 30.0f);
                    nowState = "PurchasePanelState";

                    if (player.GetComponent<PlayerCharacterController>().havingMoney >= pSC.pI.flagValue[0] && actvieFlag[0] == false ||
                        player.GetComponent<PlayerCharacterController>().havingMoney >= pSC.pI.flagValue[1] && actvieFlag[1] == false ||
                        player.GetComponent<PlayerCharacterController>().havingMoney >= pSC.pI.flagValue[2] && actvieFlag[2] == false && laps != 0 ||
                        player.GetComponent<PlayerCharacterController>().havingMoney >= pSC.pI.flagValue[3])
                    {

                        GameObject.Find("Canvas").transform.Find("PurchaseWindow").gameObject.SetActive(true);
                        GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Image").GetComponent<Image>().sprite = pSC.pI.image;
                        GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Name").GetComponent<Text>().text = pSC.pI.name;
                        GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Value").GetComponent<Text>().text = "0";

                        int count = 0;
                        for (int i = 0; i < 3; i++)
                        {
                            if (actvieFlag[i] == true)
                            {
                                count++;
                            }
                        }
                        if (count == 3) // 3개 깃발 구매 햇을 시
                        {
                            GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag3").gameObject.SetActive(true);
                            //GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Value").GetComponent<Text>().text = pSC.pI.flagValue[3].ToString();
                        }
                        else // 2개  이하 깃발 구매시
                        {
                            //사버린 깃발은 비활성화 시키기
                            for (int i = 0; i < 3; i++)
                            {
                                if (actvieFlag[i] == true)
                                {
                                    GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag" + i.ToString()).Find("Button").GetComponent<Button>().interactable = false;
                                    GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag" + i.ToString()).Find("Value").gameObject.SetActive(false);
                                    GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag" + i.ToString()).Find("Toggle").GetComponent<Toggle>().isOn = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        CloseWindow(GameObject.Find("Canvas").transform.Find("PurchaseWindow").gameObject);
                    }
                }
                else // 랜드마크 건설햇을 시
                {
                    CloseWindow(GameObject.Find("Canvas").transform.Find("PurchaseWindow").gameObject);
                }

                GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag0").Find("Button").GetComponent<Button>().onClick.AddListener(() => { FlagisOn(0); });
                GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag1").Find("Button").GetComponent<Button>().onClick.AddListener(() => { FlagisOn(1); });
                GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag2").Find("Button").GetComponent<Button>().onClick.AddListener(() => { FlagisOn(2); });
                GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag3").Find("Button").GetComponent<Button>().onClick.AddListener(() => { FlagisOn(3); });

                GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Purchase").GetComponent<Button>().onClick.AddListener(() => { purchasePanel(); });
                GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Close").GetComponent<Button>().onClick.AddListener(() => { CloseWindow(GameObject.Find("Canvas").transform.Find("PurchaseWindow").gameObject); });
            }
        }
        else if (pSC.pI.owner != player.name) //주인이 아닐시
        {
            if (player.GetComponent<PlayerCharacterController>().MagicCard == 0 || player.GetComponent<PlayerCharacterController>().MagicCard == 3)
            {
                ShowCardInfo();
            }
            else
            {
                AccessPayment();
            }
        }
    }

    #region 비용지출 and 인수영역
    public void AccessPayment()
    {
        //PanelSettingController pSC = panel.GetComponent<PanelSettingController>();
        //땅값 계산
        int money = 0, LegacyMoney = 0;
        bool isLandMarkFlag = false;

        money += pSC.pI.price;

        if (panel.GetComponent<PanelSettingController>().pI.name.Substring(0, 2) != "유물")
        {
            for (int i = 0; i < 4; i++)
            {
                if (panel.transform.Find("Flag").GetChild(i).gameObject.activeInHierarchy == true)
                {
                    money += pSC.pI.flagValue[i];
                }
                if (i == 3 && panel.transform.Find("Flag").GetChild(i).gameObject.activeInHierarchy == true)
                    isLandMarkFlag = true;
            }           
        }

        LegacyMoney = money;

        if (pSC.pI.festiver)
            money *= festival;

        money -= (int)(money * 0.5); //50% 감면
        Debug.Log("50% 할인된 비용 지불 : " + money);

        if (isSale)
        {
            isSale = false;
            money /= 2;
        }

        if(player.GetComponent<PlayerCharacterController>().MagicCard == 4) // 바가지 카드
        {
            useCard();
            money *= 2;
        }             

        if (player.GetComponent<PlayerCharacterController>().havingMoney >= money)
        {
            Payment(money);

            if (player.GetComponent<PlayerCharacterController>().havingMoney >= LegacyMoney * 2 && pSC.pI.name.Substring(0, 2) != "유물" && isLandMarkFlag == false)
            {
                TakeOverInfo(LegacyMoney * 2); // 인수창 띄움
            }
            else // 비용 지불하고 땅 살 돈이 없다
            {
                ChangeTurn();
            }
        }
        else // 돈없으면?
        {
            nowState = "BankRuptcySelectState";
            SoundManager.instance.SFXPlay("대박", GreatClip);
            if (playerCharacter.GetComponent<PlayerCharacterController>().isloan == false)
            {
                PV.RPC("RPCSetTime", RpcTarget.All, 30.0f);
                GameObject.Find("Canvas").transform.Find("BankRuptcyWindow").gameObject.SetActive(true);
            }
            else
            {
                BankRuptcySelect(1);
            }
        }

    }

    public void BankRuptcySelect(int i) // 유니티 버튼 참조
    {
        //땅값 계산
        int money = 0;

        money += pSC.pI.price;

        if (panel.GetComponent<PanelSettingController>().pI.name.Substring(0, 2) != "유물")
            for (int j = 0; j < 4; j++)
            {
                if (panel.transform.Find("Flag").GetChild(j).gameObject.activeInHierarchy == true)
                {
                    money += pSC.pI.flagValue[j];
                }
            }

        if (pSC.pI.festiver)
            money *= festival;

        money -= (int)(money * 0.5);

        if (isSale)
        {
            isSale = false;
            money /= 2;
        }

        if (i == 0) // 대출
        {
            playerCharacter.GetComponent<PlayerCharacterController>().isloan = true;
            GameObject.Find("Canvas").transform.Find("BankRuptcyWindow").gameObject.SetActive(false);

            playerCharacter.GetComponent<PlayerCharacterController>().totalMoney -= playerCharacter.GetComponent<PlayerCharacterController>().havingMoney;
            playerCharacter.GetComponent<PlayerCharacterController>().havingMoney = 0;

            playerCharacter.GetComponent<PlayerCharacterController>().htMoneyCaculation(money);
            Payment(money);

            ChangeTurn();
        }
        else // 파산
        {
            playerCharacter.GetComponent<PlayerCharacterController>().isbankRuptcy = true;
            GameObject.Find("Canvas").transform.Find("BankRuptcyWindow").gameObject.SetActive(false);

            if (playerCharacter.GetComponent<PlayerCharacterController>().totalMoney >= money)
            {
                Payment(money);
            }
            else
            {
                Payment(playerCharacter.GetComponent<PlayerCharacterController>().totalMoney);
            }
            playerCharacter.GetComponent<PlayerCharacterController>().havingMoney = 0;
            playerCharacter.GetComponent<PlayerCharacterController>().totalMoney = 0;

            playerCharacter.transform.position = new Vector3(-10, 1, -10);
            PV.RPC("RPCResetOwner", RpcTarget.AllBufferedViaServer, player.name);
            PV.RPC("RPCChangeTurn", RpcTarget.AllBufferedViaServer);
        }
    }

    void TakeOverInfo(int TakeOverValue)
    {

        GameObject.Find("Canvas").transform.Find("TakeOverWindow").Find("Purchase").GetComponent<Button>().onClick.RemoveAllListeners();
        GameObject.Find("Canvas").transform.Find("TakeOverWindow").Find("Close").GetComponent<Button>().onClick.RemoveAllListeners();

        //PanelSettingController pSC = panel.GetComponent<PanelSettingController>();
        PV.RPC("RPCSetTime", RpcTarget.All, 30.0f);
        nowState = "TakeOverInfo";

        GameObject.Find("Canvas").transform.Find("TakeOverWindow").gameObject.SetActive(true);

        GameObject.Find("Canvas").transform.Find("TakeOverWindow").Find("Image").GetComponent<Image>().sprite = pSC.pI.image;
        GameObject.Find("Canvas").transform.Find("TakeOverWindow").Find("Name").GetComponent<Text>().text = pSC.pI.name;
        GameObject.Find("Canvas").transform.Find("TakeOverWindow").Find("Value").GetComponent<Text>().text = TakeOverValue.ToString();
        GameObject.Find("Canvas").transform.Find("TakeOverWindow").Find("Owner").GetComponent<Text>().text = "소유자\n" + pSC.pI.owner.ToString() + "번 플레이어";

        GameObject.Find("Canvas").transform.Find("TakeOverWindow").Find("Purchase").GetComponent<Button>().onClick.AddListener(() => { Payment(TakeOverValue); });
        GameObject.Find("Canvas").transform.Find("TakeOverWindow").Find("Purchase").GetComponent<Button>().onClick.AddListener(() => { ChangeMasterPanel(); });
        GameObject.Find("Canvas").transform.Find("TakeOverWindow").Find("Purchase").GetComponent<Button>().onClick.AddListener(() => { GroundInfo(panel, player, laps); });
        GameObject.Find("Canvas").transform.Find("TakeOverWindow").Find("Purchase").GetComponent<Button>().onClick.AddListener(() => { GameObject.Find("Canvas").transform.Find("TakeOverWindow").gameObject.SetActive(false); });

        GameObject.Find("Canvas").transform.Find("TakeOverWindow").Find("Close").GetComponent<Button>().onClick.AddListener(() => { CloseWindow(GameObject.Find("Canvas").transform.Find("TakeOverWindow").gameObject); });
    }

    void Payment(int money)
    {
        player.GetComponent<PlayerCharacterController>().htMoneyCaculation(-money);
        for (int i = 0; i < GameObject.Find("GamePlayerGroup").transform.childCount; i++)
        {
            if (GameObject.Find("GamePlayerGroup").transform.GetChild(i).name == pSC.pI.owner) //패널주인 돈 주기
            {
                PV.RPC("RPCpayment", RpcTarget.AllBufferedViaServer, GameObject.Find("GamePlayerGroup").transform.GetChild(i).name, money);
            }
        }
    }

    [PunRPC]
    void RPCpayment(string targetPlayer, int money)
    {
        if (GameObject.Find("GamePlayerGroup").transform.Find(targetPlayer).GetComponent<PhotonView>().IsMine)
        {
            GameObject.Find("GamePlayerGroup").transform.Find(targetPlayer).GetComponent<PlayerCharacterController>().htMoneyCaculation(money);
        }
    }

    void ChangeMasterPanel()
    {
        PanelSettingController pSC = panel.GetComponent<PanelSettingController>();

        bool[] isCheck = new bool[4] { false, false, false, false };

        ChangeOwner();

        for (int i = 0; i < 4; i++)
        {
            // 활성화된 깃발 확인
            if (panel.transform.Find("Flag").GetChild(i).gameObject.activeInHierarchy == true)
            {
                isCheck[i] = true;
            }
        }

        PV.RPC("RPCFlagSet", RpcTarget.All, int.Parse(panel.name), isCheck, int.Parse(player.name));
    }
    #endregion

    void FlagisOn(int i) // 깃팔 추가시 가격 추가
    {
        GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag" + i.ToString()).Find("Toggle").GetComponent<Toggle>().isOn = !GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag" + i.ToString()).Find("Toggle").GetComponent<Toggle>().isOn;
        if (GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag" + i.ToString()).Find("Toggle").GetComponent<Toggle>().isOn == true)
        {
            GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Value").GetComponent<Text>().text = (int.Parse(GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Value").GetComponent<Text>().text) + pSC.pI.flagValue[i]).ToString();
        }
        else
        {
            GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Value").GetComponent<Text>().text = (int.Parse(GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Value").GetComponent<Text>().text) - pSC.pI.flagValue[i]).ToString();
        }

    }

    //땅이나 깃발 추가 구매
    void purchasePanel()
    {
        if (int.Parse(GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Value").GetComponent<Text>().text) <= player.GetComponent<PlayerCharacterController>().havingMoney)
        {
            // 돈 깍고 패널 머니 동기화
            player.GetComponent<PlayerCharacterController>().havingMoneyCaculation(-int.Parse(GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Value").GetComponent<Text>().text));
            // 깃발 구매 여부 체크 // 깃발 오브젝트 활성화
            bool[] isCheck = new bool[4] { false, false, false, false };
            // 구매 한 땅,깃발 총 합 계산
            int totalGroundMoney = pSC.pI.price;

            if (panel.GetComponent<PanelSettingController>().pI.name.Substring(0, 2) != "유물")
                for (int i = 0; i < 4; i++)
                {
                    // 활성화된 깃발 확인
                    if (panel.transform.Find("Flag").GetChild(i).gameObject.activeInHierarchy == true)
                    {
                        isCheck[i] = true;
                        totalGroundMoney += pSC.pI.flagValue[i];
                    }
                    // 깃발 체크한경우 깃발구매 했다는 소리 // 미리산것들은 true이므로 체크할필요 x
                    else if (GameObject.Find("Canvas").transform.Find("PurchaseWindow").Find("Flag" + i.ToString()).Find("Toggle").GetComponent<Toggle>().isOn == true)
                    {
                        isCheck[i] = true;
                        totalGroundMoney += pSC.pI.flagValue[i];
                    }
                }

            if (pSC.pI.festiver)
                totalGroundMoney *= festival;

            if (panel.GetComponent<PanelSettingController>().pI.owner != player.name)
                ChangeOwner();

            PV.RPC("RPCFlagSet", RpcTarget.All, int.Parse(panel.name), isCheck, int.Parse(player.name));
            //PV.RPC("RPCSetPanelValue", RpcTarget.All, int.Parse(panel.name), pSC.pI.name + "\n" + ((totalGroundMoney / 10000)/2) + "만원");

            if (((totalGroundMoney / 2) / 1000).ToString().Substring(((totalGroundMoney / 2) / 1000).ToString().Length - 1, 1) != "0")
                PV.RPC("RPCSetPanelValue", RpcTarget.All, int.Parse(panel.name), pSC.pI.name + "\n" + ((totalGroundMoney / 10000) / 2) + "만" + (totalGroundMoney / 2).ToString().Substring((totalGroundMoney / 2).ToString().Length - 4, 4));
            else
                PV.RPC("RPCSetPanelValue", RpcTarget.All, int.Parse(panel.name), pSC.pI.name + "\n" + ((totalGroundMoney / 10000) / 2) + "만");

            EpicVictoryCondition(int.Parse(player.name));
            CloseWindow(GameObject.Find("Canvas").transform.Find("PurchaseWindow").gameObject);
        }
        else
        {
            // 금액 초과 창 띄우기
            GameObject.Find("Canvas").transform.Find("WarningWindow").gameObject.SetActive(true);
        }
    }

    [PunRPC]
    void RPCSetPanelValue(int _panelNumber, string value)
    {
        GameObject Line;
        if (_panelNumber < 10)
        {
            Line = GameObject.Find("Map").transform.GetChild(0).gameObject;
        }
        else if (_panelNumber < 20)
        {
            Line = GameObject.Find("Map").transform.GetChild(1).gameObject;
        }
        else if (_panelNumber < 30)
        {
            Line = GameObject.Find("Map").transform.GetChild(2).gameObject;
        }
        else
        {
            Line = GameObject.Find("Map").transform.GetChild(3).gameObject;
        }

        //Line.transform.Find(_panelNumber.ToString()).Find("Content").GetComponent<TextMesh>().text = value + "만원";
        Line.transform.Find(_panelNumber.ToString()).Find("Content").GetComponent<TextMesh>().text = value;
    }

    [PunRPC]
    void RPCFlagSet(int _panelNumber, bool[] isBool, int _player)
    {
        GameObject Line;
        if (_panelNumber < 10)
        {
            Line = GameObject.Find("Map").transform.GetChild(0).gameObject;
        }
        else if (_panelNumber < 20)
        {
            Line = GameObject.Find("Map").transform.GetChild(1).gameObject;
        }
        else if (_panelNumber < 30)
        {
            Line = GameObject.Find("Map").transform.GetChild(2).gameObject;
        }
        else
        {
            Line = GameObject.Find("Map").transform.GetChild(3).gameObject;
        }

        for (int i = 0; i < 4; i++)
        {
            if (isBool[i] == true)
            {
                switch (_player)
                {
                    case 1:
                        Line.transform.Find(_panelNumber.ToString()).Find("Flag").Find("Flag" + i.ToString()).GetChild(0).GetComponent<MeshRenderer>().material.color = Color.blue;
                        break;
                    case 2:
                        Line.transform.Find(_panelNumber.ToString()).Find("Flag").Find("Flag" + i.ToString()).GetChild(0).GetComponent<MeshRenderer>().material.color = Color.yellow;
                        break;
                    case 3:
                        Line.transform.Find(_panelNumber.ToString()).Find("Flag").Find("Flag" + i.ToString()).GetChild(0).GetComponent<MeshRenderer>().material.color = Color.grey;
                        break;
                    case 4:
                        Line.transform.Find(_panelNumber.ToString()).Find("Flag").Find("Flag" + i.ToString()).GetChild(0).GetComponent<MeshRenderer>().material.color = Color.red;
                        break;
                }
                Line.transform.Find(_panelNumber.ToString()).Find("Flag").Find("Flag" + i.ToString()).transform.gameObject.SetActive(true);
            }
        }
    }

    void ChangeOwner()
    {
        panel.GetComponent<PhotonView>().RequestOwnership();
        PV.RPC("RPCChangeOwner", RpcTarget.All, int.Parse(panel.name), int.Parse(player.name));
    }

    [PunRPC]
    void RPCResetOwner(string _player)
    {
        GameObject.Find("GamePlayerGroup").transform.Find(_player).GetComponent<PlayerCharacterController>().playerInfo.transform.Find("BankRuptcy").gameObject.SetActive(true);
        GameObject resetGround;
        for (int i = 0; i < GameObject.Find("Map").transform.childCount; i++)
        {
            for (int j = 0; j < GameObject.Find("Map").transform.GetChild(i).childCount; j++)
            {
                if (GameObject.Find("Map").transform.GetChild(i).GetChild(j).GetComponent<PanelSettingController>().pI.owner == _player)
                {
                    resetGround = GameObject.Find("Map").transform.GetChild(i).GetChild(j).gameObject;
                    resetGround.GetComponent<PanelSettingController>().pI.owner = "";
                    resetGround.transform.Find("Content").GetComponent<TextMesh>().color = Color.white;
                    resetGround.transform.Find("Content").GetComponent<TextMesh>().text = resetGround.GetComponent<PanelSettingController>().pI.name + "\n" + resetGround.GetComponent<PanelSettingController>().pI.price + "만원";

                    if (resetGround.GetComponent<PanelSettingController>().pI.name.Substring(0, 2) != "유물")
                        for (int g = 0; g < resetGround.transform.Find("Flag").childCount; g++)
                        {
                            resetGround.transform.Find("Flag").GetChild(g).gameObject.SetActive(false);
                        }
                }
            }
        }
    }

    [PunRPC]
    void RPCChangeOwner(int _panelNumber, int _player)
    {
        GameObject Line;
        if (_panelNumber < 10)
        {
            Line = GameObject.Find("Map").transform.GetChild(0).gameObject;
        }
        else if (_panelNumber < 20)
        {
            Line = GameObject.Find("Map").transform.GetChild(1).gameObject;
        }
        else if (_panelNumber < 30)
        {
            Line = GameObject.Find("Map").transform.GetChild(2).gameObject;
        }
        else
        {
            Line = GameObject.Find("Map").transform.GetChild(3).gameObject;
        }

        switch (_player)
        {
            case 1:
                Line.transform.Find(_panelNumber.ToString()).Find("Content").GetComponent<TextMesh>().color = Color.blue;
                break;
            case 2:
                Line.transform.Find(_panelNumber.ToString()).Find("Content").GetComponent<TextMesh>().color = Color.yellow;
                break;
            case 3:
                Line.transform.Find(_panelNumber.ToString()).Find("Content").GetComponent<TextMesh>().color = Color.grey;
                break;
            case 4:
                Line.transform.Find(_panelNumber.ToString()).Find("Content").GetComponent<TextMesh>().color = Color.red;
                break;
        }

        Line.transform.Find(_panelNumber.ToString()).GetComponent<PanelSettingController>().pI.owner = _player.ToString();
    }

    #endregion

    #region 창닫기 and 턴 교체
    public void CloseWindow(GameObject go)
    {
        if (go != null)
            go.SetActive(false);
        ChangeTurn();
    }

    void ChangeTurn()
    {
        PV.RPC("RPCSetTime", RpcTarget.AllBufferedViaServer, 30.0f);

        if (dice[0] == dice[1])
        {
            if (isRest == false && isTravel == false)
            {
                nowState = "DiceState";
                GameObject.Find("Canvas").transform.Find("DiceRollButton").gameObject.SetActive(true);
                GameObject.Find("Canvas").transform.Find("DiceRollButton").GetComponent<Button>().interactable = true;

                GameObject.Find("Canvas").transform.Find("OddEvenToggleGroup").gameObject.SetActive(true);
                GameObject.Find("Canvas").transform.Find("OddEvenToggleGroup").GetChild(0).GetComponent<Toggle>().interactable = true;
                GameObject.Find("Canvas").transform.Find("OddEvenToggleGroup").GetChild(1).GetComponent<Toggle>().interactable = true;
            }
            else
            {
                doubleCount = 0;
                PV.RPC("RPCChangeTurn", RpcTarget.AllBufferedViaServer);
            }
        }
        else
        {
            doubleCount = 0;
            PV.RPC("RPCChangeTurn", RpcTarget.AllBufferedViaServer);
        }
    }

    [PunRPC]
    void RPCChangeTurn()
    {
        rpcCount = 0;
        Debug.Log("턴 교체");
        PlayerTurn++;

        if (PlayerTurn < 5)
            for (int i = 0; i < 2; i++)
                if (GameObject.Find("GamePlayerGroup").transform.Find(PlayerTurn.ToString()) == false)
                {
                    PlayerTurn++;
                }
                else
                {
                    break;
                }

        int bankRuptcyCount = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (GameObject.Find("GamePlayerGroup").transform.GetChild(i).GetComponent<PlayerCharacterController>().isbankRuptcy)
            {
                bankRuptcyCount++;
            }
            if (bankRuptcyCount == PhotonNetwork.PlayerList.Length - 1)
            {
                isGame = false;
            }
        }

        if (isGame == false)
        {
            int enemyMoney = 0;
            int mymoney = 0;
            for (int i = 0; i < GameObject.Find("GamePlayerGroup").transform.childCount; i++)
            {
                if (!GameObject.Find("GamePlayerGroup").transform.GetChild(i).GetComponent<PhotonView>().IsMine)
                {
                    if (GameObject.Find("GamePlayerGroup").transform.GetChild(i).GetComponent<PlayerCharacterController>().isbankRuptcy == true)
                        continue;

                    if (GameObject.Find("GamePlayerGroup").transform.GetChild(i).GetComponent<PlayerCharacterController>().totalMoney >= enemyMoney)
                    {
                        enemyMoney = GameObject.Find("GamePlayerGroup").transform.GetChild(i).GetComponent<PlayerCharacterController>().totalMoney;
                    }
                }
                else
                {
                    mymoney = GameObject.Find("GamePlayerGroup").transform.GetChild(i).GetComponent<PlayerCharacterController>().totalMoney;
                }

            }

            if (mymoney >= enemyMoney)
            {
                GameObject.Find("Canvas").transform.Find("VictoryWindow").gameObject.SetActive(true);
                Debug.Log("호출 음악");
                SoundManager.instance.BgSoundPlay(SoundManager.instance.bglist[4]);
            }
            else
            {
                GameObject.Find("Canvas").transform.Find("DefeatWindow").gameObject.SetActive(true);
                Debug.Log("호출 음악");
                SoundManager.instance.BgSoundPlay(SoundManager.instance.bglist[5]);
            }
        }
        else if (PlayerTurn >= 5)
        {
            PlayerTurn = 1;
            for (int i = 0; i < 2; i++)
                if (GameObject.Find("GamePlayerGroup").transform.Find(PlayerTurn.ToString()) == false)
                {
                    PlayerTurn++;
                }
                else
                {
                    break;
                }

            GameRoundText.text = ++GameRound + "턴 / " + NetworkController.turn.ToString() + "턴";
            if (GameRound == NetworkController.turn)
            {
                int enemyMoney = 0;
                int mymoney = 0;
                for (int i = 0; i < GameObject.Find("GamePlayerGroup").transform.childCount; i++)
                {
                    if (!GameObject.Find("GamePlayerGroup").transform.GetChild(i).GetComponent<PhotonView>().IsMine)
                    {
                        if (GameObject.Find("GamePlayerGroup").transform.GetChild(i).GetComponent<PlayerCharacterController>().isbankRuptcy == true)
                            continue;

                        if (GameObject.Find("GamePlayerGroup").transform.GetChild(i).GetComponent<PlayerCharacterController>().totalMoney >= enemyMoney)
                        {
                            enemyMoney = GameObject.Find("GamePlayerGroup").transform.GetChild(i).GetComponent<PlayerCharacterController>().totalMoney;
                        }
                    }
                    else
                    {
                        mymoney = GameObject.Find("GamePlayerGroup").transform.GetChild(i).GetComponent<PlayerCharacterController>().totalMoney;
                    }

                }

                if (mymoney >= enemyMoney)
                {
                    GameObject.Find("Canvas").transform.Find("VictoryWindow").gameObject.SetActive(true);
                    SoundManager.instance.BgSoundPlay(SoundManager.instance.bglist[4]);
                }
                else
                {
                    GameObject.Find("Canvas").transform.Find("DefeatWindow").gameObject.SetActive(true);
                    SoundManager.instance.BgSoundPlay(SoundManager.instance.bglist[5]);
                }
                isGame = false;
            }
        }

        nowState = "DiceState";
        isChangeTurn = false;
    }
    #endregion

    [PunRPC]
    void RPCDiceSound(bool isDouble, int dice)
    {
        StartCoroutine(DiceSound(isDouble, dice));
    }

    IEnumerator DiceSound(bool isDouble, int dice)
    {
        if (isDouble)
        {
            SoundManager.instance.SFXPlay("더블", DoubleClip);
            yield return new WaitForSeconds(0.5f);
        }

        switch (dice)
        {
            case 2:
                SoundManager.instance.SFXPlay("2", DiceResultClip[0]);
                break;
            case 3:
                SoundManager.instance.SFXPlay("3", DiceResultClip[1]);
                break;
            case 4:
                SoundManager.instance.SFXPlay("4", DiceResultClip[2]);
                break;
            case 5:
                SoundManager.instance.SFXPlay("5", DiceResultClip[3]);
                break;
            case 6:
                SoundManager.instance.SFXPlay("6", DiceResultClip[4]);
                break;
            case 7:
                SoundManager.instance.SFXPlay("7", DiceResultClip[5]);
                break;
            case 8:
                SoundManager.instance.SFXPlay("8", DiceResultClip[6]);
                break;
            case 9:
                SoundManager.instance.SFXPlay("9", DiceResultClip[7]);
                break;
            case 10:
                SoundManager.instance.SFXPlay("10", DiceResultClip[8]);
                break;
            case 11:
                SoundManager.instance.SFXPlay("11", DiceResultClip[9]);
                break;
            case 12:
                SoundManager.instance.SFXPlay("12", DiceResultClip[10]);
                break;
        }

    }

    [PunRPC]
    void RPCKartStartSound()
    {
        SoundManager.instance.SFXPlay("KartStartSound", KartStartClip);
    }

    [PunRPC]
    void RPCDiceRollSound()
    {
        SoundManager.instance.SFXPlay("DiceRollSound", DiceRollClip);
    }

    [PunRPC]
    void RPCFallWaterSound()
    {
        SoundManager.instance.SFXPlay("fallWaterClip", fallWaterClip);
    }

    #region 특수 승리 조건

    void EpicVictoryCondition(int player)
    {
        bool check = false;
        int LegacyCount = 0;
        int LineCount = 0;
        int[] GroupCount = new int[12] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int TotalCount = 0;

        foreach (KeyValuePair<string, PanelInfo> pair in gpi.GetComponent<GamePanelInfo>().panelDictionary)
        {
            if (pair.Value.owner == player.ToString())
            {
                if (pair.Value.group == 0) // 유물 독점
                {
                    Debug.Log("유물 출력");
                    if (++LegacyCount == 4)
                    {
                        PV.RPC("EpicGameSet", RpcTarget.AllBufferedViaServer, player);
                        check = true;
                        break;
                    }
                    Debug.Log(LegacyCount);
                }
            }
        }

        if (!check)
            foreach (KeyValuePair<string, PanelInfo> pair in gpi.GetComponent<GamePanelInfo>().panelDictionary)
            {
                if (pair.Value.owner == player.ToString())
                {
                    if (pair.Value.Line == 1) // 라인 승리
                    {
                        if (++LineCount == 8)
                        {
                            PV.RPC("EpicGameSet", RpcTarget.AllBufferedViaServer, player);
                            check = true;
                            break;
                        }
                    }
                }
            }

        if (!check)
            foreach (KeyValuePair<string, PanelInfo> pair in gpi.GetComponent<GamePanelInfo>().panelDictionary)
            {
                if (pair.Value.owner == player.ToString())
                {
                    if (pair.Value.group == 1) // 쿼터플 승리
                    {
                        if (++GroupCount[0] == 2)
                        {
                            if (++TotalCount == 4)
                            {
                                PV.RPC("EpicGameSet", RpcTarget.AllBufferedViaServer, player);
                                check = true;
                                break;
                            }
                        }
                    }

                    if (pair.Value.group == 2) // 쿼터플 승리
                    {
                        if (++GroupCount[1] == 3)
                        {
                            if (++TotalCount == 4)
                            {
                                PV.RPC("EpicGameSet", RpcTarget.AllBufferedViaServer, player);
                                check = true;
                                break;
                            }
                        }
                    }

                    if (pair.Value.group == 3) // 쿼터플 승리
                    {
                        if (++GroupCount[2] == 2)
                        {
                            if (++TotalCount == 4)
                            {
                                PV.RPC("EpicGameSet", RpcTarget.AllBufferedViaServer, player);
                                check = true;
                                break;
                            }
                        }
                    }

                    if (pair.Value.group == 4) // 쿼터플 승리
                    {
                        if (++GroupCount[3] == 2)
                        {
                            if (++TotalCount == 4)
                            {
                                PV.RPC("EpicGameSet", RpcTarget.AllBufferedViaServer, player);
                                check = true;
                                break;
                            }
                        }
                    }

                    if (pair.Value.group == 5) // 쿼터플 승리
                    {
                        if (++GroupCount[4] == 2)
                        {
                            if (++TotalCount == 4)
                            {
                                PV.RPC("EpicGameSet", RpcTarget.AllBufferedViaServer, player);
                                check = true;
                                break;
                            }
                        }
                    }

                    if (pair.Value.group == 6) // 쿼터플 승리
                    {
                        if (++GroupCount[5] == 3)
                        {
                            if (++TotalCount == 4)
                            {
                                PV.RPC("EpicGameSet", RpcTarget.AllBufferedViaServer, player);
                                check = true;
                                break;
                            }
                        }
                    }

                    if (pair.Value.group == 7) // 쿼터플 승리
                    {
                        if (++GroupCount[6] == 2)
                        {
                            if (++TotalCount == 4)
                            {
                                PV.RPC("EpicGameSet", RpcTarget.AllBufferedViaServer, player);
                                check = true;
                                break;
                            }
                        }
                    }

                    if (pair.Value.group == 8) // 쿼터플 승리
                    {
                        if (++GroupCount[7] == 2)
                        {
                            if (++TotalCount == 4)
                            {
                                PV.RPC("EpicGameSet", RpcTarget.AllBufferedViaServer, player);
                                check = true;
                                break;
                            }
                        }
                    }

                    if (pair.Value.group == 9) // 쿼터플 승리
                    {
                        if (++GroupCount[8] == 3)
                        {
                            if (++TotalCount == 4)
                            {
                                PV.RPC("EpicGameSet", RpcTarget.AllBufferedViaServer, player);
                                check = true;
                                break;
                            }
                        }
                    }

                    if (pair.Value.group == 10) // 쿼터플 승리
                    {
                        if (++GroupCount[9] == 2)
                        {
                            if (++TotalCount == 4)
                            {
                                PV.RPC("EpicGameSet", RpcTarget.AllBufferedViaServer, player);
                                check = true;
                                break;
                            }
                        }
                    }

                    if (pair.Value.group == 11) // 쿼터플 승리
                    {
                        if (++GroupCount[10] == 2)
                        {
                            if (++TotalCount == 4)
                            {
                                PV.RPC("EpicGameSet", RpcTarget.AllBufferedViaServer, player);
                                check = true;
                                break;
                            }
                        }
                    }

                    if (pair.Value.group == 12) // 쿼터플 승리
                    {
                        if (++GroupCount[11] == 3)
                        {
                            if (++TotalCount == 4)
                            {
                                PV.RPC("EpicGameSet", RpcTarget.AllBufferedViaServer, player);
                                check = true;
                                break;
                            }
                        }
                    }
                }
            }
    }

    [PunRPC]
    void EpicGameSet(int _player)
    {
        if (GameObject.Find("Canvas").transform.Find("PlayerInfoDivision").Find(_player.ToString()).GetComponent<PhotonView>().IsMine)
        {
            GameObject.Find("Canvas").transform.Find("VictoryWindow").gameObject.SetActive(true);
            SoundManager.instance.BgSoundPlay(SoundManager.instance.bglist[4]);
        }
        else
        {
            GameObject.Find("Canvas").transform.Find("DefeatWindow").gameObject.SetActive(true);
            SoundManager.instance.BgSoundPlay(SoundManager.instance.bglist[5]);
        }
        isGame = false;
    }

    #endregion

    #region 채팅
    public void Send()
    {
        // PhotonNetwork.NickName + " : " + ChatInput.text
        string msg = PhotonNetwork.NickName + " : " + ChatInput.text;
        PV.RPC("ChatRPC", RpcTarget.All, msg);
        ChatInput.text = "";
    }

    [PunRPC]
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
                    ChatText[i - 1].text = ChatText[i].text; //챗 위로 올림
                }
            }
            ChatText[ChatText.Length - 1].text = msg; // 위로 올리고 나면 빈텍스트에 msg넣음
        }
        else
        {
            ChatText[ChatText.Length - 1].text = msg; // 처음 챗을 아무것도 없어서 바로 들감
        }
    }
    #endregion

    #region 연결 끊기

    public void LeaveRoom()
    {
        if (isGame)
        {
            PV.RPC("RPCResetOwner", RpcTarget.OthersBuffered, playerCharacter.name);
            PV.RPC("LeaveRoomCheckState", RpcTarget.OthersBuffered, myturn);
        }
        else if (isGame == false && SequenceAppoint.gameObject.activeSelf == true)
        {
            PV.RPC("LeaveRoomCheckState", RpcTarget.OthersBuffered, myturn);
        }
        PhotonNetwork.LeaveRoom();
    }

    [PunRPC]
    void LeaveRoomCheckState(int myturn)
    {
        if (PhotonNetwork.PlayerList.Length - 1 == 1) // 두명일때 나가면 혼자니까 게임 끝내기
        {
            isGame = false;
            GameObject.Find("Canvas").transform.Find("VictoryWindow").gameObject.SetActive(true);
            SoundManager.instance.BgSoundPlay(SoundManager.instance.bglist[4]);

        }
        else if (myturn == PlayerTurn) // 아니면 다음사람 턴 넘겨주기
        {
            PV.RPC("RPCChangeTurn", RpcTarget.AllBufferedViaServer);
        }
    }

    public override void OnLeftRoom()
    {    
        SceneManager.LoadScene("MainScene");
        Disconnect();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) //나가면 모두 실행
    {
        PV.RPC("ChatRPC", RpcTarget.All, "<color=yellow>" + otherPlayer.NickName + "님이 퇴장하셨습니다.</color>");
       
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
       
    }
    #endregion
}
