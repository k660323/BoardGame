using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Character,//캐릭터
    Dice,//주사위
    Legacy//장착 아이템 유물
}

public class Item
{
    public ItemType itemType;

    public string itemClass;

    public string itemName;
    public Sprite itemImage;
    public string itemExplain;

    public Sprite itemAblityImage;
    public string itemAblityExplain;

    public int itemGoldPrice;
    public int itemCashPrice;

    public Item(ItemType _itemType, string _itemClass,string _itemName, Sprite _itemImage, string _itemExplain, Sprite _itemAblityImage,string _itemAblityExplain, int _itemGoldPrice,int _itemCashPrice)
    {
        itemType = _itemType;

        itemClass = _itemClass;

        itemName = _itemName;
        itemImage = _itemImage;
        itemExplain = _itemExplain;

        itemAblityImage = _itemAblityImage;
        itemAblityExplain = _itemAblityExplain;

        itemGoldPrice = _itemGoldPrice;
        itemCashPrice = _itemCashPrice;
    }

}

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase itemInstance;

   
    public List<Item> characterGoldItems = new List<Item>();
    public List<Item> characterCashItems = new List<Item>();

    public List<Item> diceGoldItems = new List<Item>();
    public List<Item> diceCashItems = new List<Item>();

    public List<Item> legacyGoldItems = new List<Item>();
    public List<Item> legacyCashItems = new List<Item>();

    void Awake()
    {
        if (itemInstance == null)
        {
            itemInstance = this;
            SetItem();
        }
    }

    void SetItem()
    {
        // 캐릭터 아이템
        //                  /아이템종류 /등급 /이름 /이미지 /설명 /능력이미지 /능력설명 /골드 /캐쉬                    
        //골드
        characterGoldItems.Add(new Item(ItemType.Character, "C", "UnityChan", Resources.Load<Sprite>("CharacterImage/UnityChan"), "기본 캐릭터", Resources.Load<Sprite>(""), "없음", 0, 0));
        characterGoldItems.Add(new Item(ItemType.Character, "S", "UnityChan2", Resources.Load<Sprite>("CharacterImage/UnityChan"), "기본 캐릭터2", Resources.Load<Sprite>(""), "없음", 0, 0));
        //캐쉬


        // 주사위 아이템
        //골드
        diceGoldItems.Add(new Item(ItemType.Dice, "C", "NormalDice", Resources.Load<Sprite>("DiceImage/NormalDice"), "일반 주사위", Resources.Load<Sprite>(""), "없음", 0, 0));
        diceGoldItems.Add(new Item(ItemType.Dice, "A", "NormalDice2", Resources.Load<Sprite>("DiceImage/NormalDice2"), "일반 주사위2", Resources.Load<Sprite>(""), "없음", 0, 0));
        //캐쉬
       
        // 유물 아이템
        //골드

        //캐쉬
        legacyCashItems.Add(new Item(ItemType.Legacy, "C", "NormalLegacy", Resources.Load<Sprite>("LegacyImage/NormalLegacy"), "일반 유물", Resources.Load<Sprite>(""), "없음", 0, 0));
        legacyCashItems.Add(new Item(ItemType.Legacy, "S", "EpicLegacy", Resources.Load<Sprite>("LegacyImage/NormalLegacy"), "에픽 유물", Resources.Load<Sprite>(""), "없음", 0, 0));
    }

    public Color SetItemClassBg(string _ItemClass)
    {
        switch (_ItemClass)
        {
            case "S":
                return new Color(255, 255, 0, 255);
            case "A":
                return new Color(255, 0, 238, 255);
            case "B":
                return new Color(0, 150, 255, 255);
            case "C":
            default:
                return new Color(255, 255, 255, 255);
        }
    }

    public void ShowItemData(Item _item)
    {
        GameObject.Find("Canvas").transform.Find("Window").Find("bg_Unclear2").gameObject.SetActive(true);
        GameObject itemInfoObject = GameObject.Find("Canvas").transform.Find("Window").Find("ItemInfoWindow").gameObject;
        itemInfoObject.SetActive(true);

        itemInfoObject.transform.Find("ItemImage").Find("Image").GetComponent<Image>().sprite = _item.itemImage;
        itemInfoObject.transform.Find("ItemName").Find("Text").GetComponent<Text>().text = _item.itemName;
        itemInfoObject.transform.Find("ItemExplain").Find("Text").GetComponent<Text>().text = _item.itemExplain;
        itemInfoObject.transform.Find("ItemAblity").Find("Image").GetComponent<Image>().sprite = _item.itemAblityImage;
        itemInfoObject.transform.Find("ItemAblity").Find("Text").GetComponent<Text>().text = _item.itemAblityExplain;

    }
}
