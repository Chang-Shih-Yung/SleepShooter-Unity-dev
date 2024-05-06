using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    //初始血量(預設血量)
    public int startingHealth = 100;
    //slider會出現在編輯器內，可以拖拉slider到這個變數
    public Slider healthSlider;
    //如果轉換關卡，血量會消失掉，所以要用static靜態方式
    private static int currentHealth;


    //受傷時畫面的圖片
    public Image damageImage;
    //受傷時畫面圖片的閃爍速度
    public float flashSpeed = 3f;
    //受傷時畫面的顏色
    public Color flashColor = new Color(1f, 0f, 0f, 1f);
    private bool damage = false;

    //在記憶體中開闢空間，準備用來存放AudioSource元件
    private AudioSource playerAudio;

    //角色死亡
    public AudioClip deathClip;

    private bool isDead = false;
    private Animator playerAnimator;
    //委託:發送端
    //是一個委託類型，它在後面定義了一個可以在玩家死亡時執行的行為(EnemyAttack腳本與EnemyManager腳本中的playerDeathAction方法)
    //接收端之後會去EnemyAttack.cs裡面訂閱這個事件
    public delegate void PlayerDeathAction();
    //PlayerDeathEvent則是一個事件，它是基於PlayerDeathAction委託類型定義的。事件可以讓其他類別訂閱並在特定條件下執行。
    //用靜態則是共享作用域，可以讓其他腳本訂閱這個事件
    //委託事件本身沒有實際意義，主要是讓其他腳本連動這個事件
    public static event PlayerDeathAction PlayerDeathEvent;




    void Awake()
    {
        healthSlider.maxValue = startingHealth;

        //表示遊戲剛開始或轉換了關卡，需要將目前血量重設為初始血量，並更新血量條的顯示值。
        if (currentHealth <= 0)
        {
            healthSlider.value = startingHealth;
            //當前血量（真實血量）也要重置等於初始血量
            currentHealth = startingHealth;
        }
        else
        {
            healthSlider.value = startingHealth;
        }

        //取得player中的AudioSource元件
        playerAudio = GetComponent<AudioSource>();
        playerAnimator = GetComponent<Animator>();

    }

    private void Death()
    {
        isDead = true;
        //播放死亡動畫
        playerAudio.clip = deathClip;
        playerAudio.Play();
        playerAnimator.SetTrigger("Dead");

        GetComponent<PlayerMovement>().enabled = false;
        //在shooting腳本在player底下，所以要用GetComponentInChildren<PlayerShooting>()，不然會找不到
        GetComponentInChildren<PlayerShooting>().enabled = false;
        //設定null很重要，因為如果不設定null，那麼當玩家死亡時，PlayerDeathEvent委託將繼續保留對PlayerDeath方法的引用，這樣會導致當玩家再次死亡時，PlayerDeath方法會被調用兩次。
        if (PlayerDeathEvent != null)
        {
            PlayerDeathEvent();
        }

    }
    public void TakeDamage(int amount)
    {
        //如果玩家已經死亡，那麼不再執行TakeDamage方法
        if (isDead == true)
        {
            return;
        }

        currentHealth -= amount;
        healthSlider.value = currentHealth;
        playerAudio.Play();
        //表示有被打到
        //true表示有受傷，那對應下方if語句，如果值為真～要做啥動作，自己往下看
        damage = true;
        //如果當前血量小於等於0，那麼玩家死亡
        if (currentHealth <= 0)
        {
            Death();
        }
    }
    private void Update()
    {   //注意這裡，之前有在其他地方有被搞混的地方。
        //別被上面初始化false聲明搞混(他只是要在一開始先預設為關閉的狀態，他的值要做的事也是要對應到這裡的if語句)，我這裡就單純在update來設定一個判斷：/是否被攻擊到/，為真的話該幹啥幹啥;不為真的話該幹啥幹啥。
        if (damage)
        {
            //如果damage為真。damage被設定為false的原因是為了在每次Update 循環之後重置damage的狀態，以便在下一次受傷時再次觸發閃爍效果。
            damageImage.color = flashColor;
            damage = false;
        }
        else
        {
            //如果damage不為真，即玩家沒有受傷，則畫面顏色會逐漸變為透明，以平滑地消除閃爍效果。
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
    }
}
