using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class player : NetworkBehaviour {
    public skills Skills;
    public inventory Inventory;

    public string Name;
    public string Password;

    void Start() {
        if( isLocalPlayer ) {
            Name = PlayerPrefs.GetString( "Name" );
            Password = PlayerPrefs.GetString( "Password" );
            CmdSetPlayerKey( Name, Password );
            
        }
    }

    [Command]
    public void CmdSetPlayerKey(string _Name, string _Password ) {
        Name = _Name;
        Password = _Password;
        LoadPlayer();
    }

    public string GetPlayerKey() {
        return Name + Password;
    }

    public void LoadPlayer() {
        Skills.RemoteLoadSkills();
        Inventory.RemoteLoadInventory();
    }
    public void SavePlayer() {
        Skills.RemoteSaveSkills();
        Inventory.RemoteSaveInventory();
    }
}
