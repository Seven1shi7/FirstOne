using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScripts : MonoBehaviour
{
    public Slider hp;
    Animator animator;
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position,target.transform.position)<=0.5f)
        {
            animator.SetBool("attack", true);
        }
        else
        {
            animator.SetBool("attack", false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "jian")
        {
            hp.value -= 0.3f;
            if(hp.value <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
