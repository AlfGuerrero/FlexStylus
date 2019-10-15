using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.IO;

public class DataLoggingScript : MonoBehaviour

{
    GameObject touchControllerObject;
    TouchInputController touchController;
    string filePathPositionLogger;

    // Start is called before the first frame update
    void Start()
    {
        touchControllerObject = GameObject.FindGameObjectWithTag("TouchController");
        touchController = touchControllerObject.GetComponent<TouchInputController>();

        filePathPositionLogger = getPathForPositionLogger();
        File.AppendAllText(filePathPositionLogger, "date, time, id , flexibility, size, itemCount, round, count, PosX, PosY, PosFrmCntrX, PosFrmCntrY, DistFrmCntr, Angle, selection, target, success, timeTaken \n");
        if (!System.IO.File.Exists(Application.dataPath + "/Data/" + "Participant_PositionData" + touchController.participant + ".csv"))
        {
            File.WriteAllText(Application.dataPath + "/Data/" + "Participant_PositionData" + touchController.participant + ".csv", "");
        }
    }

    // Update is called once per frame
    void Update()
    {
        while(touchController.isTouching == true)
        {
            
        }
        /*
         If touching == true
            if timmer is a difference of 10ms. 
         */

    }

    public string getPathForPositionLogger()
    {
        return Application.dataPath + "/Data/" + "Participant_PositionData" + touchController.participant + ".csv";
    }

}
