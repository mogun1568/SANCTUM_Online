using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MultiplayersBuildAndRun
{
    [MenuItem("Tools/Run Muitiplayer/1 players")]
    static void PerformWin64Build1()
    {
        PerformWin64Build(1);
    }
    [MenuItem("Tools/Run Muitiplayer/2 players")]
    static void PerformWin64Build2()
    {
        PerformWin64Build(2);
    }
    [MenuItem("Tools/Run Muitiplayer/3 players")]
    static void PerformWin64Build3()
    {
        PerformWin64Build(3);
    }
    [MenuItem("Tools/Run Muitiplayer/4 players")]
    static void PerformWin64Build4()
    {
        PerformWin64Build(4);
    }

    static void PerformWin64Build(int playerCount)
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);

        for (int i = 1; i <= playerCount; i++)
        {
            BuildPipeline.BuildPlayer(GetScenePaths(),
                "Builds/Win64/" + GetProjectName() + i.ToString() + "/" + GetProjectName() + i.ToString() + ".exe",
                BuildTarget.StandaloneWindows64, BuildOptions.AutoRunPlayer);
        }
    }

    static string GetProjectName()
    {
        string[] s = Application.dataPath.Split('/');
        return s[s.Length - 2];
    }

    static string[] GetScenePaths()
    {
        string[] scenes = new string[2];

        //for (int i = 0; i < scenes.Length; i++)
        //{
        //    scenes[i] = EditorBuildSettings.scenes[i].path;
        //}
        scenes[0] = EditorBuildSettings.scenes[2].path;
        scenes[1] = EditorBuildSettings.scenes[3].path;

        return scenes;
    }
}
