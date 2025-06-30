using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AppDataField {
    public string RelaTime { get; set; }
    public string RegStartAddress { get; set; }
    public string RegNum { get; set; }
    public string ExtData { get; set; }
    public string PlayMotor { get; set; }
    public string PlayMotorA { get; set; }
    public string PlayMotorB { get; set; }
    public string PlayMotorC { get; set; }
    public string PortOut { get; set; }
    public string PlayDAC { get; set; }
    public AppDataField () {
        RelaTime = "00000064";
        RegStartAddress = "00000000";
        RegNum = "";
        ExtData = "";
        PlayMotor = "";
        PortOut = "12345678";
        PlayDAC = "abcd";
    }
    public string GetToString () {
        return RelaTime + RegStartAddress + RegNum + PlayMotorA + PlayMotorB + PlayMotorC + PortOut + PlayDAC;
    }
}

