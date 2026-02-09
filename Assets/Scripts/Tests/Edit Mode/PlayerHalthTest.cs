using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerHalthTest
{

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator PlayerTakeDamage()
    {
        var playerGO = new GameObject();
        var health = playerGO.AddComponent<PlayerHealth>();

        health.Damage();

        yield return null;

        Assert.AreEqual(2.5f, health.healthPoints);

        Object.DestroyImmediate(playerGO);
    }
}
