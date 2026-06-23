namespace NahidaImpact.GameServer.Game.Ability;

// hk4e PileValue — tracks per-source contributions and computes aggregate via stack method.
// Used by modifier properties: duration, element durability, shield HP, etc.
public class PileValue
{
    private float _value;
    private float _floor;
    private float _ceiling;
    private float _limitMin;
    private float _limitMax;
    private StackMethod _stackMethod;
    private readonly Dictionary<uint, (float value, bool active)> _valueMap = [];

    public enum StackMethod { Sum, Max, Min, Average, Override }

    public float Value
    {
        get => Math.Clamp(_value, _limitMin, _limitMax);
        set => _value = Math.Clamp(value, _floor, _ceiling);
    }

    public PileValue(float defaultValue = 0f, StackMethod stackMethod = StackMethod.Sum,
        float floor = float.MinValue, float ceiling = float.MaxValue,
        float limitMin = float.MinValue, float limitMax = float.MaxValue)
    {
        _value = defaultValue;
        _floor = floor;
        _ceiling = ceiling;
        _limitMin = limitMin;
        _limitMax = limitMax;
        _stackMethod = stackMethod;
    }

    public void SetSource(uint sourceId, float value, bool active = true)
    {
        _valueMap[sourceId] = (value, active);
        Recompute();
    }

    public void RemoveSource(uint sourceId)
    {
        _valueMap.Remove(sourceId);
        Recompute();
    }

    public void SetSourceActive(uint sourceId, bool active)
    {
        if (_valueMap.TryGetValue(sourceId, out var entry))
        {
            _valueMap[sourceId] = (entry.value, active);
            Recompute();
        }
    }

    private void Recompute()
    {
        var active = _valueMap.Values.Where(v => v.active).Select(v => v.value).ToList();
        if (active.Count == 0)
        {
            _value = 0f;
            return;
        }
        _value = _stackMethod switch
        {
            StackMethod.Sum => active.Sum(),
            StackMethod.Max => active.Max(),
            StackMethod.Min => active.Min(),
            StackMethod.Average => active.Average(),
            StackMethod.Override => active.Last(),
            _ => active.Sum()
        };
        _value = Math.Clamp(_value, _floor, _ceiling);
    }

    public void SetStackMethod(StackMethod method) { _stackMethod = method; Recompute(); }
    public void SetFloor(float floor) { _floor = floor; }
    public void SetCeiling(float ceiling) { _ceiling = ceiling; }
    public void SetLimits(float min, float max) { _limitMin = min; _limitMax = max; }
}

// hk4e PileBoolValue — boolean stacking (e.g. "is invincible" from multiple sources)
public class PileBoolValue
{
    private bool _value;
    private BoolStackMethod _stackMethod;
    private readonly Dictionary<uint, bool> _valueMap = [];

    public enum BoolStackMethod { OnceTrue, OnceFalse, Override }

    public bool Value => _value;

    public PileBoolValue(BoolStackMethod stackMethod = BoolStackMethod.OnceTrue)
    {
        _stackMethod = stackMethod;
    }

    public void SetSource(uint sourceId, bool value)
    {
        _valueMap[sourceId] = value;
        Recompute();
    }

    public void RemoveSource(uint sourceId)
    {
        _valueMap.Remove(sourceId);
        Recompute();
    }

    private void Recompute()
    {
        if (_valueMap.Count == 0) { _value = false; return; }
        _value = _stackMethod switch
        {
            BoolStackMethod.OnceTrue => _valueMap.Values.Any(v => v),
            BoolStackMethod.OnceFalse => _valueMap.Values.All(v => v),
            BoolStackMethod.Override => _valueMap.Values.Last(),
            _ => _valueMap.Values.Any(v => v)
        };
    }
}
