using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SubArea
{
    [SerializeField] private string spritePath = "";
    [SerializeField] private List<Door> allDoors = new();
    [SerializeField] private List<SubArea> allAreas = new();
    [SerializeField] private Vector2 locationMap = new(0, 0);
    [SerializeField] private bool isValid = false;
    [SerializeField] private int id = 0;
    private Map owner;
    private Sprite background;
    private CustomButton opener;
    private Door selected = null;
    
    public Door Selected => selected;
    public Vector2 LocationMap => locationMap;
    public CustomButton Opener => opener;
    public bool IsValid => isValid;
    public int ID => id;
    public List<Door> AllDoors => allDoors;
    public List<SubArea> AllAreas => allAreas;
    public Map Owner => owner;

    public void SetSelected(Door _door) => selected = _door;
    
    public void SetBackground(Sprite _toSet,string _path)
    {
        spritePath = _path;
        background = _toSet;
    }
    public Sprite Background => background;
    public SubArea()
    {}

    public SubArea(Vector2 _pos,int _id)
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
            allDoors[i].CreateButton();
        }
        background = TextureLoader.LoadNewSprite(spritePath);
    }
    
    public void SetOwner(Map _owner) => owner = _owner;
    public void SetOpenner(CustomButton _toSet)
    {
        opener = _toSet;
        opener.onLeftClick.AddListener(Open);
        opener.onRightClick.AddListener(DeleteForButton);
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
        owner.Owner.SetCurrentArea(this);
        ChangeDoorsVisibility(true);
    }

    public void Delete()
    {
        if(!owner.Owner.IsEditor)return;
        for (int i = 0; i < allDoors.Count;i++)
            allDoors[i].Remove();
        for (int i = 0; i < allAreas.Count;i++)
            allAreas[i].Delete();
        owner.Owner.MyDestroyObject(opener.gameObject);
    }
    
    public void DeleteForButton()
    {
        if(!owner.Owner.IsEditor)return;
        for (int i = 0; i < allAreas.Count;i++)
            allAreas[i].Delete();
        for (int i = 0; i < allDoors.Count;i++)
            allDoors[i].Remove();
        owner.AllAreas.Remove(this);
        owner.Owner.MyDestroyObject(opener.gameObject);
    }
    
    public void BackToHubClose()
    {
        owner.ChangeVisibilityButton(true);
        owner.ChangeBackground();
        owner.Owner.SetCurrentArea(null);
        ChangeDoorsVisibility(false);
    }

    public void CloseArea()
    {
        ChangeDoorsVisibility(false);
    }

    void ChangeDoorsVisibility(bool _visibility)
    {
        for (int i = 0; i < allDoors.Count; i++)
            allDoors[i].Teleporter.gameObject.SetActive(_visibility);
        for (int i = 0; i < allAreas.Count; i++)
            allAreas[i].Opener.gameObject.SetActive(_visibility);
    }

    public void ChangeBackground()
    {
        owner.Owner.BackgroundImage.sprite = background;
    }
}
