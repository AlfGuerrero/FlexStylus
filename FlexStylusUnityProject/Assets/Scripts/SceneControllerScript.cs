using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;
public class SceneControllerScript : MonoBehaviour
{
    GameObject targetCircle;
    GameObject dockingCircle;
    GameObject referenceCircle;
    GameObject touchControllerObject;
    TouchInputController touchController;
    GameObject mainView;
    GameObject[] targetObjects;
    GameObject dockingObject;
    GameObject pieItem;
    GameObject selectedMovingTextObject;
    Image pieImage;
    GameObject pieTarget;
    Image imageTarget;
    public bool turnOn = false;
    float prev_angle;
    public int randomNum = 0;
    public float dockingAngle = 0.0f;

    float targetObjectPosX;
    float targetObjectPosY;
    public float angle = 0.0f;
    public int choosen = 0;
    float offsetRotation;
    float xRan;
    float yRan;
    float radius = 100.0f;
    public bool selectedItem = false;
    public string filePath;
    StreamWriter writer;
    public float newDockingAngle = 0.0f;
    public int itemPicked;
    public Sprite item4;
    public Sprite item8;
    public Sprite item12;

    // Start is called before the first frame update
    void Start()
    {
        pieItem = GameObject.Find("Item");
        pieImage = GameObject.Find("Item").GetComponent<Image>();
        pieImage.color = new Vector4(1, 0, 0, 0.55f);
        selectedMovingTextObject = GameObject.FindGameObjectWithTag("selectedTextObject");
        pieTarget = GameObject.Find("Target");
        imageTarget = GameObject.Find("Target").GetComponent<Image>();
        imageTarget.color = new Vector4(0, 1, 0, 0.30f);

          
        touchControllerObject = GameObject.FindGameObjectWithTag("TouchController");
        touchController = touchControllerObject.GetComponent<TouchInputController>();

        filePath = getPath();
        File.AppendAllText(filePath, "date, time, id , flexibility, sensitivity, size, itemCount, round, count, selection, target, success, timeTaken, \n");

        targetCircle = GameObject.FindGameObjectWithTag("TargetCircle");
        dockingCircle = GameObject.FindGameObjectWithTag("DockingCircle");
        referenceCircle = GameObject.FindGameObjectWithTag("ReferenceCircle");
        dockingObject = GameObject.Find("DockingObject");
        referenceCircle.transform.position = dockingCircle.transform.position;
         
    }

  

    public void PieMenuGenerator(float xDir, float yDir, int size)
    {
        
        selectedMovingTextObject.SetActive(false);
        pieItem.SetActive(false);
        pieTarget.SetActive(false);
        ItemGenerator();
        touchController.timeElasped += Time.deltaTime;
        float angle = 0.0f;
        float targetAngle = 0.0f;
        float[] targetObjectX = new float[size];
        float[] targetObjectY = new float[size];
        float dockingMagnitude = 0.0f;
        float xPos = xDir;
        float yPos = yDir;
        angle = ((2 * Mathf.PI) / size );

        dockingMagnitude = Mathf.Pow(Mathf.Pow(xPos, 2.0f) + Mathf.Pow(yPos, 2.0f), 0.5f);
        dockingAngle = Mathf.Atan2(yPos, xPos);
        dockingAngle = dockingAngle * (180 / Mathf.PI);
        if (Mathf.Sign(dockingAngle) == -1.0f)
        {
            dockingAngle = Map(dockingAngle, -180.0f, 0.0f, 180.0f, 360.0f);
        }
        targetAngle = angle;


        //  for (int i = 1; i <= size; i++)
        //  {
            pieTarget.SetActive(true);
            pieTarget.transform.position = new Vector3(touchController.touchPositionX, touchController.touchPositionY, 0.0f);
            DrawPieTarget(touchController.pieMenuSize, randomNum);

            // Entering selection mode.
            if (touchController.dockingRadius > 45.46f)
            {
                pieItem.SetActive(true);
                pieItem.transform.position = new Vector3(touchController.touchPositionX, touchController.touchPositionY, 0.0f);
                DrawPieItem(touchController.pieMenuSize, dockingAngle);
               

                // Exiting selection mode.
                if (touchController.dockingRadius > 170.0f) //150 originally. 
                {
                    DeleteRefernceMarkers();        // Delete menu 
                    selectedMovingTextObject.SetActive(true);
                    if (itemPicked == randomNum)
                    {
                        touchController.successTotal += 1;
                        touchController.successful = 1;

                    }
                    else
                    {
                        touchController.failedTotal += 1;
                        touchController.successful = 0;
                    }
                    selectedItem = true;
                    touchController.total += 1;
                    touchController.itemCounter += 1;
                    touchController.timeAverage += touchController.timeElasped;
                    touchController.restart = false;

                    File.AppendAllText(filePath, 
                           System.DateTime.Now.ToShortDateString() + "," +
                           System.DateTime.Now.ToLongTimeString() + "," +
                           touchController.participant + ", " +
                           touchController.flexibilityLevel + "," +
                           touchController.sensitivity + "," + 
                           touchController.pieMenuSize + "," +
                           touchController.itemCounter + "," + 
                           touchController.menuCount + "," +        
                           touchController.total + "," + 
                           itemPicked + ", " +
                           randomNum + ", " +
                           touchController.successful + ", " +
                           touchController.timeElasped + "\n");           
                    return;
                }
            //}
        }
    }

    public void DrawPieTarget(int pieMenuSize, int targetItemNumber)
    {
        if (pieMenuSize == 4)
        {
            pieTarget.transform.localScale = new Vector3(1.19f, 1.19f, 0.0f);
            pieTarget.transform.rotation = Quaternion.Euler(0, 0, ((360/4) * targetItemNumber) - 90);

        }
        if (pieMenuSize == 8)
        {
            pieTarget.transform.localScale = new Vector3(0.85f, 0.85f, 0.0f);
            pieTarget.transform.rotation = Quaternion.Euler(0, 0, ((360/8) *targetItemNumber) - 45 );

        }
        if (pieMenuSize == 12)
        {
            pieTarget.transform.localScale = new Vector3(0.6f, 0.6f, 0.0f);
            pieTarget.transform.rotation = Quaternion.Euler(0, 0, ((360/12) * targetItemNumber) - 30);
        }
    }
    public void DrawPieItem(int pieMenuSize, float dockingItemAngle)
    {        
        if (touchController.dockingRadius > 45.46f) // draw the red item...
        {
            if (pieMenuSize == 4)
            {
                pieItem.transform.localScale = new Vector3(1.19f, 1.19f, 0.0f);

                if (0 < dockingItemAngle && dockingItemAngle <= 90)
                {
                    itemPicked = 3;
                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 180);
                }
                else if (90 < dockingItemAngle && dockingItemAngle <= 180)
                {
                    itemPicked = 4;
                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 270);
                }
                else if (180 < dockingItemAngle && dockingItemAngle <= 270)
                {
                    itemPicked = 1;
                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else if (270 < dockingItemAngle && dockingItemAngle <= 360)
                {
                    itemPicked = 2;
                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 90);
                }


            }
            if (pieMenuSize == 8)
            {
                pieItem.transform.localScale = new Vector3(0.85f, 0.85f, 0.0f);

                if (0 < dockingItemAngle && dockingItemAngle <= 45)
                {
                    itemPicked = 5;
                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 180);
                }
                else if (45 < dockingItemAngle && dockingItemAngle <= 90)
                {
                    itemPicked = 6;
                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 225);
                }
                else if (90 < dockingItemAngle && dockingItemAngle <= 135)
                {
                    itemPicked = 7;
                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 270);
                }
                else if (135 < dockingItemAngle && dockingItemAngle <= 180)
                {
                    itemPicked = 8;
                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 315);
                }
                else if (180 < dockingItemAngle && dockingItemAngle <= 225)
                {
                    itemPicked = 1;
                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 360);
                }
                else if (225 < dockingItemAngle && dockingItemAngle <= 270)
                {
                    itemPicked = 2;
                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 45);
                }
                else if (270 < dockingItemAngle && dockingItemAngle <= 315)
                {
                    itemPicked = 3;
                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 90);
                }
                else if (315 < dockingItemAngle && dockingItemAngle <= 360)
                {
                    itemPicked = 4;

                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 135);
                }
            }
            if (pieMenuSize == 12)
            {
                pieItem.transform.localScale = new Vector3(0.6f, 0.6f, 0.0f);
                if (0 < dockingItemAngle && dockingItemAngle <= 30)
                {
                    itemPicked = 7;

                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 180);
                }
                else if (30 < dockingItemAngle && dockingItemAngle <= 60)
                {
                    itemPicked = 8;

                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 210);
                }
                else if (60 < dockingItemAngle && dockingItemAngle <= 90)
                {
                    itemPicked = 9;

                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 240);
                }
                else if (90 < dockingItemAngle && dockingItemAngle <= 120)
                {
                    itemPicked = 10;

                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 270);
                }
                else if (120 < dockingItemAngle && dockingItemAngle <= 150)
                {
                    itemPicked = 11;
                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 300);
                }
                else if (150 < dockingItemAngle && dockingItemAngle <= 180)
                {
                    itemPicked = 12;

                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 330);
                }
                else if (180 < dockingItemAngle && dockingItemAngle <= 210)
                {
                    itemPicked = 1;

                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else if (210 < dockingItemAngle && dockingItemAngle <= 240)
                {
                    itemPicked = 2;

                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 30);
                }
                else if (240 < dockingItemAngle && dockingItemAngle <= 270)
                {
                    itemPicked = 3;

                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 60);
                }
                else if (270 < dockingItemAngle && dockingItemAngle <= 300)
                {
                    itemPicked = 4;

                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 90);
                }
                else if (300 < dockingItemAngle && dockingItemAngle <= 330)
                {
                    itemPicked = 5;
                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 120);
                }
                else if (330 < dockingItemAngle && dockingItemAngle <= 360)
                {
                    itemPicked = 6;
                    pieItem.transform.rotation = Quaternion.Euler(0, 0, 150);
                }

            }
        }
    }

 

    void ItemGenerator() // loads correct menu sprite.
    {
        if (touchController.pieMenuSize == 4)
        {
            pieImage.sprite = item4;
            imageTarget.sprite = item4;

        }
        if (touchController.pieMenuSize == 8)
        {
            pieImage.sprite = item8;
            imageTarget.sprite = item8;
        }
        if (touchController.pieMenuSize == 12)
        {
            pieImage.sprite = item12;
            imageTarget.sprite = item12;
        }
    }

    public void PositionReferenceMarkers() // move position of menu with the position of stylus
    {
        referenceCircle.SetActive(true);

        referenceCircle.transform.position = new Vector3 (touchController.touchPositionX, touchController.touchPositionY, 0.0f);

    }

    public void DeleteRefernceMarkers()
    {
        referenceCircle.SetActive(false);
    }


    float Map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    public void Writer(string data, StreamWriter writer)
    {
      
        writer.WriteLine(data);
        writer.Flush();
      //  writer.Close();
        
    }
    public string getPath()
    {//C:\Users\guerr\OneDrive\Documents\FlexStylusUnityProject\Assets\Data

        return Application.dataPath + "/Data/" + "Participant_" + touchController.participant + ".csv";
       //return Application.dataPath + "/Data/" + "Participant1.csv";
        //return Application.dataPath + "/Data/" + "Participant_" + touchController.participant + "_" + touchController.flexibilityLevel + ".csv";
    }

}

