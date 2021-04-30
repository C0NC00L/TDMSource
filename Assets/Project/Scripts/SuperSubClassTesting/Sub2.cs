using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sub2 : Super
{
    public override void PrintSomthing()
    {
        Debug.Log("This is the Sub2 Class");
    }
    //in a different script you can call GetComponent<Super>().PrintSomthing(); and it will call this method
}
