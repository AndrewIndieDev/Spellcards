using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using Sirenix.OdinInspector;
using System;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private GridManager Grid => GridManager.Instance;
    private PlayerInputActions InputActions => InputManager.Instance.InputActions;

    public delegate void OnInteractUp();
    public static event OnInteractUp onInteractUp;

    [Title("Inspector References")]
    [SerializeField] private AbilityVisual r_AbilityVisual;

    [Title("Inspector Variables")]
    [SerializeField] private LayerMask table;

    [Title("Read-only Variables")]
    [SerializeField][ReadOnly] private List<CardContainer> selectedCards = new();
    [SerializeField][ReadOnly] private List<CardData> selectedCardData = new();
    [SerializeField][ReadOnly] private List<Recipe> recipes_Materials;
    [SerializeField][ReadOnly] private List<Recipe> recipes_Spells;

    #region Unity Methods
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        recipes_Materials = Resources.LoadAll<Recipe>("Recipes/Materials").ToList();
        recipes_Spells = Resources.LoadAll<Recipe>("Recipes/Spells").ToList();

        InputActions.Player.Interact.performed += Player_OnInteract;
        InputActions.Player.Interact.canceled += Player_OnInteract;
        InputActions.Player.Execute.performed += Player_OnExecute;
        InputActions.Player.Execute.canceled += Player_OnExecute;
        InputActions.Player.NavigationMouse.performed += Player_OnNavigationMouse;
        InputActions.Player.NavigationGamepad.performed += Player_OnNavigationGamepad;
        InputActions.Player.Craft.performed += Player_OnCraft; ;

        InputActions.Crafting.Craft.performed += Crafting_OnCraft;
        InputActions.Crafting.Select.performed += Crafting_OnSelect;
        InputActions.Crafting.Exit.performed += Crafting_OnExit;
        InputActions.Crafting.NavigationMouse.performed += Player_OnNavigationMouse;
    }
    private void OnDestroy()
    {
        InputActions.Player.Interact.performed -= Player_OnInteract;
        InputActions.Player.Interact.canceled -= Player_OnInteract;
        InputActions.Player.Execute.performed -= Player_OnExecute;
        InputActions.Player.Execute.canceled -= Player_OnExecute;
        InputActions.Player.NavigationMouse.performed -= Player_OnNavigationMouse;
        InputActions.Player.NavigationGamepad.performed -= Player_OnNavigationGamepad;
        InputActions.Player.Craft.performed -= Player_OnCraft; ;

        InputActions.Crafting.Craft.performed -= Crafting_OnCraft;
        InputActions.Crafting.Select.performed -= Crafting_OnSelect;
        InputActions.Crafting.Exit.performed -= Crafting_OnExit;
    }
    private void Update()
    {

    }
    #endregion

    #region Callbacks
    private void Player_OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
            Grid.InteractPlaceable(Grid.SelectionPositionGrid, EGridCellOccupiedFlags.Card);
        else if (context.canceled)
            onInteractUp?.Invoke();
    }
    private void Player_OnExecute(InputAction.CallbackContext context)
    {
        CardContainer card = Grid.GetPlaceableAtPosition(Grid.SelectionPositionGrid, EGridCellOccupiedFlags.Card) as CardContainer;
        if (card == null || card.CardData.abilities.Count <= 0)
            return;
        r_AbilityVisual.Init(card.CardData.abilities[0].abilityStyle, card);
    }
    private void Player_OnNavigationMouse(InputAction.CallbackContext context)
    {
        Grid.OnNavigationMouse(context);
    }
    private void Player_OnNavigationGamepad(InputAction.CallbackContext context)
    {
        var vpos = context.ReadValue<Vector2>();
        var newPosition = Grid.SelectionPositionGrid + new Vector2Int(Mathf.RoundToInt(vpos.x), Mathf.RoundToInt(vpos.y));
        Grid.MoveSelection(newPosition);
    }
    private void Crafting_OnExit(InputAction.CallbackContext obj)
    {
        DisableCrafting();
    }

    private void Crafting_OnSelect(InputAction.CallbackContext obj)
    {
        if (Grid.GetPlaceableAtPosition(Grid.SelectionPositionGrid, EGridCellOccupiedFlags.Card) is CardContainer card)
        {
            if (!selectedCards.Contains(card))
            {
                selectedCards.Add(card);
                selectedCardData.Add(card.CardData);
                card.Select();
            }
            else
            {
                selectedCards.Remove(card);
                selectedCardData.Remove(card.CardData);
                card.Deselect();
            }
        }
    }

    private void Player_OnCraft(InputAction.CallbackContext obj)
    {
        EnableCrafting();
    }

    private void Crafting_OnCraft(InputAction.CallbackContext obj)
    {
        /* SEARCH FOR A RECIPE WITHIN THE SELECTED CARDS
         * Get first card's recipe list
         * Search to see if any of those recipes can be made
         * Get a central position between the cards
         * Delete the selected cards in the recipe
         * Visually show the deletion of any cards that aren't in the recipe, and therefore are lost in the process
         * Create a 'crafting card' with the given recipe at that central position
         * If no recipe can be made out of the first card, it moves to the second and so on
         * If no recipe can be made out of any cards, then it deselects the cards with nothing lost
         */

        // If we have no cards selected, then we can't craft anything
        if (selectedCards.Count <= 0)
        {
            DisableCrafting();
            return;
        }

        // Get the center position of all the selected cards
        Vector3 centerPos;
        if (selectedCards.Count == 1)
            centerPos = selectedCards[0].transform.position;
        else
        {
            Vector3 pos = Vector3.zero;
            for (int i = 0; i < selectedCards.Count; i++)
                pos += selectedCards[i].Collision.Position;
            centerPos = pos / selectedCards.Count;
        }

        // Get all the neighbours of the center position
        List<Vector3> allPositions = GetNeighbours(centerPos);

        // Search for all recipes that can be made with the selected cards
        Dictionary<CardContainer, Vector3> cardsToMove = new();
        List<CardData> checking = new(selectedCardData);
        bool craftingFound = false;
        int cardsCrafted = 0;
        for (int i = 0; i < recipes_Materials.Count; i++) // Card to check
        {
            bool success = true;
            for (int o = 0; o < recipes_Materials[i].recipeCards.Count; o++)
            {
                if (checking.Contains(recipes_Materials[i].recipeCards[o]))
                    continue;
                success = false;
                break;
            }
            if (!success)
                continue;
            
            // Add the recipe to the checking list incase it's used in another recipe
            checking.Add(recipes_Materials[i].result);

            // If we get here, then we have a recipe that can be made
            craftingFound = true;

            // Remove the recipe cards from the list of selected cards so we don't double up
            for (int o = 0; o < recipes_Materials[i].recipeCards.Count; o++)
            {
                foreach (var card in selectedCards)
                {
                    if (card.CardData.name == recipes_Materials[i].recipeCards[o].name)
                    {
                        cardsToMove.Add(card, allPositions[cardsCrafted]);
                    }
                }
                checking.Remove(recipes_Materials[i].recipeCards[o]);
            }
            cardsCrafted++;

            // If we have no more cards to check, then we can stop
            if (checking.Count <= 1)
                break;
        }

        for (int i = 0; i < recipes_Spells.Count; i++) // Card to check
        {
            bool success = true;
            for (int o = 0; o < recipes_Spells[i].recipeCards.Count; o++)
            {
                if (checking.Contains(recipes_Spells[i].recipeCards[o]))
                    continue;
                success = false;
                break;
            }
            if (!success)
                continue;

            // Add the recipe to the checking list incase it's used in another recipe
            checking.Add(recipes_Spells[i].result);

            // If we get here, then we have a recipe that can be made
            craftingFound = true;

            // Remove the recipe cards from the list of selected cards so we don't double up
            for (int o = 0; o < recipes_Spells[i].recipeCards.Count; o++)
            {
                foreach (var card in selectedCards)
                {
                    if (card.CardData.name == recipes_Spells[i].recipeCards[o].name)
                    {
                        cardsToMove.Add(card, allPositions[cardsCrafted]);
                    }
                }
                checking.Remove(recipes_Spells[i].recipeCards[o]);
            }
            cardsCrafted++;

            // If we have no more cards to check, then we can stop
            if (checking.Count <= 1)
                break;
        }

        // If we have no recipes to craft, then we don't craft anything
        if (!craftingFound)
        {
            DisableCrafting();
            return;
        }

        // Move the visual for all cards to the correct position and remove them from the grid
        foreach (var toMove in cardsToMove)
        {
            Grid.UnoccupyGridField(Grid.GetGridCoordsAtWorldPosition(toMove.Value), EGridCellOccupiedFlags.Card);
            toMove.Key.MoveVisualThenDestroy(toMove.Value);
        }

        for (int i = 0; i < checking.Count; i++)
        {
            CardData data = checking[i];
            Vector2Int gridPosition = Grid.GetGridCoordsAtWorldPosition(allPositions[i]);
            CardContainer craftingCard = null;
            craftingCard = SpawningManager.Instance.SpawnCrafting(gridPosition, data.craftTime,
                ()=> {
                    craftingCard.OnKill();
                    SpawningManager.Instance.SpawnCard(data, gridPosition);
                });
        }

        DisableCrafting();
    }
    #endregion

    #region Public Methods

    #endregion

    #region Private Methods
    private void EnableCrafting()
    {
        InputActions.Player.Disable();
        InputActions.Crafting.Enable();
    }
    private void DisableCrafting()
    {
        foreach (var card in selectedCards)
        {
            card.Deselect();
        }
        selectedCards.Clear();
        selectedCardData.Clear();
        InputActions.Crafting.Disable();
        InputActions.Player.Enable();
    }
    private List<Vector3> GetNeighbours(Vector3 centerPos)
    {
        Vector2Int centerGridPos = Grid.GetGridCoordsAtWorldPosition(centerPos);
        List<Vector2Int> gridNeighbours = new List<Vector2Int>() {
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(-1, -1),
        new Vector2Int(1, -1),
        new Vector2Int(0, 1),
        new Vector2Int(-1, 1),
        new Vector2Int(1, 1)
        };

        List<Vector3> neighbours = new List<Vector3> { centerPos };
        for (int i = 0; i < gridNeighbours.Count; i++)
        {
            Vector3 neighbourPos = Grid.GetGridCellCenterPosition(centerGridPos + gridNeighbours[i]);
            Vector2Int neighbourGridPos = Grid.GetGridCoordsAtWorldPosition(neighbourPos);
            if (Grid.GetPlaceableAtPosition(neighbourGridPos, EGridCellOccupiedFlags.Card) is CardContainer card
                || Grid.ClampPositionToPlayerGrid(neighbourGridPos) != neighbourGridPos)
                continue;
            else
                neighbours.Add(neighbourPos);
        }
        return neighbours;
    }
    #endregion
}
