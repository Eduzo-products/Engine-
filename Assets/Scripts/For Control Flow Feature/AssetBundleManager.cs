using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AssetBundleManager : MonoBehaviour
{

    public void TopicDeleteButton(string directory)
    {
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }
    }

   


    //public void SaveTopic_AssertBundle_Version(string topicName, int topicId, float currentVersioncode)
    //{

    //    string key = GetAssertPlayerPrefsKey(topicName, topicId);

    //    PlayerPrefs.SetFloat(key, currentVersioncode);

    //    Debug.Log(topicName + ":Bundle Version Saved. Bundle Version Code " + currentVersioncode);
    //}


    public string GetAssertPlayerPrefsKey(string topicName, int topicId)
    {
        return topicName + "_" + topicId;
    }


    public string GetDowloadContent_MainDirectory () {

        return Application.persistentDataPath +"/TopicFiles";
    }


    public void Delete_DowloadContent_MainDirectory ()
    {
        if(Directory.Exists(GetDowloadContent_MainDirectory()))
        {
            Directory.Delete(GetDowloadContent_MainDirectory(),true);
        }
    }

    public bool CheckAssertsInLocal(string directory, string[] fileName)
    {

        bool allAssetsISThere = false;
        int count = 0;

        string topicName = fileName[0].Split(new char[] { '.' })[0];

     
        if (File.Exists(directory + "/" + topicName + ".gd"))
        {
            for (int i = 0; i < fileName.Length; i++)
            {

                string path = directory + fileName[i];
                if (File.Exists(path))
                {
                    count++;
                }
            }

            if (count == 2)
                allAssetsISThere = true;
        }
        else
        {
            allAssetsISThere = false;
        }

//        Debug.Log(allAssetsISThere);

        return allAssetsISThere;

    }

    public string GetDownloadedScene(AssetBundle bundle)
    {
        if (bundle.isStreamedSceneAssetBundle)
        {
            string[] scenePaths = bundle.GetAllScenePaths();
            return Path.GetFileNameWithoutExtension(scenePaths[0]);
        }
        return null;
    }


}
