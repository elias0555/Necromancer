using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager Instance { get; private set; }

    public InputSystem_Actions playerControls; // L'instance de notre classe générée

    private void Awake()
    {
        Instance = this;

        playerControls = new InputSystem_Actions();
    }

    // On active la map d'actions quand l'objet est activé
    private void OnEnable()
    {
        if (playerControls != null)
        {
            playerControls.Enable();
        }
    }

    // Et on la désactive pour éviter des erreurs
    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.Disable();
        }
    }

    public void EnablePlayerControls()
    {
        if (playerControls == null) return;
        playerControls.UI.Disable();
        playerControls.Player.Enable();
    }

    public void EnableUIControls()
    {
        if (playerControls == null) return;
        playerControls.Player.Disable();
        playerControls.UI.Enable();
    }

    public void DisableAllControls()
    {
        if (playerControls == null) return;
        playerControls.Disable();
    }
}
