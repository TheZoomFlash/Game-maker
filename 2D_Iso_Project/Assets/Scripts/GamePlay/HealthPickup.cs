using UnityEngine;

public class HealthPickup : Gamekit2D.InteractOnTrigger2D
{
    public int healthAmount = 1;

    protected override void ExecuteOnEnter(Collider2D other)
    {
        if (PlayerController.PlayerInstance.GetHealth(healthAmount))
        {
            base.ExecuteOnEnter(other);
        }
    }
}