using System.Collections;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPun,IPunObservable
{
    public static GameManager Instance;

    [SerializeField]
    private GameObject[] life;
    [SerializeField]
    private GameObject WatchingTex=default;
    [SerializeField]
    private GameObject NetworkManager;
    private StartNet startNet;
    public int lifeCounter { get; set; }
    public bool OneDeath { get; set; } = false; //ハートがすべてない状態で死亡した場合true
    private int DeathCounter = 0;

    private void Start()
    {
        startNet = NetworkManager.GetComponent<StartNet>();
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        lifeCounter = 2;
        
    }
    void Update()
    {
        if (OneDeath)
        {
            StartCoroutine(OnCameraSpone());
            OneDeath = false;
        } 
    }

    //ハートの削除
    public void LifeDelete(PhotonView view)
    {
        if (lifeCounter >= 0)
        {
            var lifeView = life[lifeCounter].GetComponent<PhotonView>();
            lifeView.TransferOwnership(view.OwnerActorNr);
            PhotonNetwork.Destroy(life[lifeCounter]);
        }
        photonView.RPC("LifeCountSubtraction", RpcTarget.All);
    }

    //死亡回数カウント
    [PunRPC]
    private void LifeCountSubtraction()
    {
        lifeCounter--;
    }

    //Lifeがなくなったときに呼び出し
    public IEnumerator OnCameraSpone()
    {
        yield return new WaitForSeconds(3);
        if (DeathCounter == 0 && startNet.SinglePlay == false)
        {
            WatchingTex.SetActive(true);
            var player = GameObject.FindGameObjectWithTag("Player");
            var camera = player.GetComponentInParent<CameraWork>();
            camera.OnStartFollowing();
            photonView.RPC("DeathCountCheck", RpcTarget.All);
        }
        else
        {
            photonView.RPC("GameOverSceneChange", RpcTarget.All);
        }
    }

  

    //他のPlayerの状態が死亡かの判定に用いるカウント
    [PunRPC]
    private void DeathCountCheck()
    {
        DeathCounter++;
    }

    //GameOverSceneの呼び出し
    [PunRPC]
    private void GameOverSceneChange()
    {
        SoundManager.Instance.PlayBGM("GameOverBGM");
        PhotonNetwork.LoadLevel("GameOverScene");
    }

  

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(lifeCounter);
        }
        else
        {
            lifeCounter = (int)stream.ReceiveNext();
        }
    }
}
