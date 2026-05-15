using System;
using System.IO;
using System.Linq;
using SQLite;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("GameData")]
    [SerializeField] private string _saveFileName = "save_user_data.db";

    [Header("UI Panels")]
    public GameObject settingsPanel;
    public GameObject NameSetPanel;
    public TMP_InputField NameInputField;
    [SerializeField] private GameObject MessegePanel;

    private string _dbPath;
    private User _userData;

    private void Awake()
    {
        _dbPath = Path.Combine(Application.persistentDataPath, _saveFileName);
    }

    /// <summary>Продолжить: загрузить сохранение и войти в игру (или запросить имя, если его нет).</summary>
    public void StartGame()
    {
        try
        {
            _userData = LoadOrCreateUser();

            if (string.IsNullOrEmpty(_userData.Name))
            {
                ShowNamePanel();
                return;
            }

            SceneManager.LoadScene(1);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Не удалось открыть или создать БД: {_dbPath}\n{ex}");
        }
    }

    /// <summary>Новая игра: окно подтверждения → по «Да» сброс сохранения и ввод имени.</summary>
    public void NewGame()
    {
        if (MessegePanel != null)
            MessegePanel.SetActive(true);

        MessagePrebafScript.Show(
            "Are you sure you want to start a new game?",
            ConfirmNewGame);
    }

    void ConfirmNewGame()
    {
        try
        {
            ResetSaveData();
            _userData = null;

            if (NameInputField != null)
                NameInputField.text = string.Empty;

            if (settingsPanel != null)
                settingsPanel.SetActive(false);

            ShowNamePanel();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Не удалось сбросить сохранение: {_dbPath}\n{ex}");
        }
    }

    void ShowNamePanel()
    {
        if (NameSetPanel != null)
            NameSetPanel.SetActive(true);
    }

    void ResetSaveData()
    {
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }

    public void ConfirmName()
    {
        if (NameInputField == null || string.IsNullOrWhiteSpace(NameInputField.text))
        {
            Debug.LogWarning("Имя не может быть пустым.");
            return;
        }

        try
        {
            if (_userData == null)
                _userData = LoadOrCreateUser();

            _userData.Name = NameInputField.text.Trim();
            Save(_userData);
            SceneManager.LoadScene(1);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Не удалось сохранить имя в БД: {_dbPath}\n{ex}");
        }
    }

    public void OpenSettings() => settingsPanel.SetActive(true);
    public void CloseSettings() => settingsPanel.SetActive(false);

    SQLiteConnection OpenDatabase()
    {
        string directory = Path.GetDirectoryName(_dbPath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        // ReadWrite | Create — файл создаётся при первом открытии, если его ещё нет.
        return new SQLiteConnection(_dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);
    }

    User LoadOrCreateUser()
    {
        using (var db = OpenDatabase())
        {
            db.CreateTable<User>();

            User user = db.Table<User>().OrderByDescending(x => x.Id).FirstOrDefault();
            if (user != null)
                return user;

            user = new User { Name = string.Empty };
            db.Insert(user);
            Debug.Log($"Создана новая БД и запись пользователя: {_dbPath}");
            return user;
        }
    }

    void Save(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        using (var db = OpenDatabase())
        {
            db.CreateTable<User>();

            if (user.Id == 0)
                db.Insert(user);
            else
                db.Update(user);

            Debug.Log($"Сохранено в БД: {_dbPath} (Id={user.Id}, Name={user.Name})");
        }
    }
}
