using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    Text NickName;

    static public string nickName;
    public void Login()
    {
        NickName = GameObject.Find("NickName").transform.Find("NickNameInputField").Find("Text").GetComponent<Text>();
        if(NickName.text == null || NickName.text == "")
        {
           GameObject.Find("Canvas").transform.Find("EmptyNickNameErrorWindow").gameObject.SetActive(true);
        }
        else
        {
            nickName = NickName.text;
            LoadingSceneManager.LoadScene("MainScene");
        }

    }
}
