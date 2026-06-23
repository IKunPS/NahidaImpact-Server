using Microsoft.VisualStudio.TestTools.UnitTesting;
using NahidaImpact.Data;
using NahidaImpact.GameServer.Game.Player;
using NahidaImpact.Prop;

namespace NahidaImpact.GameServer.Tests.Player;

[TestClass]
public class PlayerInstanceStaminaTests
{
    [ClassInitialize]
    public static void ClassInitialize(TestContext ctx) => ResourceManager.LoadGameData();

    #region Stamina

    [TestMethod]
    public void CurrentStamina_DefaultValue()
    {
        var player = new PlayerInstance(99991);
        Assert.AreEqual(24000, player.MaxStamina);
        Assert.AreEqual(24000, player.CurrentStamina);
    }

    [TestMethod]
    public void CurrentStamina_SetterClampsAtZero()
    {
        var player = new PlayerInstance(99991);
        player.CurrentStamina = -500;
        Assert.AreEqual(0, player.CurrentStamina);
    }

    [TestMethod]
    public void ConsumeStamina_EnoughStamina_ReturnsTrue()
    {
        var player = new PlayerInstance(99991);
        player.CurrentStamina = 1000;
        Assert.IsTrue(player.ConsumeStamina(300));
        Assert.AreEqual(700, player.CurrentStamina);
    }

    [TestMethod]
    public void ConsumeStamina_ExactAmount_ReturnsTrue()
    {
        var player = new PlayerInstance(99991);
        player.CurrentStamina = 300;
        Assert.IsTrue(player.ConsumeStamina(300));
        Assert.AreEqual(0, player.CurrentStamina);
    }

    [TestMethod]
    public void ConsumeStamina_NotEnough_ReturnsFalse()
    {
        var player = new PlayerInstance(99991);
        player.CurrentStamina = 100;
        Assert.IsFalse(player.ConsumeStamina(300));
        Assert.AreEqual(100, player.CurrentStamina); // unchanged
    }

    #endregion

    #region Dive Stamina

    [TestMethod]
    public void DiveCurrentStamina_SetterClampsAtZero()
    {
        var player = new PlayerInstance(99991);
        player.DiveCurrentStamina = -100;
        Assert.AreEqual(0, player.DiveCurrentStamina);
    }

    #endregion

    #region IsFlyable

    [TestMethod]
    public void IsFlyable_DefaultsTrue()
    {
        var player = new PlayerInstance(99991);
        Assert.IsTrue(player.IsFlyable);
    }

    [TestMethod]
    public void IsFlyable_CanBeToggled()
    {
        var player = new PlayerInstance(99991);
        player.IsFlyable = false;
        Assert.IsFalse(player.IsFlyable);
        player.IsFlyable = true;
        Assert.IsTrue(player.IsFlyable);
    }

    #endregion

    #region ApplyCosmeticDefaults

    [TestMethod]
    public void ApplyCosmeticDefaults_NewPlayer_HasDefaultFlycloak()
    {
        var player = new PlayerInstance(99991);
        // On construction, defaults should be applied
        Assert.IsTrue(player.FlyCloakList.Count > 0);
        Assert.IsTrue(player.FlyCloakList.Exists(i => i > 0));
    }

    [TestMethod]
    public void ApplyCosmeticDefaults_NewPlayer_HasDefaultNameCard()
    {
        var player = new PlayerInstance(99991);
        Assert.IsTrue(player.NameCardList.Count > 0);
    }

    [TestMethod]
    public void ApplyCosmeticDefaults_ListsNotNull()
    {
        var player = new PlayerInstance(99991);
        Assert.IsNotNull(player.FlyCloakList);
        Assert.IsNotNull(player.NameCardList);
        Assert.IsNotNull(player.CostumeList);
        Assert.IsNotNull(player.TraceEffectList);
        Assert.IsNotNull(player.ChatEmojiIdList);
    }

    #endregion

    #region PlayerProperties

    [TestMethod]
    public void GetProperty_UnsetKey_ReturnsZero()
    {
        var player = new PlayerInstance(99991);
        Assert.AreEqual(0, player.GetProperty(99999));
    }

    [TestMethod]
    public void SetProperty_StoresCorrectly()
    {
        var player = new PlayerInstance(99991);
        player.SetProperty(PlayerProp.PROP_MAX_STAMINA, 30000);
        Assert.AreEqual(30000, player.MaxStamina);
    }

    #endregion
}
