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
    }

    private void Fire()
    {
        shotPS.Play();
    }

    // Update is called once per frame
    private void Update()
    {
    }
}