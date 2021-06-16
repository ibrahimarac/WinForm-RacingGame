using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CarRaceGame.Manager
{
    public enum ObjectType
    {
        Car,
        Star,
        Obstacle
    }

    public interface IGameObject
    {
        void SetPosition(int speed);
        bool IsOutOfGameArea { get; }
        int CurrentPosition { get; }
        Size CurrentSize { get; }
        void SetObjectSize(int width, int height);
        void SetObjectImage(Image image);
        ObjectType Type { get; set; }
        Rectangle ObjectRectangle { get; }
    }
}



