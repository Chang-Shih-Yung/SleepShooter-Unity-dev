using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    public int startHealth = 100;
    private int currentHealth;
    private bool isDead;
    private Animator anim;

    private bool IsSinking = false;

    //存儲音效檔案的地方 
    public AudioClip deadClip;
    // 控制播放敵人死亡音效
    private AudioSource enemyAudio;
    private ParticleSystem hitParticles;
    //死亡粒子
    private ParticleSystem deathParticles;

    //分數變化
    public int score = 10;



    void Awake()
    {
        anim = GetComponent<Animator>();
        //初始化先把當前血量設置為滿血量
        currentHealth = startHealth;
        enemyAudio = GetComponent<AudioSource>();
        hitParticles = GetComponentInChildren<ParticleSystem>();
        // 抓取父物體的第三個子物件
        if (transform.childCount >= 3)
        {
            deathParticles = transform.GetChild(2).GetComponent<ParticleSystem>();
        }
    }

    public void Death()
    {
        //怪物死亡
        isDead = true;
        //播放死亡動畫
        anim.SetTrigger("IsDead");
        //播放死亡音效（先取得音效檔案控制性、關聯性，然後再播放）
        enemyAudio.clip = deadClip;
        enemyAudio.Play();
        //死亡就關掉，效能考量
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<EnemyMovement>().enabled = false;
        GetComponent<EnemyAttack>().enabled = false;

        //關閉碰撞器，不然怪物死亡後還會被打到
        GetComponent<SphereCollider>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;



        //分數變化
        ScoreManager.score += score;


    }
    //Vector3 position是要傳入角色雷射槍打到敵人的點位置
    public void TakeDamage(int amount, Vector3 hitPosition)
    {
        //如果怪物已經死亡，就不再執行下面的代碼
        if (isDead) return;

        //扣血
        currentHealth -= amount;
        enemyAudio.Play();
        //雷射槍打到敵人的點位置、要播放粒子特效
        hitParticles.transform.position = hitPosition;
        hitParticles.Play();

        //如果血量小於等於0，就死亡
        if (currentHealth <= 0)
        {
            Death();

        }
    }

    public void StartSinking()
    {
        //當有下沉，觸發下方條件語句：開始下沉
        //這裡是到編輯器裡去觸發，所以是public。
        IsSinking = true;
        Destroy(gameObject, 2f);
        
        if (deathParticles != null)
        {
            deathParticles.Play();
        }
    }
    void Update()
    {
        //判斷是否要下沉
        if (IsSinking)
        {
            //位移
            transform.Translate(-Vector3.up * 0.5f * Time.deltaTime);
        }
    }
}
