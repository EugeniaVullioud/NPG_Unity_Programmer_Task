/// <summary>
/// Optional capability for abilities that need to participate in fixed simulation.
/// </summary>
public interface IFixedUpdateAbility
{
    void FixedUpdateAbility(float deltaTime);
}