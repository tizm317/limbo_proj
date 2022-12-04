using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CreateAccountPacketReq
{
    public string AccountName;
    public string Password;
}

public class CreateAccountPacketRes
{
    public bool CreateOk;
}

public class LoginAccountPacketReq
{
    public string AccountName;
    public string Password;
}


public class ServerInfo
{
    public string Name;
    public string IpAddress;
    public int Port;
    public int BusyScore;
    public int Open;
}

public class LoginAccountPacketRes
{
    public bool LoginOk;
    public int AccountId;
    public int Token;
    public List<ServerInfo> ServerList = new List<ServerInfo>();
}

public class MapInfo
{
    public string Name;
    public Define.Scene scene;
}


// Certification 우회
public class CertificateWhore : CertificateHandler
{
    protected override bool ValidateCertificate(byte[] certificateData)
    {
        return true;
    }
}