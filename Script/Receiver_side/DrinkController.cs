using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Vuforia;


public class DrinkController : MonoBehaviour
{
    //the gameObjects we need
    public GameObject ReceiverDataManager;
    public GameObject liquid;
    public GameObject bottleA;
    public GameObject bottleB;
    public GameObject bottleC;
    


    //for the data from database, no need to be public
    Message message;
    public string Drink1;
    public string Drink2;
    public string Drink3;
    public int DrinkDecoration;
    public string DrinkName;

    //details of the drink the receiver made
    public string Current_Drink1;
    public string Current_Drink2;
    public string Current_Drink3;
    //public int Current_DrinkDecoration;

    //setup of the drink
    public float max_liquid_height;
    public float pourSpeed = 0.006f;
    public Material liquid_colour;
    public bool Drink1_finished = false;
    public bool Drink2_finished = false;
    public bool Drink3_finished = false;

    

    VoiceManagement voice_controller;
    public GameObject voice_manager;

    public GameObject restart_UI;
    public GameObject quit_UI;
    public GameObject Drink1_finished_UI;
    public GameObject Drink2_finished_UI;
    public GameObject Drink3_finished_UI;

    //making drink state
    public int MakingDrink_State = 0; //0: havent started, 1: making, 2:finished
    public int UserMakeDrink = 0; //1: user make it, 2: bartender make it

    public Texture2D bottleA_tex;
    public Texture2D bottleB_tex;
    public Texture2D bottleC_tex;
    public GameObject theDrink_decoration;
    public RawImage Drink1_icon;
    public RawImage Drink2_icon;
    public RawImage Drink3_icon;
    public TMP_Text drink_name;

    /* for controlling the bottles' animation in 'bartender make the drink' session */
    public GameObject bottle_anima;
    Animator bottle_animator;
    public bool Bartender_can_continue = true;
    Color bottleA_color = new Color(241.0f / 255.0f, 103.0f / 255.0f, 103.0f / 255.0f); //red
    Color bottleB_color = new Color(164.0f / 255.0f, 89.0f / 255.0f, 209.0f / 255.0f); //purple
    Color bottleC_color = new Color(255.0f / 255.0f, 184.0f / 255.0f, 76.0f / 255.0f); //orange

    int counter = 0;
    double drink_gap;

    void DrinkDecoration_check()
    {
        if (DrinkDecoration == 1)
        {
            theDrink_decoration.transform.GetChild(0).gameObject.SetActive(true);
            theDrink_decoration.transform.GetChild(1).gameObject.SetActive(false);
            theDrink_decoration.transform.GetChild(2).gameObject.SetActive(false);
        }
        else if (DrinkDecoration == 2)
        {
            theDrink_decoration.transform.GetChild(0).gameObject.SetActive(false);
            theDrink_decoration.transform.GetChild(1).gameObject.SetActive(true);
            theDrink_decoration.transform.GetChild(2).gameObject.SetActive(false);
        }
        else if (DrinkDecoration == 3)
        {
            theDrink_decoration.transform.GetChild(0).gameObject.SetActive(false);
            theDrink_decoration.transform.GetChild(1).gameObject.SetActive(false);
            theDrink_decoration.transform.GetChild(2).gameObject.SetActive(true);
        }
    }

    void DrinkOrder_check()
    {
        if (Drink1 == "A")
        {
            Drink1_icon.texture = bottleA_tex;
        }
        else if (Drink1 == "B")
        {
            Drink1_icon.texture = bottleB_tex;
        }
        else if (Drink1 == "C")
        {
            Drink1_icon.texture = bottleC_tex;
        }
        if (Drink2 == "A")
        {
            Drink2_icon.texture = bottleA_tex;
        }
        else if (Drink2 == "B")
        {
            Drink2_icon.texture = bottleB_tex;
        }
        else if (Drink2 == "C")
        {
            Drink2_icon.texture = bottleC_tex;
        }
        if (Drink3 == "A")
        {
            Drink3_icon.texture = bottleA_tex;
        }
        else if (Drink3 == "B")
        {
            Drink3_icon.texture = bottleB_tex;
        }
        else if (Drink3 == "C")
        {
            Drink3_icon.texture = bottleC_tex;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        bottle_animator = bottle_anima.GetComponent<Animator>();
        bottle_anima.SetActive(false);

        max_liquid_height = 1.5000f;
        Current_Drink1 = "";
        Current_Drink2 = "";
        Current_Drink3 = "";
        GetDrink();
        //isPouring = false;
        voice_controller = voice_manager.GetComponent<VoiceManagement>();
        Drink1_finished_UI.SetActive(false);
        Drink2_finished_UI.SetActive(false);
        Drink3_finished_UI.SetActive(false);
        restart_UI.SetActive(false);
        quit_UI.SetActive(false);

        //drink_gap = ((max_liquid_height / pourSpeed) / 3.0000f) * pourSpeed;

        liquid.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;


        drink_name.text = DrinkName;
        DrinkOrder_check();


    }
    void Update()
    {
    }

    
    void FixedUpdate()
    {

        if (UserMakeDrink == 1 && MakingDrink_State == 1) //user make the drink
        {
            User_makeDrink();
        }
        else if (UserMakeDrink == 2 && MakingDrink_State == 1) // bartender make the drink
        {
            BartenderMakeDrink();
        }
    }





    IEnumerator EvaluationWaitForSec(float duration)
    {
        yield return new WaitForSeconds(duration); 
        if ((Drink1 == Current_Drink1) && (Drink2 == Current_Drink2) && (Drink3 == Current_Drink3))
        {
            //all correct
            DrinkDecoration_check();
            drink_name.text = DrinkName;
            voice_controller.Correct_Drink();
            MakingDrink_State = 2;
        }
        else
        {
            //wrong
            voice_controller.Wrong_Drink();
            restart_UI.SetActive(true);
            quit_UI.SetActive(true);

        }
    }

    IEnumerator Animation_WaitForSec(float duration)
    {
        yield return new WaitForSeconds(duration);
        Pouring();
    }

    public void BartenderMakeDrink()
    {
        Color drink1_color = new Color(245.0f / 255.0f, 234.0f / 255.0f, 234.0f / 255.0f);
        Color drink2_color = new Color(245.0f / 255.0f, 234.0f / 255.0f, 234.0f / 255.0f);
        Color drink3_color = new Color(245.0f / 255.0f, 234.0f / 255.0f, 234.0f / 255.0f);
        /*set initial value*/
        Material drink1_animation_material = bottleA.transform.GetChild(0).transform.gameObject.GetComponent<MeshRenderer>().material;
        Material drink2_animation_material = bottleA.transform.GetChild(0).transform.gameObject.GetComponent<MeshRenderer>().material;
        Material drink3_animation_material = bottleA.transform.GetChild(0).transform.gameObject.GetComponent<MeshRenderer>().material;

        if (Drink1 == "A")
        {
            drink1_color = bottleA_color;
            drink1_animation_material = bottleA.transform.GetChild(0).transform.gameObject.GetComponent<MeshRenderer>().material;
        }
        else if (Drink1 == "B")
        {
            drink1_color = bottleB_color;
            drink1_animation_material = bottleB.transform.GetChild(0).transform.gameObject.GetComponent<MeshRenderer>().material;
        }
        else if (Drink1 == "C")
        {
            drink1_color = bottleC_color;
            drink1_animation_material = bottleC.transform.GetChild(0).transform.gameObject.GetComponent<MeshRenderer>().material;
        }

        if (Drink2 == "A")
        {
            drink2_color = bottleA_color;
            drink2_animation_material = bottleA.transform.GetChild(0).transform.gameObject.GetComponent<MeshRenderer>().material;
        }
        else if (Drink2 == "B")
        {
            drink2_color = bottleB_color;
            drink2_animation_material = bottleB.transform.GetChild(0).transform.gameObject.GetComponent<MeshRenderer>().material;
        }
        else if (Drink2 == "C")
        {
            drink2_color = bottleC_color;
            drink2_animation_material = bottleC.transform.GetChild(0).transform.gameObject.GetComponent<MeshRenderer>().material;
        }

        if (Drink3 == "A")
        {
            drink3_color = bottleA_color;
            drink3_animation_material = bottleA.transform.GetChild(0).transform.gameObject.GetComponent<MeshRenderer>().material;
        }
        else if (Drink3 == "B")
        {
            drink3_color = bottleB_color;
            drink3_animation_material = bottleB.transform.GetChild(0).transform.gameObject.GetComponent<MeshRenderer>().material;
        }
        else if (Drink3 == "C")
        {
            drink3_color = bottleC_color;
            drink3_animation_material = bottleC.transform.GetChild(0).transform.gameObject.GetComponent<MeshRenderer>().material;
        }

        if (liquid.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled == false)
        {
            liquid.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
        }

        if (liquid.transform.localScale.y >= 0.0f && liquid.transform.localScale.y < max_liquid_height / 3.0f)
        {
            if (bottle_anima.activeSelf == false)
            {
                bottle_anima.GetComponent<MeshRenderer>().material = drink1_animation_material;
                liquid_colour.SetColor("_BottomColour", drink1_color);
                liquid_colour.SetColor("_MiddleColour", drink1_color);
                liquid_colour.SetColor("_TopColour", drink1_color);
                bottle_anima.SetActive(true);
                bottle_animator.SetBool("isPouring", true); //start animation
            }
            else if (bottle_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) //finish the pouring animation, start pouring
            {
                Pouring();
            }
        }else if (Math.Round(liquid.transform.localScale.y, 2) == max_liquid_height / 3.0f)
        {
            Debug.Log("reach 1/3");
            if (bottle_animator.GetBool("isPouring") == true)
            {
                bottle_animator.SetBool("isPouring", false); //stop animation
            }
            
            if (bottle_animator.GetBool("isPouring") == false && !bottle_animator.IsInTransition(0)) //finish transition
            {
                if (bottle_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0 && bottle_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 2.0f) //wait until animation finished
                {
                    if (bottle_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.2f)
                    {
                        bottle_anima.SetActive(false);
                        StartCoroutine(Animation_WaitForSec(1.2f));
                    }
                    
                }
            }
        }
        else if (liquid.transform.localScale.y > max_liquid_height / 3.0f && liquid.transform.localScale.y < max_liquid_height*2.0f / 3.0f)
        {
            if (bottle_anima.activeSelf == false)
            {
                bottle_anima.GetComponent<MeshRenderer>().material = drink2_animation_material;
                
                bottle_anima.SetActive(true);
                bottle_animator.SetBool("isPouring", true); //start animation
            }
            else if (bottle_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) //finish the pouring animation, start pouring
            {

                Pouring();
                liquid_colour.SetColor("_MiddleColour", drink2_color);
                liquid_colour.SetColor("_TopColour", drink2_color);
            }
        }
        else if (Math.Round(liquid.transform.localScale.y, 2) == max_liquid_height*2.0f / 3.0f)
        {
            Debug.Log("reach 2/3");
            if (bottle_animator.GetBool("isPouring") == true)
            {
                bottle_animator.SetBool("isPouring", false); //stop animation
            }

            if (bottle_animator.GetBool("isPouring") == false && !bottle_animator.IsInTransition(0)) //finish transition
            {
                if (bottle_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0 && bottle_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 2.0f) //wait until animation finished
                {
                    if (bottle_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.2f)
                    {
                        bottle_anima.SetActive(false);
                        StartCoroutine(Animation_WaitForSec(1.2f));
                    }

                }
            }
        }
        else if (liquid.transform.localScale.y > max_liquid_height*2.0f / 3.0f && liquid.transform.localScale.y < max_liquid_height)
        {
            if (bottle_anima.activeSelf == false)
            {
                bottle_anima.GetComponent<MeshRenderer>().material = drink3_animation_material;
                
                bottle_anima.SetActive(true);
                bottle_animator.SetBool("isPouring", true); //start animation
            }
            else if (bottle_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) //finish the pouring animation, start pouring
            {
                Pouring();
                liquid_colour.SetColor("_TopColour", drink3_color);
            }
        }else if (Math.Round(liquid.transform.localScale.y, 2) == max_liquid_height)
        {
            Debug.Log("reach 3/3");
            if (bottle_animator.GetBool("isPouring") == true)
            {
                bottle_animator.SetBool("isPouring", false); //stop animation
            }

            if (bottle_animator.GetBool("isPouring") == false && !bottle_animator.IsInTransition(0)) //finish transition
            {
                if (bottle_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0 && bottle_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 2.0f) //wait until animation finished
                {
                    if (bottle_animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.2f)
                    {
                        bottle_anima.SetActive(false);

                        DrinkDecoration_check();
                        MakingDrink_State = 2;
                        
                        //StartCoroutine(Animation_WaitForSec(1.2f));
                    }

                }
            }
        }
    }

    public void Click_Finish_Drink1()
    {
        Drink1_finished_UI.SetActive(false);
    }

    public void Click_Finish_Drink2()
    {
        Drink2_finished_UI.SetActive(false);
        Drink2_finished = true;
    }
    public void Click_Finish_Drink3()
    {
        Drink3_finished_UI.SetActive(false);
        Drink3_finished = true;
    }

    public void User_makeDrink()
    {
        if (liquid.transform.localScale.y == 0.0f) //first drink
        {
            //Debug.Log("in 0.0f");
            if (bottleA.GetComponent<BottleStateTracker>().isPouring)
            {
                if (liquid.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled == false)
                {
                    liquid.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
                }
                Current_Drink1 = "A";
                liquid_colour.SetColor("_BottomColour", bottleA_color);
                liquid_colour.SetColor("_MiddleColour", bottleA_color);
                liquid_colour.SetColor("_TopColour", bottleA_color);
            }
            else if (bottleB.GetComponent<BottleStateTracker>().isPouring)
            {
                if (liquid.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled == false)
                {
                    liquid.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
                }
                Current_Drink1 = "B";
                liquid_colour.SetColor("_BottomColour", bottleB_color);
                liquid_colour.SetColor("_MiddleColour", bottleB_color);
                liquid_colour.SetColor("_TopColour", bottleB_color);
            }
            else if (bottleC.GetComponent<BottleStateTracker>().isPouring)
            {
                Debug.Log("in C pouring");
                if (liquid.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled == false)
                {
                    liquid.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
                }
                Current_Drink1 = "C";
                liquid_colour.SetColor("_BottomColour", bottleC_color);
                liquid_colour.SetColor("_MiddleColour", bottleC_color);
                liquid_colour.SetColor("_TopColour", bottleC_color);
            }
        }
        else if (Drink1_finished == true && Drink2_finished == false && (Math.Round(liquid.transform.localScale.y, 2) == Math.Round((max_liquid_height / 3.0f), 2))) //second drink
        {
            if (bottleA.GetComponent<BottleStateTracker>().isPouring)
            {
                Current_Drink2 = "A";
                liquid_colour.SetColor("_MiddleColour", bottleA_color);
                liquid_colour.SetColor("_TopColour", bottleA_color);
            }
            else if (bottleB.GetComponent<BottleStateTracker>().isPouring)
            {
                Current_Drink2 = "B";
                liquid_colour.SetColor("_MiddleColour", bottleB_color);
                liquid_colour.SetColor("_TopColour", bottleB_color);
            }
            else if (bottleC.GetComponent<BottleStateTracker>().isPouring)
            {
                Current_Drink2 = "C";
                liquid_colour.SetColor("_MiddleColour", bottleC_color);
                liquid_colour.SetColor("_TopColour", bottleC_color);
            }
        }
        else if (Drink2_finished == true && Drink3_finished == false && (Math.Round(liquid.transform.localScale.y, 2) == Math.Round(max_liquid_height * 2.0f / 3.0f, 2))) //last drink
        {
            if (bottleA.GetComponent<BottleStateTracker>().isPouring)
            {
                Current_Drink3 = "A";
                liquid_colour.SetColor("_TopColour", bottleA_color);
            }
            else if (bottleB.GetComponent<BottleStateTracker>().isPouring)
            {
                Current_Drink3 = "B";
                liquid_colour.SetColor("_TopColour", bottleB_color);
            }
            else if (bottleC.GetComponent<BottleStateTracker>().isPouring)
            {
                Current_Drink3 = "C";
                liquid_colour.SetColor("_TopColour", bottleC_color);
            }
        }

        if (Current_Drink1 != "" && liquid.transform.localScale.y < (max_liquid_height / 3.0f))
        {
            Pouring();
        }
        if (Current_Drink2 != "" && liquid.transform.localScale.y >= (max_liquid_height / 3.0f) && liquid.transform.localScale.y < (2.0f * max_liquid_height / 3.0f))
        {
            Pouring();
        }
        if (Current_Drink3 != "" && liquid.transform.localScale.y >= (2.0f*max_liquid_height / 3.0f) && liquid.transform.localScale.y < max_liquid_height)
        {
            Pouring();
        }

        
        if (Math.Round(liquid.transform.localScale.y, 2) == Math.Round((max_liquid_height / 3.0f), 2))
        {   //bartender talk first, then player continue
            if (Drink1_finished==false)
            {
                Drink1_finished_UI.SetActive(true);
            }
        }
        if (Math.Round(liquid.transform.localScale.y, 2) == Math.Round((2.0f * max_liquid_height / 3.0f), 2))
        {
            if (Drink2_finished == false)
            {
                Drink2_finished_UI.SetActive(true);
            }
        }
        if (Math.Round(liquid.transform.localScale.y, 2) >= Math.Round(max_liquid_height, 2))
        {
            if (Drink3_finished == false)
            {
                Drink3_finished_UI.SetActive(true);
            }
        }

        
    }
    public void Evaluation()
    {
        //finished the drink, evaluation
        if ((Math.Round(liquid.transform.localScale.y, 2) >= Math.Round(max_liquid_height, 2)) &&
            (Drink1_finished == true && Drink2_finished == true && Drink3_finished == true))
        {
            float duration = voice_manager.GetComponent<AudioSource>().clip.length;
            StartCoroutine(EvaluationWaitForSec(duration));
        }
    }

    public void Click_quitDrink()
    {
        drink_name.text = DrinkName;
        restart_UI.SetActive(false);
        quit_UI.SetActive(false);
        DrinkDecoration_check();
        voice_controller.Wrong_Drink_End();
        MakingDrink_State = 2;
    }
    public void Click_restartDrink()
    {
        if (liquid.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled == true)
        {
            liquid.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        restart_UI.SetActive(false);
        quit_UI.SetActive(false);
        Drink1_finished_UI.SetActive(false);
        Drink2_finished_UI.SetActive(false);
        Drink3_finished_UI.SetActive(false);
        Drink1_finished = false;
        Drink2_finished = false;
        Drink3_finished = false;
        Current_Drink1 = "";
        Current_Drink2 = "";
        Current_Drink3 = "";
        liquid.transform.localScale = new Vector3(1.0f, 0.0f, 1.0f);
    }

    public void GetDrink()
    {
        message = ReceiverDataManager.GetComponent<Receiver_DataManager>().message;
        Drink1 = message.Drink1;
        Drink2 = message.Drink2;
        Drink3 = message.Drink3;
        DrinkDecoration = message.DrinkDecoration;
        DrinkName = message.DrinkName;
    }
    void Pouring()
    {
        liquid.transform.localScale = new Vector3(1.0f, liquid.transform.localScale.y + pourSpeed, 1.0f);
    }

}
