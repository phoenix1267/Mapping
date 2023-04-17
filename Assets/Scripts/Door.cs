using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
//using CustomButton = UnityEngine.UI.CustomButton;

[Serializable]
public class Door
{
    private Area owningArea;
    [SerializeField] private bool asLink = false;
    [SerializeField] private int linkedAreaID = 0;
    [SerializeField] private Vector2 locationMap = new Vector2(0, 0);
    private CustomButton teleporter;
    public void SetOwner(Area _owner) => owningArea = _owner;
    public Vector2 LocationMap => locationMap;

    public void SetTeleporter(CustomButton _toSet)
    {
        teleporter = _toSet;
        teleporter.onLeftClick.AddListener(Teleport);
        teleporter.onRightClick.AddListener(Edit);
    }

    public CustomButton Teleporter
    {
        get
        {
            if (teleporter.IsUnityNull())
                CreateButton();
            return teleporter;
        }
    }

    public Door()
    {
        
    }

    public Door(Vector2 _pos)
    {
        locationMap = _pos;
    }
    
    public int LinkedAreaID => linkedAreaID;
    public bool AsLink => asLink;

    void Teleport()
    {
        
        if (asLink)
        {
            owningArea.Close();
            owningArea.Owner.AllAreas[linkedAreaID].Open();
        }
        else
            EditTeleporterDestination();
        
    }

    void EditTeleporterDestination()
    {
        owningArea.SetSelected(this);
        owningArea.Owner.Owner.PopupBinding.gameObject.SetActive(true);
        owningArea.Owner.Owner.RefreshAreaBinder();
        owningArea.Owner.Owner.SetTeleportMode(true);
    }
    
    void Edit()
    {
        if (!owningArea.Owner.Owner.IsEditor)
            EditTeleporterDestination();
        else
            RemoveButton();
    }

    public void Remove()
    {
        //owningArea.AllDoors.Remove(this);
        owningArea.Owner.Owner.MyDestroyObject(teleporter.gameObject);
        //owningArea = null;
    }

    public void RemoveButton()
    {
        owningArea.AllDoors.Remove(this);
        owningArea.Owner.Owner.MyDestroyObject(teleporter.gameObject);

    }
    
    public void AddLink(int _id)
    {
        asLink = true;
        linkedAreaID = _id;
    }

    public void RemoveLink()
    {
        asLink = false;
    }

    public void CreateButton()
    {
        teleporter = owningArea.Owner.Owner.AddButton(locationMap);
        teleporter.gameObject.SetActive(false);
        teleporter.onLeftClick.AddListener(Teleport);
        teleporter.onRightClick.AddListener(Edit);
    }

    public void CloseDoorChanger()
    {
        owningArea.SetSelected(null);
        owningArea.Owner.Owner.SetTeleportMode(false);
        if(asLink)
            teleporter.image.sprite = owningArea.Owner.Owner.SpriteTPon;
    }
}
