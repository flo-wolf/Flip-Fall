using Impulse.Progress;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FlipFall.Shop
{
    [Serializable]
    public class ShopProduct
    {
        public static BuyEvent onBuy = new BuyEvent();

        /// <summary>
        /// <para>ID, Price</para>
        /// </summary>
        public class BuyEvent : UnityEvent<int, int> { }

        public enum ProductType { Theme, Skin, Building }
        public ProductType productType;

        public int id = -1;
        public string title = "default";
        public bool owned = false;
        public int price;

        public ShopProduct(int _id, int _price, ProductType _productType)
        {
            productType = _productType;
            id = _id;
            price = _price;
        }

        public void Buy()
        {
            if (price < ProgressManager.GetProgress().starsOwned)
            {
                ProgressManager.GetProgress().starsOwned -= price;
                owned = true;
                onBuy.Invoke(id, price);
            }
        }
    }
}