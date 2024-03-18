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



public class UploadManagement : MonoBehaviour
{
    public string userID;
    public string OrderNumber;
    //for page 1 upload
    public TMP_InputField textUI;
    public RawImage imageUI;
    public Button voiceUI;
    public Button recordUI;
    public Button image_upload_UI;
    //for page 2 upload drink
    public Button Drink_1_UI;
    public Button Drink_2_UI;
    public Button Drink_3_UI;
    public TMP_InputField Drink_Name_UI;
    public Button Drink_Decoration_UI;
    public TMP_Text drink_name_preview;
    public Button Edit_Drink;
    public Button Back_Drink;
    public Material liquid_colour_preview;
    public GameObject theDrink_decoration;

    public GameObject screen_canvas;

    FirebaseFirestore database;
    FirebaseStorage database_media;
    StorageReference storage_img_ref;
    StorageReference storage_audio_ref;
    StorageReference image_ref;
    StorageReference audio_ref;


    //public AudioType audioType;

    MicController mic; //has reference, from asset

    Texture2D downloaded_image;

    //ListenerRegistration listenerRegistration;

    Message message;

    public float original_imageUI_width;
    public float original_imageUI_height;

    //bool IsRecording;

    
    string Drink1; //A, B, C
    string Drink2;
    string Drink3;
    int DrinkDecoration;  //1 - 3
    string DrinkName;

    public Sprite bottleA_tex;
    public Sprite bottleB_tex;
    public Sprite bottleC_tex;
    public Sprite Decoration1_tex;
    public Sprite Decoration2_tex;
    public Sprite Decoration3_tex;
    public GameObject Edit_Drink_Page;

    public GameObject liquid_object;
    Color bottleA_color = new Color(241.0f / 255.0f, 103.0f / 255.0f, 103.0f / 255.0f); //red
    Color bottleB_color = new Color(164.0f / 255.0f, 89.0f / 255.0f, 209.0f / 255.0f); //purple
    Color bottleC_color = new Color(255.0f / 255.0f, 184.0f / 255.0f, 76.0f / 255.0f); //orange

    IEnumerator GetImageMessage(string image_URL)
    {
        
        
        //Texture2D texture = NativeGallery.LoadImageAtPath(image_URL, 1024); //max size

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
            this.GetComponent<AudioSource>().clip = DownloadHandlerAudioClip.GetContent(request);
        }
    }

    

    // Start is called before the first frame update
    void Start()
    {
        if (LoginManagement.input_userID != null && LoginManagement.input_OrderNumber != null)
        {
            userID = LoginManagement.input_userID;
            OrderNumber = LoginManagement.input_OrderNumber;
            Debug.Log("LoginManagement.input_userID = " + LoginManagement.input_userID + "; LoginManagement.input_OrderNumber = " + LoginManagement.input_OrderNumber);
        }
        else
        {
            userID = "Yao";
            OrderNumber = "123";
        }

        

        mic = this.GetComponent<MicController>();
        mic._audioSource = this.GetComponent<AudioSource>();
        //IsRecording = false;
        

        database = FirebaseFirestore.DefaultInstance;
        database_media = FirebaseStorage.DefaultInstance;
        storage_img_ref = database_media.GetReferenceFromUrl("gs://secret-drinks-club.appspot.com/" + userID + "/" + OrderNumber + "/image/");
        storage_audio_ref = database_media.GetReferenceFromUrl("gs://secret-drinks-club.appspot.com/" + userID + "/" + OrderNumber + "/audio/");

        original_imageUI_width = imageUI.GetComponent<RectTransform>().rect.width; // the max size of the image
        original_imageUI_height = imageUI.GetComponent<RectTransform>().rect.height;

        

        GetData(); //get data from first time

        if (Drink1 == "" && Drink2 == "" && Drink3 == "") //if the drink havent set up yet,disable the liquid object
        {
            liquid_object.SetActive(false);
        }
        else
        {
            liquid_object.SetActive(true);
        }
        if (DrinkDecoration == 0)
        {
            theDrink_decoration.transform.GetChild(0).gameObject.SetActive(false);
            theDrink_decoration.transform.GetChild(1).gameObject.SetActive(false);
            theDrink_decoration.transform.GetChild(2).gameObject.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("textUI.text = " + textUI.text);
    }

    //https://discussions.unity.com/t/rotate-the-contents-of-a-texture/136686
    Texture2D rotateTexture(Texture2D originalTexture, bool clockwise)
    {
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];
        int w = originalTexture.width;
        int h = originalTexture.height;

        int iRotated, iOriginal;

        for (int j = 0; j < h; ++j)
        {
            for (int i = 0; i < w; ++i)
            {
                iRotated = (i + 1) * h - j - 1;
                iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }

        Texture2D rotatedTexture = new Texture2D(h, w);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;
    }

    public void UploadText()
    {
        //update text message
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "text", textUI.text}
        };
        //upload update to database
        database.Collection("user").Document(userID).Collection(OrderNumber).Document(OrderNumber).UpdateAsync(updates).ContinueWithOnMainThread(task =>
        {
            Debug.Log("upload text message");
        });
    }

    public void UploadImage()
    {
        //pick the image from gallery
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((image_local_path) =>
        {
            //Debug.Log("get property: " + NativeGallery.GetImageProperties(image_local_path).width + " * " + NativeGallery.GetImageProperties(image_local_path).height);
            //Debug.Log("get property rotate: " + NativeGallery.GetImageProperties(image_local_path).orientation);

            //check if the image is considered as "rotated"
            string image_rotate = Convert.ToString(NativeGallery.GetImageProperties(image_local_path).orientation);
            
            if (image_local_path != null)
            {
                //can delete
                Texture2D texture = NativeGallery.LoadImageAtPath(image_local_path, 1024); //max size
                if (texture == null)
                {

                    Debug.Log("Couldn't load texture from " + image_local_path);
                    return;
                }
                else
                {
                    if (image_rotate== "Rotate90") //if original image has rotated 90
                    {
                        //Debug.Log("image is Rotate90");
                        rotateTexture(texture,true);
                    }
                    byte[] Tex_bytes = texture.EncodeToPNG(); //save texture to png
                    string save_texture_path = Path.Combine(Application.persistentDataPath, "MyImage.png");
                    File.WriteAllBytes(save_texture_path, Tex_bytes); //save texture to folder

                    //tell firebase what is the image type
                    var newMetadata = new MetadataChange();
                    newMetadata.ContentType = "image/png";
                    StorageReference save_folder_ref = storage_img_ref.Child("user_image.png"); //what name should the image saved as

                    //string androidPath = string.Format("{0}://{1}".Uri.UriSchemeFile, image_local_path);
                    string androidPath = "file://" + save_texture_path;
                    

                    //upload image to storage database
                    save_folder_ref.PutFileAsync(androidPath, newMetadata, null, CancellationToken.None).ContinueWith((Task<StorageMetadata> task) =>
                    {
                        if (task.IsFaulted || task.IsCanceled)
                        {
                            Debug.Log(task.Exception.ToString());
                        }
                        else
                        {
                            // Metadata contains file metadata such as size, content-type, and download URL.
                            StorageMetadata metadata = task.Result;
                            string md5Hash = metadata.Md5Hash;
                            Debug.Log("Finished uploading...");
                            Debug.Log("md5 hash = " + md5Hash);

                            if (message.image=="")
                            {
                                //update image path in database
                                Dictionary<string, object> updates = new Dictionary<string, object>
                                {
                                    { "image", "user_image.png"}
                                };
                                //upload update to database
                                database.Collection("user").Document(userID).Collection(OrderNumber).Document(OrderNumber).UpdateAsync(updates).ContinueWithOnMainThread(task =>
                                {
                                    Debug.Log("upload image name");
                                });
                            }
                            

                            GetData();
                        }
                    });
                }
            }
        });
    }


    public void HoldToRecord()
    {
        recordUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Recording...";
        mic.WorkStart();
    }

    public void EndRecord()
    {
        recordUI.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Hold to record";
        mic.WorkStop();
        string audioPath = Path.Combine(Application.persistentDataPath, "MyFile.wav");
        byte[] wavFile = OpenWavParser.AudioClipToByteArray(mic._audioSource.clip);
        File.WriteAllBytes(audioPath, wavFile);
        UploadAudio(audioPath);
    }

    public void PreviewAudio()
    {
        Debug.Log("play audio");
        this.GetComponent<AudioSource>().Play();
    }

    public void UploadAudio(string audioPath)
    {
        var newMetadata = new MetadataChange();
        newMetadata.ContentType = "audio/wav";
        StorageReference save_folder_ref = storage_audio_ref.Child("user_audio.wav");//what name should the audio saved as
        string androidPath = "file://" + audioPath;

        //upload audio to database
        save_folder_ref.PutFileAsync(androidPath, newMetadata, null, CancellationToken.None).ContinueWith((Task<StorageMetadata> task) =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString());
            }
            else
            {
                // Metadata contains file metadata such as size, content-type, and download URL.
                StorageMetadata metadata = task.Result;
                string md5Hash = metadata.Md5Hash;
                Debug.Log("Finished uploading...");
                Debug.Log("md5 hash = " + md5Hash);
            }
        });

        if (message.audio == "")
        {
            //update audio path which saved in database
            Dictionary<string, object> updates = new Dictionary<string, object>
                {
                    { "audio", "user_audio.wav"}
                };
            //upload update to database
            database.Collection("user").Document(userID).Collection(OrderNumber).Document(OrderNumber).UpdateAsync(updates).ContinueWithOnMainThread(task =>
            {
                Debug.Log("upload audio name");
            });
        }
        
    }


    public void Made_Drink_click()
    {
        screen_canvas.GetComponent<RectTransform>().anchoredPosition = new Vector2( -screen_canvas.GetComponent<RectTransform>().rect.width, 0.0f);
        
    }


    public void UploadDrink()
    {
        //update text message
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "Drink1", Drink1},
            { "Drink2", Drink2},
            { "Drink3", Drink3},
            { "DrinkDecoration", DrinkDecoration},
            { "DrinkName", Drink_Name_UI.text},
        };
        //upload update to database
        database.Collection("user").Document(userID).Collection(OrderNumber).Document(OrderNumber).UpdateAsync(updates).ContinueWithOnMainThread(task =>
        {
            Debug.Log("upload drink message");
        });

        Edit_Drink_Page.SetActive(false);
        Edit_Drink.interactable = true;
        Back_Drink.interactable = true;

        /* preview the drink name */
        if (Drink_Name_UI.text != "")
        {
            drink_name_preview.text = '"' + Drink_Name_UI.text + '"';
        }

        /* preview the drink */
        if (Drink1 == "A")
        {
            liquid_colour_preview.SetColor("_BottomColour", bottleA_color);
        }
        else if (Drink1 == "B")
        {
            liquid_colour_preview.SetColor("_BottomColour", bottleB_color);
        }
        else if (Drink1 == "C")
        {
            liquid_colour_preview.SetColor("_BottomColour", bottleC_color);
        }
        if (Drink2 == "A")
        {
            liquid_colour_preview.SetColor("_MiddleColour", bottleA_color);
        }
        else if (Drink2 == "B")
        {
            liquid_colour_preview.SetColor("_MiddleColour", bottleB_color);
        }
        else if (Drink2 == "C")
        {
            liquid_colour_preview.SetColor("_MiddleColour", bottleC_color);
        }
        if (Drink3 == "A")
        {
            liquid_colour_preview.SetColor("_TopColour", bottleA_color);
        }
        else if (Drink3 == "B")
        {
            liquid_colour_preview.SetColor("_TopColour", bottleB_color);
        }
        else if (Drink3 == "C")
        {
            liquid_colour_preview.SetColor("_TopColour", bottleC_color);
        }
        liquid_object.SetActive(true);

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

    public void Edit_click()
    {
        Edit_Drink_Page.SetActive(true);
        Edit_Drink.interactable = false;
        Back_Drink.interactable = false;
    }
    public void Drink_Back_click()
    {
        screen_canvas.GetComponent<RectTransform>().anchoredPosition = new Vector2(0.0f, 0.0f);
    }
    public void Drink1_click()
    {
        if (Drink1!="") //when drink 1 is not null
        {
            if (Drink1=="A")
            {
                Drink1 = "B";
                Drink_1_UI.GetComponent<Image>().sprite = bottleB_tex;
            }
            else if (Drink1 == "B")
            {
                Drink1 = "C";
                Drink_1_UI.GetComponent<Image>().sprite = bottleC_tex;
            }
            else if (Drink1 == "C")
            {
                Drink1 = "A";
                Drink_1_UI.GetComponent<Image>().sprite = bottleA_tex;
            }
        }
        else //when drink1 is null and user click the button
        {
            Drink1 = "A";
            Drink_1_UI.GetComponent<Image>().sprite = bottleA_tex;
            Drink_1_UI.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    public void Drink2_click()
    {
        if (Drink2 != "") //when drink 2 is not null
        {
            if (Drink2 == "A")
            {
                Drink2 = "B";
                Drink_2_UI.GetComponent<Image>().sprite = bottleB_tex;
            }
            else if (Drink2 == "B")
            {
                Drink2 = "C";
                Drink_2_UI.GetComponent<Image>().sprite = bottleC_tex;
            }
            else if (Drink2 == "C")
            {
                Drink2 = "A";
                Drink_2_UI.GetComponent<Image>().sprite = bottleA_tex;
            }
        }
        else //when drink1 is null and user click the button
        {
            Drink2 = "A";
            Drink_2_UI.GetComponent<Image>().sprite = bottleA_tex;
            Drink_2_UI.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    public void Drink3_click()
    {
        if (Drink3 != "") //when drink 3 is not null
        {
            if (Drink3 == "A")
            {
                Drink3 = "B";
                Drink_3_UI.GetComponent<Image>().sprite = bottleB_tex;
            }
            else if (Drink3 == "B")
            {
                Drink3 = "C";
                Drink_3_UI.GetComponent<Image>().sprite = bottleC_tex;
            }
            else if (Drink3 == "C")
            {
                Drink3 = "A";
                Drink_3_UI.GetComponent<Image>().sprite = bottleA_tex;
            }
        }
        else //when drink1 is null and user click the button
        {
            Drink3 = "A";
            Drink_3_UI.GetComponent<Image>().sprite = bottleA_tex;
            Drink_3_UI.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
    public void DrinkDecoration_click()
    {
        if (DrinkDecoration != 0) //when drink 3 is not null
        {
            if (DrinkDecoration == 1)
            {
                DrinkDecoration = 2;
                Drink_Decoration_UI.GetComponent<Image>().sprite = Decoration2_tex;
            }
            else if (DrinkDecoration == 2)
            {
                DrinkDecoration = 3;
                Drink_Decoration_UI.GetComponent<Image>().sprite = Decoration3_tex;
            }
            else if (DrinkDecoration == 3)
            {
                DrinkDecoration = 1;
                Drink_Decoration_UI.GetComponent<Image>().sprite = Decoration1_tex;
            }
        }
        else //when drink1 is null and user click the button
        {
            DrinkDecoration = 1;
            Drink_Decoration_UI.GetComponent<Image>().sprite = Decoration1_tex;
            Drink_Decoration_UI.transform.GetChild(0).gameObject.SetActive(false);
        }
    }




    public void GetData()
    {
        database.Collection("user").Document(userID).Collection(OrderNumber).Document(OrderNumber).GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            message = task.Result.ConvertTo<Message>(); //get a message object from firestore database

            /* getting text message */
            textUI.text = message.text;

            /* getting image message */
            image_ref = storage_img_ref.Child(message.image); //generate image message url for firebase storage
            image_ref.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>    //task return an Uri object, so we need to convert it to string
            {
                if (!task.IsFaulted && !task.IsCanceled) 
                {
                    StartCoroutine(GetImageMessage(Convert.ToString(task.Result)));//task return a url
                }
                else
                {
                    Debug.Log(task.Exception);
                }
            });

            /* getting audio message */
            audio_ref = storage_audio_ref.Child(message.audio); //generate audio message url for firebase storage
            audio_ref.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
            {
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    StartCoroutine(GetAudioMessage(Convert.ToString(task.Result))); //task return an Uri object, so we need to convert it to string
                }
                else
                {
                    Debug.Log(task.Exception);
                }
            });

            /* getting drink message */
            Drink1 = message.Drink1;
            Drink2 = message.Drink2;
            Drink3 = message.Drink3;
            DrinkDecoration = message.DrinkDecoration;
            DrinkName = message.DrinkName;

            if (DrinkName!="")
            {
                drink_name_preview.text = '"' + DrinkName + '"';
            }

            if (DrinkName!="")
            {
                Drink_Name_UI.transform.GetChild(0).transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = DrinkName;
            }
            if (message.Drink1 != "") //when drink 1 is not null
            {
                if (message.Drink1 == "A")
                {
                    Drink_1_UI.GetComponent<Image>().sprite = bottleA_tex;
                    Drink_1_UI.transform.GetChild(0).gameObject.SetActive(false);
                }
                else if (message.Drink1 == "B")
                {
                    Drink_1_UI.GetComponent<Image>().sprite = bottleB_tex;
                    Drink_1_UI.transform.GetChild(0).gameObject.SetActive(false);
                }
                else if (message.Drink1 == "C")
                {
                    Drink_1_UI.GetComponent<Image>().sprite = bottleC_tex;
                    Drink_1_UI.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
            if (message.Drink2 != "") //when drink 2 is not null
            {
                if (message.Drink2 == "A")
                {
                    Drink_2_UI.GetComponent<Image>().sprite = bottleA_tex;
                    Drink_2_UI.transform.GetChild(0).gameObject.SetActive(false);
                }
                else if (message.Drink2 == "B")
                {
                    Drink_2_UI.GetComponent<Image>().sprite = bottleB_tex;
                    Drink_2_UI.transform.GetChild(0).gameObject.SetActive(false);
                }
                else if (message.Drink2 == "C")
                {
                    Drink_2_UI.GetComponent<Image>().sprite = bottleC_tex;
                    Drink_2_UI.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
            if (message.Drink3 != "") //when drink 3 is not null
            {
                if (message.Drink3 == "A")
                {
                    Drink_3_UI.GetComponent<Image>().sprite = bottleA_tex;
                    Drink_3_UI.transform.GetChild(0).gameObject.SetActive(false);
                }
                else if (message.Drink3 == "B")
                {
                    Drink_3_UI.GetComponent<Image>().sprite = bottleB_tex;
                    Drink_3_UI.transform.GetChild(0).gameObject.SetActive(false);
                }
                else if (message.Drink3 == "C")
                {
                    Drink_3_UI.GetComponent<Image>().sprite = bottleC_tex;
                    Drink_3_UI.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
            if (DrinkDecoration != 0) 
            {
                if (DrinkDecoration == 1)
                {
                    Drink_Decoration_UI.GetComponent<Image>().sprite = Decoration1_tex;
                    Drink_Decoration_UI.transform.GetChild(0).gameObject.SetActive(false);
                }
                else if (DrinkDecoration == 2)
                {
                    Drink_Decoration_UI.GetComponent<Image>().sprite = Decoration2_tex;
                    Drink_Decoration_UI.transform.GetChild(0).gameObject.SetActive(false);
                }
                else if (DrinkDecoration == 3)
                {
                    Drink_Decoration_UI.GetComponent<Image>().sprite = Decoration3_tex;
                    Drink_Decoration_UI.transform.GetChild(0).gameObject.SetActive(false);
                }
            }

            /* preview the drink */
            if (Drink1 == "A")
            {
                liquid_colour_preview.SetColor("_BottomColour", bottleA_color);
            }
            else if (Drink1 == "B")
            {
                liquid_colour_preview.SetColor("_BottomColour", bottleB_color);
            }
            else if (Drink1 == "C")
            {
                liquid_colour_preview.SetColor("_BottomColour", bottleC_color);
            }
            if (Drink2 == "A")
            {
                liquid_colour_preview.SetColor("_MiddleColour", bottleA_color);
            }
            else if (Drink2 == "B")
            {
                liquid_colour_preview.SetColor("_MiddleColour", bottleB_color);
            }
            else if (Drink2 == "C")
            {
                liquid_colour_preview.SetColor("_MiddleColour", bottleC_color);
            }
            if (Drink3 == "A")
            {
                liquid_colour_preview.SetColor("_TopColour", bottleA_color);
            }
            else if (Drink3 == "B")
            {
                liquid_colour_preview.SetColor("_TopColour", bottleB_color);
            }
            else if (Drink3 == "C")
            {
                liquid_colour_preview.SetColor("_TopColour", bottleC_color);
            }

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
        });
    }
}
