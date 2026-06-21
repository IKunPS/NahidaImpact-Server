using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NahidaImpact.Enums.Scene;

[JsonConverter(typeof(StringEnumConverter))]
public enum SceneTypeEnum
{
    SCENE_NONE = 0,
    SCENE_WORLD = 1,
    SCENE_DUNGEON = 2,
    SCENE_ROOM = 3,
    SCENE_HOME_WORLD = 4,
    SCENE_HOME_ROOM = 5,
    SCENE_ACTIVITY = 6,
    SCENE_HALL_WORLD = 7,
    SCENE_BEYOND_EDIT = 8,
    
}