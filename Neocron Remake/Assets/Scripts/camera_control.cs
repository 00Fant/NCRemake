using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class camera_control : NetworkBehaviour {

    public Transform HeadTarget;
    public Transform ThirdpersonTarget;
    public Rigidbody _Rigidbody;
    public movement _movement;

    public float MoveSpeed;
    public float RotationSpeed;
    public bool Firstperson;
    public float ThirdpersonDistance;

    private void Start() {
        if( !isLocalPlayer ) {
            enabled = false;
            return;
        }
    }

    private void Update() {

        if( Input.GetButtonDown( "ToggleThirdperson" ) ) {
            if( Firstperson ) {
                Firstperson = false;
            } else {
                Firstperson = true;
            }          
        }
    }

    void FixedUpdate () {
        MoveCamera();
    }

    void MoveCamera() {
        if( Firstperson ) {
            Camera.main.transform.position = Vector3.Lerp( Camera.main.transform.position, HeadTarget.position + ( _Rigidbody.velocity * Time.fixedDeltaTime ), MoveSpeed * Time.fixedDeltaTime );
            Camera.main.transform.rotation = Quaternion.Lerp( Camera.main.transform.rotation, Quaternion.Euler( _movement.HeadPitch, _Rigidbody.transform.eulerAngles.y, 0f ), RotationSpeed * Time.fixedDeltaTime );
        } else {
            ThirdpersonTarget.rotation = Quaternion.Lerp( ThirdpersonTarget.rotation, Quaternion.Euler( _movement.HeadPitch, _Rigidbody.transform.eulerAngles.y, 0f ), RotationSpeed * Time.fixedDeltaTime );
            Camera.main.transform.rotation = Quaternion.Lerp( Camera.main.transform.rotation, Quaternion.Euler( _movement.HeadPitch, _Rigidbody.transform.eulerAngles.y, 0f ), RotationSpeed * Time.fixedDeltaTime );
            Camera.main.transform.position = Vector3.Lerp( Camera.main.transform.position, ThirdpersonTarget.position + ( ThirdpersonTarget.forward * -ThirdpersonDistance ) + ( _Rigidbody.velocity * Time.fixedDeltaTime ), MoveSpeed * Time.fixedDeltaTime );
        }
    }
}
