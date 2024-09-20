using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemiesSC : MonoBehaviour
{



    public GameObject objsound;
    //SoundsEffect soundsEffect;
    public Transform player; // Tham chiếu đến player
    public float moveSpeed = 1f; // Tốc độ di chuyển của enemy  
    private GameObject originObject;
    public GameObject targetObject;
    public GameObject bullet;
    private Animator animator;
    private bool isAttack = false;
    public float delayAttack = 0f;
    public float rangeAttack = 1f;
    public HealthbarEnemy heathbar;
    public string Name;
    public float hpmax;
    public float hp = 100;
    public float Damge;
    public Collider characterCollider; // Thêm Collider của nhân vật vào Inspector
    private Rigidbody rb;
    public GameObject canvas;

    //0 là idle
    //1 là forward
    //2 là attack
    //3 là back to origin
    //4 là nhan sat thuong
    //5 là die

    public int State = 0;
    HealthbarEnemy healthbarEnemy;
    void Start()
    {
        hpmax = hp;
        //soundsEffect = objsound.GetComponent<SoundsEffect>();
        originObject = Instantiate(targetObject, this.transform.position, Quaternion.identity);
        animator = gameObject.GetComponent<Animator>();
        characterCollider = gameObject.GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        //Tạo thanh máu
        // var instan = Instantiate(canvas, this.transform.position, Quaternion.identity);
        // healthbarEnemy = instan.GetComponent<HealthbarEnemy>();
    }
    Vector3 direction;
    private Coroutine attCou;
    private Coroutine reactCou;
    private float rangeFollow = 8f;
    void FixedUpdate()
    {

        if (hp > 0)
        {
            float distance = Vector3.Distance(this.transform.position, player.position);
            if (!isAttack)
            {
                if (player != null && distance < rangeFollow)
                {
                    State = 1;
                    // Xác định hướng để di chuyển đến player
                    direction = player.position - transform.position;
                    direction.Normalize();
                    // Di chuyển enemy theo hướng đến player
                    transform.position += direction * moveSpeed * Time.deltaTime;
                    // Quay enemy để nhìn vào hướng player 
                    transform.LookAt(player);
                    animator.SetInteger("State", 1);
                }
                //Truong hop tan cong
                if (player != null && distance <= rangeAttack)
                {
                    // direction = player.position - transform.position + new Vector3(0, 0, 1.05f);
                    // transform.rotation = Quaternion.LookRotation(direction);

                    attCou = StartCoroutine(TransitionToAttack());
                }
                //Truong hop ra khỏi range Attack
                if (distance > rangeFollow)
                {
                    var distanceorigin = Vector3.Distance(this.transform.position, originObject.transform.position);
                    State = 3;
                    if (distanceorigin > 1)
                    {
                        direction = originObject.transform.position - transform.position;
                        direction.Normalize();
                        transform.position += direction * moveSpeed * Time.deltaTime;
                        transform.LookAt(originObject.transform);
                        animator.speed = animationSpeed;
                        animator.SetInteger("State", 1);
                        animator.speed = 1f;
                    }
                    else
                    {
                        State = 0;
                        animator.speed = 1f;
                        animator.SetInteger("State", 0);
                        this.transform.rotation = Quaternion.Euler(0, 180, 0);
                    }
                }
            }

        }
        else
        {
            Destroy(canvas);
            characterCollider.isTrigger = true; // Chuyển collider thành trigger 
            rb.useGravity = false;
            StartCoroutine(TransitionToDie());

        }
    }
    IEnumerator TransitionToDie()
    {
        if (reactCou != null)
            StopCoroutine(reactCou);
        animator.speed = 1f;
        animator.SetInteger("State", 4);
        yield return new WaitForSeconds(3f);
        Destroy(this.gameObject);
        Destroy(originObject);
    }
    public float animationSpeed;
    IEnumerator TransitionToAttack()

    {
        State = 2;
        isAttack = true;
        animator.speed = 2f;
        animator.SetInteger("State", 2);
        yield return new WaitForSeconds(delayAttack);
        var bulletScript = Instantiate(bullet, new Vector3(this.transform.position.x, this.transform.position.y + 1f, this.transform.position.z), Quaternion.identity);
        BulletSC bulletSC = bulletScript.GetComponent<BulletSC>();
        bulletSC.isMove = true;
        bulletSC.TakeDamge = Damge;
        bulletScript.transform.forward = transform.forward;
        yield return new WaitForSeconds(2f); // Thời gian chờ 0.3 giây
        animator.speed = 1f;
        isAttack = false;
        animator.SetInteger("State", 0);
        //Tao đạn 
    }

    public void GetDamge(float damge)
    {
        if (hp > 0)
        {
            hp -= damge;
            // heathbar.TakeDamge(damge);
            // if (isAttack == true)
            //     reactCou = StartCoroutine(TransitionToReact());
        }
    }
    IEnumerator TransitionToReact()
    {
        if (attCou != null)
            StopCoroutine(attCou);
        if (reactCou != null)
            StopCoroutine(reactCou);
        animator.SetInteger("State", 3);
        isAttack = true;
        yield return new WaitForSeconds(1f); // Thời gian chờ 0.3 giây
        isAttack = false;
        animator.SetInteger("State", 0);
    }
    // private void OnTriggerEnter(Collider other)
    // {
    //     if (other.tag == "weapon")
    //         GetDamge(2);
    // }
}
