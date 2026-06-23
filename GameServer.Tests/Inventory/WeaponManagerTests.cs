using Microsoft.VisualStudio.TestTools.UnitTesting;
using NahidaImpact.GameServer.Game.Inventory;

namespace NahidaImpact.GameServer.Tests.Inventory;

[TestClass]
public class WeaponManagerTests
{
    #region GetMaxLevelByPromote

    [TestMethod]
    [DataRow(0, 20)]
    [DataRow(1, 40)]
    [DataRow(2, 50)]
    [DataRow(3, 60)]
    [DataRow(4, 70)]
    [DataRow(5, 80)]
    [DataRow(6, 90)]
    public void GetMaxLevelByPromote_ReturnsCorrectMaxLevel(int promoteLevel, int expectedMax)
    {
        Assert.AreEqual(expectedMax, WeaponManager.GetMaxLevelByPromote(promoteLevel));
    }

    [TestMethod]
    [DataRow(7)]
    [DataRow(8)]
    [DataRow(99)]
    [DataRow(-1)]
    public void GetMaxLevelByPromote_BeyondMax_Returns90(int promoteLevel)
    {
        Assert.AreEqual(90, WeaponManager.GetMaxLevelByPromote(promoteLevel));
    }

    #endregion

    #region GetMinPromoteLevel

    [TestMethod]
    [DataRow(1, 0)]
    [DataRow(20, 0)]
    [DataRow(21, 1)]
    [DataRow(40, 1)]
    [DataRow(41, 2)]
    [DataRow(50, 2)]
    [DataRow(51, 3)]
    [DataRow(60, 3)]
    [DataRow(61, 4)]
    [DataRow(70, 4)]
    [DataRow(71, 5)]
    [DataRow(80, 5)]
    [DataRow(81, 6)]
    [DataRow(90, 6)]
    public void GetMinPromoteLevel_ReturnsCorrectPromoteLevel(int level, int expectedPromote)
    {
        Assert.AreEqual(expectedPromote, WeaponManager.GetMinPromoteLevel(level));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(-100)]
    public void GetMinPromoteLevel_BelowLevel1_Returns0(int level)
    {
        Assert.AreEqual(0, WeaponManager.GetMinPromoteLevel(level));
    }

    #endregion

    #region GetExpRequired

    [TestMethod]
    public void GetExpRequired_HasFallbackForUnknownLevels()
    {
        // Without GameData loaded, all levels fall back to level * 100
        Assert.AreEqual(100, WeaponManager.GetExpRequired(1));
        Assert.AreEqual(1000, WeaponManager.GetExpRequired(10));
        Assert.AreEqual(2000, WeaponManager.GetExpRequired(20));
        Assert.AreEqual(9000, WeaponManager.GetExpRequired(90));
    }

    [TestMethod]
    public void GetExpRequired_ZeroLevel_Returns0()
    {
        Assert.AreEqual(0, WeaponManager.GetExpRequired(0));
    }

    #endregion
}
