using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSC : MonoBehaviour
{

    public float TakeDamge = 20f;
    public int Type;
    public GameObject objBeAttack;
    public float speed = 15f;
    public bool isMove = false;
    public float timeDestroy = 0;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TransitionToDestroy());
    }
    IEnumerator TransitionToDestroy()
    {
        yield return new WaitForSeconds(timeDestroy);
        Destroy(this.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        if (isMove)
            this.gameObject.transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Va chạm với Player
        if (Type == 1)
        {
            // Gây dame cho enemy


            if (other.gameObject.tag != "Player" && other.gameObject.tag != "BulletEnemy")
            {
                Debug.Log("Be attack " + other.name);
                if (other.gameObject.tag == "Enemy")
                {
                    EnemiesSC monsterSC = other.GetComponent<EnemiesSC>();
                    if (monsterSC != null)
                    {
                        monsterSC.GetDamge(TakeDamge);
                        if (objBeAttack != null)
                            Instantiate(objBeAttack, this.transform.position, Quaternion.identity);
                        //Destroy(this.gameObject);
                    }
                    else
                    {
                        EnemySC EnemySC = other.GetComponent<EnemySC>();
                        if (EnemySC != null)
                        {
                            EnemySC.TakeDamge(TakeDamge);
                            if (objBeAttack != null)
                                Instantiate(objBeAttack, this.transform.position, Quaternion.identity);
                        }
                    }
                }

            }
        }
        if (Type == 2)
        {
            if (other.gameObject.tag != "Enemy" && other.gameObject.tag != "PlayerBullet")
            {
                PlayerSC PlayerSc = other.GetComponent<PlayerSC>();
                if (PlayerSc != null)
                {

                    PlayerSc.GetDamge(TakeDamge);
                    Destroy(this.gameObject);
                }
                if (objBeAttack != null)
                    Instantiate(objBeAttack, this.transform.position, Quaternion.identity);

            }

        }
    }


}
