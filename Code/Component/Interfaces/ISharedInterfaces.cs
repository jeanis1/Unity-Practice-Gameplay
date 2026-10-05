using UnityEngine;

namespace Code.Component
{
    // Shared interfaces for both Player and AI components

    public interface IHealthReceiver
    {
        void AddHealth(float amount);
    }

    public interface ISpeedReceiver
    {
        void BuffSpeed(float amount);
        void RestoreOriginalSpeed();
    }

    public interface IDamageable
    {
        void TakeDamage(Vector3 attackDirection, float damage);
    }

    public interface ISoundPlayer
    {
        void PlaySound(string soundName);
    }
    public interface IResetObject
    {
        void ResetToInitialize();
    }
}
