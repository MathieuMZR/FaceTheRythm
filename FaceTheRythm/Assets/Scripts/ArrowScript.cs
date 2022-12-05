using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    public float basicSpeed = 1;
    public SoundScript.types type;
    public Sprite[] sprites;
    public Sprite[] spritesOutline;
    public Color color;

    public int spawnSide; //0 = left, 1 : right

    public SpriteRenderer spriteRenderer;
    public SpriteRenderer spriteRendererOutline;

    private void Start()
    {
        basicSpeed = FindObjectOfType<DataTraveller>().arrowSpeed;
        
        switch (type)
        {
            case SoundScript.types.right: spriteRenderer.sprite = sprites[0];
                spriteRendererOutline.sprite = spritesOutline[0];
                GetComponentInChildren<ParticleSystem>().textureSheetAnimation.SetSprite(0, sprites[0]);
                break;
            case SoundScript.types.left: spriteRenderer.sprite = sprites[1];
                spriteRendererOutline.sprite = spritesOutline[1];
                GetComponentInChildren<ParticleSystem>().textureSheetAnimation.SetSprite(0, sprites[1]);
                break;
            case SoundScript.types.up: spriteRenderer.sprite = sprites[2];
                spriteRendererOutline.sprite = spritesOutline[2];
                GetComponentInChildren<ParticleSystem>().textureSheetAnimation.SetSprite(0, sprites[2]);
                break;
            case SoundScript.types.down: spriteRenderer.sprite = sprites[3];
                spriteRendererOutline.sprite = spritesOutline[3];
                GetComponentInChildren<ParticleSystem>().textureSheetAnimation.SetSprite(0, sprites[3]);
                break;
            case SoundScript.types.none: spriteRenderer.color = new Color(1,1,1,0);
                GetComponentInChildren<ParticleSystem>().Stop();
                break;
        }

        var mainModule = GetComponentInChildren<ParticleSystem>().main;
        mainModule.startColor = new ParticleSystem.MinMaxGradient(new Color(color.r, color.g, color.b, 0.2f));
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3.down * basicSpeed * Time.deltaTime);
    }

    public void Rotate(float Zvalue)
    {
        transform.DORotate(new Vector3(0, 0, Zvalue), 0.5f);
    }
}
