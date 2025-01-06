using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Canvas").transform.Find("Gold").Find("Image").Find("Text").GetComponent<Text>().text = PlayerInfo.playerInfo.getGold().ToString();
        GameObject.Find("Canvas").transform.Find("Cash").Find("Image").Find("Text").GetComponent<Text>().text = PlayerInfo.playerInfo.getCash().ToString();
        GameObject.Find("Canvas").transform.Find("MyImageButton").Find("Image").Find("Text").GetComponent<Text>().text = PlayerInfo.playerInfo.playerName;

        GameObject.Find("Canvas").transform.Find("PlayerInfo").Find("PlayerRecord").GetComponent<Text>().text = "전 : "+PlayerInfo.playerInfo.total+"\n"+"승 : "+PlayerInfo.playerInfo.win+" / 패 : "+PlayerInfo.playerInfo.lose+"\n"+"무 : "+PlayerInfo.playerInfo.draw +"\n"+"승률 : ";

        SetPlayerEquipInfo();
    }

    public void SetPlayerEquipInfo()
    {
        string legacyitem = PlayerInfo.playerInfo.GetEquipItem(ItemType.Legacy);
        string[] legacy = legacyitem.Split(',');

        GameObject.Find("Canvas").transform.Find("PlayerEquipItem").Find("CharacterImage").GetComponent<Image>().sprite = Resources.Load<Sprite>("CharacterImage/"+ PlayerInfo.playerInfo.GetEquipItem(ItemType.Character));

        GameObject.Find("Canvas").transform.Find("PlayerEquipItem").Find("DiceImage").Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("DiceImage/" + PlayerInfo.playerInfo.GetEquipItem(ItemType.Dice));

        if(legacy[0] != "null")
        {
            GameObject.Find("Canvas").transform.Find("PlayerEquipItem").Find("LegacyImage0").Find("Image").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("PlayerEquipItem").Find("LegacyImage0").Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("LegacyImage/" + legacy[0]);
        }
        else
        {
            GameObject.Find("Canvas").transform.Find("PlayerEquipItem").Find("LegacyImage0").Find("Image").gameObject.SetActive(false);
        }

        if (legacy[1] != "null")
        {
            GameObject.Find("Canvas").transform.Find("PlayerEquipItem").Find("LegacyImage1").Find("Image").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("PlayerEquipItem").Find("LegacyImage1").Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("LegacyImage/" + legacy[1]);
        }
        else
        {
            GameObject.Find("Canvas").transform.Find("PlayerEquipItem").Find("LegacyImage1").Find("Image").gameObject.SetActive(false);
        }

        if (legacy[2] != "null")
        {
            GameObject.Find("Canvas").transform.Find("PlayerEquipItem").Find("LegacyImage2").Find("Image").gameObject.SetActive(true);
            GameObject.Find("Canvas").transform.Find("PlayerEquipItem").Find("LegacyImage2").Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("LegacyImage/" + legacy[2]);
        }
        else
        {
            GameObject.Find("Canvas").transform.Find("PlayerEquipItem").Find("LegacyImage2").Find("Image").gameObject.SetActive(false);
        }
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit(0);
        #endif
    }
}
