﻿using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Orbis.Engine;
using Orbis.Rendering;
using Orbis.Simulation;
using Orbis.World;

namespace Orbis
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Orbis : Game
    {
        public static readonly int TEST_SEED = 19450513;
        public static readonly int TEST_CIVS = 22;
        public static readonly int TEST_RADIUS = 128;
        public static readonly int TEST_TICKS = 10000;

        public InputHandler Input { get { return InputHandler.GetInstance(); } }
        public GraphicsDeviceManager Graphics { get { return graphics; } }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private Scene scene;
        private Simulator simulator;

        private SceneRendererComponent sceneRenderer;


        private SpriteFont fontDebug;

        float worldUpdateTimer;

        public Orbis()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            sceneRenderer = new SceneRendererComponent(this);
            Components.Add(sceneRenderer);
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
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();

            base.Initialize();
        }

        private void GenerateWorld(int seed)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            // Generate world
            Debug.WriteLine("Generating world for seed " + seed);
            scene = new Scene(seed);
            var generator = new WorldGenerator(scene);
            generator.GenerateWorld(TEST_RADIUS);
            generator.GenerateCivs(TEST_CIVS);

            simulator = new Simulator(scene, TEST_TICKS);

            stopwatch.Stop();
            Debug.WriteLine("Generated world in " + stopwatch.ElapsedMilliseconds + " ms");

            // Coloring data
            sceneRenderer.OnNewWorldGenerated(scene, seed);
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
            AudioManager.PlaySong("DEV_TEST");

            fontDebug = Content.Load<SpriteFont>("DebugFont");

            //--- LOG TEST
            // Create logger and write text to log
            Events.Logger logger = new Events.Logger();
            logger.AddLog("The kingdom of the idk Tribe has fallen!");
            logger.AddLog("WANT WAAROM NIET");

            // Create a fileWriter
            Events.Writers.FileWriter fileWriter = new Events.Writers.FileWriter();
            Events.Writers.XMLWriter xmlWriter = new Events.Writers.XMLWriter();

            // Create a logWriter
            Events.LogWriter logWriter = new Events.LogWriter();
            logWriter.AddWriter(fileWriter);
            logWriter.AddWriter(xmlWriter);

            // Write the log
            logWriter.Write(logger.GetLog());
            //---  END LOG TEST

            GenerateWorld(TEST_SEED);
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

            // Update renderer if we can
            if(sceneRenderer.ReadyForUpdate)
            {
                simulator.Update(gameTime);
                Cell[] updatedCells = null;
                do
                {
                    updatedCells = simulator.GetChangedCells();
                    if (updatedCells != null && updatedCells.Length > 0)
                    {
                        sceneRenderer.UpdateScene(updatedCells);
                    }
                } while (updatedCells != null);
            }

            if (Input.IsKeyDown(Keys.S, new Keys[] { Keys.LeftShift, Keys.K, Keys.Y}))
            {
                Exit();
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

            spriteBatch.Begin();

            spriteBatch.DrawString(fontDebug, "Tick: " + simulator.CurrentTick, new Vector2(10, 50), Color.Red);
            float y = 80;
            foreach(var civ in scene.Civilizations)
            {
                spriteBatch.DrawString(fontDebug, civ.Name + " - " + civ.Territory.Count + " - Population: " + civ.Population + " - Is Alive: " + civ.IsAlive, new Vector2(10, y), Color.Red);
                y += 15;
            }

            spriteBatch.DrawString(fontDebug, "FPS: " + (1 / gameTime.ElapsedGameTime.TotalSeconds).ToString("##.##"), new Vector2(10, 30), Color.Red);

            spriteBatch.End();
        }
    }
}
