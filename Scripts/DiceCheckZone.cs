using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCheckZone : MonoBehaviour
{
    Vector3[] diceVelocity;
    public GameObject[] dice;

    public int[] dicenum;
    public bool[] isDice;

    private void Start()
    {
        diceVelocity = new Vector3[2];
        dicenum = new int[2];
        isDice = new bool[2];

        dicenum[0] = 0;
        dicenum[1] = 0;
        isDice[0] = false;
        isDice[1] = false;
    }

    public void InitDiceZone()
    {
        dicenum[0] = 0;
        dicenum[1] = 0;
        isDice[0] = true;
        isDice[1] = true;
    }


    private void FixedUpdate()
    {
        diceVelocity[0] = dice[0].GetComponent<Rigidbody>().velocity;
        diceVelocity[1] = dice[1].GetComponent<Rigidbody>().velocity;
    }

    private void OnTriggerStay(Collider col)
    {
         if (isDice[0])
         {
             if (diceVelocity[0].x == 0f && diceVelocity[0].y == 0f && diceVelocity[0].z == 0f)
             {
                 switch (col.gameObject.name)
                 {
                     case "Side1":
                         col.gameObject.SetActive(false);
                         isDice[0] = false;
                         dicenum[0] = 6;
                         if (++GameController.dicecount == 2) GameObject.Find("GameController").GetComponent<GameController>().DiceMove(dicenum,5.0f);
                         break;
                     case "Side2":
                         col.gameObject.SetActive(false);
                         isDice[0] = false;
                         dicenum[0] = 5;
                         if (++GameController.dicecount == 2) GameObject.Find("GameController").GetComponent<GameController>().DiceMove(dicenum, 5.0f);
                         break;
                     case "Side3":
                         col.gameObject.SetActive(false);
                         isDice[0] = false;
                         dicenum[0] = 4;
                         if (++GameController.dicecount == 2) GameObject.Find("GameController").GetComponent<GameController>().DiceMove(dicenum, 5.0f);
                         break;
                     case "Side4":
                         col.gameObject.SetActive(false);
                         isDice[0] = false;
                         dicenum[0] = 3;
                         if (++GameController.dicecount == 2) GameObject.Find("GameController").GetComponent<GameController>().DiceMove(dicenum, 5.0f);
                         break;
                     case "Side5":
                         col.gameObject.SetActive(false);
                         isDice[0] = false;
                         dicenum[0] = 2;
                         if (++GameController.dicecount == 2) GameObject.Find("GameController").GetComponent<GameController>().DiceMove(dicenum, 5.0f);
                         break;
                     case "Side6":
                         col.gameObject.SetActive(false);
                         isDice[0] = false;
                         dicenum[0] = 1;
                         if (++GameController.dicecount == 2) GameObject.Find("GameController").GetComponent<GameController>().DiceMove(dicenum, 5.0f);
                         break;
                 }             
             }     
         }

         if (isDice[1])
         {
             if (diceVelocity[1].x == 0f && diceVelocity[1].y == 0f && diceVelocity[1].z == 0f)
             {
                 switch (col.gameObject.name)
                 {
                     case "Side1":
                         col.gameObject.SetActive(false);
                         isDice[1] = false;
                         dicenum[1] = 6;
                         if (++GameController.dicecount == 2) GameObject.Find("GameController").GetComponent<GameController>().DiceMove(dicenum, 5.0f);
                         break;
                     case "Side2":
                         col.gameObject.SetActive(false);
                         isDice[1] = false;
                         dicenum[1] = 5;
                         if (++GameController.dicecount == 2) GameObject.Find("GameController").GetComponent<GameController>().DiceMove(dicenum, 5.0f);
                         break;
                     case "Side3":
                         col.gameObject.SetActive(false);
                         isDice[1] = false;
                         dicenum[1] = 4;
                         if (++GameController.dicecount == 2) GameObject.Find("GameController").GetComponent<GameController>().DiceMove(dicenum, 5.0f);
                         break;
                     case "Side4":
                         col.gameObject.SetActive(false);
                         isDice[1] = false;
                         dicenum[1] = 3;
                         if (++GameController.dicecount == 2) GameObject.Find("GameController").GetComponent<GameController>().DiceMove(dicenum, 5.0f);
                         break;
                     case "Side5":
                         col.gameObject.SetActive(false);
                         isDice[1] = false;
                         dicenum[1] = 2;
                         if (++GameController.dicecount == 2) GameObject.Find("GameController").GetComponent<GameController>().DiceMove(dicenum, 5.0f);
                         break;
                     case "Side6":
                         col.gameObject.SetActive(false);
                         isDice[1] = false;
                         dicenum[1] = 1;
                         if (++GameController.dicecount == 2) GameObject.Find("GameController").GetComponent<GameController>().DiceMove(dicenum, 5.0f);
                         break;
                 }
             }
         }  
    }
}
