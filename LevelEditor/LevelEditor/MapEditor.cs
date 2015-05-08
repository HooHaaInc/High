using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using System.IO;
using TileEngine;

namespace LevelEditor
{
    public partial class MapEditor : Form
    {
        public Game1 game;
        public MapEditor()
        {
            
            InitializeComponent();
        }
        int tileCount = 0;
        private void LoadImageList()
        {
            string filepath = Application.StartupPath + @"\Content\Textures\PlatformTiles.png";
            Bitmap tileSheet = new Bitmap(filepath);
            //int tilecount = 0;
            for (int y = 0; y < tileSheet.Height / TileMap.TileHeight;y++){
                for (int x = 0; x < tileSheet.Width / TileMap.TileWidth;x++){
                    Bitmap newBitmap = tileSheet.Clone(new System.Drawing.Rectangle(
                    x * TileMap.TileWidth, y * TileMap.TileHeight, TileMap.TileWidth,
                    TileMap.TileHeight), System.Drawing.Imaging.PixelFormat.DontCare);
                    imgListTiles.Images.Add(newBitmap);
                    string itemName = "";
                    if (tileCount == 0){
                        itemName = "Empty";
                    }
                    if (tileCount == 1){
                        itemName = "White";
                    }
                    listTiles.Items.Add(new ListViewItem(itemName, tileCount++));
                }
            }
        }
        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            DialogResult result = DialogResult.OK;
            if (game.hasChanged) {
                TileMap.EditorMode = false;
                result = MessageBox.Show("Changes have been made to the map. Are you sure you want to discard?", "Yes", MessageBoxButtons.OKCancel);
                TileMap.EditorMode = true;
            }
            try
            {
                if (result == DialogResult.OK)
                TileMap.LoadMap(new FileStream(
                Application.StartupPath + @"\MAP" +
                cboMapNumber.Items[cboMapNumber.SelectedIndex] +
                ".MAP",
                FileMode.Open));
                game.hasChanged = false;
            }
            catch
            {
                System.Diagnostics.Debug.Print("Unable to load map file");
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to save?", "Yes", MessageBoxButtons.OKCancel);
            if(result == DialogResult.OK)
            TileMap.SaveMap(new FileStream( Application.StartupPath + 
                            @"\MAP" + 
                            cboMapNumber.Items[cboMapNumber.SelectedIndex] + 
                            ".MAP",
                            FileMode.Create));
            game.hasChanged = false;
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            game.Exit();
            Application.Exit();
        }

        private void MapEditor_Load(object sender, EventArgs e)
        {
            LoadImageList();
            FixScrollBarScales();
            FixScrollBarScales();
            cboCodeValues.Items.Clear();
            cboCodeValues.Items.Add("Drug");
            cboCodeValues.Items.Add("Enemy");
            cboCodeValues.Items.Add("Lethal");
            cboCodeValues.Items.Add("EnemyBlocking");
            cboCodeValues.Items.Add("Platform");
            cboCodeValues.Items.Add("Start");
            cboCodeValues.Items.Add("Clear");
            cboCodeValues.Items.Add("Custom");
            for (int x = 0; x < tileCount; x++)
            {
                cboMapNumber.Items.Add(x.ToString().PadLeft(3, '0'));
            }
            cboMapNumber.SelectedIndex = 0;
            TileMap.EditorMode = true;
        }
        private void FixScrollBarScales()
        {
            Camera.ViewPortWidth = pctSurface.Width;
            Camera.ViewPortHeight = pctSurface.Height;
            Camera.Move(Vector2.Zero);
            vScrollBar1.Minimum = 0;
            vScrollBar1.Maximum = Camera.WorldRectangle.Height - Camera.ViewPortHeight;
            hScrollBar1.Minimum = 0;
            hScrollBar1.Maximum = Camera.WorldRectangle.Width - Camera.ViewPortWidth;
        }

        private void MapEditor_Resize(object sender, EventArgs e)
        {
            FixScrollBarScales();
        }

        private void cboCodeValues_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtNewCode.Enabled = false;
            switch( cboCodeValues.Items[cboCodeValues.SelectedIndex].ToString())
            {
                case "Drug" :
                    txtNewCode.Text = "DRUG";
                    break;
                case "Enemy" :
                    txtNewCode.Text = "ENEMY";
                    break;
                case "Lethal" :
                    txtNewCode.Text = "DEAD";
                    break;
                case "EnemyBlocking":
                    txtNewCode.Text = "BLOCK";
                    break;
                case "Start":
                    txtNewCode.Text = "START";
                    break;
                case "Clear":
                    txtNewCode.Text = "";
                    break;
                case "Platform":
                    txtNewCode.Text = "PLATFORM";
                    break;
                case "Custom":
                    txtNewCode.Text = "";
                    txtNewCode.Enabled = true;
                    break;
            }
        }

        private void listTiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listTiles.SelectedIndices.Count > 0)
            {
                game.DrawTile = listTiles.SelectedIndices[0];
            }
        }

        private void radioPassable_CheckedChanged(object sender, EventArgs e)
        {
           if (radioPassable.Checked)game.EditingCode = false;
           else game.EditingCode = true;
           // game.EditingCode = !game.EditingCode;
        }

        private void radioCode_CheckedChanged(object sender, EventArgs e)
        {
            if (radioPassable.Checked) game.EditingCode = false;
            else game.EditingCode = true;
            //game.EditingCode = !game.EditingCode;
        }

        private void txtNewCode_TextChanged(object sender, EventArgs e)
        {
            game.CurrentCodeValue = txtNewCode.Text;
        }

        private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.DrawLayer = 0;
            backgroundToolStripMenuItem.Checked = true;
            interactiveToolStripMenuItem.Checked = false;
            foregroundToolStripMenuItem.Checked = false;
        }

        private void interactiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.DrawLayer = 1;
            backgroundToolStripMenuItem.Checked = false;
            interactiveToolStripMenuItem.Checked = true;
            foregroundToolStripMenuItem.Checked = false;
        }

        private void foregroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            game.DrawLayer = 2;
            backgroundToolStripMenuItem.Checked = false;
            interactiveToolStripMenuItem.Checked = false;
            foregroundToolStripMenuItem.Checked = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (hScrollBar1.Maximum < 0)
            {
                FixScrollBarScales();
            }
            game.Tick();
            if (game.HoverCodeValue != lblCurrentCode.Text)
            lblCurrentCode.Text = game.HoverCodeValue;
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            TileMap.ClearMap();
        }

        private void MapEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            game.Exit();
            Application.Exit();
        }
    }
}
