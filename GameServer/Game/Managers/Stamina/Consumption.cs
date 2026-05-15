namespace NahidaImpact.GameServer.Game.Managers.Stamina;

public class Consumption
{
    public ConsumptionType Type = ConsumptionType.None;
    public int Amount = 0;

    public Consumption() { }

    public Consumption(ConsumptionType type)
    {
        Type = type;
        Amount = (int)type;
    }

    public Consumption(ConsumptionType type, int amount)
    {
        Type = type;
        Amount = amount;
    }
}
