using UnityEngine;

public class RecipeCardButton : MonoBehaviour
{
    [Tooltip("0 = Coffee, 1 = Tea, 2 = Muffin, 3 = Pancakes, 4 = Special Pawfee")]
    public int recipeIndex; 

    // Called automatically by the card's button UI hook
    public void OnCardClicked()
    {
        if (RecipeManager.Instance != null)
        {
            RecipeManager.Instance.SelectRecipeCard(recipeIndex);
        }
    }
}

