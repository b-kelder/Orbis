using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Orbis.Engine;
using Orbis.Rendering;
using Orbis.Simulation;
using Orbis.States;
using Orbis.World;

using Orbis.UI.Windows;
using Orbis.UI;
using System;
using System.Linq;

namespace Orbis
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Orbis : Game
    {
        public static readonly int TEST_SEED = 1000;
        public static readonly int TEST_CIVS = 15;
        public static readonly int TEST_RADIUS = 128;
        public static readonly int TEST_TICKS = 10000;

        public InputHandler Input { get { return InputHandler.GetInstance(); } }
        public GraphicsDeviceManager Graphics { get { return graphics; } }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        InputHandler input;

        public UIManager UI;

        public Scene Scene { get; set; }
        public Simulator Simulator { get; set; }
        public XMLModel.DecorationCollection DecorationSettings { get; private set; }
        public XMLModel.WorldSettings WorldSettings { get; private set; }
        public XMLModel.Civilization[] CivSettings { get; private set; }
        public BiomeCollection BiomeCollection { get; private set; }

        public SceneRendererComponent SceneRenderer { get; private set; }


        private SpriteFont fontDebug;

        private bool drawDebugText;
        private List<string> debugLines;

        public Orbis()
        {
            debugLines = new List<string>();
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            SceneRenderer = new SceneRendererComponent(this)
            {
                DrawOrder = 0
            };
            Components.Add(SceneRenderer);

            input = InputHandler.GetInstance();
            UI = new UI.UIManager(this)
            {
                DrawOrder = 1
            };
            Components.Add(UI);

            graphics.PreparingDeviceSettings += (sender, args) =>
            {
                args.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            };
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            AudioManager.Initialize();

            this.IsFixedTimeStep = false;
            this.IsMouseVisible = true;
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();

            base.Initialize();
        }

        public void GenerateWorld(int seed, XMLModel.DecorationCollection decorationSettings, XMLModel.WorldSettings worldSettings, World.BiomeCollection biomeCollection, XMLModel.Civilization[] civSettings, int civs, int radius, int ticks)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            // Generate world
            Debug.WriteLine("Generating world for seed " + seed);
            Scene = new Scene(seed, worldSettings, decorationSettings, biomeCollection);
            var random = new Random(seed);
            var randomCivs = civSettings.OrderBy(x => random.Next()).ToArray();
            var generator = new WorldGenerator(Scene, randomCivs);
            generator.GenerateWorld(radius);
            generator.GenerateCivs(civs);

            Simulator = new Simulator(Scene, ticks);

            stopwatch.Stop();
            Debug.WriteLine("Generated world in " + stopwatch.ElapsedMilliseconds + " ms");
            // Let the scene renderer generate the new meshes
            SceneRenderer.OnNewWorldGenerated(Scene, seed);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            AudioManager.LoadContent(Content);

            DecorationSettings = Content.Load<XMLModel.DecorationCollection>("Config/Decorations");
            WorldSettings = Content.Load<XMLModel.WorldSettings>("Config/WorldSettings");
            CivSettings = Content.Load<XMLModel.Civilization[]>("Config/Civilization");
            fontDebug = Content.Load<SpriteFont>("DebugFont");

            // Biome table test
            var biomeData = Content.Load<XMLModel.BiomeCollection>("Config/Biomes");
            BiomeCollection = new BiomeCollection(biomeData, Content);

            UI.CurrentWindow = new MenuUI(this);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Update user input
            Input.UpdateInput();

            // Check if the state has changed
            if (StateManager.GetInstance().IsStateChanged())
            {
                StateManager.GetInstance().RunState();
            }

            // Update renderer if we can
            if (SceneRenderer.ReadyForUpdate)
            {
                Simulator.Update(gameTime);
                Cell[] updatedCells = null;
                do
                {
                    updatedCells = Simulator.GetChangedCells();
                    if (updatedCells != null && updatedCells.Length > 0)
                    {
                        SceneRenderer.UpdateScene(updatedCells);
                    }
                } while (updatedCells != null);
            }

            if(Input.IsKeyDown(Keys.F1))
            {
                drawDebugText = !drawDebugText;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if(drawDebugText)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(fontDebug,
                    "FPS: " + (1 / gameTime.ElapsedGameTime.TotalSeconds).ToString("##") + "   " +
                    "Render Instances: " + SceneRenderer.RenderInstanceCount
                    , new Vector2(40, 40), Color.Red);

                float y = 55;
                const float dy = 15;
                foreach(var line in debugLines)
                {
                    spriteBatch.DrawString(fontDebug, line, new Vector2(40, y), Color.Red);
                    y += dy;
                }
                spriteBatch.End();
            }
            debugLines.Clear();
        }

        public void DrawDebugLine(string text)
        {
            debugLines.Add(text);
        }
    }
}
