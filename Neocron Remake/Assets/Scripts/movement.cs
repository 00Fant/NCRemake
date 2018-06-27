using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class movement : NetworkBehaviour {
    // Params
    public float MoveSpeed;
    public float RunMultiplicator;
    public float CrouchMultiplicator;
    public float JumpPower;
    public float AirForce;
    public float TurnSpeed;
    public float TurnLerpSpeed;
    public float Drag;
    public float AirDrag;

    // Move ConsumCosts
    public int RunCostPerSec;

    //Outputs
    public bool onGround;
    public bool isCrouching;
    public bool isRunning;
    public float BodyYaw;
    public float HeadPitch;
    
    //Components
    public Rigidbody RB;  
    public CapsuleCollider HitBox;
    public animation_control AnimationControl;
    public Transform HeadTransform;
    public Transform ChestTransform;
    public stats Stats;
    public skills Skills;

    private float ColliderHeight;
    private Vector3 ColliderCenter;
    private Vector3 Force;
    private Vector3 MoveDir;
    private float RunCostTimer;

    

    private void Start() {
        ColliderHeight = HitBox.height;
        ColliderCenter = HitBox.center;
        if( isLocalPlayer ) {
            AnimationControl.isLocalPlayer = true;
        }       
    }

    void Update () {
        onGround = isGrounded();
        Movement();
        if( isServer ) {
            if( RunCostTimer <= 0f && isRunning ) {
                RunCostTimer = 1f;
                Stats.consumStamina( RunCostPerSec );
            }
            if( RunCostTimer > 0f ) {
                RunCostTimer -= Time.deltaTime;
            }
        }
    }

    void FixedUpdate() {
        Rotation();
    }

    void Movement() {
        if( !isLocalPlayer ) {
            return;
        }
        
        MoveDir = Vector3.zero;

        if( Input.GetButton( "Forward" ) ) {
            MoveDir.z = 1;
        }
        if( Input.GetButton( "Backward" ) ) {
            MoveDir.z = -1;
        }
        if( Input.GetButton( "Strafe Left" ) ) {
            MoveDir.x = -1;
        }
        if( Input.GetButton( "Strafe Right" ) ) {
            MoveDir.x = 1;
        }
        if( onGround ) {
            MoveDir = MoveDir.normalized * ( MoveSpeed + ( Skills.GetSubSkillValue( "Agility" ) / 100f ) );

            if( Input.GetButton( "Run" ) && Stats.current_Stamina >= RunCostPerSec && MoveDir != Vector3.zero ) {
                MoveDir *= RunMultiplicator;
                if( !isRunning ) {
                    isRunning = true;
                    CmdSetRunning( isRunning );
                }
                
            }
            if( ( Input.GetButton( "Run" ) == false || MoveDir == Vector3.zero ) && isRunning ) {
                isRunning = false;
                CmdSetRunning( isRunning );
            }
            if( Input.GetButton( "Crouch" ) ) {
                //MoveDir *= CrouchMultiplicator;
                MoveDir = Vector3.zero;
            }
        } else {
            MoveDir = MoveDir.normalized * MoveSpeed;
        }
        if( Input.GetButtonDown( "Crouch" ) ) {
            HitBox.height = ColliderHeight / 2f;
            HitBox.center = ColliderCenter / 2f;
            isCrouching = true;
        }
        if( Input.GetButtonUp( "Crouch" ) ) {
            HitBox.height = ColliderHeight;
            HitBox.center = ColliderCenter;
            isCrouching = false;
        }
        if( Input.GetButtonDown( "Jump" ) && onGround && Stats.current_Stamina > 1 ) {
            MoveDir.y = JumpPower + ( Skills.GetSubSkillValue( "Athletics" ) / 100f );
            //AnimationControl.doJump();
            //onGround = false;
            CmdJump();
        }

        if( MoveDir == Vector3.zero && onGround ) {
            RB.velocity = new Vector3( RB.velocity.x * Drag, RB.velocity.y - 0.1f, RB.velocity.z * Drag );
            return;
        }
       
        Force = Vector3.zero;
        Force += transform.forward * MoveDir.z;
        Force += transform.right * MoveDir.x;
        Force += Vector3.up * ( RB.velocity.y + MoveDir.y );

        if( Force != Vector3.zero && onGround ) {          
            RB.velocity = new Vector3( Force.x, Force.y, Force.z );
        }
        if( Force != Vector3.zero && !onGround && Vector3.Distance(Vector3.zero, RB.velocity ) < Vector3.Distance( Vector3.zero, new Vector3( Force.x, 0, Force.z ) ) ) {
            RB.AddForce( new Vector3( Force.x, 0, Force.z ) * AirForce, ForceMode.Acceleration );
        }
        if( !onGround ) {
            RB.velocity = new Vector3( RB.velocity.x * AirDrag, RB.velocity.y, RB.velocity.z * AirDrag );
        }
    }

    [Command]
    void CmdSetRunning( bool state ) {
        isRunning = state;
    }
    
    [Command]
    void CmdJump() {
        RpcJump();
    }
    [ClientRpc]
    void RpcJump() {
        AnimationControl.doJump();
    }

    void Rotation() {
        if( !isLocalPlayer ) {
            return;
        }
        if( Cursor.visible ) {
            return;
        }
        BodyYaw += Input.GetAxis( "Mouse X" ) * Time.fixedDeltaTime * TurnSpeed;
        if( BodyYaw < 0f ) {
            BodyYaw = 360f + BodyYaw;
        }
        if( BodyYaw > 360f ) {
            BodyYaw = 0f + ( BodyYaw - 360f);
        }
        HeadPitch -= Input.GetAxis( "Mouse Y" ) * Time.fixedDeltaTime * TurnSpeed;
        HeadPitch = Mathf.Clamp( HeadPitch, -89f, 89f );

        transform.rotation = Quaternion.Lerp( transform.rotation, Quaternion.Euler( 0, BodyYaw, 0 ), TurnLerpSpeed * Time.fixedDeltaTime );
        HeadTransform.localRotation = Quaternion.Lerp( HeadTransform.localRotation, Quaternion.Euler( HeadPitch / 2, 0, 0 ), TurnLerpSpeed * Time.fixedDeltaTime );
        ChestTransform.localRotation = Quaternion.Lerp( ChestTransform.localRotation, Quaternion.Euler( HeadPitch / 2, 0, 0 ), TurnLerpSpeed * Time.fixedDeltaTime );
    }

    bool isGrounded() {
        RaycastHit GroundHit;
        Ray GroundRay = new Ray( transform.position + transform.up, -transform.up );
        float RayRadius = 0.4f;
        float MaxRayDistance = 1.01f - ( RayRadius / 2f);
        if( Physics.SphereCast( GroundRay , RayRadius, out GroundHit, MaxRayDistance ) ) {
            if( GroundHit.collider && GroundHit.collider != HitBox ) {
                return true;
            }           
        }
        return false;
    }
}

