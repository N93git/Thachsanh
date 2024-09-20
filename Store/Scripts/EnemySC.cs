using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySC : MonoBehaviour
{
    public float healthMax;
    public float health = 100;
    public GameObject dieObject;
    public string Name;
    private MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        dieObject.GetComponent<MeshRenderer>().enabled = false;
        healthMax = health;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void TakeDamge(float damge)
    {
        health -= damge;
        if (health <= 0)
        {
            StartCoroutine(HideObjectForSeconds(5f));
        }

    }
    IEnumerator HideObjectForSeconds(float seconds)
    {
        meshRenderer.enabled = false;
        dieObject.GetComponent<MeshRenderer>().enabled = true;
        yield return new WaitForSeconds(seconds);
        meshRenderer.enabled = true;
        health = 50;
        dieObject.GetComponent<MeshRenderer>().enabled = false;
    }
}
