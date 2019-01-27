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
    static public IAttackLauncher Create(AttackKind kind, IPlayerAudioManager playerAudioManager)
    {
        switch (kind) 
        {
            case AttackKind.CatProjectile:
                return new CatProjectileLauncher(playerAudioManager);
        }

        return null;
    }
}

public class CatProjectileLauncher : IAttackLauncher
{
    bool buttonDown = false;
    IPlayerAudioManager AudioManager;

    public CatProjectileLauncher(IPlayerAudioManager playerAudioManager)
    {
        AudioManager = playerAudioManager;
    }

    public void Update(bool attackButtonPressed, Vector3 position, Vector3 direction)
    {
        if (attackButtonPressed && !buttonDown)
        {
            var pukeObj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/CatProjectile"), position, Quaternion.identity);
            pukeObj.GetComponent<CatProjectileController>().Init(direction);
            AudioManager.PlayProjectileAudio();
            buttonDown = true;
        }

        if (!attackButtonPressed)
            buttonDown = false;
    }
}
