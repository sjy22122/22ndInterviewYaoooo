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

  ```
  
  ```

### 3. Receiver Page
* Receiver_DataManager
  ```
  code blocks for commands
  ```
* VoiceManagement
  ```
  code blocks for commands
  ```
* GiftController
  ```
  code blocks for commands
  ```
* DrinkController
  ```
  code blocks for commands
  ```
* BottleStateTracker
  ```
  code blocks for commands
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
