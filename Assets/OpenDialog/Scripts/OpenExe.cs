using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
public class OpenExe : MonoBehaviour
{

    Button Btn;
    Button OkBtn;
    Text text;

    string path;

    private void Awake() {

        Btn = GameObject.Find("Open").GetComponent<Button>();
        OkBtn = GameObject.Find("OkBtn").GetComponent<Button>();
        text = GameObject.Find("Path").GetComponent<Text>();

        OkBtn.onClick.AddListener(() => {
            text.text = OpenProject();
            path = text.text.Replace("\\","/");
        });

        Btn.onClick.AddListener(() => {
            OpenExeApp(path, path.Substring(path.LastIndexOf("/")));
        });
    }



    void OpenExeApp(string exePathName,string exeArgs) {

        try {

            Process m_process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo(exePathName, exeArgs);
            m_process.StartInfo = processStartInfo;
            m_process.StartInfo.UseShellExecute = false;
            m_process.Start();

        } catch (Exception e) {
            UnityEngine.Debug.Log(e.Message);
            throw;
        }

    }

    public string OpenProject() {
        OpenFileDlg pth = new OpenFileDlg();
        pth.structSize = Marshal.SizeOf(pth);
        pth.filter = "exe";
        pth.file = new string(new char[256]);
        pth.maxFile = pth.file.Length;
        pth.fileTitle = new string(new char[64]);
        pth.maxFileTitle = pth.fileTitle.Length;
        pth.initialDir = Application.dataPath.Replace("/", "\\") + "\\Resources"; //默认路径
        pth.title = "打开项目";
        pth.defExt = "dat";
        pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        if (OpenFileDialog.GetOpenFileName(pth)) {
            string filepath = pth.file; //选择的文件路径;  
            UnityEngine.Debug.Log(filepath);
            return pth.file;
        } else {
            return null;
        }
    }

    /// <summary>
    /// 保存文件项目
    /// </summary>
    public void SaveProject() {
        SaveFileDlg pth = new SaveFileDlg();
        pth.structSize = Marshal.SizeOf(pth);
        pth.filter = "All files (*.*)|*.*";
        pth.file = new string(new char[256]);
        pth.maxFile = pth.file.Length;
        pth.fileTitle = new string(new char[64]);
        pth.maxFileTitle = pth.fileTitle.Length;
        pth.initialDir = Application.dataPath; //默认路径
        pth.title = "保存项目";
        pth.defExt = "dat";
        pth.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
        if (SaveFileDialog.GetSaveFileName(pth)) {
            string filepath = pth.file; //选择的文件路径;  
            UnityEngine.Debug.Log(filepath);
        }
    }

}
