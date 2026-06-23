using Microsoft.VisualStudio.TestTools.UnitTesting;
using NahidaImpact.Enums.Item;
using NahidaImpact.GameServer.Command.Commands;

namespace NahidaImpact.GameServer.Tests.Command;

[TestClass]
public class CommandGiveTests
{
    #region IsExcludedAvatar

    [TestMethod]
    [DataRow(10000000)] // below min
    [DataRow(9999999)]
    [DataRow(1)]
    [DataRow(0)]
    [DataRow(-1)]
    public void IsExcludedAvatar_IdBelowRange_ReturnsTrue(int id)
    {
        Assert.IsTrue(CommandGive.IsExcludedAvatar(id));
    }

    [TestMethod]
    [DataRow(11000000)] // above max
    [DataRow(11000001)]
    [DataRow(12000000)]
    [DataRow(99999999)]
    public void IsExcludedAvatar_IdAboveRange_ReturnsTrue(int id)
    {
        Assert.IsTrue(CommandGive.IsExcludedAvatar(id));
    }

    [TestMethod]
    [DataRow(10000900)] // start of test avatar range
    [DataRow(10000901)]
    [DataRow(10000905)]
    [DataRow(10000910)] // end of test avatar range
    public void IsExcludedAvatar_TestAvatars_ReturnsTrue(int id)
    {
        Assert.IsTrue(CommandGive.IsExcludedAvatar(id));
    }

    [TestMethod]
    [DataRow(10000002)] // first valid
    [DataRow(10000003)]
    [DataRow(10000050)]
    [DataRow(10000899)] // just before test range
    [DataRow(10000911)] // just after test range
    [DataRow(10500000)]
    [DataRow(10999999)] // last valid
    public void IsExcludedAvatar_ValidIds_ReturnsFalse(int id)
    {
        Assert.IsFalse(CommandGive.IsExcludedAvatar(id));
    }

    #endregion

    #region IsValidWeaponId

    [TestMethod]
    [DataRow(10000)] // min valid
    [DataRow(10001)]
    [DataRow(13000)]
    [DataRow(15000)]
    [DataRow(19999)] // max valid
    public void IsValidWeaponId_ValidIds_ReturnsTrue(int id)
    {
        Assert.IsTrue(CommandGive.IsValidWeaponId(id));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(9999)]  // below min
    [DataRow(20000)] // above max
    [DataRow(99999)]
    public void IsValidWeaponId_InvalidIds_ReturnsFalse(int id)
    {
        Assert.IsFalse(CommandGive.IsValidWeaponId(id));
    }

    #endregion

    #region IsValidRelicId

    [TestMethod]
    [DataRow(20002)] // min valid
    [DataRow(20003)]
    [DataRow(50000)]
    [DataRow(99998)]
    [DataRow(99999)] // max valid
    public void IsValidRelicId_ValidIds_ReturnsTrue(int id)
    {
        Assert.IsTrue(CommandGive.IsValidRelicId(id));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(20000)] // below min
    [DataRow(20001)] // below min
    [DataRow(100000)] // above max
    [DataRow(999999)]
    public void IsValidRelicId_InvalidIds_ReturnsFalse(int id)
    {
        Assert.IsFalse(CommandGive.IsValidRelicId(id));
    }

    #endregion

    #region ParseTaggedArgs

    [TestMethod]
    public void ParseTaggedArgs_AllTagged_NoPositionalRemaining()
    {
        var args = new List<string> { "lv90", "x5", "r3", "c6", "sl10" };
        var param = new CommandGive.GiveParams();

        var positional = CommandGive.ParseTaggedArgs(args, param);

        Assert.AreEqual(0, positional.Count);
        Assert.AreEqual(90, param.Level);
        Assert.AreEqual(5, param.Amount);
        Assert.AreEqual(3, param.Refinement);
        Assert.AreEqual(6, param.Constellation);
        Assert.AreEqual(10, param.SkillLevel);
    }

    [TestMethod]
    public void ParseTaggedArgs_MixedPositionalAndTagged()
    {
        var args = new List<string> { "all", "lv80", "x10" };
        var param = new CommandGive.GiveParams();

        var positional = CommandGive.ParseTaggedArgs(args, param);

        Assert.AreEqual(1, positional.Count);
        Assert.AreEqual("all", positional[0]);
        Assert.AreEqual(80, param.Level);
        Assert.AreEqual(10, param.Amount);
    }

    [TestMethod]
    public void ParseTaggedArgs_OnlyPositional_NoTagsSet()
    {
        var args = new List<string> { "weapons", "mats", "avatars" };
        var param = new CommandGive.GiveParams();

        var positional = CommandGive.ParseTaggedArgs(args, param);

        Assert.AreEqual(3, positional.Count);
        Assert.AreEqual("weapons", positional[0]);
        Assert.AreEqual("mats", positional[1]);
        Assert.AreEqual("avatars", positional[2]);
        // Defaults remain untouched
        Assert.AreEqual(1, param.Level);
        Assert.AreEqual(1, param.Amount);
        Assert.AreEqual(1, param.Refinement);
    }

    [TestMethod]
    public void ParseTaggedArgs_CaseInsensitive()
    {
        var args = new List<string> { "LV50", "X99", "R5", "C6", "SL10" };
        var param = new CommandGive.GiveParams();

        CommandGive.ParseTaggedArgs(args, param);

        Assert.AreEqual(50, param.Level);
        Assert.AreEqual(99, param.Amount);
        Assert.AreEqual(5, param.Refinement);
        Assert.AreEqual(6, param.Constellation);
        Assert.AreEqual(10, param.SkillLevel);
    }

    [TestMethod]
    public void ParseTaggedArgs_PartialTags()
    {
        var args = new List<string> { "lv70", "c3" };
        var param = new CommandGive.GiveParams();

        var positional = CommandGive.ParseTaggedArgs(args, param);

        Assert.AreEqual(0, positional.Count);
        Assert.AreEqual(70, param.Level);
        Assert.AreEqual(3, param.Constellation);
        // Untagged stay at defaults
        Assert.AreEqual(1, param.Amount);
        Assert.AreEqual(1, param.Refinement);
        Assert.AreEqual(1, param.SkillLevel);
    }

    [TestMethod]
    public void ParseTaggedArgs_InvalidTagFormats_Ignored()
    {
        var args = new List<string> { "lvabc", "x", "r", "cxyz", "sl" };
        var param = new CommandGive.GiveParams();

        var positional = CommandGive.ParseTaggedArgs(args, param);

        // Invalid tagged args become positional
        Assert.AreEqual(5, positional.Count);
        Assert.AreEqual(1, param.Level); // default
    }

    [TestMethod]
    public void ParseTaggedArgs_EmptyList_ReturnsEmpty()
    {
        var args = new List<string>();
        var param = new CommandGive.GiveParams();

        var positional = CommandGive.ParseTaggedArgs(args, param);

        Assert.AreEqual(0, positional.Count);
    }

    #endregion

    #region ParseRelicArgs

    [TestMethod]
    public void ParseRelicArgs_SingleMainProp()
    {
        var param = new CommandGive.GiveParams();
        var args = new List<string> { "15001" };

        CommandGive.ParseRelicArgs(param, args);

        Assert.AreEqual(15001, param.MainPropId);
        Assert.IsNull(param.AppendPropIdList);
    }

    [TestMethod]
    public void ParseRelicArgs_MainPropAndSingleSubstat()
    {
        var param = new CommandGive.GiveParams();
        var args = new List<string> { "15001", "501" };

        CommandGive.ParseRelicArgs(param, args);

        Assert.AreEqual(15001, param.MainPropId);
        Assert.IsNotNull(param.AppendPropIdList);
        Assert.AreEqual(1, param.AppendPropIdList!.Count);
        Assert.AreEqual(501, param.AppendPropIdList![0]);
    }

    [TestMethod]
    public void ParseRelicArgs_MainPropAndRepeatedSubstat()
    {
        var param = new CommandGive.GiveParams();
        var args = new List<string> { "15001", "501,5" };

        CommandGive.ParseRelicArgs(param, args);

        Assert.AreEqual(15001, param.MainPropId);
        Assert.IsNotNull(param.AppendPropIdList);
        Assert.AreEqual(5, param.AppendPropIdList!.Count);
        foreach (var id in param.AppendPropIdList)
            Assert.AreEqual(501, id);
    }

    [TestMethod]
    public void ParseRelicArgs_MultipleSubstats()
    {
        var param = new CommandGive.GiveParams();
        var args = new List<string> { "15001", "501,2", "502,3", "503" };

        CommandGive.ParseRelicArgs(param, args);

        Assert.AreEqual(15001, param.MainPropId);
        Assert.IsNotNull(param.AppendPropIdList);
        Assert.AreEqual(6, param.AppendPropIdList!.Count);
        // 2x 501, 3x 502, 1x 503
        Assert.AreEqual(501, param.AppendPropIdList[0]);
        Assert.AreEqual(501, param.AppendPropIdList[1]);
        Assert.AreEqual(502, param.AppendPropIdList[2]);
        Assert.AreEqual(502, param.AppendPropIdList[3]);
        Assert.AreEqual(502, param.AppendPropIdList[4]);
        Assert.AreEqual(503, param.AppendPropIdList[5]);
    }

    [TestMethod]
    public void ParseRelicArgs_CountClampedToMax200()
    {
        var param = new CommandGive.GiveParams();
        var args = new List<string> { "15001", "501,999" };

        CommandGive.ParseRelicArgs(param, args);

        Assert.IsNotNull(param.AppendPropIdList);
        Assert.AreEqual(200, param.AppendPropIdList!.Count);
    }

    [TestMethod]
    public void ParseRelicArgs_CountClampedToMin1()
    {
        var param = new CommandGive.GiveParams();
        var args = new List<string> { "15001", "501,0" };

        CommandGive.ParseRelicArgs(param, args);

        Assert.IsNotNull(param.AppendPropIdList);
        Assert.AreEqual(1, param.AppendPropIdList!.Count);
    }

    [TestMethod]
    public void ParseRelicArgs_InvalidMainProp_StillParsesSubstats()
    {
        var param = new CommandGive.GiveParams();
        var args = new List<string> { "notanumber", "501" };

        CommandGive.ParseRelicArgs(param, args);

        // MainPropId retains default value since parse fails
        Assert.AreEqual(-1, param.MainPropId);
        // Substat still parsed
        Assert.IsNotNull(param.AppendPropIdList);
    }

    [TestMethod]
    public void ParseRelicArgs_EmptyArgs_NoChange()
    {
        var param = new CommandGive.GiveParams();
        var args = new List<string>();

        CommandGive.ParseRelicArgs(param, args);

        Assert.AreEqual(-1, param.MainPropId);
        Assert.IsNull(param.AppendPropIdList);
    }

    [TestMethod]
    public void ParseRelicArgs_InvalidSubstatId_Skipped()
    {
        var param = new CommandGive.GiveParams();
        var args = new List<string> { "15001", "notanumber" };

        CommandGive.ParseRelicArgs(param, args);

        Assert.IsNotNull(param.AppendPropIdList);
        Assert.AreEqual(0, param.AppendPropIdList!.Count);
    }

    #endregion

    #region IsExcludedMaterialType

    [TestMethod]
    public void IsExcludedMaterialType_OnlyAvatar_ReturnsTrue()
    {
        Assert.IsTrue(CommandGive.IsExcludedMaterialType(MaterialType.MATERIAL_AVATAR));
    }

    [TestMethod]
    public void IsExcludedMaterialType_EverythingElse_ReturnsFalse()
    {
        Assert.IsFalse(CommandGive.IsExcludedMaterialType(MaterialType.MATERIAL_FLYCLOAK));
        Assert.IsFalse(CommandGive.IsExcludedMaterialType(MaterialType.MATERIAL_COSTUME));
        Assert.IsFalse(CommandGive.IsExcludedMaterialType(MaterialType.MATERIAL_NAMECARD));
        Assert.IsFalse(CommandGive.IsExcludedMaterialType(MaterialType.MATERIAL_FOOD));
        Assert.IsFalse(CommandGive.IsExcludedMaterialType(MaterialType.MATERIAL_WIDGET));
        Assert.IsFalse(CommandGive.IsExcludedMaterialType(MaterialType.MATERIAL_CONSUME));
        Assert.IsFalse(CommandGive.IsExcludedMaterialType(MaterialType.MATERIAL_TALENT));
        Assert.IsFalse(CommandGive.IsExcludedMaterialType(MaterialType.MATERIAL_NONE));
    }

    #endregion
}
