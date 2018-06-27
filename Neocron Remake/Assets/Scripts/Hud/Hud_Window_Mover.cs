using UnityEngine;
using UnityEngine.EventSystems;

public class Hud_Window_Mover : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    public RectTransform Window;
    private bool move;
    private Vector2 oldMousePosition = Vector2.zero;

    private void Start() {
        LoadLastWindowPosition();
    }

    void SaveLastWindowPosition() {
        PlayerPrefs.SetFloat( Window.name, Window.position.x );
        PlayerPrefs.SetFloat( Window.name, Window.position.y );
    }

    void LoadLastWindowPosition() {
        float LoadWindowPositionX = PlayerPrefs.GetFloat( Window.name );
        float LoadWindowPositionY = PlayerPrefs.GetFloat( Window.name );
        if( LoadWindowPositionX <= 0 ) {
            LoadWindowPositionX = Screen.width / 2f;
        }
        if( LoadWindowPositionY <= 0 ) {
            LoadWindowPositionY = Screen.height / 2f;
        }
        if( LoadWindowPositionX > Screen.width ) {
            LoadWindowPositionX = Screen.width / 2f;
        }
        if( LoadWindowPositionY > Screen.height ) {
            LoadWindowPositionY = Screen.height / 2f;
        }
        Vector3 NewWindowPosition = new Vector3( LoadWindowPositionX, LoadWindowPositionY, 0 );

        Window.position = NewWindowPosition;
    }

    void OnGUI() {     
        Vector2 currentMousePosition = Event.current.mousePosition;
        float MouseDeltaX = ( oldMousePosition.x - currentMousePosition.x );
        float MouseDeltaY = ( oldMousePosition.y - currentMousePosition.y );
        oldMousePosition = currentMousePosition;
        if( !move ) {
            return;
        }      
        if( MouseDeltaX != 0.0f || MouseDeltaY != 0.0f ) {
            Vector3 NewWindowPosition = Window.position;
            NewWindowPosition += new Vector3( -MouseDeltaX, MouseDeltaY ) * Time.deltaTime * 50f;
            if( NewWindowPosition.x - ( Window.rect.width / 2 ) < 0f ) {
                NewWindowPosition.x = ( Window.rect.width / 2 );
            }
            if( NewWindowPosition.x + ( Window.rect.width / 2 ) > Screen.width ) {
                NewWindowPosition.x = Screen.width - ( Window.rect.width / 2);
            }
            if( NewWindowPosition.y - ( Window.rect.height / 2 )  < 0f ) {
                NewWindowPosition.y = ( Window.rect.height / 2 );
            }
            if( NewWindowPosition.y + ( Window.rect.height / 2 ) > Screen.height ) {
                NewWindowPosition.y = Screen.height - ( Window.rect.height / 2 );
            }
            Window.position = NewWindowPosition;
        }       
    }

    public void OnPointerDown( PointerEventData eventData ) {
        move = true;
    }
    
    public void OnPointerUp( PointerEventData eventData ) {
        move = false;
        SaveLastWindowPosition();
    }
}
