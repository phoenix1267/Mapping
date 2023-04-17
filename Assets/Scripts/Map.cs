using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Map
{
    [SerializeField] private List<Area> allAreas = new();
    [SerializeField,HideInInspector] private string backgroundPath = "";
    private MappingSystem owner;
    private Sprite fullMap;
    public List<Area> AllAreas => allAreas;
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
            allAreas[i].SetOpenner(owner.AddButton(allAreas[i].LocationMap,owner.TestArea));
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
