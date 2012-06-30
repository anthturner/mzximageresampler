using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MZXImageResampler
{
    public partial class MainUI : Form
    {
        public MainUI()
        {
            InitializeComponent();
        }

        private void MainUI_Load(object sender, EventArgs e)
        {
            var file = @"comicTest.png";

            WorkingImage.CharacterX = 40;
            WorkingImage.CharacterY = 20;
            WorkingImage.Ditherer = DitheringEngine.Custom;

            WorkingImage.Init(file);

            var firstPass = new MZX.MZM(WorkingImage.CharacterImage, 0);

            Optimizers.BasicCharacterOptimizers.PixelDifference(ref WorkingImage.CharacterImage, 10);

            var secondPass = new MZX.MZM(WorkingImage.CharacterImage, 0);

            Optimizers.LocalizedDensityComparator.Run(ref WorkingImage.CharacterImage);

            var thirdPass = new MZX.MZM(WorkingImage.CharacterImage, 0);

            pictureBox1.Image = WorkingImage.UIImage;

            MessageBox.Show("Pass #1: " + firstPass.NumChars + ", Pass #2: " + secondPass.NumChars + ", Pass #3: " +
                            thirdPass.NumChars);
        }
    }
}
