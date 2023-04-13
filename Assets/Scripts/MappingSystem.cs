using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MappingSystem : MonoBehaviour
{
    [SerializeField] private bool isEditor = true;
    [SerializeField] private bool isBackgroundMode = false;
    [SerializeField] private bool isTeleportMode = false;
    [SerializeField] private Map currentMap;
    [SerializeField] private Area currentArea = null;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image backgroundEditorBackground;
    [SerializeField] private Image backgroundBinding;
    [SerializeField] private Button buttonModel;
    [SerializeField] private Button backgroundButton;
    [SerializeField] private Sprite hub;
    [SerializeField] private Sprite spriteEditorOn;
    [SerializeField] private Sprite spriteEditorOff;
    [SerializeField] private Sprite spriteTPon;
    [SerializeField] private Button toggleEditor;
    [SerializeField] private Button save;
    [SerializeField] private Button load;
    [SerializeField] private Image imageToggleEditor;
    [SerializeField] private Sprite testArea;
    [SerializeField] private Canvas popupBackground;
    [SerializeField] private Canvas popupBinding;
    private List<Button> allBackgroundButton = new List<Button>();
    private List<Button> allBindingButton = new List<Button>();
    string m_Path = "";
    
    public Image BackgroundImage => backgroundImage;
    public Sprite Hub => hub;
    public Sprite SpriteTPon => spriteTPon;
    public Sprite TestArea => testArea;
    
    public Canvas PopupBinding => popupBinding;
    
    public Area CurrentArea => currentArea;
    public void SetCurrentArea(Area _toSet) => currentArea = _toSet;
    public void SetTeleportMode(bool _val) => isTeleportMode = _val;
    private void Start()
    {

        Init();
    }

    private void Init()
    {
        m_Path = Application.streamingAssetsPath;
        //Debug.Log("dataPath : " + m_Path);
        InitBackground();
        RefreshAreaBinder();
        InitButton();
        currentArea = null;
        currentMap.SetOwner(this);
        currentMap.Init();
        toggleEditor.onClick.AddListener(ToggleEditor);
        backgroundButton.onClick.AddListener(ToggleBackgroundEditor);
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
        List<string> _result = Directory.GetFiles(m_Path, "*.png").ToList();
        Vector2 _pos = new Vector2(210, 430);
        for (int i = 0; i < _result.Count; i++)
        {
            string _curr = _result[i];
            Button _created = Instantiate(buttonModel,_pos,Quaternion.identity,backgroundEditorBackground.transform);
            RectTransform _rectTransform = _created.transform as RectTransform;
            _rectTransform.anchorMin = new Vector2(0,1);
            _rectTransform.anchorMax = new Vector2(0,1);
            _rectTransform.pivot = new Vector2(0, 1);
            _rectTransform.localScale = new Vector3(2, 2, 1);
            _rectTransform.position = _pos;
            _created.image.sprite = TextureLoader.LoadNewSprite(_curr);
            allBackgroundButton.Add(_created);
            _created.onClick.AddListener(delegate { SetBackground(_created.image.sprite,_curr); });
            _pos.y -= 65;
            if (_pos.y < 110)
            {
                _pos.y = 430;
                _pos.x += 65;
            }
        }
    }
    
    
    private void Update()
    {
        if (isEditor && !isBackgroundMode && !isTeleportMode)
        {
            if (currentArea.IsUnityNull() || !currentArea.IsValid)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if(Screen.height-50 < Input.mousePosition.y) 
                        return;
                    for (int i = 0; i < currentMap.AllAreas.Count; i++)
                    {
                        if (Vector3.Distance(currentMap.AllAreas[i].Openner.gameObject.transform.position, Input.mousePosition) < 40) 
                            return;
                    }
                        
                    Area _tempo = new Area(Input.mousePosition,currentMap.AllAreas.Count);
                    _tempo.SetOpenner(AddButton(_tempo.LocationMap,testArea));
                    _tempo.SetOwner(currentMap);
                    _tempo.Init();
                    currentMap.AllAreas.Add(_tempo);
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if(Screen.height-50 < Input.mousePosition.y) 
                        return;
                    if (Vector3.Distance(currentArea.BackToHub.gameObject.transform.position, Input.mousePosition) < 60) 
                        return;
                    for (int i = 0; i < currentArea.AllDoors.Count; i++)
                    {
                        if (Vector3.Distance(currentArea.AllDoors[i].Teleporter.gameObject.transform.position, Input.mousePosition) < 40) 
                            return;
                    }
                    Door _tempo = new Door(Input.mousePosition);
                    _tempo.SetOwner(currentArea);
                    _tempo.SetTeleporter(AddButton(_tempo.LocationMap,testArea));
                    currentArea.AllDoors.Add(_tempo);
                }
            }
        }
    }

    void ToggleEditor()
    {
        isEditor = !isEditor;
        if (isEditor)
            imageToggleEditor.sprite = spriteEditorOn;
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
        isBackgroundMode = !isBackgroundMode;
        popupBackground.gameObject.SetActive(isBackgroundMode);
        if (isBackgroundMode)
        {
            InitBackground();
        }
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
    
    public Button AddButton(Vector2 _locationMap,Sprite _sprite)
    {
        Button _created = Instantiate(buttonModel,_locationMap,Quaternion.identity,backgroundImage.transform);
        _created.image.sprite = _sprite;
        return _created;
    }
    public Button AddButton(Vector2 _locationMap)
    {
        Button _created = Instantiate(buttonModel,_locationMap,Quaternion.identity,backgroundImage.transform);
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
            Button _created = Instantiate(buttonModel,_pos,Quaternion.identity,backgroundBinding.transform);
            RectTransform _rectTransform = _created.transform as RectTransform;
            _rectTransform.anchorMin = new Vector2(0,1);
            _rectTransform.anchorMax = new Vector2(0,1);
            _rectTransform.pivot = new Vector2(0, 1);
            _rectTransform.localScale = new Vector3(2, 2, 1);
            _rectTransform.position = _pos;
            _created.image.sprite = _curr.Background;
            allBindingButton.Add(_created);
            _created.onClick.AddListener(delegate {currentArea.Selected.AddLink(_curr.ID);});
            _pos.y -= 65;
            if (_pos.y < 110)
            {
                _pos.y = 430;
                _pos.x += 65;
            }
        }
    }

    public void ClosePopupBinding()
    {
        popupBinding.gameObject.SetActive(false);
        currentArea.Selected.CloseDoorChanger();
    }

    public void Save()
    {
        string _json = JsonUtility.ToJson(currentMap);
        //Debug.Log(_json);
        File.WriteAllText(m_Path+"/Save.txt",_json);
    }

    public void Load()
    {
        for (int i = 0; i < currentMap.AllAreas.Count; i++)
        {
            Destroy(currentMap.AllAreas[i].Openner.gameObject);
        }
        currentMap.AllAreas.Clear();
        string _json = File.ReadAllText(m_Path + "/Save.txt");
        currentMap = JsonUtility.FromJson<Map>(_json);
        currentMap.SetOwner(this);
        RefreshAreaBinder();
        InitBackground();
        currentMap.Init();
    }
}
