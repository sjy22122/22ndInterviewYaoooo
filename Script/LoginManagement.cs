using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// for firebase
using System.Threading.Tasks;
using System.Threading;
using Firebase.Storage;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine.Networking;

public class LoginManagement : MonoBehaviour
{
    public static string input_userID;
    public static string input_OrderNumber;

    public TMP_InputField userID_UI;
    public TMP_InputField OrderNumber_UI;

    public GameObject error_message;

    FirebaseFirestore database;

    void Start()
    {
        error_message.SetActive(false);
        database = FirebaseFirestore.DefaultInstance;
    }

    IEnumerator ShowNotification()
    {
        yield return new WaitForSeconds(3.0f);  //wait for seconds
        error_message.SetActive(false);
    }

    
    public void TryLogin()
    {
        database.Collection("user").Document(userID_UI.text).Collection(OrderNumber_UI.text).Document(OrderNumber_UI.text).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists == false)
            {
                Debug.Log("order does not exist!");
                error_message.SetActive(true);
                StartCoroutine(ShowNotification());
            }
            else
            {
                input_userID = userID_UI.text;
                input_OrderNumber = OrderNumber_UI.text;

                if (ScenesManagement.current_role == 1) //if it's giver login and going to giver side
                {
                    SceneManager.LoadScene("Giver_side");
                }
                else if (ScenesManagement.current_role == 2) //if it's receiver login and going to receiver side
                {
                    SceneManager.LoadScene("Receiver_side");
                }
            }
        });
    }
}
