using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public Yaogan yaogan;
    private bool isdown =false;
    public Animation animation;
    // Start is called before the first frame update
    void Start()
    {
        yaogan.move = move;
        animation = GetComponent<Animation>();
    }

    private void move(Vector2 vector)
    {
        Quaternion to = Quaternion.LookRotation(new Vector3(vector.x, 0, vector.y));
        transform.rotation = to;
    }

    // Update is called once per frame
    void Update()
    {
        if (yaogan.ismove )
        {
            isdown = true;
            transform.position += transform.forward * 5 * Time.deltaTime;
          
        }
        if (Input.GetButtonDown("Fire1") && isdown == true)
        {
            animation.Play("walk");
        }

        if (Input.GetButtonUp("Fire1"))
        {
            animation.Stop("walk");
            animation.Play("idle");
        }

    }


}
