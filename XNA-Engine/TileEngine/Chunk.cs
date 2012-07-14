using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine.Tiles
{
    public class Chunk<TValue>
    {
        public TValue[] Tiles { get; protected set; }
        public string TileHash { get; protected set; }
        
        int globalX, globalY;
        public Point GlobalPosition
        {
            get
            {
                return new Point(globalX, globalY);
            }
        }
        public Point Dimensions { get; protected set; }
        public bool IsLoaded { get; protected set; }

        public Chunk(int x, int y, int width, int height){
            globalX = x;
            globalY = y;
            Dimensions = new Point(width, height);
            InitializeTiles();
            IsLoaded = false;
        }

        public Chunk(Point position, Point dimensions) 
            :this(position.X, position.Y, dimensions.X, dimensions.Y) { }

        void InitializeTiles()
        {
            int n = Dimensions.X * Dimensions.Y;
            Tiles = new TValue[n];
            for (int i = 0; i < n; i++)
                Tiles[i] = default(TValue);
        }

        public void UpdateTile(Point localTilePosition, TValue value)
        {
            Tiles[localTilePosition.X + Dimensions.X * localTilePosition.Y] = value;
        }


        public void LoadData(TValue[] data, int startIndex)
        {
            Array.Copy(data, startIndex, Tiles, 0, Dimensions.X * Dimensions.Y);
            IsLoaded = true;
        }

        public void ClearData()
        {
            IsLoaded = false;
        }

        public void SetGlobalPosition(Point position)
        {
            globalX = position.X;
            globalY = position.Y;
        }

        public void SetGlobalPosition(int x, int y)
        {
            globalX = x;
            globalY = y;
        }
    }
}
