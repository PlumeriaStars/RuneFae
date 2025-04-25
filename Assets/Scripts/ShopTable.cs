using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LootTable", menuName = "Loot Table")]
public class LootTable : ScriptableObject
{
    [SerializeField] private List<ShopItem> _items;
    [System.NonSerialized] private bool isInitialized = false;
    private float _totalWeight;
    private void Initialize()
    {
        if (!isInitialized)
        {
            _totalWeight = 0;
            foreach (var item in _items)
                _totalWeight += item.itemWeight;

            isInitialized = true;
        }
    }

    public ShopItem GetRandomItem()
    {
        Initialize();

        float diceRoll = Random.Range(0f, _totalWeight);

        foreach (var item in _items)
        {
            if (item.itemWeight >= diceRoll)
                return item;

            diceRoll -= item.itemWeight;
        }

        throw new System.Exception("Item generation failed!");
    }
}

public class ShopItem
{
    public string itemName;
    public string itemDescription;
    public float itemWeight;
    public Sprite itemSprite;
}
