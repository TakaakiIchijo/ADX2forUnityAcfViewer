using System;
using System.Collections.Generic;
using System.Linq;
using ACFDataClass;
using UnityEditor;
using UnityEngine;

public class CriAtomAcfViewerWindow : EditorWindow
{
    const string acfScriptableObjectPath = "Assets/Editor/CriWare/CriAtom/acfDataSo.asset";
    
    private Vector2 scrollPos_Window, scrollPosCategory, scroppPosAisacControl;
    
    private int selectedInfoIndex;
    
    bool hideDefaultAisacControls = true;
    
    private AcfDataSo acfDataSo;
    
    [MenuItem("Window/CRIWARE/Open Acf Viewer Window", false, 100)]
    static void OpenWindow()
    {
        GetWindow<CriAtomAcfViewerWindow>("Acf Viewer");
    }

    private void OnEnable()
    {
        acfDataSo = AssetDatabase.LoadAssetAtPath<AcfDataSo>(acfScriptableObjectPath);

        if (acfDataSo == null)
        {
            acfDataSo = CreateInstance<AcfDataSo>();
            AssetDatabase.CreateAsset(acfDataSo, acfScriptableObjectPath);
            AssetDatabase.Refresh();
        }
    }
    public void OnGUI()
    { 
        DrawSearchButton();

        if (acfDataSo == null) return;
        
        this.scrollPos_Window = GUILayout.BeginScrollView(this.scrollPos_Window);
        {
            DrawCategoryItemNames();

            float categoryHeight = this.position.height - 300.0f;
            if (categoryHeight < 100.0f) categoryHeight = 100.0f;
            scrollPosCategory = EditorGUILayout.BeginScrollView(scrollPosCategory, GUILayout.Height(categoryHeight));
            
            DrawCategoryList();
            
            EditorGUILayout.EndScrollView();
            
            DrawAisacControlItemNames();

            float aisacControlHeight = this.position.height - 300.0f;
            if (aisacControlHeight < 100.0f) aisacControlHeight = 100.0f;
            
            scroppPosAisacControl = EditorGUILayout.BeginScrollView(scroppPosAisacControl);
            
            DrawAisacControlList();
            
            EditorGUILayout.EndScrollView();
        }
        
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(2));

        hideDefaultAisacControls = GUILayout.Toggle(hideDefaultAisacControls, "Hide default name AISAC controls");

        GUILayout.Space(10);
        EditorGUILayout.EndScrollView();
    }
    
    private void DrawSearchButton()
    {
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Load ACF data from runtime", GUILayout.Width(200), GUILayout.Height(50)))
            {
                if (!EditorApplication.isPlaying)
                {
                    Debug.LogError("Please load acf data in editor playing");
                }
                else
                {
                    acfDataSo = AssetDatabase.LoadAssetAtPath<AcfDataSo>(acfScriptableObjectPath);
                    
                    acfDataSo.categoryInfoList = LoadCategory();
                    acfDataSo.aisacControlInfoList = LoadAisacControlList();
                    
                    EditorUtility.SetDirty(acfDataSo);
                    AssetDatabase.SaveAssets();
                }
            }
            
            EditorGUILayout.BeginVertical ();
            
            GUILayout.Space(8f);
            GUILayout.Label("Use the load button when the game playing");
            
            GUILayout.Space(5f);
            GUILayout.Label("Click each element copies to clipboard");
            
            EditorGUILayout.EndVertical ();
        }
        GUILayout.EndHorizontal();
    }
    private List<CategoryInfo> LoadCategory()
    {
        List<CategoryInfo> categoryInfoList = new List<CategoryInfo>();
        
        for (ushort i = 0; i < CriAtomExAcfDebug.GetNumCategories(); i++)
        {
            CriAtomExAcfDebug.GetCategoryInfoByIndex(i, out var categoryInfo);
            
            categoryInfoList.Add(new CategoryInfo(
                categoryInfo.groupNo,
                categoryInfo.id, 
                categoryInfo.name,
                categoryInfo.numCueLimits,
                categoryInfo.volume));
        }

        return categoryInfoList;
    }
    private List<AisacControlInfo> LoadAisacControlList()
    {
        List<AisacControlInfo> aisacControlInfoList =new List<AisacControlInfo>();
        
        for (ushort i = 0; i < CriAtomExAcfDebug.GetNumAisacControls(); i++)
        {
            CriAtomExAcfDebug.GetAisacControlInfo(i, out var aisacControlInfo);

            aisacControlInfoList.Add(new AisacControlInfo(
                aisacControlInfo.name,
                aisacControlInfo.id));
        }

        aisacControlInfoList.Sort((a,b) => (int)a.id - (int)b.id);

        return aisacControlInfoList;
    }
    private void DrawCategoryItemNames()
    {
        List<CategoryInfo> categoryInfoList = acfDataSo.categoryInfoList;
        
        GUILayout.BeginHorizontal();
        {
            GUIStyle style = new GUIStyle(EditorStyles.miniButtonMid);
            style.alignment = TextAnchor.LowerLeft;
            
           if(GUILayout.Button("Category Name", style))
           {
               categoryInfoList.Sort((a,b) => String.CompareOrdinal(a.name, b.name));
           }

           if(GUILayout.Button("Volume", style, GUILayout.Width(80)))
            {
                categoryInfoList.Sort((a,b) => (int)a.volume - (int)b.volume);
            }

            
            if(GUILayout.Button("Cue Limit", style, GUILayout.Width(80)))
            {
                categoryInfoList.Sort((a,b) => (int)a.numCueLimits - (int)b.numCueLimits);
            }

            
            if(GUILayout.Button("Id", style, GUILayout.Width(80)))
            {
                categoryInfoList.Sort((a,b) => (int)a.id - (int)b.id);
            }
            
            if(GUILayout.Button("Group No", style, GUILayout.Width(80)))
            {
                categoryInfoList.Sort((a,b) => (int)a.groupNo - (int)b.groupNo);
            }

        }
        GUILayout.EndHorizontal();
    }
    private void DrawCategoryList()
    {
        List<CategoryInfo> categoryInfoList = acfDataSo.categoryInfoList;
        
        for(int i = 0; i < categoryInfoList.Count; ++i) {
            EditorGUILayout.BeginHorizontal();
            if (this.selectedInfoIndex == i) {
                GUI.color = Color.yellow;
            } else {
                GUI.color = Color.white;
            }

            var categoryName = categoryInfoList[i].name;
            
            if (GUILayout.Button(categoryName, EditorStyles.radioButton)) {
                
                EditorGUIUtility.systemCopyBuffer = categoryName;
                this.selectedInfoIndex = i;

                Debug.Log("Saved to clipboard " + "\""+categoryName+ "\"");
            }
            
            GUILayout.Label(categoryInfoList[i].volume.ToString(), GUILayout.Width(75));

            string numCueLimits = categoryInfoList[i].numCueLimits == UInt32.MaxValue
                ? "Unlimited"
                : categoryInfoList[i].numCueLimits.ToString();
            
            GUILayout.Label(numCueLimits, GUILayout.Width(75));

            GUILayout.Label(categoryInfoList[i].id.ToString(), GUILayout.Width(75));
            GUILayout.Label(categoryInfoList[i].groupNo.ToString(), GUILayout.Width(70));
            
            EditorGUILayout.EndHorizontal();
            
        }
        GUI.color = Color.white;
    }
    private void DrawAisacControlItemNames()
    {
        List<AisacControlInfo> aisacControlInfoList = acfDataSo.aisacControlInfoList;
        
        GUILayout.BeginHorizontal();
        {
            GUIStyle style = new GUIStyle(EditorStyles.miniButtonMid);
            style.alignment = TextAnchor.LowerLeft;

            if (GUILayout.Button("Aisac Control Name", style))
            {
                aisacControlInfoList.Sort((a,b) => String.CompareOrdinal(a.name, b.name));
            }

            if (GUILayout.Button("ID", style, GUILayout.Width(80)))
            {
                aisacControlInfoList.Sort((a,b) => (int)a.id - (int)b.id);
            }
        }
        GUILayout.EndHorizontal();
    }
    private void DrawAisacControlList()
    {
        List<AisacControlInfo> list = acfDataSo.aisacControlInfoList;
        
        if (hideDefaultAisacControls)
        {
            list = list.Where(a => a.name.Contains("AisacControl_") == false).ToList();
        }
        
        for(int i = 0; i < list.Count; ++i) {
            EditorGUILayout.BeginHorizontal();
            if (this.selectedInfoIndex == i) {
                GUI.color = Color.yellow;
            } else {
                GUI.color = Color.white;
            }

            var aisacName = list[i].name;
            
            if (GUILayout.Button(aisacName, EditorStyles.radioButton)) {
                
                EditorGUIUtility.systemCopyBuffer = aisacName;
                this.selectedInfoIndex = i;

                Debug.Log("Saved to clipboard " + "\""+aisacName+ "\"");
            }
            
            GUILayout.Label(list[i].id.ToString(), GUILayout.Width(70));
            
            EditorGUILayout.EndHorizontal();
        }
        GUI.color = Color.white;
    }
}
