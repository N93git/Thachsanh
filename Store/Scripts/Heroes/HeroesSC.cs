using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heroes : MonoBehaviour
{
    public float health;
    public float mana;
    protected Animator animator;
    public float speed = 5.0f;
    public float rotationSpeed = 10.0f;

    public ParticleSystem attJ;
    public ParticleSystem dash;
    protected Vector3 moveDirection = Vector3.zero;
    protected bool hasAttack = false;
    public float jumpForce = 10f; // Lực dash của nhân vật
    public float rangeAttack = 3;
    public GameObject bullet;
    public int comboStep = 0;
    protected Vector3 characterDirection;
    public GameObject objsound;
    protected SoundsEffect soundsEffect;
    //Health bar
    public GameObject objhealthbar;
    protected CircularHealthBar CircularHealthBar;
    // Start is called before the first frame update

    void Start()
    {
        soundsEffect = objsound.GetComponent<SoundsEffect>();
        CircularHealthBar = objhealthbar.GetComponent<CircularHealthBar>();
        animator = GetComponent<Animator>();
        if (attJ != null)
            attJ.Stop(); // Dừng particle system khi bắt đầu
        //dash.Stop();
    }
    void PlayAttackSound()
    {
        soundsEffect.PlaySounds(0, 1f);
    }
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        if (!hasAttack)
        {
            moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
            if (moveDirection != Vector3.zero)
            {

                if (makesound)
                {
                    StartCoroutine(MakesoundWalking());
                }
                // soundsEffect.PlaySounds(2, 1f);
                RotateCharacter();
                animator.SetInteger("State", 1);
            }
            else
            {
                soundsEffect.StopSounds(2);
                makesound = true;
                if (!hasAttack)
                    animator.SetInteger("State", 0);
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && !hasAttack)
        {
            StartCoroutine(TransitionToDash());
        }

        if (Input.GetMouseButtonDown(0) && !hasAttack)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 targetPosition = hit.point;
                targetPosition.y = transform.position.y; // Giữ nguyên y để chỉ xoay xung quanh trục y

                characterDirection = targetPosition - transform.position;

                if (characterDirection != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(characterDirection);
                }

                StartCoroutine(TransitionToAttack());
            }

        }

    }
    private bool makesound = true;
    IEnumerator MakesoundWalking()
    {
        makesound = false;
        soundsEffect.PlaySounds(2, 1f);
        yield return new WaitForSeconds(4f);
        makesound = true;
        soundsEffect.StopSounds(2);
    }
    IEnumerator TransitionToDash()
    {
        soundsEffect.PlaySounds(1, 1f);
        speed = jumpForce;
        hasAttack = true;
        animator.speed = 1.8f;
        animator.SetInteger("State", 3);
        yield return new WaitForSeconds(0.6f); // Thời gian chờ 0.3 giây
        animator.speed = 1f;                            // animator.speed = 1f;
        hasAttack = false;
        speed = 5.0f;
        animator.SetInteger("State", 0);

    }
    IEnumerator TransitionToAttack()
    {
        moveDirection = new Vector3(0, 0, 0);
        comboStep += 1;
        if (comboStep == 4)
            comboStep = 1;
        // GameObject closestObject = FindClosestObjectToPosition(this.gameObject.transform.position);
        // if (closestObject != null)
        // {

        //     float distance = Vector3.Distance(this.transform.position, closestObject.transform.position);

        //     if (characterDirection != Vector3.zero && distance <= rangeAttack)
        //     {
        //         PlayAttackSound();
        //         characterDirection = closestObject.transform.position - transform.position;
        //         transform.rotation = Quaternion.LookRotation(characterDirection);


        //     }

        // }
        // else
        // {
        //     Debug.Log("Không có mục tiêu để tấn công");
        // }
        PlayAttackSound();
        var bulletScript = Instantiate(bullet, new Vector3(this.transform.position.x, this.transform.position.y + 1f, this.transform.position.z), Quaternion.identity);
        BulletSC bulletSC = bulletScript.GetComponent<BulletSC>();
        bulletSC.isMove = true;
        bulletSC.transform.rotation = Quaternion.LookRotation(characterDirection);
        hasAttack = true;
        animator.speed = 2f;
        if (comboStep == 1)
            animator.SetInteger("State", 2);
        if (comboStep == 2)
            animator.SetInteger("State", 4);
        if (comboStep == 3)
            animator.SetInteger("State", 5);
        if (attJ != null)
            attJ.Play();
        yield return new WaitForSeconds(0.4f); // Thời gian chờ 0.3 giây
        animator.speed = 1f;
        hasAttack = false;

        animator.SetInteger("State", 0);
        if (attJ != null)
            attJ.Stop(); // Dừng particle system khi bắt đầu


    }
    void FixedUpdate()
    {
        transform.Translate(moveDirection * speed * Time.fixedDeltaTime, Space.World);
    }

    void RotateCharacter()
    {
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    public GameObject FindClosestObjectToPosition(Vector3 position)
    {
        GameObject closestObject = null;
        float closestDistance = Mathf.Infinity;

        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Enemy"); // Thay "YourTag" bằng tag của object bạn muốn xem xét

        foreach (GameObject obj in allObjects)
        {
            MonsterSC monsterSC = obj.GetComponent<MonsterSC>();
            if (monsterSC.hp > 0)
            {
                float distance = Vector3.Distance(obj.transform.position, position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestObject = obj;
                }
            }

        }

        return closestObject;
    }

    public void GetDamge(float damge)
    {
        if (health > 0)
        {
            health -= damge;
            CircularHealthBar.SetHealth(health);
        }
    }

}
