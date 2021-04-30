using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    [Header("Info")]
    public float spinSpeed;
    public float floatSpeed;
    public float floatHeight;

    Vector3 tempPos;
    Vector3 posOffset;

    // Start is called before the first frame update
    void Start()
    {
        posOffset = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, spinSpeed);

        tempPos = posOffset;
        tempPos.y += Mathf.Sin(Time.fixedTime * Mathf.PI * floatSpeed) * floatHeight;
        transform.position = tempPos;
    }
}
