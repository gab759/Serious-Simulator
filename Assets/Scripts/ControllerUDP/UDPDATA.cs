using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UDPDATA {
    public AppControlField mAppControlField;
    public AppWhoField mAppWhoField;
    public AppDataField mAppDataField;
    public UDPDATA () {
        mAppControlField = new AppControlField ();
        mAppWhoField = new AppWhoField ();
        mAppDataField = new AppDataField ();
    }
    public string GetToString () {
        return mAppControlField.GetToString () + mAppWhoField.GetToString () + mAppDataField.GetToString ();
    }
}
