using SQLite;
using UnityEngine;

public class User 
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Name { get; set; } 
}
