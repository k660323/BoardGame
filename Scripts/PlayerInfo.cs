using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using Photon.Pun.UtilityScripts;
using UnityEngine.UI;
using Photon.Realtime;

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo playerInfo;

    private Dictionary<string, Item> havingGoldCharacter;
    private Dictionary<string, Item> havingGoldDice;
    private Dictionary<string, Item> havingGoldLegacy;

    private Dictionary<string, Item> havingCashCharacter;
    private Dictionary<string, Item> havingCashDice;
    private Dictionary<string, Item> havingCashLegacy;

    string data;
    public string playerName;
    string equipCharacter;
    public string equipDice;
    string equipLegacy;
    public string equipLegacy1;
    public string equipLegacy2;
    public string equipLegacy3;

    int gold;
    int cash;

    public int total;
    public int win;
    public int lose;
    public int draw;

    float effectSound;
    float backgroundSound;


    void Init()
    {
        if (LoginManager.nickName == null)
        {
            playerName = "GM";
        }
        else
        {
            playerName = LoginManager.nickName;        // 플레이어 닉네임
        }
        /*
        #if UNITY_EDITOR
                if(LoginManager.nickName == null)
                playerName = "GM";        // 플레이어 닉네임
        #endif
        */
        equipCharacter = "UnityChan";  // 현재 착용중인 캐릭터

        equipDice = "NormalDice";            // 현재 착용중인 주사위

        equipLegacy1 = "null";             // 현재 착용중인 아이템 1,2,3
        equipLegacy2 = "null";
        equipLegacy3 = "null";

        gold = 0;
        cash = 0;

        total = 0;
        win = 0;
        lose = 0;
        draw = 0;
        //winRate = (float)((win/(total))*100);

        havingGoldCharacter = new Dictionary<string, Item>();
        havingGoldDice = new Dictionary<string, Item>();
        havingGoldLegacy = new Dictionary<string, Item>();

        havingCashCharacter = new Dictionary<string, Item>();
        havingCashDice = new Dictionary<string, Item>();
        havingCashLegacy = new Dictionary<string, Item>();

        effectSound = 0.5f;
        backgroundSound = 0.5f;
    }
    /*
    // 생성자 매개변수로 초기화시 문제 발생
    PlayerInfo()
    {
        playerName = LoginManager.nickName;        // 플레이어 닉네임

#if UNITY_EDITOR
        if(LoginManager.nickName == null)
        playerName = "GM";        // 플레이어 닉네임
#endif

        equipCharacter = "UnityChan";  // 현재 착용중인 캐릭터

        equipDice = "NormalDice";            // 현재 착용중인 주사위
        
        equipLegacy1 = "null";             // 현재 착용중인 아이템 1,2,3
        equipLegacy2 = "null";
        equipLegacy3 = "null";

        gold = 0;
        cash = 0;

        total = 0;
        win = 0;
        lose = 0;
        draw = 0;
        //winRate = (float)((win/(total))*100);

        havingGoldCharacter = new Dictionary<string, Item>();
        havingGoldDice = new Dictionary<string, Item>();
        havingGoldLegacy = new Dictionary<string, Item>();

        havingCashCharacter = new Dictionary<string, Item>();
        havingCashDice = new Dictionary<string, Item>();
        havingCashLegacy = new Dictionary<string, Item>(); 

        effectSound = 0.5f;
        backgroundSound = 0.5f;
    }*/

    // Start is called before the first frame update
    void Awake()
    {
        if (playerInfo == null)
        {
            if (Resources.Load("JsonFile/PlayerInfo") == null)
            {
                playerInfo = this;
                Init();
                //기본 캐릭터 주사위 설정
                setDictionary(true, ItemType.Character, 0);
                setDictionary(true, ItemType.Dice, 0);

                DontDestroyOnLoad(this);
            }
            else if (Resources.Load("JsonFile/PlayerInfo") != null)// 컴퓨터에 저장된 정보를 받아와 객체에 저장
            {
                data = (Resources.Load("JsonFile/PlayerInfo") as TextAsset).text;
                playerInfo = JsonConvert.DeserializeObject<PlayerInfo>(data);
                playerInfo.playerName = LoginManager.nickName;
                DontDestroyOnLoad(this);
            }
        }
    }

    public void setGold(int setgold)
    {
        gold += setgold;
    }

    public int getGold()
    {
        return gold;
    }


    public void setCash(int setcash)
    {
        cash = setcash;
    }

    public int getCash()
    {
        return cash;
    }

    public void setBgSound(float _bgSound)
    {
        backgroundSound = _bgSound;
    }

    public float getBgSound()
    {
        return backgroundSound;
    }

    public void setEffectSound(float _effectSound)
    {
        effectSound = _effectSound;
    }

    public float getEffectSound()
    {
        return effectSound;
    }


    public void setDictionary(bool isGold, ItemType _itemType, int _index)
    {
        if (isGold)
        {
            switch (_itemType)
            {
                case ItemType.Character:
                    PlayerInfo.playerInfo.havingGoldCharacter.Add(ItemDatabase.itemInstance.characterGoldItems[_index].itemName, ItemDatabase.itemInstance.characterGoldItems[_index]);
                    break;
                case ItemType.Dice:
                    PlayerInfo.playerInfo.havingGoldDice.Add(ItemDatabase.itemInstance.diceGoldItems[_index].itemName, ItemDatabase.itemInstance.diceGoldItems[_index]);
                    break;
                case ItemType.Legacy:
                    PlayerInfo.playerInfo.havingGoldLegacy.Add(ItemDatabase.itemInstance.legacyGoldItems[_index].itemName, ItemDatabase.itemInstance.legacyGoldItems[_index]);
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (_itemType)
            {
                case ItemType.Character:
                    PlayerInfo.playerInfo.havingCashCharacter.Add(ItemDatabase.itemInstance.characterCashItems[_index].itemName, ItemDatabase.itemInstance.characterCashItems[_index]);
                    break;
                case ItemType.Dice:
                    PlayerInfo.playerInfo.havingCashDice.Add(ItemDatabase.itemInstance.diceCashItems[_index].itemName, ItemDatabase.itemInstance.diceCashItems[_index]);
                    break;
                case ItemType.Legacy:
                    PlayerInfo.playerInfo.havingCashLegacy.Add(ItemDatabase.itemInstance.legacyCashItems[_index].itemName, ItemDatabase.itemInstance.legacyCashItems[_index]);
                    break;
                default:
                    break;
            }
        }
    }

    public Dictionary<String,Item> getDictionary(bool isGold,ItemType _itemType)
    {
        if (isGold)
        {
              switch (_itemType)
              {
                  case ItemType.Character:
                      return playerInfo.havingGoldCharacter;
                  case ItemType.Dice:
                      return playerInfo.havingGoldDice;
                  case ItemType.Legacy:
                  default:
                      return playerInfo.havingGoldLegacy;
                     
              }
        }
        else
        {
            switch (_itemType)
            {
                case ItemType.Character:
                    return playerInfo.havingCashCharacter;
                case ItemType.Dice:
                    return playerInfo.havingCashDice;
                case ItemType.Legacy:
                default:
                    return playerInfo.havingCashLegacy;

            }
        }
    }

    public void SetEquipItem(ItemType _itemType,string _itemName)
    {
        switch (_itemType)
        {
            case ItemType.Character:
                PlayerInfo.playerInfo.equipCharacter = _itemName;
                break;
            case ItemType.Dice:
                PlayerInfo.playerInfo.equipDice = _itemName;
                break;
            case ItemType.Legacy:
                if (GameObject.Find("Canvas").transform.Find("Window").Find("MyItemInventoryWindow").Find("LegacyItemSlotDropdown").Find("Label").GetComponent<Text>().text == "1번 슬롯")
                {
                    PlayerInfo.playerInfo.equipLegacy1 = _itemName;
                }
                else if (GameObject.Find("Canvas").transform.Find("Window").Find("MyItemInventoryWindow").Find("LegacyItemSlotDropdown").Find("Label").GetComponent<Text>().text == "2번 슬롯")
                {
                    PlayerInfo.playerInfo.equipLegacy2 = _itemName;
                }
                else if (GameObject.Find("Canvas").transform.Find("Window").Find("MyItemInventoryWindow").Find("LegacyItemSlotDropdown").Find("Label").GetComponent<Text>().text == "3번 슬롯")
                {
                    PlayerInfo.playerInfo.equipLegacy3 = _itemName;
                }
                break;
        }
    }

    public String GetEquipItem(ItemType _itemType)
    {
        switch (_itemType)
        {
            case ItemType.Character:
                return equipCharacter;
            case ItemType.Dice:
                return equipDice;
            case ItemType.Legacy:
            default:
                return equipLegacy = equipLegacy1 + "," + equipLegacy2 + "," + equipLegacy3;
        }
    }
}
