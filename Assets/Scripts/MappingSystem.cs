using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MappingSystem : MonoBehaviour
{
    [SerializeField] private bool isEditor = true, isBackgroundMode = false, isTeleportMode = false;
    [SerializeField] private Map currentMap;
    [SerializeField] private Image backgroundImage, backgroundEditorBackground, backgroundBinding, imageToggleEditor;
    [SerializeField] private CustomButton buttonModel;
    [SerializeField] private Sprite hub, spriteEditorOn, spriteEditorOff, spriteTPon, testArea;
    [SerializeField] private Button toggleEditor, save, load, backToHub, backgroundButton,unbindButton;
    [SerializeField] private Canvas popupBackground, popupBinding,mainApp;
    private Area currentArea = null;
    private List<CustomButton> allBackgroundButton = new();
    private List<CustomButton> allBindingButton = new();
    string path = "";
    
    public Image BackgroundImage => backgroundImage;
    public Sprite SpriteTPon => spriteTPon;
    public Sprite TestArea => testArea;
    public Canvas PopupBinding => popupBinding;
    public Canvas MainApp => mainApp;
    public bool IsEditor => isEditor;
    public bool IsBackgroundMode => isBackgroundMode;

    public void SetCurrentArea(Area _toSet)
    {
        currentArea = _toSet;
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
        currentArea = null;
        currentMap.SetOwner(this);
        currentMap.Init();
        toggleEditor.onClick.AddListener(ToggleEditor);
        backgroundButton.onClick.AddListener(ToggleBackgroundEditor);
        backToHub.onClick.AddListener(CloseCurrentArea);
        unbindButton.onClick.AddListener(UnbindDoor);
    }

    void UnbindDoor()
    {
        currentArea.Selected.Unbind();
    }
    
    void CloseCurrentArea()
    {
        currentArea.Close();
        backToHub.gameObject.SetActive(false);

    }
    
    private void InitButton()
    {
        backgroundButton.gameObject.SetActive(isEditor);
        save.gameObject.SetActive(isEditor);
        load.gameObject.SetActive(isEditor);
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
                if (currentArea.IsUnityNull() || !currentArea.IsValid)
                {
                    for (int i = 0; i < currentMap.AllAreas.Count; i++)
                        if (Vector3.Distance(currentMap.AllAreas[i].Opener.gameObject.transform.position, _mousePos) < 40) 
                            return;

                    Area _tempo = new Area(_mousePos,currentMap.AllAreas.Count);
                    _tempo.SetOpenner(AddButton(_tempo.LocationMap,testArea));
                    _tempo.SetOwner(currentMap);
                    _tempo.Init();
                    currentMap.AllAreas.Add(_tempo);
                    
                }
                else
                {
                    for (int i = 0; i < currentArea.AllDoors.Count; i++)
                        if (Vector3.Distance(currentArea.AllDoors[i].Teleporter.gameObject.transform.position, _mousePos) < 40) 
                            return;
                    
                    Door _tempo = new Door(_mousePos);
                    _tempo.SetOwner(currentArea);
                    _tempo.Teleporter = AddButton(_tempo.LocationMap,testArea);
                    currentArea.AllDoors.Add(_tempo);
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

        }
        else
        {
            imageToggleEditor.sprite = spriteEditorOff;
            if(isBackgroundMode)
                ToggleBackgroundEditor();
        }
        backgroundButton.gameObject.SetActive(isEditor);
        save.gameObject.SetActive(isEditor);
        load.gameObject.SetActive(isEditor);
    }

    public void ToggleBackgroundEditor()
    {
        if (isTeleportMode) return;
        isBackgroundMode = !isBackgroundMode;
        popupBackground.gameObject.SetActive(isBackgroundMode);
        if (isBackgroundMode)
            InitBackground();
    }

    public void CloseBackgroundEditor()
    {
        if (isBackgroundMode)
            ToggleBackgroundEditor();
    }
    
    void SetBackground(Sprite _toSet,string _path)
    {
        if (currentArea.IsUnityNull() || !currentArea.IsValid)
        {
            currentMap.SetBackground(_toSet, _path);
            currentMap.ChangeBackground();
        }
        else
        {
            currentArea.SetBackground(_toSet,_path);
            currentArea.ChangeBackground();
        }
    }
    
    public CustomButton AddButton(Vector2 _locationMap,Sprite _sprite)
    {
        CustomButton _created = Instantiate(buttonModel,_locationMap,Quaternion.identity,backgroundImage.transform);
        _created.image.sprite = _sprite;
        return _created;
    }
    public CustomButton AddButton(Vector2 _locationMap)
    {
        CustomButton _created = Instantiate(buttonModel,_locationMap,Quaternion.identity,backgroundImage.transform);
        _created.image.sprite = testArea;
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
        for (int i = 0; i < currentMap.AllAreas.Count; i++)
        {
            Area _curr = currentMap.AllAreas[i];
            CustomButton _created = Instantiate(buttonModel,_pos,Quaternion.identity,backgroundBinding.transform);
            RectTransform _rectTransform = _created.transform as RectTransform;
            _rectTransform.anchorMin = new Vector2(0,1);
            _rectTransform.anchorMax = new Vector2(0,1);
            _rectTransform.pivot = new Vector2(0, 1);
            //_rectTransform.localScale = new Vector3(2, 2, 1);
            //_rectTransform.position = _pos;
            _created.image.sprite = _curr.Background;
            allBindingButton.Add(_created);
            _created.onLeftClick.AddListener(delegate {currentArea.Selected.AddLink(_curr.ID);});
            /*_pos.y -= 65;
            if (_pos.y < 110)
            {
                _pos.y = 430;
                _pos.x += 65;
            }*/
        }
    }

    public void ClosePopupBinding()
    {
        popupBinding.gameObject.SetActive(false);
        if(!currentArea.IsUnityNull() && !currentArea.Selected.IsUnityNull())
            currentArea.Selected.CloseDoorChanger();

    }

    public void Save()
    {
        string _json = JsonUtility.ToJson(currentMap);
        File.WriteAllText(path+"/Save.txt",_json);
    }

    public void Load()
    {
        for (int i = 0; i < currentMap.AllAreas.Count; i++)
        {
            currentMap.AllAreas[i].Delete();
        }
        currentMap.AllAreas.Clear();
        currentArea = null;
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

}
