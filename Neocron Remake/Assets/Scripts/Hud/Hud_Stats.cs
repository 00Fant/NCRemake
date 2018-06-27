using UnityEngine;
using UnityEngine.UI;

public class Hud_Stats : MonoBehaviour {
    public Hud hud;
    public GameObject[] HealthBars;
    public Text HealthText;
    public GameObject[] StaminaBars;
    public Text StaminaText;
    public GameObject[] PsiBars;
    public Text PsiText;

    private stats Stats;
    private int oldHealth;
    private int oldStamina;
    private int oldPsi;

    private void Update() {
        RefreshHudStats();
    }

    void RefreshHudStats() {
        if( !hud ) {
            return;
        }
        if( !hud.Stats ) {
            return;
        }

        if( hud.Stats.current_Health != oldHealth ) {
            ShowBars( HealthBars, hud.Stats.current_Health, hud.Stats.maximal_Health );
            HealthText.text = hud.Stats.current_Health.ToString();
        }
        oldHealth = hud.Stats.current_Health;

        if( hud.Stats.current_Stamina != oldStamina ) {
            ShowBars( StaminaBars, hud.Stats.current_Stamina, hud.Stats.maximal_Stamina );
            StaminaText.text = hud.Stats.current_Stamina.ToString();
        }
        oldStamina = hud.Stats.current_Stamina;

        if( hud.Stats.current_Psi != oldPsi ) {
            ShowBars( PsiBars, hud.Stats.current_Psi, hud.Stats.maximal_Psi );
            PsiText.text = hud.Stats.current_Psi.ToString();
        }
        oldPsi = hud.Stats.current_Psi;
    }

    void ShowBars( GameObject[] Bars, int min, int max ) {
        if( min == 0 && max == 0 ) {
            return;
        }
        float value = (float)min / (float)max;
        value *= Bars.Length;
        for( int i = 0; i < Bars.Length; i++ ) {
            if( i < value ) {
                Bars[i].SetActive( true );
            } else {
                Bars[i].SetActive( false );
            }
        }
    }
}
