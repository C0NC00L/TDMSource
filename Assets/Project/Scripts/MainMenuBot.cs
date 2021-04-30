using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBot : MonoBehaviour
{
    [Header("Animations")]
    public Animation myAnim;
    List<AnimationState> states = new List<AnimationState>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (AnimationState state in myAnim)
        {
            states.Add(state);
        }
        PlayNewAnim();
    }
    void PlayNewAnim()
    {
        int num = Random.Range(0, states.Count);
        myAnim.Play(states[num].name);
    }
    private void Update()
    {
        if (!myAnim.isPlaying)
            PlayNewAnim();
    }
}
