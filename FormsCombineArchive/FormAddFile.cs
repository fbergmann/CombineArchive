using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LibCombine;

namespace FormsCombineArchive
{
    public partial class FormAddFile : Form
    {
        public FormAddFile()
        {
            InitializeComponent();
        }
        public string FileName
        {
            get
            {
                return txtFileName.Text;
            } 
            set
            {
            	txtFileName.Text = value;
            }
        }

        public string Format
        {
            get
            {
                return txtFormat.Text;
            }

            set
            {
                txtFormat.Text = value;
            }
        }

        public OmexDescription Description
        {
            get
            {
                return new OmexDescription { 
                    Created = DateTime.UtcNow,
                    Creators = new List<VCard> { controlCreator1.VCard }, 
                    Description = textBox1.Text, 
                    About = FileName
                };
            } 
            set
            {
                if (value.Creators != null && value.Creators.Count > 0)
                    controlCreator1.VCard = value.Creators[0];
                textBox1.Text = value.Description;
            }
        }

        private void OnBrowseFile(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog {Filter = "All files|*.*", FileName = FileName})
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    FileName = dialog.FileName;
                    Lookup(Path.GetExtension(dialog.FileName).Replace(".", ""));
                }
            }
        }

        private void Lookup(string format)
        {
            if (Entry.KnownFormats.ContainsKey(format))
                Format = Entry.KnownFormats[format];
        }
        private void cmdLookup_Click(object sender, EventArgs e)
        {
            string format = Format.ToLowerInvariant();
            Lookup(format);
        }
    }
}
