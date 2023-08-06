using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class AbilityVisual : MonoBehaviour
{
    private GridManager Grid => GridManager.Instance;
    private PlayerInputActions InputActions => InputManager.Instance.InputActions;

    [Title("Inspector References")]
    [SerializeField] private MeshRenderer r_ArrowHead;
    [SerializeField] private LineRenderer r_LineRenderer;

    private new bool enabled = false;
    private CardContainer current;

    #region Callbacks
    private void OnSelectionGridPositionChanged(Vector2Int gridPosition)
    {
         if (!enabled)
            return;
        SetPosition(Grid.GetGridCellCenterPosition(gridPosition));
    }
    private void OnInteract(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!enabled)
            return;
        current.OnExecute();
        DisableVisual();
    }
    private void OnExecute(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!enabled)
            return;
        DisableVisual();
    }
    #endregion

    #region Unity Methods
    private void Start()
    {
        GridManager.onSelectionGridPositionChanged += OnSelectionGridPositionChanged;
        InputActions.Player.Interact.started += OnInteract;
        InputActions.Player.Execute.performed += OnExecute;
    }
    #endregion

    #region Public Methods
    public void Init(AbilityStyle style, CardContainer card)
    {
        current = card;

        r_ArrowHead.material = style.arrowHead;
        r_ArrowHead.material.color = style.arrowColour;
        r_LineRenderer.material = style.lineMaterial;
        r_LineRenderer.startColor = style.lineColour;
        r_LineRenderer.endColor = style.lineColour;

        EnableVisual();
    }
    #endregion

    #region Private Methods
    private void SetPosition(Vector3 position)
    {
        r_ArrowHead.transform.position = position;
        r_ArrowHead.transform.LookAt(position + (position - current.Collision.Position).normalized);
        r_LineRenderer.SetPositions(new Vector3[] { current.Collision.Position, position });
    }
    private void DisableVisual()
    {
        enabled = false;
        r_ArrowHead.enabled = false;
        r_LineRenderer.enabled = false;
        current = null;
    }
    private void EnableVisual()
    {
        StartCoroutine(EnabledTimer());

        SetPosition(current.Collision.Position);

        r_ArrowHead.enabled = true;
        r_LineRenderer.enabled = true;
    }
    private IEnumerator EnabledTimer()
    {
        yield return new WaitForSeconds(0.1f);
        enabled = true;
    }
    #endregion
}