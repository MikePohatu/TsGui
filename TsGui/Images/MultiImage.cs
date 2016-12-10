//    Copyright (C) 2016 Mike Pohatu

//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; version 2 of the License.

//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License along
//    with this program; if not, write to the Free Software Foundation, Inc.,
//    51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.

// MultiImage.cs - object responsible for providing the correct image for a given scaling
// setting. Holds multiple images for each scaling ratio, and methods for handling windows
// scaling events

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;

using TsGui.Helpers;

namespace TsGui.Images
{
    public class MultiImage: INotifyPropertyChanged
    {
        private string _rootpath;
        private string _imagename;
        private string _imageextn;
        private int _scale;
        private SortedDictionary<int, BitmapImage> _images;
        private BitmapImage _currentimage;
        private MainController _controller;

        public BitmapImage CurrentImage
        {
            set { this._currentimage = value; this.OnPropertyChanged(this,"CurrentImage"); }
            get { return this._currentimage; }
        }
        public MultiImage (string FileName, MainController MainController)
        {
            this._controller = MainController;
            this._controller.WindowLoaded += this.OnWindowLoaded;
            this._controller.WindowMouseUp += this.OnWindowMouseUp;

            this._images = new SortedDictionary<int, BitmapImage>(new ReverseComparer<int>(Comparer<int>.Default));
            this._rootpath = AppDomain.CurrentDomain.BaseDirectory + @"\images\";

            string fullpath = this._rootpath + FileName;
            this._imagename = Path.GetFileNameWithoutExtension(fullpath);
            this._imageextn = Path.GetExtension(fullpath);
            this.LoadImages();
        }

        //Events
        #region 
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(object sender, string name)
        {
            PropertyChanged?.Invoke(sender, new PropertyChangedEventArgs(name));
        }
        #endregion

        public void OnWindowMouseUp(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(() => this.UpdateImage(this.GetScaling())));
        }

        public void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            this.UpdateImage(this.GetScaling());
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
            if (this._scale == Scale) { return; }

            this._scale = Scale;
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

        private int GetScaling()
        {
            Window mainwindow = Application.Current.MainWindow;
            PresentationSource MainWindowPresentationSource = PresentationSource.FromVisual(mainwindow);
            Matrix m = MainWindowPresentationSource.CompositionTarget.TransformToDevice;
            int returnval = Convert.ToInt32(m.M11) * 100;
            Debug.WriteLine("GetScaling: " + returnval);
            return returnval;
        }
    }
}
