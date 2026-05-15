namespace NahidaImpact.Data.Binout;

/// <summary>
/// Server global values (SGV) for entities. Mirrors Java ConfigGlobalValue.
/// </summary>
public class ConfigGlobalValue
{
    public List<string>? ServerGlobalValues { get; set; }
    public Dictionary<string, float>? InitServerGlobalValues { get; set; }
}
