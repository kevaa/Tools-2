using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public static class Snapper
{
    const string UNDO_STR_SNAP = "snap objects";

    // menu item grayed out if this returns false
    [MenuItem("Edit/Snap selected Object %&S", isValidateFunction:true)]
    public static bool SnapTheThingsValidate()
    {
        return Selection.gameObjects.Length > 0;
    }

    // shortcut ctr alt s
    [MenuItem("Edit/Snap selected Object %&S")]
    public static void SnapTheThings()
    {
        foreach (var go in Selection.gameObjects)
        {
            // allows undo and redo and shows asterisk when changed by this menuitem
            Undo.RecordObject(go.transform, UNDO_STR_SNAP);
            go.transform.position = go.transform.position.Round();
        }
    }

}
