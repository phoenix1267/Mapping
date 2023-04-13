using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Area
{
    private Map owner;
    private Sprite background;
    [SerializeField] private string spritePath = "";
    [SerializeField] private List<Door> allDoors = new List<Door>();
    [SerializeField] private Vector2 locationMap = new Vector2(0, 0);
    private Button openner;
    private Button backToHub;
    [SerializeField] private bool isValid = false;
    [SerializeField] private int id = 0;

    private Door selected = null;
    public Door Selected => selected;
    
    public Vector2 LocationMap => locationMap;
    public Button Openner => openner;
    public Button BackToHub => backToHub;
    public bool IsValid => isValid;
    public int ID => id;
    public List<Door> AllDoors => allDoors;
    public Map Owner => owner;

    public void SetSelected(Door _door) => selected = _door;
    
    public void SetBackground(Sprite _toSet,string _path)
    {
        spritePath = _path;
        background = _toSet;
    }
    public Sprite Background => background;
    public Area()
    {
        
    }

    public Area(Vector2 _pos,int _id)
    {
        locationMap = _pos;
        id = _id;
    }
    
    public void Init()
    {
        isValid = true;
        for (int i = 0; i < allDoors.Count; i++)
        {
            allDoors[i].SetOwner(this);
        }
        backToHub = owner.Owner.AddButton(new Vector2(500, 50), owner.Owner.Hub);
        backToHub.onClick.AddListener(Close);
        backToHub.gameObject.SetActive(false);
        background = TextureLoader.LoadNewSprite(spritePath);
    }
    
    public void SetOwner(Map _owner) => owner = _owner;
    public void SetOpenner(Button _toSet)
    {
        openner = _toSet;
        openner.onClick.AddListener(Open);
    }

    public List<int> GetAllLikedArea()
    {
        List<int> _result = new List<int>();
        for (int i = 0; i < allDoors.Count; i++)
        {
            if(allDoors[i].AsLink)
                _result.Add(allDoors[i].LinkedAreaID);
        }
        return _result;
    }

    public void Open()
    {
        owner.ChangeVisibilityButton(false);
        ChangeBackground();
        backToHub.gameObject.SetActive(true);
        owner.Owner.SetCurrentArea(this);
        ChangeDoorsVisibility(true);
    }

    public void Close()
    {
        owner.ChangeVisibilityButton(true);
        owner.ChangeBackground();
        backToHub.gameObject.SetActive(false);
        owner.Owner.SetCurrentArea(null);
        ChangeDoorsVisibility(false);
    }

    void ChangeDoorsVisibility(bool _visibility)
    {
        for (int i = 0; i < allDoors.Count; i++)
        {
            allDoors[i].Teleporter.gameObject.SetActive(_visibility);
        }
    }

    public void ChangeBackground()
    {
        owner.Owner.BackgroundImage.sprite = background;
    }
}
