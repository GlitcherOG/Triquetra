﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

[RequireComponent(typeof(AudioSource))]

public class PlayerHandler : MonoBehaviour
{
    [Header("Character Variables")]
    public int character;
    public float[] moveSpeed;
    public float[] jumpHeight;
    public float[] climbSpeed;
    public string[] names;
    public Text characternames;

    [Header("Value Variables")]
    public float curHealth;
    public float maxHealth;
    private float healthPerSection;
    private float prevHealth;
        
    [Header("Heart Slots")]
    public Image[] heartSlots;
    public Sprite[] heartSprites;

    [Header("Damage Effect Variables")]
    public Image damageImage;
    public Image deathImage;
    public AudioClip deathClip;
    public float flashSpeed = 5;
    public Color flashColour = new Color(1, 0, 0, .2f);
    AudioSource playerAudio;
    static public bool isDead;
    bool damaged;

    [Header("Check Point")]
    public Transform curCheckPoint;
    private float damageTimer;

    [Header("Camera")]
    public CharacterController2D[] controller;
    public CinemachineVirtualCamera Camera;
    public float portalDistance = 1f;


    void Start()
    {
        playerAudio = GetComponent<AudioSource>();
        healthPerSection = (maxHealth / (heartSlots.Length) * .2f);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHeart();

        //Player is Dead
        if (curHealth <= 0 && !isDead)
        {
            Death();
        }

        //Test Damage
        if (Input.GetKeyDown(KeyCode.X))
        {
            curHealth -= 5;
            damaged = true;
        }


        //Player is Damaged
        if (damaged && !isDead)
        {
            damageImage.color = flashColour;
            damaged = false;
        }
        else
        {
            damageImage.color = Color.Lerp(damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        if (damageTimer > 0)
        {
            damageTimer -= Time.deltaTime;
        }
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        bool isJumping = Input.GetButtonDown("Jump");

        if (isJumping)
        {
            controller[character].Jump(jumpHeight[character]);
        }

        controller[character].Climb(vertical * climbSpeed[character]);
        controller[character].Move(horizontal * moveSpeed[character]);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (character >= controller.Length - 1)
            {
                character = 0;
            }
            else
            {
                character++;
            }
            characternames.text = names[character];
            Camera.Follow = controller[character].gameObject.transform;
        }
    }


    void UpdateHeart()
    {

        //for all the hearts in heart slots        

        for (int i = 0; i < heartSlots.Length; i++)
        {
            if (curHealth >= (healthPerSection * 5) + healthPerSection * 5 * i)
            {
                heartSlots[i].sprite = heartSprites[0];
            }            
            else
            {
                heartSlots[i].sprite = heartSprites[1];
            }
        }

    }

    public void Damage()
    {

        if (damageTimer <= 0)
        {
            curHealth -= 5;
            damageTimer = 1;
        }
        
    }

    public void Death()
    {
        // set the death flag to this funciton int's called again
        isDead = true;

        //Set the AudioSource to play the death clip
        playerAudio.clip = deathClip;
        playerAudio.Play();

        deathImage.gameObject.GetComponent<Animator>().SetTrigger("Dead");
        Invoke("Revive", 9f);

    }

    void Revive()
    {
        isDead = false;
        curHealth = maxHealth;
        //curMana = maxMana;
        ///curStamina = maxStamina;

        //move and rotate spawn location
        this.transform.position = curCheckPoint.position;
        this.transform.rotation = curCheckPoint.rotation;
        deathImage.gameObject.GetComponent<Animator>().SetTrigger("Alive");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CheckPoint"))
        {
            curCheckPoint = other.transform;
            //saveAndLoad.Save();
        }
    }

}