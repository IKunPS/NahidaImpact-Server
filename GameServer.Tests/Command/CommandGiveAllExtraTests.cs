using Microsoft.VisualStudio.TestTools.UnitTesting;
using NahidaImpact.Data.Common;
using NahidaImpact.Data.Excel;
using NahidaImpact.GameServer.Command.Commands;

namespace NahidaImpact.GameServer.Tests.Command;

[TestClass]
public class CommandGiveAllExtraTests
{
    #region ExtractUseParam

    [TestMethod]
    public void ExtractUseParam_FindsMatchingOp()
    {
        var data = new ItemDataExcel
        {
            ItemUseActions = new List<ItemUseActionData>
            {
                new() { UseOp = "ITEM_USE_GAIN_FLYCLOAK", UseParam = new List<string> { "140001" } }
            }
        };
        Assert.AreEqual(140001, CommandGive.ExtractUseParam(data, "ITEM_USE_GAIN_FLYCLOAK"));
    }

    [TestMethod]
    public void ExtractUseParam_ReturnsZero_WhenNotFound()
    {
        var data = new ItemDataExcel
        {
            ItemUseActions = new List<ItemUseActionData>
            {
                new() { UseOp = "ITEM_USE_GAIN_COSTUME", UseParam = new List<string> { "200301" } }
            }
        };
        Assert.AreEqual(0, CommandGive.ExtractUseParam(data, "ITEM_USE_GAIN_FLYCLOAK"));
    }

    [TestMethod]
    public void ExtractUseParam_NullActions_ReturnsZero()
    {
        var data = new ItemDataExcel { ItemUseActions = null! };
        Assert.AreEqual(0, CommandGive.ExtractUseParam(data, "ITEM_USE_GAIN_FLYCLOAK"));
    }

    [TestMethod]
    public void ExtractUseParam_EmptyUseParam_ReturnsZero()
    {
        var data = new ItemDataExcel
        {
            ItemUseActions = new List<ItemUseActionData>
            {
                new() { UseOp = "ITEM_USE_GAIN_FLYCLOAK", UseParam = new List<string>() }
            }
        };
        Assert.AreEqual(0, CommandGive.ExtractUseParam(data, "ITEM_USE_GAIN_FLYCLOAK"));
    }

    [TestMethod]
    public void ExtractUseParam_NonNumericParam_ReturnsZero()
    {
        var data = new ItemDataExcel
        {
            ItemUseActions = new List<ItemUseActionData>
            {
                new() { UseOp = "ITEM_USE_GAIN_FLYCLOAK", UseParam = new List<string> { "abc" } }
            }
        };
        Assert.AreEqual(0, CommandGive.ExtractUseParam(data, "ITEM_USE_GAIN_FLYCLOAK"));
    }

    #endregion

    #region IsExcludedMaterialType

    [TestMethod]
    public void IsExcludedMaterialType_FlycloakCostumeNamecard_NotExcluded()
    {
        Assert.IsFalse(CommandGive.IsExcludedMaterialType(Enums.Item.MaterialType.MATERIAL_FLYCLOAK));
        Assert.IsFalse(CommandGive.IsExcludedMaterialType(Enums.Item.MaterialType.MATERIAL_COSTUME));
        Assert.IsFalse(CommandGive.IsExcludedMaterialType(Enums.Item.MaterialType.MATERIAL_NAMECARD));
    }

    [TestMethod]
    public void IsExcludedMaterialType_Avatar_Excluded()
    {
        Assert.IsTrue(CommandGive.IsExcludedMaterialType(Enums.Item.MaterialType.MATERIAL_AVATAR));
    }

    #endregion
}

