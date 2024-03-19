# AR mobile application for hybrid gifting
Gifting has essential meaning from both social and economic perspectives. As a social process, it helps to build bonds between people.
Hybrid gifts are the type of gifting that combine physical content with digital content. They are composed of a physical gift with digital wrapping or vice versa. 

The research is going to explore a novel form of hybrid gift which allows givers to create customized AR experiences for receivers. We call the new implementation as “game-like” AR technology in hybrid gift. It should enhance the user experience and engagement of hybrid gifts, as well as creating hybrid gifts that are valued by receivers.

## Description

It is a AR moblie application. The users are able to interact with the AR content through physical objects. This application allows users to login as either "Giver" or "Receiver". Depending on the user's status, the corresponding page shows up and provides different features to the user. 

There are four pages in the application.
* 1 - Home Page
* 2 - Login Page
* 3 - Giver Page
* 4 - Receiver Page

## Code
Some parts of the code may be simplified to improve readability.

### 1. Common documents
* CanvasResizeManager.cs
  * This file is used for Canvas auto-resizing. However it was not used in the final prototype.

  ```
  float Screen_Width;
  float Screen_Height;
  void Start()
  {
      Screen_Width = this.gameObject.GetComponent<RectTransform>().rect.width;
      Screen_Height = this.gameObject.GetComponent<RectTransform>().rect.height;
  }
  ```
  
* GettingMessage.cs
  * This file is used for getting an "Message" object from Firebase. The object contains all the information about this hybrid gift. Once we get the object, we can change the game objects correspondingly.
  
  ```
  using Firebase.Firestore;

  [FirestoreData]
  public struct Message
  {
    [FirestoreProperty]
    public string Drink1 { get; set; }

    [FirestoreProperty]
    public int DrinkDecoration { get; set; }

    (...)
  }
  ```

* LoginManagement.cs
  * This is used for Login. In Home Page, the user can choose either "Login as Giver" or "Login as Receiver". After that, the user will be sent to Login Page.
  * In Login Page, once the user click the login button, the system is going to check the "userID" and "OrderNumber".
  * If the details are not correct, the user is able to enter the details again.

  ```
  public class LoginManagement : MonoBehaviour
  {
    (...)
    public static string input_userID; //can be used later
    public static string input_OrderNumber; //can be used later
    FirebaseFirestore database; //Firebase database object
    (...)
  
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
      
      Function1 //check the details in database, then get the result back
      {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists == false) // if details are wrong
            {
                Debug.Log("order does not exist!");
                (...)
            }
            else // if details are correct
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
  ```
* ScenesManagement.cs
  * This function is used for transferring the user between different pages.
  ```
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
  
  }
  ```

### 2. Giver Page
* UploadManagement.cs
  * The giver can upload image from his or her mobile phone
  * The giver can record 20s voice message
  * The giver can upload text message
  * The giver can make the customized recipe
  * All the information of this hybrid gift can be uploaded and downloaded from the database

  ```
  public class UploadManagement : MonoBehaviour
  {
    public string userID;
    public string OrderNumber;
    //for page 1 upload
    (...public objects * 5)
    //for page 2 upload drink
    (...public objects * 10)

    public GameObject screen_canvas;

    // For Firebase
    (...Firebase objects and path references * 6)
  
    MicController mic; //has reference, from Asset Store

    Texture2D downloaded_image; // image downloaded from database

    Message message; // Firebase "Message" object

    public float original_imageUI_width;
    public float original_imageUI_height;
  
    string Drink1; //A, B, C
    string Drink2;
    string Drink3;
    int DrinkDecoration;  //1 - 3
    string DrinkName;

    // For game objects in Unity
    (...public objects * 7)

    // For the liquid in the glass
    public GameObject liquid_object;
  
    Color bottleA_color = new Color(241.0f / 255.0f, 103.0f / 255.0f, 103.0f / 255.0f); //red
    Color bottleB_color = new Color(164.0f / 255.0f, 89.0f / 255.0f, 209.0f / 255.0f); //purple
    Color bottleC_color = new Color(255.0f / 255.0f, 184.0f / 255.0f, 76.0f / 255.0f); //orange

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
            //first, get the image from database

            /* resize the Raw Image base on the ratio of the downloaded image */
            if (downloaded_image.width > downloaded_image.height)
            {
                (...)
            }
            else
            {
                (...)
            }
            imageUI.texture = downloaded_image;
        }
    }

    IEnumerator GetAudioMessage(string voice_URL)
    {
        (...)
    }

    void Start()
    {
        if (LoginManagement.input_userID != null && LoginManagement.input_OrderNumber != null)
        {
            (...) //Firstly, Get the userID and OrderNumber
        }
        else
        {
            (...)
        }

        (...Variables initialization)

        GetData(); //get data from database for the first time

        if (Drink1 == "" && Drink2 == "" && Drink3 == "") //if the drink havent set up yet,disable the liquid object
        {
            liquid_object.SetActive(false);
        }
        else
        {
            liquid_object.SetActive(true);
        }

        (...)

        //https://discussions.unity.com/t/rotate-the-contents-of-a-texture/136686
        Texture2D rotateTexture(Texture2D originalTexture, bool clockwise)
        {
            (...Rotate the image)
        }

        public void UploadText()
        {
            (...)
        }

        public void UploadImage()
        {
            //pick the image from gallery
            NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((image_local_path) =>
            {
                //check if the image is considered as "rotated"
                string image_rotate = Convert.ToString(NativeGallery.GetImageProperties(image_local_path).orientation);
                
                if (image_local_path != null)
                {
                    Texture2D texture = NativeGallery.LoadImageAtPath(image_local_path, 1024); //max size
                    if (texture == null)
                    {
                        return;
                    }
                    else
                    {
                        if (image_rotate== "Rotate90") //if original image has rotated 90
                        {
                            rotateTexture(texture,true);
                        }
                        byte[] Tex_bytes = texture.EncodeToPNG(); //save texture to png
                        string save_texture_path = Path.Combine(Application.persistentDataPath, "MyImage.png");
                        File.WriteAllBytes(save_texture_path, Tex_bytes); //save texture to folder
    
                        //tell firebase what is the image type
                        var newMetadata = new MetadataChange();
                        newMetadata.ContentType = "image/png";
                        StorageReference save_folder_ref = storage_img_ref.Child("user_image.png"); //what name should the image saved as
  
                        string androidPath = "file://" + save_texture_path;
  
                        //upload image to storage database
                        save_folder_ref.PutFileAsync(androidPath, newMetadata, null, CancellationToken.None).ContinueWith((Task<StorageMetadata> task) =>
                        {
                            (...)
                        });
                    }
                }
            });
        }

        public void HoldToRecord(){(...)}
        public void EndRecord(){(...)}
        public void PreviewAudio(){(...)}
        public void UploadAudio(string audioPath){(...)}

        public void Made_Drink_click(){(...)} // sent user to drink making page

        public void UploadDrink(){(...)}

        (public functions for making the customized drink * 6)

        public void GetData(){(...)} // get data from database
  
  }
  ```

### 3. Receiver Page
* Receiver_DataManager.cs
  * Similar to "UploadManagement.cs".
  * For getting the data from Firebase database
  
* VoiceManagement.cs
  * Controlling bartender's animation and voice.

* GiftController.cs
  * Controlling the "Gifts" screen
  * Controlling the displayment and interaction of the hybrid gift
  * Using Vuforia

* DrinkController.cs
  * Controlling the display of the customized drink
  * Using Vuforia
  * Using shader to change the colour of the liquid
  
* BottleStateTracker.cs
  * To detect whether the bottle is pouring liquid.
  ```
  (...)

  void Update()
  {
      Vector3 forward = tracker1.transform.TransformDirection(Vector3.forward) * ray_distance;
      forward = tracker2.transform.TransformDirection(Vector3.forward) * ray_distance;
      forward = tracker3.transform.TransformDirection(Vector3.forward) * (ray_distance + 0.5f);
  }

  private void FixedUpdate()
  {
      (...)

      // Does the ray intersect any objects excluding the player layer
      if (hit_result_1 || hit_result_2 || hit_result_3)
      {
          (...)
      }
      else
      {
          isPouring = false;
      }
  }
  
  ```


## Tools
There are three tools used in this project for application development.

* Unity 3D
  * It is a mature game development tool, with a strong community, AR support, and a rich resource library. It allows developers to switch and build for different platforms, such as Android, IOS, PC, etc. The plugins and assets in the Unity Asset Store can also be used across different platform. Unity also has utility as a medium which connects the database, application and AR engine together. It’s easy to use, and it is capable of complex interaction building. 

* Vuforia
  * Vuforia can be exported to both Android and IOS platforms. It’s simple to use and offers multiple styles of tracker for the object, for example a cylinder. There are three bottles in the box, which are a perfect candidate for use of the cylinder tracker.

* Firebase
  * The accessibility of the AR application for the receivers is essential. Firebase is simple to use and easy to manage. It provides a satisfactory solution for data storage and transmission between different platforms. It also has strong features such as Firestore database and Cloud storage. It’s stable and has scalability.

## Others
* Links
    * [Demo video](https://vimeo.com/864371345?share=copy)
    * [APK (screen size is fixed)](https://drive.google.com/file/d/1v6BLsK_7Sa0bQtlpITBR7XsuSEhuMr85/view)
    * [Unity File](https://drive.google.com/file/d/1jqVau3Nw5XsKb75BnauutUqzUMZTPt7W/view)
