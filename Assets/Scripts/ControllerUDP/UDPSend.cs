using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Linq;

public class UDPSend : MonoBehaviour
{
    private static int localPort;

    IPEndPoint remoteEndPoint;
    UDPDATA mUDPDATA = new UDPDATA();

    private string IP;
    private int port;
    public Transform vehicle;

    public float alturaObjetivo = 1.5f;
    public float velocidadMotor = 1f;

    [SerializeField]
    float valueMotor = 0;
    [Range(0f, 1f)]
    public float motorAmplitude = 1f;
    [Range(1f, 90f)]
    public float maxRotation = 35f;

    UdpClient client;
    string sendpack;
    bool active = false;
    string strMessage = "";
    bool quit = false;

    [Range(0f, 200f)]
    public float A = 0, B = 0, C = 0;

    public float longg;
    public float SmoothEngine;

    public void init()
    {
        IP = "192.168.15.201";
        port = 7408;

        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient(53342);

        mUDPDATA.mAppControlField.ConfirmCode = "55aa";
        mUDPDATA.mAppControlField.PassCode = "0000";
        mUDPDATA.mAppControlField.FunctionCode = "1301";

        mUDPDATA.mAppWhoField.AcceptCode = "ffffffff";
        mUDPDATA.mAppWhoField.ReplyCode = "";

        mUDPDATA.mAppDataField.RelaTime = "00000064";
        mUDPDATA.mAppDataField.PlayMotorA = "00000000";
        mUDPDATA.mAppDataField.PlayMotorB = "00000000";
        mUDPDATA.mAppDataField.PlayMotorC = "00000000";
        mUDPDATA.mAppDataField.PortOut = "12345678";

        A = Mathf.Clamp(100, 0, 200);
        B = Mathf.Clamp(100, 0, 200);
        C = Mathf.Clamp(100, 0, 200);
    }

    public void Start()
    {
        init();
        alturaObjetivo = vehicle.position.y;
    }

    void CalcularAltitud()
    {
        float alturaActual = vehicle.position.y;

        if (alturaActual > alturaObjetivo)
        {
            valueMotor = Mathf.Lerp(valueMotor, 100f, Time.deltaTime * velocidadMotor);
        }
        else
        {
            valueMotor = 0f;
        }
    }

    void CalcularRotacion()
    {
        // Obtener rotaciones normalizadas
        float pitch = NormalizeAngle(vehicle.localEulerAngles.x);  // Adelante / Atrás
        float roll = NormalizeAngle(vehicle.localEulerAngles.z);   // Izquierda / Derecha

        float basePower = 100f;
        float sensitivityPitch = 1.0f;
        float sensitivityRoll = 1.0f;

        // Corrección por Pitch (adelante/atrás)
        float correctionPitch = Mathf.Clamp((pitch / maxRotation) * 100f * sensitivityPitch, -100f, 100f);

        // Corrección por Roll (izquierda/derecha)
        float correctionRoll = Mathf.Clamp((roll / maxRotation) * 100f * sensitivityRoll, -100f, 100f);

        // Motor A (trasero) => Solo reacciona al Pitch (adelante/atrás)
        A = Mathf.Clamp(basePower - correctionPitch, 0f, 200f);

        // Motor B (izquierdo) => Suma Pitch y Roll
        B = Mathf.Clamp(basePower + correctionPitch - correctionRoll, 0f, 200f);

        // Motor C (derecho) => Suma Pitch y Roll (opuesto)
        C = Mathf.Clamp(basePower + correctionPitch + correctionRoll, 0f, 200f);

        // Guardar valores en hexadecimal
        mUDPDATA.mAppDataField.PlayMotorA = DecToHexMove(A);
        mUDPDATA.mAppDataField.PlayMotorB = DecToHexMove(B);
        mUDPDATA.mAppDataField.PlayMotorC = DecToHexMove(C);

        // Paquete completo
        string hexMessage = mUDPDATA.GetToString();
        Debug.Log("HEX: " + hexMessage);
    }

    float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        return angle;
    }

    void ActiveSend()
    {
        active = true;
    }

    void ResertPositionEngine()
    {
        mUDPDATA.mAppDataField.RelaTime = "00001F40";

        mUDPDATA.mAppDataField.PlayMotorA = "00000000";
        mUDPDATA.mAppDataField.PlayMotorB = "00000000";
        mUDPDATA.mAppDataField.PlayMotorC = "00000000";

        sendString(mUDPDATA.GetToString());

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
        ResertPositionEngine();

        quit = true;

        if (client != null)
            client.Close();
        Application.Quit();
    }

    byte[] StringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();
    }

    string DecToHexMove(float num)
    {
        int d = (int)((num / 5f) * 10000f);
        return "000" + d.ToString("X");
    }

    private void sendString(string message)
    {
        try
        {
            if (message != "")
            {
                Debug.Log(message);
            }
        }
        catch (Exception err)
        {
        }
    }

    private double DegreeToRadian(double angle)
    {
        return Math.PI * angle / 180.0f;
    }

    private double RadianToDegree(double angle)
    {
        return angle * (180.0f / Math.PI);
    }

    void OnDisable()
    {
        client.Close();
    }
}