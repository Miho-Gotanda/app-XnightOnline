using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DoorManager : MonoBehaviourPunCallbacks,IPunObservable
{
    public static int DoorCount { get; set; }
    [SerializeField] private GameObject doorObj;
    private Animator animator;
    
    void Start()
    {
        animator = doorObj.GetComponent<Animator>();
    }

    void Update()
    {
        Debug.Log(DoorCount);
        if (DoorCount >= 2)
        {
            photonView.RPC("DoorAnimation", RpcTarget.All);
        }
    }

    [PunRPC]
    private void DoorAnimation()
    {
        animator.SetTrigger("DoorTrigger");
        PhotonNetwork.Destroy(this.gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(DoorCount);
        }
        else
        {
            DoorCount= (int)stream.ReceiveNext();
        }
    }

}
