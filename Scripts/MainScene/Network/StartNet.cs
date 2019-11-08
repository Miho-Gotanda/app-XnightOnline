using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class StartNet : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject playerCountText=default;
    [SerializeField]
    private GameObject waitPanel=default;
    [SerializeField]
    private GameObject singlePanel=default;
    private Text playerCounter;
    private byte playerCount=0;
    private NetworkManager networkManager;
    private bool starting = false;
    public bool SinglePlay { get;private set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        playerCounter = playerCountText.GetComponent<Text>();
        networkManager = GetComponent<NetworkManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤー数に応じてルームを締め切る
        if (playerCount != 0)
        {
           
            playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            playerCounter.text = playerCount + "/2";
            if (playerCount > 1)
            {
                playerCounter.text = playerCount + "/2";
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
            else
            {
                playerCounter.text = playerCount + "/2";
                if (starting == false)
                {
                    PhotonNetwork.CurrentRoom.IsOpen = true;
                }
                else if (starting == true)
                    PhotonNetwork.CurrentRoom.IsOpen = false;
            }
        }
    }

    public override void OnJoinedRoom()
    {
        playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
    }

    //プレイヤーをゲームの世界に出現させる
    IEnumerator SetPlayer(float time)
    {
        yield return new WaitForSeconds(time);
        GameObject player = PhotonNetwork.Instantiate("GloryArmor_01", new Vector3(-27.7f, 43.2f, -44.9f), Quaternion.identity, 0);

    }

    public void StartButton()
    {
        waitPanel.SetActive(false);
        if (playerCount < 2)
        {
            singlePanel.SetActive(true);
        }
        else
        {
            starting = true;
            //プレイヤーを登場させる
            networkManager.StartCoroutine("SetPlayer", 0f);
            playerCount = 0;
        }
    }

    public void SingleYes()
    {
        starting = true;
        SinglePlay = true;
        singlePanel.SetActive(false);
        //プレイヤーを登場させる
        networkManager.StartCoroutine("SetPlayer", 0f);
        playerCount = 0;
    }

    public void SingleNo()
    {
        singlePanel.SetActive(false);
        waitPanel.SetActive(true);
    }
}
