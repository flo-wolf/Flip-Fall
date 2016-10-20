using Impulse.Background;
using Impulse.Theme;
using Impulse.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// information about what items were bought/unlocked, including skins and editoritems

namespace Impulse.Progress
{
    [Serializable]
    public class Unlocks
    {
        public static ProductInfoChangeEvent onProductInfoChange = new ProductInfoChangeEvent();

        [Serializable]
        public class ProductInfoChangeEvent : UnityEvent { }

        // reference to UIShopProducts items and their buy/equip states.
        public List<ProductInfo> productInfos = new List<ProductInfo>();

        public List<ThemeManager.Skin> skins;
        public bool editorUnlocked;

        public Unlocks()
        {
            skins = new List<ThemeManager.Skin>();
            productInfos = new List<ProductInfo>();
            editorUnlocked = false;
        }

        public void UnlockSkin(ThemeManager.Skin skin)
        {
            if (!skins.Any(x => x == skin))
                skins.Add(skin);
        }

        // mistake here
        public void ReportUIProductsExistence(int _id)
        {
            if (GetProductInfo(_id) == null)
            {
                Debug.Log("is null " + _id);
                productInfos.Add(new ProductInfo(_id, false, false));
            }
            else
                Debug.Log("is not null " + _id);
        }

        public void ReportUpdateUIProduct(int _id, bool _owned, bool _equiped)
        {
            ProductInfo info = GetProductInfo(_id);
            if (info == null)
            {
                productInfos.Add(new ProductInfo(_id, _owned, _equiped));
                Debug.Log("is null " + _id);
            }
            else
            {
                GetProductInfo(_id).owned = _owned;
                if (_equiped)
                    EquipProduct(_id);
                Debug.Log("is not null " + _id);
            }
            //onProductInfoChange.Invoke();
        }

        public bool BuyProduct(int _id)
        {
            Debug.Log("BuyProduct(id) " + _id);
            ProductInfo product = GetProductInfo(_id);
            UIProduct uiProduct = GetUIProductById(_id);
            if (product != null && uiProduct != null)
            {
                if (!IsOwned(_id) && uiProduct.price <= ProgressManager.GetProgress().starsOwned)
                {
                    ProgressManager.GetProgress().AddStarsToWallet(-uiProduct.price);

                    productInfos.Find(x => x.id == _id).owned = true;
                    EquipProduct(_id);
                    return true;
                }
                Debug.Log("starsOwned " + ProgressManager.GetProgress().starsOwned);
                return false;
            }
            return false;
        }

        public bool EquipProduct(int _id)
        {
            ProductInfo product = GetProductInfo(_id);
            if (product != null && IsOwned(_id) && !IsEquipped(_id))
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

        public UIProduct GetUIProductById(int _id)
        {
            if (UIShopManager.uiProducts.Any(x => x.id == _id))
            {
                return UIShopManager.uiProducts.Find(x => x.id == _id);
            }
            return null;
        }

        public ProductInfo GetProductInfo(int _id)
        {
            if (productInfos.Count > 0 && productInfos.Any(x => x.id == _id))
            {
                return productInfos.Find(x => x.id == _id);
            }
            //create productinfo, not owned, not bought
            return null;
        }

        public bool IsOwned(int _id)
        {
            ProductInfo product = GetProductInfo(_id);
            if (product != null)
            {
                return productInfos.Find(x => x.id == _id).owned;
            }
            return false;
        }

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

        public void UnlockEditor()
        {
            editorUnlocked = true;
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