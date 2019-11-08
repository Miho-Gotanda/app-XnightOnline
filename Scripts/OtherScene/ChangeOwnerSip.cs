using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ChangeOwnerSip : MonoBehaviourPun,IPunOwnershipCallbacks
{
    private GameObject[] Life;
    private PhotonView lifeVie;
    private PlayerMove playerMove;
    private int lifeCounter = 2;

    void Start()
    {
        Life = GameObject.FindGameObjectsWithTag("Life");
        playerMove = GetComponent<PlayerMove>();
    }

    void Update()
    {
        if (playerMove.GetPlayerState() == PlayerMove.PlayerState.Dead)
        {
            lifeVie = Life[lifeCounter].GetComponent<PhotonView>();
            if (photonView.IsMine)
            {
                lifeVie.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
                PhotonNetwork.Destroy(Life[lifeCounter]);
                lifeCounter++;
            }
            
        }
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        targetView.TransferOwnership(requestingPlayer);
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        throw new System.NotImplementedException();
    }
}
