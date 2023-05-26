using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Door
{
    [SerializeField] private bool asLink = false;
    [SerializeField] private int linkedAreaID = 0;
    [SerializeField] private Vector2 locationMap = new Vector2(0, 0);
    [NonSerialized] private SubArea owningSubArea;
    private CustomButton teleporter;
    public void SetOwner(SubArea _owner) => owningSubArea = _owner;
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
            owningSubArea.BackToHubClose();
            for (int i = 0; i < owningSubArea.Owner.AllAreasGlobal.Count; i++)
            {
                if (owningSubArea.Owner.AllAreasGlobal[i].ID == linkedAreaID)
                {
                    owningSubArea.Owner.AllAreasGlobal[i].Open();
                    break;
                }
            }
            //owningSubArea.Owner.AllAreasGlobal[linkedAreaID].Open();
        }
        else
            EditTeleporterDestination();
        
    }

    void EditTeleporterDestination()
    {
        if(owningSubArea.Owner.Owner.IsBackgroundMode)return;
        owningSubArea.SetSelected(this);
        owningSubArea.Owner.Owner.PopupBinding.gameObject.SetActive(true);
        owningSubArea.Owner.Owner.RefreshAreaBinder();
        owningSubArea.Owner.Owner.SetTeleportMode(true);
    }
    
    void Edit()
    {
        if (!owningSubArea.Owner.Owner.IsEditor)
            EditTeleporterDestination();
        else
            RemoveButton();
    }

    public void Remove()
    {
        owningSubArea.Owner.Owner.MyDestroyObject(teleporter.gameObject);
    }

    public void RemoveButton()
    {
        owningSubArea.AllDoors.Remove(this);
        owningSubArea.Owner.Owner.MyDestroyObject(teleporter.gameObject);
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
        teleporter.image.sprite = owningSubArea.Owner.Owner.SpriteTpOff;
    }
    
    public void CreateButton()
    {
        teleporter = owningSubArea.Owner.Owner.AddButton(locationMap,asLink ? owningSubArea.Owner.Owner.SpriteTPon : owningSubArea.Owner.Owner.SpriteTpOff);
        teleporter.gameObject.SetActive(false);
        teleporter.onLeftClick.AddListener(Teleport);
        teleporter.onRightClick.AddListener(Edit);
    }
    public void CloseDoorChanger()
    {
        owningSubArea.SetSelected(null);
        owningSubArea.Owner.Owner.SetTeleportMode(false);
        if(asLink)
            teleporter.image.sprite = owningSubArea.Owner.Owner.SpriteTPon;
    }
}
