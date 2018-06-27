using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animation_control : MonoBehaviour {

    public Animator _Animator;
    public Rigidbody _Rigidbody;
    public movement _movement;
    public AudioSource _AudioSource;
    public Transform _Weaponholder;

    public AudioClip[] FootStepGravelSound;
    public AudioClip[] JumpSounds;
    public AudioClip[] LandSounds;
    public float FootStepDelay;
    public bool isLocalPlayer;
    public int WeaponTypeIndex;
    private Vector3 relativeVelocity;
    private float FootStepCooldown;

    void Start () {
		
	}

	void Update () {
        if( isLocalPlayer ) {
            AnimatorLoop();
        }
        SoundControl();
    }

    void AnimatorLoop() {
        relativeVelocity = RelativeVelocity();
        _Animator.SetFloat( "Velocity_forward", relativeVelocity.z );
        _Animator.SetFloat( "Velocity_side", relativeVelocity.x );
        _Animator.SetBool( "onGround", _movement.onGround );
        _Animator.SetBool( "isCrouching", _movement.isCrouching );
        _Animator.SetInteger( "WeaponTypeIndex", WeaponTypeIndex );
    }

    void SoundControl() {
        if( FootStepCooldown > 0f ) {
            FootStepCooldown -= Time.deltaTime;
        }
    }

    public void doJump() {
        _Animator.SetTrigger("Jump");
    }

    public void FootStepSound() {
        if( !_AudioSource ) {
            return;
        }
        if( FootStepCooldown > 0f ) {
            return;
        }
        if( !_movement.onGround ) {
            return;
        }
        FootStepCooldown = FootStepDelay;
        PlaySound( GetRandomAudioClip( FootStepGravelSound ) );
    }

    public void JumpSound() {
        if( !_AudioSource ) {
            return;
        }
        //if( !_movement.onGround ) {
        //    return;
        //}
        PlaySound( GetRandomAudioClip( JumpSounds ) );
    }

    void PlaySound( AudioClip clip ) {
        if( !_AudioSource ) {
            return;
        }
        if( !clip ) {
            return;
        }
        _AudioSource.PlayOneShot( clip );
    }

    AudioClip GetRandomAudioClip( AudioClip[] Array ) {
        return Array[Random.Range( 0, Array.Length )];
    }

    Vector3 RelativeVelocity() {
        if( !_Rigidbody ) {
            return Vector3.zero;
        }
        return transform.InverseTransformDirection( _Rigidbody.velocity );
    }
}
