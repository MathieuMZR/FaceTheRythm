using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    public DataTraveller dataTraveller;
    public Slider Slider;
    public Slider SliderArrowSpeed;

    public TextMeshProUGUI difficulty;
    public TextMeshProUGUI BPMText;
    
    public TextMeshProUGUI arrowSpeed;
    public TextMeshProUGUI arrowSpeedtext;

    public TMP_InputField inputField;

    public void SetBPM()
    {
        dataTraveller.BPM = (int) Slider.value;
        dataTraveller.arrowSpeed = (int) SliderArrowSpeed.value;
    }

    public void SetSong()
    {
        StartCoroutine(dataTraveller.GetAudioClip(inputField.text));
    }

    public void Play()
    {
        GameObject.FindGameObjectWithTag("Transition").GetComponent<Animator>().SetTrigger("Transition");
        Invoke("LoadScene", 2.5f);
    }

    void LoadScene()
    {
        SceneManager.LoadScene("Niveau 1");
    }

    private void Update()
    {
        var x = (int)Slider.value;
        BPMText.text = $"{x}<br>BPM";
        
        if (x >= 100 && x < 110)
        {
            difficulty.text = $"Difficulty : <br><color=#00FF00>Easy</color>";
        }
        else if (x > 110 && x < 160)
        {
            difficulty.text = $"Difficulty : <br><color=#96E100>Normal</color>";
        }
        else if (x > 160 && x < 210)
        {
            difficulty.text = $"Difficulty : <br><color=#EAE300>Medium</color>";
        }
        else if (x > 210 && x < 300)
        {
            difficulty.text = $"Difficulty : <br><color=#EA8700>Hard</color>";
        }
        else if (x > 300 && x < 450)
        {
            difficulty.text = $"Difficulty : <br><color=#EA3900>Very Hard</color>";
        }
        else if (x > 450)
        {
            difficulty.text = $"Difficulty : <br><color=#D70000>Extreme</color>";
        }
        
        var y = (int)SliderArrowSpeed.value;
        arrowSpeed.text = $"{y}<br>SPD";
        
        if (y >= 2 && y < 8)
        {
            arrowSpeedtext.text = $"Arrow Speed : <br><color=#00FF00>Very Slow</color>";
        }
        else if (y > 8 && y < 12)
        {
            arrowSpeedtext.text = $"Arrow Speed : <br><color=#96E100>Slow</color>";
        }
        else if (y > 12 && y < 18)
        {
            arrowSpeedtext.text = $"Arrow Speed : <br><color=#EAE300>Normal</color>";
        }
        else if (y > 18 && y < 23)
        {
            arrowSpeedtext.text = $"Arrow Speed : <br><color=#EA8700>Fast</color>";
        }
        else if (y > 23 && y < 27)
        {
            arrowSpeedtext.text = $"Arrow Speed : <br><color=#EA3900>Very Fast</color>";
        }
        else if (y > 27)
        {
            arrowSpeedtext.text = $"Arrow Speed : <br><color=#D70000>Flash</color>";
        }
    }
}
