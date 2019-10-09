using UnityEngine;
using System.IO.Ports;
using System.Collections;
using System.Threading;
public class ArduinoController : MonoBehaviour
{
    SerialPort sp = new SerialPort("COM3", 9600);
    // Start is called before the first frame update
    GameObject touchControllerObject;
    TouchInputController touchController;

    int xPos;
    int yPos;
    public string[] str;
    public string data;
    Thread sampleThread;
    public Vector3 filteredData;
    void Start()
    {
        touchControllerObject = GameObject.FindGameObjectWithTag("TouchController");
        touchController = touchControllerObject.GetComponent<TouchInputController>();
        
        sp.Open();
        sp.ReadTimeout = 1;
        sp.DtrEnable = true;
        //sampleThread = new Thread(new ThreadStart(SampleFunction));
       // sampleThread.IsBackground = true;
        //sampleThread.Start();

    }
    // Update is called once per frame
    void Update()
    {

       // str = data.Split(',');
        
        if (sp.IsOpen)
        {
            try
            {
                string s = sp.ReadLine();
                str = s.Split(',');
                
            }
            catch (System.Exception)
            {
                throw;
            }
        }

       // filteredData = new Vector3(float.Parse(str[0]),float.Parse(str[1]),0); 
   
    }
    /*
    public void SampleFunction()
    {
        while (sampleThread.IsAlive)
        {
             data = sp.ReadLine();
        }
    }*/


}
