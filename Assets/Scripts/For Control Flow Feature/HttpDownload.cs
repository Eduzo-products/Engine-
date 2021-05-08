using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CI.HttpClient;
using System;
using System.Linq;
using System.IO;

public class HttpDownload  {


    public HttpClient client;
    public int downloadPercentage;

    //  public DownloadStatus myStatus;

    public Byte[] data;
    bool isDownloadOver;


    public void CreateNewClient(string url, Action<bool> iscompleted,string savePath) {


        isDownloadOver = false;
        downloadPercentage = 0;
       

      //  myStatus = DownloadStatus.Inprogress;

        client = new HttpClient();

        FileStream fileStream = File.Create(savePath);

        client.Get(new Uri(url), HttpCompletionOption.StreamResponseContent, (r) =>
        {
            if (r.IsSuccessStatusCode) {

             //   myStatus = DownloadStatus.Started;
                downloadPercentage = r.PercentageComplete;
               
                isDownloadOver = ((100 - downloadPercentage) == 0) ? true : false;

                using (Stream responseStream = r.ReadAsStream())
                {
                    byte[] bytesInStream = new byte[responseStream.Length];
                   
                    int read;
                    do
                    {
                        read = responseStream.Read(bytesInStream, 0, (int)bytesInStream.Length);
                    //   bytesInStream = AvoEx.AesEncryptor.Encrypt(bytesInStream);

                        if (read > 0)
                            fileStream.Write(bytesInStream, 0, read);
                    }
                    while (read > 0);
                }

             


                if (isDownloadOver) {
            //        myStatus = DownloadStatus.Completed;
                    fileStream.Close();
                    // data = AssertBundleSave.Instance.CombineBytes(bytesArrayList.ToArray());
                    iscompleted(true);
                }

            }
            else {
                Debug.Log("Connection failed");
         //       myStatus = DownloadStatus.Failed;
                iscompleted(false);
            }

        });
    }

    public void CreateNewClientJsonFile(string url, Action<bool> iscompleted)
    {
        List<Byte[]> bytesArrayList = new List<byte[]>();
        isDownloadOver = false;
        downloadPercentage = 0;
        string jsonString = "";
       // myStatus = DownloadStatus.Inprogress;

        client = new HttpClient();

        client.Get(new System.Uri(url), HttpCompletionOption.AllResponseContent, (r) =>
        {
            if (r.IsSuccessStatusCode)
            {
       //         myStatus = DownloadStatus.Started;
                downloadPercentage = r.PercentageComplete;
                bytesArrayList.Add(r.ReadAsByteArray());

                jsonString = r.ReadAsString(System.Text.Encoding.UTF8);
                           Debug.Log("Json string : " + jsonString);

                isDownloadOver = (((100 - downloadPercentage) == 0) ? true : false);

                if (isDownloadOver)
                {
        //            myStatus = DownloadStatus.Completed;
                    data = CombineBytes(bytesArrayList.ToArray());
                    iscompleted(true);
                }
            }
            else
            {
                Debug.Log("Connection failed");
      //          myStatus = DownloadStatus.Failed;
                iscompleted(false);
            }

        });
    }
    public byte[] CombineBytes(params byte[][] arrays)
    {
        byte[] rv = new byte[arrays.Sum(a => a.Length)];
        int offset = 0;
        foreach (byte[] array in arrays)
        {
            Buffer.BlockCopy(array, 0, rv, offset, array.Length);
            offset += array.Length;
        }
        return rv;
    }
}




public class AssertBundleData
{

    public HttpDownload downloadHttp;

    public bool isJsonDownloaded;

    private int downloadPercentage;


    byte[] data;

    public int DownloadPercentage
    {
        get
        {
            return downloadHttp.downloadPercentage;
        }
    }


    public byte[] Data
    {
        get
        {
            return downloadHttp.data;
        }
    }


  
}

