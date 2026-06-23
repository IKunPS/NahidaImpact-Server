using Microsoft.VisualStudio.TestTools.UnitTesting;
using NahidaImpact.Data;
using NahidaImpact.Data.Excel;

namespace NahidaImpact.GameServer.Tests.Data;

[TestClass]
public class GameDataExcelTests
{
    #region IsFlycloakValid

    [TestMethod]
    public void IsFlycloakValid_WithLoadedData_ReturnsTrueForKnownIds()
    {
        ResourceManager.LoadGameData();
        Assert.IsTrue(GameData.IsFlycloakValid(140001)); // default glider
        Assert.IsTrue(GameData.IsFlycloakValid(140002)); // Starlit
        Assert.IsTrue(GameData.IsFlycloakValid(140010)); // Ayus
    }

    [TestMethod]
    public void IsFlycloakValid_InvalidIds_ReturnsFalse()
    {
        Assert.IsFalse(GameData.IsFlycloakValid(0));
        Assert.IsFalse(GameData.IsFlycloakValid(999999));
        Assert.IsFalse(GameData.IsFlycloakValid(-1));
    }

    [TestMethod]
    public void FlycloakData_DictionaryIsPopulated_AfterLoad()
    {
        ResourceManager.LoadGameData();
        Assert.IsTrue(GameData.FlycloakData.Count >= 18, $"Expected >= 18, got {GameData.FlycloakData.Count}");

        var defaultGlider = GameData.FlycloakData[140001];
        Assert.AreEqual(140001u, defaultGlider.FlycloakId);
        Assert.IsFalse(string.IsNullOrEmpty(defaultGlider.Icon));
        Assert.IsFalse(string.IsNullOrEmpty(defaultGlider.PrefabPath));
        Assert.IsFalse(defaultGlider.Hide);
    }

    #endregion

    #region IsCostumeValid

    [TestMethod]
    public void IsCostumeValid_WithLoadedData_ReturnsTrueForKnownIds()
    {
        ResourceManager.LoadGameData();
        Assert.IsTrue(GameData.IsCostumeValid(200301)); // Qipao
        Assert.IsTrue(GameData.IsCostumeValid(201401)); // Barbara summer
    }

    [TestMethod]
    public void IsCostumeValid_InvalidIds_ReturnsFalse()
    {
        Assert.IsFalse(GameData.IsCostumeValid(0));
        Assert.IsFalse(GameData.IsCostumeValid(999999));
        Assert.IsFalse(GameData.IsCostumeValid(-1));
    }

    [TestMethod]
    public void CostumeData_DictionaryIsPopulated_AfterLoad()
    {
        ResourceManager.LoadGameData();
        Assert.IsTrue(GameData.CostumeData.Count > 0, "CostumeData should have entries after load");
    }

    #endregion

    #region ConstValueData

    [TestMethod]
    public void ConstValueMap_HasExpectedDefaultKeys()
    {
        ResourceManager.LoadGameData();
        Assert.IsTrue(GameData.ConstValueMap.ContainsKey("CONST_VALUE_DEFAULT_FLYCLOAK_CONFIG"));
        Assert.IsTrue(GameData.ConstValueMap.ContainsKey("CONST_VALUE_FLY_COST_STAMINA"));
        Assert.IsTrue(GameData.ConstValueMap.ContainsKey("CONST_VALUE_DEFAULT_NAME_CARD_ID"));
    }

    [TestMethod]
    public void ConstValueData_CorrectType()
    {
        ResourceManager.LoadGameData();
        var data = GameData.ConstValueMap["CONST_VALUE_DEFAULT_FLYCLOAK_CONFIG"];
        Assert.IsInstanceOfType<ConstValueDataExcel>(data);
        Assert.AreEqual("CONST_VALUE_DEFAULT_FLYCLOAK_CONFIG", data.Name);
    }

    [TestMethod]
    public void ConstValue_GetUint_ReturnsExpectedDefaults()
    {
        ResourceManager.LoadGameData();
        Assert.AreEqual(140001u, ConstValue.GetUint("CONST_VALUE_DEFAULT_FLYCLOAK_CONFIG"));
        Assert.AreEqual(3u, ConstValue.GetUint("CONST_VALUE_FLY_COST_STAMINA"));
    }

    #endregion
}
