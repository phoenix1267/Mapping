using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MappingSystem : MonoBehaviour
{
    [SerializeField] private bool isEditor = true, isBackgroundMode = false, isTeleportMode = false, isDoorPlacerMode = false, isAreaPlacerMode = false;
    [SerializeField] private Map currentMap;
    [SerializeField] private Image backgroundImage, backgroundEditorBackground, backgroundBinding, imageToggleEditor;
    [SerializeField] private CustomButton buttonModel;
    [SerializeField] private Sprite hub, spriteEditorOn, spriteEditorOff, spriteTpOn, areaSprite,spriteTpOff;
    [SerializeField] private Button toggleEditor, save, load, backToHub, backgroundButton,unbindButton,toggleDoor,toggleArea,reloadBackgroundData;
    [SerializeField] private Canvas popupBackground, popupBinding,mainApp;
    [SerializeField] private Color buttonBackgroundColorDefault,buttonBackgroundColorActive;
    private SubArea currentSubArea = null;
    private List<CustomButton> allBackgroundButton = new();
    private List<CustomButton> allBindingButton = new();
    string path = "";
    
    public Image BackgroundImage => backgroundImage;
    public Sprite SpriteTPon => spriteTpOn;
   // public Sprite TestArea => testArea;
    public Sprite AreaSprite => areaSprite;
    public Sprite SpriteTpOff => spriteTpOff;
    public Canvas PopupBinding => popupBinding;
    public Canvas MainApp => mainApp;
    public bool IsEditor => isEditor;
    public bool IsBackgroundMode => isBackgroundMode;

    public void SetCurrentArea(SubArea _toSet)
    {
        currentSubArea = _toSet;
        backToHub.gameObject.SetActive(true);
    }
    public void SetTeleportMode(bool _val) => isTeleportMode = _val;
    private void Start() => Init();
    private void Init()
    {
        path = Application.streamingAssetsPath;
        InitBackground();
        RefreshAreaBinder();
        InitButton();
        currentSubArea = null;
        currentMap.SetOwner(this);
        currentMap.Init();
        toggleEditor.onClick.AddListener(ToggleEditor);
        backgroundButton.onClick.AddListener(ToggleBackgroundEditor);
        backToHub.onClick.AddListener(CloseCurrentArea);
        unbindButton.onClick.AddListener(UnbindDoor);
    }

    void UnbindDoor()
    {
        currentSubArea.Selected.Unbind();
    }
    
    void CloseCurrentArea()
    {
        currentSubArea.BackToHubClose();
        backToHub.gameObject.SetActive(false);

    }
    
    private void InitButton()
    {
        backgroundButton.gameObject.SetActive(isEditor);
        save.gameObject.SetActive(isEditor);
        load.gameObject.SetActive(isEditor);
        toggleArea.gameObject.SetActive(isEditor);
        toggleDoor.gameObject.SetActive(isEditor);
        reloadBackgroundData.gameObject.SetActive(isEditor);
    }
    
    public void InitBackground()
    {
        for (int i = 0; i < allBackgroundButton.Count; i++)
        {
            Destroy(allBackgroundButton[i].gameObject);
        }
        allBackgroundButton.Clear();
        List<string> _result = Directory.GetFiles(path, "*.png").ToList();
        //Vector2 _pos = new Vector2(210, 430);
        for (int i = 0; i < _result.Count; i++)
        {
            string _curr = _result[i];
            CustomButton _created = Instantiate(buttonModel,Vector3.zero, Quaternion.identity,backgroundEditorBackground.transform);
            RectTransform _rectTransform = _created.transform as RectTransform;
            _rectTransform.anchorMin = new Vector2(0,1);
            _rectTransform.anchorMax = new Vector2(0,1);
            _rectTransform.pivot = new Vector2(0, 1);
            //_rectTransform.localScale = new Vector3(2, 2, 1);
            //_rectTransform.position = _pos;
            _created.image.sprite = TextureLoader.LoadNewSprite(_curr);
            allBackgroundButton.Add(_created);
            _created.onLeftClick.AddListener(delegate { SetBackground(_created.image.sprite,_curr); });
            /*_pos.y -= 65;
            if (_pos.y < 110)
            {
                _pos.y = 430;
                _pos.x += 65;
            }*/
        }
    }
    
    
    private void Update()
    {
        if (isEditor && !isBackgroundMode && !isTeleportMode)
        {
            Vector3 _mousePos = Input.mousePosition;
            if(Screen.height-50 < _mousePos.y) 
                return;
            if (Input.GetMouseButtonDown(0))
            {
                if (isAreaPlacerMode)
                {
                    if (currentSubArea.IsUnityNull() || !currentSubArea.IsValid)
                    {
                        for (int i = 0; i < currentMap.AllAreas.Count; i++)
                            if (Vector3.Distance(currentMap.AllAreas[i].Opener.gameObject.transform.position, _mousePos) < 40) 
                                return;
                    }
                    else
                    {
                        for (int i = 0; i < currentSubArea.AllAreas.Count; i++)
                            if (Vector3.Distance(currentSubArea.AllAreas[i].Opener.gameObject.transform.position, _mousePos) < 40) 
                                return;
                        for (int i = 0; i < currentSubArea.AllDoors.Count; i++)
                            if (Vector3.Distance(currentSubArea.AllDoors[i].Teleporter.gameObject.transform.position, _mousePos) < 40) 
                                return;
                    }

                    SubArea _tempo = new SubArea(_mousePos,currentMap.AllAreas.Count);
                    _tempo.SetOpenner(AddButton(_tempo.LocationMap,areaSprite));
                    _tempo.SetOwner(currentMap);
                    _tempo.Init();
                    if (currentSubArea.IsUnityNull() || !currentSubArea.IsValid)
                    {
                        currentMap.AllAreas.Add(_tempo);
                    }
                    else
                    {
                        _tempo.SetSubOwner(currentSubArea);
                        _tempo.Opener.onLeftClick.AddListener(currentSubArea.CloseArea);
                        currentSubArea.AllAreas.Add(_tempo);
                    }
                }
                else if (isDoorPlacerMode && !currentSubArea.IsUnityNull() && currentSubArea.IsValid)
                {
                    for (int i = 0; i < currentSubArea.AllDoors.Count; i++)
                        if (Vector3.Distance(currentSubArea.AllDoors[i].Teleporter.gameObject.transform.position, _mousePos) < 40) 
                            return;
                    Door _tempo = new Door(_mousePos);
                    _tempo.SetOwner(currentSubArea);
                    _tempo.Teleporter = AddButton(_tempo.LocationMap,spriteTpOff);
                    currentSubArea.AllDoors.Add(_tempo);
                }
            }
        }
    }

    void ToggleEditor()
    {
        isEditor = !isEditor;
        if (isEditor)
        {
            imageToggleEditor.sprite = spriteEditorOn;
            toggleEditor.image.color = buttonBackgroundColorActive;
        }
        else
        {
            imageToggleEditor.sprite = spriteEditorOff;
            if(isBackgroundMode)
                ToggleBackgroundEditor();
            toggleEditor.image.color = buttonBackgroundColorDefault;
        }
        backgroundButton.gameObject.SetActive(isEditor);
        save.gameObject.SetActive(isEditor);
        load.gameObject.SetActive(isEditor);
        toggleArea.gameObject.SetActive(isEditor);
        toggleDoor.gameObject.SetActive(isEditor);
        reloadBackgroundData.gameObject.SetActive(isEditor);
    }

    public void ToggleBackgroundEditor()
    {
        if (isTeleportMode) return;
        isBackgroundMode = !isBackgroundMode;
        popupBackground.gameObject.SetActive(isBackgroundMode);
        backgroundButton.image.color = isBackgroundMode ? buttonBackgroundColorActive : buttonBackgroundColorDefault;
    }

    public void CloseBackgroundEditor()
    {
        if (isBackgroundMode)
            ToggleBackgroundEditor();
    }
    
    void SetBackground(Sprite _toSet,string _path)
    {
        if (currentSubArea.IsUnityNull() || !currentSubArea.IsValid)
        {
            currentMap.SetBackground(_toSet, _path);
            currentMap.ChangeBackground();
        }
        else
        {
            currentSubArea.SetBackground(_toSet,_path);
            currentSubArea.ChangeBackground();
        }
    }
    
    public CustomButton AddButton(Vector2 _locationMap,Sprite _sprite)
    {
        CustomButton _created = Instantiate(buttonModel,_locationMap,Quaternion.identity,backgroundImage.transform);
        _created.image.sprite = _sprite;
        return _created;
    }

    public void RefreshAreaBinder()
    {
        for (int i = 0; i < allBindingButton.Count; i++)
        {
            Destroy(allBindingButton[i].gameObject);
        }
        allBindingButton.Clear();
        Vector2 _pos = new Vector2(210, 430);
        currentMap.AllAreasGlobal.Clear();
        for (int i = 0; i < currentMap.AllAreas.Count; i++)
        {
            currentMap.AllAreasGlobal.Add(currentMap.AllAreas[i]);
            List<SubArea> _curr = currentMap.AllAreas[i].GetAllUnderAreas();
            for (int j = 0; j < _curr.Count; j++)
            {
                currentMap.AllAreasGlobal.Add(_curr[j]);
            }
        }
        
        for (int i = 0; i < currentMap.AllAreasGlobal.Count; i++)
        {
            CreateAreaBinderButton(currentMap.AllAreasGlobal[i], _pos);
        }
    }

    void CreateAreaBinderButton(SubArea _curr,Vector2 _pos)
    {
        CustomButton _created = Instantiate(buttonModel,_pos,Quaternion.identity,backgroundBinding.transform);
        RectTransform _rectTransform = _created.transform as RectTransform;
        _rectTransform.anchorMin = new Vector2(0,1);
        _rectTransform.anchorMax = new Vector2(0,1);
        _rectTransform.pivot = new Vector2(0, 1);
        _created.image.sprite = _curr.Background;
        allBindingButton.Add(_created);
        _created.onLeftClick.AddListener(delegate {currentSubArea.Selected.AddLink(_curr.ID);});
    }
    
    public void ClosePopupBinding()
    {
        popupBinding.gameObject.SetActive(false);
        if(!currentSubArea.IsUnityNull() && !currentSubArea.Selected.IsUnityNull())
            currentSubArea.Selected.CloseDoorChanger();
    }

    public void Save()
    {
        string _json = JsonUtility.ToJson(currentMap);
        File.WriteAllText(path+"/Save.txt",_json);
    }

    public void Load()
    {
        for (int i = 0; i < currentMap.AllAreas.Count; i++)
            currentMap.AllAreas[i].Delete();
        for (int i = 0; i < currentMap.AllDoors.Count; i++)
            currentMap.AllDoors[i].Remove();
        currentMap.AllAreas.Clear();
        currentSubArea = null;
        backToHub.gameObject.SetActive(false);
        isTeleportMode = false;
        isBackgroundMode = false;
        
        string _json = File.ReadAllText(path + "/Save.txt");
        currentMap = JsonUtility.FromJson<Map>(_json);
        
        InitBackground();
        currentMap.SetOwner(this);
        currentMap.Init();
        RefreshAreaBinder();
    }

    public void MyDestroyObject(GameObject _toDestroy)
    {
        Destroy(_toDestroy);
    }

    public void ToggleAreaMode()
    {
        isAreaPlacerMode = !isAreaPlacerMode;
        if (isAreaPlacerMode)
        {
            isDoorPlacerMode = false;
            toggleDoor.image.color = buttonBackgroundColorDefault;
            toggleArea.image.color = buttonBackgroundColorActive;
        }
        else
            toggleArea.image.color = buttonBackgroundColorDefault;
    }
    public void ToggleDoorMode()
    {
        isDoorPlacerMode = !isDoorPlacerMode;
        if (isDoorPlacerMode)
        {
            isAreaPlacerMode = false;
            toggleArea.image.color = buttonBackgroundColorDefault;
            toggleDoor.image.color = buttonBackgroundColorActive;
        }
        else
            toggleDoor.image.color = buttonBackgroundColorDefault;
    }

}
