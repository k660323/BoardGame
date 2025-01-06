using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSettingController : MonoBehaviour
{
    [SerializeField]
    GamePanelInfo gpi;
    [SerializeField]
    public string name;

    public PanelInfo pI;


    // Start is called before the first frame update
    void Awake()
    {
        Init();
    }

    void Init()
    {
        if (gpi != null)
            gpi.panelDictionary.TryGetValue(name, out pI);
        else
            pI = new PanelInfo();
    }

}
