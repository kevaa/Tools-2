using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Linq;
using System.IO;
public class LanguageManager : EditorWindow
{

    List<string> languageNames = new List<string>();
    int selectedLanguageInd = 0;
    int selectedTermInd = 0;
    int languageToRemoveInd = 0;
    int removeTermInd = 0;
    int languageToModifyInd = 0;
    [SerializeField] string languageToAdd;
    [SerializeField] string addTerm;
    [SerializeField] string setTerm;

    Language globalLanguage;
    List<Language> languages = new List<Language>();
    List<string> terms = new List<string>();
    string termsOfSelectedLanguage = "";

    [MenuItem("Tools/Language Manager")]
    public static void OpenMenuItem() => GetWindow<LanguageManager>("LanguageManager");

    private void OnEnable()
    {
        string[] languageFiles = Directory.GetFiles(Application.dataPath + "\\Languages", "*.asset", SearchOption.AllDirectories);
        foreach (string languageFile in languageFiles)
        {
            string path = "Assets" + languageFile.Replace(Application.dataPath, "").Replace('\\', '/');
            languages.Add((Language)AssetDatabase.LoadAssetAtPath(path, typeof(Language)));
        }
        foreach (Language language in languages)
        {
            languageNames.Add(language.language);
        }
        if (languages.Count > 0)
        {
            foreach (var key in languages[0].keys)
            {
                terms.Add(key);
            }
        }
        globalLanguage = (Language)AssetDatabase.LoadAssetAtPath("Assets/GlobalLanguage.asset", typeof(Language));
        if (languageNames.Contains(globalLanguage.language))
        {
            selectedLanguageInd = languageNames.IndexOf(globalLanguage.language);
        }
        else
        {
            if (languageNames.Count > 0)
            {
                SetGlobalLanguage();
            }
            else
            {
                ResetGlobalLanguage();
            }
        }
    }

    private void OnGUI()
    {
        var newSelectedLanguageInd = EditorGUILayout.Popup("Game Language", selectedLanguageInd, languageNames.ToArray());
        if (newSelectedLanguageInd != selectedLanguageInd)
        {
            selectedLanguageInd = newSelectedLanguageInd;
            SetGlobalLanguage();
        }
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Languages", EditorStyles.boldLabel);
        AddLanguageGUI();
        RemoveLanguageGUI();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Terms", EditorStyles.boldLabel);
        AddTermGUI();
        languageToModifyInd = EditorGUILayout.Popup("Language", languageToModifyInd, languageNames.ToArray());
        SetTermGUI();
        RemoveTermGUI();

    }
    void AddLanguageGUI()
    {
        EditorGUILayout.LabelField("Add Language");
        EditorGUILayout.BeginHorizontal();
        languageToAdd = EditorGUILayout.TextField(languageToAdd);
        using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(languageToAdd)))
        {
            if (GUILayout.Button("Add"))
            {
                AddLanguage();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    void RemoveLanguageGUI()
    {
        EditorGUILayout.LabelField("Remove Language");
        EditorGUILayout.BeginHorizontal();
        languageToRemoveInd = EditorGUILayout.Popup(languageToRemoveInd, languageNames.ToArray());
        using (new EditorGUI.DisabledScope(languageNames.Count == 0))
        {
            if (GUILayout.Button("Remove"))
            {
                RemoveLanguage();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    void AddTermGUI()
    {
        EditorGUILayout.LabelField("Add Term");
        EditorGUILayout.BeginHorizontal();
        addTerm = EditorGUILayout.TextField(addTerm);
        using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(addTerm)))
        {
            if (GUILayout.Button("Add"))
            {
                AddTerm();
            }
        }
        EditorGUILayout.EndHorizontal();
    }
    void SetGlobalLanguage()
    {
        globalLanguage.keys = new List<string>(languages[selectedLanguageInd].keys);
        globalLanguage.values = new List<string>(languages[selectedLanguageInd].values);
        globalLanguage.language = languages[selectedLanguageInd].language;
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    void AddLanguage()
    {
        var newLanguage = ScriptableObject.CreateInstance<Language>();
        foreach (var term in terms)
        {
            newLanguage.Set(term, "");
        }
        newLanguage.language = languageToAdd;
        languageNames.Add(languageToAdd);
        languages.Add(newLanguage);
        AssetDatabase.CreateAsset(newLanguage, string.Format("Assets/Languages/{0}.asset", languageToAdd));
        SetGlobalLanguage();
    }
    void SetTermGUI()
    {
        EditorGUILayout.LabelField("Set Term");
        EditorGUILayout.BeginHorizontal();
        selectedTermInd = EditorGUILayout.Popup(selectedTermInd, terms.ToArray());
        setTerm = EditorGUILayout.TextField(setTerm);
        using (new EditorGUI.DisabledScope(string.IsNullOrEmpty(setTerm) || terms.Count == 0))
        {
            if (GUILayout.Button("Set"))
            {
                SetTerm();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    void RemoveTermGUI()
    {
        EditorGUILayout.LabelField("Remove Term");
        EditorGUILayout.BeginHorizontal();
        removeTermInd = EditorGUILayout.Popup(removeTermInd, terms.ToArray());
        using (new EditorGUI.DisabledScope(terms.Count == 0))
        {
            if (GUILayout.Button("Remove"))
            {
                RemoveTerm();
            }
        }
        EditorGUILayout.EndHorizontal();
    }
    void RemoveLanguage()
    {
        var languageToRemove = languageNames.ElementAt(languageToRemoveInd);

        AssetDatabase.DeleteAsset(string.Format("Assets/Languages/{0}.asset", languageNames.ElementAt(languageToRemoveInd)));
        languageNames.RemoveAt(languageToRemoveInd);
        languages.RemoveAt(languageToRemoveInd);
        if (globalLanguage.language.Equals(languageToRemove))
        {
            if (languages.Count > 0)
            {
                SetGlobalLanguage();
            }
            else
            {
                ResetGlobalLanguage();
            }
        }
    }

    void ResetGlobalLanguage()
    {
        globalLanguage.language = "";
        globalLanguage.ResetTerms();
    }
    void AddTerm()
    {
        terms.Add(addTerm);
        foreach (var language in languages)
        {
            if (!language.ContainsKey(addTerm))
            {
                language.Set(addTerm, "");
            }
        }
        if (!globalLanguage.ContainsKey(addTerm))
        {
            globalLanguage.Set(addTerm, "");
        }
    }

    void RemoveTerm()
    {
        var termToRemove = terms[removeTermInd];
        terms.RemoveAt(removeTermInd);
        foreach (var language in languages)
        {
            if (language.ContainsKey(termToRemove))
            {
                language.Remove(termToRemove);
            }
        }
        if (globalLanguage.ContainsKey(termToRemove))
        {
            globalLanguage.Remove(termToRemove);
        }
    }

    void SetTerm()
    {
        var term = terms[selectedTermInd];
        languages[languageToModifyInd].Set(term, setTerm);
        EditorUtility.SetDirty(languages[languageToModifyInd]);
        if (globalLanguage.language.Equals(languageNames[languageToModifyInd]))
        {
            globalLanguage.Set(term, setTerm);
        }

    }
}
