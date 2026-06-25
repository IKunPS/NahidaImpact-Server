﻿using NahidaImpact.Database.Account;
using NahidaImpact.Internationalization;
using NahidaImpact.Util;
using SqlSugar;
using System.Collections.Concurrent;
using System.Globalization;

namespace NahidaImpact.Database;

public class DatabaseHelper
{
    public static Logger logger = new("Database");
    public static SqlSugarScope? sqlSugarScope;
    public static readonly ConcurrentDictionary<int, List<BaseDatabaseDataHelper>> UidInstanceMap = [];
    private static readonly object _saveLock = new();
    private static readonly List<int> _toSaveUidList = [];
    public static long LastSaveTick = DateTime.UtcNow.Ticks;
    public static Thread? SaveThread;
    public static readonly CancellationTokenSource SaveCts = new();
    public static readonly ManualResetEventSlim AccountLoadedEvent = new(false);
    public static readonly ManualResetEventSlim AllDataLoadedEvent = new(false);
    public static bool LoadAccount;
    public static bool LoadAllData;

    public void Initialize()
    {
        logger.Info(I18NManager.Translate("Server.ServerInfo.LoadingItem", I18NManager.Translate("Word.Database")));
        var f = new FileInfo(ConfigManager.Config.Path.DatabasePath + "/" + ConfigManager.Config.GameServer.DatabaseName);
        if (!f.Exists && f.Directory != null) f.Directory.Create();

        sqlSugarScope = new SqlSugarScope(new ConnectionConfig
        {
            ConnectionString = $"Data Source={f.FullName};",
            DbType = DbType.Sqlite,
            IsAutoCloseConnection = true,
            ConfigureExternalServices = new ConfigureExternalServices
            {
                SerializeService = new CustomSerializeService()
            }
        });

        InitializeSqlite();

        var baseType = typeof(BaseDatabaseDataHelper);
        var assembly = typeof(BaseDatabaseDataHelper).Assembly;
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));

        var list = sqlSugarScope.Queryable<AccountData>().ToList();
        foreach (var inst in list)
        {
            var value = UidInstanceMap.GetOrAdd(inst.Uid, _ => []);
            lock (value)
            {
                value.Add(inst);
            }
        }

        // start dispatch server
        LoadAccount = true;
        AccountLoadedEvent.Set();

        var res = Parallel.ForEach(list, account =>
        {
            Parallel.ForEach(types, t =>
            {
                if (t == typeof(AccountData)) return; // skip the account data

                try
                {
                    typeof(DatabaseHelper).GetMethod(nameof(InitializeTable))?.MakeGenericMethod(t)
                        .Invoke(null, [account.Uid]);
                }
                catch (Exception e)
                {
                    logger.Error("Database initialization error: ", e);
                }
                
            }); // cache the data
        });

        LastSaveTick = DateTime.UtcNow.Ticks;

        SaveThread = new Thread(() =>
        {
            var token = SaveCts.Token;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    CalcSaveDatabase();
                    Thread.Sleep(1000);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    logger.Error("Error in save thread", e);
                }
            }
        });
        SaveThread.IsBackground = true;
        SaveThread.Start();

        LoadAllData = true;
        AllDataLoadedEvent.Set();
    }

    public static void InitializeTable<T>(int uid) where T : BaseDatabaseDataHelper, new()
    {
        var list = sqlSugarScope?.Queryable<T>()
            .Select(x => x)
            .Select<T>()
            .Where(x => x.Uid == uid)
            .ToList();

        foreach (var inst in list!.Select(instance => (instance as BaseDatabaseDataHelper)!))
        {
            var value = UidInstanceMap.GetOrAdd(inst.Uid, _ => []);
            lock (value)
            {
                value.Add(inst);
            }
        }
    }

    public static void InitializeSqlite()
    {
        var baseType = typeof(BaseDatabaseDataHelper);
        var assembly = typeof(BaseDatabaseDataHelper).Assembly;
        var types = assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
        foreach (var type in types)
            typeof(DatabaseHelper).GetMethod("InitializeSqliteTable")?.MakeGenericMethod(type).Invoke(null, null);
    }

    // DO NOT DEL ReSharper disable once UnusedMember.Global
    public static void InitializeSqliteTable<T>() where T : BaseDatabaseDataHelper, new()
    {
        try
        {
            sqlSugarScope?.CodeFirst.InitTables<T>();
        }
        catch (Exception e)
        {
            logger.Error($"Failed to initialize table for {typeof(T).Name}", e);
        }
    }

    public static T? GetInstance<T>(int uid, bool forceReload = false) where T : BaseDatabaseDataHelper, new()
    {
        try
        {
            if (!forceReload && UidInstanceMap.TryGetValue(uid, out var value))
            {
                T? instance;
                lock (value) { instance = value.OfType<T>().FirstOrDefault(); }
                if (instance != null) return instance;
            }
            var t = sqlSugarScope?.Queryable<T>()
                .Where(x => x.Uid == uid)
                .ToList();

            if (t is { Count: > 0 })
            {
                var instance = t[0];
                var cacheList = UidInstanceMap.GetOrAdd(uid, _ => []);
                lock (cacheList)
                {
                    cacheList.RemoveAll(i => i is T);
                    cacheList.Add(instance);
                }
                return instance;
            }

            return null;
        }
        catch (Exception e)
        {
            logger.Error("Unsupported type", e);
            return null;
        }
    }

    public static T GetInstanceOrCreateNew<T>(int uid) where T : BaseDatabaseDataHelper, new()
    {
        var instance = GetInstance<T>(uid);
        if (instance != null) return instance;

        instance = new T
        {
            Uid = uid
        };
        CreateInstance(instance);

        return instance;
    }

    public static List<T>? GetAllInstance<T>() where T : BaseDatabaseDataHelper, new()
    {
        try
        {
            return sqlSugarScope?.Queryable<T>()
                .Select(x => x)
                .ToList();
        }
        catch (Exception e)
        {
            logger.Error("Unsupported type", e);
            return null;
        }
    }

    public static void UpdateInstance<T>(T instance) where T : BaseDatabaseDataHelper, new()
    {
        sqlSugarScope?.Updateable(instance).ExecuteCommand();
        NeedSave(instance.Uid);
    }

    public static void NeedSave(int uid)
    {
        lock (_saveLock)
        {
            if (!_toSaveUidList.Contains(uid))
                _toSaveUidList.Add(uid);
        }
    }

    public static void CreateInstance<T>(T instance) where T : BaseDatabaseDataHelper, new()
    {
        sqlSugarScope?.Insertable(instance).ExecuteCommand();
        var value = UidInstanceMap.GetOrAdd(instance.Uid, _ => []);
        lock (value)
        {
            value.Add(instance);
        }
    }

    public static void DeleteInstance<T>(int key) where T : BaseDatabaseDataHelper, new()
    {
        try
        {
            sqlSugarScope?.Deleteable<T>().Where(x => x.Uid == key).ExecuteCommand();
        }
        catch (Exception e)
        {
            logger.Error("An error occurred while delete the database", e);
        }
    }

    public static void DeleteAllInstance(int key)
    {
        if (!UidInstanceMap.TryGetValue(key, out var value)) return;
        lock (value)
        {
            foreach (var instance in value)
            {
                var type = instance.GetType();
                typeof(DatabaseHelper).GetMethod("DeleteInstance")?.MakeGenericMethod(type)
                    .Invoke(null, [key]);
            }
        }

        if (UidInstanceMap.TryRemove(key, out _))
        {
            lock (_saveLock) { _toSaveUidList.RemoveAll(x => x == key); }
        }
    }

    // Auto save per 5 min
    public static void CalcSaveDatabase()
    {
        if (LastSaveTick + TimeSpan.TicksPerMinute * 5 > DateTime.UtcNow.Ticks) return;
        SaveDatabase();
    }

    public static void SaveDatabase()
    {
        try
        {
            var prev = DateTime.Now;

            List<int> list;
            lock (_saveLock)
            {
                list = _toSaveUidList.ToList();
                _toSaveUidList.Clear();
            }

            foreach (var uid in list)
            {
                if (!UidInstanceMap.TryGetValue(uid, out var value)) continue;
                lock (value)
                {
                    foreach (var instance in value)
                    {
                        var type = instance.GetType();
                        typeof(DatabaseHelper).GetMethod("SaveDatabaseType")?.MakeGenericMethod(type)
                            .Invoke(null, [instance]);
                    }
                }
            }

            var t = (DateTime.Now - prev).TotalSeconds;
            logger.Info(I18NManager.Translate("Server.ServerInfo.SaveDatabase",
                Math.Round(t, 2).ToString(CultureInfo.InvariantCulture)));
        }
        catch (Exception e)
        {
            logger.Error("An error occurred while saving the database", e);
        }

        LastSaveTick = DateTime.UtcNow.Ticks;
    }

    // DO NOT DEL ReSharper save database from cache
    public static void SaveDatabaseType<T>(T instance) where T : BaseDatabaseDataHelper, new()
    {
        try
        {
            sqlSugarScope?.Updateable(instance).ExecuteCommand();
        }
        catch (Exception e)
        {
            logger.Error("An error occurred while saving the database", e);
        }
    }
}