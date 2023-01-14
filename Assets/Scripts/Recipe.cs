using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SlamJam2023/Recipe", fileName = "new Recipe")]
public class Recipe : ScriptableObject
{
    public List<CardData> recipeCards = new();
    public CardData result;
}