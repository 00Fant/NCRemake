using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class itemBeg : NetworkBehaviour {

    [SyncVar]
    public string ItemData;

	void Start () {
    }

    public void SetBeg( string _data ) {
        CmdSetBeg( _data );
    }

    [Command]
    public void CmdSetBeg( string _data ) {
        RpcSetBeg( _data );
        ItemData = _data;
    }

    [ClientRpc]
    public void RpcSetBeg( string _data ) {
        ItemData = _data;
    }

}
