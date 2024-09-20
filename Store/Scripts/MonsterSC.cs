using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSC : MonoBehaviour
{
    public Transform player; // Tham chiếu đến player
    public float moveSpeed = 2f; // Tốc độ di chuyển của enemy  
    private GameObject originObject;
    public GameObject targetObject;
    public GameObject bullet;
    private Animator animator;
    private bool isAttack = false;
    public float rangeAttack = 2.5f;
    public HealthbarSC heathbar;
    public float hp = 100;

    //0 là idle
    //1 là forward
    //2 là attack
    //3 là back to origin
    //4 là nhan sat thuong
    //5 là die

    public int State = 0;
    void Start()
    {
        originObject = Instantiate(targetObject, this.transform.position, Quaternion.identity);
        animator = gameObject.GetComponent<Animator>();
    }
    Vector3 direction;
    void FixedUpdate()
    {
        if (hp > 0)
        {
            float distance = Vector3.Distance(this.transform.position, player.position);
            if (player != null && distance < 10 && !isAttack)
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
            if (player != null && distance <= rangeAttack && !isAttack)
            {
                // direction = player.position - transform.position + new Vector3(0, 0, 1.05f);
                // transform.rotation = Quaternion.LookRotation(direction);

                StartCoroutine(TransitionToAttack());
            }
            //Truong hop ra khỏi range Attack
            if (distance > 10 && !isAttack)
            {
                var distanceorigin = Vector3.Distance(this.transform.position, originObject.transform.position);
                State = 3;
                if (distanceorigin > 1)
                {
                    direction = originObject.transform.position - transform.position;
                    direction.Normalize();
                    transform.position += direction * moveSpeed * Time.deltaTime;
                    transform.LookAt(originObject.transform);
                    animator.SetInteger("State", 1);
                }
                else
                {
                    State = 0;
                    animator.SetInteger("State", 0);
                    this.transform.rotation = Quaternion.Euler(0, 180, 0);
                }
            }
        }
        else
        {
            StartCoroutine(TransitionToDie());

        }
    }
    IEnumerator TransitionToDie()
    {
        animator.SetInteger("State", 3);
        yield return new WaitForSeconds(2f);
        Destroy(this.gameObject);
    }
    IEnumerator TransitionToAttack()

    {
        var bulletScript = Instantiate(bullet, new Vector3(this.transform.position.x, this.transform.position.y + 1f, this.transform.position.z), Quaternion.identity);
        bulletScript.transform.forward = transform.forward;
        transform.Rotate(0, -20, 0);
        State = 2;
        isAttack = true;
        animator.speed = 2f;
        animator.SetInteger("State", 2);
        yield return new WaitForSeconds(0.5f); // Thời gian chờ 0.3 giây
        BulletSC bulletSC = bulletScript.GetComponent<BulletSC>();
        bulletSC.isMove = true;
        yield return new WaitForSeconds(1.5f); // Thời gian chờ 0.3 giây
        animator.speed = 1f;
        animator.SetInteger("State", 0);
        isAttack = false;
        transform.Rotate(0, 20, 0);
        //Tao đạn


    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "BullettPlayer")
        {
            hp -= 20;
            heathbar.TakeDamge(20);
            Debug.Log("attata");
            Destroy(collision.gameObject);
            // Do something when collision happens with an object with the tag "YourTag"
        }
    }
}
