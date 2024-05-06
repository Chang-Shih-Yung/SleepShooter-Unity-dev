using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    public GameObject[] enemies;
    public float delayTime = 1.0f;
    public float repeatRate = 3.0f;
    public Transform[] spawnPoints;
    private bool playerIsDead = false;
    private void playerDeathAction()
    {
        //假設角色死亡，執行下方 Spawmn()內的if判斷式
        playerIsDead = true;


    }
    // 判斷角色死亡或活著，就停止或生成敵人
    private void OnEnable()
    {
        PlayerHealth.PlayerDeathEvent += playerDeathAction;
    }

    private void OnDisable()
    {
        PlayerHealth.PlayerDeathEvent -= playerDeathAction;
    }
    //產生器
    private void Spawmn()
    {
        if (playerIsDead)
        {
            CancelInvoke("Spawmn");
            return;
        }

        // 隨機選擇要生成的敵人
        GameObject enemyToSpawn = enemies[Random.Range(0, enemies.Length)];

        //隨機生成點：0-3(>=0, <4)，編輯器有四個點位
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);
        //生成敵人
        Instantiate(enemyToSpawn, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
    }
    void Start()
    {
        //延遲時間，重複時間。延遲一秒生成，每三秒生成一次
        InvokeRepeating("Spawmn", delayTime, repeatRate);
    }
}
