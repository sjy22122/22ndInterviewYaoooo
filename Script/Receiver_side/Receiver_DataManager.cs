using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// for firebase
using System.Threading.Tasks;
using System.Threading;
using Firebase.Storage;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine.Networking;

public class Receiver_DataManager : MonoBehaviour
{
    public string userID;
    public string OrderNumber;
    public Message message;
    //[SerializeField] Button getdataButton;
    public TMP_Text textUI;
    public RawImage imageUI;
    public Button voiceUI;
    public GameObject DrinkController;
    FirebaseFirestore database;
    FirebaseStorage database_media;
    StorageReference storage_img_ref;
    StorageReference storage_audio_ref;
    StorageReference image_ref;
    StorageReference audio_ref;

    public float original_imageUI_width;
    public float original_imageUI_height;
    Texture2D downloaded_image;

    // Start is called before the first frame update
    void Start()
    {
        if (LoginManagement.input_userID != null)
        {
            userID = LoginManagement.input_userID;
            OrderNumber = LoginManagement.input_OrderNumber;
        }

        database = FirebaseFirestore.DefaultInstance;
        database_media = FirebaseStorage.DefaultInstance;
        storage_img_ref = database_media.GetReferenceFromUrl("gs://secret-drinks-club.appspot.com/" + userID + "/" + OrderNumber + "/image/");
        storage_audio_ref = database_media.GetReferenceFromUrl("gs://secret-drinks-club.appspot.com/" + userID + "/" + OrderNumber + "/audio/");

        original_imageUI_width = imageUI.GetComponent<RectTransform>().rect.width;
        original_imageUI_height = imageUI.GetComponent<RectTransform>().rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GetImageMessage(string image_URL)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(image_URL); //create request
        yield return request.SendWebRequest(); //wait for the request to complete
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log("image error");
        }
        else
        {
            imageUI.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, original_imageUI_width);
            imageUI.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, original_imageUI_height);
            //first, get the image from database
            downloaded_image = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Debug.Log("image size: " + downloaded_image.width + " * " + downloaded_image.height);
            /* resize the Raw Image base on the ratio of the downloaded image */
            if (downloaded_image.width > downloaded_image.height)
            {
                float ratio = downloaded_image.height * 1.0f / downloaded_image.width;
                //Debug.Log("flag1, ratio = " + ratio);

                imageUI.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, original_imageUI_width * ratio);
            }
            else
            {
                float ratio = downloaded_image.width * 1.0f / downloaded_image.height;
                //Debug.Log("flag2, ratio = " + ratio);
                imageUI.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, original_imageUI_height * ratio);
            }
            imageUI.texture = downloaded_image;
        }
    }

    IEnumerator GetAudioMessage(string voice_URL)
    {
        Debug.Log(voice_URL);
        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(voice_URL, AudioType.WAV);
        yield return request.SendWebRequest(); //wait for the request to complete
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log("voice error");
        }
        else
        {
            voiceUI.GetComponent<AudioSource>().clip = DownloadHandlerAudioClip.GetContent(request);
        }
    }

    public void PreviewAudio()
    {
        voiceUI.GetComponent<AudioSource>().Play();
    }

    public void GetData()
    {
        database.Collection("user").Document(userID).Collection(OrderNumber).Document(OrderNumber).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            message = task.Result.ConvertTo<Message>(); //get a message object from firestore database

            textUI.text = message.text;
            //old_image_name = message.image;

            image_ref = storage_img_ref.Child(message.image); //generate image message url
            Debug.Log("task path: " + image_ref);
            //task return an Uri object, so we need to convert it to string
            image_ref.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    //task return a url
                    StartCoroutine(GetImageMessage(Convert.ToString(task.Result)));
                }
                else
                {
                    Debug.Log(task.Exception);
                }

            });

            audio_ref = storage_audio_ref.Child(message.audio); //generate image message url
            //task return an Uri object, so we need to convert it to string
            audio_ref.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    //task return a url
                    StartCoroutine(GetAudioMessage(Convert.ToString(task.Result)));
                }
                else
                {
                    Debug.Log(task.Exception);
                }
            });

            DrinkController.GetComponent<DrinkController>().GetDrink();

        });

        
    }

}


