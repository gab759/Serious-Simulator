using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
 
using System.Linq;

public class UDPSend : MonoBehaviour
{
    private static int localPort;
    
    IPEndPoint remoteEndPoint;
    UDPDATA mUDPDATA = new UDPDATA ();
    // prefs
    private string IP;  // define in init
    private int port;  // define in init
    public Transform vehicle;

    public float alturaObjetivo = 1.5f; // Cambia según lo que consideres "alto"
    public float velocidadMotor = 1f;   // Velocidad del Lerp para subir el motor
    
    [SerializeField]
    float valueMotor = 0;
    [Range(0f, 1f)]
    public float motorAmplitude = 1f;
    [Range(1f, 90f)]
    public float maxRotation = 35f;
    UdpClient client;
    string sendpack ;
    bool active = false;
    // gui
    string strMessage = "";
    bool quit = false;

    [Range(0f, 200f)]
    public float A = 0, B = 0, C = 0;

    public float longg;
    public float  SmoothEngine;

    
    public void init()
    {

        // define
        IP = "192.168.15.201";
        port = 7408;

        // ----------------------------
        // Senden
        // ----------------------------
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient(53342);


        // AppControlField
        mUDPDATA.mAppControlField.ConfirmCode = "55aa";
        mUDPDATA.mAppControlField.PassCode = "0000";
        mUDPDATA.mAppControlField.FunctionCode = "1301";
        // AppWhoField
        mUDPDATA.mAppWhoField.AcceptCode = "ffffffff";
        mUDPDATA.mAppWhoField.ReplyCode = "";//"00000001";
                                             // AppDataField
        mUDPDATA.mAppDataField.RelaTime = "00000064";
        mUDPDATA.mAppDataField.PlayMotorA = "00000000";
        mUDPDATA.mAppDataField.PlayMotorB = "00000000";
        mUDPDATA.mAppDataField.PlayMotorC = "00000000";

        mUDPDATA.mAppDataField.PortOut = "12345678";

        //ResertPositionEngine();

        //Invoke("ActiveSend", 8);

        A = Mathf.Clamp(100, 0, 200);
        B = Mathf.Clamp(100, 0, 200);
        C = Mathf.Clamp(100, 0, 200);
    }
    // start from unity3d
    public void Start()
    {
        init();
        alturaObjetivo = vehicle.position.y;
    }
    void CalcularAltitud () 
    {

        float alturaActual = vehicle.position.y;

        // Si está por encima de la altura objetivo, aumentar suavemente valueMotor hasta 100
        if (alturaActual > alturaObjetivo)
        {
            valueMotor = Mathf.Lerp(valueMotor, 100f, Time.deltaTime * velocidadMotor);
        }
        else
        {
            valueMotor = 0f; // O podrías hacer que baje gradualmente si quieres
        }

        

    }
    void CalcularRotacion()
    {
        float total = 200f; // Valor total a distribuir entre los motores

        // Obtener las rotaciones absolutas normalizadas (entre -180 y 180)
        float rotX = Mathf.Abs(NormalizeAngle(vehicle.localEulerAngles.x));
        float rotY = Mathf.Abs(NormalizeAngle(vehicle.localEulerAngles.y));
        float rotZ = Mathf.Abs(NormalizeAngle(vehicle.localEulerAngles.z));

        // Suma total para normalizar
        float suma = rotX + rotY + rotZ;
        if (suma <= 0.01f) suma = 1f; // Previene división por cero

        // Asignación proporcional (clamp para asegurar que estén en el rango 0-200)
        A = Mathf.Clamp((rotX / suma) * total, 0, 200);
        B = Mathf.Clamp((rotY / suma) * total, 0, 200);
        C = Mathf.Clamp((rotZ / suma) * total, 0, 200);

        // Actualizar campos del paquete UDP con valores hexadecimales
        mUDPDATA.mAppDataField.PlayMotorA = DecToHexMove(A);
        mUDPDATA.mAppDataField.PlayMotorB = DecToHexMove(B);
        mUDPDATA.mAppDataField.PlayMotorC = DecToHexMove(C);

        // Debug logs para verificación
        Debug.Log($"Valores A: {A:F2}, B: {B:F2}, C: {C:F2} -> Hex: A={mUDPDATA.mAppDataField.PlayMotorA}, B={mUDPDATA.mAppDataField.PlayMotorB}, C={mUDPDATA.mAppDataField.PlayMotorC}");
        Debug.Log("Mensaje Enviado: " + mUDPDATA.GetToString());

        // Enviar por UDP
        sendString(mUDPDATA.GetToString());
    }
    float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }
    void ActiveSend () 
    {
        active = true;
    }
    void ResertPositionEngine ()
    {

        mUDPDATA.mAppDataField.RelaTime = "00001F40";

        mUDPDATA.mAppDataField.PlayMotorA = "00000000";
        mUDPDATA.mAppDataField.PlayMotorB = "00000000";
        mUDPDATA.mAppDataField.PlayMotorC = "00000000";

        sendString (mUDPDATA.GetToString ());

        mUDPDATA.mAppDataField.RelaTime = "00000064";

    }
    
    void FixedUpdate()
    {

        CalcularAltitud();

        if (valueMotor >= 99f)
        {
            CalcularRotacion();
        }

        mUDPDATA.mAppDataField.PlayMotorC = DecToHexMove(C);
        mUDPDATA.mAppDataField.PlayMotorA = DecToHexMove(A);
        mUDPDATA.mAppDataField.PlayMotorB = DecToHexMove(B);

        if (valueMotor > 0)
            sendString(mUDPDATA.GetToString());

    }
    void OnApplicationQuit()
    {
        active = false;
        ResertPositionEngine ();

        quit = true;
       
        if(client!=null)
            client.Close();
        Application.Quit();
    }
    // init
    
    byte[] StringToByteArray (string hex) {
        return Enumerable.Range (0, hex.Length)
                         .Where (x => x % 2 == 0)
                         .Select (x => Convert.ToByte (hex.Substring (x, 2), 16))
                         .ToArray ();
    }
    string DecToHexMove (float num) {
        int d = (int)((num / 5f) * 10000f);
        return "000"+d.ToString ("X");
    }
    // sendData
    private void sendString (string message) {
        try {
            // Bytes empfangen.
            if (message != "") {

                //byte[] data = StringToByteArray (message);
                // Den message zum Remote-Client senden.
                //client.Send (data, data.Length, remoteEndPoint);
                Debug.Log(message);
            }


        }
        catch (Exception err) {
            //print (err.ToString ());
        }
    }
    private double DegreeToRadian (double angle) {
        return Math.PI * angle / 180.0f;
    }
    private double RadianToDegree (double angle) {
        return angle * (180.0f / Math.PI);
    }
    void OnDisable()
    {
        client.Close();
    }
     
}

