using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSc : MonoBehaviour
{
    public float speed = 5.0f;
    public float rotationSpeed = 10.0f;
    private Animator animator;

    private Vector3 moveDirection = Vector3.zero;
    private bool hasAttack = false;
    void Start()
    {
        animator = GetComponent<Animator>();
        GameObject obj = GameObject.Find("ThachSanhAnm"); // Thay "ObjectName" bằng tên đối tượng chứa mesh

        if (obj != null)
        {
            // Lấy danh sách các MeshFilter của đối tượng
            MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();

            foreach (MeshFilter meshFilter in meshFilters)
            {
                // Kiểm tra tên của mesh
                if (meshFilter.mesh.name == "Human")
                {
                    // Ẩn mesh
                    meshFilter.gameObject.SetActive(false);
                }
            }
            Debug.Log("Null");
        }
        else
        {

        }

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

                RotateCharacter();
                animator.SetInteger("State", 1);
            }
            else
            {
                if (!hasAttack)
                    animator.SetInteger("State", 0);
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

                Vector3 direction = targetPosition - transform.position;

                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }
            }

            StartCoroutine(TransitionToAttack());
        }



    }
    IEnumerator TransitionToAttack()
    {
        moveDirection = new Vector3(0, 0, 0);
        hasAttack = true;
        animator.speed = 3f;
        animator.SetInteger("State", 2);
        yield return new WaitForSeconds(0.5f); // Thời gian chờ 0.3 giây
        animator.speed = 1f;
        hasAttack = false;
        animator.SetInteger("State", 0);
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
}
