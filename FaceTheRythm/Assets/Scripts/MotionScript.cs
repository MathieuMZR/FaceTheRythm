using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using Random = UnityEngine.Random;
using static SoundScript.types;

public class MotionScript : MonoBehaviour
{
    private List<SoundScript.types> composition;
    public int lenghtRandomComposition;
    public float BPM;
    
    public Transform[] spawnPoints;
    
    public GameObject arrow;
    private GameObject arrowInstance;

    public Color[] colors;

    public Light2D lightRed, lightBlue;
    private Sequence lightSequence;

    public bool isLose;

    private void Start()
    {
        BPM = FindObjectOfType<DataTraveller>().BPM;
        composition = new List<SoundScript.types>(lenghtRandomComposition);
        
        for (int i = 0; i < lenghtRandomComposition; i++)
        {
            composition.Add((SoundScript.types)Random.Range(0,5));
        }

        StartCoroutine(PlaySound());
    }

    //Start Playing the sound with all parameters
    IEnumerator PlaySound()
    {
        foreach (var currentCompositionIndex in composition)
        {
            if (!isLose)
            {
                switch (currentCompositionIndex)
                {
                    case left:
                        SpawnArrow(left);
                        break;
                    case right:
                        SpawnArrow(right);
                        break;
                    case up:
                        SpawnArrow(up);
                        break;
                    case down:
                        SpawnArrow(down);
                        break;
                    case none:
                        break;
                }

                yield return new WaitForSeconds(60f / BPM);
            }
        }
    }

    void SpawnArrow(SoundScript.types type)
    {
        var spawnPoint = Random.Range(0, 2);
        arrowInstance = Instantiate(arrow, spawnPoints[spawnPoint].position, Quaternion.identity);

        if (arrowInstance)
        {
            arrowInstance.GetComponent<ArrowScript>().type = type;

            switch (spawnPoint)
            {
                case 0:arrowInstance.GetComponent<ArrowScript>().spriteRenderer.color = colors[0];
                    arrowInstance.GetComponent<ArrowScript>().color = colors[0];
                    arrowInstance.GetComponent<ArrowScript>().spawnSide = 0;
                    
                    lightSequence.Kill();

                    lightSequence.Append(lightRed.DOLight2DIntensity(0f, 0f))
                        .Append(lightRed.DOLight2DIntensity(1.5f, 0.05f))
                        .Append(lightRed.DOLight2DIntensity(0f, 0.3f).SetDelay(0.1f));
                        
                    break;
                case 1:arrowInstance.GetComponent<ArrowScript>().spriteRenderer.color = colors[1];
                    arrowInstance.GetComponent<ArrowScript>().color = colors[1];
                    arrowInstance.GetComponent<ArrowScript>().spawnSide = 1;
                    
                    lightSequence.Kill();

                    lightSequence.Append(lightBlue.DOLight2DIntensity(0f, 0f))
                        .Append(lightBlue.DOLight2DIntensity(1.5f, 0.05f))
                        .Append(lightBlue.DOLight2DIntensity(0f, 0.3f).SetDelay(0.1f));
                    
                    break;
            }
        }
    }
}
