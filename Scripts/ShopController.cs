using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    [SerializeField]
    GameObject goldItemMask;
    [SerializeField]
    GameObject cashItemMask;
    [SerializeField]
    Image tabCharacterButton, tabDiceButton, tabLegacyButton;

    GameObject element,purchaseGameobject;

    Image itemImage;
    Text itemName, itemPrice;

    // 구입할 아이템 정보
    int tempItemPrice;
    bool isGold;
    ItemType itemType;
    int itemIndex;

    public void SetDefaultItem()
    {
        //버튼 모양 체인지
        tabCharacterButton.sprite = Resources.Load<Sprite>("BtnImage/Btn_title_pressed");
        tabDiceButton.sprite = Resources.Load<Sprite>("BtnImage/Btn_title_normal");
        tabLegacyButton.sprite = Resources.Load<Sprite>("BtnImage/Btn_title_normal");

        // 다른 탭의 내용물을 삭제
        for (int i = 0; i < goldItemMask.transform.Find("ScrollerBack").childCount; i++)
        {         
            Destroy(goldItemMask.transform.Find("ScrollerBack").transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < cashItemMask.transform.Find("ScrollerBack").childCount; i++)
        {
            Destroy(cashItemMask.transform.Find("ScrollerBack").transform.GetChild(i).gameObject);
        }

        //골드 마스크에 내용물 생성
        for (int i = 1; i < ItemDatabase.itemInstance.characterGoldItems.Count; i++)
        {
            //프리팹 위치 설정
            element = Instantiate(Resources.Load<GameObject>("Prefabs/GoldElement"));
            element.transform.parent = goldItemMask.transform.Find("ScrollerBack");
            element.transform.position = goldItemMask.transform.Find("ScrollerBack").position;
            element.transform.localScale = Vector3.one;

            // 아이템 등급에 따른 배경색 설정
            element.GetComponent<Image>().color = ItemDatabase.itemInstance.SetItemClassBg(ItemDatabase.itemInstance.characterGoldItems[i].itemClass);

            itemImage = element.transform.Find("ItemImage").Find("Image").GetComponent<Image>();
            itemImage.sprite = ItemDatabase.itemInstance.characterGoldItems[i].itemImage;

            itemName = element.transform.Find("ItemName").GetComponent<Text>();
            itemName.text = ItemDatabase.itemInstance.characterGoldItems[i].itemName;

            itemPrice = element.transform.Find("ItemPrice").Find("Text").GetComponent<Text>();
            itemPrice.text = ItemDatabase.itemInstance.characterGoldItems[i].itemGoldPrice.ToString();

            int index = i;
            element.transform.Find("ItemPrice").GetComponent<Button>().onClick.AddListener(() => { ActivePurchaseWindow(true, ItemType.Character, index); });

            element.transform.Find("TipButton").GetComponent<Button>().onClick.AddListener(() => { ItemDatabase.itemInstance.ShowItemData(ItemDatabase.itemInstance.characterGoldItems[index]); });
        }

        //캐쉬 마스크에 내용물 생성
        for (int i = 0; i < ItemDatabase.itemInstance.characterCashItems.Count; i++)
        {
            //프리팹 위치 설정
            element = Instantiate(Resources.Load<GameObject>("Prefabs/CashElement"));
            element.transform.parent = cashItemMask.transform.Find("ScrollerBack");
            element.transform.position = cashItemMask.transform.Find("ScrollerBack").position;
            element.transform.localScale = Vector3.one;

            // 아이템 등급에 따른 배경색 설정
            element.GetComponent<Image>().color = ItemDatabase.itemInstance.SetItemClassBg(ItemDatabase.itemInstance.characterCashItems[i].itemClass);

           // element.GetComponent<Image>().color = ItemDatabase.itemInstance.SetItemClassBg2(ItemDatabase.itemInstance.characterCashItems[i].itemClass);

            itemImage = element.transform.Find("ItemImage").Find("Image").GetComponent<Image>();
            itemImage.sprite = ItemDatabase.itemInstance.characterCashItems[i].itemImage;

            itemName = element.transform.Find("ItemName").GetComponent<Text>();
            itemName.text = ItemDatabase.itemInstance.characterCashItems[i].itemName;

            itemPrice = element.transform.Find("ItemPrice").Find("Text").GetComponent<Text>();
            itemPrice.text = ItemDatabase.itemInstance.characterCashItems[i].itemCashPrice.ToString();

            int index = i;
            element.transform.Find("ItemPrice").GetComponent<Button>().onClick.AddListener(() => { ActivePurchaseWindow(false, ItemType.Character, index); });

            element.transform.Find("TipButton").GetComponent<Button>().onClick.AddListener(() => { ItemDatabase.itemInstance.ShowItemData(ItemDatabase.itemInstance.characterCashItems[index]); });
        }

    }


    public void SetCharacterItem()
    {  
        if(tabCharacterButton.sprite == Resources.Load<Sprite>("BtnImage/Btn_title_normal"))
        {
            //버튼 모양 체인지
            tabCharacterButton.sprite = Resources.Load<Sprite>("BtnImage/Btn_title_pressed");
            tabDiceButton.sprite = Resources.Load<Sprite>("BtnImage/Btn_title_normal");
            tabLegacyButton.sprite = Resources.Load<Sprite>("BtnImage/Btn_title_normal");

            // 다른 탭의 내용물을 삭제
            for(int i=0; i< goldItemMask.transform.Find("ScrollerBack").childCount; i++)
            {
                Destroy(goldItemMask.transform.Find("ScrollerBack").transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < cashItemMask.transform.Find("ScrollerBack").childCount; i++)
            {
                Destroy(cashItemMask.transform.Find("ScrollerBack").transform.GetChild(i).gameObject);
            }

            //골드 마스크에 내용물 생성
            for(int i=1; i< ItemDatabase.itemInstance.characterGoldItems.Count; i++)
            {
                //프리팹 위치 설정
                element = Instantiate(Resources.Load<GameObject>("Prefabs/GoldElement"));
                element.transform.parent = goldItemMask.transform.Find("ScrollerBack");
                element.transform.position = goldItemMask.transform.Find("ScrollerBack").position;
                element.transform.localScale = Vector3.one;

                // 아이템 등급에 따른 배경색 설정
                element.GetComponent<Image>().color = ItemDatabase.itemInstance.SetItemClassBg(ItemDatabase.itemInstance.characterGoldItems[i].itemClass);

                itemImage = element.transform.Find("ItemImage").Find("Image").GetComponent<Image>();
                itemImage.sprite = ItemDatabase.itemInstance.characterGoldItems[i].itemImage;

                itemName = element.transform.Find("ItemName").GetComponent<Text>();
                itemName.text = ItemDatabase.itemInstance.characterGoldItems[i].itemName;

                itemPrice = element.transform.Find("ItemPrice").Find("Text").GetComponent<Text>();
                itemPrice.text = ItemDatabase.itemInstance.characterGoldItems[i].itemGoldPrice.ToString();

                int index = i;
                element.transform.Find("ItemPrice").GetComponent<Button>().onClick.AddListener(() => { ActivePurchaseWindow(true, ItemType.Character, index); });

                element.transform.Find("TipButton").GetComponent<Button>().onClick.AddListener(() => { ItemDatabase.itemInstance.ShowItemData(ItemDatabase.itemInstance.characterGoldItems[index]); });
            }

            //캐쉬 마스크에 내용물 생성
            for (int i = 0; i < ItemDatabase.itemInstance.characterCashItems.Count; i++)
            {
                //프리팹 위치 설정
                element = Instantiate(Resources.Load<GameObject>("Prefabs/CashElement"));
                element.transform.parent = cashItemMask.transform.Find("ScrollerBack");
                element.transform.position = cashItemMask.transform.Find("ScrollerBack").position;
                element.transform.localScale = Vector3.one;
                // 아이템 등급에 따른 배경색 설정
                element.GetComponent<Image>().color = ItemDatabase.itemInstance.SetItemClassBg(ItemDatabase.itemInstance.characterCashItems[i].itemClass);

                itemImage = element.transform.Find("ItemImage").Find("Image").GetComponent<Image>();
                itemImage.sprite = ItemDatabase.itemInstance.characterCashItems[i].itemImage;

                itemName = element.transform.Find("ItemName").GetComponent<Text>();
                itemName.text = ItemDatabase.itemInstance.characterCashItems[i].itemName;

                itemPrice = element.transform.Find("ItemPrice").Find("Text").GetComponent<Text>();
                itemPrice.text = ItemDatabase.itemInstance.characterCashItems[i].itemCashPrice.ToString();

                int index = i;
                element.transform.Find("ItemPrice").GetComponent<Button>().onClick.AddListener(() => { ActivePurchaseWindow(false, ItemType.Character, index); });

                element.transform.Find("TipButton").GetComponent<Button>().onClick.AddListener(() => { ItemDatabase.itemInstance.ShowItemData(ItemDatabase.itemInstance.characterCashItems[index]); });
            }
        }


    }

    public void SetDiceItem()
    {
        if (tabDiceButton.sprite == Resources.Load<Sprite>("BtnImage/Btn_title_normal"))
        {
            //버튼 모양 체인지
            tabCharacterButton.sprite = Resources.Load<Sprite>("BtnImage/Btn_title_normal");
            tabDiceButton.sprite = Resources.Load<Sprite>("BtnImage/Btn_title_pressed");
            tabLegacyButton.sprite = Resources.Load<Sprite>("BtnImage/Btn_title_normal");

            // 다른 탭의 내용물을 삭제
            for (int i = 0; i < goldItemMask.transform.Find("ScrollerBack").childCount; i++)
            {
                Destroy(goldItemMask.transform.Find("ScrollerBack").transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < cashItemMask.transform.Find("ScrollerBack").childCount; i++)
            {
                Destroy(cashItemMask.transform.Find("ScrollerBack").transform.GetChild(i).gameObject);
            }

            //골드 마스크에 내용물 생성
            for (int i = 1; i < ItemDatabase.itemInstance.diceGoldItems.Count; i++)
            {
                //프리팹 위치 설정
                element = Instantiate(Resources.Load<GameObject>("Prefabs/GoldElement"));
                element.transform.parent = goldItemMask.transform.Find("ScrollerBack");
                element.transform.position = goldItemMask.transform.Find("ScrollerBack").position;
                element.transform.localScale = Vector3.one;

                // 아이템 등급에 따른 배경색 설정
                element.GetComponent<Image>().color = ItemDatabase.itemInstance.SetItemClassBg(ItemDatabase.itemInstance.diceGoldItems[i].itemClass);

                itemImage = element.transform.Find("ItemImage").Find("Image").GetComponent<Image>();
                itemImage.sprite = ItemDatabase.itemInstance.diceGoldItems[i].itemImage;

                itemName = element.transform.Find("ItemName").GetComponent<Text>();
                itemName.text = ItemDatabase.itemInstance.diceGoldItems[i].itemName;

                itemPrice = element.transform.Find("ItemPrice").Find("Text").GetComponent<Text>();
                itemPrice.text = ItemDatabase.itemInstance.diceGoldItems[i].itemGoldPrice.ToString();

                int index = i;
                element.transform.Find("ItemPrice").GetComponent<Button>().onClick.AddListener(() => { ActivePurchaseWindow(true, ItemType.Dice, index); });


                element.transform.Find("TipButton").GetComponent<Button>().onClick.AddListener(() => { ItemDatabase.itemInstance.ShowItemData(ItemDatabase.itemInstance.diceGoldItems[index]); });
            }

            //캐쉬 마스크에 내용물 생성
            for (int i = 0; i < ItemDatabase.itemInstance.diceCashItems.Count; i++)
            {
                //프리팹 위치 설정
                element = Instantiate(Resources.Load<GameObject>("Prefabs/CashElement"));
                element.transform.parent = cashItemMask.transform.Find("ScrollerBack");
                element.transform.position = cashItemMask.transform.Find("ScrollerBack").position;
                element.transform.localScale = Vector3.one;

                // 아이템 등급에 따른 배경색 설정
                element.GetComponent<Image>().color = ItemDatabase.itemInstance.SetItemClassBg(ItemDatabase.itemInstance.diceCashItems[i].itemClass);

                itemImage = element.transform.Find("ItemImage").Find("Image").GetComponent<Image>();
                itemImage.sprite = ItemDatabase.itemInstance.diceCashItems[i].itemImage;

                itemName = element.transform.Find("ItemName").GetComponent<Text>();
                itemName.text = ItemDatabase.itemInstance.diceCashItems[i].itemName;

                itemPrice = element.transform.Find("ItemPrice").Find("Text").GetComponent<Text>();
                itemPrice.text = ItemDatabase.itemInstance.diceCashItems[i].itemCashPrice.ToString();

                int index = i;
                element.transform.Find("ItemPrice").GetComponent<Button>().onClick.AddListener(() => { ActivePurchaseWindow(false, ItemType.Dice, index); });

                element.transform.Find("TipButton").GetComponent<Button>().onClick.AddListener(() => { ItemDatabase.itemInstance.ShowItemData(ItemDatabase.itemInstance.diceCashItems[index]); });
            }
        }
    }

    public void SetLegacyItem()
    {
        if (tabLegacyButton.sprite == Resources.Load<Sprite>("BtnImage/Btn_title_normal"))
        {
            //버튼 모양 체인지
            tabCharacterButton.sprite = Resources.Load<Sprite>("BtnImage/Btn_title_normal");
            tabDiceButton.sprite = Resources.Load<Sprite>("BtnImage/Btn_title_normal");
            tabLegacyButton.sprite = Resources.Load<Sprite>("BtnImage/Btn_title_pressed");

            // 다른 탭의 내용물을 삭제
            for (int i = 0; i < goldItemMask.transform.Find("ScrollerBack").childCount; i++)
            {
                Destroy(goldItemMask.transform.Find("ScrollerBack").transform.GetChild(i).gameObject);
            }
            for (int i = 0; i < cashItemMask.transform.Find("ScrollerBack").childCount; i++)
            {
                Destroy(cashItemMask.transform.Find("ScrollerBack").transform.GetChild(i).gameObject);
            }

            //골드 마스크에 내용물 생성
            for (int i = 0; i < ItemDatabase.itemInstance.legacyGoldItems.Count; i++)
            {
                //프리팹 위치 설정
                element = Instantiate(Resources.Load<GameObject>("Prefabs/GoldElement"));
                element.transform.parent = goldItemMask.transform.Find("ScrollerBack");
                element.transform.position = goldItemMask.transform.Find("ScrollerBack").position;
                element.transform.localScale = Vector3.one;

                // 아이템 등급에 따른 배경색 설정
                element.GetComponent<Image>().color = ItemDatabase.itemInstance.SetItemClassBg(ItemDatabase.itemInstance.legacyGoldItems[i].itemClass);

                itemImage = element.transform.Find("ItemImage").Find("Image").GetComponent<Image>();
                itemImage.sprite = ItemDatabase.itemInstance.legacyGoldItems[i].itemImage;

                itemName = element.transform.Find("ItemName").GetComponent<Text>();
                itemName.text = ItemDatabase.itemInstance.legacyGoldItems[i].itemName;

                itemPrice = element.transform.Find("ItemPrice").Find("Text").GetComponent<Text>();
                itemPrice.text = ItemDatabase.itemInstance.legacyGoldItems[i].itemGoldPrice.ToString();

                int index = i;
                element.transform.Find("ItemPrice").GetComponent<Button>().onClick.AddListener(() => { ActivePurchaseWindow(true, ItemType.Legacy, index); });

                element.transform.Find("TipButton").GetComponent<Button>().onClick.AddListener(() => { ItemDatabase.itemInstance.ShowItemData(ItemDatabase.itemInstance.legacyGoldItems[index]); });
            }

            //캐쉬 마스크에 내용물 생성
            for (int i = 0; i < ItemDatabase.itemInstance.legacyCashItems.Count; i++)
            {
                //프리팹 위치 설정
                element = Instantiate(Resources.Load<GameObject>("Prefabs/CashElement"));
                element.transform.parent = cashItemMask.transform.Find("ScrollerBack");
                element.transform.position = cashItemMask.transform.Find("ScrollerBack").position;
                element.transform.localScale = Vector3.one;

                // 아이템 등급에 따른 배경색 설정
                element.GetComponent<Image>().color = ItemDatabase.itemInstance.SetItemClassBg(ItemDatabase.itemInstance.legacyCashItems[i].itemClass);

                itemImage = element.transform.Find("ItemImage").Find("Image").GetComponent<Image>();
                itemImage.sprite = ItemDatabase.itemInstance.legacyCashItems[i].itemImage;

                itemName = element.transform.Find("ItemName").GetComponent<Text>();
                itemName.text = ItemDatabase.itemInstance.legacyCashItems[i].itemName;

                itemPrice = element.transform.Find("ItemPrice").Find("Text").GetComponent<Text>();
                itemPrice.text = ItemDatabase.itemInstance.legacyCashItems[i].itemCashPrice.ToString();

                int index = i;
                element.transform.Find("ItemPrice").GetComponent<Button>().onClick.AddListener(() => { ActivePurchaseWindow(false, ItemType.Legacy, index); });

                element.transform.Find("TipButton").GetComponent<Button>().onClick.AddListener(() => { ItemDatabase.itemInstance.ShowItemData(ItemDatabase.itemInstance.legacyCashItems[index]); });
            }
        }
    }

    void ActivePurchaseWindow(bool _isGold,ItemType _itemType,int _index) // 돈?캐쉬 // 아이템 종류 // 몇번째 아이템
    {
        purchaseGameobject = GameObject.Find("Window").transform.Find("PurchaseWindow").gameObject;
        purchaseGameobject.SetActive(true);
        if(_isGold) // 골드템
        {
            isGold = _isGold;
            itemIndex = _index;
            itemType = _itemType;
            if (_itemType == ItemType.Character) // 캐릭터
            {
                // 아이템 이미지
                purchaseGameobject.transform.Find("ItemImage").Find("Image").GetComponent<Image>().sprite = ItemDatabase.itemInstance.characterGoldItems[_index].itemImage;
                purchaseGameobject.transform.Find("ItemInfo").Find("ItemName").GetComponent<Text>().text = ItemDatabase.itemInstance.characterGoldItems[_index].itemName;
                purchaseGameobject.transform.Find("ItemInfo").Find("ItemType").GetComponent<Text>().text = ItemDatabase.itemInstance.characterGoldItems[_index].itemType.ToString();
                purchaseGameobject.transform.Find("ItemInfo").Find("Calcuration").GetComponent<Text>().text = PlayerInfo.playerInfo.getGold().ToString() + " - " + ItemDatabase.itemInstance.characterGoldItems[_index].itemGoldPrice.ToString()+
                    "\n = " + (PlayerInfo.playerInfo.getGold()- ItemDatabase.itemInstance.characterGoldItems[_index].itemGoldPrice).ToString();
                tempItemPrice = ItemDatabase.itemInstance.characterGoldItems[_index].itemGoldPrice;
            }
            else if(_itemType == ItemType.Dice)//주사위
            {
                // 아이템 이미지
                purchaseGameobject.transform.Find("ItemImage").Find("Image").GetComponent<Image>().sprite = ItemDatabase.itemInstance.diceGoldItems[_index].itemImage;
                purchaseGameobject.transform.Find("ItemInfo").Find("ItemName").GetComponent<Text>().text = ItemDatabase.itemInstance.diceGoldItems[_index].itemName;
                purchaseGameobject.transform.Find("ItemInfo").Find("ItemType").GetComponent<Text>().text = ItemDatabase.itemInstance.diceGoldItems[_index].itemType.ToString();
                purchaseGameobject.transform.Find("ItemInfo").Find("Calcuration").GetComponent<Text>().text = PlayerInfo.playerInfo.getGold().ToString() + " - " + ItemDatabase.itemInstance.diceGoldItems[_index].itemGoldPrice.ToString() +
                    "\n = " + (PlayerInfo.playerInfo.getGold() - ItemDatabase.itemInstance.diceGoldItems[_index].itemGoldPrice).ToString();
                tempItemPrice = ItemDatabase.itemInstance.diceGoldItems[_index].itemGoldPrice;
            }
            else if(_itemType == ItemType.Legacy)//유물
            {
                // 아이템 이미지
                purchaseGameobject.transform.Find("ItemImage").Find("Image").GetComponent<Image>().sprite = ItemDatabase.itemInstance.legacyGoldItems[_index].itemImage;
                purchaseGameobject.transform.Find("ItemInfo").Find("ItemName").GetComponent<Text>().text = ItemDatabase.itemInstance.legacyGoldItems[_index].itemName;
                purchaseGameobject.transform.Find("ItemInfo").Find("ItemType").GetComponent<Text>().text = ItemDatabase.itemInstance.legacyGoldItems[_index].itemType.ToString();
                purchaseGameobject.transform.Find("ItemInfo").Find("Calcuration").GetComponent<Text>().text = PlayerInfo.playerInfo.getGold().ToString() + " - " + ItemDatabase.itemInstance.legacyGoldItems[_index].itemGoldPrice.ToString() +
                    "\n = " + (PlayerInfo.playerInfo.getGold() - ItemDatabase.itemInstance.legacyGoldItems[_index].itemGoldPrice).ToString();
                tempItemPrice = ItemDatabase.itemInstance.legacyGoldItems[_index].itemGoldPrice;
            }
        }
        else // 캐시템
        {
            isGold = _isGold;
            itemIndex = _index;
            itemType = _itemType;
            if (_itemType == ItemType.Character) // 캐릭터
            {
                // 아이템 이미지
                purchaseGameobject.transform.Find("ItemImage").Find("Image").GetComponent<Image>().sprite = ItemDatabase.itemInstance.characterCashItems[_index].itemImage;
                purchaseGameobject.transform.Find("ItemInfo").Find("ItemName").GetComponent<Text>().text = ItemDatabase.itemInstance.characterCashItems[_index].itemName;
                purchaseGameobject.transform.Find("ItemInfo").Find("ItemType").GetComponent<Text>().text = ItemDatabase.itemInstance.characterCashItems[_index].itemType.ToString();
                purchaseGameobject.transform.Find("ItemInfo").Find("Calcuration").GetComponent<Text>().text = PlayerInfo.playerInfo.getCash().ToString() + " - " + ItemDatabase.itemInstance.characterCashItems[_index].itemCashPrice.ToString() +
                    "\n = " + (PlayerInfo.playerInfo.getCash() - ItemDatabase.itemInstance.characterCashItems[_index].itemCashPrice).ToString();
                tempItemPrice = ItemDatabase.itemInstance.characterCashItems[_index].itemCashPrice;
            }
            else if (_itemType == ItemType.Dice)//주사위
            {
                // 아이템 이미지
                purchaseGameobject.transform.Find("ItemImage").Find("Image").GetComponent<Image>().sprite = ItemDatabase.itemInstance.diceCashItems[_index].itemImage;
                purchaseGameobject.transform.Find("ItemInfo").Find("ItemName").GetComponent<Text>().text = ItemDatabase.itemInstance.diceCashItems[_index].itemName;
                purchaseGameobject.transform.Find("ItemInfo").Find("ItemType").GetComponent<Text>().text = ItemDatabase.itemInstance.diceCashItems[_index].itemType.ToString();
                purchaseGameobject.transform.Find("ItemInfo").Find("Calcuration").GetComponent<Text>().text = PlayerInfo.playerInfo.getCash().ToString() + " - " + ItemDatabase.itemInstance.diceCashItems[_index].itemCashPrice.ToString() +
                    "\n = " + (PlayerInfo.playerInfo.getCash() - ItemDatabase.itemInstance.diceCashItems[_index].itemCashPrice).ToString();
                tempItemPrice = ItemDatabase.itemInstance.diceCashItems[_index].itemCashPrice;
            }
            else if (_itemType == ItemType.Legacy)//유물
            {
                // 아이템 이미지
                purchaseGameobject.transform.Find("ItemImage").Find("Image").GetComponent<Image>().sprite = ItemDatabase.itemInstance.legacyCashItems[_index].itemImage;
                purchaseGameobject.transform.Find("ItemInfo").Find("ItemName").GetComponent<Text>().text = ItemDatabase.itemInstance.legacyCashItems[_index].itemName;
                purchaseGameobject.transform.Find("ItemInfo").Find("ItemType").GetComponent<Text>().text = ItemDatabase.itemInstance.legacyCashItems[_index].itemType.ToString();
                purchaseGameobject.transform.Find("ItemInfo").Find("Calcuration").GetComponent<Text>().text = PlayerInfo.playerInfo.getCash().ToString() + " - " + ItemDatabase.itemInstance.legacyCashItems[_index].itemCashPrice.ToString() +
                    "\n = " + (PlayerInfo.playerInfo.getCash() - ItemDatabase.itemInstance.legacyCashItems[_index].itemCashPrice).ToString();
                tempItemPrice = ItemDatabase.itemInstance.legacyCashItems[_index].itemCashPrice;
            }
        }
    }

    public void ConfirmItem()
    {
        if (isGold)
        {
             if(PlayerInfo.playerInfo.getGold() >= tempItemPrice)
             {
                 GameObject.Find("Canvas").transform.Find("Window").Find("bg_Black2").gameObject.SetActive(true);
                 GameObject.Find("Canvas").transform.Find("Window").Find("PCompleteWindow").gameObject.SetActive(true);
                 
                 GameObject.Find("Canvas").transform.Find("Window").Find("PCompleteWindow").Find("ItemType").GetComponent<Text>().text = GameObject.Find("Canvas").transform.Find("Window").Find("PurchaseWindow").Find("ItemInfo").Find("ItemType").GetComponent<Text>().text;
                 GameObject.Find("Canvas").transform.Find("Window").Find("PCompleteWindow").Find("ItemImage").GetComponent<Image>().sprite = GameObject.Find("Canvas").transform.Find("Window").Find("PurchaseWindow").Find("ItemImage").Find("Image").GetComponent<Image>().sprite;
                 GameObject.Find("Canvas").transform.Find("Window").Find("PCompleteWindow").Find("ItemName").GetComponent<Text>().text = GameObject.Find("Canvas").transform.Find("Window").Find("PurchaseWindow").Find("ItemInfo").Find("ItemName").GetComponent<Text>().text;


                // 계산 //아이템종류(TYPE)//해당 아이템(인덱스)
                PlayerInfo.playerInfo.setGold(-tempItemPrice);
                PlayerInfo.playerInfo.setDictionary(true, itemType, itemIndex);
             }
             else
             {
                GameObject.Find("Canvas").transform.Find("Window").Find("WarningWindow").gameObject.SetActive(true);
                GameObject.Find("Canvas").transform.Find("Window").Find("WarningWindow").Find("WarningText").GetComponent<Text>().text = "돈이 부족 합니다.";
             }
        }
        else
        {
            if (PlayerInfo.playerInfo.getCash() >= tempItemPrice)
            {
                GameObject.Find("Canvas").transform.Find("Window").Find("bg_Black2").gameObject.SetActive(true);
                GameObject.Find("Canvas").transform.Find("Window").Find("PCompleteWindow").gameObject.SetActive(true);

                GameObject.Find("Canvas").transform.Find("Window").Find("PCompleteWindow").Find("ItemType").GetComponent<Text>().text = GameObject.Find("Canvas").transform.Find("Window").Find("PurchaseWindow").Find("ItemInfo").Find("ItemType").GetComponent<Text>().text;
                GameObject.Find("Canvas").transform.Find("Window").Find("PCompleteWindow").Find("ItemImage").GetComponent<Image>().sprite = GameObject.Find("Canvas").transform.Find("Window").Find("PurchaseWindow").Find("ItemImage").Find("Image").GetComponent<Image>().sprite;
                GameObject.Find("Canvas").transform.Find("Window").Find("PCompleteWindow").Find("ItemName").GetComponent<Text>().text = GameObject.Find("Canvas").transform.Find("Window").Find("PurchaseWindow").Find("ItemInfo").Find("ItemName").GetComponent<Text>().text;

                // 계산 //아이템종류(TYPE)//해당 아이템(인덱스) // 인벤토리에 추가
                PlayerInfo.playerInfo.setCash(-tempItemPrice);
                PlayerInfo.playerInfo.setDictionary(false, itemType, itemIndex);
            }
            else
            {
                GameObject.Find("Canvas").transform.Find("Window").Find("WarningWindow").gameObject.SetActive(true);
                GameObject.Find("Canvas").transform.Find("Window").Find("WarningWindow").Find("WarningText").GetComponent<Text>().text = "캐쉬가 부족 합니다.";
            }
        }
    }
}
