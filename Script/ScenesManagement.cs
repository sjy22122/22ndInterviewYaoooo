using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ScenesManagement : MonoBehaviour
{
    public static int current_role; //1: giver, 2: receiver

    public void ToHomePage()
    {
        SceneManager.LoadScene("HomePage");
    }

    public void GiverToLogin()
    {
        current_role = 1;
        SceneManager.LoadScene("Login");
    }
    public void ReceiverToLogin()
    {
        current_role = 2;
        SceneManager.LoadScene("Login");
    }

    public void ToLogin()
    {
        /*
        if (current_role == 1) //if it's giver login and going to giver side
        {
            SceneManager.LoadScene("Giver_side");
        }else if(current_role == 2) //if it's receiver login and going to receiver side
        {
            SceneManager.LoadScene("Receiver_side");
        }*/
    }


    


}
