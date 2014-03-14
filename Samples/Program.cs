using System;
using System.Drawing;
using LogitechWrapper;

namespace Samples
{
    public class Program
    {
        private static void Main(string[] args)
        {
            //WriteText();
            //GetButtons();
            DrawImage();
            Console.ReadLine();
        }

        private static void WriteText()
        {
            var lcd = new Lcd("Write Text Sample", LcdScreenType.Mono) {AutoUpdate = true};
            lcd.WriteText(Line.First, "         WriteText         ");
            lcd.WriteText(Line.Second, "   This is the WriteText   ");
            lcd.WriteText(Line.Third, "  sample. I hope you learn ");
            lcd.WriteText(Line.Fourth, " something.    Made by Keta");
        }

        private static void GetButtons()
        {
            var lcd = new Lcd("Get Buttons Sample", LcdScreenType.Mono) {AutoUpdate = true};
            lcd.Button1.Clicked += (sender, args) => Console.WriteLine("Button 1 was clicked!");
            lcd.Button2.Clicked += (sender, args) => Console.WriteLine("Button 2 was clicked!");
            lcd.Button3.Clicked += (sender, args) => Console.WriteLine("Button 3 was clicked!");
            lcd.Button4.Clicked += (sender, args) => Console.WriteLine("Button 4 was clicked!");
            lcd.HandleButtons(100);
        }

        private static void DrawImage()
        {
            var lcd = new Lcd("Draw Image Sample", LcdScreenType.Mono) {AutoUpdate = true};
            lcd.DrawImage(new Img(@"Data\Image.png"));
        }
    }

    public class Img : IImage
    {
        private Image _image;

        public Img(string path)
        {
            _image = new Bitmap(path);
        }

        public int Width
        {
            get { return _image.Width; }
        }

        public int Height
        {
            get { return _image.Height; }
        }

        public byte GetPixel(int x, int y)
        {
            Color pixel = ((Bitmap) _image).GetPixel(x, y);

            if (pixel.A <= 0)
                return 0;

            return (byte) ((pixel.R + pixel.G + pixel.B)/3);
        }

        public void Resize(int width, int height)
        {
            _image = new Bitmap(_image, width, height);
        }
    }
}