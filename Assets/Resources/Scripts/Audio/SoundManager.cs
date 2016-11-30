using FlipFall.Cam;
using FlipFall.Levels;
using FlipFall.Progress;
using FlipFall.UI;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Listens to game events and plays sounds accordingly through the SoundPlayer class
/// </summary>
namespace FlipFall.Audio
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager _instance;
        private SoundPlayer soundPlayer;

        [Header("Game Sounds")]
        public AudioClip playSound;
        public AudioClip spawnSound;
        public AudioClip deathSound;
        public AudioClip reflectSound;
        public AudioClip chargeSound;
        public AudioClip winSound;
        public AudioClip turretShot;
        public AudioClip attractorRumble;

        [Header("UI Sounds")]
        public AudioClip buttonClickSound;
        public AudioClip buttonReleaseSound;
        public AudioClip levelChangeSound;
        public AudioClip timerSound;
        public AudioClip unvalidSound;
        public AudioClip defaultButtonSound;
        public AudioClip levelselectionAppearSound;
        public AudioClip camTransitionSound;
        public AudioClip uiLevelSwitchSound;
        public AudioClip purchaseSound;
        public AudioClip achievementSound;
        public AudioClip purchaseFailSound;
        public AudioClip wooshSound;
        public AudioClip starGetSound;

        [Header("Music")]
        public AudioClip backgroundSound;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(this.gameObject);

            Game.onGameStateChange.AddListener(GameStateChanged);
            Player.onPlayerAction.AddListener(PlayerAction);
            Player.onPlayerStateChange.AddListener(PlayerStateChanged);
            CamMove.onCamMoveStateChange.AddListener(CamMoveStateChanged);
            UIGameButtons.onButtonClick.AddListener(ButtonClicked);
            UIGameButtons.onButtonRelease.AddListener(ButtonReleased);
            //UILevelselectionManager.onUILevelSwitch.AddListener(UILevelSwitched);
            UILevelDrag.onBounceBack.AddListener(PlayLevelSwitchSound);
            Main.onSceneChange.AddListener(SceneChanging);
            Main.onAchievementUnlock.AddListener(AchievementUnlocked);
            UIProduct.onBuy.AddListener(ProductPurchased);
            UIProduct.onBuyFail.AddListener(BuyFail);
            UIProduct.onEquip.AddListener(ProductEquipped);
        }

        private void Start()
        {
            soundPlayer = SoundPlayer._instance;
            soundPlayer.PlayMusic(backgroundSound);
        }

        private void SceneChanging(Main.Scene scene)
        {
            switch (scene)
            {
                case Main.Scene.game:
                    break;

                default:
                    //soundPlayer.PlaySingle(reflectSound);
                    break;
            }
        }

        //Listener
        private void PlayerAction(Player.PlayerAction playerAction)
        {
            switch (playerAction)
            {
                case Player.PlayerAction.reflect:
                    soundPlayer.RandomizeSfx(reflectSound);
                    break;

                case Player.PlayerAction.charge:
                    soundPlayer.PlaySingle(chargeSound);
                    break;

                case Player.PlayerAction.decharge:
                    break;

                default:
                    break;
            }
        }

        private void CamMoveStateChanged(CamMove.CamMoveState moveState)
        {
            switch (moveState)
            {
                case CamMove.CamMoveState.transitioning:
                    soundPlayer.RandomizeSfx(camTransitionSound);
                    break;

                default:
                    break;
            }
        }

        //Listener
        private void PlayerStateChanged(Player.PlayerState playerState)
        {
            switch (playerState)
            {
                case Player.PlayerState.alive:
                    break;

                case Player.PlayerState.dead:
                    soundPlayer.PlaySingle(deathSound);
                    PlayWooshSound();
                    break;

                case Player.PlayerState.win:
                    soundPlayer.PlaySingle(winSound);
                    soundPlayer.PlaySingle(achievementSound);
                    //PlayWooshSound();
                    break;

                default:
                    break;
            }
        }

        private void GameStateChanged(Game.GameState gameState)
        {
            switch (gameState)
            {
                case Game.GameState.playing:
                    //soundPlayer.PlaySingle(playSound);
                    break;

                case Game.GameState.deathscreen:
                    break;

                case Game.GameState.finishscreen:

                    //play win sound
                    break;

                //case Game.GameState.levelselection:
                //    soundPlayer.PlaySingle(levelselectionAppearSound);
                //    //soundPlayer.PlaySingle(spawnSound);
                //    break;

                default:
                    break;
            }
        }

        // event listener
        private void BuyFail(UIProduct product)
        {
            PlayUnvalidSound();
        }

        private void AchievementUnlocked()
        {
            soundPlayer.PlaySingle(achievementSound);
        }

        private void ProductPurchased(UIProduct product)
        {
            soundPlayer.PlaySingle(purchaseSound);
        }

        public static void ProductPurchaseFailed()
        {
            _instance.soundPlayer.PlaySingle(_instance.purchaseFailSound);
        }

        private void ProductEquipped(UIProduct product)
        {
            soundPlayer.PlaySingle(buttonClickSound);
        }

        private void ButtonClicked(Button b)
        {
            soundPlayer.PlaySingle(buttonClickSound);
        }

        public static void ButtonClicked()
        {
            if (_instance.soundPlayer != null)
                _instance.soundPlayer.PlaySingle(_instance.buttonClickSound);
        }

        private void ButtonReleased(Button b)
        {
            soundPlayer.PlaySingle(buttonReleaseSound);
        }

        public static void PlayWooshSound()
        {
            if (_instance.soundPlayer != null)
                _instance.soundPlayer.PlaySingle(_instance.wooshSound);
        }

        public static void PlayLevelSwitchSound()
        {
            if (_instance.soundPlayer != null)
                _instance.soundPlayer.PlaySingle(_instance.levelChangeSound);
        }

        public void PlayTimerSound()
        {
            soundPlayer.PlaySingle(timerSound);
        }

        public static void PlayRumbleSound(Vector3 pos)
        {
            _instance.soundPlayer.PlayAttractorRumble(_instance.attractorRumble, pos);
        }

        public static void PlayUnvalidSound()
        {
            _instance.soundPlayer.PlaySingle(_instance.unvalidSound);
        }

        public static void PlayStarGetSound()
        {
            _instance.soundPlayer.PlaySingle(_instance.starGetSound);
        }

        public static void PlayLightWobble()
        {
            _instance.soundPlayer.PlaySingle(_instance.turretShot);
            Debug.Log("playlight");
        }

        public static void PlayLightWobble(float pitch)
        {
            _instance.soundPlayer.PlaySingle(_instance.turretShot, pitch);
            Debug.Log("playlight pitched");
        }

        public static void TurretShot(Vector3 position)
        {
            if (Player._instance != null)
            {
                float distanceToPlayer = Vector3.Distance(Player._instance.transform.position, position);
                _instance.soundPlayer.PlaySingleAt(_instance.turretShot, position, distanceToPlayer);
            }
        }

        public static void UILevelSwitched()
        {
            _instance.soundPlayer.PlaySingle(_instance.uiLevelSwitchSound);
        }

        //CHANGE SOUND IN HERE!
        public static void UILevelBouncedBack()
        {
            _instance.soundPlayer.PlaySingle(_instance.uiLevelSwitchSound);
        }

        public static void PlayCamTransitionSound()
        {
            _instance.soundPlayer.PlaySingle(_instance.camTransitionSound);
        }
    }
}