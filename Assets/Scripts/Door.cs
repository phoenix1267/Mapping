using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Door
{
    [SerializeField] private bool asLink = false;
    [SerializeField] private int linkedAreaID = 0;
    [SerializeField] private Vector2 locationMap = new Vector2(0, 0);
    private Area owningArea;
    private CustomButton teleporter;
    public void SetOwner(Area _owner) => owningArea = _owner;
    public Vector2 LocationMap => locationMap;
    public int LinkedAreaID => linkedAreaID;
    public bool AsLink => asLink;
    
    public CustomButton Teleporter
    {
        get
        {
            if (teleporter.IsUnityNull())
                CreateButton();
            return teleporter;
        }

        set
        {
            teleporter = value;
            teleporter.onLeftClick.AddListener(Teleport);
            teleporter.onRightClick.AddListener(Edit);
        }
    }

    public Door()
    {}

    public Door(Vector2 _pos)
    {
        locationMap = _pos;
    }
    

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
        if(owningArea.Owner.Owner.IsBackgroundMode)return;
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
        owningArea.Owner.Owner.MyDestroyObject(teleporter.gameObject);
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

    public void Unbind()
    {
        asLink = false;
        linkedAreaID = 0;
        teleporter.image.sprite = owningArea.Owner.Owner.TestArea;
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
