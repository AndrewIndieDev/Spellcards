using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

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

    #region Unity Methods
    private void Update()
    {
        transform.position = r_Container.Collision.transform.position;
    }
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
    #endregion

    #region Private Methods
    /// <summary>
    /// Sets the Image of the card.
    /// </summary>
    /// <param name="material">Material to use</param>
    private void SetImage(Sprite material)
    {
        m_Image.material.mainTexture = material.texture;
    }
    /// <summary>
    /// Sets the Background Image of the card.
    /// </summary>
    /// <param name="material">Material to use</param>
    private void SetBackground(Sprite material)
    {
        m_Background.material.mainTexture = material.texture;
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
