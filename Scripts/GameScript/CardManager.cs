using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInfo
{
    public string name;
    public Sprite image;
    public string ex;

    public CardInfo(string _name,Sprite _image,string _ex)
    {
        name = _name;
        image = _image;
        ex = _ex;
    }
}



public class CardManager : MonoBehaviour
{
    public Dictionary<int, CardInfo> CardList = new Dictionary<int, CardInfo>();

    void Start()
    {
        // 방어카드
        CardList.Add(0, new CardInfo("천사", Resources.Load<Sprite>("MagicCard/천사"), "통행료 면제 + 공격 방어"));
        CardList.Add(1, new CardInfo("방어", Resources.Load<Sprite>("MagicCard/방어"), "상대 공격 모두 방어(*사실 꽝임)"));
        CardList.Add(2, new CardInfo("무인도 탈출", Resources.Load<Sprite>("MagicCard/무인도 탈출"), "무인도 즉시 탈출"));
        CardList.Add(3, new CardInfo("할인 쿠폰", Resources.Load<Sprite>("MagicCard/할인 쿠폰"), "통행료 반값 할인~"));
        
        // 복불복
        CardList.Add(4, new CardInfo("바가지", Resources.Load<Sprite>("MagicCard/바가지"), "통행료 두배"));
        //CardList.Add(5, new CardInfo("도시기부", Resources.Load<Sprite>("MagicCard/도시기부"), "도시 기부"));
        //CardList.Add(6, new CardInfo("탈세", Resources.Load<Sprite>("MagicCard/탈세"), "세금 납부"));
        //CardList.Add(7, new CardInfo("올림픽 관광", Resources.Load<Sprite>("MagicCard/올림픽 관광"), "즉시 올림픽 관광지로"));
    }
}
