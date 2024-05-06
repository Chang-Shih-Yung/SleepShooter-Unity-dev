using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 3f;
    private Vector3 movement;
    private Animator animator;
    private Rigidbody playerRigidbody;

    //旋轉

    //攝影機射線：用來判斷玩家滑鼠的位置
    private float camRayLength = 100f;
    //打到地板的層
    private int floorMask;

    //先做初始化：聲明＋取得component
    void Awake()
    {
        animator = GetComponent<Animator>();
        playerRigidbody = GetComponent<Rigidbody>();
        //初始化取得地板的layer
        floorMask = LayerMask.GetMask("floor");
    }

    //旋轉方法
    void Turning()
    {
        //射線：從攝影機到滑鼠位置
        //主攝像機的ScreenPointToRay方法，將滑鼠的位置轉換成一條射線並從攝影機端發出指向鼠標位置（就想像你拿著雷射筆）
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        //用來存放射線打到的地板位置
        RaycastHit floorHit;
        //如果射線打到地板(不懂可以看Raycast內部需要帶入的參數，他需要：射線、目標物（引數＋out/為hit返回值）、射線長度、指定層)
        if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
        {
            Vector3 playerToMouse = floorHit.point - transform.position;
            playerToMouse.y = 0;
            //直接轉向
            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
            //將直接轉向變成平滑轉向，每秒轉360度
            Quaternion targetRotation = Quaternion.RotateTowards(transform.rotation, newRotation, 360 * Time.deltaTime);
            playerRigidbody.MoveRotation(targetRotation);
        }
    }


    //水平、垂直移動
    void Move(float h, float v)
    {
        movement.Set(h, 0f, v);//先做水平移動
        //normalized取方向，1/-1。Time.deltaTime等於一秒走某方向的六個單位:主要是要做平滑移動
        movement = movement.normalized * speed * Time.deltaTime;
        //物件位置=現在位置＋移動量
        playerRigidbody.MovePosition(transform.position + movement);

    }

    //動畫
    void Animating(float h, float v)
    {
        //如果有移動(任一前後或左右非0)，就播放走路動畫
        bool walking = h != 0f || v != 0f;
        //抓動畫編輯器裡面設定的參數IsWalking
        animator.SetBool("IsWalking", walking);
    }

    //有關物理運動在這裡更新
    void FixedUpdate()
    {
        //取得水平、垂直軸向
        //有Raw，就是值接取0/1/-1沒有中間值。指判斷上下前後左右
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        //呼叫移動
        Move(h, v);
        Turning();
        Animating(h, v);



    }
}
