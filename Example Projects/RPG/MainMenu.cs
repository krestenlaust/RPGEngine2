﻿using CommonComponents.UI;
using RPG.GameObjects;
using RPG.UI;
using RPGEngine2;
using System;
using static RPGEngine2.EngineMain;

namespace RPG
{
    public static class MainMenu
    {
        private const float ANIMATION_DURATION_PER_BUTTON = 2.5f;
        private const float NEW_BUTTON_START_MARGIN = 2.0f;
        private const int BUTTON_WIDTH = 20;
        private const int TARGET_X = 50;

        public static int SelectedMenuOption = 0;
        public static bool MenuShown { get; private set; } = true;
        public static float AnimationTime;
        public static bool isAnimating;
        private static MenuOption[] MainMenuButtons;
        private static bool hasGameBeenStarted = false;

        public static void StartGame()
        {
            DisableMenu();

            if (hasGameBeenStarted)
                return;

            Progressbar healthbar = new Progressbar(8, '#', '\0');
            Instantiate(healthbar);
            GameCode.PlayerObj = new Player(healthbar, new Vector2(20, 10));
            Instantiate(GameCode.PlayerObj);
        }

        private static void DisableMenu()
        {
            MenuShown = false;
            isAnimating = false;
            
            for (int i = 0; i < MainMenuButtons.Length; i++)
            {
                MainMenuButtons[i].Active = false;
            }
        }

        public static void EnableMenu()
        {
            MenuShown = true;
            AnimationTime = 0;
            isAnimating = true;

            for (int i = 0; i < MainMenuButtons.Length; i++)
            {
                MainMenuButtons[i].Position = new Vector2(-BUTTON_WIDTH, MainMenuButtons[i].Position.y);
                MainMenuButtons[i].Active = true;
            }
        }

        public static void LoadMenu()
        {
            int startX = -BUTTON_WIDTH;

            MainMenuButtons = new MenuOption[]{
                new MenuOption(
                    "Start Game",
                    new Vector2(startX, 4),
                    BUTTON_WIDTH,
                    delegate{
                        StartGame();
                    }),
                new MenuOption(
                    "Options",
                    new Vector2(startX, 12),
                    BUTTON_WIDTH),

                new MenuOption(
                    "Quit",
                    new Vector2(startX, 20),
                    BUTTON_WIDTH,
                    delegate {
                        EngineStop();
                    })
            };

            for (int i = 0; i < MainMenuButtons.Length; i++)
            {
                Instantiate(MainMenuButtons[i]);
            }
        }

        public static void Update()
        {
            if (!isAnimating)
                return;

            AnimationTime += FixedDeltaTime;

            for (int i = 0; i < MainMenuButtons.Length; i++)
            {
                float animationStartTime = ANIMATION_DURATION_PER_BUTTON * i - (NEW_BUTTON_START_MARGIN * i);
                if (animationStartTime > AnimationTime)
                {
                    // not time to animate yet.
                    //continue;
                    break; // if the previous one shouldn't animate, then this one shouldn't either.
                }

                float animationEndTime = animationStartTime + ANIMATION_DURATION_PER_BUTTON;
                if (animationEndTime <= AnimationTime)
                {
                    if (i == MainMenuButtons.Length - 1)
                    {
                        // all animations are done.
                        isAnimating = false;
                    }

                    // done animating.
                    continue;
                }

                float progress = Math.Min((AnimationTime - animationStartTime) / animationEndTime, 1);

                UIElementBase buttonObj = MainMenuButtons[i];

                buttonObj.Position = Vector2.Lerp(
                    new Vector2(-BUTTON_WIDTH, buttonObj.Position.y),
                    new Vector2(TARGET_X, buttonObj.Position.y),
                    Easings.Quintic.Out(progress)
                    );
            }

        }
    }
}
