using Impulse;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public ParticleSystem shotPS;
    public Animation shotAnimation;

    // Use this for initialization
    private void Start()
    {
        Player.onPlayerStateChange.AddListener(PlayerStateChanged);
        shotPS = GetComponent<ParticleSystem>();
    }

    private void PlayerStateChanged(Player.PlayerState playerState)
    {
        switch (playerState)
        {
            case Player.PlayerState.alive:
                shotPS.Clear();
                shotPS.Stop();
                shotPS.Play();
                break;

            case Player.PlayerState.dead:
                break;

            case Player.PlayerState.win:
                break;

            default:
                break;
        }
    }

    private void Fire()
    {
        shotPS.Play();
        shotAnimation.Play();
    }

    // Update is called once per frame
    private void Update()
    {
    }
}