using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LocalVariables : MonoBehaviourPun
{
    //現在のHP
    
    static public float currentHP = 100;
    static public int enemyHealth = 8000;
    static public float deathblow = 0;      //必殺技ゲージ

    // Use this for initialization
    void Start()
    {
        if(photonView.IsMine)
        VariableReset();
    }

    static public void VariableReset() //変数初期化
    {
        currentHP = 100;
        deathblow = 0;
    }
}
