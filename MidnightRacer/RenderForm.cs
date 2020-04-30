using MidnightRacer.Engine;
using MidnightRacer.GameObjects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MidnightRacer
{
    public partial class RenderForm : Form
    {
        public RenderForm() { InitializeComponent(); }

        public void InitRender()
        {
            Engine.View.Height = Height;
            Engine.View.Width = Width;
        }

        private void RenderForm_Load(object sender, EventArgs e)
        {
            InitRender();

            timer1.Start();
            coneSpawnTimer.Start();

            DoubleBuffered = true;

            World.OnCreation();
            World.StartNewGame();
        }

        private void OnWorldPaint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            World.DoWorldTick(g);
        }

        private void RenderTick(object sender, EventArgs e) { Refresh(); }

        private void RenderForm_KeyDown(object sender, KeyEventArgs e)
        {
            World.HandleKeyDown(e.KeyCode);
        }

        private void RenderForm_KeyUp(object sender, KeyEventArgs e)
        {
            World.HandleKeyUp(e.KeyCode);
        }

        public void coneSpawnTimer_Tick(object sender, EventArgs e)
        {
            World.HandleConeTimerTick();
        }
    }
}