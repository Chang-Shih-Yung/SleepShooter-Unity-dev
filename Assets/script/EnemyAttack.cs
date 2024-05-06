using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{

    public int attackDmage = 10;
    //玩家有沒有在可攻擊範圍內
    private bool playerInRange;

    //去抓這個腳本內的一些方法，例如傳遞傷害amount參數
    //抓到player的血量的script
    private PlayerHealth playerHealth;

    //用時間來控制攻擊頻率
    private float timer;
    private float timeBetweenAttacks = 0.5f;

    private Animator enemyAnimator;
    private bool PlayerIsDead = false;

    void Awake()
    {
        //初始化抓物件
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        //初始化抓到物件的方法
        playerHealth = player.GetComponent<PlayerHealth>();
        //在這裡取得怪物的animator
        enemyAnimator = GetComponent<Animator>();
    }

    private void playerDeathAction()
    {
        PlayerIsDead = true;

        //當角色死亡，就不跑、回到idle。動畫裡可以看到這個trigger
        enemyAnimator.SetTrigger("PlayerDead");

        //當角色死亡，就不會再跟隨角色
        GetComponent<EnemyMovement>().enabled = false;
        GetComponent<NavMeshAgent>().enabled = false; 
    }

    //當這個物件被啟用（角色是死亡的狀態），就訂閱事件
    //訂閱 PlayerDeathEvent 事件的目的是讓 playerDeathAction 方法知道角色已經死亡。也就是委託事件是傳遞訊息的功能
    //在playerHealth腳本中，在TakeDamage方法中當角色血量小於等於零，就調用Death();然後Death();裡面的PlayerDeathEvent();也就會調用，這裡就是訂閱這個事件
    //這裡是讓playerDeathAction方法在EnemyAttack腳本中訂閱了PlayerHealth腳本中的這個事件。這意味著當PlayerHealth腳本中觸發角色死亡時，playerDeathAction方法會“連帶”被調用，從而執行一系列動作，例如停止敵人的追擊和移動，並且將敵人的狀態設定為不活動。這就是訂閱PlayerDeathEvent事件的作用。
    private void OnEnable()
    {
        //訂閱事件
        PlayerHealth.PlayerDeathEvent += playerDeathAction;
    }

    //當這個物件被關閉時，就取消訂閱事件
    private void OnDisable()
    {
        //取消訂閱事件
        PlayerHealth.PlayerDeathEvent -= playerDeathAction;
    }


    //用Sphere Collider判斷是否在攻擊範圍內
    //當角色碰到怪物的sphere collider時(他有打勾trigger)，就會觸發這個方法
    private void OnTriggerEnter(Collider other)
    {
        //other就是上面引數的other，在這裡也就是player了
        //或者也可以寫直接寫other.tag == playerHealth.tag，，但比較推薦用CompareTag
        //Unity中每個物件都有其自己的tag ，在這裡就是這個意思
        if (playerHealth.CompareTag(other.tag))
        {
            playerInRange = true;
        }
    }

    //當角色沒碰到怪物的sphere collider時(他有打勾trigger)，就不會觸發這個方法
    private void OnTriggerExit(Collider other)
    {
        //或者也可以寫直接寫other.tag == playerHealth.tag，但比較推薦用CompareTag
        if (playerHealth.CompareTag(other.tag))
        {
            playerInRange = false;
        }
    }

    //觸碰到就扣血
    //多包一個攻擊方法然後下方調用，看起來比較乾淨
    private void Attack()
    {
        //調用PlayerHealth腳本的TakeDamage方法
        //對player造成傷害/傳入引數attackDmage=10
        playerHealth.TakeDamage(attackDmage);

    }

    // Update is called once per frame
    void Update()
    {
        //
        timer += Time.deltaTime;
        //如果player在攻擊範圍內，就對player造成傷害
        if (playerInRange && PlayerIsDead == false)
        {
            if (timer >= timeBetweenAttacks)
            {
                Attack();
                //每攻擊一次，時間歸零，這樣才能重算、重置下次攻擊
                timer = 0;
            }
        }
    }
}
