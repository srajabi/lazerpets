using System.Collections.Generic;
using UnityEngine;

public enum AttackKind
{
    CatProjectile,
    DogPuke,
    BirdPoop,
}

public interface IAttackLauncher
{
    void Update(bool attackButtonPressed, Vector3 position, Vector3 direction);
}

static public class AttackLauncherFactory
{
    static public IAttackLauncher Create(AttackKind kind)
    {
        switch (kind) 
        {
            case AttackKind.CatProjectile:
                return new CatProjectileLauncher();
        }

        return null;
    }
}

public class CatProjectileLauncher : IAttackLauncher
{
    bool buttonDown = false;

    public void Update(bool attackButtonPressed, Vector3 position, Vector3 direction)
    {
        if (attackButtonPressed && !buttonDown)
        {
            var pukeObj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/CatProjectile"), position, Quaternion.identity);
            pukeObj.GetComponent<CatProjectileController>().Init(direction);
            buttonDown = true;
        }

        if (!attackButtonPressed)
            buttonDown = false;
    }
}
