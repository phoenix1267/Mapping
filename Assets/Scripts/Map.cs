using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Map 
{
    [SerializeField] private List<SubArea> allAreas = new(),allAreasGlobal = new();
    [SerializeField] private List<Door> allDoors = new();
    [SerializeField,HideInInspector] private string backgroundPath = "";
    private MappingSystem owner;
    private Sprite fullMap;
    public List<SubArea> AllAreas => allAreas;
    public List<SubArea> AllAreasGlobal => allAreasGlobal;
    public List<Door> AllDoors => allDoors;
    public void SetOwner(MappingSystem _owner) => owner = _owner;

    public void SetBackground(Sprite _toSet,string _path)
    {
        backgroundPath = _path;
        fullMap = _toSet;
    }

    public MappingSystem Owner => owner;
    public void Init()
    {
        for (int i = 0; i < allAreas.Count; i++)
        {
            allAreas[i].SetOwner(this);
            allAreas[i].Init();
        }
        fullMap = TextureLoader.LoadNewSprite(backgroundPath);
        ChangeBackground();
        InitButton();
    }

    void InitButton()
    {
        for (int i = 0; i < allAreas.Count; i++)
        {
            //allAreas[i].SetOpenner(owner.AddButton(allAreas[i].LocationMap,owner.AreaSprite));
            allAreas[i].InitOpenners();
        }
    }

    public void ChangeVisibilityButton(bool _value)
    {
        for (int i = 0; i < allAreas.Count; i++)
        {
            allAreas[i].Opener.gameObject.SetActive(_value);
        }
    }

    public void ChangeBackground()
    {
        owner.BackgroundImage.sprite = fullMap;
    }

}
