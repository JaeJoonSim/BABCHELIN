using UnityEngine;

public class HealthPlayer : Health
{
    public delegate void HPUpdated(HealthPlayer Target);
    public delegate void TotalHPUpdated(HealthPlayer Target);

    public static bool ResetHealthData = true;

    public override float HP
    {
        get
        {
            return _HP;
        }
        set
        {
            Debug.Log($"HP CHANGE: {_HP} + {value}");
        }
    }
}
