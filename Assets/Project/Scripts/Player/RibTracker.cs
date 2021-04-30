using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RibTracker : MonoBehaviour
{
    [Header("Objects")]
    public Transform ribTracObj;

    // Update is called once per frame
    void Update()
    {
        transform.position = ribTracObj.position;
    }
}
