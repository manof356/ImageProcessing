using System;
using WixSharp;
using WixSharp.Forms;

namespace ImageProcessing.SetupWS
{
    public class Program
    {
        static void Main()
        {
            // отдельная "фича" - чекбокс "создать ярлык на рабочем столе"
            var desktopShortcutFeature = new Feature("Ярлык на рабочем столе");

            var project = new ManagedProject("Редакторы фотографий",
                          new Dir(@"%ProgramFiles%\CrookedPhotoEditor",
                          new Files(@"..\ImageProcessing\bin\Release\*.*",
                            f => !f.EndsWith(".pdb")),
                          // сам ярлык, привязанный к этой фиче
                          new Dir(@"%Desktop%",
                          new ExeFileShortcut("Редакторы фотографий", @"[INSTALLDIR]ImageProcessing.exe", "")
                          { Feature = desktopShortcutFeature })));

            project.GUID = new Guid("C31D7CDD-03C4-458A-BCBD-3C0BDAC4BFA5");
            // версия берётся из exe, так она меняется только в AssemblyInfo.cs
            var exePath = @"..\ImageProcessing\bin\Release\ImageProcessing.exe";
            project.Version = new Version(System.Diagnostics.FileVersionInfo.GetVersionInfo(exePath).FileVersion);
            project.Platform = Platform.x64;
            project.Encoding = System.Text.Encoding.UTF8;
            project.Codepage = "1251";
            project.Language = "ru-RU";
            project.ControlPanelInfo.Manufacturer = "Завал горизонта";

            project.LicenceFile = @"Resources\license.rtf";      // текст соглашения
            project.BannerImage = @"Resources\banner.png";       // полоса сверху в диалогах, 493×58
            project.BackgroundImage = @"Resources\dialog.png";   // картинка слева на Welcome и Exit, 493×312
            project.LocalizationFile = @"Resources\ru.wxl";
            project.ControlPanelInfo.ProductIcon = @"Resources\sunflower.ico"; // иконка в списке программ

            project.ManagedUI = new ManagedUI();

            project.ManagedUI.InstallDialogs.Add(Dialogs.Welcome)
                                            .Add(Dialogs.Licence)
                                            .Add(Dialogs.InstallDir)
                                            .Add(Dialogs.Progress)
                                            .Add(Dialogs.Exit);

            project.ManagedUI.ModifyDialogs.Add(Dialogs.MaintenanceType)
                                           .Add(Dialogs.Progress)
                                           .Add(Dialogs.Exit);

            project.BuildMsi();
        }
    }
}