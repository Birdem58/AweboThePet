using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireController : MonoBehaviour
{
    private Animator animator;
    private bool isExtinguished = false;
    private bool isEnding = false;
    private float endTimer = 0f;
    

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isEnding)
        {
            endTimer += Time.deltaTime;
            if (endTimer >= 0.4f)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnMouseDown()
    {
        if (!isExtinguished)
        {
            PetManager.Instance.ExtinguishFire(gameObject);
            animator.SetTrigger("Extinguish");
            isExtinguished = true;
            isEnding = true;
        }
    }
}
