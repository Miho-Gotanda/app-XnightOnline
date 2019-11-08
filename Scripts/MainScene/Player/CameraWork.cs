using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWork : MonoBehaviour
{
    #region Private Fields
    [SerializeField]
    private float distance=7.0f; //x-z平面内のターゲットまでの距離
    [SerializeField]
    private float height = 3.0f;　//カメラをターゲット上に配置する高さ
    [SerializeField]
    private float heightSmoothLag = 0.3f; //カメラの高さに対するタイムラグ
    [SerializeField]
    private Vector3 centerOffset = Vector3.zero; //カメラを垂直方向にずらして配置
    [Tooltip("プレハブのコンポーネントがPhoton Networkによってインスタンス化される場合はこれをfalseに設定し、必要に応じて手動でOnStartFollowing（）を呼び出します")]
    [SerializeField]
    private bool followOnStart=false;
    [SerializeField]
    private float rotationSpeed=50f;
    [SerializeField]
    private float rotationSpeedY = 0.2f;
    private float velocity = 0f;
    private float maxRotateX = 45f;

    //ターゲットのトランスフォーム
    private Transform cameraTransform;
    //ターゲットが失われた場合やカメラが切り替えられた場合に再接続するためのフラグ
    private bool isFollowing;
    //現在の速度を表す。この値は、呼び出すたびにSmoothDamp（）によって変更される
    private float heightVelocity;
    //SmoothDamp（）を使用して到達しようとしている位置を表します
    private float targetHeight = 100000.0f;
    #endregion

    #region MonoBehaviour CallBacks

    void Start()
    {
        //必要に応じてターゲットを追跡し始めます
        if (followOnStart)
        {
            OnStartFollowing();
        }
    }

    void LateUpdate()
    {
        //新しいシーンをロードするたびにメインカメラが異なるコーナーのケースをカバーし、それが発生したときに再接続する必要がある
        if (cameraTransform == null && isFollowing)
        {
            OnStartFollowing();
        }
        if (isFollowing)
        {
            Apply();
            UpdateXAxisCameraRotate();
        }
    }
    #endregion

    #region Public Methods
    //追跡開始イベントを発生させます
    public void OnStartFollowing()
    {
        cameraTransform = Camera.main.transform;
        isFollowing = true;
        Cut();
    }
    #endregion

    #region Private Method
    //ターゲットを追う
    private void Apply()
    {
        Vector3 targetCenter = transform.position + centerOffset;
        //現在の回転角度と目標回転角度を計算
        float originalTargetAngle = transform.eulerAngles.y;
        float currentAngle = cameraTransform.eulerAngles.y;
        //カメラがロックされているときに実際の目標角度を調整する
        float targetAngle = originalTargetAngle;
        //currentAngle = targetAngle;
        targetHeight = targetCenter.y + height;
        
        if (Input.GetKey(KeyCode.RightArrow))
        {
            currentAngle +=rotationSpeed*Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            currentAngle -= rotationSpeed*Time.deltaTime;
        }

        float currentHeight = cameraTransform.position.y;
        currentHeight = Mathf.SmoothDamp(currentHeight, targetHeight, ref heightVelocity, heightSmoothLag);
        //角度を回転に変換し、それによってカメラを再配置
        Quaternion currentRotation = Quaternion.Euler(0, currentAngle, 0);
        //ターゲットの後ろの距離メーター
        cameraTransform.position = targetCenter;
        cameraTransform.position += currentRotation * Vector3.back * distance;


        //カメラの高さを設定
        cameraTransform.position = new Vector3(cameraTransform.position.x,currentHeight, cameraTransform.position.z);

        SetUpRotation(targetCenter);
    }

    //指定されたターゲットと中心にカメラを直接置きます。
    private void Cut()
    {
        float oldHeightSmooth = heightSmoothLag;
        heightSmoothLag = 0.001f;
        Apply();
        heightSmoothLag = oldHeightSmooth;
    }

    //常にターゲットの背後にあるようにカメラの回転を設定します
    private void SetUpRotation(Vector3 centerPos)
    {
        Vector3 cameraPos = cameraTransform.position;
        Vector3 offsetToCenter = centerPos - cameraPos;
        //y軸周りにのみベース回転を生成
        Quaternion yRotation = Quaternion.LookRotation(new Vector3(offsetToCenter.x, 0, offsetToCenter.z));
        Vector3 relativeOffset = Vector3.forward * distance + Vector3.down * height;
        cameraTransform.rotation = yRotation * Quaternion.LookRotation(relativeOffset);
    }


    //縦にカメラを操作するためのメソッド
    private void UpdateXAxisCameraRotate()
    {
        if (Input.GetKey(KeyCode.DownArrow) == true)
        {
            velocity = Mathf.Max(velocity +Time.deltaTime * rotationSpeedY, 0f);
        }
        else if (Input.GetKey(KeyCode.UpArrow) == true)
        {
            velocity = Mathf.Min(velocity - Time.deltaTime * rotationSpeedY, 0f);
        }
        else
        {
            if (velocity >= 0f)
            {
                velocity = Mathf.Max(velocity - Time.deltaTime * rotationSpeed, 0f);
            }
            else
            {
                velocity = Mathf.Min(velocity + Time.deltaTime * rotationSpeed, 0f);
            }
        }
        var angle = velocity * maxRotateX;
        var currentAngle = cameraTransform.eulerAngles;
        currentAngle.x = angle;
        cameraTransform.eulerAngles = currentAngle;
    }


    #endregion
}
