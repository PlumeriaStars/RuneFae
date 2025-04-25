using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShrineList : MonoBehaviour
{
    [SerializeField] ShrineController _shrineController;
    [SerializeField] PlayerBehavior _playerController;

    private void Start()
    {
        _playerController = FindObjectOfType<PlayerBehavior>().GetComponent<PlayerBehavior>();
    }
    private void Update()
    {

        if (_playerController.GetSpiritCount() >= int.Parse(_shrineController._itemCost[0].text))
            _shrineController._itemButton[0].interactable = true;
        if (_playerController.GetSpiritCount() >= int.Parse(_shrineController._itemCost[1].text))
            _shrineController._itemButton[1].interactable = true;
        if (_playerController.GetSpiritCount() >= int.Parse(_shrineController._itemCost[2].text))
            _shrineController._itemButton[2].interactable = true;

        if (_playerController.GetSpiritCount() < int.Parse(_shrineController._itemCost[0].text))
            _shrineController._itemButton[0].interactable = false;
        if (_playerController.GetSpiritCount() < int.Parse(_shrineController._itemCost[1].text))
            _shrineController._itemButton[1].interactable = false;
        if (_playerController.GetSpiritCount() < int.Parse(_shrineController._itemCost[2].text))
            _shrineController._itemButton[2].interactable = false;
    }

    public void SetHealth01(int itemNumber)
    {
        _shrineController._itemNames[itemNumber].text = "Health I";
        _shrineController._itemDescriptions[itemNumber].text = "Increase maximum health by a small amount, and heal some health..";
        int itemCost = Random.Range(1, 6);
        _shrineController._itemCost[itemNumber].text = "";
        _shrineController._itemCost[itemNumber].text = itemCost + "";
        _shrineController._itemButton[itemNumber].onClick.AddListener(delegate { IncreasePlayerHealth(5, itemCost, itemNumber); });

        EnoughSpirits(itemNumber, itemCost);

    }

    private void IncreasePlayerHealth(int health, int cost, int itemNum)
    {
        _playerController.IncreaseMaxHealth(health);
        _playerController.DecreaseSpiritCount(cost);
        _shrineController.DisplayConfirmation("Increased Health");
        _shrineController.RefreshShopFull();
        _shrineController.SetReroll(true);
    }

    public void SetSpeed01(int itemNumber)
    {
        _shrineController._itemNames[itemNumber].text = "Speed I";
        _shrineController._itemDescriptions[itemNumber].text = "Increase maximum speed by a small amount.";
        int itemCost = Random.Range(2, 7);
        _shrineController._itemCost[itemNumber].text = "";
        _shrineController._itemCost[itemNumber].text = itemCost + "";
        _shrineController._itemButton[itemNumber].onClick.AddListener(delegate { IncreasePlayerSpeed(0.25f, itemCost, itemNumber); });
        
        EnoughSpirits(itemNumber, itemCost);

    }

    private void IncreasePlayerSpeed(float speed, int cost, int itemNum)
    {
        Debug.Log("Cost: " + cost);
        _shrineController._itemButton[itemNum].onClick.RemoveAllListeners();
        _playerController.IncreaseMaxSpeed(speed);
        _playerController.DecreaseSpiritCount(cost);
        _shrineController.DisplayConfirmation("Increased Speed");
        _shrineController.RefreshShopFull();
        _shrineController.SetReroll(true);
    }

    public void SetShield(int itemNumber)
    {
        _shrineController._itemNames[itemNumber].text = "Shield";
        _shrineController._itemDescriptions[itemNumber].text = "Gain an impenetrable shield. Takes up the [E] slot.";
        int itemCost = Random.Range(18, 25);
        _shrineController._itemCost[itemNumber].text = "";
        _shrineController._itemCost[itemNumber].text = itemCost + "";

        _shrineController._itemButton[itemNumber].onClick.AddListener(delegate { EnableShield(itemCost, itemNumber); });

        EnoughSpirits(itemNumber, itemCost);
    }

    private void EnableShield(int cost, int itemNum)
    {
        _playerController.EnableShield();
        _playerController.DecreaseSpiritCount(cost);
        _shrineController.DisplayConfirmation("Purchased Shield");
        _shrineController.RefreshShopFull();
        _shrineController.SetReroll(true);
    }

    public void SetDoubleJump(int itemNumber)
    {
        _shrineController._itemNames[itemNumber].text = "Cloud Step";
        _shrineController._itemDescriptions[itemNumber].text = "Gain the double jump ability. Takes up the [E] slot.";
        int itemCost = Random.Range(18, 25);
        _shrineController._itemCost[itemNumber].text = "";
        _shrineController._itemCost[itemNumber].text = itemCost + "";

        _shrineController._itemButton[itemNumber].onClick.AddListener(delegate { EnableDoubleJump(itemCost, itemNumber); });

        EnoughSpirits(itemNumber, itemCost);
    }

    private void EnableDoubleJump(int cost, int itemNum)
    {
        _playerController.EnableDoubleJump();
        _playerController.DecreaseSpiritCount(cost);
        _shrineController.DisplayConfirmation("Purchased Double Jump");
        _shrineController.RefreshShopFull();
        _shrineController.SetReroll(true);
    }

    public void SetFireRune(int itemNumber)
    {
        _shrineController._itemNames[itemNumber].text = "Molten Stone";
        _shrineController._itemDescriptions[itemNumber].text = "A strange stone that burns with the power of a primordial core.";
        int itemCost = Random.Range(20, 26);
        _shrineController._itemCost[itemNumber].text = "" + itemCost;

        _shrineController._itemButton[itemNumber].onClick.AddListener(delegate { ChangeElement(1, itemCost, itemNumber); });

        EnoughSpirits(itemNumber, itemCost);
    }


    public void SetWaterRune(int itemNumber)
    {
        _shrineController._itemNames[itemNumber].text = "Ocean Shell";
        _shrineController._itemDescriptions[itemNumber].text = "A strangely cold and wet shell. The ocean waves can be heard within it.";
        int itemCost = Random.Range(20, 26);
        _shrineController._itemCost[itemNumber].text = "" + itemCost;

        _shrineController._itemButton[itemNumber].onClick.AddListener(delegate { ChangeElement(2, itemCost, itemNumber); });

        EnoughSpirits(itemNumber, itemCost);
    }

    public void SetWindRune(int itemNumber)
    {
        _shrineController._itemNames[itemNumber].text = "Dandelion Seed";
        _shrineController._itemDescriptions[itemNumber].text = "A strange seed that is easily swept up by the wind..";
        int itemCost = Random.Range(20, 26);
        _shrineController._itemCost[itemNumber].text = "" + itemCost;

        _shrineController._itemButton[itemNumber].onClick.AddListener(delegate { ChangeElement(3, itemCost, itemNumber); });

        EnoughSpirits(itemNumber, itemCost);
    }

    private void ChangeElement(int elem, int cost, int itemNum)
    {
        _playerController.MeleeAttackFinish();
        _playerController.ChangeElement(elem);
        _playerController.DecreaseSpiritCount(cost);

        if (elem == 1)
            _shrineController.DisplayConfirmation("Power of Fire");
        if(elem == 2)
            _shrineController.DisplayConfirmation("Power of Water");
        if(elem == 3)
            _shrineController.DisplayConfirmation("Power of Wind");

        _shrineController.RefreshShopFull();
        _shrineController.SetReroll(true);
    }

    private void EnoughSpirits(int itemNumber, int itemCost)
    {
        if (_playerController.GetSpiritCount() < itemCost)
            _shrineController._itemButton[itemNumber].interactable = false;
        else if(_playerController.GetSpiritCount() >= itemCost)
            _shrineController._itemButton[itemNumber].interactable = true;
    }

    public void RemoveAllListeners()
    {
        _shrineController._itemButton[0].onClick.RemoveAllListeners();
        _shrineController._itemButton[1].onClick.RemoveAllListeners();
        _shrineController._itemButton[2].onClick.RemoveAllListeners();
    }
}
