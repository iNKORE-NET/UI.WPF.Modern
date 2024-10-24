using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace iNKORE.UI.WPF.Modern.Common
{
    public struct CornerRadiusEx
    {
        public double TopLeftX => this._topLeftX;        
        private double _topLeftX;
        public double TopLeftY => this._topLeftY;
        private double _topLeftY;

        public double TopRightX => this._topRightX;
        private double _topRightX;
        public double TopRightY => this._topRightY;
        private double _topRightY;

        public double BottomLeftX => this._bottomLeftX;
        private double _bottomLeftX;
        public double BottomLeftY => this._bottomLeftY;
        private double _bottomLeftY;

        public double BottomRightX => this._bottomRightX;
        private double _bottomRightX;
        public double BottomRightY => this._bottomRightY;
        private double _bottomRightY;


        public CornerRadiusEx(double topLeftX, double topLeftY, double topRightX, double topRightY, double bottomLeftX, double bottomLeftY, double bottomRightX, double bottomRightY)
        {
            this._topLeftX = topLeftX;
            this._topLeftY = topLeftY;
            this._topRightX = topRightX;
            this._topRightY = topRightY;
            this._bottomLeftX = bottomLeftX;
            this._bottomLeftY = bottomLeftY;
            this._bottomRightX = bottomRightX;
            this._bottomRightY = bottomRightY;
        }

        public CornerRadiusEx(CornerRadius cornerRadius): this(cornerRadius.TopLeft, cornerRadius.TopLeft, cornerRadius.TopRight, cornerRadius.TopRight, cornerRadius.BottomLeft, cornerRadius.BottomLeft, cornerRadius.BottomRight, cornerRadius.BottomRight)
        {
        }

        public CornerRadiusEx(double uniformRadius) : this(uniformRadius, uniformRadius, uniformRadius, uniformRadius, uniformRadius, uniformRadius, uniformRadius, uniformRadius)
        {
        }

        public static implicit operator CornerRadiusEx(CornerRadius cornerRadius)
        {
            return new CornerRadiusEx(cornerRadius);
        }

        public override string ToString()
        {
            return $"{TopLeftX} {TopLeftY}, {TopRightX} {TopRightY}, {BottomLeftX} {BottomLeftY},{BottomRightX} {BottomRightY}";
        }
    }
}
