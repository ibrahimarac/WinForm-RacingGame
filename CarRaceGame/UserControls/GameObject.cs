using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CarRaceGame.Manager;

namespace CarRaceGame.UserControls
{
    public partial class GameObject : UserControl,IGameObject
    {
        private ObjectType objType;

        public GameObject()
        {
            InitializeComponent();
        }
        public bool IsOutOfGameArea
        {
            get
            {
                return this.Top >= this.Parent.Height;
            }
        }
        public int CurrentPosition
        {
            get
            {
                return this.Top;
            }
        }
        public Size CurrentSize
        {
            get
            {
                return this.Size;
            }
        }
        public ObjectType Type
        {
            get
            {
                return objType;
            }
            set
            {
                objType = value;
            }
        }
        public Rectangle ObjectRectangle
        {
            get
            {
                return new Rectangle(new Point(this.Left,this.Top),this.Size);
            }
        }
        public void SetObjectImage(Image image)
        {
            this.BackgroundImage = image;
        }
        public void SetObjectSize(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }        
        public void SetPosition(int speed)
        {
            this.Top += speed;
        }
    }
}
