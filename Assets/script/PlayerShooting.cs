using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerShooting : MonoBehaviour
{
    //破壞力
    public int damagePerShot = 20;
    //射擊距離
    public float range = 100f;
    //射線
    private Ray shootRay;
    //射線打到的地方
    private RaycastHit shootHit;
    //能被射到的層：敵人
    private int shootableMask;
    // private int shootableMask1;
    //射擊燈光
    private Light gunLight;
    //射擊粒子
    private ParticleSystem gunParticle;
    //射擊聲音
    private AudioSource gunAudio;
    private LineRenderer gunLine;

    //發射頻率
    public float timeBetweenBullets = 0.15f;
    //特效顯示多少時間
    private float effectsDisplayTime = 0.2f;
    private float timer;


    void Awake()
    {
        //敵人的層
        shootableMask = LayerMask.GetMask("enemy");
        //環境的層
        // shootableMask1 = LayerMask.GetMask("environment");
        //去得物件的component（功能元件）
        gunParticle = GetComponent<ParticleSystem>();
        gunLight = GetComponent<Light>();
        gunAudio = GetComponent<AudioSource>();
        gunLine = GetComponent<LineRenderer>();

    }
    private void Shoot()
    {
        timer = 0f;
        gunAudio.Play();
        gunLight.enabled = true;
        //先停止粒子系統
        gunParticle.Stop();
        gunParticle.Play();

        gunLine.enabled = true;

        //設定射線原起始位置：用的是transform不是Vector3（因為角色一直動，是針對角色的動態位置，所以不可以用vector3世界座標系統）
        //編號零
        gunLine.SetPosition(0, transform.position);
        //抓到原點
        shootRay.origin = transform.position;
        //抓到方向
        shootRay.direction = transform.forward;

        //發射射線到敵人的mask
        //gunLine位置編號1
        if (Physics.Raycast(shootRay, out shootHit, range, shootableMask))
        {
            //如果打到敵人(shootHit打到的對象下面的Component)，就去敵人的血量腳本中的TakeDamage方法，傳遞傷害參數
            EnemyHealth enemyHealth = shootHit.collider.GetComponent<EnemyHealth>();
            //看EnemyHealth腳本的TakeDamage方法，他需要兩個參數：傷害、打到的位置
            enemyHealth.TakeDamage(damagePerShot, shootHit.point);

            //如果打到敵人，那gunLine位置就是到敵人的位置
            gunLine.SetPosition(1, shootHit.point);

        }
        else
        {
            //如果沒打到敵人，那gunLine位置就是算原點到最遠位置（方向x距離）
            //不過我上面有把環境的層也加進去，所以如果打到環境，那gunLine位置最遠就是到環境的位置
            gunLine.SetPosition(1, shootRay.GetPoint(range) - shootRay.origin);

        }

        // //發射射線到環境的mask，但是加上去有點卡，所以先註解掉
        // if (Physics.Raycast(shootRay, out shootHit, range, shootableMask1))
        // {

        //     //如果打到敵人，那gunLine位置就是到敵人的位置
        //     gunLine.SetPosition(1, shootHit.point);

        // }
        // else
        // {
        //     //如果沒打到敵人，那gunLine位置就是算原點到最遠位置（方向x距離）
        //     //不過我上面有把環境的層也加進去，所以如果打到環境，那gunLine位置最遠就是到環境的位置
        //     gunLine.SetPosition(1, shootRay.GetPoint(range) - shootRay.origin);

        // }


    }
    //關閉特效
    void DisableEffect()
    {
        gunLight.enabled = false;
        gunLine.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {   //計時器:累加的時間
        timer += Time.deltaTime;
        //按下滑鼠左鍵，可以去project-setting-inputs看到Fire1的設定
        //timer >= timeBetweenBullets（0.15f）是為了確保射擊效果的頻率符合預期
        if (Input.GetButtonDown("Fire1") && timer >= timeBetweenBullets)
        {
            //觸發射擊的效果(每間隔0.15f可以打一發子彈)
            Shoot();
        }
        //如果時間大於特效顯示時間（0.03f），就關閉特效
        //timeBetweenBullets * effectsDisplayTime就是特效顯示完整持續的總時間（動態乘積）
        //為啥運算式是相乘？我還是不知道，但是這樣寫就對了
        if (timer >= timeBetweenBullets * effectsDisplayTime)
        {
            DisableEffect();
        }

    }
}
