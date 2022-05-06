// using UnityEngine;
// using System.Collections.Generic;
// using UnityEngine.UI;
// using UnityEditor;

// [CustomEditor(typeof(Language))]
// public class CustomLanguageEditor : Editor
// {
//     Language lang;
//     private void OnEnable()
//     {
//         lang = (Language)target;
//     }
//     public override void OnInspectorGUI()
//     {
//         DrawDefaultInspector();
//         var termsString = "\nTerms:\n";
//         foreach (var pair in lang.terms)
//         {
//             termsString += pair.Key + ": " + pair.Value;
//         }
//         EditorGUILayout.LabelField(termsString);
//     }
// }