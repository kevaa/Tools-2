using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class SnapperTool : EditorWindow
{
    SerializedProperty gridSizeProp;
    SerializedProperty clipProp;
    SerializedProperty handleColorProp;
    SerializedProperty handleLineThicknessProp;

    [SerializeField] float gridSize = 1f;

    [SerializeField] bool clip = false;
    [ColorUsageAttribute(false)]
    [SerializeField] Color handleColor = Color.black;
    [SerializeField] float handleLineThickness = 1f;



    float extent = 5f;
    SerializedObject so;
    [MenuItem("Tools/Snapper")]
    public static void OpenTheThing() => GetWindow<SnapperTool>("Snapper");

    private void OnEnable()
    {
        so = new SerializedObject(this);
        gridSizeProp = so.FindProperty("gridSize");
        clipProp = so.FindProperty("clip");
        handleColorProp = so.FindProperty("handleColor");
        handleLineThicknessProp = so.FindProperty("handleLineThickness");


        Selection.selectionChanged += Repaint;
        SceneView.duringSceneGui += DrawGrid;
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= Repaint;
        SceneView.duringSceneGui -= DrawGrid;
    }

    private void OnGUI()
    {
        so.Update();
        EditorGUILayout.PropertyField(gridSizeProp);
        gridSizeProp.floatValue = Mathf.Clamp(gridSizeProp.floatValue, .25f, 5f);
        EditorGUILayout.PropertyField(clipProp);

        EditorGUILayout.PropertyField(handleColorProp);
        EditorGUILayout.PropertyField(handleLineThicknessProp);
        handleLineThicknessProp.floatValue = Mathf.Clamp(handleLineThicknessProp.floatValue, 0, 5);
        so.ApplyModifiedProperties();

        using (new EditorGUI.DisabledScope(Selection.gameObjects.Length == 0))
        {
            if (GUILayout.Button("Snap Selection"))
            {
                SnapSelection();
            }
        }
    }

    void DrawGrid(SceneView view)
    {
        Handles.color = handleColor;
        if (Event.current.type == EventType.Repaint)
        {
            var gridOrigin = getSelectionAvgPos().Round();
            var farthestDistFromOrigin = GetMaxDistInSelection(gridOrigin);
            if (!clip)
            {
                Handles.zTest = CompareFunction.LessEqual;
            }
            DrawHorizontalGrid(farthestDistFromOrigin + extent, gridOrigin);

        }
    }

    void DrawHorizontalGrid(float gridDrawExtent, Vector3 origin)
    {

        var verticalP1Pos = new Vector3(origin.x, 0, origin.z - gridDrawExtent);
        var verticalP2Pos = new Vector3(origin.x, 0, origin.z + gridDrawExtent);
        var verticalP1Neg = new Vector3(origin.x, 0, origin.z - gridDrawExtent);
        var verticalP2Neg = new Vector3(origin.x, 0, origin.z + gridDrawExtent);
        var horizontalP1Pos = new Vector3(origin.x - gridDrawExtent, 0, origin.z);
        var horizontalP2Pos = new Vector3(origin.x + gridDrawExtent, 0, origin.z);
        var horizontalP1Neg = new Vector3(origin.x - gridDrawExtent, 0, origin.z);
        var horizontalP2Neg = new Vector3(origin.x + gridDrawExtent, 0, origin.z);

        while (verticalP1Pos.x <= origin.x + gridDrawExtent)
        {
            Handles.DrawLine(verticalP1Pos, verticalP2Pos, handleLineThickness);
            Handles.DrawLine(verticalP1Neg, verticalP2Neg, handleLineThickness);
            Handles.DrawLine(horizontalP1Pos, horizontalP2Pos, handleLineThickness);
            Handles.DrawLine(horizontalP1Neg, horizontalP2Neg, handleLineThickness);
            verticalP1Pos.x += gridSize;
            verticalP2Pos.x += gridSize;
            verticalP1Neg.x -= gridSize;
            verticalP2Neg.x -= gridSize;

            horizontalP1Pos.z += gridSize;
            horizontalP2Pos.z += gridSize;
            horizontalP1Neg.z -= gridSize;
            horizontalP2Neg.z -= gridSize;
        }

        var finalVerticalP1Neg = new Vector3(origin.x - gridDrawExtent, 0, origin.z + gridDrawExtent);
        var finalVerticalP2Neg = new Vector3(origin.x - gridDrawExtent, 0, origin.z - gridDrawExtent);
        var finalVerticalP1Pos = new Vector3(origin.x + gridDrawExtent, 0, origin.z + gridDrawExtent);
        var finalVerticalP2Pos = new Vector3(origin.x + gridDrawExtent, 0, origin.z - gridDrawExtent);
        var finalHorizontalP1Neg = new Vector3(origin.x - gridDrawExtent, 0, origin.z + gridDrawExtent);
        var finalHorizontalP2Neg = new Vector3(origin.x + gridDrawExtent, 0, origin.z + gridDrawExtent);
        var finalHorizontalP1Pos = new Vector3(origin.x - gridDrawExtent, 0, origin.z - gridDrawExtent);
        var finalHorizontalP2Pos = new Vector3(origin.x + gridDrawExtent, 0, origin.z - gridDrawExtent);
        Handles.DrawLine(finalVerticalP1Neg, finalVerticalP2Neg, handleLineThickness);
        Handles.DrawLine(finalVerticalP1Pos, finalVerticalP2Pos, handleLineThickness);
        Handles.DrawLine(finalHorizontalP1Neg, finalHorizontalP2Neg, handleLineThickness);
        Handles.DrawLine(finalHorizontalP1Pos, finalHorizontalP2Pos, handleLineThickness);

    }

    float GetMaxDistInSelection(Vector3 origin)
    {
        var maxDist = 0f;
        foreach (var go in Selection.gameObjects)
        {
            maxDist = Mathf.Max(maxDist, (go.transform.position - origin).magnitude);
        }
        return maxDist;
    }
    Vector3 getSelectionAvgPos()
    {
        var avgPos = Vector3.zero;
        foreach (var go in Selection.gameObjects)
        {
            avgPos += go.transform.position;
        }
        return avgPos / Selection.gameObjects.Length;
    }
    void SnapSelection()
    {
        foreach (var go in Selection.gameObjects)
        {
            // allows undo and redo and shows asterisk when changed by this menuitem
            Undo.RecordObject(go.transform, "Snap Selection");
            go.transform.position = go.transform.position.Round(gridSize);
        }
    }

}
