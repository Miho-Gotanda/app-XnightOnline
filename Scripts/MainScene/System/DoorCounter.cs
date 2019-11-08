using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DoorCounter : MonoBehaviourPunCallbacks
{
    private int DoorCount;
    private BoxCollider boxCollider=null;
    private bool doorCounterTrigger = false;

    private void Update()
    {
        //プレイヤー死亡時Trigger解除
        if (photonView.IsMine) {
            if (LocalVariables.currentHP <= 0){
                DoorBoolReset();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine&&other.gameObject.tag == "DoorCollider")
        {
            boxCollider = other.gameObject.GetComponent<BoxCollider>();
        }
        if (other.gameObject.tag == "DoorSecond" && !doorCounterTrigger)
        {
            if (photonView.IsMine)
            {
                boxCollider.isTrigger = false;
                doorCounterTrigger = true;
            }
            
        }   
    }

    public void DoorBoolReset()
    {
        boxCollider.isTrigger = true;
        doorCounterTrigger = false;
    }
}
