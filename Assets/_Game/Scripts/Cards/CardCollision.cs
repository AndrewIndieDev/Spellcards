using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;

public class CardCollision : MonoBehaviour
{
    [Title("Container Reference")]
    [SerializeField] private CardContainer r_Container;

    [Title("Inspector References")]
    [SerializeField] private BoxCollider r_Collider;
    [SerializeField] private Rigidbody r_Rigidbody;

    [Title("Read Only Variables")]
    [ReadOnly][SerializeField] private bool v_PickedUp;

    public Vector3 Position => transform.position;
    public bool PickedUp => v_PickedUp;

    #region Unity Methods
    
    #endregion

    #region Public Methods
    public void OnInteract()
    {
        if (!CanCardBePickedUp) return;

        v_PickedUp = true;
    }
    public void OnInteractUp()
    {
        if (!v_PickedUp) return;
        v_PickedUp = false;

        if (r_Container.Grid.GridPositionFree(r_Container.Grid.SelectionPositionGrid, EGridCellOccupiedFlags.Card))
            MoveInstant(r_Container.Grid.SelectionPositionWorld);
    }
    /// <summary>
    /// Enables the most appropriate collider at the time.
    /// </summary>
    public void CheckForUpdates()
    {
        DisableColliders();
        r_Collider.enabled = true;
    }
    /// <summary>
    /// Moves and Rotates the object to the desired position with zero rotation to the parent.
    /// </summary>
    /// <param name="trans">Moves to position and rotation.</param>
    public void Move(Transform trans)
    {
        Move(trans.position);
        Rotate(Vector3.zero);
    }
    /// <summary>
    /// Tweens the position.
    /// </summary>
    /// <param name="position">New positioin to tween to.</param>
    public void Move(Vector3 position)
    {
        UpdateOccupancy(r_Container.Grid.GetGridCoordsAtWorldPosition(position));
        transform.DOMove(position, r_Container.DEFAULT_TWEEN_TIME);
    }
    /// <summary>
    /// Moves the object to a certain position on a given axis.
    /// </summary>
    /// <param name="axis">The given axis to move on.</param>
    /// <param name="destination">The destination float for the move.</param>
    public void Move(EAxis axis, float destination)
    {
        Move(transform.localPosition.IgnoreAxis(axis, destination));
    }
    /// <summary>
    /// Instnatly move and rotate the object to the given Transform.
    /// </summary>
    /// <param name="trans">Given Transform to move to.</param>
    public void MoveInstant(Transform trans)
    {
        MoveInstant(trans.position);
        RotateInstant(trans.eulerAngles);
    }
    /// <summary>
    /// Instantly move the object to the given position.
    /// </summary>
    /// <param name="position"></param>
    public void MoveInstant(Vector3 position)
    {
        UpdateOccupancy(r_Container.Grid.GetGridCoordsAtWorldPosition(position));
        transform.position = position;
    }
    /// <summary>
    /// Tweens the rotation.
    /// </summary>
    /// <param name="rotation">New rotation to tween to.</param>
    public void Rotate(Vector3 rotation)
    {
        transform.DOLocalRotate(rotation, r_Container.DEFAULT_TWEEN_TIME);
    }
    /// <summary>
    /// Locally rotates the object a certain amount around a given axis.
    /// </summary>
    /// <param name="rotateBy">Amount to rotate by (negative for left, positive for right)</param>
    /// <param name="axis">Axis to rotate on.</param>
    public void Rotate(float rotateBy, EAxis axis)
    {
        Vector3 rot = transform.localEulerAngles + new Vector3(
            axis == EAxis.X ? rotateBy : 0,
            axis == EAxis.Y ? rotateBy : 0,
            axis == EAxis.Z ? rotateBy : 0
            );
        transform.DOLocalRotate(rot, r_Container.DEFAULT_TWEEN_TIME);
    }
    /// <summary>
    /// IOnstantly rotates the object to the given rotation
    /// </summary>
    /// <param name="rot"></param>
    public void RotateInstant(Vector3 rot)
    {
        transform.eulerAngles = rot;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Disables all colliders for this object.
    /// </summary>
    private void DisableColliders()
    {
        r_Collider.enabled = false;
    }
    /// <summary>
    /// Checks to see if the card can be picked up.
    /// </summary>
    private bool CanCardBePickedUp => r_Container.CardData.GetType() != typeof(EnemyCard);
    /// <summary>
    /// When moving, we should tell the grid where we are moving.
    /// </summary>
    private void UpdateOccupancy(Vector2Int newPosition)
    {
        r_Container.Grid.UnoccupyGridField(r_Container.GridPosition, EGridCellOccupiedFlags.Card);
        r_Container.Grid.OccupyGridField(newPosition, EGridCellOccupiedFlags.Card, r_Container);
    }
    #endregion
}
