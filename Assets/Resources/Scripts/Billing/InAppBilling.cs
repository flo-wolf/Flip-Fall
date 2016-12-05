using FlipFall.Progress;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

/// <summary>
/// Handles InApp Billing through the Unity Services, which connects to the google products i am selling (the pro version).
/// </summary>

// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
public class InAppBilling : MonoBehaviour, IStoreListener
{
    public static InAppBilling _instance;

    public static ProBuyEvent onProBuy = new ProBuyEvent();

    public class ProBuyEvent : UnityEvent { }

    public static bool initialized = false;

    private static IStoreController m_StoreController; // Reference to the Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // Reference to store-specific Purchasing

    // com.company.project.item_name value
    private static string kItem = "pro_upgrade"; // General handle for the consumable product.

    private static string kGooglePlayItem = "flipfall.pro"; // Google Play Store identifier for the consumable product.

    private void Start()
    {
        Debug.Log("IN APP BILLING - Start");
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _instance = this;

        DontDestroyOnLoad(this);

        if (m_StoreController == null && initialized == false)
        {
            Debug.Log("IN APP BILLING - both null, initialize");
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }

        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(kItem);
            if (product != null && product.hasReceipt)
            {
                // Owned Non Consumables and Subscriptions should always have receipts.
                // So here the Non Consumable product has already been bought.
                ProgressManager.GetProgress().proVersion = true;
            }
            else
            {
                ProgressManager.GetProgress().proVersion = false;
            }
        }

        Debug.Log("ProgressManager: Owns Pro Version: " + ProgressManager.GetProgress().proVersion);
        //ZPlayerPrefs.Initialize("----------------", SystemInfo.deviceUniqueIdentifier);
        // If we haven't set up the Unity Purchasing reference
    }

    public void InitializePurchasing()
    {
        Debug.Log("InAppBilling: InitializePurchasing()");
        initialized = true;

        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            Debug.Log("InAppBilling: Trying to initialize, but it's already initialized.");
            return;
        }

        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add a product to sell / restore by way of its identifier, associating the general identifier with its store-specific identifiers.
        builder.AddProduct(kItem, ProductType.NonConsumable, new IDs() { { kGooglePlayItem, GooglePlay.Name } });// Continue adding the non-consumable product.

        UnityPurchasing.Initialize(this, builder);
    }

    private static bool IsInitialized()
    {
        Debug.Log("InAppBilling: IsInitialized() m_StoreController: " + m_StoreController + " m_StoreExtensionProvider: " + m_StoreExtensionProvider);
        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public static bool ProIsOwned()
    {
        bool owned = false;
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(kItem);
            if (product != null && product.hasReceipt)
            {
                // Owned Non Consumables and Subscriptions should always have receipts.
                // So here the Non Consumable product has already been bought.
                owned = true;
                ProgressManager.GetProgress().proVersion = true;
            }
            else
            {
                owned = false;
                ProgressManager.GetProgress().proVersion = false;
            }
        }
        return owned;
    }

    public static bool BuyPro()
    {
        Debug.Log("InAppBilling: BuyPro()");
        bool bought = BuyProductID(kItem);
        Debug.Log("InAppBilling: BuyPro() " + bought);
        // Buy the non-consumable product using its general identifier. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
        return bought;
    }

    private static bool BuyProductID(string productId)
    {
        // If the stores throw an unexpected exception, use try..catch to protect my logic here.
        try
        {
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                // ... look up the Product reference with the general product identifier and the Purchasing system's products collection.
                Product product = m_StoreController.products.WithID(productId);

                // If the look up found a product for this device's store and that product is ready to be sold ...
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
                    m_StoreController.InitiatePurchase(product);
                    return true;
                }
                // Otherwise ...
                else
                {
                    // ... report the product look-up failure situation
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else
            {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }
        // Complete the unexpected exception handling ...
        catch (Exception e)
        {
            // ... by reporting any unexpected exception for later diagnosis.
            Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
        }
        return false;
    }

    // Restore purchases previously made by this customer. Some platforms automatically restore purchases. Apple currently requires explicit purchase restoration for IAP.
    public static void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ...
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }

    //
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        // A consumable product has been purchased by this user.
        if (String.Equals(args.purchasedProduct.definition.id, kItem, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            //If the item has been successfully purchased, store the item for later use!
            ProgressManager.GetProgress().proVersion = true;
            onProBuy.Invoke();
            //PlayerPrefs.SetInt("Items_All", 1);
            //_Main.Invoke("Got_Items", 0); //Call a function in another script to play some effects.
        }
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
            ProgressManager.GetProgress().proVersion = false;
        }
        // Return a flag indicating wither this product has completely been received, or if the application needs to be reminded of this purchase at next app launch. Is useful when saving purchased products to the cloud, and when that save is delayed.
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing this reason with the user.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }
}