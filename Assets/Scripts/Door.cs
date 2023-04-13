using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

[Serializable]
public class Door
{
    private Area owningArea;
    [SerializeField] private bool asLink = false;
    [SerializeField] private int linkedAreaID = 0;
    [SerializeField] private Vector2 locationMap = new Vector2(0, 0);
    private Button teleporter;
    public void SetOwner(Area _owner) => owningArea = _owner;
    public Vector2 LocationMap => locationMap;

    public void SetTeleporter(Button _toSet)
    {
        teleporter = _toSet;
        teleporter.onClick.AddListener(Teleport);
    }

    public Button Teleporter
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
        {
            owningArea.SetSelected(this);
            owningArea.Owner.Owner.RefreshAreaBinder();
            owningArea.Owner.Owner.PopupBinding.gameObject.SetActive(true);
            owningArea.Owner.Owner.SetTeleportMode(true);
            
        }
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
        teleporter.onClick.AddListener(Teleport);
        
    }

    public void CloseDoorChanger()
    {
        owningArea.SetSelected(null);
        owningArea.Owner.Owner.SetTeleportMode(false);
        if(asLink)
            teleporter.image.sprite = owningArea.Owner.Owner.SpriteTPon;
    }
}
