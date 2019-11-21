using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room
{
    private string name;
    private string description;
    private Transform location;
    private Image image;
    private string distanceToUser;

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

    public Image Image
    {
        get { return image; }
        set { image = value; }
    }

    public string DistanceToUser
    {
        get { return distanceToUser; }
        set { distanceToUser = value; }
    }

}