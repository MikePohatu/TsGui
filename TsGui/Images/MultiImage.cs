﻿#region license
// Copyright (c) 2025 Mike Pohatu
//
// This file is part of TsGui.
//
// TsGui is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3 of the License.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
#endregion

// MultiImage.cs - object responsible for providing the correct image for a given scaling
// setting. Holds multiple images for each scaling ratio, and methods for handling windows
// scaling events

using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Collections.Generic;
using System.Windows.Threading;

using TsGui.Helpers;
using Core;

namespace TsGui.Images
{
    public class MultiImage: ViewModelBase
    {
        public event RoutedEventHandler ImageScalingUpdate;

        private string _rootpath;
        private string _imagename;
        private string _imageextn;
        private int _currentscaling;
        private SortedDictionary<int, BitmapImage> _images;
        private BitmapImage _currentimage;

        public int CurrentScaling
        {
            set { this._currentscaling = value; this.OnPropertyChanged(this, "CurrentScaling"); }
            get { return this._currentscaling; }
        }
        public BitmapImage CurrentImage
        {
            set { this._currentimage = value; this.OnPropertyChanged(this,"CurrentImage"); }
            get { return this._currentimage; }
        }
        public string CurrentFilePath
        {
            get { return this._currentimage?.UriSource.LocalPath; }
        }
        public MultiImage (string FileName)
        {
            Director.Instance.PageLoaded += this.OnPageLoaded;

            this._images = new SortedDictionary<int, BitmapImage>(new ReverseComparer<int>(Comparer<int>.Default));
            this._rootpath = AppDomain.CurrentDomain.BaseDirectory + @"\images\";

            string fullpath = this._rootpath + FileName;
            this._imagename = Path.GetFileNameWithoutExtension(fullpath);
            this._imageextn = Path.GetExtension(fullpath);
            this.LoadImages();

            if (this._images.Count > 1) { Director.Instance.WindowMouseUp += this.OnWindowMouseUp; }
        }

        public void OnWindowMouseUp(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => this.UpdateImage(DisplayInformation.GetScaling())));
        }

        public void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            this.UpdateImage(DisplayInformation.GetScaling());
        }

        private void LoadImages()
        {
            this.AddImage(this._imagename);
            this.AddImage(this._imagename, 100);
            this.AddImage(this._imagename, 125);
            this.AddImage(this._imagename, 150);
            this.AddImage(this._imagename, 175);
            this.AddImage(this._imagename, 200);
            this.AddImage(this._imagename, 250);
            this.AddImage(this._imagename, 300);
            this.AddImage(this._imagename, 400);
            this.AddImage(this._imagename, 500);
        }

        private void UpdateImage(int Scale)
        {
            if (this.CurrentScaling == Scale) { return; }

            this.CurrentScaling = Scale;
            BitmapImage outimage;
            this._images.TryGetValue(Scale, out outimage);
            if (outimage != null) { this.CurrentImage = outimage; }
            else
            {
                foreach (KeyValuePair<int,BitmapImage> kv in this._images)
                {
                    if ((outimage == null) || (kv.Key > Scale)) { this.CurrentImage = kv.Value; }
                    else { break; }
                }
            }
            this.OnPropertyChanged(this, "CurrentFileName");
            this.ImageScalingUpdate?.Invoke(this, new RoutedEventArgs());
        }

        private void AddImage(string ImageName)
        { this.AddImage(ImageName, 0); }

        private void AddImage(string ImageName, int Scale)
        {
            string suffix;
            int setscale;

            if (Scale == 0)
            {
                suffix = string.Empty;
                setscale = 100;
            }
            else
            {
                suffix = "_" + Scale.ToString();
                setscale = Scale;
            }

            //check file exists
            string filepath = this._rootpath + ImageName + suffix + this._imageextn;
            if (File.Exists(filepath)==false) { return; }

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(filepath);
            image.EndInit();

            BitmapImage outimage;
            this._images.TryGetValue(setscale, out outimage);
            if (outimage == null) { this._images.Add(setscale, image); }
            else { this._images[setscale] = image; }
            
        }
    }
}
