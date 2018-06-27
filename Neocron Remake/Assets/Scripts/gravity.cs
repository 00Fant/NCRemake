using UnityEngine;

public class gravity : MonoBehaviour {
    
    public GameObject PlanetGo;
    public bool OnGround;
    public bool InWater;
    public bool InAir;
    public bool zeroGravity;
    public bool KeepUpRight; 
    public float oldDrag;
    public float DistanceToGround;
    public float PlanetGravitation;

    private Rigidbody RB;
    private float searchtimer;
    private float LastOnGroundTime;
    private float LastInWaterTime;
    private float lavaDmgTimer;

    void Start () {
        RB = GetComponent<Rigidbody>();
        oldDrag = RB.drag;
    }

    void FixedUpdate() {
        if( !PlanetGo ) {
            PlanetGo = GameObject.FindGameObjectWithTag("Planet");
            return;
        }
        Gravitation();
        Rotate();
    }

  
    void Gravitation() {
        if( !RB ) {
            RB = GetComponent<Rigidbody>();
            return;
        }
        if( !PlanetGo ) {
            return;
        }
    
        if( zeroGravity ) {
            return;
        }
        
        PlanetGravitation = 9.81f;
      
        Vector3 GravityForce = ( transform.position - PlanetGo.transform.position ).normalized * PlanetGravitation;

        if( !InWater ) {
            RB.AddForce( -GravityForce, ForceMode.Acceleration  );
        }
        if( InWater ) {
            RB.AddForce( GravityForce, ForceMode.Acceleration );
        }      
        if( InWater && LastInWaterTime + 0.15f < Time.realtimeSinceStartup ) {
            InWater = false;
            RB.drag = oldDrag;
        }
        if( OnGround && LastOnGroundTime + 0.15f < Time.realtimeSinceStartup ) {
            OnGround = false;
        }
        if( !OnGround && !InWater ) {
            InAir = true;
        } else {
            InAir = false;
        }
    }
 
    void Rotate() {
        if( !KeepUpRight ) {
            return;
        }
        if( !PlanetGo ) {
            return;
        }
        transform.rotation = Quaternion.FromToRotation( transform.up, ( transform.position - PlanetGo.transform.position ).normalized ) * transform.rotation;      
    }

    void OnCollisionStay( Collision collisionInfo ) {
        if( !collisionInfo.collider ) {
            return;
        }
        if( collisionInfo.contacts.Length <= 0 ) {
            return;
        }
        LastOnGroundTime = Time.realtimeSinceStartup;
        if( !OnGround ) {
            OnGround = true;          
        }
    }

    void OnTriggerStay( Collider _Collider ) {
        if( !RB ) {
            return;
        }
        if( _Collider.gameObject.layer == LayerMask.NameToLayer( "Water" ) ) {
            LastInWaterTime = Time.realtimeSinceStartup;
            if( !InWater ) {
                InWater = true;
                oldDrag = RB.drag;
                RB.drag = 10f;
            }
        }
    }

}
