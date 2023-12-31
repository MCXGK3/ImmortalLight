﻿using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Satchel.Futils;
using UObject = UnityEngine.Object;
using Satchel;
using Galaxy.Api;

namespace ImmortalLight
{
    public class ImmortalLight : Mod,ITogglableMod,IMenuMod,IGlobalSettings<Setting>
    {
        internal static ImmortalLight Instance;

        public Setting setting=new();

        //public override List<ValueTuple<string, string>> GetPreloadNames()
        //{
        //    return new List<ValueTuple<string, string>>
        //    {
        //        new ValueTuple<string, string>("White_Palace_18", "White Palace Fly")
        //    };
        //}

        public ImmortalLight() : base("ImmortalLight")
        {
            Instance = this;
        }

        public override string GetVersion()
        {
            return "0.0.1.1";
        }

        public bool ToggleButtonInsideMenu => true;

        

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Log("Initializing");

            Instance = this;
            ModHooks.LanguageGetHook += ChangeLanguage;
            On.PlayMakerFSM.OnEnable += Modify;

            Log("Initialized");
        }

        private void Modify(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            try
            {
                if (self.FsmName == "Control" && self.gameObject.name == "Absolute Radiance")
                {
                    Log("Radiance");
                    Log(BossSequenceController.IsInSequence);
                    if (setting.inPantheon || !BossSequenceController.IsInSequence)
                    {
                        Log("Immortal");
                        self.gameObject.AddComponent<Immortal>();
                        Log("End");
                    }
                }
            }
            catch(Exception e) { Log(e); }
            orig(self);
        }

        private string ChangeLanguage(string key, string sheetTitle, string orig)
        {
            switch (key)
            {
                case "NAME_FINAL_BOSS": return OtherLanguage("永恒之光","ImmortalLight");
                case "ABSOLUTE_RADIANCE_SUPER": return OtherLanguage("直面","Confront");
                case "ABSOLUTE_RADIANCE_MAIN": return OtherLanguage("永恒之光", "ImmortalLight");
                case "GODSEEKER_RADIANCE_STATUE": return OtherLanguage("不会······消失······","won't......disappear......");
                case "GG_S_RADIANCE": return OtherLanguage("永远不变，也永远变化；永远熄灭，也永远在燃烧", "Changes never and changes forever; burns up and burns out");
                default: return orig;
            }
        }

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            var menus= new List<IMenuMod.MenuEntry>();
            if (toggleButtonEntry != null) { menus.Add(toggleButtonEntry.Value); }

                menus.Add(new IMenuMod.MenuEntry()
                {
                    Name = OtherLanguage("万神殿出现","In Pantheon?"),
                    Description = OtherLanguage("开启后，将在万神殿同样出现此mod","When enabled, ImmortalLight will appear in Pantheon."),
                    Values = new string[]
                {
                    Language.Language.Get("MOH_ON", "MainMenu"),
                    Language.Language.Get("MOH_OFF", "MainMenu")
                },
                    Loader = () => setting.inPantheon ? 0 : 1,
                    Saver = i => setting.inPantheon = i == 0

                }
                );
                menus.Add(new IMenuMod.MenuEntry()
                {
                    Name = OtherLanguage("梦语开关","Dream messages?"),
                    Description = OtherLanguage("关闭后，辐光p3将不再出现梦语（原本梦语不受影响）","When enabled, there will be some dream messages in climb phase."),
                    Values = new string[]
                    {
                    Language.Language.Get("MOH_ON", "MainMenu"),
                    Language.Language.Get("MOH_OFF", "MainMenu")
                    },
                    Loader = () => setting.performance ? 0 : 1,
                    Saver = i => setting.performance = i == 0

                }
                    );
                menus.Add(new IMenuMod.MenuEntry()
                {
                    Name = OtherLanguage("视野调整","Zoom Change"),
                    Description = OtherLanguage("开启后将在p3和p4调整视野大小","When enabled, the camera will zoom out in climb and final phase."),
                    Values = new string[]
                    {
                    Language.Language.Get("MOH_ON", "MainMenu"),
                    Language.Language.Get("MOH_OFF", "MainMenu")
                    },
                    Loader = () => setting.zoom ? 0 : 1,
                    Saver = i => setting.zoom = i == 0

                }
                    );
            

            return menus;
        }
        public void OnLoadGlobal(Setting s)
        {
            setting = s;
        }

        public Setting OnSaveGlobal()
        {
            return setting;
        }

        private string OtherLanguage(string chinese,string english)
        {
            if (Language.Language.CurrentLanguage() == Language.LanguageCode.ZH)
            {
                return chinese;
            }
            else return english;
        }
        public void Unload()
        {
            ModHooks.LanguageGetHook -= ChangeLanguage;
            On.PlayMakerFSM.OnEnable -= Modify;
        }
    }
}