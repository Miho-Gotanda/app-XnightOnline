using UnityEngine;
using System.Collections;

public class SetPosition : MonoBehaviour
{

    //　初期位置
    private Vector3 startPosition;
    //　目的地
    private Vector3 destination;
    //　巡回する位置
    [SerializeField]
    private Transform[] patrolPositions=default;
    //　次に巡回する位置
    private int nowPos = 0;

    void Start()
    {
        //　初期位置を設定
        startPosition = transform.position;
        //　巡回地点を設定
        var patrolParent = GameObject.Find("EnemyPosition");
        for (int i = 0; i < patrolParent.transform.childCount; i++)
        {
            patrolPositions[i] = patrolParent.transform.GetChild(i);
        }
    }

    //　巡回地点を順に周る
    public void NextPosition()
    {
        SetDestination(patrolPositions[nowPos].position);
        nowPos=Random.Range(0,4);
        if (nowPos >= patrolPositions.Length)
        {
            nowPos = 0;
        }
    }

    //　目的地を設定する
    public void SetDestination(Vector3 position)
    {
        destination = position;
    }

    //　目的地を取得する
    public Vector3 GetDestination()
    {
        NextPosition();
        return destination;
    }
}
