using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sub1 : Super
{
    public override void PrintSomthing()
    {
        Debug.Log("This is the Sub1 Class");
    }
    //in a different script you can call GetComponent<Super>().PrintSomthing(); and it will call this method
}
