using Sirenix.OdinInspector;
using System;
using UnityEngine;

/*
    <   |=      > sets bit to 1
    <   &= ~    > sets bit to 0
    <   ^=      > toggles the bit
*/
[Flags]
public enum EState
{
    Null = 0,
    CardInFront = 0x1,
    CardBehind = 0x2,

    All = 0b111111111111
}

public class CardState : MonoBehaviour
{
    [Title("Container Reference")]
    [SerializeField] private CardContainer r_Container;

    [Title("Private In-Game References")]
    public Transform GetBehindTransform => r_CardBehindParent;
    [SerializeField] private Transform r_CardBehindParent;

    [Title("Read Only Variables")]
    [ReadOnly][SerializeField] private EState r_State = EState.Null;
    [ReadOnly][SerializeField] private CardContainer r_CardInFront;
    [ReadOnly][SerializeField] private CardContainer r_CardBehind;

    public CardContainer CardInFront => r_CardInFront;
    public CardContainer CardBehind => r_CardBehind;
    public bool HasCardInFront { get { return r_State.HasFlag(EState.CardInFront); } }
    public bool HasCardBehind { get { return r_State.HasFlag(EState.CardBehind); } }

    #region Public Methods
    /// <summary>
    /// Checks the state of the card and updates the EState to reflect it.
    /// </summary>
    public void CheckForUpdates()
    {
        r_State = EState.Null;
        if (r_CardInFront)
            r_State |= EState.CardInFront;
        if (r_CardBehind)
            r_State |= EState.CardBehind;
    }
    /// <summary>
    /// Full method to attach a card behind another card.
    /// </summary>
    /// <param name="parent">The new card in front of this card.</param>
    public void AttachBehind(CardContainer parent)
    {
        // Gets the last card in the pile.
        while (parent.State.HasCardBehind)
            parent = parent.State.CardBehind;

        // Set CardBehind of the parent to this card.
        parent.State.SetCardBehind(r_Container);

        // Set the parent of this card.
        SetParent(parent.transform);

        // Set CardInFront to the parent.
        SetCardInFront(parent);

        // Move the Collision to the new position.
        MoveCollision(parent.State.GetBehindTransform);

        // Check if there needs to be any updates.
        CheckForUpdates();
    }
    /// <summary>
    /// Sets the CardBehid variable to arg0. Then Updates the State of this card.
    /// </summary>
    /// <param name="card">The card behind this card.</param>
    public void SetCardBehind(CardContainer card)
    {
        // Sets the CardBehind variable to arg0.
        r_CardBehind = card;

        // Check if there needs to be any updates.
        CheckForUpdates();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Tells the Container that this card will now be a child of another.
    /// </summary>
    /// <param name="parent">The new parent of the container.</param>
    private void SetParent(Transform parent)
    {
        // Sets the containers parent, and sets it's local transform to zero.
        r_Container.SetParent(parent);
    }
    /// <summary>
    /// Sets the CardInFront variable to arg0.
    /// </summary>
    /// <param name="card">The card in front of this card.</param>
    private void SetCardInFront(CardContainer card)
    {
        r_CardInFront = card;
    }
    /// <summary>
    /// Tells the Collision gameobject to move to a position/rotation.
    /// </summary>
    /// <param name="trans">Transform to move to.</param>
    private void MoveCollision(Transform trans)
    {
        // Lerps the Transform of the Collision object to the new Transform.
        r_Container.Collision.Move(trans);
    }
    #endregion
}