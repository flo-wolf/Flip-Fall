using FlipFall.Audio;
using FlipFall.LevelObjects;
using FlipFall.Progress;
using FlipFall.Theme;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// The UI version of a product sold in the Shop Menu.
/// Is always in sync with a ProductInfo, which gets stored in the current progres with the same id.
/// </summary>

[SerializeField]
public class UIProduct : MonoBehaviour
{
    public static BuyEvent onBuy = new BuyEvent();

    public static EquipEvent onEquip = new EquipEvent();

    public static BuyEvent onBuyFail = new BuyEvent();

    /// <summary>
    /// Can you buy this multiple times (building-tools) or just once and forever?
    /// </summary>
    public enum BuyType { consumable, nonConsumable, subscription };

    /// <summary>
    /// what kind of product is sold
    /// </summary>
    public enum ProductType { theme, skin, supply }

    public class BuyEvent : UnityEvent<UIProduct> { }

    public class EquipEvent : UnityEvent<UIProduct> { }

    [Header("UIProduct Settings")]
    [Tooltip("Unique identifier, same as coresponding entry in the progress save-file")]
    public int id = -1;

    [Tooltip("Displayed title, not saved in progress")]
    public string title = "default";

    [Tooltip("How much does this cost?")]
    public int price;

    [Tooltip("Rebuy possible /or/ Buy once and forever")]
    public BuyType buyType;

    [Tooltip("What type is this product")]
    public ProductType productType;

    [Tooltip("In case the producttype is set to theme, unlock this")]
    public ThemeManager.Skin themeToUnlock;

    [Tooltip("In case the producttype is set to supply, unlock this")]
    public LevelObject.ObjectType supplyToUnlock;

    [Header("Debug")]
    [Tooltip("You own this item.")]
    public bool owned = false;

    [Tooltip("This item is equipped.")]
    public bool equipped = false;

    [Header("UIProduct Components")]
    public Text titleText;
    public Text priceText;
    public Button buyButton;
    public Toggle equipToggle;

    // prevent changes to toogle.isOn via script from invoking the tooglechange call and executing the Equip mothod
    private bool unvalidToogleCall = false;

    // corresponding product information data, stored in progress save-file
    private ProductInfo productInfo;

    public void Start()
    {
        ProgressManager.GetProgress().unlocks.ReportUIProductsExistence(id);

        owned = ProgressManager.GetProgress().unlocks.IsOwned(id);
        equipped = ProgressManager.GetProgress().unlocks.IsEquipped(id);
        UpdateToggles();
        UpdateTexts();

        Unlocks.onProductInfoChange.AddListener(GetCorespondingProductInfo);
    }

    // get the corresponding product info and update the UIProduct values
    // called whenever any productInfo gets updated
    private void GetCorespondingProductInfo()
    {
        productInfo = ProgressManager.GetProgress().unlocks.GetProductInfo(id);
        if (productInfo == null)
            Debug.LogError("ProductInfo is null, couldn't update it!");
        else
        {
            owned = productInfo.owned;

            if (buyType == BuyType.nonConsumable)
                equipped = productInfo.equipped;
            else
                equipped = false;

            UpdateToggles();
        }
    }

    // set texts to fit inspector variables
    public void UpdateTexts()
    {
        titleText.text = title;
        priceText.text = price.ToString();
    }

    // set toggles to fit values gathered from the progress file
    public void UpdateToggles()
    {
        if (owned && buyType == BuyType.nonConsumable)
        {
            buyButton.gameObject.SetActive(false);
            equipToggle.gameObject.SetActive(true);

            unvalidToogleCall = true;

            if (equipped)
                equipToggle.isOn = true;
            else
                equipToggle.isOn = false;

            unvalidToogleCall = false;
        }
        else
        {
            buyButton.gameObject.SetActive(true);
            equipToggle.gameObject.SetActive(false);
        }
    }

    // buys this product, if funds allow it
    public void Buy()
    {
        if (ProgressManager.GetProgress().unlocks.BuyProduct(id, buyType))
        {
            owned = true;
            UpdateToggles();
            Debug.Log("Product bought and equipped: " + title);
            onBuy.Invoke(this);
            if (productType == ProductType.theme)
            {
                ProgressManager.GetProgress().unlocks.UnlockTheme(themeToUnlock);
            }
            else if (productType == ProductType.supply)
            {
                ProgressManager.GetProgress().unlocks.inventory.Add(supplyToUnlock, 1);
            }
        }
        else
        {
            Debug.Log("Product couldn't be bought: " + title);
            SoundManager.ProductPurchaseFailed();
            onBuyFail.Invoke(this);
        }
    }

    // equips this product, if it is owned and equipable. Alsoi de-quips all other products.
    public void Equip()
    {
        if (!unvalidToogleCall)
        {
            if (ProgressManager.GetProgress().unlocks.EquipProduct(id))
            {
                Debug.Log("Product was equipped: " + title);
                onEquip.Invoke(this);
                //if (productType == ProductType.theme && themeToUnlock != ThemeManager.Skin.unset && ProgressManager.GetProgress().unlocks.currentSkin != themeToUnlock)
                //{
                Debug.Log("Equip and Switch " + themeToUnlock);
                ProgressManager.GetProgress().unlocks.SwitchTheme(themeToUnlock);
                //}
            }
            else
                Debug.Log("Product couldn't be equipped: " + title);

            if (equipped)
            {
                unvalidToogleCall = true;
                equipToggle.isOn = true;
                unvalidToogleCall = false;
            }
        }
        else
        {
            unvalidToogleCall = false;
        }
    }

    public class Product : Theme
    {
        public enum ProductType { theme, skin, supply }
        private ProductType productType;

        public Theme theme;
    }
}