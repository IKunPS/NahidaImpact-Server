using Microsoft.VisualStudio.TestTools.UnitTesting;
using NahidaImpact.Database.Inventory;
using NahidaImpact.GameServer.Game.Inventory;

namespace NahidaImpact.GameServer.Tests.Inventory;

[TestClass]
public class ItemDataTests
{
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
        Assert.AreEqual(expectedPromote, ItemData.GetMinPromoteLevel(level));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(-100)]
    public void GetMinPromoteLevel_BelowLevel1_Returns0(int level)
    {
        Assert.AreEqual(0, ItemData.GetMinPromoteLevel(level));
    }

    [TestMethod]
    public void GetMinPromoteLevel_BoundaryValues()
    {
        // Level 20 boundary: still promote 0
        Assert.AreEqual(0, ItemData.GetMinPromoteLevel(20));
        // Level 21 boundary: promote 1
        Assert.AreEqual(1, ItemData.GetMinPromoteLevel(21));
        // Level 40 boundary
        Assert.AreEqual(1, ItemData.GetMinPromoteLevel(40));
        // Level 41 boundary
        Assert.AreEqual(2, ItemData.GetMinPromoteLevel(41));
        // Level 50 boundary
        Assert.AreEqual(2, ItemData.GetMinPromoteLevel(50));
        // Level 51 boundary
        Assert.AreEqual(3, ItemData.GetMinPromoteLevel(51));
        // Level 60 boundary
        Assert.AreEqual(3, ItemData.GetMinPromoteLevel(60));
        // Level 61 boundary
        Assert.AreEqual(4, ItemData.GetMinPromoteLevel(61));
        // Level 70 boundary
        Assert.AreEqual(4, ItemData.GetMinPromoteLevel(70));
        // Level 71 boundary
        Assert.AreEqual(5, ItemData.GetMinPromoteLevel(71));
        // Level 80 boundary
        Assert.AreEqual(5, ItemData.GetMinPromoteLevel(80));
        // Level 81 boundary
        Assert.AreEqual(6, ItemData.GetMinPromoteLevel(81));
        // Level 90 cap
        Assert.AreEqual(6, ItemData.GetMinPromoteLevel(90));
    }

    #endregion

    #region Default Properties

    [TestMethod]
    public void NewItemData_HasExpectedDefaults()
    {
        var item = new ItemData();

        Assert.AreEqual(1, item.Level);
        Assert.AreEqual(0, item.Count);
        Assert.AreEqual(0, item.Exp);
        Assert.AreEqual(0, item.TotalExp);
        Assert.AreEqual(0, item.PromoteLevel);
        Assert.IsFalse(item.Favourite);
        Assert.IsFalse(item.Locked);
        Assert.AreEqual(0, item.Affixes.Count);
        Assert.AreEqual(0, item.AppendPropIdList.Count);
        Assert.AreEqual(0, item.EquipCharacter);
        Assert.IsFalse(item.IsEquipped);
        Assert.IsTrue(item.IsDestroyable);
    }

    [TestMethod]
    public void LockedItem_IsNotDestroyable()
    {
        var item = new ItemData { Locked = true };
        Assert.IsFalse(item.IsDestroyable);
        Assert.IsTrue(item.IsLocked);
    }

    [TestMethod]
    public void EquippedItem_IsNotDestroyable()
    {
        var item = new ItemData { EquipCharacter = 123 };
        Assert.IsTrue(item.IsEquipped);
        Assert.IsFalse(item.IsDestroyable);
    }

    #endregion

    #region ToItemParamData

    [TestMethod]
    public void ToItemParamData_ReturnsCorrectValues()
    {
        var item = new ItemData { ItemId = 12345, Count = 99 };
        var param = item.ToItemParamData();

        Assert.AreEqual(12345, param.Id);
        Assert.AreEqual(99, param.Count);
    }

    #endregion
}

[TestClass]
public class InventoryManagerVirtualItemTests
{
    #region IsVirtualItem

    [TestMethod]
    [DataRow(101)]
    [DataRow(102)]
    [DataRow(105)]
    [DataRow(106)]
    [DataRow(107)]
    [DataRow(121)]
    [DataRow(201)]
    [DataRow(202)]
    [DataRow(203)]
    [DataRow(204)]
    public void IsVirtualItem_KnownVirtualIds_ReturnsTrue(int itemId)
    {
        Assert.IsTrue(InventoryManager.IsVirtualItem(itemId));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(100)]
    [DataRow(103)]
    [DataRow(104)]
    [DataRow(108)]
    [DataRow(200)]
    [DataRow(205)]
    [DataRow(99999)]
    public void IsVirtualItem_NonVirtualIds_ReturnsFalse(int itemId)
    {
        Assert.IsFalse(InventoryManager.IsVirtualItem(itemId));
    }

    #endregion
}
