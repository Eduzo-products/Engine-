using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

public class AssetBundleLoad : MonoBehaviour {


    
  
    public static MemoryStream previousLoadedBunlde;

   
    //public string FetchJsonData (String pathDirectory) {
    //    return File.ReadAllText(pathDirectory);
    //}

    //public AssetBundle FetchFileFromLocal(String pathDirectory) {

    //    previousLoadedBunlde = AssetBundle.LoadFromFile(pathDirectory);
    //    return previousLoadedBunlde;
    //}

    public IEnumerator FetchFileFromLocal(string pathDirectory, Action<MemoryStream> callback = null) 
    {
                

        FileStream fileStram = new FileStream(pathDirectory, FileMode.Open, FileAccess.Read);
        long whereToStartReading = 0;
        long totalBytesToRead = fileStram.Length;
        int index = 0;

        using (fileStram)
        {
            int defByteCount = 10000;

            int byteCount = defByteCount;

            index++;
            byte[] buffer = new byte[byteCount];
            fileStram.Seek(whereToStartReading, SeekOrigin.Begin);
            int bytesRead = fileStram.Read(buffer, 0, byteCount);
           
            MemoryStream memoryStream = new MemoryStream();
           
            while (bytesRead > 0)
            {

                memoryStream.Write(buffer, 0, byteCount);
                if (((fileStram.Length - (index * defByteCount)) / defByteCount) == 0)
                {

                    byteCount = (int)(fileStram.Length % defByteCount);
                    Debug.Log("Last");
                }
                else
                {
                    byteCount = defByteCount;
                }
                bytesRead = fileStram.Read(buffer, 0, byteCount);
                index++;
                
            }
            whereToStartReading += buffer.Length;

          
            previousLoadedBunlde = memoryStream;

            yield return null;
            
            callback(memoryStream);
        }

        #region encode & decode
        //  using (FileStream fsSource = new FileStream(pathDirectory,
        //FileMode.Open, FileAccess.Read))
        //  {

        //      // Read the source file into a byte array.
        //      byte[] bytes = new byte[fsSource.Length];
        //      int numBytesToRead = (int)fsSource.Length;
        //      int numBytesRead = 0;
        //      while (numBytesToRead > 0)
        //      {
        //          // Read may return anything from 0 to numBytesToRead.
        //          int n = fsSource.Read(bytes, numBytesRead, numBytesToRead);

        //          // Break when the end of the file is reached.
        //          if (n == 0)
        //              break;

        //          numBytesRead += n;
        //          numBytesToRead -= n;
        //      }
        //      numBytesToRead = bytes.Length;

        //      // Write the byte array to the other FileStream.
        //      using (FileStream fsNew = new FileStream(pathDirectory,
        //          FileMode.Create, FileAccess.Write))
        //      {
        //          fsNew.Write(bytes, 0, numBytesToRead);
        //      }
        //  }


        //var fileStream = new FileStream(pathDirectory, FileMode.Open, FileAccess.Read);
        //AssetBundleCreateRequest createRequest = AssetBundle.LoadFromStreamAsync(fileStream);
        //while (!createRequest.isDone) { yield return null; }
        //previousLoadedBunlde = createRequest.assetBundle;
        //callback(createRequest.assetBundle);

        #endregion

    }

    public IEnumerator FetchFileFromLocal(string pathDirectory, Action<string> callback = null)
    {

        byte[] downloadedBytes = File.ReadAllBytes(pathDirectory);
       
        callback(Encoding.UTF8.GetString(downloadedBytes));
       
        yield return null;

    }


}
