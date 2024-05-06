using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFellow : MonoBehaviour
{
    public Transform target;
    public float smooth = 50f;
    //相機與目標的距離（差值）
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        //相機(此腳本對象) 與目標的距離（差值）：為一個固定值，等等要用來計算相機的實時位置
        offset = transform.position - target.position;
    }

    void FixedUpdate()
    {   //相機實時更新位置：主角位置+相機與目標的距離（差值）
        Vector3 targetCameraPosition = target.position + offset;
        //給相機一個線性的移動差值
        transform.position = Vector3.Lerp(transform.position, targetCameraPosition, smooth * Time.deltaTime);
    }
}
