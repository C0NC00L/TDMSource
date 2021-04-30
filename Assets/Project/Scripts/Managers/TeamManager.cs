using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    [Header("Team")]
    public bool blueTeam;
    public bool redTeam;
    public int teamNum;

    [Header("Team Materials")]
    public Material[] materials;

    public static TeamManager instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        if(gameObject.name == "TeamManager(Clone)")
            DontDestroyOnLoad(gameObject);
    }
    public Material GetMyMat()
    {
        if (blueTeam)
            return materials[0];
        else
            return materials[1];
    }
}
