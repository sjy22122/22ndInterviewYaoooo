using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuforia;

public class GiftController : MonoBehaviour
{
    public GameObject gift_box;
    public GameObject card_messages;
    public GameObject card_messages_big;
    public GameObject drink_small;
    public GameObject drink_big;
    public GameObject User_make_UI;
    public GameObject Bartender_make_UI;
    public bool isCardZoomIn;
    public Transform card_location;
    //int MakingDrink_State; //0: havent started, 1: making, 2:finished

    Receiver_DataManager receiver_dataManager;
    //public GameObject Message_canvas;

    VoiceManagement voice_controller;
    public GameObject voice_manager;

    // Start is called before the first frame update
    void Start()
    {
        VuforiaApplication.Instance.Initialize();

        gift_box.SetActive(true); //show the gift box first
        card_messages.SetActive(false);
        card_messages_big.SetActive(false);
        drink_small.SetActive(false);
        drink_big.SetActive(false);
        drink_big.GetComponent<DrinkController>().bottle_anima.SetActive(false);
        isCardZoomIn = false;


        card_messages.transform.GetChild(0).transform.gameObject.GetComponent<BoxCollider>().enabled = false;
        card_messages.transform.GetChild(1).transform.gameObject.GetComponent<BoxCollider>().enabled = false;

        receiver_dataManager = GetComponent<Receiver_DataManager>(); //get the data from database
        voice_controller = voice_manager.GetComponent<VoiceManagement>(); //get voice controller

    }


    IEnumerator Card_Collider_WaitForSec(float duration)
    {
        yield return new WaitForSeconds(duration);
        card_messages.transform.GetChild(0).transform.gameObject.GetComponent<BoxCollider>().enabled = true;
        card_messages.transform.GetChild(1).transform.gameObject.GetComponent<BoxCollider>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //if user open gift box
            if (Physics.Raycast(ray, out hit) && hit.collider.name == gift_box.name) 
            {
                
                gift_box.SetActive(false);
                card_messages.SetActive(true);
                drink_small.SetActive(true);
                receiver_dataManager.GetData();
                voice_controller.Click_Message();
                
                StartCoroutine(Card_Collider_WaitForSec(1.0f));

            }
            if (Physics.Raycast(ray, out hit) && (hit.collider.name == "Card_page1" || hit.collider.name == "Card_page2"))
            {
                if (isCardZoomIn == false)
                {
                    card_messages.transform.position = card_messages_big.transform.position;
                    card_messages.transform.rotation = card_messages_big.transform.rotation;
                    card_messages.transform.localScale = card_messages_big.transform.localScale;
                    isCardZoomIn = true;
                }
                else
                {
                    card_messages.transform.position = card_location.position;
                    card_messages.transform.rotation = card_location.rotation;
                    card_messages.transform.localScale = card_location.localScale;
                    isCardZoomIn = false;
                }

            }

            //if user click on the small glass
            if (Physics.Raycast(ray, out hit) && hit.collider.name == "glass_small") 
            {
                if (drink_big.GetComponent<DrinkController>().MakingDrink_State == 0) //first time click in
                {
                    voice_controller.Click_empty_glass();
                    drink_big.GetComponent<DrinkController>().MakingDrink_State = 1;
                    drink_small.SetActive(false);
                    drink_big.SetActive(true);

                    if (drink_big.GetComponent<DrinkController>().DrinkDecoration == 1)
                    {
                        drink_small.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.SetActive(true);
                        drink_small.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(false);
                        drink_small.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject.SetActive(false);
                    }
                    else if (drink_big.GetComponent<DrinkController>().DrinkDecoration == 2)
                    {
                        drink_small.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.SetActive(false);
                        drink_small.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(true);
                        drink_small.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject.SetActive(false);
                    }
                    else if (drink_big.GetComponent<DrinkController>().DrinkDecoration == 3)
                    {
                        drink_small.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.SetActive(false);
                        drink_small.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(false);
                        drink_small.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject.SetActive(true);
                    }
                }
                else
                {
                    drink_small.SetActive(false);
                    drink_big.SetActive(true);
                }
                
            }

            //if user click on the big glass
            if (Physics.Raycast(ray, out hit) && hit.collider.name == "glass_big") 
            {
                if (drink_big.GetComponent<DrinkController>().MakingDrink_State != 1)
                {
                    drink_big.SetActive(false);
                    drink_small.SetActive(true);
                    if (drink_small.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled == false)
                    {
                        drink_small.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
                    }
                    drink_small.transform.GetChild(0).gameObject.transform.localScale = drink_big.transform.GetChild(0).gameObject.transform.localScale;
                }
            }
        }


#elif UNITY_ANDROID
        //for Android devices //learn from https://www.youtube.com/watch?v=shnkvN4bykM
        //GetTouch is very sensitive, we need more conditions

        if (Input.GetTouch(0).phase == TouchPhase.Stationary || 
            (Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(0).deltaPosition.magnitude<1.2f))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.collider.name == gift_box.name) //if user open gift box
            {

                gift_box.SetActive(false);
                card_messages.SetActive(true);
                drink_small.SetActive(true);
                receiver_dataManager.GetData();
                voice_controller.Click_Message();

                StartCoroutine(Card_Collider_WaitForSec(1.0f));
            }

            if (Physics.Raycast(ray, out hit) && (hit.collider.name == "Card_page1" || hit.collider.name == "Card_page2"))
            {
                if (isCardZoomIn == false)
                {
                    card_messages.transform.position = card_messages_big.transform.position;
                    card_messages.transform.rotation = card_messages_big.transform.rotation;
                    card_messages.transform.localScale = card_messages_big.transform.localScale;
                    isCardZoomIn = true;
                }
                else
                {
                    card_messages.transform.position = card_location.position;
                    card_messages.transform.rotation = card_location.rotation;
                    card_messages.transform.localScale = card_location.localScale;
                    isCardZoomIn = false;
                }

            }

            if (Physics.Raycast(ray, out hit) && hit.collider.name == "glass_small") //if user click on the small glass
            {
                if (drink_big.GetComponent<DrinkController>().MakingDrink_State == 0) //first time click in
                {
                    voice_controller.Click_empty_glass();
                    drink_big.GetComponent<DrinkController>().MakingDrink_State = 1;
                    drink_small.SetActive(false);
                    drink_big.SetActive(true);

                    if (drink_big.GetComponent<DrinkController>().DrinkDecoration == 1)
                    {
                        drink_small.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.SetActive(true);
                        drink_small.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(false);
                        drink_small.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject.SetActive(false);
                    }
                    else if (drink_big.GetComponent<DrinkController>().DrinkDecoration == 2)
                    {
                        drink_small.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.SetActive(false);
                        drink_small.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(true);
                        drink_small.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject.SetActive(false);
                    }
                    else if (drink_big.GetComponent<DrinkController>().DrinkDecoration == 3)
                    {
                        drink_small.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.SetActive(false);
                        drink_small.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.SetActive(false);
                        drink_small.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject.SetActive(true);
                    }
                }
                else
                {
                    drink_small.SetActive(false);
                    drink_big.SetActive(true);
                }

            }
            if (Physics.Raycast(ray, out hit) && hit.collider.name == "glass_big") //if user click on the small glass
            {
                if (drink_big.GetComponent<DrinkController>().MakingDrink_State != 1)
                {
                    drink_big.SetActive(false);
                    drink_small.SetActive(true);
                    if (drink_small.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled == false)
                    {
                        drink_small.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
                    }
                    drink_small.transform.GetChild(0).gameObject.transform.localScale = drink_big.transform.GetChild(0).gameObject.transform.localScale;
                }
                
            }
        }
#endif
    }

    public void Click_userMakeDrink()
    {
        drink_big.GetComponent<DrinkController>().UserMakeDrink = 1;
        User_make_UI.SetActive(false);
        Bartender_make_UI.SetActive(false);
    }
    public void Click_bartenderMakeDrink()
    {
        drink_big.GetComponent<DrinkController>().UserMakeDrink = 2;
        User_make_UI.SetActive(false);
        Bartender_make_UI.SetActive(false);
    }
}
