using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class VoiceManagement : MonoBehaviour
{
    /* for controlling the bartender's animation */
    public GameObject bartender_object;
    Animator bartender_animator;

    /* for controlling the bartender's voice */
    public AudioClip[] voice_list; //all the voice files
    public float duration; //how long will the clip last
    int voice_ID = -1; //which clip is going to be played
    public bool voice_finished; //has the clip finished playing

    IEnumerator VoiceFinishChecker()
    {
        yield return new WaitForSeconds(duration); //wait until bartender finish her sentence
        print("bartender finished sentence");
        voice_finished = true;
    }


    void Start()
    {
        bartender_animator = bartender_object.GetComponent<Animator>();        
    }

    private void Update()
    {
        if (voice_finished == true)
        {
            bartender_animator.SetBool("isTalking", false); //stop talking
        }
    }

    public void Click_Intro()
    {
        voice_finished = false;
        voice_ID = 0;
        GetComponent<AudioSource>().clip = voice_list[voice_ID];
        duration = GetComponent<AudioSource>().clip.length; //how long is the clip?
        GetComponent<AudioSource>().Play(); //play the clip
        bartender_animator.SetBool("isTalking", true); //bartender start talking
        StartCoroutine(VoiceFinishChecker()); //start counting time
    }

    public void Click_Message()
    {
        voice_finished = false;
        voice_ID = 1;
        GetComponent<AudioSource>().clip = voice_list[voice_ID];
        duration = GetComponent<AudioSource>().clip.length;
        GetComponent<AudioSource>().Play();
        bartender_animator.SetBool("isTalking", true); 
        StartCoroutine(VoiceFinishChecker());
    }

    public void Click_empty_glass()
    {
        voice_finished = false;
        voice_ID = 2;
        GetComponent<AudioSource>().clip = voice_list[voice_ID];
        duration = GetComponent<AudioSource>().clip.length;
        GetComponent<AudioSource>().Play();
        bartender_animator.SetBool("isTalking", true);
        StartCoroutine(VoiceFinishChecker()); 
    }
    public void Click_bartender_makeDrink()
    {
        voice_finished = false;
        voice_ID = 3;
        GetComponent<AudioSource>().clip = voice_list[voice_ID];
        duration = GetComponent<AudioSource>().clip.length;
        GetComponent<AudioSource>().Play();
        bartender_animator.SetBool("isTalking", true);
        StartCoroutine(VoiceFinishChecker());
    }
    public void Click_receiver_makeDrink()
    {
        voice_finished = false;
        voice_ID = 4;
        GetComponent<AudioSource>().clip = voice_list[voice_ID];
        duration = GetComponent<AudioSource>().clip.length;
        GetComponent<AudioSource>().Play();
        bartender_animator.SetBool("isTalking", true);
        StartCoroutine(VoiceFinishChecker());
    }
    public void Receiver_Pour_1()
    {
        voice_finished = false;
        voice_ID = 5;
        GetComponent<AudioSource>().clip = voice_list[voice_ID];
        duration = GetComponent<AudioSource>().clip.length;
        GetComponent<AudioSource>().Play();
        bartender_animator.SetBool("isTalking", true);
        StartCoroutine(VoiceFinishChecker());
    }
    public void Receiver_Pour_2()
    {
        voice_finished = false;
        voice_ID = 6;
        GetComponent<AudioSource>().clip = voice_list[voice_ID];
        duration = GetComponent<AudioSource>().clip.length;
        GetComponent<AudioSource>().Play();
        bartender_animator.SetBool("isTalking", true);
        StartCoroutine(VoiceFinishChecker());
    }
    public void Receiver_Pour_3()
    {
        voice_finished = false;
        voice_ID = 7;
        GetComponent<AudioSource>().clip = voice_list[voice_ID];
        duration = GetComponent<AudioSource>().clip.length;
        GetComponent<AudioSource>().Play();
        bartender_animator.SetBool("isTalking", true);
        StartCoroutine(VoiceFinishChecker());
    }
    public void Receiver_FinishedDrink()
    {
        voice_finished = false;
        voice_ID = 8;
        GetComponent<AudioSource>().clip = voice_list[voice_ID];
        duration = GetComponent<AudioSource>().clip.length;
        GetComponent<AudioSource>().Play();
        bartender_animator.SetBool("isTalking", true);
        StartCoroutine(VoiceFinishChecker());
    }
    public void Wrong_Drink()
    {
        voice_finished = false;
        voice_ID = 9;
        GetComponent<AudioSource>().clip = voice_list[voice_ID];
        duration = GetComponent<AudioSource>().clip.length;
        GetComponent<AudioSource>().Play();
        bartender_animator.SetBool("isTalking", true);
        StartCoroutine(VoiceFinishChecker());
    }
    public void Correct_Drink()
    {
        voice_finished = false;
        voice_ID = 10;
        GetComponent<AudioSource>().clip = voice_list[voice_ID];
        duration = GetComponent<AudioSource>().clip.length;
        GetComponent<AudioSource>().Play();
        bartender_animator.SetBool("isTalking", true);
        StartCoroutine(VoiceFinishChecker());
    }
    public void Wrong_Drink_End()
    {
        voice_finished = false;
        voice_ID = 11;
        GetComponent<AudioSource>().clip = voice_list[voice_ID];
        duration = GetComponent<AudioSource>().clip.length;
        GetComponent<AudioSource>().Play();
        bartender_animator.SetBool("isTalking", true);
        StartCoroutine(VoiceFinishChecker());
    }
    public void Drink_Model_toSmall()
    {
        voice_finished = false;
        voice_ID = 12;
        GetComponent<AudioSource>().clip = voice_list[voice_ID];
        duration = GetComponent<AudioSource>().clip.length;
        GetComponent<AudioSource>().Play();
        bartender_animator.SetBool("isTalking", true);
        StartCoroutine(VoiceFinishChecker());
    }
}
