using SQLite;
using UnityEngine;

public class User 
{
    [PrimaryKey]
    public int Id { get; set; }
    public string Name { get; set; } 
}
