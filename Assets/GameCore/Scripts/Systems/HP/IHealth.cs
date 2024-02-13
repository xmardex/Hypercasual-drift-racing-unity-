public interface IHealth
{
    public float MaxHP { get; }
    public float CurrentHP { get; }

    public void ReciveDamageFrom(float damageFactor, IDamageDealer from);
    public float CalculateActualDamage(float damageFactor);
    public void Damage(float damage);
    public void Heal(float heal);
}
