using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Room
{
    private string name;
    private string description;
    private Transform location;

    private Room() { }

    //TODO Room tags?
    public Room(string name, Transform location, string description)
    {
        Name = name;
        Location = location;
        Description = description;
    }

    public string Name
    {
        get { return name; }
        set { name = value; }

    }

    public Transform Location
    {
        get { return location; }
        set { location = value; }
    }

    public string Description
    {
        get { return description; }
        set { description = value; }
    }

}