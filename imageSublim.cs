using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
// using System.Windows.Forms.PaintEventHandler;

namespace ImageSublim
{
	public class CommandLineArguments
	{
	   bool? Verbose { get; set; }
	   int? RunId { get; set; }
	}

    /*
     * Create a class which is a Child of Form
     * 
     */
    public partial class imageSublim : Form, IMessageFilter
    {
 		PictureBox pb;
 		
		Image normal;
		Image img;
		string invertImg = "true";
		string imagePath = "";
		float imageOp = 1.0f;
		float imgOp = 0.2f;
		float tOp = 0.2f;
		string noResize = "resize";
		float initTrans = 1.0f;

		float imgOp_prev = 1.0f;
		float tOp_prev = 1.0f;
		float initTrans_prev = 1.0f;
		
		int setWidth = 100;
		int setHeight = 100;
		
		string data = "";
		string init_data = "";
		Font data_font = new Font("Tahoma", 5, FontStyle.Bold);
		
 		int screen_width = SystemInformation.VirtualScreen.Width;
		int screen_height = SystemInformation.VirtualScreen.Height;
	
		[DllImport("user32.dll")]
		private static extern bool SetForegroundWindow(IntPtr hWnd);

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

		[DllImport("user32.dll", SetLastError = true)]
		static extern IntPtr SetFocus(IntPtr hWnd);
		
		[DllImport("kernel32.dll")]
		public static extern Int32 AllocConsole();		

		[DllImport("user32.dll")]
		[return:MarshalAs(UnmanagedType.Bool)]
		public static extern bool InvertRect(IntPtr hDC, ref System.Drawing.Rectangle lprc);


		enum KeyModifier
		{
			None = 0,
			Alt = 1,
			Control = 2,
			Shift = 4,
			WinKey = 8
		}

		public bool PreFilterMessage(ref Message m)
		{
			const int WM_KEYUP = 0x101;
			if (m.Msg == WM_KEYUP)
			{
				return true;
			} else {
				return false;
			}

		}
        
        public imageSublim()
        {
        	Application.AddMessageFilter(this);
        	
        	int id = 0;

			this.FormBorderStyle = FormBorderStyle.None;
			/*
			* Initialize component
			* 
			*/
			InitializeComponent();

			/*
			 * Form background, border and transparency
			 * 
			 */
			this.Bounds = Screen.PrimaryScreen.Bounds;
			this.TopMost = true;
			this.BackColor = Color.White;
			this.TransparencyKey  = Color.Black;

			this.Opacity = this.initTrans;
			RegisterHotKey(this.Handle, id, (int)KeyModifier.Shift, Keys.Space.GetHashCode());
			this.Show();
		}
	
		protected override CreateParams CreateParams
		{
			get
			{
			CreateParams createParams = base.CreateParams;
			createParams.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT

			return createParams;
			}
		}
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
				switch(keyData)
				{
					case Keys.H:
						this.tOp_prev = this.tOp;
						this.imgOp_prev = this.imgOp;
						this.initTrans_prev = this.initTrans;
						this.Opacity = 0.0f;
						this.pb.Visible = false;
						break;
					case Keys.U:
						this.tOp = this.tOp_prev;
						this.imgOp = this.imgOp_prev;
						this.initTrans = this.initTrans_prev;
						this.Opacity = this.initTrans;
						this.pb.Visible = true;
						break;
					case Keys.N:
						this.Opacity = this.initTrans;
						this.img = Image.FromFile(this.imagePath);

						if (this.invertImg == "true")
						{
							this.img = InvertImage(this.img);
						}
						if (this.noResize == "resize")
						{
							this.img = ResizeImage(this.img, this.screen_width, this.screen_height);
						}
						else if (this.noResize == "small")
						{
							this.img = ResizeImage(this.img, 100, 100);
						}
						else if (this.noResize == "custom")
						{
							this.img = ResizeImage(this.img, this.setWidth, this.setHeight);
						}

						Bitmap bm = new Bitmap(this.img);
						Graphics gr = Graphics.FromImage(bm);

						this.img = ChangeOpacity(this.img, this.imgOp);
						
						for (int i = 0; i < 1000; i ++)
						{		
							this.data = this.data + "    " + this.init_data;
						}

						if (this.data.Length >= 31500)
						{
							this.data = this.data.Substring(0, 31000);
						}
						int font_size = (int) this.data_font.Size;
						
						if (this.noResize == "resize")
						{
							this.normal = Convert_Text_to_Image(this.data, font_size, this.screen_width, this.screen_height);
						}
						else if (this.noResize == "small")
						{
							this.normal = Convert_Text_to_Image(this.data, font_size, 100, 100);
						}
						else if (this.noResize == "no")
						{
							this.normal = Convert_Text_to_Image(this.data, font_size, this.img.Width, this.img.Height);
						}
						else if (this.noResize == "custom")
						{
							this.normal = Convert_Text_to_Image(this.data, font_size, this.setWidth, this.setHeight);
						}
						
						this.normal = ChangeOpacity(this.normal, this.tOp);
						gr.DrawImage(this.normal, 0, 0);
						this.normal = (Image) bm;
						if (this.noResize == "resize")
						{
							this.pb.Width = this.screen_width;
							this.pb.Height = this.screen_height;
							this.Size = new Size(this.screen_width, this.screen_height);
						}
						else if (this.noResize == "no")
						{
							this.pb.Width = this.img.Width;
							this.pb.Height = this.img.Height;
							this.Size = this.pb.Size;
							
						}
						else if (this.noResize == "small")
						{
							this.pb.Width = 100;
							this.pb.Height = 100;
							this.Size = new Size(100, 100);
						}
						else if (this.noResize == "custom")
						{
							this.pb.Width = this.setWidth;
							this.pb.Height = this.setHeight;
							this.Size = this.pb.Size;		
						}
						this.pb.Image = this.normal;
						break;
					case Keys.I:
						this.imageOp = 1 + 0.05f;
						this.normal = ChangeOpacity(this.normal, this.imageOp);
						this.pb.Image = this.normal;
						this.pb.Update();
						break;
					case Keys.D:
						this.imageOp = 1 - 0.05f;
						this.normal = ChangeOpacity(this.normal, this.imageOp);
						this.pb.Image = this.normal;
						this.pb.Update();
						break;
					case Keys.S:
						this.pb.Image.Save("test.jpg");
						break;
					case Keys.Q:
						this.Close();
						Application.Exit();
						break;
			}	
			return base.ProcessCmdKey(ref msg, keyData);
		}

		public static Bitmap ResizeImage(Image image, int width, int height)
		{
		    var destRect = new Rectangle(0, 0, width, height);
		    var destImage = new Bitmap(width, height);

		    destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

		    using (var graphics = Graphics.FromImage(destImage))
		    {
			graphics.CompositingMode = CompositingMode.SourceCopy;
			graphics.CompositingQuality = CompositingQuality.HighQuality;
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

			using (var wrapMode = new ImageAttributes())
			{
			    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
			    graphics.DrawImage(image, destRect, 0, 0, image.Width,image.Height, GraphicsUnit.Pixel, wrapMode);
			}
		    }

		    return destImage;
		}

		public static Bitmap Convert_Text_to_Image(string txt, int fsize, int w, int h)
		{
		    Bitmap bmp = new Bitmap(w, h);
				RectangleF rectf = new RectangleF(0, 0, w, h);
				using(Graphics g = Graphics.FromImage(bmp))
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.InterpolationMode = InterpolationMode.HighQualityBicubic;
					g.PixelOffsetMode = PixelOffsetMode.HighQuality;
					StringFormat sf = new StringFormat();
					sf.Alignment = StringAlignment.Center;
					sf.LineAlignment = StringAlignment.Center;
					g.DrawString(txt, new System.Drawing.Font("Tahoma", fsize, FontStyle.Regular), Brushes.Black, rectf, sf);
				}           

		    return bmp;
		 }

		private static Image InvertImage(Image originalImg)
		{
		    	Bitmap invertedBmp = null;

		    	using (Bitmap originalBmp = new Bitmap(originalImg))
		    	{
				invertedBmp = new Bitmap(originalBmp.Width, originalBmp.Height);

				for (int x = 0; x < originalBmp.Width; x++)
				{
			    		for (int y = 0; y < originalBmp.Height; y++)
			    		{
						//Get the color
						Color clr = originalBmp.GetPixel(x, y);

						//Invert the clr
						clr = Color.FromArgb(255 - clr.R, 255 - clr.G, 255 - clr.B);

						//Update the color
						invertedBmp.SetPixel(x, y, clr);
			    		}
				}
		    	}

		    	return (Image)invertedBmp;
		}

		public static Bitmap ChangeOpacity(Image img, float opacityvalue)
		{
	    		Bitmap bmp = new Bitmap(img.Width,img.Height); // Determining Width and Height of Source Image
	    		Graphics graphics = Graphics.FromImage(bmp);
	    		System.Drawing.Imaging.ColorMatrix colormatrix = new System.Drawing.Imaging.ColorMatrix();
	    		colormatrix.Matrix33 = opacityvalue;
	    		System.Drawing.Imaging.ImageAttributes imgAttribute = new System.Drawing.Imaging.ImageAttributes();
	    		imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
		   		graphics.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
	    		graphics.Dispose();   // Releasing all resource used by graphics 
	    		return bmp;
		}

		/*
		 * Unregister HotKey when form closes
		 *
		 */
		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			UnregisterHotKey(this.Handle, 0);       // Unregister hotkey with id 0 before closing the form. You might want to call this more than once with different id values if you are planning to register more than one hotkey.
		}

		/*
		 * This function is run whenever a message is sent to a window
		 *
		 */
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			if (m.Msg == 0x0312)
			{
				/*
				* Note that the three lines below are not needed if you only want to register one hotkey.
				* The below lines are useful in case you want to register multiple keys, which you can use a switch with the id as argument, or if you want to know which key/modifier was pressed for some particular reason
				*
				*/

				Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);                  // The key of the hotkey that was pressed.
				KeyModifier modifier = (KeyModifier)((int)m.LParam & 0xFFFF);       // The modifier of the hotkey that was pressed.
				int id = m.WParam.ToInt32();                                        // The id of the hotkey that was pressed.

				/*
				 * Below sleep is important for the hotkeys to function
				 *
				 */
				SetFocus(this.Handle);
				SetForegroundWindow(this.Handle);
			}
		}

        #region Windows Form Designer generated code
	
		private void InitializeComponent()
		{
			this.AutoSize = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "imageSublim";
			this.Text = "imageSublim";
			this.ResumeLayout(false);
			this.PerformLayout();
			this.pb = new PictureBox();
			this.pb.Size = new Size(400, 400);
			this.Controls.Add(this.pb);
		}	
	
		#endregion

		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			imageSublim i = new imageSublim();
			string[] args = Environment.GetCommandLineArgs();
			
			string fnt = "";
			int fsize = 5;
			
       		for(int j = 0; j < args.Length; j ++)
       		{
       			string arg = args[j];
       			
       			if (arg == "-h")
       			{
       				MessageBox.Show("imageSublim.exe -d message -p path_to_image -top text_opacity -imgop image_opacity -f font_name -fs font_size -r resize/no/small/custom -1 init_image_opacity -i true/false");
				Application.Exit();
       			}
       			if (arg == "-d")
       			{
       				i.data = args[j + 1];
       				i.init_data = i.data;
       			}

       			if (arg == "-p")
       			{
       				i.imagePath = args[j + 1];
       			}
			if (arg == "-top")
			{
				i.tOp = float.Parse(args[j + 1]);
			}
			if (arg == "-imgop")
			{
				i.imgOp = float.Parse(args[j + 1]);
			}
			if (arg == "-f")
			{
				fnt = args[j + 1];
			}
			if (arg == "-fs")
			{
				fsize = Convert.ToInt32(args[j + 1]);
			}
			if (arg == "-r")
			{
				i.noResize = args[j + 1];
				if (i.noResize == "custom")
				{
					string[] lst = args[j + 2].Split('x');
					i.setWidth = Convert.ToInt32(lst[0]);
					i.setHeight = Convert.ToInt32(lst[1]);
					Console.WriteLine(lst[0]);
				}
			}
			if (arg == "-1")
			{
				i.initTrans = float.Parse(args[j + 1]);
			}
			if (arg == "-i")
			{
				i.invertImg = args[j + 1];
			}
       		}
       		
			i.data_font = new Font(fnt, fsize, FontStyle.Bold);
			
			Application.Run(i);
		}
    }
}
