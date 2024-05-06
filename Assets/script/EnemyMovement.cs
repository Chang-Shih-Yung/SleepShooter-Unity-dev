using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    //用private也可以，因為下面用tag找到player。那編輯器就不會顯示player變數
    private Transform player;
    private NavMeshAgent nav;
    void Awake()
    {
        //跟隨角色，先用tag找角色元件
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nav = GetComponent<NavMeshAgent>();
    }
   

    void Update()
    {
        //抓到player的位置＆設定目的地
        nav.SetDestination(player.position);
    }
}
