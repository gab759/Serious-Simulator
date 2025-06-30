using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppControlField {
    public string ConfirmCode { get; set; }
    public string PassCode { get; set; }
    public string FunctionCode { get; set; }
    public string ObjectChannel { get; set; }
    public AppControlField () {
        ConfirmCode = "55aa";
        PassCode = "0000";
        FunctionCode = "1401";
        ObjectChannel = "0000";
    }
    public string GetToString () {

        return ConfirmCode + PassCode + FunctionCode + ObjectChannel;
    }
}
