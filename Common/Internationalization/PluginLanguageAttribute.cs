using NahidaImpact.Enums.Language;

namespace NahidaImpact.Internationalization;

[AttributeUsage(AttributeTargets.Class)]
public class PluginLanguageAttribute(ProgramLanguageTypeEnum languageType) : Attribute
{
    public ProgramLanguageTypeEnum LanguageType { get; } = languageType;
}