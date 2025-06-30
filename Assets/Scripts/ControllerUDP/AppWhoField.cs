using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppWhoField {
    public string AcceptCode { get; set; }
    public string ReplyCode { get; set; }
    public AppWhoField () {
        AcceptCode = "ffffffff";
        ReplyCode = "00000000";
    }
    public string GetToString () {

        return AcceptCode + ReplyCode;
    }
}
