using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class stats : NetworkBehaviour {

    [Header("Stats")]
    public int current_Health;
    public int maximal_Health;
    public int regenerate_Health;
    public int base_Health;
    public int HealthSkillMultiplicator;
    [Space()]
    public int current_Stamina;
    public int maximal_Stamina;
    public int regenerate_Stamina;
    public int base_Stamina;
    public int StaminaSkillMultiplicator;
    [Space()]
    public int current_Psi;
    public int maximal_Psi;
    public int regenerate_Psi;
    public int base_Psi;
    public int PsiSkillMultiplicator;
    [Space()]
    public float RegenerateDelay;
    private float RegenerateTimer;
   
    [Header("Components")]
    public skills Skills;
    public Hud hud;
    public movement Movement;

    private void Awake() {        
        if( isServer ) {
            loadStats();
        }
    }

    private void Update() {
        if( isServer ) {
            RegenerateStats();
        }       
    }

    public void loadStats() {
        CalculateStats();
        if( current_Health <= 0 ) {
            current_Health = maximal_Health;
        }
        if( current_Stamina <= 0 ) {
            current_Stamina = maximal_Stamina;
        }
        if( current_Psi <= 0 ) {
            current_Psi = maximal_Psi;
        }
        UpdateStats();
    }

    public void CalculateStats() {
        maximal_Health = base_Health + ( HealthSkillMultiplicator * Skills.GetSubSkillValue( "Body Health" ) );
        maximal_Stamina = base_Stamina + ( StaminaSkillMultiplicator * Skills.GetSubSkillValue( "Endurance" ) );
        maximal_Psi = base_Psi + ( PsiSkillMultiplicator * Skills.GetSubSkillValue( "Psi Power" ) );
        maximal_Psi += ( Skills.GetSubSkillValue( "Psi Use" ) );

        regenerate_Stamina = 5 + ( Skills.GetSubSkillValue( "Endurance" ) / 10 );
    }

    void RegenerateStats() {
        
        if( RegenerateTimer > 0 ) {
            RegenerateTimer -= Time.deltaTime;
            return;
        }
        RegenerateTimer = RegenerateDelay;
        CalculateStats();
        if( regenerate_Health > 0 ) {
            if( current_Health < maximal_Health ) {
                current_Health += regenerate_Health;
                if( current_Health > maximal_Health ) {
                    current_Health = maximal_Health;
                }
                RpcSetValue( "current_Health", current_Health );
                RpcSetValue( "maximal_Health", maximal_Health );
            }
        }

        if( regenerate_Stamina > 0 ) {
            if( current_Stamina < maximal_Stamina && !Movement.isRunning ) {
                current_Stamina += regenerate_Stamina;
                if( current_Stamina > maximal_Stamina ) {
                    current_Stamina = maximal_Stamina;
                }
                RpcSetValue( "current_Stamina", current_Stamina );
                RpcSetValue( "maximal_Stamina", maximal_Stamina );
            }
        }

        if( regenerate_Psi > 0 ) {
            if( current_Psi < maximal_Psi ) {
                current_Psi += regenerate_Psi;
                if( current_Psi > maximal_Psi ) {
                    current_Psi = maximal_Psi;
                }
            }
            RpcSetValue( "current_Psi", current_Psi );
            RpcSetValue( "maximal_Psi", maximal_Psi );
        }
    }

    public void consumStamina( int consum ) {
        if( current_Stamina <= 0 ) {
            return;
        }
        current_Stamina -= consum;
        if( current_Stamina < 0 ) {
            current_Stamina = 0;
        }
        RpcSetValue( "current_Stamina", current_Stamina );
    }

    public void Die() {

    }

    public void getStats() {
        CmdUpdateStats();
    }

    [Command]
    void CmdUpdateStats() {
        UpdateStats();
    }

    public void UpdateStats() {
        RpcSetValue( "current_Health", current_Health );
        RpcSetValue( "maximal_Health", maximal_Health );
        //RpcSetValue( "regenerate_Health", regenerate_Health );
        //RpcSetValue( "base_Health", base_Health );
        //RpcSetValue( "HealthSkillMultiplicator", HealthSkillMultiplicator );

        RpcSetValue( "current_Stamina", current_Stamina );
        RpcSetValue( "maximal_Stamina", maximal_Stamina );
        //RpcSetValue( "regenerate_Stamina", regenerate_Stamina );
        //RpcSetValue( "base_Stamina", base_Stamina );
        //RpcSetValue( "StaminaSkillMultiplicator", StaminaSkillMultiplicator );

        RpcSetValue( "current_Psi", current_Psi );
        RpcSetValue( "maximal_Psi", maximal_Psi );
        //RpcSetValue( "regenerate_Psi", regenerate_Psi );
        //RpcSetValue( "base_Psi", base_Psi );
        //RpcSetValue( "PsiSkillMultiplicator", PsiSkillMultiplicator );
    }

    [Command]
    public void CmdSetValue( string Type, int Value ) {
        RpcSetValue( Type, Value );
    }
    [ClientRpc]
    public void RpcSetValue( string Type, int Value ) {
        if( Type == "current_Health" ) {
            current_Health = Value;
            return;
        }
        if( Type == "maximal_Health" ) {
            maximal_Health = Value;
            return;
        }
        if( Type == "regenerate_Health" ) {
            regenerate_Health = Value;
            return;
        }
        if( Type == "base_Health" ) {
            base_Health = Value;
            return;
        }
        if( Type == "HealthSkillMultiplicator" ) {
            HealthSkillMultiplicator = Value;
            return;
        }

        if( Type == "current_Stamina" ) {
            current_Stamina = Value;
            return;
        }
        if( Type == "maximal_Stamina" ) {
            maximal_Stamina = Value;
            return;
        }
        if( Type == "regenerate_Stamina" ) {
            regenerate_Stamina = Value;
            return;
        }
        if( Type == "base_Stamina" ) {
            base_Stamina = Value;
            return;
        }
        if( Type == "StaminaSkillMultiplicator" ) {
            StaminaSkillMultiplicator = Value;
            return;
        }

        if( Type == "current_Psi" ) {
            current_Psi = Value;
            return;
        }
        if( Type == "maximal_Psi" ) {
            maximal_Psi = Value;
            return;
        }
        if( Type == "regenerate_Psi" ) {
            regenerate_Psi = Value;
            return;
        }
        if( Type == "base_Psi" ) {
            base_Psi = Value;
            return;
        }
        if( Type == "PsiSkillMultiplicator" ) {
            PsiSkillMultiplicator = Value;
            return;
        }
    }
}
