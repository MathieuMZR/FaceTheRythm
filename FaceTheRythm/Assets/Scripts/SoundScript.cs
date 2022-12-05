using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Sound", menuName = "Sounds/new Sound", order = 1)]
public class SoundScript : ScriptableObject
{
    //********************************************//
    // BPM is Beats Per Minute                    //
    // It mean that with 120 BPM you have 2 BPS   //
    // 120 / 60 = 2                               //
    //********************************************//

    public float BPM;

    public enum types
    {
        right, left, up, down, none
    }
    
    public types[] composition;

    public AudioClip soundClip;
}
