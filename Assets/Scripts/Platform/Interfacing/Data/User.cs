using System;

namespace Platform.Interfacing.Data
{

/// <summary>
/// Abstract class to hold user date.
/// </summary>
public abstract class User
{
    public int? id { get; private set; }
    public string name { get; private set; }
    public string email { get; private set; }

    public User(int id, string name, string email)
    {

        this.id = id;
        this.name = name;
        this.email = email;
    }
}

}
