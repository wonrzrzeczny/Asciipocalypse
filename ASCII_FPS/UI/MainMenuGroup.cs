using ASCII_FPS.GameComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace ASCII_FPS.UI
{
    public class MainMenuGroup
    {
        private readonly UIStack uiStack;

        private readonly UIMenu mainMenu;
        private readonly UIMenu optionsMenuMain;
        private readonly UIMenu optionsMenuKeybinds;
        private readonly UIMenu tutorialMenu;

        public Action LoadGame { private get; set; }
        public Action NewGame { private get; set; }
        public Action ExitGame { private get; set; }
        public Action ChangeFullScreen { private get; set; }
        public Action<int> ChangeResolution { private get; set; }
        public Action SaveOptions { private get; set; }

        public Func<bool> ContinueEntryPred { private get; set; }


        private readonly byte colorRed = Mathg.ColorTo8Bit(Color.Red.ToVector3());
        private readonly byte colorBlack = Mathg.ColorTo8Bit(Color.Black.ToVector3());
        private readonly byte colorGray = Mathg.ColorTo8Bit(Color.DarkGray.ToVector3());
        private readonly byte colorLightGray = Mathg.ColorTo8Bit(Color.LightGray.ToVector3());
        private readonly byte colorWhite = Mathg.ColorTo8Bit(Color.White.ToVector3());
        private readonly byte colorForestGreen = Mathg.ColorTo8Bit(Color.ForestGreen.ToVector3());
        private readonly byte colorLightBlue = Mathg.ColorTo8Bit(Color.LightBlue.ToVector3());


        public MainMenuGroup()
        {
            mainMenu = new UIMenu();
            optionsMenuMain = new UIMenu();
            optionsMenuKeybinds = new UIMenu();
            tutorialMenu = new UIMenu();
            uiStack = new UIStack(mainMenu);
        }

        public void Init()
        {
            InitMainMenu();
            InitOptionsMenu();
            InitTutorialMenu();
        }


        private void InitMainMenu()
        {
            mainMenu.AddEntry(new MenuEntry(6,  @"_______                                        ___                    ", colorWhite));
            mainMenu.AddEntry(new MenuEntry(7,  @"|     |______________________________________  | |__ _________________", colorWhite));
            mainMenu.AddEntry(new MenuEntry(8,  @"| ___ |   |   ||_||_||     |     |   ||     |  | | | | |     |   |   |", colorWhite));
            mainMenu.AddEntry(new MenuEntry(9,  @"| |_| | __| __|______| ___ | ___ | __|| ___ |  | | | | | ___ | __| __|", colorWhite));
            mainMenu.AddEntry(new MenuEntry(10, @"|     |   | |  | || || | | | | | | |  | | | |  | | | | | | | |   |  | ", colorWhite));
            mainMenu.AddEntry(new MenuEntry(11, @"| ___ |__ | |__| || || |_| | |_| | |__| |_| |__| | |_| | |_| |__ | _|_", colorWhite));
            mainMenu.AddEntry(new MenuEntry(12, @"| | | |   |   || || ||     |     |   ||       || |     |     |   |   |", colorWhite));
            mainMenu.AddEntry(new MenuEntry(13, @"|_| |_|___|___||_||_|| ____|_____|___||_______||_|____ | ____|___|___|", colorWhite));
            mainMenu.AddEntry(new MenuEntry(14, @"                     | |                             | | |            ", colorWhite));
            mainMenu.AddEntry(new MenuEntry(15, @"                     | |                             | | |            ", colorWhite));
            mainMenu.AddEntry(new MenuEntry(16, @"                     |_|                             |_|_|            ", colorWhite));

            MenuEntry continueEntry = new MenuEntry(30, "Continue", LoadGame, colorGray, colorLightBlue);
            continueEntry.HiddenPred = ContinueEntryPred;
            mainMenu.AddEntry(continueEntry);
            mainMenu.AddEntry(new MenuEntry(32, "New game", () => { uiStack.Push(tutorialMenu); }, colorGray, colorLightBlue));
            mainMenu.AddEntry(new MenuEntry(34, "Options", () => { uiStack.Push(optionsMenuMain); }, colorGray, colorLightBlue));
            mainMenu.AddEntry(new MenuEntry(36, "Exit", ExitGame, colorGray, colorLightBlue));
        }

        private void InitOptionsMenu()
        {
            // Options main
            optionsMenuMain.AddEntry(new MenuEntry(
                12, "Back to main menu", () => { uiStack.Pop(); SaveOptions.Invoke(); }, colorGray, colorLightBlue
            ));
            optionsMenuMain.AddEntry(new MenuEntry(16, "Keybinds", () => { uiStack.Push(optionsMenuKeybinds); }, colorGray, colorLightBlue));
            optionsMenuMain.AddEntry(new MenuEntry(18, "Fullscreen", ChangeFullScreen, colorGray, colorLightBlue));

            optionsMenuMain.AddEntry(new MenuEntry(22, "Resolution", colorWhite));
            for (int i = 0; i < ASCII_FPS.resolutions.Length; i++)
            {
                int resX = ASCII_FPS.resolutions[i].Width;
                int resY = ASCII_FPS.resolutions[i].Height;
                string line = resX + " x " + resY;
                if (resX == 1920 && resY == 1080) line += " (recommended)";
                int j = i; // manual closure :D
                optionsMenuMain.AddEntry(new MenuEntry(24 + 2 * i, line, () => { ChangeResolution(j); }, colorGray, colorLightBlue));
            }
        }

        private void InitTutorialMenu()
        {
            tutorialMenu.AddEntry(new MenuEntry(12, "Controls", colorWhite));

            tutorialMenu.AddEntry(new MenuEntry(16, "Walk forward / backwards - " + Keybinds.forward + " / " + Keybinds.backwards, colorWhite));
            tutorialMenu.AddEntry(new MenuEntry(18, "Turn left / right - " + Keybinds.turnLeft + " / " + Keybinds.turnRight, colorWhite));
            tutorialMenu.AddEntry(new MenuEntry(20, "Strafe left / right - " + Keybinds.strafeLeft + " / " + Keybinds.strafeRight, colorWhite));
            tutorialMenu.AddEntry(new MenuEntry(22, "Hold " + Keybinds.sprint + " - faster movement", colorWhite));
            tutorialMenu.AddEntry(new MenuEntry(24, "Hold " + Keybinds.fire + " - shoot", colorWhite));
            tutorialMenu.AddEntry(new MenuEntry(26, Keybinds.action + " - use barrel / ladder", colorWhite));
            tutorialMenu.AddEntry(new MenuEntry(28, Keybinds.skills + " - skill menu", colorWhite));
            tutorialMenu.AddEntry(new MenuEntry(30, "1/2/3/4 - upgrade skill", colorWhite));
            tutorialMenu.AddEntry(new MenuEntry(32, "Escape - pause game", colorWhite));

            tutorialMenu.AddEntry(new MenuEntry(36, "To progress you must kill at least half of the monsters on the floor", colorWhite));

            tutorialMenu.AddEntry(new MenuEntry(40, "Press enter to start the game", colorWhite));


            tutorialMenu.AddEntry(new MenuEntry(0, "dummy", NewGame, colorBlack, colorBlack));
        }


        public void Update(KeyboardState keyboard, KeyboardState keyboardPrev)
        {
            uiStack.Update(keyboard, keyboardPrev);
        }

        public void Draw(Console console)
        {
            uiStack.Draw(console);
        }

        public void ToggleMainMenu()
        {
            uiStack.Clear();
            mainMenu.MoveToFirst();
        }
    }
}
