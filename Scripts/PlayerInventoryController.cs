using JetBrains.Annotations;
using Photon.Pun.Demo.Cockpit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryController : MonoBehaviour
{
    [SerializeField]
    GameObject goldItemMask;
    [SerializeField]
    GameObject cashItemMask;

    [SerializeField]
    Image tabCharacterButton, tabDiceButton, tabLegacyButton;

    GameObject element,beforeCharacterElement, beforeDiceElement;
    GameObject[] beforeLegacyElement = new GameObject[3];

    string[] templegacy;

    void SetChangeButtonImage(ItemType _itemType,GameObject _UsingItem)
    {
        switch (_itemType)
        {
            case ItemType.Character:
                _UsingItem.transform.Find("EquipItemButton").gameObject.SetActive(false); // 착용할 아이템
                _UsingItem.transform.Find("UsingItemButton").gameObject.SetActive(true); 

                beforeCharacterElement.transform.Find("EquipItemButton").gameObject.SetActive(true); // 착용해제한 아이템
                beforeCharacterElement.transform.Find("UsingItemButton").gameObject.SetActive(false);

                beforeCharacterElement = _UsingItem; // 착용할 아이템이 들어간다.
                break;
            case ItemType.Dice:
                _UsingItem.transform.Find("EquipItemButton").gameObject.SetActive(false); // 착용할 아이템
                _UsingItem.transform.Find("UsingItemButton").gameObject.SetActive(true);

                beforeDiceElement.transform.Find("EquipItemButton").gameObject.SetActive(true); // 착용해제한 아이템
                beforeDiceElement.transform.Find("UsingItemButton").gameObject.SetActive(false);

                beforeDiceElement = _UsingItem; // 착용할 아이템이 들어간다.
                break;
            case ItemType.Legacy:
                _UsingItem.transform.Find("EquipItemButton").gameObject.SetActive(false); // 착용할 아이템
                _UsingItem.transform.Find("UsingItemButton").gameObject.SetActive(true);

                switch (GameObject.Find("Canvas").transform.Find("Window").Find("MyItemInventoryWindow").Find("LegacyItemSlotDropdown").Find("Label").GetComponent<Text>().text)
                {
                    case "1번 슬롯":
                        if(beforeLegacyElement[0] != null)
                        {
                             beforeLegacyElement[0].transform.Find("EquipItemButton").gameObject.SetActive(true);
                             beforeLegacyElement[0].transform.Find("UsingItemButton").gameObject.SetActive(false);

                             beforeLegacyElement[0] = _UsingItem; // 착용할 아이템이 들어간다.
                        }
                        break;
                    case "2번 슬롯":
                        if (beforeLegacyElement[1] != null)
                        {
                            beforeLegacyElement[1].transform.Find("EquipItemButton").gameObject.SetActive(true);
                            beforeLegacyElement[1].transform.Find("UsingItemButton").gameObject.SetActive(false);

                            beforeLegacyElement[1] = _UsingItem; // 착용할 아이템이 들어간다.
                        }
                        break;
                    case "3번 슬롯":
                        if (beforeLegacyElement[2] != null)
                        {
                            beforeLegacyElement[2].transform.Find("EquipItemButton").gameObject.SetActive(true);
                            beforeLegacyElement[2].transform.Find("UsingItemButton").gameObject.SetActive(false);

                            beforeLegacyElement[2] = _UsingItem; // 착용할 아이템이 들어간다.
                        }
                        break;
                }

                break;
        }
        
    }

    // Start is called before the first frame update
    public void SetDefaultInventoryItem()
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

        //가지고 있는 골드 아이템
        foreach (KeyValuePair<string, Item> pair in PlayerInfo.playerInfo.getDictionary(true, ItemType.Character))
        {
            element = Instantiate(Resources.Load<GameObject>("Prefabs/HavingItemElement"));
            element.transform.parent = goldItemMask.transform.Find("ScrollerBack");
            element.transform.position = goldItemMask.transform.Find("ScrollerBack").position;
            element.transform.localScale = Vector3.one;

            // 아이템 등급에 따른 배경색 설정
            element.GetComponent<Image>().color = ItemDatabase.itemInstance.SetItemClassBg(pair.Value.itemClass);

            element.transform.Find("ItemImage").GetComponent<Image>().sprite = pair.Value.itemImage;
            element.transform.Find("TipButton").GetComponent<Button>().onClick.AddListener(() => { ItemDatabase.itemInstance.ShowItemData(pair.Value); });
            element.transform.Find("ItemName").GetComponent<Text>().text = pair.Value.itemName;

            // 아이템 착용 상태 // 버튼 이미지 변경
            GameObject characterItem = element;
            element.transform.Find("EquipItemButton").GetComponent<Button>().onClick.AddListener(() => { PlayerInfo.playerInfo.SetEquipItem(pair.Value.itemType, pair.Value.itemName); });
            element.transform.Find("EquipItemButton").GetComponent<Button>().onClick.AddListener(() => { SetChangeButtonImage(ItemType.Character, characterItem); });       

            if (PlayerInfo.playerInfo.GetEquipItem(ItemType.Character) == pair.Value.itemName)//이게 사용중
            {
                beforeCharacterElement = characterItem;
                element.transform.Find("EquipItemButton").gameObject.SetActive(false);
                element.transform.Find("UsingItemButton").gameObject.SetActive(true);
            }
            else
            {            
                element.transform.Find("EquipItemButton").gameObject.SetActive(true);
                element.transform.Find("UsingItemButton").gameObject.SetActive(false);
            }
        }

        // 가지고 있는 캐릭터 캐쉬 아이템 표시
        foreach (KeyValuePair<string, Item> pair in PlayerInfo.playerInfo.getDictionary(false, ItemType.Character))
        {
            element = Instantiate(Resources.Load<GameObject>("Prefabs/HavingItemElement"));
            element.transform.parent = goldItemMask.transform.Find("ScrollerBack");
            element.transform.position = goldItemMask.transform.Find("ScrollerBack").position;
            element.transform.localScale = Vector3.one;

            // 아이템 등급에 따른 배경색 설정
            element.GetComponent<Image>().color = ItemDatabase.itemInstance.SetItemClassBg(pair.Value.itemClass);

            element.transform.Find("ItemImage").GetComponent<Image>().sprite = pair.Value.itemImage;
            element.transform.Find("TipButton").GetComponent<Button>().onClick.AddListener(() => { ItemDatabase.itemInstance.ShowItemData(pair.Value); });
            element.transform.Find("ItemName").GetComponent<Text>().text = pair.Value.itemName;

            // 아이템 착용 상태 // 버튼 이미지 변경
            GameObject characterItem = element;
            element.transform.Find("EquipItemButton").GetComponent<Button>().onClick.AddListener(() => { PlayerInfo.playerInfo.SetEquipItem(pair.Value.itemType, pair.Value.itemName); });
            element.transform.Find("EquipItemButton").GetComponent<Button>().onClick.AddListener(() => { SetChangeButtonImage(ItemType.Character, characterItem); });

            if (PlayerInfo.playerInfo.GetEquipItem(ItemType.Character) == pair.Value.itemName)//이게 사용중
            {
                beforeCharacterElement = characterItem;
                element.transform.Find("EquipItemButton").gameObject.SetActive(false);
                element.transform.Find("UsingItemButton").gameObject.SetActive(true);
            }
            else
            {
                element.transform.Find("EquipItemButton").gameObject.SetActive(true);
                element.transform.Find("UsingItemButton").gameObject.SetActive(false);
            }

        }

        if (PlayerInfo.playerInfo.equipLegacy1 == "null")
        {
            beforeLegacyElement[0] = null;
        }
        if (PlayerInfo.playerInfo.equipLegacy2 == "null")
        {
            beforeLegacyElement[1] = null;
        }
        if (PlayerInfo.playerInfo.equipLegacy3 == "null")
        {
            beforeLegacyElement[2] = null;
        }


    }

    public void SetCharacterItem()
    {
        if (tabCharacterButton.sprite != Resources.Load<Sprite>("BtnImage/Btn_title_pressed"))
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

            //가지고 있는 골드 아이템
            foreach (KeyValuePair<string, Item> pair in PlayerInfo.playerInfo.getDictionary(true, ItemType.Character))
            {
                element = Instantiate(Resources.Load<GameObject>("Prefabs/HavingItemElement"));
                element.transform.parent = goldItemMask.transform.Find("ScrollerBack");
                element.transform.position = goldItemMask.transform.Find("ScrollerBack").position;
                element.transform.localScale = Vector3.one;

                // 아이템 등급에 따른 배경색 설정
                element.GetComponent<Image>().color = ItemDatabase.itemInstance.SetItemClassBg(pair.Value.itemClass);

                element.transform.Find("ItemImage").GetComponent<Image>().sprite = pair.Value.itemImage;
                element.transform.Find("TipButton").GetComponent<Button>().onClick.AddListener(() => { ItemDatabase.itemInstance.ShowItemData(pair.Value); });
                element.transform.Find("ItemName").GetComponent<Text>().text = pair.Value.itemName;

                // 아이템 착용 상태 // 버튼 이미지 변경
                GameObject characterItem = element;
                element.transform.Find("EquipItemButton").GetComponent<Button>().onClick.AddListener(() => { PlayerInfo.playerInfo.SetEquipItem(pair.Value.itemType, pair.Value.itemName); });
                element.transform.Find("EquipItemButton").GetComponent<Button>().onClick.AddListener(() => { SetChangeButtonImage(ItemType.Character, characterItem); });

                if (PlayerInfo.playerInfo.GetEquipItem(ItemType.Character) == pair.Value.itemName)//이게 사용중
                {
                    beforeCharacterElement = characterItem;
                    element.transform.Find("EquipItemButton").gameObject.SetActive(false);
                    element.transform.Find("UsingItemButton").gameObject.SetActive(true);
                }
                else
                {
                    element.transform.Find("EquipItemButton").gameObject.SetActive(true);
                    element.transform.Find("UsingItemButton").gameObject.SetActive(false);
                }

            }

            // 가지고 있는 캐릭터 캐쉬 아이템 표시
            foreach (KeyValuePair<string, Item> pair in PlayerInfo.playerInfo.getDictionary(false, ItemType.Character))
            {
                element = Instantiate(Resources.Load<GameObject>("Prefabs/HavingItemElement"));
                element.transform.parent = goldItemMask.transform.Find("ScrollerBack");
                element.transform.position = goldItemMask.transform.Find("ScrollerBack").position;
                element.transform.localScale = Vector3.one;

                // 아이템 등급에 따른 배경색 설정
                element.GetComponent<Image>().color = ItemDatabase.itemInstance.SetItemClassBg(pair.Value.itemClass);

                element.transform.Find("ItemImage").GetComponent<Image>().sprite = pair.Value.itemImage;
                element.transform.Find("TipButton").GetComponent<Button>().onClick.AddListener(() => { ItemDatabase.itemInstance.ShowItemData(pair.Value); });
                element.transform.Find("ItemName").GetComponent<Text>().text = pair.Value.itemName;

                // 아이템 착용 상태 // 버튼 이미지 변경
                GameObject characterItem = element;
                element.transform.Find("EquipItemButton").GetComponent<Button>().onClick.AddListener(() => { PlayerInfo.playerInfo.SetEquipItem(pair.Value.itemType, pair.Value.itemName); });
                element.transform.Find("EquipItemButton").GetComponent<Button>().onClick.AddListener(() => { SetChangeButtonImage(ItemType.Character, characterItem); });

                if (PlayerInfo.playerInfo.GetEquipItem(ItemType.Character) == pair.Value.itemName)//이게 사용중
                {
                    beforeCharacterElement = characterItem;
                    element.transform.Find("EquipItemButton").gameObject.SetActive(false);
                    element.transform.Find("UsingItemButton").gameObject.SetActive(true);
                }
                else
                {
                    element.transform.Find("EquipItemButton").gameObject.SetActive(true);
                    element.transform.Find("UsingItemButton").gameObject.SetActive(false);
                }

            }

        }
    }

    public void SetDiceItem()
    {
        if (tabDiceButton.sprite != Resources.Load<Sprite>("BtnImage/Btn_title_pressed"))
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

            //가지고 있는 골드 아이템
            foreach (KeyValuePair<string, Item> pair in PlayerInfo.playerInfo.getDictionary(true, ItemType.Dice))
            {
                element = Instantiate(Resources.Load<GameObject>("Prefabs/HavingItemElement"));
                element.transform.parent = goldItemMask.transform.Find("ScrollerBack");
                element.transform.position = goldItemMask.transform.Find("ScrollerBack").position;
                element.transform.localScale = Vector3.one;

                // 아이템 등급에 따른 배경색 설정
                element.GetComponent<Image>().color = ItemDatabase.itemInstance.SetItemClassBg(pair.Value.itemClass);

                element.transform.Find("ItemImage").GetComponent<Image>().sprite = pair.Value.itemImage;
                element.transform.Find("TipButton").GetComponent<Button>().onClick.AddListener(() => { ItemDatabase.itemInstance.ShowItemData(pair.Value); });
                element.transform.Find("ItemName").GetComponent<Text>().text = pair.Value.itemName;

                // 아이템 착용 상태 // 버튼 이미지 변경
                GameObject diceItem = element;
                element.transform.Find("EquipItemButton").GetComponent<Button>().onClick.AddListener(() => { PlayerInfo.playerInfo.SetEquipItem(pair.Value.itemType, pair.Value.itemName); });
                element.transform.Find("EquipItemButton").GetComponent<Button>().onClick.AddListener(() => { SetChangeButtonImage(ItemType.Dice, diceItem); });

                if (PlayerInfo.playerInfo.GetEquipItem(ItemType.Dice) == pair.Value.itemName)//이게 사용중
                {
                    beforeDiceElement = diceItem;
                    element.transform.Find("EquipItemButton").gameObject.SetActive(false);
                    element.transform.Find("UsingItemButton").gameObject.SetActive(true);
                }
                else
                {
                    element.transform.Find("EquipItemButton").gameObject.SetActive(true);
                    element.transform.Find("UsingItemButton").gameObject.SetActive(false);
                }

            }

            // 가지고 있는 캐릭터 캐쉬 아이템 표시
            foreach (KeyValuePair<string, Item> pair in PlayerInfo.playerInfo.getDictionary(false, ItemType.Dice))
            {
                element = Instantiate(Resources.Load<GameObject>("Prefabs/HavingItemElement"));
                element.transform.parent = goldItemMask.transform.Find("ScrollerBack");
                element.transform.position = goldItemMask.transform.Find("ScrollerBack").position;
                element.transform.localScale = Vector3.one;

                // 아이템 등급에 따른 배경색 설정
                element.GetComponent<Image>().color = ItemDatabase.itemInstance.SetItemClassBg(pair.Value.itemClass);

                element.transform.Find("ItemImage").GetComponent<Image>().sprite = pair.Value.itemImage;
                element.transform.Find("TipButton").GetComponent<Button>().onClick.AddListener(() => { ItemDatabase.itemInstance.ShowItemData(pair.Value); });
                element.transform.Find("ItemName").GetComponent<Text>().text = pair.Value.itemName;

                // 아이템 착용 상태 // 버튼 이미지 변경
                GameObject diceItem = element;
                element.transform.Find("EquipItemButton").GetComponent<Button>().onClick.AddListener(() => { PlayerInfo.playerInfo.SetEquipItem(pair.Value.itemType, pair.Value.itemName); });
                element.transform.Find("EquipItemButton").GetComponent<Button>().onClick.AddListener(() => { SetChangeButtonImage(ItemType.Dice, diceItem); });

                if (PlayerInfo.playerInfo.GetEquipItem(ItemType.Dice) == pair.Value.itemName)//이게 사용중
                {
                    beforeDiceElement = diceItem;
                    element.transform.Find("EquipItemButton").gameObject.SetActive(false);
                    element.transform.Find("UsingItemButton").gameObject.SetActive(true);
                }
                else
                {
                    element.transform.Find("EquipItemButton").gameObject.SetActive(true);
                    element.transform.Find("UsingItemButton").gameObject.SetActive(false);
                }

            }
        }

    }

    public void SetLegacyItem()
    {
        if (tabLegacyButton.sprite != Resources.Load<Sprite>("BtnImage/Btn_title_pressed"))
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

            //가지고 있는 골드 아이템
            foreach (KeyValuePair<string, Item> pair in PlayerInfo.playerInfo.getDictionary(true, ItemType.Legacy))
            {
                element = Instantiate(Resources.Load<GameObject>("Prefabs/HavingItemElement"));
                element.transform.parent = goldItemMask.transform.Find("ScrollerBack");
                element.transform.position = goldItemMask.transform.Find("ScrollerBack").position;
                element.transform.localScale = Vector3.one;

                // 아이템 등급에 따른 배경색 설정
                element.GetComponent<Image>().color = ItemDatabase.itemInstance.SetItemClassBg(pair.Value.itemClass);

                element.transform.Find("ItemImage").GetComponent<Image>().sprite = pair.Value.itemImage;
                element.transform.Find("TipButton").GetComponent<Button>().onClick.AddListener(() => { ItemDatabase.itemInstance.ShowItemData(pair.Value); });
                element.transform.Find("ItemName").GetComponent<Text>().text = pair.Value.itemName;

                // 아이템 착용 상태 // 버튼 이미지 변경
                GameObject legacyItem = element;
                element.transform.Find("EquipItemButton").GetComponent<Button>().onClick.AddListener(() => { PlayerInfo.playerInfo.SetEquipItem(pair.Value.itemType, pair.Value.itemName); });
                element.transform.Find("EquipItemButton").GetComponent<Button>().onClick.AddListener(() => { SetChangeButtonImage(ItemType.Legacy, legacyItem); });

                templegacy = PlayerInfo.playerInfo.GetEquipItem(ItemType.Legacy).Split(','); //사용중인 3가지 유물 이름 가져옴

                for (int i = 0; i < 3; i++)
                {
                    if (templegacy[i] == pair.Value.itemName)//이게 사용중
                    {
                        beforeLegacyElement[i] = legacyItem;
                        element.transform.Find("EquipItemButton").gameObject.SetActive(false);
                        element.transform.Find("UsingItemButton").gameObject.SetActive(true);
                    }
                    else
                    {

                        element.transform.Find("EquipItemButton").gameObject.SetActive(true);
                        element.transform.Find("UsingItemButton").gameObject.SetActive(false);
                    }
                }

                element.transform.Find("NoEquipItemButton").gameObject.SetActive(true);
                element.transform.Find("NoEquipItemButton").GetComponent<Button>().onClick.AddListener(() => { NoEquipItem(legacyItem); });
            }

            //가지고 있는 골드 아이템
            foreach (KeyValuePair<string, Item> pair in PlayerInfo.playerInfo.getDictionary(false, ItemType.Legacy))
            {
                element = Instantiate(Resources.Load<GameObject>("Prefabs/HavingItemElement"));
                element.transform.parent = cashItemMask.transform.Find("ScrollerBack");
                element.transform.position = cashItemMask.transform.Find("ScrollerBack").position;
                element.transform.localScale = Vector3.one;

                // 아이템 등급에 따른 배경색 설정
                element.GetComponent<Image>().color = ItemDatabase.itemInstance.SetItemClassBg(pair.Value.itemClass);

                element.transform.Find("ItemImage").GetComponent<Image>().sprite = pair.Value.itemImage;
                element.transform.Find("TipButton").GetComponent<Button>().onClick.AddListener(() => { ItemDatabase.itemInstance.ShowItemData(pair.Value); });
                element.transform.Find("ItemName").GetComponent<Text>().text = pair.Value.itemName;

                // 아이템 착용 상태 // 버튼 이미지 변경
                GameObject legacyItem = element;
                element.transform.Find("EquipItemButton").GetComponent<Button>().onClick.AddListener(() => { PlayerInfo.playerInfo.SetEquipItem(pair.Value.itemType, pair.Value.itemName); });
                element.transform.Find("EquipItemButton").GetComponent<Button>().onClick.AddListener(() => { SetChangeButtonImage(ItemType.Legacy, legacyItem); });

                templegacy = PlayerInfo.playerInfo.GetEquipItem(ItemType.Legacy).Split(','); //사용중인 3가지 유물 이름 가져옴

                for (int i = 0; i < 3; i++)
                {
                    if (templegacy[i] == pair.Value.itemName)//이게 사용중
                    {
                        beforeLegacyElement[i] = legacyItem;
                        element.transform.Find("EquipItemButton").gameObject.SetActive(false);
                        element.transform.Find("UsingItemButton").gameObject.SetActive(true);
                    }
                    else
                    {
                        element.transform.Find("EquipItemButton").gameObject.SetActive(true);
                        element.transform.Find("UsingItemButton").gameObject.SetActive(false);
                    }
                }

                element.transform.Find("NoEquipItemButton").gameObject.SetActive(true);
                element.transform.Find("NoEquipItemButton").GetComponent<Button>().onClick.AddListener(() => { NoEquipItem(legacyItem); });
            }
        }
    }

    void NoEquipItem(GameObject _legacy)
    {
        
        if(_legacy.transform.Find("ItemName").GetComponent<Text>().text == PlayerInfo.playerInfo.equipLegacy1)
        {
            _legacy.transform.Find("EquipItemButton").gameObject.SetActive(true);
            _legacy.transform.Find("UsingItemButton").gameObject.SetActive(false);
            beforeLegacyElement[0] = null;
            PlayerInfo.playerInfo.equipLegacy1 = "null";
        }
        else if(_legacy.transform.Find("ItemName").GetComponent<Text>().text == PlayerInfo.playerInfo.equipLegacy2)
        {
            _legacy.transform.Find("EquipItemButton").gameObject.SetActive(true);
            _legacy.transform.Find("UsingItemButton").gameObject.SetActive(false);
            beforeLegacyElement[1] = null;
            PlayerInfo.playerInfo.equipLegacy2 = "null";
        }
        else if(_legacy.transform.Find("ItemName").GetComponent<Text>().text == PlayerInfo.playerInfo.equipLegacy3)
        {
            _legacy.transform.Find("EquipItemButton").gameObject.SetActive(true);
            _legacy.transform.Find("UsingItemButton").gameObject.SetActive(false);
            beforeLegacyElement[2] = null;
            PlayerInfo.playerInfo.equipLegacy3 = "null";
           
        }
    }
}
