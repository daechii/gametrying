using SQLite;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("GameData")]
    [SerializeField] private User _userData = null;
    [SerializeField] private string _saveFileName = "save_user_data.db";

    [Header("UI Panels")]
    public GameObject settingsPanel;
    public GameObject NameSetPanel;
    public TMP_InputField NameInputField; 

    private string _dbPath;

    private void Awake()
    {
        _dbPath = Path.Combine(Application.persistentDataPath, _saveFileName);
    }

    public void StartGame()
    {
        _userData = Load();

        if (string.IsNullOrEmpty(_userData.Name))
        {
            NameSetPanel.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene(1);
        }

        string path = Path.Combine(Application.persistentDataPath, _saveFileName);
        if (File.Exists(path))
        {
            Debug.Log("<color=green>Файл базы найден по адресу: </color>" + path);
        }
        else
        {
            Debug.LogError("<color=red>Файла НЕТ! Проверь выполнение метода Load().</color>");
        }
    }

    public void ConfirmName()
    {
        if (!string.IsNullOrEmpty(NameInputField.text))
        {
            _userData.Name = NameInputField.text;
            Save(_userData); 
            SceneManager.LoadScene(1); 
        }
        else
        {
            Debug.Log("Имя не может быть пустым!");
        }
    }


    public void OpenSettings() => settingsPanel.SetActive(true);
    public void CloseSettings() => settingsPanel.SetActive(false);
    public void ExitGame() => Application.Quit();


    private User Load()
    {
        using (var dbConnection = new SQLiteConnection(_dbPath))
        {
            dbConnection.CreateTable<User>();

            var user = dbConnection.Table<User>().OrderByDescending(x => x.Id).FirstOrDefault();

            if (user == null)
            {
                user = new User { Id = 1, Name = "" };
                dbConnection.Insert(user);
            }
            return user;
        }
    }

    private void Save(User user)
    {
        using (var dbConnection = new SQLiteConnection(_dbPath))
        {
            dbConnection.InsertOrReplace(user);
        }
    }
}