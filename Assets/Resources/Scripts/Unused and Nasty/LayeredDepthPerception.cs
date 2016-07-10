using Sliders;
using System.Collections;
using UnityEngine;

/*
 *	Script by Florian Wolf, 30.12.2015
*/

public class LayeredDepthPerception : MonoBehaviour
{
    public GameObject parentObject;
    public GameObject[] gameObjects;
    public float[] distances;
    //distances[0] beschreibt die Z-Distanz zwischen dem referenceObject und gameObjects[0]
    //Die Distanz zweier Elemente führt dann zu unterschiedlich starken Tiefenwirkungen

    private bool isGameObjectsCorrupt()
    {
        if (gameObjects.Length > 0)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                if (gameObject == null)
                    return true;
            }
            return false;
        }
        else
            return true;
    }

    private bool isDistancesCorrupt()
    {
        if (distances.Length > 0)
        {
            foreach (float distance in distances)
            {
                if (distance == null || distance <= 0)
                    return true;
            }
            return false;
        }
        else
            return true;
    }

    private void FixedUpdate()
    {
        if (!isGameObjectsCorrupt() && !isDistancesCorrupt() && gameObjects.Length == distances.Length)
        {
            for (int i = 0; i < gameObjects.Length; i++)
            {
                if (parentObject.GetComponent<Rigidbody2D>() != null)
                {
                    if (gameObjects[i].GetComponent<Rigidbody2D>() == null)
                    {
                        Rigidbody2D addedRigidBody = gameObjects[i].AddComponent<Rigidbody2D>();
                        addedRigidBody.gravityScale = 0f;
                        addedRigidBody.velocity = parentObject.GetComponent<Rigidbody2D>().velocity * distances[i];
                    }
                    else if (Player.IsAlive())
                    {
                        gameObjects[i].GetComponent<Rigidbody2D>().gravityScale = 0f;
                        gameObjects[i].GetComponent<Rigidbody2D>().velocity = parentObject.GetComponent<Rigidbody2D>().velocity * distances[i];
                    }
                }
            }
        }
    }
}