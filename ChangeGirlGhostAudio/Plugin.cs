using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections;
using System.Net;
using System.Net.Http;
using UnityEngine;
using UnityEngine.Networking;

namespace ToxicStar.ChangeGirlGhostAudio
{
    [BepInPlugin("ToxicStar.ChangeGirlGhostAudio", "ChangeGirlGhostAudio", "1.0.0.0")]
    public class ToxicStarPlugin : BaseUnityPlugin
    {
        private const string modGUID = "ToxicStar.ChangeGirlGhostAudio";

        private const string modName = "ChangeGirlGhostAudio";

        private const string modVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony("ToxicStar.ChangeGirlGhostAudio");

        private static ToxicStarPlugin Instance;

        internal static AudioClip[] newSFX;

        private void Awake()
        {
            if ((Object)(object)Instance == (Object)null)
            {
                Instance = this;
            }
            StartCoroutine(LoadAudio());
        }

        private IEnumerator LoadAudio()
        {
            //寻找路径并加载音频到内存
            var path = $"{Application.dataPath.Substring(0, Application.dataPath.Length - 20)}/BepInEx/plugins/{modGUID}/WinnerWinnerChickenDinner.mp3";
            Logger.LogInfo("音频存放路径：" + path);
            var uwr = UnityWebRequestMultimedia.GetAudioClip(Application.dataPath + "/WinnerWinnerChickenDinner.mp3", AudioType.MPEG);
            yield return uwr.SendWebRequest();
            //转换
            var audioClip = DownloadHandlerAudioClip.GetContent(uwr);
            if (audioClip == null)
            {
                Logger.LogError("加载音频失败");
                yield break;
            }
            //???
            newSFX = new AudioClip[1] { audioClip };
            Logger.LogInfo(newSFX.Length);
            Logger.LogInfo(newSFX[0].name);
            Logger.LogInfo(newSFX[0].ToString());
            harmony.PatchAll(typeof(GhostGirlAudioPatch));
            harmony.PatchAll(typeof(ToxicStarPlugin));
        }
    }

    [HarmonyPatch(typeof(DressGirlAI))]
    internal class GhostGirlAudioPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        private static void patchM(ref AudioClip breathingSFX)
        {
            AudioClip[] newSFX = ToxicStarPlugin.newSFX;
            if (newSFX != null && newSFX.Length >= 0)
            {
                AudioClip val = newSFX[0];
                breathingSFX = val;
            }
        }
    }
}