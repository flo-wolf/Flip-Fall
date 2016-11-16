using FlipFall.Background;
using FlipFall.Editor;
using FlipFall.Theme;
using FlipFall.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// information about what items were bought/unlocked, including skins and editoritems

namespace FlipFall.Progress
{
    [Serializable]
    public class Unlocks
    {
        public static ProductInfoChangeEvent onProductInfoChange = new ProductInfoChangeEvent();

        [Serializable]
        public class ProductInfoChangeEvent : UnityEvent { }

        // reference to UIShopProducts items and their buy/equip states.
        public List<ProductInfo> productInfos = new List<ProductInfo>();

        // contains all levelObjects
        public Inventory inventory;

        public List<ThemeManager.Skin> unlockedThemes;
        public ThemeManager.Skin defaultSkin;
        public ThemeManager.Skin currentSkin;

        public Unlocks()
        {
            defaultSkin = ThemeManager.Skin.toxic;
            currentSkin = ThemeManager.Skin.toxic;

            inventory = new Inventory();

            unlockedThemes = new List<ThemeManager.Skin>();
            unlockedThemes.Add(defaultSkin);

            productInfos = new List<ProductInfo>();
            productInfos.Add(new ProductInfo(1, true, true));
        }

        public void UnlockTheme(ThemeManager.Skin theme)
        {
            if (!unlockedThemes.Any(x => x == theme))
                unlockedThemes.Add(theme);
            ThemeManager.SetSkin(theme);
        }

        public void SwitchTheme(ThemeManager.Skin theme)
        {
            ThemeManager.SetSkin(theme);
        }

        // mistake here
        public void ReportUIProductsExistence(int _id)
        {
            if (GetProductInfo(_id) == null)
                productInfos.Add(new ProductInfo(_id, false, false));
        }

        public void ReportUpdateUIProduct(int _id, bool _owned, bool _equiped)
        {
            ProductInfo info = GetProductInfo(_id);
            if (info == null)
            {
                productInfos.Add(new ProductInfo(_id, _owned, _equiped));
            }
            else
            {
                GetProductInfo(_id).owned = _owned;
                if (_equiped)
                    EquipProduct(_id);
            }
            //onProductInfoChange.Invoke();
        }

        // Buys the product, if the product exists and the funds allow for it
        public bool BuyProduct(int _id, UIProduct.BuyType buyType)
        {
            Debug.Log("BuyProduct(id) " + _id);
            ProductInfo product = GetProductInfo(_id);
            UIProduct uiProduct = GetUIProductById(_id);
            if (product != null && uiProduct != null)
            {
                if (((!IsOwned(_id) && buyType == UIProduct.BuyType.nonConsumable) || buyType == UIProduct.BuyType.consumable) && uiProduct.price <= ProgressManager.GetProgress().starsOwned)
                {
                    ProgressManager.GetProgress().AddStarsToWallet(-uiProduct.price);

                    productInfos.Find(x => x.id == _id).owned = true;
                    EquipProduct(_id);

                    Social.ReportProgress("CgkIqIqqjZYFEAIQCw", 100.0f, (bool success) =>
                    {
                    });
                    return true;
                }
                return false;
            }
            return false;
        }

        // Equips the product, if the product is owned and not yet equipped. De-Equips all other products.
        public bool EquipProduct(int _id)
        {
            ProductInfo product = GetProductInfo(_id);
            Debug.Log("EquipProduct(id) " + product);
            if (product != null && IsOwned(_id))
            {
                Debug.Log("EquipProduct(id) " + _id);

                // de-equip all
                foreach (ProductInfo s in productInfos)
                {
                    s.equipped = false;

                    // ...except this one
                    if (s.id == _id)
                        s.equipped = true;
                }

                onProductInfoChange.Invoke();
                return true;
            }
            return false;
        }

        // Gets the corresponding UIProduct of a ProductInfo by ID
        public UIProduct GetUIProductById(int _id)
        {
            if (UIShopManager.uiProducts.Any(x => x.id == _id))
            {
                return UIShopManager.uiProducts.Find(x => x.id == _id);
            }
            return null;
        }

        // Gets the corresponding ProductInfo of an UIProduct by ID
        public ProductInfo GetProductInfo(int _id)
        {
            if (productInfos.Count > 0 && productInfos.Any(x => x.id == _id))
            {
                return productInfos.Find(x => x.id == _id);
            }
            return null;
        }

        // Is the user already in posession of this product?
        public bool IsOwned(int _id)
        {
            ProductInfo product = GetProductInfo(_id);
            if (product != null)
            {
                return productInfos.Find(x => x.id == _id).owned;
            }
            return false;
        }

        // is this product already equipped?
        public bool IsEquipped(int _id)
        {
            ProductInfo product = GetProductInfo(_id);
            if (product != null)
            {
                Debug.Log("eqiped? " + product.equipped + " id: " + _id);
                return product.equipped;
            }
            return false;
        }
    }

    [Serializable]
    public class ProductInfo
    {
        public int id;
        public bool equipped;
        public bool owned;

        public ProductInfo(int _id, bool _owned, bool _equipped)
        {
            id = _id;
            owned = _owned;
            equipped = _equipped;
        }
    }
}