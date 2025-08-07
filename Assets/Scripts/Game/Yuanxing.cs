using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yuanxing : MonoBehaviour
{
    CircleRangeDetector circleRangeDetector;
    Move move;
    public GameObject enemy;
    // Start is called before the first frame update
    void Start()
    {
        
        
        CircleRangeDetector circleRangeDetector = GetComponent<CircleRangeDetector>();
        Move move = GetComponent<Move>();

        move.accack1.interactable = false;
        enemy = GameObject.Find("Enemy");
        if (circleRangeDetector.IsTargetInRange(enemy.transform.position))
        {
            Debug.Log("·¶Î§ÄÚÓÐ¹ÖÎï");
            move.accack1.interactable = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
