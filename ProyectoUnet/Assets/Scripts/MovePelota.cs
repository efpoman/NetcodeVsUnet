using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePelota : MonoBehaviour
{
    
    float y;
    float x;
    float z;
    private void Start()
    {
        x = transform.position.x;
        y = transform.position.y;
        z = transform.position.z;
    }
    void Update()
    {
        if (y <= 2.5f)
        {
            
            y = 9f;
            transform.position = new Vector3(x,y, z);
        }
        else
        {
            y -= 0.3f * Time.deltaTime;
            transform.position = new Vector3(x, y, z);
        }
        
    }
   
}
