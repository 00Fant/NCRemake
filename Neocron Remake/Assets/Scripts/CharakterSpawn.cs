using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CharakterSpawn : NetworkBehaviour {
    public int ClassIndex;
    public GameObject Spy_Prefab;
    public GameObject PrivateEye_Prefab;
    public GameObject Tank_Prefab;
    public GameObject PsiMonk_Prefab;

    void Start () {
        if( isLocalPlayer ) {
            CmdSpawnCharakter( netId, Camera.main.GetComponentInChildren<Hud_CharakterSpawn>( true ).ClassIndex );
        }
        

    }

	void Update () {
		
	}

    [Command]
    public void CmdSpawnCharakter( NetworkInstanceId playerNetId, int _ClassIndex ) {
        GameObject oldPlayer = NetworkServer.FindLocalObject( playerNetId );
        var conn = oldPlayer.GetComponent<NetworkIdentity>().connectionToClient;

        GameObject NewPlayer = null;
        if( _ClassIndex == 0 ) {
            NewPlayer = (GameObject)Instantiate( Spy_Prefab, transform.position, Quaternion.identity );
        }
        if( _ClassIndex == 1 ) {
            NewPlayer = (GameObject)Instantiate( PrivateEye_Prefab, transform.position, Quaternion.identity );
        }
        if( _ClassIndex == 2 ) {
            NewPlayer = (GameObject)Instantiate( Tank_Prefab, transform.position, Quaternion.identity );
        }
        if( _ClassIndex == 3 ) {
            NewPlayer = (GameObject)Instantiate( PsiMonk_Prefab, transform.position, Quaternion.identity );         
        }
        if( NewPlayer ) {
            Destroy( oldPlayer.gameObject );
            NetworkServer.ReplacePlayerForConnection( conn, NewPlayer, 0 );
        }
        
    }

    //[Command]
    //public void CmdSpawnCharakter( int _ClassIndex ) {
    //    GameObject playerPrefab = null;
    //    if( _ClassIndex == 0 ) {
    //        playerPrefab = Resources.Load( "Prefab/Spy", typeof( GameObject ) ) as GameObject;
    //    }
    //    if( _ClassIndex == 1 ) {
    //        playerPrefab = Resources.Load( "Prefab/PrivateEye", typeof( GameObject ) ) as GameObject;
    //    }
    //    if( _ClassIndex == 2 ) {
    //        playerPrefab = Resources.Load( "Prefab/Tank", typeof( GameObject ) ) as GameObject;
    //    }
    //    if( _ClassIndex == 3 ) {
    //        playerPrefab = Resources.Load( "Prefab/PsiMonk", typeof( GameObject ) ) as GameObject;
    //    }
    //    if( !playerPrefab ) {
    //        return;
    //    }
    //    GameObject NewPlayer = (GameObject)Instantiate( playerPrefab, transform.position, Quaternion.identity );
    //    NetworkServer.SpawnWithClientAuthority( NewPlayer, connectionToClient );
    //    NetworkServer.ReplacePlayerForConnection
    //}
}
