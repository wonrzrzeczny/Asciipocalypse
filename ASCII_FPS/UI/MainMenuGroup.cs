using ASCII_FPS.GameComponents;
using Microsoft.Xna.Framework.Input;
using System;

namespace ASCII_FPS.UI
{
    public class MainMenuGroup
    {
        private readonly UIStack uiStack;

        private readonly UIMenu mainMenu;
        private readonly UIMenu optionsMenu;
        private readonly UIMenu tutorialMenu;
        private readonly UIKeybinds keybindsMenu;

        public Action LoadGame { private get; set; }
        public Action NewGame { private get; set; }
        public Action ExitGame { private get; set; }
        public Action ChangeFullScreen { private get; set; }
        public Action<int> ChangeResolution { private get; set; }
        public Action SaveOptions { private get; set; }

        public Func<bool> ContinueEntryPred { private get; set; }


        public MainMenuGroup()
        {
            mainMenu = new UIMenu();
            optionsMenu = new UIMenu();
            tutorialMenu = new UIMenu();
            keybindsMenu = new UIKeybinds();
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
            mainMenu.AddEntry(new MenuEntry(6,  @"_______                                        ___                    ", UIUtils.colorWhite));
            mainMenu.AddEntry(new MenuEntry(7,  @"|     |______________________________________  | |__ _________________", UIUtils.colorWhite));
            mainMenu.AddEntry(new MenuEntry(8,  @"| ___ |   |   ||_||_||     |     |   ||     |  | | | | |     |   |   |", UIUtils.colorWhite));
            mainMenu.AddEntry(new MenuEntry(9,  @"| |_| | __| __|______| ___ | ___ | __|| ___ |  | | | | | ___ | __| __|", UIUtils.colorWhite));
            mainMenu.AddEntry(new MenuEntry(10, @"|     |   | |  | || || | | | | | | |  | | | |  | | | | | | | |   |  | ", UIUtils.colorWhite));
            mainMenu.AddEntry(new MenuEntry(11, @"| ___ |__ | |__| || || |_| | |_| | |__| |_| |__| | |_| | |_| |__ | _|_", UIUtils.colorWhite));
            mainMenu.AddEntry(new MenuEntry(12, @"| | | |   |   || || ||     |     |   ||       || |     |     |   |   |", UIUtils.colorWhite));
            mainMenu.AddEntry(new MenuEntry(13, @"|_| |_|___|___||_||_|| ____|_____|___||_______||_|____ | ____|___|___|", UIUtils.colorWhite));
            mainMenu.AddEntry(new MenuEntry(14, @"                     | |                             | | |            ", UIUtils.colorWhite));
            mainMenu.AddEntry(new MenuEntry(15, @"                     | |                             | | |            ", UIUtils.colorWhite));
            mainMenu.AddEntry(new MenuEntry(16, @"                     |_|                             |_|_|            ", UIUtils.colorWhite));

            MenuEntry continueEntry = new MenuEntry(30, "Continue", LoadGame, UIUtils.colorGray, UIUtils.colorLightBlue);
            continueEntry.HiddenPred = ContinueEntryPred;
            mainMenu.AddEntry(continueEntry);
            mainMenu.AddEntry(new MenuEntry(32, "New game", () => { uiStack.Push(tutorialMenu); }, UIUtils.colorGray, UIUtils.colorLightBlue));
            mainMenu.AddEntry(new MenuEntry(34, "Options", () => { uiStack.Push(optionsMenu); }, UIUtils.colorGray, UIUtils.colorLightBlue));
            mainMenu.AddEntry(new MenuEntry(36, "Exit", ExitGame, UIUtils.colorGray, UIUtils.colorLightBlue));
        }

        private void InitOptionsMenu()
        {
            // Options main
            optionsMenu.AddEntry(new MenuEntry(
                12, "Back to main menu", () => { uiStack.Pop(); SaveOptions.Invoke(); }, UIUtils.colorGray, UIUtils.colorLightBlue
            ));
            optionsMenu.AddEntry(new MenuEntry(16, "Keybinds", () => { uiStack.Push(keybindsMenu); }, UIUtils.colorGray, UIUtils.colorLightBlue));
            optionsMenu.AddEntry(new MenuEntry(18, "Fullscreen", ChangeFullScreen, UIUtils.colorGray, UIUtils.colorLightBlue));

            optionsMenu.AddEntry(new MenuEntry(22, "Resolution", UIUtils.colorWhite));
            for (int i = 0; i < ASCII_FPS.resolutions.Length; i++)
            {
                int resX = ASCII_FPS.resolutions[i].Width;
                int resY = ASCII_FPS.resolutions[i].Height;
                string line = resX + " x " + resY;
                if (resX == 1920 && resY == 1080) line += " (recommended)";
                int j = i; // manual closure :D
                optionsMenu.AddEntry(new MenuEntry(24 + 2 * i, line, () => { ChangeResolution(j); }, UIUtils.colorGray, UIUtils.colorLightBlue));
            }

            keybindsMenu.BackAction = uiStack.Pop;
        }

        private void InitTutorialMenu()
        {
            tutorialMenu.AddEntry(new MenuEntry(12, "Controls", UIUtils.colorWhite));

            tutorialMenu.AddEntry(new MenuEntry(16, "Walk forward / backwards - " + Keybinds.forward + " / " + Keybinds.backwards, UIUtils.colorWhite));
            tutorialMenu.AddEntry(new MenuEntry(18, "Turn left / right - " + Keybinds.turnLeft + " / " + Keybinds.turnRight, UIUtils.colorWhite));
            tutorialMenu.AddEntry(new MenuEntry(20, "Strafe left / right - " + Keybinds.strafeLeft + " / " + Keybinds.strafeRight, UIUtils.colorWhite));
            tutorialMenu.AddEntry(new MenuEntry(22, "Hold " + Keybinds.sprint + " - faster movement", UIUtils.colorWhite));
            tutorialMenu.AddEntry(new MenuEntry(24, "Hold " + Keybinds.fire + " - shoot", UIUtils.colorWhite));
            tutorialMenu.AddEntry(new MenuEntry(26, Keybinds.action + " - use barrel / ladder", UIUtils.colorWhite));
            tutorialMenu.AddEntry(new MenuEntry(28, Keybinds.skills + " - skill menu", UIUtils.colorWhite));
            tutorialMenu.AddEntry(new MenuEntry(30, "1/2/3/4 - upgrade skill", UIUtils.colorWhite));
            tutorialMenu.AddEntry(new MenuEntry(32, "Escape - pause game", UIUtils.colorWhite));

            tutorialMenu.AddEntry(new MenuEntry(36, "To progress you must kill at least half of the monsters on the floor", UIUtils.colorWhite));

            tutorialMenu.AddEntry(new MenuEntry(40, "Press enter to start the game", UIUtils.colorWhite));


            tutorialMenu.AddEntry(new MenuEntry(0, "dummy", NewGame, UIUtils.colorBlack, UIUtils.colorBlack));
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
