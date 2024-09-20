using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class HealthbarEnemy : MonoBehaviour
{
    Slider heathSlider;
    public float maxHeath = 100f;
    public float heath;
    //public GameObject player; // Tham chiếu đến Transform của Player
    RectTransform rectTransform;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        heathSlider = GetComponent<Slider>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log(heath);
        UpdateGui();
        //  Collider playerCollider = player.GetComponent<Collider>();

        // transform.position = new Vector3(player.transform.position.x, player.transform.position.y + (playerCollider.bounds.size.y + 0.6f), player.transform.position.z);

        // Giữ Canvas không quay theo hướng của Player
        //rectTransform.rotation = Quaternion.identity;

    }
    public void UpdateGui()
    {
        if (heathSlider.value != heath)
        {
            heathSlider.value = heath;
        }
    }
    public void TakeDamge(float damge)
    {
        heath -= damge;
        if (heathSlider.value != heath)
        {
            heathSlider.value = heath;
        }
    }
}
