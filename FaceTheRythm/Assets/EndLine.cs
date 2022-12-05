using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class EndLine : MonoBehaviour
{
    public GameObject goodEffect;

    private int comboInt;
    private float scoreInt = 250f;
    private float highScore = 250f;
    
    public TextMeshProUGUI textFeedback;
    public TextMeshProUGUI combo;
    public TextMeshProUGUI score;
    public TextMeshProUGUI songName;
    public Image life;
    
    public float animTimeIn;
    public float animTimeOut;
    public float animTimeStay;

    public GameObject line1, line2;

    public Material[] material;

    private Sequence _sequence;

    public List<GameObject> inCollider;

    private void Start()
    {
        FindObjectOfType<DataTraveller>().StartSound();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        combo.text = $"x{comboInt}";
        score.text = $"{scoreInt}";

        if (scoreInt <= 0)
        {
            FindObjectOfType<MotionScript>().isLose = true;

            foreach (var x in FindObjectsOfType<ArrowScript>())
            {
                if (x)
                {
                    var y = new float[] {-45f, 45f};
                    x.GetComponent<ArrowScript>().Rotate(y[Random.Range(0,1)]);

                    x.GetComponent<CircleCollider2D>().enabled = false;
                    
                    DOTween.To(() => x.GetComponent<ArrowScript>().basicSpeed,
                        y => x.GetComponent<ArrowScript>().basicSpeed = y,
                        x.GetComponent<ArrowScript>().basicSpeed * 1.25f, 0.75f);

                    StartCoroutine(ResetMaterial(x.gameObject));
                }
            }
        }

        life.DOFillAmount(Mathf.Lerp(0, 1, scoreInt / highScore), 0.2f);
        songName.text = FindObjectOfType<DataTraveller>().songDuration;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Arrow"))
        {
            inCollider.Add(col.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Arrow"))
        {
            if (Input.GetKey(KeyCode.LeftArrow) && col.GetComponent<ArrowScript>().type == SoundScript.types.left)
            {
                SpawnGoodEffect(col);

                foreach (var colToDestroy in inCollider.ToList())
                {
                    scoreInt += 10;
                    
                    if (scoreInt > highScore)
                    {
                        highScore = scoreInt;
                    }
                    
                    inCollider.Remove(colToDestroy);
                    Destroy(colToDestroy);
                }
                
                Animtext("Good !");

                comboInt++;
            }
            
            if (Input.GetKey(KeyCode.RightArrow) && col.GetComponent<ArrowScript>().type == SoundScript.types.right)
            {
                SpawnGoodEffect(col);
                
                foreach (var colToDestroy in inCollider.ToList())
                {
                    scoreInt += 10;
                    
                    if (scoreInt > highScore)
                    {
                        highScore = scoreInt;
                    }
                    
                    inCollider.Remove(colToDestroy);
                    Destroy(colToDestroy);
                }
                
                Animtext("Good !");

                comboInt++;
            }
            
            if (Input.GetKey(KeyCode.UpArrow) && col.GetComponent<ArrowScript>().type == SoundScript.types.up)
            {
                SpawnGoodEffect(col);
                
                foreach (var colToDestroy in inCollider.ToList())
                {
                    scoreInt += 5;
                    
                    if (scoreInt > highScore)
                    {
                        highScore = scoreInt;
                    }
                    
                    inCollider.Remove(colToDestroy);
                    Destroy(colToDestroy);
                }
                
                Animtext("Good !");

                comboInt++;
            }
            
            if (Input.GetKey(KeyCode.DownArrow) && col.GetComponent<ArrowScript>().type == SoundScript.types.down)
            {
                SpawnGoodEffect(col);
                
                foreach (var colToDestroy in inCollider.ToList())
                {
                    scoreInt += 5;
                    
                    if (scoreInt > highScore)
                    {
                        highScore = scoreInt;
                    }
                    
                    inCollider.Remove(colToDestroy);
                    Destroy(colToDestroy);
                }
                
                Animtext("Good !");

                comboInt++;
            }
        }
    }

    void SpawnGoodEffect(Collider2D col)
    {
        if (col.GetComponent<ArrowScript>())
        {
            switch (col.GetComponent<ArrowScript>().spawnSide)
            {
                case 0:Instantiate(goodEffect, line1.transform.position, Quaternion.identity);
                    break;
                case 1:Instantiate(goodEffect, line2.transform.position, Quaternion.identity);
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Arrow") && inCollider.Contains(other.gameObject))
        {
            scoreInt -= 25;
            life.DOColor(Color.red, 0.1f);
            life.DOColor(Color.white, 0.25f).SetDelay(0.1f);

            inCollider.Remove(other.gameObject);

            var x = new float[] {-45f, 45f};
            other.GetComponent<ArrowScript>().Rotate(x[Random.Range(0,1)]);

            DOTween.To(() => other.GetComponent<ArrowScript>().basicSpeed,
                y => other.GetComponent<ArrowScript>().basicSpeed = y,
                other.GetComponent<ArrowScript>().basicSpeed * 2f, 0.5f);

            StartCoroutine(ResetMaterial(other.gameObject));

            Destroy(other.gameObject, 1f);
            
            Animtext("Bad...");

            comboInt = 0;
        }
    }

    IEnumerator ResetMaterial(GameObject x)
    {
        x.GetComponentInChildren<SpriteRenderer>().material = material[0];
        x.GetComponentInChildren<SpriteRenderer>().color = Color.white;
        yield return new WaitForSeconds(0.1f);
        x.GetComponentInChildren<SpriteRenderer>().material = material[1];
        x.GetComponentInChildren<SpriteRenderer>().DOColor(Color.black, 0.2f);
    }

    void Animtext(string Text)
    {
        _sequence.Kill();
        
        textFeedback.transform.DOScale(Vector3.zero, 0f);
        
        textFeedback.text = Text;
        
        _sequence.Append(textFeedback.transform.DOScale(Vector3.one, animTimeIn)).Append(textFeedback.transform.DOScale(Vector3.zero, animTimeOut).SetDelay(animTimeIn + animTimeStay));
    }
}
