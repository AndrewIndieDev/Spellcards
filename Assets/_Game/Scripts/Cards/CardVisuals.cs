using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using DG.Tweening;
using Sirenix.OdinInspector.Editor.Drawers;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

public class CardVisuals : MonoBehaviour
{
    private GridManager Grid => GridManager.Instance;

    [Title("Container Reference")]
    [SerializeField] private CardContainer r_Container;

    [Title("Inspector References")]
    [SerializeField] private MeshRenderer m_Image;
    [SerializeField] private MeshRenderer m_Background;
    [SerializeField] private MeshRenderer m_Outline;
    [SerializeField] private TMP_Text t_Name;
    [SerializeField] private TMP_Text t_SellCost; // Unused, selling was removed.
    [SerializeField] private TMP_Text t_CardHealth;
    [SerializeField] private TMP_Text t_CardAttack;

    public Tween CurrentTween => currentTween;
    public Vector3 Position => transform.position;
    public Vector3 CardScale => new Vector3(r_Container.CardData.xSize, 1f, r_Container.CardData.ySize);

    private Tween currentTween;

    #region Unity Methods
    private void OnDestroy()
    {
        if (currentTween != null)
            currentTween.Kill();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Updates the visuals with new data.
    /// </summary>
    /// <param name="data">The new data that needs to be reflected in the visuals</param>
    public void Set(CardData data)
    {
        transform.localScale = new Vector3(data.xSize, 1f, data.ySize);
        MoveInstant(Grid.GetGridCellPosition(r_Container.GridPosition));
        SetImage(data.cardImage);
        SetBackground(data.cardBackground);
        SetName(data.name);
        SetHealth(data.cardStats.health);
        SetAttack(data.cardStats.attack);
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
    /// <param name="position">New position to tween to.</param>
    public void Move(Vector3 position)
    {
        position = Grid.GetGridCellPosition(position) + GetLocalSizePosition();
        if (currentTween != null)
            currentTween.Kill();
        currentTween = transform.DOMove(position, r_Container.DEFAULT_TWEEN_TIME);
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
    /// <param name="worldPosition">World position to move to.</param>
    public void MoveInstant(Vector3 worldPosition)
    {
        transform.position = Grid.GetGridCellPosition(worldPosition) + GetLocalSizePosition();
    }
    public void MoveThenDestroy(Vector3 position)
    {
        Move(position);
        currentTween.onComplete += () => Destroy(gameObject);
    }
    /// <summary>
    /// Tweens the rotation.
    /// </summary>
    /// <param name="rotation">New rotation to tween to.</param>
    public void Rotate(Vector3 rotation)
    {
        if (currentTween != null)
            currentTween.Kill();
        currentTween = transform.DOLocalRotate(rotation, r_Container.DEFAULT_TWEEN_TIME);
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
        Rotate(rot);
    }
    /// <summary>
    /// Instantly rotates the object to the given rotation
    /// </summary>
    /// <param name="rot"></param>
    public void RotateInstant(Vector3 rot)
    {
        transform.eulerAngles = rot;
    }
    /// <summary>
    /// Called when the Interact button has been cancelled.
    /// </summary>
    public void OnInteractUp()
    {
        Move(r_Container.Collision.Position);
    }
    public void UpdateHealth(int amount)
    {
        t_CardHealth.text = amount.ToString();
    }
    public void UpdateAttack(int amount)
    {
        t_CardAttack.text = amount.ToString();
    }
    public void OnHit()
    {
        if (currentTween != null)
            currentTween.Complete();
        SetScale(1.2f);
        currentTween = transform.DOScale(CardScale, r_Container.DEFAULT_TWEEN_TIME);
    }
    public void Select()
    {
        SetGlow(1f);
    }
    public void Deselect()
    {
        SetGlow(0f);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Sets the Image of the card.
    /// </summary>
    /// <param name="texture">Material to use.</param>
    private void SetImage(Texture2D texture)
    {
        m_Image.material.mainTexture = texture;
    }
    /// <summary>
    /// Sets the Background Image of the card.
    /// </summary>
    /// <param name="texture">Material to use</param>
    private void SetBackground(Texture2D texture)
    {
        m_Background.material.mainTexture = texture;
    }
    /// <summary>
    /// Sets the Name of the card.
    /// </summary>
    /// <param name="name">New name of the card</param>
    private void SetName(string name)
    {
        t_Name.text = name;
    }
    /// <summary>
    /// Sets the Sell Cost of the card.
    /// </summary>
    /// <param name="sellCost">The amount of currency you get for selling the card</param>
    private void SetSellCost(int sellCost)
    {
        t_SellCost.text = sellCost.ToString();
    }
    private void SetHealth(int amount)
    {
        if (amount > 0)
            t_CardHealth.text = amount.ToString();
        else
            t_CardHealth.transform.parent.gameObject.SetActive(false);
    }
    private void SetAttack(int amount)
    {
        if (amount > 0)
            t_CardAttack.text = amount.ToString();
        else
            t_CardAttack.transform.parent.gameObject.SetActive(false);
    }
    private void SetScale(float scale)
    {
        transform.localScale = CardScale * scale;
    }
    private Vector3 GetLocalSizePosition()
    {
        return new Vector3(r_Container.CardData.xSize / 2f * Grid.gridHorizontalSize, 0f, r_Container.CardData.ySize / 2f * Grid.gridVerticalSize);
    }
    private void SetGlow(float amount)
    {
        m_Outline.material.SetFloat("_Opacity", Mathf.Clamp01(amount));
    }
    #endregion
}
