using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public Yaogan yaogan;
    // Start is called before the first frame update
    void Start()
    {
        yaogan.move = move;
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
            transform.position += transform.forward * 5 * Time.deltaTime;
        }

    }
}
