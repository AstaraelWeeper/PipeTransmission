using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PipeClient
{
    public partial class VideoPlayer : Form
    {
        public VideoPlayer(string path)
        {
            InitializeComponent();
            newVideo(path);
        }


        public void newVideo(string path)
        {
            axVLCPlugin21.playlist.items.clear();
            axVLCPlugin21.playlist.add("File:///" + path, null, null);
            axVLCPlugin21.playlist.playItem(0);
        }
    }
}
