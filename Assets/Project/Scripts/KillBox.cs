using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Transform[] spawn;
        if (TeamManager.instance.teamNum == 0)
            spawn = GameObject.Find("BlueSpawnPoints").GetComponentsInChildren<Transform>();
        else
            spawn = GameObject.Find("RedSpawnPoints").GetComponentsInChildren<Transform>();
        int num = Random.Range(0, spawn.Length);
        other.gameObject.transform.parent.parent.transform.position = spawn[num].position;
    }
}
