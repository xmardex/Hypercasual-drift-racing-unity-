public interface IDamageDealer
{
    public float MinDamage { get; }
    public float MaxDamage { get; }

    public float MinDamageFactorValue { get; }
    public float MaxDamageFactorValue { get; }

    public float CalculateActualDamage(float damageFactor);
    public void SendDamageTo(float damageFactor, IHealth to);
    
}
