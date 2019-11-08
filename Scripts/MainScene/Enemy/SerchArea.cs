using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SerchArea : MonoBehaviour
{
    private MoveEnemy moveEnemy;
    [SerializeField]
    private GameObject enemyCanavas;

    // Start is called before the first frame update
    void Start()
    {
        moveEnemy = GetComponentInParent<MoveEnemy>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            MoveEnemy.EnemyState state= moveEnemy.GetState();
            //敵が追いかける状態でなければ追いかける状態に
            if (state==MoveEnemy.EnemyState.Wait||state==MoveEnemy.EnemyState.Walk)
            {
                    moveEnemy.SetState(MoveEnemy.EnemyState.Chase, other.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            SoundManager.Instance.StopBGM();
            SoundManager.Instance.PlayBGM("WalkBGM");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (enemyCanavas.activeSelf == false)
                enemyCanavas.SetActive(true);
            SoundManager.Instance.StopBGM();
            SoundManager.Instance.PlayBGM("ChaseBGM");
        }
    }

}
