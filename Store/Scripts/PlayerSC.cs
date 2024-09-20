using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Player
{
    public float healthcurrent;
    public float PositionX, PositionY, PositionZ;
    public float rotX, rotY, rotZ;
}
public class PlayerSC : MonoBehaviour
{
    public List<Material> AoMaterials;
    public List<Renderer> Renderers;
    public float healthmax;
    public float healthcurrent;
    protected float healthregen;
    public float Damge;
    public float manamax;
    public float manacurrent;
    protected float manaregen;
    private float attackSpeed = 0.9f;
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
    //Health bar
    public GameObject objhealthbar;
    public GameObject objmanathbar;
    public GameObject enemyHealthbar;
    HealthbarEnemy healthbarEnemy;
    public Text text;
    // Start is called before the first frame update
    private bool isRegenmana = true;
    private bool isRegenhealth = true;
    private int demoPre = 0;
    // Start is called before the first frame update 
    public void LoadSave()
    {
        string json = PlayerPrefs.GetString("PlayerData");
        Debug.Log(json);
        Player loadedPlayerSC = JsonConvert.DeserializeObject<Player>(json);
        //this.transform.position = new Vector3(loadedPlayerSC.PositionX, this.transform.position.y, loadedPlayerSC.PositionZ);
        // Camera.current.transform.rotation = Quaternion.Euler(0, 0, 0);

        healthcurrent = loadedPlayerSC.healthcurrent;

    }
    void Start()
    {
        LoadSave();

        animator = GetComponent<Animator>();
        // ClothSetup();
        //OnEnableMesh();
        enemyHealthbar.gameObject.SetActive(false);
        healthbarEnemy = enemyHealthbar.GetComponent<HealthbarEnemy>();
        //
        // healthcurrent = PlayerPrefs.GetFloat("PlayerData");
        if (healthcurrent == 0)
            healthcurrent = 100;
        HealthbarSC healthbarSC = objhealthbar.GetComponent<HealthbarSC>();
        healthbarSC.heath = healthcurrent * 100 / healthmax;
        //

    }
    void OnEnableMesh()
    {

    }
    void ClothSetup()
    {
        // Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (var item in Renderers)
        {
            item.enabled = false;
        }
        // Renderers[7].material = newMaterial;
    }
    void EnableMesh(string name, int materialIndex)
    {
        foreach (var item in Renderers)
        {
            if (item.name == name)
            {
                item.enabled = true;
                item.material = AoMaterials[materialIndex];
                break;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        // PlayerPrefs.SetFloat("PlayerData", healthcurrent);
        // PlayerPrefs.Save();

        Player player = new Player
        {
            healthcurrent = healthcurrent,
            PositionX = transform.position.x,
            PositionY = transform.position.y,
            PositionZ = transform.position.z,
            rotX = transform.rotation.x,
            rotY = transform.rotation.y,
            rotZ = transform.rotation.z,
        };
        string json = JsonConvert.SerializeObject(player);
        PlayerPrefs.SetString("PlayerData", json);
        PlayerPrefs.Save(); // Lưu thay đổi vào bộ nhớ


        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        if (Input.GetKeyDown(KeyCode.K))
        {
            healthcurrent += 10;
        }
        if (!hasAttack)
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                BaseAttack();
            }
            if (!hasAttack)
                moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
            if (moveDirection != Vector3.zero)
            {

                if (!hasAttack)
                {
                    // soundsEffect.PlaySounds(2, 1f);
                    RotateCharacter();
                    animator.SetInteger("State", 1);
                    characterDirection = moveDirection;
                }
            }
            else
            {
                if (!hasAttack)
                    animator.SetInteger("State", 0);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(TransitionToDash());
            }


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
                BaseAttack();

            }

        }
    }
    GameObject closestObject;
    GameObject closestItem;
    public GameObject circle;
    float distance;
    float distanceitem;

    void FixedUpdate()
    {
        closestObject = FindClosestObjectToPosition(this.gameObject.transform.position);
        closestItem = FindClosestItemToPosition(this.gameObject.transform.position);
        if (closestObject != null)
            distance = Vector3.Distance(this.transform.position, closestObject.transform.position);
        if (closestItem != null)
            distanceitem = Vector3.Distance(this.transform.position, closestItem.transform.position);
        if (distance <= rangeAttack)
        {
            if (closestObject != null && closestObject.gameObject.tag == "Enemy")
            {


                circle.transform.position = closestObject.transform.position;

                enemyHealthbar.gameObject.SetActive(true);
                EnemiesSC hb = closestObject.GetComponent<EnemiesSC>();
                if (hb != null)
                {
                    text.text = hb.Name;
                    healthbarEnemy.heath = hb.hp * 100 / hb.hpmax;
                }
                else
                {
                    EnemySC hb2 = closestObject.GetComponent<EnemySC>();
                    if (hb2 != null)
                    {
                        text.text = hb2.Name;
                        healthbarEnemy.heath = hb2.health * 100 / hb2.healthMax;
                    }
                }
            }
        }
        else
        {
            if (distanceitem <= rangeAttack)
            {
                if (closestItem != null && closestItem.gameObject.tag == "Item")
                {

                    circle.transform.position = closestItem.transform.position;
                    enemyHealthbar.gameObject.SetActive(true);
                    ItemSC it = closestItem.GetComponent<ItemSC>();
                    if (it != null)
                    {
                        text.text = it.Name;
                        healthbarEnemy.heath = it.health;
                    }
                }
            }
            else
            {
                circle.transform.position = new Vector3(0, 198, 0);
                enemyHealthbar.gameObject.SetActive(false);
            }

        }
        transform.Translate(moveDirection * speed * Time.fixedDeltaTime, Space.World);

    }
    void RotateCharacter()
    {
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    IEnumerator TransitionToDash()
    {
        speed = jumpForce;
        hasAttack = true;
        animator.speed = 3f;
        animator.SetInteger("State", 3);
        yield return new WaitForSeconds(0.8f); // Thời gian chờ 0.3 giây
        animator.speed = 1f;                            // animator.speed = 1f;
        hasAttack = false;
        speed = 5.0f;
        animator.SetInteger("State", 0);

    }
    public virtual void BaseAttack()
    {
        StartCoroutine(TransitionToAttack());
    }
    IEnumerator TransitionToPickUp()
    {
        hasAttack = true; animator.speed = 3f;
        animator.SetInteger("State", 5);
        yield return new WaitForSeconds(1f); // Thời gian chờ 0.3 giây
        animator.speed = 1f;
        animator.SetInteger("State", 0);
        hasAttack = false;
    }
    public IEnumerator TransitionToAttack()
    {
        moveDirection = new Vector3(0, 0, 0);
        comboStep += 1;
        if (comboStep == 3)
            comboStep = 1;
        if (closestObject != null)
        {
            //Nếu có Enemy và đủ tầm
            if (characterDirection != Vector3.zero && distance <= rangeAttack)
            {
                //  PlayAttackSound();
                characterDirection = closestObject.transform.position - transform.position;
                transform.rotation = Quaternion.LookRotation(characterDirection);
                hasAttack = true;
                animator.speed = 2f;
                if (comboStep == 1)
                    animator.SetInteger("State", 2);
                if (comboStep == 2)
                    animator.SetInteger("State", 4);
                yield return new WaitForSeconds(0.2f);
                var bulletScript = Instantiate(bullet, new Vector3(this.transform.position.x, this.transform.position.y + 1f, this.transform.position.z), Quaternion.identity);
                BulletSC bulletSC = bulletScript.GetComponent<BulletSC>();
                bulletSC.isMove = true;
                bulletSC.TakeDamge = Damge;
                bulletSC.transform.rotation = Quaternion.LookRotation(characterDirection);


                // if (comboStep == 3)
                //     animator.SetInteger("State", 5);
                if (attJ != null)
                    attJ.Play();
                yield return new WaitForSeconds(attackSpeed); // Thời gian chờ 0.3 giây
                animator.speed = 1f;
                animator.SetInteger("State", 0);
                hasAttack = false;
                if (attJ != null)
                    attJ.Stop(); // Dừng particle system khi bắt đầu
            }
            else
            {
                //Truong hop item du tầm
                if (distanceitem <= rangeAttack)
                {
                    //Pick up item
                    characterDirection = closestItem.transform.position - transform.position;

                    transform.rotation = Quaternion.LookRotation(characterDirection);
                    StartCoroutine(TransitionToPickUp());
                }
                else
                {
                    Debug.Log("Không đủ  tầm tấn công");
                }
            }
        }
        else
        {
            Debug.Log("Không có mục tiêu để tấn công");
        }



    }
    public void GetDamge(float damge)
    {
        if (healthcurrent > 0)
        {
            healthcurrent -= damge;
            HealthbarSC healthbarSC = objhealthbar.GetComponent<HealthbarSC>();
            if (healthbarSC != null)
            {
                healthbarSC.TakeDamge(damge);
            }
        }
    }

    public GameObject FindClosestObjectToPosition(Vector3 position)
    {
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Enemy"); // Thay "YourTag" bằng tag của đối tượng bạn muốn tìm

        GameObject closestObject = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject obj in allObjects)
        {
            if (obj.GetComponent<MeshRenderer>() != null)
            {
                if (obj.GetComponent<MeshRenderer>().enabled)
                {
                    float distance = Vector3.Distance(obj.transform.position, position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestObject = obj;
                    }
                }
            }

        }

        return closestObject;
    }
    public GameObject FindClosestItemToPosition(Vector3 position)
    {
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("Item"); // Thay "YourTag" bằng tag của đối tượng bạn muốn tìm

        GameObject closestObject = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject obj in allObjects)
        {
            float distance = Vector3.Distance(obj.transform.position, position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = obj;
            }
        }

        return closestObject;
    }
}
