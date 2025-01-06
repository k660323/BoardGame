using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PanelInfo : MonoBehaviour
{
    public string owner;
    public int Line;
    public int group;
    public string name;
    public int price;
    public int[] flagValue = new int[4];
    public bool festiver;
    public Sprite image;

    public PanelInfo()
    {
        owner =null;
        Line = 0;
        group = 0;
        name = null;
        price =0;
        flagValue = new int[4] { 0, 0, 0, 0 };
        festiver = false;
        image = null;
    }

    public PanelInfo(string _owner,int _Line,int _group,string _name, int _price,int _flagValue0, int _flagValue1, int _flagValue2, int _flagValue3, Sprite _image)
    {
        owner = _owner;
        Line = _Line;
        group = _group;
        price = _price;
        name = _name;

        flagValue[0] = _flagValue0;
        flagValue[1] = _flagValue1;
        flagValue[2] = _flagValue2;
        flagValue[3] = _flagValue3;

        image = _image;

        festiver = false;
    }


}



public class GamePanelInfo : MonoBehaviourPunCallbacks
{
    [SerializeField]
    public Dictionary<string, PanelInfo> panelDictionary = new Dictionary<string, PanelInfo>();
    


    private void Awake()
    {
        Init();
    }

    void Init()
    {
        // 특수 지역
        panelDictionary.Add("캠프", new PanelInfo("",1,-1,"캠프", 0, 0, 0, 0, 0, Resources.Load<Sprite>("GamePanelImage/캠프")));
        panelDictionary.Add("휴식처", new PanelInfo("", 2, -1 ,"휴식처", 0, 0, 0, 0, 0, Resources.Load<Sprite>("GamePanelImage/휴식처")));
        panelDictionary.Add("축제의 등불", new PanelInfo("", 3, -1, "축제의 등불", 0, 0, 0, 0, 0, Resources.Load<Sprite>("GamePanelImage/축제의 등불")));
        panelDictionary.Add("마인카트", new PanelInfo("", 4, -1, "마인카트", 0, 0, 0, 0, 0, Resources.Load<Sprite>("GamePanelImage/마인카트")));       
        
        panelDictionary.Add("마법카드", new PanelInfo("", 0, -1, "마법카드", 0, 0, 0, 0, 0, Resources.Load<Sprite>("GamePanelImage/마법카드")));
        panelDictionary.Add("폭포수", new PanelInfo("", 2, -1, "폭포수", 0, 0, 0, 0, 0, Resources.Load<Sprite>("GamePanelImage/폭포수")));
        panelDictionary.Add("보물창고", new PanelInfo("", 4, -1, "보물창고", 0, 0, 0, 0, 0, Resources.Load<Sprite>("GamePanelImage/보물창고")));
        
        //1번 라인

        panelDictionary.Add("목제장", new PanelInfo("", 1, 1, "목제장", 300000,200000,350000,400000,350000, Resources.Load<Sprite>("GamePanelImage/목제장")));

        panelDictionary.Add("유물", new PanelInfo("", 1, 0, "유물", 500000,0,0,0,0, Resources.Load<Sprite>("GamePanelImage/유물")));
        panelDictionary.Add("오두막집", new PanelInfo("", 1, 1, "오두막집", 350000,150000,400000,400000,450000, Resources.Load<Sprite>("GamePanelImage/오두막집")));
        panelDictionary.Add("토마토 농장", new PanelInfo("", 1, 2, "토마토 농장", 200000,100000,250000,300000,250000, Resources.Load<Sprite>("GamePanelImage/토마토 농장")));
        panelDictionary.Add("비트 농장", new PanelInfo("", 1, 2, "비트 농장", 250000,150000,350000,300000,300000, Resources.Load<Sprite>("GamePanelImage/비트 농장")));
        panelDictionary.Add("배추 농장", new PanelInfo("", 1, 2, "배추 농장", 250000,250000,250000,300000,450000, Resources.Load<Sprite>("GamePanelImage/배추 농장")));
        panelDictionary.Add("허수아비", new PanelInfo("", 1, 3, "허수아비", 250000,100000,300000,350000,400000, Resources.Load<Sprite>("GamePanelImage/허수아비")));
        panelDictionary.Add("목제 작업장", new PanelInfo("", 1, 3, "목제 작업장", 300000,150000,450000,300000,350000, Resources.Load<Sprite>("GamePanelImage/목제 작업장")));


        //2번 라인

        panelDictionary.Add("물통", new PanelInfo("", 2, 4, "물통", 50000,150000,300000,250000,200000, Resources.Load<Sprite>("GamePanelImage/물통")));
        panelDictionary.Add("암벽", new PanelInfo("", 2, 4, "암벽", 200000,100000,250000,300000,250000, Resources.Load<Sprite>("GamePanelImage/암벽")));
        panelDictionary.Add("나무", new PanelInfo("", 2, 5, "나무", 250000,140000,70000,150000,350000, Resources.Load<Sprite>("GamePanelImage/나무")));
        panelDictionary.Add("나무2", new PanelInfo("", 2, 5, "나무2", 280000, 80000, 300000, 150000, 350000, Resources.Load<Sprite>("GamePanelImage/나무2")));
        panelDictionary.Add("물레방아", new PanelInfo("", 2, 6, "물레방아", 250000,200000,320000,350000,500000, Resources.Load<Sprite>("GamePanelImage/물레방아")));
        panelDictionary.Add("유물2", new PanelInfo("", 2, 0, "유물2", 500000, 0, 0, 0, 0, Resources.Load<Sprite>("GamePanelImage/유물")));
        panelDictionary.Add("우물", new PanelInfo("", 2, 6, "우물", 250000,150000,300000,450000,620000, Resources.Load<Sprite>("GamePanelImage/우물")));
        panelDictionary.Add("대장간", new PanelInfo("", 2, 6, "대장간", 400000,100000,400000,450000,650000, Resources.Load<Sprite>("GamePanelImage/대장간")));


        //3번 라인
        panelDictionary.Add("풍차", new PanelInfo("", 3, 7, "풍차", 300000,250000,500000,300000,250000, Resources.Load<Sprite>("GamePanelImage/풍차")));
        
        panelDictionary.Add("펜션", new PanelInfo("", 3, 7, "펜션", 550000,350000,400000,500000,500000, Resources.Load<Sprite>("GamePanelImage/펜션")));
        panelDictionary.Add("에임 연습장", new PanelInfo("", 3, 8, "에임 연습장", 400000,200000,400000,550000,450000, Resources.Load<Sprite>("GamePanelImage/에임 연습장")));
        panelDictionary.Add("장작불", new PanelInfo("", 3, 8, "장작불", 400000,450000,500000,600000,650000, Resources.Load<Sprite>("GamePanelImage/장작불")));
        panelDictionary.Add("유물3", new PanelInfo("", 3, 0, "유물3", 500000, 0, 0, 0, 0, Resources.Load<Sprite>("GamePanelImage/유물")));
        panelDictionary.Add("버섯집", new PanelInfo("", 3, 9, "버섯집", 550000, 400000,620000,600000,400000, Resources.Load<Sprite>("GamePanelImage/버섯집")));
        panelDictionary.Add("풀숲", new PanelInfo("", 3, 9, "풀숲", 580000,400000,580000,650000,600000, Resources.Load<Sprite>("GamePanelImage/풀숲")));
        panelDictionary.Add("채굴장", new PanelInfo("", 3, 9, "채굴장", 650000,600000,700000,750000,750000, Resources.Load<Sprite>("GamePanelImage/채굴장")));

        //4번 라인
        
        panelDictionary.Add("버섯풀숲", new PanelInfo("", 4, 10, "버섯풀숲", 420000, 650000,400000,550000,480000, Resources.Load<Sprite>("GamePanelImage/버섯풀숲")));
        panelDictionary.Add("공동묘지", new PanelInfo("", 4, 10, "공동묘지", 400000,300000,600000,700000,600000, Resources.Load<Sprite>("GamePanelImage/공동묘지")));
        panelDictionary.Add("죽어버린 나무", new PanelInfo("", 4, 11, "죽어버린 나무", 500000,500000,500000,800000,610000, Resources.Load<Sprite>("GamePanelImage/죽어버린 나무")));
        panelDictionary.Add("둠피 무덤", new PanelInfo("", 4, 11, "둠피 무덤", 440000,350000,560000,900000,700000, Resources.Load<Sprite>("GamePanelImage/둠피 무덤")));
        
        panelDictionary.Add("버러진 집", new PanelInfo("", 4, 12, "버러진 집", 350000,300000,450000,400000,550000, Resources.Load<Sprite>("GamePanelImage/버러진 집")));
        panelDictionary.Add("유물4", new PanelInfo("", 4, 0, "유물4", 500000, 0, 0, 0, 0, Resources.Load<Sprite>("GamePanelImage/유물")));
        panelDictionary.Add("요새", new PanelInfo("", 4, 12, "요새", 420000,430000,700000,750000,600000, Resources.Load<Sprite>("GamePanelImage/요새")));
        panelDictionary.Add("주막집", new PanelInfo("", 4, 12, "주막집", 400000,500000,520000,720000,300000, Resources.Load<Sprite>("GamePanelImage/주막집")));

        var pN = panelDictionary.OrderBy(Item => Item.Key);

    }

    public void DebugLog()
    {
        int count = 0;
        foreach(KeyValuePair<string, PanelInfo> pair in panelDictionary)
        {
            Debug.Log(++count);
            Debug.Log(pair.Key);
            Debug.Log(pair.Value.owner);
        }       
    }
}
