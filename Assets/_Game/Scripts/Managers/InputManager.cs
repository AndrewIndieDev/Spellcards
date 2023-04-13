using UnityEngine;
using Sirenix.OdinInspector;
using System;

/*
         * To rebind keys, you need to:
         * 1. Disable the input
         * 2. Call the method "PerformInteractiveRebinding()" on the input you want to rebind
         *      - "input.Player.Interact.PerformInteractionRebind()"
         * 3. Add any extra's:
         *      - ".WithControlsExcluding("Mouse")" to exclude the mouse from the rebind
         *      - ".OnComplete(callback => {})" to add a callback when the rebind is complete
         * 4. Add ".Start()" to start the rebind
         */

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [Title("Inspector References")]
    [SerializeField] private PlayerInputActions input;

    public PlayerInputActions InputActions { get { return input; } }

    #region Unity Methods
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        input = new PlayerInputActions();
        input.Player.Enable();
    }
    #endregion

    #region Public Methods

    #endregion
}
