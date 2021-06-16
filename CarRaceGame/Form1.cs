using CarRaceGame.Manager;
using CarRaceGame.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CarRaceGame
{
    public partial class Form1 : Form
    {
        private int velocity = 15;
        GameManager manager;        

        public Form1()
        {
            InitializeComponent();
            manager = new GameManager(this);
        }

        //Form üzerinde yer alan oyuncuya ulaşmak için aşağıdaki property yazıldı.
        public Control Player
        {
            get
            {
                return player;
            }
        }

        ObjectType GenerateObjectType()
        {
            ObjectType type = ObjectType.Car;
            bool isStar = new Random().Next(1, 1000) % 4 == 0;
            bool isObstacle = new Random().Next(1, 1000) % 5 == 0;
            if (isStar)
                type = ObjectType.Star;
            if (isObstacle)
                type = ObjectType.Obstacle;
            return type;
        }

        //Sahneye bir Timer eklenerek enabled özelliğini True yapıyorum.
        //Interval özelliği ise 10 olarak ayarlandı.
        private void timer1_Tick(object sender, EventArgs e)
        {
            manager.UpdateCars(velocity);
            manager.AddObjectToGameArea(GenerateObjectType());
        }
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left)
            {
                player.Left = 45;
            }
            else if (keyData == Keys.Right)
            {
                player.Left = 205;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }       
    }
}
