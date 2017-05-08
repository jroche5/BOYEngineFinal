using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeExample : EditorWindow
{

    public List<Stitch> myStitches = new List<Stitch>();
    public List<NodeBaseClass> myNodes = new List<NodeBaseClass>();
    public Spool targetSpool;
    public string[] statusOptions = new string[] { "start", "end", "regular", "auto" };

    public bool editorToggle = false;
    public int nodeSelected = 0;

    public int nodeAttachID = -1;
    [MenuItem("Node Editor/ Editor")]
    public static void showWindow()
    {
        GetWindow<NodeExample>();
    }

    public void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        targetSpool = (Spool)EditorGUILayout.ObjectField(targetSpool, typeof(Spool), false);
        if (EditorGUI.EndChangeCheck())
        {
            myNodes.Clear();
            if (targetSpool != null)
            {
                for (int i = 0; i < targetSpool.stitchCollection.Length; i++)
                {
                    myNodes.Add(new StitchNode(new Rect(100 * i, 40, 100, 100), i));
                }
            }
        }

        if (targetSpool != null)
        {

            for (int i = 0; i < targetSpool.stitchCollection.Length; i++)
            {
                for (int j = 0; j < targetSpool.stitchCollection[i].yarns.Length; j++)
                {
                    if (targetSpool.stitchCollection[i].yarns[j].choiceStitch != null)
                        DrawNodeCurve(myNodes[i].rect, myNodes[targetSpool.stitchCollection[i].yarns[j].choiceStitch.stitchID].rect);
                }
            }
        }

        BeginWindows();
        for (int i = 0; i < myNodes.Count; i++)
        {
            myNodes[i].rect = GUI.Window(i, myNodes[i].rect, myNodes[i].DrawGUI, targetSpool.stitchCollection[i].stitchName);
        }
        EndWindows();
        if (GUI.Button(new Rect(0, 20, 40, 20), "ADD"))
        {
            AddNewStitch();
        }
        for (int i = 0; i < myNodes.Count; i++)
        {
            if (GUI.Button(new Rect(myNodes[i].rect.x + myNodes[i].rect.height - 70, myNodes[i].rect.yMax, 40, 20), "Edit"))
            {
                //Open Edit Window
                editorToggle = true;
                nodeSelected = i;
            }
            Color temp = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;

            if (GUI.Button(new Rect(myNodes[i].rect.x + 82, myNodes[i].rect.y - 18, 18, 18), "X"))
            {
                RemoveNode(myNodes[i].id);
            }
            GUI.backgroundColor = temp;

            /*if (GUI.Button(new Rect(myNodes[i].rect.xMax - 10, myNodes[i].rect.y + myNodes[i].rect.height / 2, 20, 20), "+"))
            {
                BeginAttachment(i);
            }

            if (GUI.Button(new Rect(myNodes[i].rect.xMin - 10, myNodes[i].rect.y + myNodes[i].rect.height / 2, 20, 20), "O"))
            {
                EndAttachment(i);
            }*/
        }

        if (editorToggle)
        {
            EditorGUIUtility.LookLikeControls(88f, (position.width / 3));
            Rect editorWindow = new Rect(position.width - position.width / 3, 0, position.width / 3 - 20, position.height);
            GUI.BeginGroup(editorWindow);
            EditorGUILayout.LabelField("Scene Editor");
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Stitch ID: " + targetSpool.stitchCollection[nodeSelected].stitchID);
            targetSpool.stitchCollection[nodeSelected].stitchName = EditorGUILayout.TextField("Scene Name: ", targetSpool.stitchCollection[nodeSelected].stitchName);
            EditorGUILayout.LabelField("Summary: ");
            targetSpool.stitchCollection[nodeSelected].summary = EditorGUILayout.TextArea(targetSpool.stitchCollection[nodeSelected].summary);
            EditorGUILayout.LabelField("Background: ");
            targetSpool.stitchCollection[nodeSelected].background = (Sprite)EditorGUILayout.ObjectField(targetSpool.stitchCollection[nodeSelected].background, typeof(Sprite), false);

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Performers: ");

            EditorGUILayout.Space();

            for (int i = 0; i < targetSpool.stitchCollection[nodeSelected].performers.Length; i++)
            {
                EditorGUILayout.BeginVertical();

                EditorGUILayout.LabelField("#" + (i + 1));
                targetSpool.stitchCollection[nodeSelected].performers[i].actorSprite = (ActorSprite)EditorGUILayout.ObjectField("Actor Sprite: ", targetSpool.stitchCollection[nodeSelected].performers[i].actorSprite, typeof(ActorSprite), false);
                targetSpool.stitchCollection[nodeSelected].performers[i].transform = (RectTransform)EditorGUILayout.ObjectField("Transform: ", targetSpool.stitchCollection[nodeSelected].performers[i].transform, typeof(RectTransform), false);
                targetSpool.stitchCollection[nodeSelected].performers[i].order = EditorGUILayout.IntField("Order: ", targetSpool.stitchCollection[nodeSelected].performers[i].order);

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Dialogs: ");

            EditorGUILayout.Space();

            for (int j = 0; j < targetSpool.stitchCollection[nodeSelected].dialogs.Length; j++)
            {
                EditorGUILayout.BeginVertical();

                EditorGUILayout.LabelField("#" + (j + 1));
                targetSpool.stitchCollection[nodeSelected].dialogs[j].nameShown = EditorGUILayout.TextField("Name Shown: ", targetSpool.stitchCollection[nodeSelected].dialogs[j].nameShown);
                targetSpool.stitchCollection[nodeSelected].dialogs[j].textShown = EditorGUILayout.TextField("Text Shown: ", targetSpool.stitchCollection[nodeSelected].dialogs[j].textShown);
            }

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Yarns: ");

            EditorGUILayout.Space();

            for (int j = 0; j < targetSpool.stitchCollection[nodeSelected].yarns.Length; j++)
            {
                EditorGUILayout.BeginVertical();

                EditorGUILayout.LabelField("#" + (j + 1));
                targetSpool.stitchCollection[nodeSelected].yarns[j].choiceStitch = (Stitch)EditorGUILayout.ObjectField("Choice Stitch: ", targetSpool.stitchCollection[nodeSelected].yarns[j].choiceStitch, typeof(Stitch), false);
                targetSpool.stitchCollection[nodeSelected].yarns[j].choiceString = EditorGUILayout.TextField("Choice String: ", targetSpool.stitchCollection[nodeSelected].yarns[j].choiceString);

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Popup(0, statusOptions);
            GUI.EndGroup();
            //EditorGUI.DrawRect(editorWindow, Color.black);
        }
    }

    public void RemoveNode(int id)
    {
        for (int i = 0; i < myNodes.Count; i++)
        {
            myNodes[i].linkedNodes.RemoveAll(item => item.id == id);
        }
        myNodes.RemoveAt(id);
        UpdateNodeIDs();

        Stitch[] newStitches = new Stitch[targetSpool.stitchCollection.Length - 1];
        int k = 0;
        for(int j = 0; j < newStitches.Length; j++)
        {
            if (k != id)
            {
                newStitches[j] = targetSpool.stitchCollection[k];
            }
            else
            {
                j--;
            }
            k++;
        }

        targetSpool.stitchCollection = newStitches;
    }

    public void UpdateNodeIDs()
    {
        for (int i = 0; i < myNodes.Count; i++)
        {
            myNodes[i].ReassignID(i);
        }
    }

    public void BeginAttachment(int winID)
    {
        nodeAttachID = winID;
    }

    public void EndAttachment(int winID)
    {
        if (nodeAttachID > -1)
        {
            myNodes[nodeAttachID].AttachComplete(myNodes[winID]);
        }
        nodeAttachID = -1;
    }

    public void AddNewStitch()
    {
        Stitch[] newStitch = new Stitch[targetSpool.stitchCollection.Length + 1];
        
        for(int i = 0; i < targetSpool.stitchCollection.Length; i++)
        {
            newStitch[i] = targetSpool.stitchCollection[i];
        }
        newStitch[targetSpool.stitchCollection.Length] = new Stitch();
        newStitch[targetSpool.stitchCollection.Length].stitchID = targetSpool.stitchCollection.Length;
        newStitch[targetSpool.stitchCollection.Length].stitchName = "Scene" + (targetSpool.stitchCollection.Length + 1).ToString();
        newStitch[targetSpool.stitchCollection.Length].summary = "";
        newStitch[targetSpool.stitchCollection.Length].performers = new Performer[1];
        newStitch[targetSpool.stitchCollection.Length].dialogs = new Dialog[1];
        newStitch[targetSpool.stitchCollection.Length].yarns = new Yarn[1];

        int sceneNum = (targetSpool.stitchCollection.Length + 1);

        AssetDatabase.CreateAsset(newStitch[targetSpool.stitchCollection.Length], "Assets/TestStory/Stitch" + sceneNum.ToString() + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        targetSpool.stitchCollection = newStitch;

        myNodes.Clear();
        if (targetSpool != null)
        {
            for (int i = 0; i < targetSpool.stitchCollection.Length; i++)
            {
                myNodes.Add(new StitchNode(new Rect(100 * i, 40, 100, 100), i));
            }
        }
    }

    void DrawNodeCurve(Rect start, Rect end)
    {
        Vector3 startPos = new Vector3(start.x + start.width, start.y + (start.height / 2) + 10, 0);
        Vector3 endPos = new Vector3(end.x, end.y + (end.height / 2) + 10, 0);
        Vector3 startTan = startPos + Vector3.right * 100;
        Vector3 endTan = endPos + Vector3.left * 100;
        Color shadowCol = new Color(0, 0, 0, 0.06f);


        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 5);
    }

}
