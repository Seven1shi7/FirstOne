using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Move : MonoBehaviour
{
    public Yaogan yaogan;
    private bool isdown = false;
    public Animation animation;
    public Button accack1;
    public Button accack2;
    public Button accack3;
    public Button accack4;
    // Start is called before the first frame update
    void Start()
    {
        yaogan.move = move;
        animation = GetComponent<Animation>();
        accack1.onClick.AddListener(() =>
        {

            animation.Play("attack");
            
        });

        accack2.onClick.AddListener(() =>
        {
            animation.Play("attack2");
        });

        accack3.onClick.AddListener(() =>
        {
            animation.Play("attack3");
        });

        accack4.onClick.AddListener(() =>
        {
            animation.Play("skill");
        });

        

    }

    private void move(Vector2 vector)
    {
        Quaternion to = Quaternion.LookRotation(new Vector3(vector.x, 0, vector.y));
        transform.rotation = to;
    }

    // Update is called once per frame
    void Update()
    {

        if (yaogan.ismove)
        {
            // isdown = true;
            transform.position += transform.forward * 5 * Time.deltaTime;
            animation.Stop("free");
            animation.Play("walk");
        }
        else
        {
            animation.Stop("walk");
            animation.Blend("idle");
        }

    }


    public void LateUpdate()
    {


    }

}
