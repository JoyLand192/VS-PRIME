using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class d : MonoBehaviour
{
    float c;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        c += 180f * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, 0, c);
    }
}
