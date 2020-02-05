using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Transition : MonoBehaviour
{
    public LayerMask layers;
    public float stay_time = 1f;

    float stayTimer = 0f;
    bool isStaying = false;

    private void Awake()
    {
        stayTimer = 0f;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (layers.Contains(collision.gameObject))
        {
            isStaying = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (layers.Contains(collision.gameObject))
        {
            isStaying = false;
        }
    }


    private void FixedUpdate()
    {
        if(isStaying)
        {
            stayTimer += Time.deltaTime;
            if(stayTimer >= stay_time)
            {
                stayTimer = 0f;
                GameManager.Instance.LoadNextScene();
            }
        }
        else
        {
            stayTimer = 0f;
        }
    }

    void OnDrawGizmos()
    {
        //Gizmos.DrawIcon(transform.position, "Level_Transition", false);
    }
}
