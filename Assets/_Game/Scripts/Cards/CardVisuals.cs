using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using DG.Tweening;
using Sirenix.OdinInspector.Editor.Drawers;

public class CardVisuals : MonoBehaviour
{
    [Title("Container Reference")]
    [SerializeField] private CardContainer r_Container;

    [Title("Inspector References")]
    [SerializeField] private MeshRenderer m_Image;
    [SerializeField] private MeshRenderer m_Background;
    [SerializeField] private MeshRenderer m_Outline;
    [SerializeField] private TMP_Text t_Name;
    [SerializeField] private TMP_Text t_SellCost;

    public Tween CurrentTween => currentTween;
    public Vector3 Position => transform.position;

    private Tween currentTween;

    #region Unity Methods

    #endregion

    #region Public Methods
    /// <summary>
    /// Updates the visuals with new data.
    /// </summary>
    /// <param name="data">The new data that needs to be reflected in the visuals</param>
    public void Set(CardData data)
    {
        SetImage(data.cardImage);
        SetBackground(data.cardBackground);
        SetName(data.name);
        SetSellCost(data.rewardAmount);
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
    /// <param name="pos"></param>
    public void MoveInstant(Vector3 pos)
    {
        transform.position = pos;
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
    #endregion
}
