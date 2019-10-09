using UnityEngine;
using System;
using System.IO;
using UnityEngine.UI;

public class TouchInputController : MonoBehaviour
{


    // Input.GetTouch example.
    //
    // Attach to an origin based cube.
    // A screen touch moves the cube on an iPhone or iPad.
    // A second screen touch reduces the cube size.
    public int pieMenuSize = 0;
    public int participant = 0;
    public int flexibilityLevel = 0;
    public int sensitivity = 0; // 10 or 5?
    public int trials = 0;
    public int menuCount = 0;
    public int indexMenu = 0;
    public float total = 0;
    public float previousTotal = 0;
    public float successTotal = 0;
    public int failedTotal = 0;
    public int num = 1;
    public int itemCounter = 1;
    float getXChange;
    float getYChange;
    public int[] flexLevelArray = { 1, 2, 3, 4};
    public int[] populatedArray;
    public int[] populatedMenuSize = { 4, 8, 12 };

    int flexRandom;
    public Vector3 preFilteredData;

    private Vector3 position;
    private float width;
    private float height;
    public bool isTouching = false;
    public Vector3 oldPosition;
    float getXPositive;
    float getYPositive;
    float dockingX;
    float dockingY;
    public float dockingRadius;
    public float touchPositionX;
    public float touchPositionY;
    public bool restart = false;
    public int successful = 0;

    public bool guiTrigger = true;
    public float timeElasped = 0.0f;
    public float successRate = 0.0f;
    public float timeAverage = 0.0f;
    public float calibrateX = 0.0f;
    public float calibrateY = 0.0f;
    public string mode;
    //public int tool = 1;
    public int flexLevelArrayIndex = 0;
    public Sprite pie4;
    public Sprite pie8;
    public Sprite pie12;
    public bool taskCompleted = false;
    public string message;
    public Text selectedNumberDisplay;
    public Text selectedMovingNumberDisplay;

    public Text targetNumberDisplay;
    Vector2 pos = new Vector2(0.0f, 0.0f);

    GameObject arduinoControllerObject;
    ArduinoController arduinoController;

    GameObject SceneControllerObject;
    SceneControllerScript sceneController;
    GameObject dockingObject;
    GameObject filteredDockingObject;
    public Button button;
    Image pieMenuDisplay;
    
    void Awake()
    {
        filteredDockingObject = GameObject.Find("FilteredDockingObject");

        arduinoControllerObject = GameObject.FindGameObjectWithTag("ArduinoController");
        arduinoController = arduinoControllerObject.GetComponent<ArduinoController>();

        dockingObject = GameObject.FindGameObjectWithTag("DockingCircle");
        //GameObject.Find("ReferenceCircle").GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/Pie" + pieMenuSize);
        // xRan = Mathf.Cos(angle) * radius;
        // yRan = Mathf.Sin(angle) * radius;

        pieMenuDisplay = GameObject.Find("ReferenceCircle").GetComponent<Image>();
        // targetCircle.transform.position = new Vector3(xRan + dockingCircle.transform.position.x, yRan + dockingCircle.transform.position.y, 0.0f);

        SceneControllerObject = GameObject.FindGameObjectWithTag("SceneController");
        sceneController = SceneControllerObject.GetComponent<SceneControllerScript>();

        width = (float)Screen.width / 2.0f;
        height = (float)Screen.height / 2.0f;
        // Position used for the cube.
        position = new Vector3(0.0f, 0.0f, 0.0f);

    }

    private void Start()
    {
        button = GameObject.Find("Button").GetComponent<Button>();

        //Button btn = button.GetComponent<Button>();
        button.onClick.AddListener(buttonCall);


        GenerateRandomMenuOrder(); // shuffle populatedMenuSize Array
        pieMenuSize = populatedMenuSize[0];
        StudyController(pieMenuSize);

        sceneController.randomNum = GenerateRandomNumber(); // generate pie menu array
        if (!System.IO.File.Exists(Application.dataPath + "/Data/" + "Participant_" + participant  + ".csv"))
        {
            File.WriteAllText(Application.dataPath + "/Data/" + "Participant_" + participant  + ".csv", "");
        }

        //sceneController.RandomObjectGenerator();
        // random number from 1 to 4.
        GenerateRandomFlexibilityOrder();

        flexibilityLevel = flexLevelArray[flexLevelArrayIndex];
    }

    void OnGUI()
    {
        // Compute a fontSize based on the size of the screen width.
        GUI.skin.label.fontSize = (int)(Screen.width / 70.0f);
        GUI.color = Color.black;
        /*  GUI.Label(new Rect(20, 20, width, height * 0.25f),
              "x = " + position.x.ToString("f2") +
              ", y = " + position.y.ToString("f2"));*/

        
    

        GUI.Label(new Rect(450, 300, width * 3.0f, height * 0.55f),
            "Flexibility Level: " + flexLevelArray[flexLevelArrayIndex] +
            " Menu Size: " + pieMenuSize + 
            " Items: " + num + 
            message + " "
            );
    }

    void buttonCall()
    {
        if (taskCompleted == true && (menuCount != (trials * 3)))
        {
            StudyController(pieMenuSize); // generate new array.
            taskCompleted = false;
        }

        if (total == (4 + 8 + 12) * trials  && (populatedArray.Length == 0))
        {

            previousTotal = 0;
             total = 0;
             indexMenu = 0;
             menuCount = 0;
             num = 0;

            GenerateRandomMenuOrder();

            pieMenuSize = populatedMenuSize[0];
            StudyController(pieMenuSize);

            flexLevelArrayIndex += 1;
            flexibilityLevel = flexLevelArray[flexLevelArrayIndex];
           
            
        }
    }

    void Update()
    {
        if (itemCounter == pieMenuSize) itemCounter = 0;

        selectedMovingNumberDisplay.text = sceneController.itemPicked.ToString();
        selectedNumberDisplay.text = sceneController.itemPicked.ToString();
        targetNumberDisplay.text = sceneController.randomNum.ToString();

        PieMenuSizeGenerator();
      
        getXPositive = Mathf.Sign(getXChange);
        getYPositive = Mathf.Sign(getYChange);
        guiTrigger = true;

        //position = new Vector3(pos.x , pos.y, -365.0f);

        /*
        if (Input.GetKeyUp("space"))
        {
            if (taskCompleted == true && (menuCount != (trials * 3)))
            {
                StudyController(pieMenuSize); // generate new array.
                taskCompleted = false;
            }
        }*/


        if (Input.touchCount > 0) // remember me
        {
            Touch touch = Input.GetTouch(0);
            getXChange = float.Parse(arduinoController.str[0]) - calibrateX;
            getYChange = float.Parse(arduinoController.str[1]) - calibrateY;
            pos = touch.position;

            if (isTouching == true)
            {
                pos = touch.position; // pen touch position
                if (sceneController.selectedItem == true)
                {
                    sceneController.randomNum = GenerateRandomNumber();
                    sceneController.selectedItem = false;
                }
                dockingObject.SetActive(true);
                filteredDockingObject.SetActive(true);
                calibrateX = float.Parse(arduinoController.str[0]);
                calibrateY = float.Parse(arduinoController.str[1]);
               // filteredDockingObject.transform.position = new Vector3(pos.x, pos.y);
                isTouching = false;
            }

            
            touchPositionX = touch.position.x;
            touchPositionY = touch.position.y;

            dockingX = touchPositionX - getXChange;
            dockingY = touchPositionY - getYChange;


            preFilteredData = (new Vector3(-(getXChange * sensitivity), -(getYChange * sensitivity)));
            dockingObject.transform.position = new Vector3 (pos.x , pos.y);
            dockingObject.transform.Translate(preFilteredData);

            
            // for old object.
            dockingRadius = Mathf.Pow((Mathf.Pow(touchPositionX - dockingObject.transform.position.x, 2) + Mathf.Pow(touchPositionY - dockingObject.transform.position.y, 2)), 0.5f);
            
            
            // for filterede object.
           // dockingRadius = Mathf.Pow((Mathf.Pow(touchPositionX - filteredDockingObject.transform.position.x, 2) + Mathf.Pow(touchPositionY - filteredDockingObject.transform.position.y, 2)), 0.5f);

            if (restart == true)
                {
                    sceneController.PositionReferenceMarkers(); // change these to position of touch rather than the object.
                    sceneController.PieMenuGenerator(getXChange, getYChange, pieMenuSize);                
                }

            if (touch.phase == TouchPhase.Ended)
            {

                restart = true;
                timeElasped = 0.0f;
            }
        }

        else
        {
            sceneController.DeleteRefernceMarkers();
            dockingObject.SetActive(false);
            filteredDockingObject.SetActive(false);
            isTouching = true;
            successRate = (successTotal / total) * 100;

            if (menuCount == (trials * 3))
            {
                message = " Round " + menuCount + "/" + trials * 3 +
                " Last round.";
               // Application.Quit();
            }

            else if (populatedArray.Length == 0)
            {
               // message = message = " Round " + menuCount + "/" + trials * 3 + " Press Button to continue.";
                message = " Press button to continue.";
                if (taskCompleted == false)
                {
                    timeAverage = timeAverage / pieMenuSize;
                    timeAverage = 0;
                    taskCompleted = true;
                }


            }
            else
            {
                message = " Round " + menuCount + "/" + trials*3;
            }
        }


    }

    public void StudyController(int size)
    {
       

        if (menuCount == trials)
        {
            indexMenu++;
        }
        if (menuCount == (trials * 2))
        {
            indexMenu++;
        }

        menuCount++;

        
        size = populatedMenuSize[indexMenu];
        pieMenuSize = size;
        populatedArray = new int[size];

        for (int i = 0; i < size; i++)
        {
            populatedArray[i] = i + 1;
        }
        
        int tempGO;
        for (int i = 0; i < populatedArray.Length; i++)
        {
            int rnd = UnityEngine.Random.Range(0, populatedArray.Length);
            tempGO = populatedArray[rnd];
            populatedArray[rnd] = populatedArray[i];
            populatedArray[i] = tempGO;
            
        }

        for (int i = 0; i < populatedArray.Length; i++)
        {
            Debug.Log(populatedArray[i]);
        }
        
        num = 0;

    }
    public int GenerateRandomNumber()
    {
        int x;
            x = populatedArray[0];
       
   
        if (total != previousTotal)
        {
            num++;
            previousTotal = total;
        }

        RemoveAt(ref populatedArray, 0);
        return x;
    } // Random Item
    public void GenerateRandomMenuOrder() 
    {
        int tempGO2;
        for (int i = 0; i < populatedMenuSize.Length; i++)
        {
            int rnd = UnityEngine.Random.Range(0, populatedMenuSize.Length);
            tempGO2 = populatedMenuSize[rnd];
            populatedMenuSize[rnd] = populatedMenuSize[i];
            populatedMenuSize[i] = tempGO2;
        }

        int tempGO3;
        for (int i = 0; i < populatedMenuSize.Length; i++)
        {
            int rnd = UnityEngine.Random.Range(0, populatedMenuSize.Length);
            tempGO3 = populatedMenuSize[rnd];
            populatedMenuSize[rnd] = populatedMenuSize[i];
            populatedMenuSize[i] = tempGO3;
        }

    } // Random Menu Order
    public void GenerateRandomFlexibilityOrder()
    {
        int tempGO2;
        for (int i = 0; i < flexLevelArray.Length; i++)
        {
            int rnd = UnityEngine.Random.Range(0, flexLevelArray.Length);
            tempGO2 = flexLevelArray[rnd];
            flexLevelArray[rnd] = flexLevelArray[i];
            flexLevelArray[i] = tempGO2;
        }
    } // Random Flexibility Order
    public static void RemoveAt<T>(ref T[] arr, int index)
    {
        // replace the element at index with the last element
        arr[index] = arr[arr.Length - 1];
        // finally, let's decrement Array's size by one
        Array.Resize(ref arr, arr.Length - 1);
    } // Remove item from an Array
    void PieMenuSizeGenerator()
    {
        if (pieMenuSize == 4)
            pieMenuDisplay.sprite = pie4;
        if (pieMenuSize == 8)
            pieMenuDisplay.sprite = pie8;
        if (pieMenuSize == 12)
            pieMenuDisplay.sprite = pie12;
    }
}
