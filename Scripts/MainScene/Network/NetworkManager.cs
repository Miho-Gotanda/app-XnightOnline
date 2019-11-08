using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Text informationText=default;

    [SerializeField]
    private GameObject loginUI=default;

    //部屋の名前
    [SerializeField]
    private InputField roomName=default;

    //ログアウトボタン
    [SerializeField]
    private GameObject logoutButton=default;

    [SerializeField]
    private GameObject reStartTextObj;

    //プレイヤーのインスタンス
    private GameObject player;

    //ReStartパネル参照
    [HideInInspector]
    public GameObject reStartPanel;

    [SerializeField]
    private GameObject waitPanel=default;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    // Update is called once per frame
    void Update()
    {
        informationText.text = PhotonNetwork.NetworkClientState.ToString();
    }

    public override void OnConnectedToMaster()
    {
         Debug.Log("ロビーに入る");
        loginUI.SetActive(true);
    }

    public override void OnJoinedLobby()
    {
        loginUI.SetActive(true);
    }

    //　ログインボタンを押した時に実行するメソッド
    public void LoginGame()
    {
        
        RoomOptions ro = new RoomOptions()
        {
            
            // 部屋の最大人数
            MaxPlayers = 2
        };
        if (roomName.text != "")
        {
            //部屋がない場合は作って入室
            PhotonNetwork.JoinOrCreateRoom(roomName.text, ro, TypedLobby.Default);
        }
        else
        {
            PhotonNetwork.JoinOrCreateRoom("DefaultRoom", ro, TypedLobby.Default);
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        RoomOptions ro = new RoomOptions()
        {
            //ルームを見えるようにする
            IsVisible = true,
            // 部屋の最大人数
            MaxPlayers = 2
        };
        var count = 1;
        PhotonNetwork.JoinOrCreateRoom(count.ToString() + "Room", ro, TypedLobby.Default);
        count++;
    }

    //ルーム入室後
    public override void OnJoinedRoom()
    {
        Debug.Log(roomName.text);
        loginUI.SetActive(false);
        logoutButton.SetActive(true);
        waitPanel.SetActive(true);    
    }

    //ログアウトボタンを押したときの処理
    public void LogoutGame()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("TitleScene");
    }


    //プレイヤーをゲームの世界に出現させる
    IEnumerator SetPlayer(float time)
    {
        yield return new WaitForSeconds(time);
        reStartTextObj.SetActive(false);
        player = PhotonNetwork.Instantiate("Pfb_GloryArmor_01", new Vector3(Random.Range(-68.3f,-69.9f), 4.5f, 134f), Quaternion.identity, 0);

    }

    //Player死亡時呼び出し
    public void Restart()
    {
        reStartTextObj.SetActive(true);
        StartCoroutine(SetPlayer(6));
    }

}
