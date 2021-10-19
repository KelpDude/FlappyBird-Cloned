using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;       //custom files imported
using CodeMonkey.Utils; //custom files imported


public class GameManager : MonoBehaviour
{
    void Start()
    {
        Debug.Log("GameManager.Start");
        Score.Start();
    }

}
