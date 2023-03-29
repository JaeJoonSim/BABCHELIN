using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeSystem : MonoBehaviour
{
    public List<Recipe> recipeList;

    public void AddRecipe(List<Item> ingredients, Item result)
    {
        Recipe recipe = new Recipe();
        recipe.ingredients = ingredients;
        recipe.result = result;
        recipeList.Add(recipe);
    }

    public List<Recipe> FindCraftableRecipes(List<Item> inventoryItems)
    {
        List<Recipe> craftableRecipes = new List<Recipe>();
        foreach (Recipe recipe in recipeList)
        {
            if (recipe.CanCraft(inventoryItems))
            {
                craftableRecipes.Add(recipe);
            }
        }
        return craftableRecipes;
    }
}

public class Recipe
{
    public List<Item> ingredients;
    public Item result;

    public bool CanCraft(List<Item> inventoryItems)
    {
        foreach (Item ingredient in ingredients)
        {
            if (!inventoryItems.Contains(ingredient))
            {
                return false;
            }
        }
        return true;
    }
}