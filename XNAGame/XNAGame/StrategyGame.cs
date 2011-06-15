using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GameEngine;
using System.IO;

namespace XNAGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class StrategyGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        StateManager _stateManager;

        public StrategyGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;

            int width = graphics.PreferredBackBufferWidth;
            int height = graphics.PreferredBackBufferHeight;

            GameArea gameArea = new GameArea(width, height, 6, 24);
            CameraSystem camera = new CameraSystem(new Size(width, height), gameArea.GameAreaSize);

            _stateManager = new StateManager(gameArea, camera);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            handleInput();

            _stateManager.Update((long)gameTime.ElapsedGameTime.TotalMilliseconds);       

            base.Update(gameTime);
        }

        int prevX;
        int prevY;
        bool pressed = false;

        private void handleInput()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.F))
                _stateManager.HandleInput(new Zoom(1));
            else if (Keyboard.GetState().IsKeyDown(Keys.R))
                _stateManager.HandleInput(new Zoom(-1));
            else if (Keyboard.GetState().IsKeyDown(Keys.W))
                _stateManager.HandleInput(new MoveCameraTagetLocation(0, -1));
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
                _stateManager.HandleInput(new MoveCameraTagetLocation(0, 1));
            else if (Keyboard.GetState().IsKeyDown(Keys.A))
                _stateManager.HandleInput(new MoveCameraTagetLocation(-1, 0));
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
                _stateManager.HandleInput(new MoveCameraTagetLocation(1, 0));

            checkMouse();  
        }

        private void checkMouse()
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (pressed == false)
                {
                    // First click
                    _stateManager.HandleInput(new MouseDownAction(Mouse.GetState().X, Mouse.GetState().Y));
                }
                else
                {
                    _stateManager.HandleInput(new MouseMoveAction(Mouse.GetState().X, Mouse.GetState().Y));        
                }

                prevX = Mouse.GetState().X;
                prevY = Mouse.GetState().Y;
                pressed = true;
            }
            else
            {
                if (pressed)
                    _stateManager.HandleInput(new MouseUpAction(Mouse.GetState().X, Mouse.GetState().Y));   

                pressed = false;
                prevX = 0;
                prevY = 0;
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            List<DrawObject> drawObjects = _stateManager.GetDrawableObjects();

            foreach (DrawObject drawObject in drawObjects)
            {
                MemoryStream ms = new MemoryStream();
                drawObject.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                Texture2D texture = Texture2D.FromStream(GraphicsDevice, ms);
                
                Microsoft.Xna.Framework.Rectangle destinationRectangle = new Microsoft.Xna.Framework.Rectangle(
                    (int)(drawObject.X - drawObject.Width / 2), (int)(drawObject.Y - drawObject.Height / 2), (int)drawObject.Width, (int)drawObject.Height);

                spriteBatch.Draw(texture, destinationRectangle, null, Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        
    }
}
