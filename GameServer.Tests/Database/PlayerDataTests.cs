using Microsoft.VisualStudio.TestTools.UnitTesting;
using NahidaImpact.Database;
using NahidaImpact.Database.Player;

namespace NahidaImpact.GameServer.Tests.Database;

[TestClass]
public class PlayerDataTests
{
    #region Fields

    [TestMethod]
    public void NewPlayerData_HasEmptyCosmeticLists()
    {
        var data = new PlayerData { Uid = 99999 };
        Assert.IsNotNull(data.FlyCloakList);
        Assert.IsNotNull(data.NameCardList);
        Assert.IsNotNull(data.CostumeList);
        Assert.IsNotNull(data.TraceEffectList);
        Assert.IsNotNull(data.ChatEmojiIdList);
        Assert.AreEqual(0, data.FlyCloakList.Count);
        Assert.AreEqual(0, data.NameCardList.Count);
        Assert.AreEqual(0, data.CostumeList.Count);
    }

    [TestMethod]
    public void PlayerData_SaveAndLoad_PreservesFlycloaks()
    {
        var uid = 99998;
        try
        {
            // Create with flycloaks
            var original = PlayerData.GetOrCreatePlayerData(uid);
            original.FlyCloakList = new List<int> { 140001, 140005, 140010 };
            original.NameCardList = new List<int> { 210001 };
            PlayerData.SavePlayerData(original);

            // Reload and verify
            var loaded = PlayerData.GetPlayerByUid(uid);
            Assert.IsNotNull(loaded);
            Assert.AreEqual(3, loaded!.FlyCloakList.Count);
            Assert.AreEqual(140001, loaded.FlyCloakList[0]);
            Assert.AreEqual(140005, loaded.FlyCloakList[1]);
            Assert.AreEqual(140010, loaded.FlyCloakList[2]);
            Assert.AreEqual(1, loaded.NameCardList.Count);
        }
        finally
        {
            DatabaseHelper.DeleteInstance<PlayerData>(uid);
        }
    }

    [TestMethod]
    public void PlayerData_EmptyList_PersistsAsEmpty()
    {
        var uid = 99997;
        try
        {
            var data = PlayerData.GetOrCreatePlayerData(uid);
            data.FlyCloakList = new List<int>();
            data.NameCardList = new List<int>();
            data.CostumeList = new List<int>();
            PlayerData.SavePlayerData(data);

            var loaded = PlayerData.GetPlayerByUid(uid);
            Assert.IsNotNull(loaded);
            Assert.AreEqual(0, loaded!.FlyCloakList.Count);
            Assert.AreEqual(0, loaded.NameCardList.Count);
            Assert.AreEqual(0, loaded.CostumeList.Count);
        }
        finally
        {
            DatabaseHelper.DeleteInstance<PlayerData>(uid);
        }
    }

    [TestMethod]
    public void PlayerData_CostumeList_PersistsCorrectly()
    {
        var uid = 99996;
        try
        {
            var data = PlayerData.GetOrCreatePlayerData(uid);
            data.CostumeList = new List<int> { 200301, 201401 };
            data.FlyCloakList = new List<int> { 140001 };
            PlayerData.SavePlayerData(data);

            var loaded = PlayerData.GetPlayerByUid(uid);
            Assert.IsNotNull(loaded);
            Assert.AreEqual(2, loaded!.CostumeList.Count);
            Assert.AreEqual(1, loaded.FlyCloakList.Count);
        }
        finally
        {
            DatabaseHelper.DeleteInstance<PlayerData>(uid);
        }
    }

    #endregion
}
