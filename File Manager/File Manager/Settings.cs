using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace File_Manager {
    public class Settings {
        public bool AskDeleteConfirm;
        public bool FindReplaceFolders;
        public bool RemoveBrackets;
        public bool RemoveBracketsNonfilters;
        public bool RemoveUnwanted;
        public bool ReplaceCheckedByDefault;
        public bool FindUsesRegexByDefault;
        public bool AutoOrganiseOnLoad;
        public bool AutoRenameOnLoad;
        public bool AutoRenameOnQuickSetting;
        public TVClass TV;
        public ErrorClass Error;
        public WarningClass Warning;
        public MKVClass MKV;
        public FormsClass Forms;

        public Settings() {
            TV = new TVClass();
            Error = new ErrorClass();
            Warning = new WarningClass();
            MKV = new MKVClass();
            ResetToDefault(true);
        }

        public void ResetToDefault(bool resetAll) {
            if (resetAll) {
                TV.ResetToDefault();
                Error.ResetToDefault();
                Warning.ResetToDefault();
                MKV.ResetToDefault();
                //Forms.ResetToDefault();
            }
            FindReplaceFolders = false;
            RemoveBrackets = true;
            RemoveBracketsNonfilters = true;
            RemoveUnwanted = true;
            AskDeleteConfirm = true;
            ReplaceCheckedByDefault = true;
            FindUsesRegexByDefault = false;
            AutoOrganiseOnLoad = true;
            AutoRenameOnLoad = true;
            AutoRenameOnQuickSetting = true;
        }

        public bool LoadSettings(TextReader tr) {
            try {
                if (!tr.ReadLine().StartsWith("#"))
                    return false;
                AskDeleteConfirm = Convert.ToBoolean(tr.ReadLine().After("="));
                FindReplaceFolders = Convert.ToBoolean(tr.ReadLine().After("="));
                RemoveBrackets = Convert.ToBoolean(tr.ReadLine().After("="));
                RemoveBracketsNonfilters = Convert.ToBoolean(tr.ReadLine().After("="));
                RemoveUnwanted = Convert.ToBoolean(tr.ReadLine().After("="));
                ReplaceCheckedByDefault = Convert.ToBoolean(tr.ReadLine().After("="));
                FindUsesRegexByDefault = Convert.ToBoolean(tr.ReadLine().After("="));
                AutoOrganiseOnLoad = Convert.ToBoolean(tr.ReadLine().After("="));
                AutoRenameOnLoad = Convert.ToBoolean(tr.ReadLine().After("="));
                AutoRenameOnQuickSetting = Convert.ToBoolean(tr.ReadLine().After("="));
            } catch (Exception) {
                return false;
            }
            if(TV.LoadSettings(tr) && Warning.LoadSettings(tr) && Error.LoadSettings(tr) && MKV.LoadSettings(tr))
                return true;
            return false;
        }
        
        public bool SaveSettings(TextWriter tw) {
            try {
                tw.WriteLine("#You can edit this file if you are careful, but do not change the order of any values or add any new lines.");
                tw.WriteLine("AskDeleteConfirm="+AskDeleteConfirm.ToString());
                tw.WriteLine("FindReplaceFolders=" + FindReplaceFolders.ToString());
                tw.WriteLine("RemoveBrackets=" + RemoveBrackets.ToString());
                tw.WriteLine("RemoveBracketsNonfilters=" + RemoveBracketsNonfilters.ToString());
                tw.WriteLine("RemoveUnwanted=" + RemoveUnwanted.ToString());
                tw.WriteLine("ReplaceCheckedByDefault=" + ReplaceCheckedByDefault.ToString());
                tw.WriteLine("FindUsesRegexByDefault=" + FindUsesRegexByDefault.ToString());
                tw.WriteLine("AutoOrganiseOnLoad=" + AutoOrganiseOnLoad.ToString());
                tw.WriteLine("AutoRenameOnLoad=" + AutoRenameOnLoad.ToString());
                tw.WriteLine("AutoRenameOnQuickSetting=" + AutoRenameOnQuickSetting.ToString());
            }
            catch(Exception) {
                return false;
            }
            if(TV.SaveSettings(tw) && Warning.SaveSettings(tw) && Error.SaveSettings(tw) && MKV.SaveSettings(tw))
                return true;
            return false;
        }

        public class TVClass {
            private string[] whiteSpaces = { " ", ".", "_", "-" };
            public string SpaceOption;
            public string Season;
            public string Episode;
            public string Double;
            public string TitleSeparator;
            public string SeriesSeparator;
            public bool GuessSeries;
            public bool ForceGuessSeries;
            public bool UseSeries;
            public bool UseTitle;
            public bool OnlyEpisode;
            public bool FormatEpisode;
            public int SeasonDigits;
            public int EpisodeDigits;

            public void ResetToDefault() {
                SpaceOption = " "; //can be changed to . or _ or -
                Episode = "E";
                Season = "S";
                Double = SpaceOption + "&" + SpaceOption + Episode;
                GuessSeries = true;
                ForceGuessSeries = false;
                UseSeries = true;
                UseTitle = true;
                OnlyEpisode = false;
                TitleSeparator = SpaceOption + "-" + SpaceOption;
                SeriesSeparator = SpaceOption;
                FormatEpisode = true;
                EpisodeDigits = 2;
                SeasonDigits = 2;
                
            }

            public bool LoadSettings(TextReader tr) {
                try {
                    SpaceOption = whiteSpaces[Int32.Parse(tr.ReadLine().After("="))];
                    Season = tr.ReadLine().After("=");
                    Episode = tr.ReadLine().After("=");
                    Double = tr.ReadLine().After("=");
                    TitleSeparator = tr.ReadLine().After("=");
                    SeriesSeparator = tr.ReadLine().After("=");
                    GuessSeries = Convert.ToBoolean(tr.ReadLine().After("="));
                    ForceGuessSeries = Convert.ToBoolean(tr.ReadLine().After("="));
                    UseSeries = Convert.ToBoolean(tr.ReadLine().After("="));
                    UseTitle = Convert.ToBoolean(tr.ReadLine().After("="));
                    OnlyEpisode = Convert.ToBoolean(tr.ReadLine().After("="));
                    FormatEpisode = Convert.ToBoolean(tr.ReadLine().After("="));
                    SeasonDigits = Int32.Parse(tr.ReadLine().After("="));
                    EpisodeDigits = Int32.Parse(tr.ReadLine().After("="));
                } catch (Exception) {
                    return false;
                }
                return true;
            }

            public bool SaveSettings(TextWriter tw) {
                try {
                    tw.WriteLine("SpaceOption=" + Array.FindIndex(whiteSpaces,s => s == SpaceOption).ToString());
                    tw.WriteLine("Season=" + Season.ToString());
                    tw.WriteLine("Episode=" + Episode.ToString());
                    tw.WriteLine("Double=" + Double.ToString());
                    tw.WriteLine("TitleSeparator=" + TitleSeparator.ToString());
                    tw.WriteLine("ShowNameSeparator=" + SeriesSeparator.ToString());
                    tw.WriteLine("GuessSeries=" + GuessSeries.ToString());
                    tw.WriteLine("ForceGuessSeries=" + ForceGuessSeries.ToString());
                    tw.WriteLine("UseShowName=" + UseSeries.ToString());
                    tw.WriteLine("UseTitle=" + UseTitle.ToString());
                    tw.WriteLine("OnlyEpisode=" + OnlyEpisode.ToString());
                    tw.WriteLine("FormatEpisode=" + FormatEpisode.ToString());
                    tw.WriteLine("SeasonDigits=" + SeasonDigits.ToString());
                    tw.WriteLine("EpisodeDigits=" + EpisodeDigits.ToString());
                }
                catch (Exception) {
                    return false;
                }
                return true;
            }
        }

        public class ErrorClass {
            public bool FilterFile;
            public bool NonFilterFile;

            public void ResetToDefault() {
                FilterFile = true;
                NonFilterFile = true;
            }

            public bool LoadSettings(TextReader tr) {
                try {
                    FilterFile = Convert.ToBoolean(tr.ReadLine().After("="));
                    NonFilterFile = Convert.ToBoolean(tr.ReadLine().After("="));
                } catch (Exception){
                    return false;
                }
                return true;
            }

            public bool SaveSettings(TextWriter tw) {
                tw.WriteLine("ErrorFilterFile=" + FilterFile.ToString());
                tw.WriteLine("ErrorNonFilterFile=" + NonFilterFile.ToString());
                return true;
            }
        }

        public class WarningClass {
            public bool FolderCombiner;
            public bool Deletions;
            public int TreeFreezeFirst;
            public int TreeFreezeLast;

            public void ResetToDefault() {
               FolderCombiner = true;
               Deletions = true;
                TreeFreezeFirst = 100;
                TreeFreezeLast = 1000;
            }

            public bool LoadSettings(TextReader tr) {
                try {
                    FolderCombiner = Convert.ToBoolean(tr.ReadLine().After("="));
                    Deletions = Convert.ToBoolean(tr.ReadLine().After("="));
                    TreeFreezeFirst = Int32.Parse(tr.ReadLine().After("="));
                    TreeFreezeLast = Int32.Parse(tr.ReadLine().After("="));
                } catch (Exception) {
                    return false;
                }
                return true;
            }

            public bool SaveSettings(TextWriter tw) {
                try {
                    tw.WriteLine("WarningFolderCombiner=" + FolderCombiner.ToString());
                    tw.WriteLine("WarningDeletions=" + Deletions.ToString());
                    tw.WriteLine("WarningTreeFreezeFirst=" + TreeFreezeFirst.ToString());
                    tw.WriteLine("WarningTreeFreezeLast=" + TreeFreezeLast.ToString());
                } catch (Exception) {
                    return false;
                }
                return true;
            }
        }

        public class MKVClass {
            public string ProgramPath;
            public int Title;
            public void ResetToDefault() {
                ProgramPath = @"C:\Program Files\MKVToolNix";
                Title = 1;
            }

            public bool LoadSettings(TextReader tr) {
                try {
                    ProgramPath = tr.ReadLine().After("=");
                    Title = Int32.Parse(tr.ReadLine().After("="));
                } catch (Exception) {
                    return false;
                }
                return true;
            }

            public bool SaveSettings(TextWriter tw) {
                try {
                    tw.WriteLine("MKVProgramPath=" + ProgramPath);
                    tw.WriteLine("MKVTitle=" + Title.ToString());
                } catch (Exception) {
                    return false;
                }
                return true;
            }
        }

        public class FormsClass {
            //This group of settings saves and loads on program start and exit
            //It is not yet implemented
            public int MainPosX;
            public int MainPosY;
            public int MainSizeX;
            public int MainSizeY;
            public int SettingsPosX;
            public int SettingsPosY;
            public int SettingsSizeX;
            public int SettingsSizeY;
            public int MKVPosX;
            public int MKVPosY;
            public int MKVSizeX;
            public int MKVSizeY;

            public bool LoadSettings(TextReader tr) {
                try {
                    MainPosX = Int32.Parse(tr.ReadLine().After("="));
                } catch (Exception) {
                    return false;
                }
                return true;
            }

            public bool SaveSettings(TextWriter tw) {
                try {
                    tw.WriteLine("MainPosX=" + MainPosX.ToString());
                } catch (Exception) {
                    return false;
                }
                return true;
            }
        }

    }
}
