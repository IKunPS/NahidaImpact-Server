using Microsoft.VisualStudio.TestTools.UnitTesting;
using NahidaImpact.GameServer.Game.Inventory;
using NahidaImpact.GameServer.Game.Avatar;

namespace NahidaImpact.GameServer.Tests.Inventory;

[TestClass]
public class ExpCalculationTests
{
    #region GetRelicExpRequired

    [TestMethod]
    public void GetRelicExpRequired_FormulaIsLinear()
    {
        // Formula: level * 100 + rankLevel * 50
        Assert.AreEqual(150, InventoryManager.GetRelicExpRequired(1, 1));   // 1*100 + 1*50
        Assert.AreEqual(250, InventoryManager.GetRelicExpRequired(1, 2));   // 2*100 + 1*50
        Assert.AreEqual(300, InventoryManager.GetRelicExpRequired(2, 2));   // 2*100 + 2*50
        Assert.AreEqual(1050, InventoryManager.GetRelicExpRequired(1, 10)); // 10*100 + 1*50
        Assert.AreEqual(2250, InventoryManager.GetRelicExpRequired(5, 20)); // 20*100 + 5*50
    }

    [TestMethod]
    public void GetRelicExpRequired_ZeroValues()
    {
        Assert.AreEqual(0, InventoryManager.GetRelicExpRequired(0, 0));
        Assert.AreEqual(50, InventoryManager.GetRelicExpRequired(1, 0));
        Assert.AreEqual(0, InventoryManager.GetRelicExpRequired(0, 0));
    }

    [TestMethod]
    public void GetRelicExpRequired_HighRankIncreasesCost()
    {
        // Same level, higher rank = more EXP needed
        int rank1Cost = InventoryManager.GetRelicExpRequired(1, 5);
        int rank3Cost = InventoryManager.GetRelicExpRequired(3, 5);
        int rank5Cost = InventoryManager.GetRelicExpRequired(5, 5);

        Assert.IsTrue(rank3Cost > rank1Cost);
        Assert.IsTrue(rank5Cost > rank3Cost);
        // Difference is exactly (rankDiff * 50)
        Assert.AreEqual(100, rank3Cost - rank1Cost);
        Assert.AreEqual(200, rank5Cost - rank1Cost);
    }

    [TestMethod]
    public void GetRelicExpRequired_LevelIncreasesCostLinearly()
    {
        // Same rank, the cost difference between adjacent levels is always 100
        for (int level = 1; level < 20; level++)
        {
            int current = InventoryManager.GetRelicExpRequired(3, level);
            int next = InventoryManager.GetRelicExpRequired(3, level + 1);
            Assert.AreEqual(100, next - current, $"Level {level} → {level + 1} should differ by 100");
        }
    }

    #endregion

    #region GetAvatarLevelExpRequired

    [TestMethod]
    public void GetAvatarLevelExpRequired_HasFallbackFormula()
    {
        // Without GameData loaded, falls back to level * 100
        Assert.AreEqual(100, AvatarManager.GetAvatarLevelExpRequired(1));
        Assert.AreEqual(500, AvatarManager.GetAvatarLevelExpRequired(5));
        Assert.AreEqual(9000, AvatarManager.GetAvatarLevelExpRequired(90));
    }

    [TestMethod]
    public void GetAvatarLevelExpRequired_LevelZero_Returns0()
    {
        Assert.AreEqual(0, AvatarManager.GetAvatarLevelExpRequired(0));
    }

    #endregion
}
