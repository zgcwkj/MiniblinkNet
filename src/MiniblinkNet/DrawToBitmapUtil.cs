using MiniblinkNet.MiniBlink;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MiniblinkNet
{
    public class DrawToBitmapUtil
    {
        private MiniblinkBrowser _miniblink;
        private Action<ScreenshotImage> _callback;
        private IDictionary<int, Image> _images = new SortedDictionary<int, Image>();
        private int _scrollTopBak;
        private int _imageHeight;
        private int _contentHeight;
        private int _contentWidth;
        private string _cssBak;

        public DrawToBitmapUtil(MiniblinkBrowser miniblink)
        {
            _miniblink = miniblink;
        }

        public void ToImage(Action<ScreenshotImage> callback)
        {
            _callback = callback;
            _cssBak = _miniblink.RunJs(@"
                var b = document.getElementsByTagName('body')[0];
                var v = b.style.overflow;
                b.style.overflow='hidden';
                return v;").ToString();
            _contentHeight = _miniblink.ScrollHeight;
            _contentWidth = _miniblink.ContentWidth;

            if (_contentWidth > Screen.PrimaryScreen.WorkingArea.Width)
            {
                _contentWidth = Screen.PrimaryScreen.WorkingArea.Width;
            }

            if (_contentWidth < _miniblink.ViewWidth)
            {
                _contentWidth = _miniblink.ViewWidth;
            }
            _imageHeight = Screen.PrimaryScreen.WorkingArea.Height;

            if (_contentHeight < _imageHeight)
            {
                _imageHeight = _contentHeight;
            }
            _scrollTopBak = _miniblink.ScrollTop;
            _miniblink.ScrollTop = 0;
            _miniblink.PaintUpdated += WaitToImagePaint;
            MBApi.wkeResize(_miniblink.MiniblinkHandle, _contentWidth, _imageHeight);
        }

        private void WaitToImagePaint(object sender, PaintUpdatedEventArgs e)
        {
            e.Cancel = true;

            if (_miniblink.ViewWidth != _contentWidth || _miniblink.ViewHeight != _imageHeight)
            {
                return;
            }

            var height = _imageHeight;
            var scrTop = _imageHeight;
            var srcY = 0;
            var isLast = false;
            if ((_images.Count + 1) * _imageHeight >= _contentHeight)
            {
                height = _contentHeight - _images.Count * _imageHeight;
                scrTop = height;
                srcY = _imageHeight - height;
                isLast = true;
            }
            var bmp = new Bitmap(_contentWidth, height);

            using (var g = Graphics.FromImage(bmp))
            {
                g.DrawImage(e.Image,
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    new Rectangle(0, srcY, bmp.Width, bmp.Height),
                    GraphicsUnit.Pixel);
            }

            _images[_miniblink.ScrollTop] = bmp;

            if (isLast)
            {
                _miniblink.PaintUpdated -= WaitToImagePaint;
                _miniblink.ScrollTop = _scrollTopBak;
                _miniblink.RunJs($"document.getElementsByTagName('body')[0].style.overflow='{_cssBak}'");
                _miniblink.PaintUpdated += DisablePaintUpdated;
                MBApi.wkeResize(_miniblink.MiniblinkHandle, _miniblink.Width, _miniblink.Height);

                var ss = new ScreenshotImage(_contentWidth, _contentHeight, _images.Values);
                _images.Clear();
                _callback?.Invoke(ss);
                _callback = null;
            }
            else
            {
                _miniblink.ScrollTop += scrTop;
            }
        }

        private void DisablePaintUpdated(object sender, PaintUpdatedEventArgs e)
        {
            _miniblink.PaintUpdated -= DisablePaintUpdated;
            e.Cancel = true;
        }
    }
}
