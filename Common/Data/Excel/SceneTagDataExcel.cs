using Newtonsoft.Json;
using System.Collections.Generic;

namespace NahidaImpact.Data.Excel;

[ResourceEntity("SceneTagConfigData.json")]
public class SceneTagDataExcel : ExcelResource
{
    [JsonProperty("id")]
    public uint Id { get; set; }

    [JsonProperty("sceneId")]
    public int SceneId { get; set; }

    [JsonProperty("isDefaultValid")]
    public bool IsDefaultValid { get; set; }

    [JsonProperty("sceneTagName")]
    public string SceneTagName { get; set; } = string.Empty;

    [JsonProperty("cond")]
    public List<SceneTagCondition> Cond { get; set; } = [];

    public override uint GetId() => Id;

    public override void Loaded()
    {
        if (Cond != null)
            Cond.RemoveAll(c => c == null);
        else
            Cond = [];

        GameData.SceneTagData[(int)Id] = this;
    }
}

public class SceneTagCondition
{
    [JsonProperty("condType")]
    public SceneTagCondType CondType { get; set; }

    [JsonProperty("param1")]
    public int Param1 { get; set; }

    [JsonProperty("param2")]
    public int Param2 { get; set; }
}

public enum SceneTagCondType
{
    SCENE_TAG_COND_TYPE_NONE = 0,
    SCENE_TAG_COND_TYPE_ACTIVITY_CONTENT_OPEN = 1,
    SCENE_TAG_COND_TYPE_QUEST_FINISH = 2,
    SCENE_TAG_COND_TYPE_QUEST_GLOBAL_VAR_EQUAL = 3,
    SCENE_TAG_COND_TYPE_SPECIFIC_ACTIVITY_OPEN = 4
}