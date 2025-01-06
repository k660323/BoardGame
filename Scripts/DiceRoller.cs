using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRoller : MonoBehaviourPunCallbacks, IPunObservable
{
    PhotonView PV;
    Rigidbody rb;
    public Vector3 diceVelocitry;
    Vector3 DestPos;
    Quaternion DestRot;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            DestPos = (Vector3)stream.ReceiveNext();
            DestRot = (Quaternion)stream.ReceiveNext();
        }
    }

    void Start()
    {
        PV = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        for (int i = 0; i < 6; i++)
            gameObject.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);

    }

    void Update()
    {
        if(!PV.IsMine)
        {
            float distance = (DestPos - transform.position).sqrMagnitude;
            if(distance > 50)
            {
                transform.position = DestPos;
                transform.rotation = DestRot;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, DestPos, 50f * Time.deltaTime);
                float t = Mathf.Clamp(20f * Time.deltaTime, 0f, 0.99999f);
                transform.rotation = Quaternion.Lerp(transform.rotation, DestRot, t);
            }
        }
    }

    [PunRPC]
    void SetDice(string diceMaterial)
    {
        gameObject.transform.Find("pCube").GetComponent<MeshRenderer>().material = Resources.Load<Material>("Material/" + diceMaterial);
        switch (diceMaterial)
        {
            case "OddDiceMaterial":
                gameObject.transform.Find("pCube").GetChild(0).name = "Side6"; //side1 을 side6  6 -> 1
                gameObject.transform.Find("pCube").GetChild(1).name = "Side2"; // 5
                gameObject.transform.Find("pCube").GetChild(2).name = "Side4"; //side3 을 side4  4 -> 3
                gameObject.transform.Find("pCube").GetChild(3).name = "Side4"; // 3
                gameObject.transform.Find("pCube").GetChild(4).name = "Side2"; //side5 을 side2  2 -> 5
                gameObject.transform.Find("pCube").GetChild(5).name = "Side6"; // 1
                break;
            case "EvenDiceMaterial":
                gameObject.transform.Find("pCube").GetChild(0).name = "Side1"; // 6
                gameObject.transform.Find("pCube").GetChild(1).name = "Side5"; // side2 을 side5 5 -> 2
                gameObject.transform.Find("pCube").GetChild(2).name = "Side3"; // 4
                gameObject.transform.Find("pCube").GetChild(3).name = "Side3"; // side4 을 side3 3 -> 4
                gameObject.transform.Find("pCube").GetChild(4).name = "Side5"; // 2
                gameObject.transform.Find("pCube").GetChild(5).name = "Side1"; // side6 을 side1 1 -> 6
                break;
            case "NormalDiceMaterial":
                gameObject.transform.Find("pCube").GetChild(0).name = "Side1";
                gameObject.transform.Find("pCube").GetChild(1).name = "Side2";
                gameObject.transform.Find("pCube").GetChild(2).name = "Side3";
                gameObject.transform.Find("pCube").GetChild(3).name = "Side4";
                gameObject.transform.Find("pCube").GetChild(4).name = "Side5";
                gameObject.transform.Find("pCube").GetChild(5).name = "Side6";
                break;
        }
    }

    public void DiceRoll(string diceMaterial)
    {
        if (gameObject.transform.Find("pCube").GetComponent<MeshRenderer>().material.name != diceMaterial)
        {
            PV.RPC("SetDice", RpcTarget.All, diceMaterial);
        }

        float dirX = Random.Range(0, 500);
        float dirY = Random.Range(0, 500);
        float dirZ = Random.Range(0, 500);

        transform.position = new Vector3(10, 2, 10);
        transform.rotation = Quaternion.identity;

        for (int i = 0; i < 6; i++)
            gameObject.transform.GetChild(0).GetChild(i).gameObject.SetActive(true);

        rb.AddForce(transform.up * 500);
        rb.AddTorque(dirX, dirY, dirZ);
    }   
}
