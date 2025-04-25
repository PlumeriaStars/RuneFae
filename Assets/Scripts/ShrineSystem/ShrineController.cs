using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShrineController : MonoBehaviour
{
    [Header("UI Elements")]
    public Text[] _itemNames;
    public Text[] _itemDescriptions;
    public Text[] _itemCost;
    public Button[] _itemButton;
    public Image[] _itemImage;
    [SerializeField] GameObject[] _itemPanels;
    [SerializeField] GameObject _shrinePanel;
    [SerializeField] GameObject _statusPanel;
    [SerializeField] Text[] _purchaseText;
    [SerializeField] Button _rerollButton;
    [SerializeField] GameObject[] _itemSlots;
    [SerializeField] Sprite[] _itemSprites;

    [Header("Item Weights")]
    [SerializeField] int _healthWeight = 31;
    [SerializeField] int _speedWeight = 30;
    [SerializeField] int _shieldWeight = 12;
    [SerializeField] int _jumpWeight = 12;
    [SerializeField] int _fireWeight = 5;
    [SerializeField] int _waterWeight = 5;
    [SerializeField] int _windWeight = 5;

    [SerializeField] ShrineList _shrineListController = null;
    [SerializeField] PlayerBehavior _playerController;

    private bool _needReroll = true;
    private int _rerollsLeft = 3;

    void Start()
    {
        DisablePanels();
        _playerController = FindObjectOfType<PlayerBehavior>().GetComponent<PlayerBehavior>();
    }

    private void EnablePanels()
    {
        _shrinePanel.SetActive(true);
        if(_needReroll)
            RefreshShopFull();
          
        _statusPanel.SetActive(true);
    }

    private void DisablePanels()
    {
        _shrinePanel.SetActive(false);
        _statusPanel.SetActive(false);
        DisablePuchaseText();
    }

    public void PlayerNotInShop()
    {
        _playerController.ClosedShop();
    }

    private void RollItem(int itemNum)
    {
        int randomValue = Random.Range(1, 101);
        int healthBound = _healthWeight;
        int speedBound = _healthWeight + _speedWeight;
        int shieldBound = speedBound + _shieldWeight;
        int jumpBound = shieldBound + _jumpWeight;
        int fireBound = jumpBound + _fireWeight;
        int waterBound = fireBound + _waterWeight;
        int windBound = waterBound + _windWeight;

        if (randomValue >= 0 && randomValue <= healthBound)
        {
            _shrineListController.SetHealth01(itemNum);
            SetSprite(itemNum, 0);
        }
        else if(randomValue > healthBound && randomValue <= speedBound)
        {
            _shrineListController.SetSpeed01(itemNum);
            SetSprite(itemNum, 1);
        }
        else if (randomValue > speedBound && randomValue <= shieldBound)
        {
            _shrineListController.SetShield(itemNum);
            SetSprite(itemNum, 2);
        }
        else if (randomValue > shieldBound && randomValue <= jumpBound)
        {
            _shrineListController.SetDoubleJump(itemNum);
            SetSprite(itemNum, 3);
        }
        else if (randomValue > jumpBound && randomValue <= fireBound)
        {
            _shrineListController.SetFireRune(itemNum);
            SetSprite(itemNum, 4);
        }
        else if (randomValue > fireBound && randomValue <= waterBound)
        {
            _shrineListController.SetWaterRune(itemNum);
            SetSprite(itemNum, 5);
        }
        else if (randomValue > waterBound && randomValue <= windBound)
        {
            _shrineListController.SetWindRune(itemNum);
            SetSprite(itemNum, 6);
        }
    }

    public void RefreshShopFull()
    {
        _shrineListController.RemoveAllListeners();
        RollItem(0);
        RollItem(1);
        RollItem(2);

        _needReroll = false;
        _rerollsLeft = 3;
        _rerollButton.interactable = true;
        _rerollButton.GetComponentInChildren<Text>().text = "Rerolls Left: " + _rerollsLeft;
    }

    public void RefreshShopPartial()
    {
        _shrineListController.RemoveAllListeners();
        RollItem(0);
        RollItem(1);
        RollItem(2);

        _needReroll = false;
        _rerollsLeft--;

        if (_rerollsLeft <= 0)
            _rerollButton.interactable = false;
        _rerollButton.GetComponentInChildren<Text>().text = "Rerolls Left: " + _rerollsLeft;
    }

    public void EnableShop()
    {
        EnablePanels();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void DisableShop()
    {
        DisablePanels();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetReroll(bool value)
    {
        _needReroll = value;
    }

    public void SetSprite(int item, int spriteIndex)
    {
        _itemSlots[item].GetComponent<Image>().sprite = _itemSprites[spriteIndex];
    }

    public void DisplayConfirmation(string purchase)
    {
        _statusPanel.SetActive(true);
        _purchaseText[0].enabled = true;
        _purchaseText[0].text = purchase;
    }

    public void DisplayInvaild()
    {
        _purchaseText[1].enabled = true;
        _purchaseText[0].enabled = false;
    }

    public void DisablePuchaseText()
    {
        _purchaseText[0].text = "";
        _purchaseText[0].enabled = false;
        _purchaseText[1].enabled = false;
    }

}

/*
 * TIER 1 TABLE            WEIGHTS
 * Health                   31
 * Speed                    30
 * Shield                   12
 * Jump                     12
 * Fire                     05
 * Water                    05
 * Wind                     05
 * */
