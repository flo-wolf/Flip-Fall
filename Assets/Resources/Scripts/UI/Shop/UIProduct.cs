using Impulse.Audio;
using Impulse.Progress;
using Impulse.Theme;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// The UI version of a product. Is always in sync with a ProductInfo stored in the Progres with the same id.
/// </summary>

[SerializeField]
public class UIProduct : MonoBehaviour
{
    public static BuyEvent onBuy = new BuyEvent();

    public static EquipEvent onEquip = new EquipEvent();

    /// <summary>
    /// Can you buy this multiple times (building-tools) or just once and forever?
    /// </summary>
    public enum BuyType { consumable, nonConsumable, subscription };

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
    public BuyType type;

    [Tooltip("What item gets unlocked on purchase")]
    public Product product;

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

            if (type == BuyType.nonConsumable)
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
        if (owned)
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
        if (ProgressManager.GetProgress().unlocks.BuyProduct(id))
        {
            owned = true;
            UpdateToggles();
            Debug.Log("Product bought and equipped: " + title);
            onBuy.Invoke(this);
        }
        else
        {
            Debug.Log("Product couldn't be bought: " + title);
            SoundManager.ProductPurchaseFailed();
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