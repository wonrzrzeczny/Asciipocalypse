using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace ASCII_FPS.UI
{
    public class MainMenuGroup
    {
        private Menu mainMenu;
        private Menu optionsMenu;
        private Menu tutorialMenu;
        private Menu activeMenu;

        private readonly byte colorRed = Mathg.ColorTo8Bit(Color.Red.ToVector3());
        private readonly byte colorBlack = Mathg.ColorTo8Bit(Color.Black.ToVector3());
        private readonly byte colorGray = Mathg.ColorTo8Bit(Color.DarkGray.ToVector3());
        private readonly byte colorLightGray = Mathg.ColorTo8Bit(Color.LightGray.ToVector3());
        private readonly byte colorWhite = Mathg.ColorTo8Bit(Color.White.ToVector3());
        private readonly byte colorForestGreen = Mathg.ColorTo8Bit(Color.ForestGreen.ToVector3());
        private readonly byte colorLightBlue = Mathg.ColorTo8Bit(Color.LightBlue.ToVector3());


        public MainMenuGroup(Action newGameAction, Action loadGameAction, Action exitAction)
        {
            mainMenu = new Menu();
            optionsMenu = new Menu();
            tutorialMenu = new Menu();

            InitMainMenu(loadGameAction, exitAction);
            InitOptionsMenu();
            InitTutorialMenu(newGameAction);
        }

        private void InitMainMenu(Action loadGameAction, Action exitAction)
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

            mainMenu.AddEntry(new MenuEntry(30, "Continue", loadGameAction, colorGray, colorLightBlue));
            mainMenu.AddEntry(new MenuEntry(32, "New game", () => { activeMenu = tutorialMenu; }, colorGray, colorLightBlue));
            mainMenu.AddEntry(new MenuEntry(34, "Options", () => { activeMenu = optionsMenu; }, colorGray, colorLightBlue));
            mainMenu.AddEntry(new MenuEntry(36, "Exit", exitAction, colorGray, colorLightBlue));
        }

        private void InitOptionsMenu()
        {
            throw new NotImplementedException();
        }

        private void InitTutorialMenu(Action newGameAction)
        {
            throw new NotImplementedException();
        }


        public void Update(KeyboardState keyboard, KeyboardState keyboardPrev)
        {
            activeMenu.Update(keyboard, keyboardPrev);
        }

        public void Draw(Console console)
        {
            activeMenu.Draw(console);
        }
    }
}
