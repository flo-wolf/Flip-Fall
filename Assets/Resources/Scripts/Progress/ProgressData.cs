using FlipFall.Levels;
using FlipFall.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This is the actual Progress Data that is saved and loaded.
/// It consists out of an array of scoreboars, each of them containing the scores for a level.
/// It also holds other variables important for the players progress like achievemnts or settings.
/// </summary>

namespace FlipFall.Progress
{
    [Serializable]
    public class WalletUpdateEvent : UnityEvent { }

    [Serializable]
    public class ProgressData
    {
        public string warning;
        public string checksum;

        // level highscores and stars
        public Highscores highscores;

        public static WalletUpdateEvent onWalletUpdate = new WalletUpdateEvent();

        // sound settings etc.
        public Settings settings;

        // bought/unlocked items
        public Unlocks unlocks;

        public StoryProgress storyProgress;

        // amount of currently owned stars
        public int starsOwned;
        public int starsSpent;
        public int starsEarned;

        // pro version owned? => no ads, editor access
        public bool proVersion;

        public ProgressData()
        {
            warning = "Editing this file corrupts it and will cause a new file to be created with all the progress lost.";
            settings = new Settings();
            highscores = new Highscores();

            unlocks = new Unlocks();
            storyProgress = new StoryProgress();

            starsOwned = 0;
            starsSpent = 0;
            starsEarned = 0;
            proVersion = false;
        }

        public string GenerateChecksum()
        {
            string jsonProgress =
                "Katzenfutter"
                + JsonUtility.ToJson(highscores)
                + JsonUtility.ToJson(unlocks)
                + JsonUtility.ToJson(storyProgress)
                + starsOwned
                + starsSpent
                + starsEarned
                + proVersion
                + SystemInfo.deviceUniqueIdentifier;
            return Md5Sum(jsonProgress);
        }

        // creates an MD5 Hash out of an input string
        public string Md5Sum(string strToEncrypt)
        {
            System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);

            // encrypt bytes
            System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashBytes = md5.ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)
            string hashString = "";

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
            }

            return hashString.PadLeft(32, '0');
        }

        public int GetStarsSpent()
        {
            // get from google
            int spent = 0;
            return spent;
        }

        public void AddStarsToWallet(int stars)
        {
            starsOwned += stars;
            onWalletUpdate.Invoke();
        }
    }
}